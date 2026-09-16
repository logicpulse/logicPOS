using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class DocumentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DocumentTypeService _documentTypeService;
    private readonly DocumentSeriesService _documentSeriesService;
    private readonly PaymentMethodService _paymentMethodService;

    public DocumentService(
        ApplicationDbContext dbContext,
        DocumentTypeService documentTypeService,
        DocumentSeriesService documentSeriesService,
        PaymentMethodService paymentMethodService)
    {
        _dbContext = dbContext;
        _documentTypeService = documentTypeService;
        _documentSeriesService = documentSeriesService;
        _paymentMethodService = paymentMethodService;
    }

    public async Task<DocumentsPaginatedResponse> GetPagedAsync(
        int page, int pageSize, string[]? types, Guid? customerId, Guid? paymentMethodId, string? paymentStatus, char? status,
        CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 25 : pageSize;

        var query = _dbContext.ApiDocuments.AsNoTracking().Include(d => d.PaymentMethods).AsQueryable();

        if (types is { Length: > 0 })
        {
            query = query.Where(d => types.Contains(d.Type));
        }

        if (customerId.HasValue)
        {
            query = query.Where(d => d.CustomerId == customerId.Value);
        }

        if (paymentMethodId.HasValue)
        {
            query = query.Where(d => d.PaymentMethods.Any(pm => pm.PaymentMethodId == paymentMethodId.Value));
        }

        if (status.HasValue)
        {
            query = status.Value == 'A'
                ? query.Where(d => d.Status == ApiDocumentStatus.Cancelled)
                : query.Where(d => d.Status != ApiDocumentStatus.Cancelled);
        }

        if (paymentStatus == "Paid")
        {
            query = query.Where(d => d.TotalPaid >= d.TotalFinal);
        }
        else if (paymentStatus == "Unpaid")
        {
            query = query.Where(d => d.TotalPaid < d.TotalFinal);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);

        var documents = await query.OrderByDescending(d => d.CreatedUtc).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new DocumentsPaginatedResponse
        {
            Items = documents.Select(MapToViewModel).ToList(),
            ItemsCount = documents.Count,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<DocumentsPaginatedResponse> GetUnsettledForSettlementAsync(int page, int pageSize, Guid? customerId, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 25 : pageSize;

        // Mirrors DocumentViewModel.IsPayable: active (non-draft, non-cancelled), unpaid, and an invoice-family type.
        var invoiceFamily = new[] { "FT", "FS", "FR", "NC", "ND" };
        var query = _dbContext.ApiDocuments.AsNoTracking()
            .Where(d => d.Status == ApiDocumentStatus.Active && d.TotalPaid < d.TotalFinal && invoiceFamily.Contains(d.Type));

        if (customerId.HasValue)
        {
            query = query.Where(d => d.CustomerId == customerId.Value);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
        var documents = await query.OrderBy(d => d.CreatedUtc).Skip((page - 1) * pageSize).Take(pageSize).Include(d => d.PaymentMethods).ToListAsync(cancellationToken);

        return new DocumentsPaginatedResponse
        {
            Items = documents.Select(MapToViewModel).ToList(),
            ItemsCount = documents.Count,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<DocumentFullResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var document = await _dbContext.ApiDocuments.AsNoTracking()
            .Include(d => d.Details)
            .Include(d => d.PaymentMethods)
            .SingleOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (document is null)
        {
            return null;
        }

        return await MapToFullAsync(document, cancellationToken);
    }

    public async Task<bool?> IsFromOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var document = await _dbContext.ApiDocuments.AsNoTracking().SingleOrDefaultAsync(d => d.Id == id, cancellationToken);
        return document is null ? null : document.OrderId.HasValue;
    }

    public async Task<IReadOnlyList<DocumentDetailResponse>?> GetDetailsAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var document = await _dbContext.ApiDocuments.AsNoTracking().Include(d => d.Details).SingleOrDefaultAsync(d => d.Id == documentId, cancellationToken);
        if (document is null)
        {
            return null;
        }

        return document.Details.OrderBy(d => d.Order).Select(MapDetail).ToList();
    }

    public async Task<(bool Success, string? Field, string? Error, IssueDocumentResponse? Response)> IssueAsync(IssueDocumentRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Type))
        {
            return (false, "type", "Type é obrigatório.", null);
        }

        var documentType = await _documentTypeService.GetByAcronymAsync(request.Type, cancellationToken);
        if (documentType is null)
        {
            return (false, "type", $"Tipo de documento '{request.Type}' não existe.", null);
        }

        // Line items come either straight from the request, or pulled live from an existing Order.
        // UseCatalogPrice is only true when the caller genuinely omitted a price (null) — an explicit 0
        // (e.g. a comped item) must never be silently replaced by the article's current catalog price,
        // and an order line's price is always already-decided, never a "use catalog price" placeholder.
        List<(Guid ArticleId, decimal Quantity, decimal UnitPrice, bool UseCatalogPrice, decimal Discount, Guid? VatRateId, string? SerialNumber)> sourceLines;

        if (request.OrderId.HasValue)
        {
            var order = await _dbContext.ApiOrders.Include(o => o.Tickets).ThenInclude(t => t.Details)
                .SingleOrDefaultAsync(o => o.Id == request.OrderId.Value, cancellationToken);

            if (order is null)
            {
                return (false, "orderId", "OrderId não existe.", null);
            }

            sourceLines = order.Tickets.SelectMany(t => t.Details)
                .Select(d => (d.ArticleId, d.Quantity, d.Price, false, d.Discount, (Guid?)null, (string?)null))
                .ToList();
        }
        else
        {
            if (request.Details is null || request.Details.Count == 0)
            {
                return (false, "details", "Pelo menos um item é obrigatório.", null);
            }

            sourceLines = request.Details
                .Select(d => (d.ArticleId, d.Quantity, d.UnitPrice ?? 0m, d.UnitPrice is null, d.Discount ?? 0m, d.VatRateId, d.SerialNumber))
                .ToList();
        }

        if (sourceLines.Count == 0)
        {
            return (false, "details", "O documento não tem itens.", null);
        }

        var document = new ApiDocument
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Type = documentType.Acronym,
            OrderId = request.OrderId,
            ParentId = request.ParentId,
            CustomerName = request.Customer?.Name,
            CustomerFiscalNumber = request.Customer?.FiscalNumber,
            CustomerAddress = request.Customer?.Address,
            CustomerLocality = request.Customer?.Locality,
            CustomerZipCode = request.Customer?.ZipCode,
            CustomerCity = request.Customer?.City,
            CustomerEmail = request.Customer?.Email,
            CustomerPhone = request.Customer?.Phone,
            Discount = request.Discount,
            Notes = request.Notes ?? string.Empty,
            Status = request.IsDraft ? ApiDocumentStatus.Draft : ApiDocumentStatus.Active,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        uint lineOrder = 1;
        foreach (var line in sourceLines)
        {
            var article = await _dbContext.ApiArticles.AsNoTracking().SingleOrDefaultAsync(a => a.Id == line.ArticleId, cancellationToken);
            if (article is null)
            {
                return (false, "articleId", $"ArticleId {line.ArticleId} não existe.", null);
            }

            var vatRateId = line.VatRateId ?? article.VatDirectSellingId;
            var vatRate = await _dbContext.ApiVatRates.AsNoTracking().SingleOrDefaultAsync(v => v.Id == vatRateId, cancellationToken);
            if (line.VatRateId.HasValue && vatRate is null)
            {
                return (false, "vatRateId", $"VatRateId {line.VatRateId} não existe.", null);
            }

            var unitPrice = line.UseCatalogPrice ? article.Price1Value : line.UnitPrice;
            var grossLine = line.Quantity * unitPrice;
            var discountAmount = grossLine * (line.Discount / 100m);
            var netLine = grossLine - discountAmount;
            var vatPercent = vatRate?.Value ?? 0;
            var taxAmount = netLine * (vatPercent / 100m);

            document.Details.Add(new ApiDocumentDetail
            {
                Id = Guid.NewGuid(),
                DocumentId = document.Id,
                Order = lineOrder++,
                ArticleId = article.Id,
                Code = article.Code,
                Designation = article.Designation,
                Quantity = line.Quantity,
                Price = unitPrice,
                Discount = line.Discount,
                VatRateId = vatRate?.Id,
                VatDesignation = vatRate?.Designation,
                VatCode = vatRate?.Code,
                VatPercentage = vatPercent,
                TotalNet = netLine,
                TotalDiscount = discountAmount,
                TotalTax = taxAmount,
                TotalFinal = netLine + taxAmount,
                SerialNumber = line.SerialNumber
            });
        }

        document.TotalNet = document.Details.Sum(d => d.TotalNet);
        document.TotalDiscount = document.Details.Sum(d => d.TotalDiscount);
        document.TotalTax = document.Details.Sum(d => d.TotalTax);
        document.TotalFinal = document.Details.Sum(d => d.TotalFinal);

        // Drafts are not legally numbered: they can still be freely deleted (DeleteDraft), so no series
        // number is ever wasted on a document that might never become real. Only non-draft issues claim one.
        if (!request.IsDraft)
        {
            var series = await _documentSeriesService.GetActiveForTypeAsync(documentType.Id, cancellationToken);
            if (series is null)
            {
                return (false, "type", $"Não existe série ativa para o tipo de documento '{documentType.Acronym}'.", null);
            }

            var (success, error, number) = await _documentSeriesService.IssueNumberAsync(series.Id, cancellationToken);
            if (!success)
            {
                return (false, "type", error, null);
            }

            document.DocumentSeriesId = series.Id;
            document.Number = number!;
        }
        else
        {
            document.Number = string.Empty;
        }

        if (request.PaymentMethods is not null)
        {
            foreach (var payment in request.PaymentMethods)
            {
                if (!await _paymentMethodService.ExistsAsync(payment.PaymentMethodId, cancellationToken))
                {
                    return (false, "paymentMethods", $"PaymentMethodId {payment.PaymentMethodId} não existe.", null);
                }

                document.PaymentMethods.Add(new ApiDocumentPaymentMethod
                {
                    Id = Guid.NewGuid(),
                    DocumentId = document.Id,
                    PaymentMethodId = payment.PaymentMethodId,
                    Amount = payment.Amount
                });
            }

            document.TotalPaid = document.PaymentMethods.Sum(pm => pm.Amount);
        }

        _dbContext.ApiDocuments.Add(document);

        if (!request.IsDraft && request.OrderId.HasValue)
        {
            var order = await _dbContext.ApiOrders.SingleOrDefaultAsync(o => o.Id == request.OrderId.Value, cancellationToken);
            if (order is not null && order.Status == ApiOrderStatus.Open)
            {
                order.Status = ApiOrderStatus.Closed;
                order.UpdatedUtc = DateTime.UtcNow;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, null, new IssueDocumentResponse { Id = document.Id, AtDocCodeId = null });
    }

    public async Task<(bool Success, string? Error)> CancelAsync(Guid id, string? reason, CancellationToken cancellationToken = default)
    {
        var document = await _dbContext.ApiDocuments.SingleOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (document is null)
        {
            return (false, "Document não encontrado.");
        }

        if (document.Status != ApiDocumentStatus.Active)
        {
            return (false, "Apenas documentos ativos podem ser anulados.");
        }

        // Mirrors DocumentViewModel.HasValidTimeToCancel in the client: only cancellable within the
        // calendar month it was issued.
        if (document.CreatedUtc.Month != DateTime.UtcNow.Month || document.CreatedUtc.Year != DateTime.UtcNow.Year)
        {
            return (false, "Documento só pode ser anulado no mês de emissão.");
        }

        document.Status = ApiDocumentStatus.Cancelled;
        document.CancelReason = reason;
        document.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<bool> DeleteDraftAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var document = await _dbContext.ApiDocuments.SingleOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (document is null || document.Status != ApiDocumentStatus.Draft)
        {
            return false;
        }

        _dbContext.ApiDocuments.Remove(document);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Issues a Receipt ("RC") document for the payment and applies it against the given documents' balances,
    /// oldest first, until the amount is exhausted. Returns the new receipt's id.
    /// </summary>
    public async Task<(bool Success, string? Field, string? Error, AddEntityIdResponse? Response)> PayAsync(PayDocumentsRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _paymentMethodService.ExistsAsync(request.PaymentMethodId, cancellationToken))
        {
            return (false, "paymentMethodId", "PaymentMethodId não existe.", null);
        }

        if (request.Documents.Count == 0)
        {
            return (false, "documents", "Pelo menos um documento é obrigatório.", null);
        }

        var documents = await _dbContext.ApiDocuments.Where(d => request.Documents.Contains(d.Id)).OrderBy(d => d.CreatedUtc).ToListAsync(cancellationToken);
        if (documents.Count != request.Documents.Count)
        {
            return (false, "documents", "Um ou mais documentos não existem.", null);
        }

        var receiptType = await _documentTypeService.GetByAcronymAsync("RC", cancellationToken);
        if (receiptType is null)
        {
            return (false, "documents", "Tipo de documento 'RC' (Recibo) não está configurado.", null);
        }

        var series = await _documentSeriesService.GetActiveForTypeAsync(receiptType.Id, cancellationToken);
        if (series is null)
        {
            return (false, "documents", "Não existe série ativa para Recibos (RC).", null);
        }

        var remaining = request.Amount;
        var settledNumbers = new List<string>();

        foreach (var document in documents)
        {
            if (remaining <= 0)
            {
                break;
            }

            var outstanding = document.TotalFinal - document.TotalPaid;
            if (outstanding <= 0)
            {
                continue;
            }

            var applied = Math.Min(outstanding, remaining);
            document.TotalPaid += applied;
            document.UpdatedUtc = DateTime.UtcNow;
            remaining -= applied;
            settledNumbers.Add(document.Number);
        }

        var (numberSuccess, numberError, number) = await _documentSeriesService.IssueNumberAsync(series.Id, cancellationToken);
        if (!numberSuccess)
        {
            return (false, "documents", numberError, null);
        }

        var receipt = new ApiDocument
        {
            Id = Guid.NewGuid(),
            Type = receiptType.Acronym,
            DocumentSeriesId = series.Id,
            Number = number!,
            CustomerId = documents[0].CustomerId,
            CustomerName = documents[0].CustomerName,
            CustomerFiscalNumber = documents[0].CustomerFiscalNumber,
            TotalNet = request.Amount,
            TotalFinal = request.Amount,
            TotalPaid = request.Amount,
            Status = ApiDocumentStatus.Active,
            RelatedDocumentNumbers = string.Join(",", settledNumbers),
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        receipt.PaymentMethods.Add(new ApiDocumentPaymentMethod
        {
            Id = Guid.NewGuid(),
            DocumentId = receipt.Id,
            PaymentMethodId = request.PaymentMethodId,
            Amount = request.Amount
        });

        _dbContext.ApiDocuments.Add(receipt);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (true, null, null, new AddEntityIdResponse { Id = receipt.Id });
    }

    internal IReadOnlyList<ReceiptViewModelResponse> MapReceipts(List<ApiDocument> receipts)
    {
        return receipts.Select(r => new ReceiptViewModelResponse
        {
            Id = r.Id,
            Notes = r.Notes,
            CreatedAt = r.CreatedUtc,
            UpdatedAt = r.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = r.Status == ApiDocumentStatus.Cancelled,
            RefNo = r.Number,
            Status = r.Status == ApiDocumentStatus.Cancelled ? "A" : "N",
            Amount = r.TotalFinal,
            Customer = r.CustomerName,
            CustomerFiscalNumber = r.CustomerFiscalNumber,
            RelatedDocuments = string.IsNullOrWhiteSpace(r.RelatedDocumentNumbers) ? [] : r.RelatedDocumentNumbers.Split(',').ToList(),
            Agt = new AgtDocumentInfoResponse()
        }).ToList();
    }

    private static DocumentViewModelResponse MapToViewModel(ApiDocument document)
    {
        return new DocumentViewModelResponse
        {
            Id = document.Id,
            Notes = document.Notes,
            CreatedAt = document.CreatedUtc,
            UpdatedAt = document.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = document.Status == ApiDocumentStatus.Cancelled,
            Number = document.Number,
            Type = document.Type,
            Status = document.Status == ApiDocumentStatus.Cancelled ? "A" : "N",
            IsDraft = document.Status == ApiDocumentStatus.Draft,
            TotalFinal = document.TotalFinal,
            TotalPaid = document.TotalPaid,
            TotalToPay = document.TotalFinal - document.TotalPaid,
            AtDocCodeId = null,
            AtResendDocument = false,
            RelatedDocuments = string.IsNullOrWhiteSpace(document.RelatedDocumentNumbers) ? [] : document.RelatedDocumentNumbers.Split(',').ToList(),
            Paid = document.TotalPaid >= document.TotalFinal
        };
    }

    private async Task<DocumentFullResponse> MapToFullAsync(ApiDocument document, CancellationToken cancellationToken)
    {
        var paymentMethodIds = document.PaymentMethods.Select(pm => pm.PaymentMethodId).Distinct().ToList();
        var paymentMethods = new Dictionary<Guid, PaymentMethodResponse>();
        foreach (var id in paymentMethodIds)
        {
            var pm = await _paymentMethodService.GetByIdInternalAsync(id, cancellationToken);
            if (pm is not null)
            {
                paymentMethods[id] = pm;
            }
        }

        var details = document.Details.OrderBy(d => d.Order).Select(MapDetail).ToList();

        return new DocumentFullResponse
        {
            Id = document.Id,
            Notes = document.Notes,
            CreatedAt = document.CreatedUtc,
            UpdatedAt = document.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = document.Status == ApiDocumentStatus.Cancelled,
            CustomerId = document.CustomerId ?? Guid.Empty,
            Type = document.Type,
            Number = document.Number,
            Discount = document.Discount,
            TotalNet = document.TotalNet,
            TotalDiscount = document.TotalDiscount,
            TotalTax = document.TotalTax,
            TotalFinal = document.TotalFinal,
            Customer = new DocumentCustomerResponse
            {
                Name = document.CustomerName,
                Address = document.CustomerAddress,
                Locality = document.CustomerLocality,
                ZipCode = document.CustomerZipCode,
                City = document.CustomerCity,
                FiscalNumber = document.CustomerFiscalNumber,
                Email = document.CustomerEmail,
                Phone = document.CustomerPhone
            },
            PaymentMethods = document.PaymentMethods.Select(pm => new DocumentPaymentMethodResponse
            {
                Id = pm.Id,
                PaymentMethodId = pm.PaymentMethodId,
                PaymentMethod = paymentMethods.GetValueOrDefault(pm.PaymentMethodId),
                Amount = pm.Amount
            }).ToList(),
            Details = details,
            TaxResumes = details
                .GroupBy(d => d.Tax?.Percentage ?? 0)
                .Select(group => new TaxResumeResponse
                {
                    Code = group.First().Tax?.Code,
                    Designation = group.First().Tax?.Designation,
                    Rate = group.Key,
                    Base = group.Sum(d => d.TotalNet),
                    Total = group.Sum(d => d.TotalTax)
                }).ToList()
        };
    }

    private static DocumentDetailResponse MapDetail(ApiDocumentDetail detail)
    {
        return new DocumentDetailResponse
        {
            Id = detail.Id,
            Notes = string.Empty,
            Order = detail.Order,
            ArticleId = detail.ArticleId,
            Quantity = detail.Quantity,
            Unit = detail.Unit,
            Price = detail.Price,
            Tax = detail.VatRateId is null
                ? null
                : new DocumentDetailTaxResponse
                {
                    TaxId = detail.VatRateId.Value,
                    Designation = detail.VatDesignation,
                    Code = detail.VatCode,
                    Percentage = detail.VatPercentage
                },
            VatExemptionReason = detail.VatExemptionReason,
            Discount = detail.Discount,
            TotalNet = detail.TotalNet,
            TotalGross = detail.Quantity * detail.Price,
            TotalDiscount = detail.TotalDiscount,
            TotalTax = detail.TotalTax,
            TotalFinal = detail.TotalFinal,
            Code = detail.Code,
            Designation = detail.Designation,
            SerialNumber = detail.SerialNumber
        };
    }
}
