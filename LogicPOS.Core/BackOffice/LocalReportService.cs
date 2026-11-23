using LogicPOS.Application.Features.Finance.Customers;
using LogicPOS.Application.Features.Finance.Documents;
using LogicPOS.Application.Features.Reports;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

public sealed class LocalReportService : IReportService
{
    /// <summary>
    /// Stable token when a stock filter report has no rows.
    /// The Avalonia UI shows the existing alert dialog instead of writing a PDF.
    /// </summary>
    public const string NoRecordsError = "report-filter-no-records";

    private const int MaxRows = 2000;

    private readonly IServiceScopeFactory _scopes;

    public LocalReportService(IServiceScopeFactory scopes)
    {
        _scopes = scopes;
    }

    public IReadOnlyList<ReportDefinition> Catalog => ReportCatalog.All;

    public async Task<ListingSaveResult> GenerateAsync(
        string key,
        ReportFilterRequest filters,
        CancellationToken cancellationToken = default)
    {
        var report = ReportCatalog.All.FirstOrDefault(item => item.Key == key);
        if (report is null)
        {
            return ListingSaveResult.Fail("Relatório desconhecido.");
        }

        var start = filters.Start.Date;
        var end = filters.End.Date;
        if (report.ShowDates && end < start)
        {
            return ListingSaveResult.Fail("A data final tem de ser igual ou posterior à data inicial.");
        }

        if (report.NeedsCustomer && (filters.CustomerId is null || filters.CustomerId == Guid.Empty))
        {
            return ListingSaveResult.Fail(report.CustomerIsSupplier ? "Escolha um fornecedor." : "Escolha um cliente.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        byte[] bytes;
        try
        {
            var branded = scope.ServiceProvider.GetService<ILocalQuestPdfReportService>();
            byte[]? brandedBytes = null;
            if (branded is not null)
            {
                brandedBytes = await branded.TryGenerateAsync(
                    new LocalReportPdfRequest
                    {
                        Key = report.Key,
                        Start = start,
                        End = end,
                        DocumentTypeAcronym = filters.DocumentTypeAcronym,
                        TerminalId = filters.TerminalId,
                        CustomerId = filters.CustomerId,
                        ArticleId = filters.ArticleId,
                        WarehouseId = filters.WarehouseId,
                        VatRateId = filters.VatRateId,
                        FamilyId = filters.FamilyId,
                        SubfamilyId = filters.SubfamilyId,
                        SerialNumber = string.IsNullOrWhiteSpace(filters.SerialNumber)
                            ? null
                            : filters.SerialNumber.Trim(),
                        DocumentNumber = string.IsNullOrWhiteSpace(filters.DocumentNumber)
                            ? null
                            : filters.DocumentNumber.Trim()
                    },
                    cancellationToken);
            }

            if (report.Key is "stock-article" or "stock-supplier" or "stock-gain" && brandedBytes is null)
            {
                return ListingSaveResult.Fail(NoRecordsError);
            }

            if (brandedBytes is not null)
            {
                bytes = brandedBytes;
            }
            else
            {
                bytes = await BuildTablePdfAsync(scope.ServiceProvider, report, filters, cancellationToken);
            }
        }
        catch (Exception exception)
        {
            return ListingSaveResult.Fail(exception.Message);
        }

        var path = Path.Combine(Path.GetTempPath(), $"logicpos-report-{report.Key}-{DateTime.Now:yyyyMMddHHmmss}.pdf");
        await File.WriteAllBytesAsync(path, bytes, cancellationToken);
        return ListingSaveResult.Ok(Guid.Empty, path);
    }

    private static async Task<byte[]> BuildTablePdfAsync(
        IServiceProvider services,
        ReportDefinition report,
        ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var database = services.GetRequiredService<LogicPOSDbContext>();
        var balances = services.GetRequiredService<ICustomerOutstandingBalanceService>();
        var rows = await ReadRowsAsync(database, balances, report, filters, cancellationToken);
        if (rows.Count == 0)
        {
            rows.Add(Row(("Informação", "Sem dados no intervalo indicado.")));
        }

        var headers = rows[0].Keys.ToList();
        var table = rows.Select(row => (IReadOnlyList<string>)headers.Select(header => row[header]).ToList()).ToList();
        return ListingTableExport.ToPdf(report.Name, headers, table);
    }

    private static async Task<List<Dictionary<string, string>>> ReadRowsAsync(
        LogicPOSDbContext database,
        ICustomerOutstandingBalanceService balances,
        ReportDefinition report,
        ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var start = filters.Start.Date;
        var until = filters.End.Date.AddDays(1);

        return report.Key switch
        {
            "articles" => await ArticlesAsync(database, cancellationToken),
            "customers" => await PartiesAsync(database, suppliers: false, cancellationToken),
            "suppliers" => await PartiesAsync(database, suppliers: true, cancellationToken),
            "stock" or "stock-article" => await StockOnHandAsync(database, filters, cancellationToken),
            "stock-gain" => await StockGainAsync(database, filters, cancellationToken),
            "stock-supplier" => await StockBySupplierAsync(database, filters, cancellationToken),
            "stock-movement" => await StockMovementsAsync(database, filters, start, until, cancellationToken),
            "audits" => await AuditsAsync(database, filters, start, until, cancellationToken),
            "commissions" => await CommissionsAsync(database, filters, start, until, cancellationToken),
            "deleted-orders" => await DeletedOrdersAsync(database, start, until, cancellationToken),
            "customer-balance-summary" => await CustomerBalanceSummaryAsync(database, balances, filters, start, until, cancellationToken),
            "customer-balance-details" => await CustomerBalanceDetailsAsync(database, filters, start, until, cancellationToken),
            "company-billing" => await CompanyBillingAsync(database, filters, start, until, cancellationToken),
            "sales-payment-method" or "sales-payment-method-detail" => await PaymentMethodsAsync(database, report, filters, start, until, cancellationToken),
            "sales-vat-type" or "sales-vat-class" or "sales-vat-group-detail"
                or "articles-sold" or "sales-family-detail" or "sales-subfamily-detail"
                => await SalesLinesAsync(database, report, filters, start, until, cancellationToken),
            "sales-place-detail" or "sales-table-detail" => await SalesByPlaceAsync(database, report, filters, start, until, cancellationToken),
            _ => await SalesAsync(database, report, filters, start, until, cancellationToken)
        };
    }

    private static async Task<List<Dictionary<string, string>>> ArticlesAsync(
        LogicPOSDbContext database,
        CancellationToken cancellationToken)
    {
        var articles = await database.Articles.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .OrderBy(item => item.Designation)
            .Select(item => new
            {
                item.Code,
                item.Designation,
                item.Barcode,
                Family = item.Subfamily!.Family!.Designation,
                Subfamily = item.Subfamily.Designation
            })
            .Take(MaxRows)
            .ToListAsync(cancellationToken);
        return articles.Select(item => Row(
            ("Código", item.Code),
            ("Designação", item.Designation),
            ("Família", item.Family),
            ("Subfamília", item.Subfamily),
            ("Código de barras", item.Barcode ?? string.Empty))).ToList();
    }

    private static async Task<List<Dictionary<string, string>>> PartiesAsync(
        LogicPOSDbContext database,
        bool suppliers,
        CancellationToken cancellationToken)
    {
        var customers = await database.Customers.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Supplier == suppliers)
            .OrderBy(item => item.Name)
            .Select(item => new
            {
                item.Name,
                item.FiscalNumber,
                item.City,
                Country = item.Country != null ? item.Country.Designation : string.Empty
            })
            .Take(MaxRows)
            .ToListAsync(cancellationToken);
        return customers.Select(item => Row(
            ("Nome", item.Name),
            ("NIF", item.FiscalNumber ?? string.Empty),
            ("Cidade", item.City ?? string.Empty),
            ("País", item.Country ?? string.Empty))).ToList();
    }

    private static async Task<List<Dictionary<string, string>>> StockOnHandAsync(
        LogicPOSDbContext database,
        ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var stock = database.WarehouseArticles.AsNoTracking().Where(item => item.IsDeleted == false);
        if (filters.ArticleId is Guid article && article != Guid.Empty)
        {
            stock = stock.Where(item => item.ArticleId == article);
        }

        if (filters.WarehouseId is Guid warehouse && warehouse != Guid.Empty)
        {
            stock = stock.Where(item => item.WarehouseLocation != null && item.WarehouseLocation.WarehouseId == warehouse);
        }

        if (string.IsNullOrWhiteSpace(filters.SerialNumber) == false)
        {
            var serial = filters.SerialNumber.Trim();
            stock = stock.Where(item => item.SerialNumber == serial);
        }

        var lines = await stock
            .OrderBy(item => item.Article!.Designation)
            .Select(item => new
            {
                Warehouse = item.WarehouseLocation!.Warehouse!.Designation,
                Location = item.WarehouseLocation.Designation,
                item.Article!.Code,
                item.Article.Designation,
                item.Quantity,
                item.SerialNumber
            })
            .Take(MaxRows)
            .ToListAsync(cancellationToken);
        return lines.Select(item => Row(
            ("Armazém", item.Warehouse ?? string.Empty),
            ("Localização", item.Location ?? string.Empty),
            ("Código", item.Code ?? string.Empty),
            ("Designação", item.Designation ?? string.Empty),
            ("Quantidade", Qty(item.Quantity)),
            ("Série", item.SerialNumber ?? string.Empty))).ToList();
    }

    private static async Task<List<Dictionary<string, string>>> StockGainAsync(
        LogicPOSDbContext database,
        ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var articles = database.WarehouseArticles.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Quantity > 0 && item.SerialNumber != null);
        if (filters.ArticleId is Guid article && article != Guid.Empty)
        {
            articles = articles.Where(item => item.ArticleId == article);
        }

        var lines = await articles
            .OrderBy(item => item.Article!.Designation)
            .Select(item => new
            {
                item.SerialNumber,
                item.Article!.Code,
                item.Article.Designation,
                item.Quantity,
                Price = item.Article.Price1.Value
            })
            .Take(MaxRows)
            .ToListAsync(cancellationToken);
        if (lines.Count == 0)
        {
            return [];
        }

        var serials = lines.Select(item => item.SerialNumber).ToArray();
        var movements = database.StockMovements.AsNoTracking()
            .Where(item => item.Quantity > 0 && serials.Contains(item.SerialNumber));
        if (filters.CustomerId is Guid supplier && supplier != Guid.Empty)
        {
            movements = movements.Where(item => item.CustomerId == supplier);
        }

        var purchases = await movements
            .Select(item => new
            {
                item.SerialNumber,
                item.Price,
                item.Date,
                item.DocumentNumber,
                Customer = item.Customer != null ? item.Customer.Name : string.Empty
            })
            .ToListAsync(cancellationToken);
        if (filters.CustomerId is Guid selected && selected != Guid.Empty)
        {
            var matched = purchases.Select(item => item.SerialNumber).ToHashSet();
            lines = lines.Where(item => matched.Contains(item.SerialNumber)).ToList();
        }

        return lines.Select(item =>
        {
            var purchase = purchases.FirstOrDefault(movement => movement.SerialNumber == item.SerialNumber);
            var cost = purchase?.Price ?? 0m;
            return Row(
                ("Código", item.Code ?? string.Empty),
                ("Designação", item.Designation ?? string.Empty),
                ("Série", item.SerialNumber ?? string.Empty),
                ("Quantidade", Qty(item.Quantity)),
                ("Preço", Money(item.Price)),
                ("Compra", Money(cost)),
                ("Ganho", Money((item.Price - cost) * item.Quantity)),
                ("Fornecedor", purchase?.Customer ?? string.Empty),
                ("Documento", purchase?.DocumentNumber ?? string.Empty),
                ("Data", purchase is null ? string.Empty : DateText(purchase.Date)));
        }).ToList();
    }

    private static async Task<List<Dictionary<string, string>>> StockBySupplierAsync(
        LogicPOSDbContext database,
        ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var movements = database.StockMovements.AsNoTracking().Where(item => item.IsDeleted == false);
        if (filters.CustomerId is Guid supplier && supplier != Guid.Empty)
        {
            movements = movements.Where(item => item.CustomerId == supplier);
        }

        if (string.IsNullOrWhiteSpace(filters.DocumentNumber) == false)
        {
            var number = filters.DocumentNumber.Trim();
            movements = movements.Where(item => item.DocumentNumber == number);
        }

        var lines = await movements
            .OrderBy(item => item.Date)
            .Select(item => new
            {
                Customer = item.Customer != null ? item.Customer.Name : string.Empty,
                item.DocumentNumber,
                item.Date,
                item.Article!.Code,
                item.Article.Designation,
                item.Quantity,
                item.Price
            })
            .Take(MaxRows)
            .ToListAsync(cancellationToken);
        return lines.Select(item => Row(
            ("Fornecedor", item.Customer),
            ("Documento", item.DocumentNumber ?? string.Empty),
            ("Data", DateText(item.Date)),
            ("Código", item.Code ?? string.Empty),
            ("Designação", item.Designation ?? string.Empty),
            ("Quantidade", Qty(item.Quantity)),
            ("Preço", Money(item.Price)))).ToList();
    }

    private static async Task<List<Dictionary<string, string>>> StockMovementsAsync(
        LogicPOSDbContext database,
        ReportFilterRequest filters,
        DateTime start,
        DateTime until,
        CancellationToken cancellationToken)
    {
        var movements = database.StockMovements.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Date >= start && item.Date < until);
        if (filters.ArticleId is Guid article && article != Guid.Empty)
        {
            movements = movements.Where(item => item.ArticleId == article);
        }

        if (filters.CustomerId is Guid customer && customer != Guid.Empty)
        {
            movements = movements.Where(item => item.CustomerId == customer);
        }

        var lines = await movements
            .OrderBy(item => item.Date)
            .Select(item => new
            {
                item.Date,
                item.Article!.Code,
                item.Article.Designation,
                item.Quantity,
                item.Price,
                item.DocumentNumber,
                Customer = item.Customer != null ? item.Customer.Name : string.Empty
            })
            .Take(MaxRows)
            .ToListAsync(cancellationToken);
        return lines.Select(item => Row(
            ("Data", DateText(item.Date)),
            ("Código", item.Code ?? string.Empty),
            ("Designação", item.Designation ?? string.Empty),
            ("Quantidade", Qty(item.Quantity)),
            ("Preço", Money(item.Price)),
            ("Documento", item.DocumentNumber ?? string.Empty),
            ("Entidade", item.Customer))).ToList();
    }

    private static async Task<List<Dictionary<string, string>>> AuditsAsync(
        LogicPOSDbContext database,
        ReportFilterRequest filters,
        DateTime start,
        DateTime until,
        CancellationToken cancellationToken)
    {
        var audits = database.SystemAudits.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Date >= start && item.Date < until);
        if (filters.TerminalId is Guid terminal && terminal != Guid.Empty)
        {
            audits = audits.Where(item => item.CreatedWhere == terminal);
        }

        var lines = await audits
            .OrderBy(item => item.Date)
            .Select(item => new { item.Date, Type = item.Type!.Designation, item.Description })
            .Take(MaxRows)
            .ToListAsync(cancellationToken);
        return lines.Select(item => Row(
            ("Data", item.Date.ToString("dd/MM/yyyy HH:mm")),
            ("Tipo", item.Type ?? string.Empty),
            ("Descrição", item.Description ?? string.Empty))).ToList();
    }

    private static async Task<List<Dictionary<string, string>>> CommissionsAsync(
        LogicPOSDbContext database,
        ReportFilterRequest filters,
        DateTime start,
        DateTime until,
        CancellationToken cancellationToken)
    {
        var query = database.Commissions.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .Where(item => item.DocumentDetail!.Document!.IsDeleted == false
                && item.DocumentDetail.Document.IsDraft == false
                && item.DocumentDetail.Document.Status != "A")
            .Where(item => item.DocumentDetail!.Document!.CreatedAt >= start && item.DocumentDetail.Document.CreatedAt < until);
        if (string.IsNullOrWhiteSpace(filters.DocumentTypeAcronym) == false)
        {
            var type = filters.DocumentTypeAcronym.Trim();
            query = query.Where(item => item.DocumentDetail!.Document!.Type == type);
        }

        if (filters.TerminalId is Guid terminal && terminal != Guid.Empty)
        {
            query = query.Where(item => item.DocumentDetail!.Document!.CreatedWhere == terminal);
        }

        var lines = await query
            .OrderBy(item => item.DocumentDetail!.Document!.CreatedAt)
            .Select(item => new
            {
                item.DocumentDetail!.Document!.CreatedAt,
                User = item.User!.Name,
                item.DocumentDetail.Document.Number,
                item.CommissionValue,
                item.Total
            })
            .Take(MaxRows)
            .ToListAsync(cancellationToken);
        return lines.Select(item => Row(
            ("Data", DateText(item.CreatedAt)),
            ("Utilizador", item.User ?? string.Empty),
            ("Documento", item.Number ?? string.Empty),
            ("Comissão", Money(item.CommissionValue)),
            ("Total", Money(item.Total)))).ToList();
    }

    private static async Task<List<Dictionary<string, string>>> DeletedOrdersAsync(
        LogicPOSDbContext database,
        DateTime start,
        DateTime until,
        CancellationToken cancellationToken)
    {
        var lines = await database.OrderDetails.AsNoTracking()
            .Where(item => item.Ticket!.Order!.IsDeleted
                && item.Ticket.Order.DeletedAt >= start
                && item.Ticket.Order.DeletedAt < until)
            .OrderBy(item => item.Ticket!.Order!.DeletedAt)
            .Select(item => new
            {
                item.Ticket!.Order!.DeletedAt,
                Table = item.Ticket.Order.Table!.Designation,
                item.Designation,
                item.Quantity,
                item.TotalFinal
            })
            .Take(MaxRows)
            .ToListAsync(cancellationToken);
        return lines.Select(item => Row(
            ("Data", item.DeletedAt.ToString("dd/MM/yyyy HH:mm")),
            ("Mesa", item.Table ?? string.Empty),
            ("Artigo", item.Designation ?? string.Empty),
            ("Quantidade", Qty(item.Quantity)),
            ("Total", Money(item.TotalFinal)))).ToList();
    }

    private static async Task<List<Dictionary<string, string>>> CompanyBillingAsync(
        LogicPOSDbContext database,
        ReportFilterRequest filters,
        DateTime start,
        DateTime until,
        CancellationToken cancellationToken)
    {
        var sales = await LoadSalesAsync(database, filters, start, until, customerId: null, cancellationToken);
        var rows = sales.Select(item => Row(
            ("Data", DateText(item.CreatedAt)),
            ("Número", item.Number),
            ("Cliente", item.Customer),
            ("NIF", item.FiscalNumber),
            ("Líquido", Money(item.SignedNet)),
            ("IVA", Money(item.SignedTax)),
            ("Total", Money(item.SignedTotal)))).ToList();
        if (rows.Count == 0)
        {
            return rows;
        }

        rows.Add(Row(
            ("Data", string.Empty),
            ("Número", string.Empty),
            ("Cliente", "Total"),
            ("NIF", string.Empty),
            ("Líquido", Money(sales.Sum(item => item.SignedNet))),
            ("IVA", Money(sales.Sum(item => item.SignedTax))),
            ("Total", Money(sales.Sum(item => item.SignedTotal)))));
        return rows;
    }

    private static async Task<List<Dictionary<string, string>>> CustomerBalanceSummaryAsync(
        LogicPOSDbContext database,
        ICustomerOutstandingBalanceService balances,
        ReportFilterRequest filters,
        DateTime start,
        DateTime until,
        CancellationToken cancellationToken)
    {
        var documents = SalesQuery(database, filters, start, until, filters.CustomerId)
            .Where(item => item.Type == "NC" || item.Type == "FT" || item.Type == "ND");
        var documentRows = await documents
            .Select(item => new { item.CustomerId, item.Type, item.TotalFinal })
            .ToListAsync(cancellationToken);
        var documentByCustomer = documentRows
            .GroupBy(item => item.CustomerId)
            .ToDictionary(
                group => group.Key,
                group => (
                    Debit: group.Where(item => item.Type is "FT" or "ND").Sum(item => item.TotalFinal),
                    Credit: group.Where(item => item.Type == "NC").Sum(item => item.TotalFinal)));

        var payments = database.Payments.AsNoTracking()
            .Where(item => item.Receipt!.Status != "A" && item.Document != null)
            .Where(item => item.Receipt!.CreatedAt >= start && item.Receipt.CreatedAt < until);
        if (filters.CustomerId is Guid customer && customer != Guid.Empty)
        {
            payments = payments.Where(item => item.Document!.CustomerId == customer);
        }

        var paymentRows = await payments
            .Select(item => new { item.Document!.CustomerId, item.CreditAmount, item.DebitAmount })
            .ToListAsync(cancellationToken);
        var paymentByCustomer = paymentRows
            .GroupBy(item => item.CustomerId)
            .ToDictionary(
                group => group.Key,
                group => (Credit: group.Sum(item => item.CreditAmount), Debit: group.Sum(item => item.DebitAmount)));

        var ids = documentByCustomer.Keys.Union(paymentByCustomer.Keys).ToList();
        if (filters.CustomerId is Guid selected && selected != Guid.Empty && ids.Contains(selected) == false)
        {
            ids.Add(selected);
        }

        if (ids.Count == 0)
        {
            return [];
        }

        var customers = await database.Customers.AsNoTracking()
            .Where(item => ids.Contains(item.Id))
            .OrderBy(item => item.Name)
            .Select(item => new { item.Id, item.Name, item.FiscalNumber })
            .ToListAsync(cancellationToken);
        var lifetime = await balances.GetLifetimeBalancesAsync(customers.Select(item => item.Id), cancellationToken);
        return customers.Select(item =>
        {
            documentByCustomer.TryGetValue(item.Id, out var document);
            paymentByCustomer.TryGetValue(item.Id, out var payment);
            var debit = document.Debit + payment.Debit;
            var credit = document.Credit + payment.Credit;
            return Row(
                ("Cliente", item.Name),
                ("NIF", item.FiscalNumber ?? string.Empty),
                ("Débito", Money(debit)),
                ("Crédito", Money(credit)),
                ("Saldo do período", Money(credit - debit)),
                ("Saldo", Money(lifetime.GetValueOrDefault(item.Id))));
        }).ToList();
    }

    private static async Task<List<Dictionary<string, string>>> CustomerBalanceDetailsAsync(
        LogicPOSDbContext database,
        ReportFilterRequest filters,
        DateTime start,
        DateTime until,
        CancellationToken cancellationToken)
    {
        var customerId = filters.CustomerId!.Value;
        var documentRows = await SalesQuery(database, filters, start, until, customerId)
            .Where(item => item.Type == "NC" || item.Type == "FT" || item.Type == "ND" || item.Type == "FR" || item.Type == "FS")
            .OrderBy(item => item.CreatedAt)
            .Select(item => new
            {
                item.CreatedAt,
                item.Type,
                item.Number,
                Designation = item.Series!.DocumentType!.Designation,
                item.TotalFinal
            })
            .Take(MaxRows)
            .ToListAsync(cancellationToken);
        var documents = documentRows.Select(item => new
        {
            item.CreatedAt,
            item.Designation,
            item.Number,
            Debit = item.Type == "NC" ? 0m : item.TotalFinal,
            Credit = item.Type is "NC" or "FR" or "FS" ? item.TotalFinal : 0m
        }).ToList();
        var receipts = await database.Payments.AsNoTracking()
            .Where(item => item.Receipt!.Status != "A" && item.Document!.CustomerId == customerId)
            .Where(item => item.Receipt!.CreatedAt >= start && item.Receipt.CreatedAt < until)
            .Select(item => new
            {
                item.Receipt!.CreatedAt,
                Type = "RC",
                Number = item.Receipt.RefNo,
                Designation = "Recibo",
                Debit = item.DebitAmount,
                Credit = item.CreditAmount
            })
            .Take(MaxRows)
            .ToListAsync(cancellationToken);

        var rows = documents.Select(item => (item.CreatedAt, item.Designation, item.Number, item.Debit, item.Credit))
            .Concat(receipts.Select(item => (item.CreatedAt, item.Designation, item.Number, item.Debit, item.Credit)))
            .OrderBy(item => item.CreatedAt)
            .Select(item => Row(
                ("Data", DateText(item.CreatedAt)),
                ("Documento", item.Designation ?? string.Empty),
                ("Número", item.Number ?? string.Empty),
                ("Débito", Money(item.Debit)),
                ("Crédito", Money(item.Credit))))
            .ToList();
        if (rows.Count == 0)
        {
            return rows;
        }

        var debit = documents.Sum(item => item.Debit) + receipts.Sum(item => item.Debit);
        var credit = documents.Sum(item => item.Credit) + receipts.Sum(item => item.Credit);
        rows.Add(Row(
            ("Data", string.Empty),
            ("Documento", "Total"),
            ("Número", string.Empty),
            ("Débito", Money(debit)),
            ("Crédito", Money(credit))));
        return rows;
    }

    private static async Task<List<Dictionary<string, string>>> PaymentMethodsAsync(
        LogicPOSDbContext database,
        ReportDefinition report,
        ReportFilterRequest filters,
        DateTime start,
        DateTime until,
        CancellationToken cancellationToken)
    {
        var query = database.DocumentPaymentMethods.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Document != null && item.Document.IsDeleted == false)
            .Where(item => item.Document!.IsDraft == false && item.Document.Status != "A")
            .Where(item => item.Document!.Series!.DocumentType!.SaftDocumentType == SaftDocumentType.SalesInvoices)
            .Where(item => item.Document!.CreatedAt >= start && item.Document.CreatedAt < until);
        if (string.IsNullOrWhiteSpace(filters.DocumentTypeAcronym) == false)
        {
            var type = filters.DocumentTypeAcronym.Trim();
            query = query.Where(item => item.Document!.Type == type);
        }

        if (filters.TerminalId is Guid terminal && terminal != Guid.Empty)
        {
            query = query.Where(item => item.Document!.CreatedWhere == terminal);
        }

        var lines = await query
            .OrderBy(item => item.Document!.CreatedAt)
            .Select(item => new
            {
                Method = item.PaymentMethod!.Designation,
                item.Amount,
                item.Document!.CreatedAt,
                item.Document.Number,
                item.Document.Type,
                Customer = item.Document.Customer.Name
            })
            .Take(MaxRows)
            .ToListAsync(cancellationToken);
        var signed = lines.Select(item => new
        {
            item.Method,
            item.CreatedAt,
            item.Number,
            item.Customer,
            Amount = item.Type == "NC" ? -item.Amount : item.Amount
        }).ToList();

        if (report.Key is "sales-payment-method")
        {
            return signed
                .GroupBy(item => item.Method ?? string.Empty)
                .OrderBy(group => group.Key)
                .Select(group => Row(
                    ("Método", group.Key),
                    ("Documentos", group.Count().ToString()),
                    ("Total", Money(group.Sum(item => item.Amount)))))
                .ToList();
        }

        return signed.Select(item => Row(
            ("Método", item.Method ?? string.Empty),
            ("Data", DateText(item.CreatedAt)),
            ("Número", item.Number ?? string.Empty),
            ("Cliente", item.Customer ?? string.Empty),
            ("Total", Money(item.Amount)))).ToList();
    }

    private static async Task<List<Dictionary<string, string>>> SalesLinesAsync(
        LogicPOSDbContext database,
        ReportDefinition report,
        ReportFilterRequest filters,
        DateTime start,
        DateTime until,
        CancellationToken cancellationToken)
    {
        var query = database.DocumentDetails.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Document != null && item.Document.IsDeleted == false)
            .Where(item => item.Document!.IsDraft == false && item.Document.Status != "A")
            .Where(item => item.Document!.Series!.DocumentType!.SaftDocumentType == SaftDocumentType.SalesInvoices)
            .Where(item => item.Document!.CreatedAt >= start && item.Document.CreatedAt < until);
        if (string.IsNullOrWhiteSpace(filters.DocumentTypeAcronym) == false)
        {
            var type = filters.DocumentTypeAcronym.Trim();
            query = query.Where(item => item.Document!.Type == type);
        }

        if (filters.TerminalId is Guid terminal && terminal != Guid.Empty)
        {
            query = query.Where(item => item.Document!.CreatedWhere == terminal);
        }

        if (filters.VatRateId is Guid vat && vat != Guid.Empty)
        {
            query = query.Where(item => item.Tax.TaxId == vat);
        }

        if (filters.FamilyId is Guid family && family != Guid.Empty)
        {
            query = query.Where(item => item.Article!.Subfamily!.FamilyId == family);
        }

        if (filters.SubfamilyId is Guid subfamily && subfamily != Guid.Empty)
        {
            query = query.Where(item => item.Article!.SubfamilyId == subfamily);
        }

        if (filters.ArticleId is Guid article && article != Guid.Empty)
        {
            query = query.Where(item => item.ArticleId == article);
        }

        var lines = await query
            .Select(item => new
            {
                item.Article!.Code,
                item.Article.Designation,
                Family = item.Article.Subfamily!.Family!.Designation,
                Subfamily = item.Article.Subfamily.Designation,
                ArticleType = item.Article.Type!.Designation,
                ArticleClass = item.Article.Class!.Designation,
                Vat = item.Tax.Designation,
                item.Quantity,
                item.TotalFinal,
                item.TotalNet,
                item.TotalTax,
                DocType = item.Document!.Type
            })
            .Take(MaxRows)
            .ToListAsync(cancellationToken);
        var signed = lines.Select(item =>
        {
            var sign = item.DocType == "NC" ? -1m : 1m;
            return new
            {
                Article = $"{item.Code} {item.Designation}".Trim(),
                item.Family,
                item.Subfamily,
                item.ArticleType,
                item.ArticleClass,
                item.Vat,
                Quantity = item.Quantity * sign,
                Total = item.TotalFinal * sign,
                Net = item.TotalNet * sign,
                Tax = item.TotalTax * sign
            };
        }).ToList();

        return report.Key switch
        {
            "sales-vat-type" => signed
                .GroupBy(item => (item.Vat, item.ArticleType))
                .OrderBy(group => group.Key.Vat).ThenBy(group => group.Key.ArticleType)
                .Select(group => Row(
                    ("Taxa IVA", group.Key.Vat ?? string.Empty),
                    ("Tipo de artigo", group.Key.ArticleType ?? string.Empty),
                    ("Quantidade", Qty(group.Sum(item => item.Quantity))),
                    ("Total", Money(group.Sum(item => item.Total)))))
                .ToList(),
            "sales-vat-class" => signed
                .GroupBy(item => (item.Vat, item.ArticleClass))
                .OrderBy(group => group.Key.Vat).ThenBy(group => group.Key.ArticleClass)
                .Select(group => Row(
                    ("Taxa IVA", group.Key.Vat ?? string.Empty),
                    ("Classe", group.Key.ArticleClass ?? string.Empty),
                    ("Quantidade", Qty(group.Sum(item => item.Quantity))),
                    ("Total", Money(group.Sum(item => item.Total)))))
                .ToList(),
            "sales-vat-group-detail" => signed
                .GroupBy(item => (item.Vat, item.Article))
                .OrderBy(group => group.Key.Vat).ThenBy(group => group.Key.Article)
                .Select(group => Row(
                    ("Taxa IVA", group.Key.Vat ?? string.Empty),
                    ("Artigo", group.Key.Article),
                    ("Quantidade", Qty(group.Sum(item => item.Quantity))),
                    ("Líquido", Money(group.Sum(item => item.Net))),
                    ("IVA", Money(group.Sum(item => item.Tax))),
                    ("Total", Money(group.Sum(item => item.Total)))))
                .ToList(),
            "sales-family-detail" => signed
                .GroupBy(item => (item.Family, item.Article))
                .OrderBy(group => group.Key.Family).ThenBy(group => group.Key.Article)
                .Select(group => Row(
                    ("Família", group.Key.Family ?? string.Empty),
                    ("Artigo", group.Key.Article),
                    ("Quantidade", Qty(group.Sum(item => item.Quantity))),
                    ("Total", Money(group.Sum(item => item.Total)))))
                .ToList(),
            "sales-subfamily-detail" => signed
                .GroupBy(item => (item.Family, item.Subfamily, item.Article))
                .OrderBy(group => group.Key.Family).ThenBy(group => group.Key.Subfamily).ThenBy(group => group.Key.Article)
                .Select(group => Row(
                    ("Família", group.Key.Family ?? string.Empty),
                    ("Subfamília", group.Key.Subfamily ?? string.Empty),
                    ("Artigo", group.Key.Article),
                    ("Quantidade", Qty(group.Sum(item => item.Quantity))),
                    ("Total", Money(group.Sum(item => item.Total)))))
                .ToList(),
            _ => signed
                .GroupBy(item => item.Article)
                .OrderBy(group => group.Key)
                .Select(group => Row(
                    ("Artigo", group.Key),
                    ("Quantidade", Qty(group.Sum(item => item.Quantity))),
                    ("Total", Money(group.Sum(item => item.Total)))))
                .ToList()
        };
    }

    private static async Task<List<Dictionary<string, string>>> SalesByPlaceAsync(
        LogicPOSDbContext database,
        ReportDefinition report,
        ReportFilterRequest filters,
        DateTime start,
        DateTime until,
        CancellationToken cancellationToken)
    {
        var query = database.OrderDocuments.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Document != null && item.Document.IsDeleted == false)
            .Where(item => item.Document!.IsDraft == false && item.Document.Status != "A")
            .Where(item => item.CreatedAt >= start && item.CreatedAt < until);
        if (string.IsNullOrWhiteSpace(filters.DocumentTypeAcronym) == false)
        {
            var type = filters.DocumentTypeAcronym.Trim();
            query = query.Where(item => item.Document!.Type == type);
        }

        if (filters.TerminalId is Guid terminal && terminal != Guid.Empty)
        {
            query = query.Where(item => item.Document!.CreatedWhere == terminal);
        }

        var lines = await query
            .OrderBy(item => item.CreatedAt)
            .Select(item => new
            {
                Place = item.Order!.Table!.Place!.Designation,
                Table = item.Order.Table.Designation,
                item.CreatedAt,
                item.Document!.Number,
                item.Document.Type,
                item.Document.TotalFinal
            })
            .Take(MaxRows)
            .ToListAsync(cancellationToken);
        var rows = lines.Select(item => Row(
            ("Zona", item.Place ?? string.Empty),
            ("Mesa", item.Table ?? string.Empty),
            ("Data", DateText(item.CreatedAt)),
            ("Número", item.Number ?? string.Empty),
            ("Total", Money(item.Type == "NC" ? -item.TotalFinal : item.TotalFinal)))).ToList();
        if (report.Key is "sales-table-detail")
        {
            return rows
                .OrderBy(row => row["Mesa"])
                .ThenBy(row => row["Data"])
                .ToList();
        }

        return rows;
    }

    private static async Task<List<Dictionary<string, string>>> SalesAsync(
        LogicPOSDbContext database,
        ReportDefinition report,
        ReportFilterRequest filters,
        DateTime start,
        DateTime until,
        CancellationToken cancellationToken)
    {
        Guid? customerId = report.ShowCustomer ? filters.CustomerId : null;
        var sales = await LoadSalesAsync(database, filters, start, until, customerId, cancellationToken);
        if (sales.Count == 0)
        {
            return [];
        }

        var detailed = report.Key.EndsWith("-detail", StringComparison.Ordinal);
        var users = report.Key.StartsWith("sales-employee", StringComparison.Ordinal)
            ? await database.Users.AsNoTracking().Select(item => new { item.Id, item.Name }).ToDictionaryAsync(item => item.Id, item => item.Name, cancellationToken)
            : null;
        var terminals = report.Key.StartsWith("sales-terminal", StringComparison.Ordinal)
            ? await database.Terminals.AsNoTracking().Select(item => new { item.Id, item.Designation }).ToDictionaryAsync(item => item.Id, item => item.Designation, cancellationToken)
            : null;

        string KeyOf(SaleLine item) => report.Key switch
        {
            "sales-day" or "sales-day-detail" => DateText(item.CreatedAt),
            "sales-employee" or "sales-employee-detail" => Lookup(users, item.CreatedBy),
            "sales-terminal" or "sales-terminal-detail" => Lookup(terminals, item.CreatedWhere),
            "sales-customer" or "sales-customer-detail" => item.Customer,
            "sales-payment-condition" or "sales-payment-condition-detail" => item.PaymentCondition,
            "sales-currency" or "sales-currency-detail" => item.Currency,
            "sales-country" or "sales-country-detail" => item.Country,
            _ => item.Type
        };
        var header = report.Key switch
        {
            "sales-day" or "sales-day-detail" => "Data",
            "sales-employee" or "sales-employee-detail" => "Funcionário",
            "sales-terminal" or "sales-terminal-detail" => "Terminal",
            "sales-customer" or "sales-customer-detail" => "Cliente",
            "sales-payment-condition" or "sales-payment-condition-detail" => "Condição",
            "sales-currency" or "sales-currency-detail" => "Moeda",
            "sales-country" or "sales-country-detail" => "País",
            _ => "Tipo"
        };

        if (detailed == false)
        {
            return sales
                .GroupBy(KeyOf)
                .OrderBy(group => group.Key)
                .Select(group => Row(
                    (header, group.Key),
                    ("Documentos", group.Count().ToString()),
                    ("Total", Money(group.Sum(item => item.SignedTotal)))))
                .ToList();
        }

        return sales.Select(item =>
        {
            var cells = new List<(string Key, string Value)>
            {
                (header, KeyOf(item)),
                ("Número", item.Number),
                ("Total", Money(item.SignedTotal))
            };
            if (header != "Data")
            {
                cells.Insert(1, ("Data", DateText(item.CreatedAt)));
            }

            if (header != "Cliente")
            {
                cells.Insert(cells.Count - 1, ("Cliente", item.Customer));
            }

            return Row(cells.ToArray());
        }).ToList();
    }

    private static IQueryable<Document> SalesQuery(
        LogicPOSDbContext database,
        ReportFilterRequest filters,
        DateTime start,
        DateTime until,
        Guid? customerId)
    {
        var documents = database.Documents.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .ApplyActiveSalesInvoicesFilter()
            .Where(item => item.CreatedAt >= start && item.CreatedAt < until);
        if (string.IsNullOrWhiteSpace(filters.DocumentTypeAcronym) == false)
        {
            documents = documents.ApplyDocumentTypeFilter(filters.DocumentTypeAcronym.Trim());
        }

        if (filters.TerminalId is Guid terminal && terminal != Guid.Empty)
        {
            documents = documents.ApplyTerminalFilter(terminal);
        }

        if (customerId is Guid customer && customer != Guid.Empty)
        {
            documents = documents.Where(item => item.CustomerId == customer);
        }

        return documents;
    }

    private static async Task<List<SaleLine>> LoadSalesAsync(
        LogicPOSDbContext database,
        ReportFilterRequest filters,
        DateTime start,
        DateTime until,
        Guid? customerId,
        CancellationToken cancellationToken)
    {
        var raw = await SalesQuery(database, filters, start, until, customerId)
            .OrderBy(item => item.CreatedAt)
            .Select(item => new
            {
                item.CreatedAt,
                item.Number,
                item.Type,
                item.TotalFinal,
                item.TotalNet,
                item.TotalTax,
                Customer = item.Customer.Name,
                FiscalNumber = item.Customer.FiscalNumber,
                Country = item.Customer.Country,
                PaymentCondition = item.PaymentCondition != null ? item.PaymentCondition.Designation : string.Empty,
                Currency = item.Currency != null ? item.Currency.Designation : string.Empty,
                item.CreatedBy,
                item.CreatedWhere
            })
            .Take(MaxRows)
            .ToListAsync(cancellationToken);
        return raw.Select(item => new SaleLine(
            item.CreatedAt,
            item.Number ?? string.Empty,
            item.Type ?? string.Empty,
            item.TotalFinal,
            item.TotalNet,
            item.TotalTax,
            item.Customer ?? string.Empty,
            item.FiscalNumber ?? string.Empty,
            item.Country ?? string.Empty,
            item.PaymentCondition ?? string.Empty,
            item.Currency ?? string.Empty,
            item.CreatedBy,
            item.CreatedWhere)).ToList();
    }

    private static string Lookup(IReadOnlyDictionary<Guid, string>? names, Guid id)
        => names is not null && names.TryGetValue(id, out var name) && string.IsNullOrWhiteSpace(name) == false
            ? name
            : "—";

    private static string Money(decimal value) => value.ToString("0.00");

    private static string Qty(decimal value) => value.ToString("0.##");

    private static string DateText(DateTime value) => value.ToString("dd/MM/yyyy");

    private static Dictionary<string, string> Row(params (string Key, string Value)[] cells)
        => cells.ToDictionary(cell => cell.Key, cell => cell.Value ?? string.Empty);

    private sealed record SaleLine(
        DateTime CreatedAt,
        string Number,
        string Type,
        decimal TotalFinal,
        decimal TotalNet,
        decimal TotalTax,
        string Customer,
        string FiscalNumber,
        string Country,
        string PaymentCondition,
        string Currency,
        Guid CreatedBy,
        Guid CreatedWhere)
    {
        public decimal Sign => Type == "NC" ? -1m : 1m;

        public decimal SignedTotal => TotalFinal * Sign;

        public decimal SignedNet => TotalNet * Sign;

        public decimal SignedTax => TotalTax * Sign;
    }
}
