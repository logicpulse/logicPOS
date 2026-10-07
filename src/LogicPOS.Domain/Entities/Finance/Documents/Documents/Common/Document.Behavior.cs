using LogicPOS.Domain.Entities.Documents;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Finance.Calculations;
using LogicPOS.Domain.Entities.Finance.Documents.Documents.Create;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;
using LogicPOS.Domain.Services;
using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Domain.Entities;

public partial class Document
{
    public decimal GetTaxPayableForAmount(decimal amount)
    {
        if (TotalFinal == 0) return 0;
        decimal taxPayable = (amount * TotalTax) / TotalFinal;
        return taxPayable.Round();
    }

    internal void CalculateTotals()
    {
        TotalFinal = TotalNet + TotalTax;

        if (TypeAnalyzer.RequireFullPayment())
        {
            if (TotalDelivery != 0)
            {
                TotalChange = TotalDelivery - TotalFinal;
                Paid = TotalDelivery.Round() >= TotalFinal.Round();
            }
            else
            {
                TotalDelivery = TotalFinal;
                Paid = true;
            }
        }
    }

    public DocumentTypeAnalyzer TypeAnalyzer => new(Type);

    public Result<Payment> AssociateReceipt(Receipt receipt,
        decimal paymentAmount,
        decimal totalPaidTillNow)
    {
        if (Paid)
        {
            return Result.InvalidOperation<Payment>("Documento já está pago");
        }

        var documentPayment = new Payment
        {
            DocumentId = Id,
            ReceiptId = receipt.Id,
            CreditAmount = paymentAmount,
            DebitAmount = 0
        };

        SyncPaymentStatus(totalPaidTillNow + paymentAmount);

        return documentPayment;
    }

    public Result<Payment> AssociateReceiptDebit(Receipt receipt)
    {
        if (Paid)
        {
            return Result.InvalidOperation<Payment>($"Documento {Number} já está pago");
        }

        var debitAmount = TotalFinal.Round();
        var documentPayment = new Payment
        {
            DocumentId = Id,
            ReceiptId = receipt.Id,
            CreditAmount = 0,
            DebitAmount = debitAmount
        };

        SyncPaymentStatus(debitAmount);

        return documentPayment;
    }

    public void SyncPaymentStatus(decimal totalPaid)
    {
        var shouldBePaid = totalPaid.Round() >= TotalFinal.Round();
        var wasPaid = Paid;

        Paid = shouldBePaid;

        if (shouldBePaid && !wasPaid)
        {
            PaymentDate = DateTime.Now;
        }
        else if (!shouldBePaid)
        {
            PaymentDate = null;
        }
    }
    

    internal async Task<Result> AddDetailsAsync(IEnumerable<CreateDocumentDetailDto> createDetailDtos,
        DocumentDetailReferences references,
        CancellationToken cancellationToken = default)
    {
        var detailList = createDetailDtos as IList<CreateDocumentDetailDto> ?? createDetailDtos.ToList();
        var lookup = references.Lookup
            ?? await DocumentDetailLookup.LoadAsync(detailList, references, cancellationToken);
        var referencesWithLookup = references with { Lookup = lookup };

        foreach (var createDetailDto in detailList)
        {
            var addDetailResult = await AddDetailAsync(createDetailDto,
                referencesWithLookup,
                cancellationToken);

            if (addDetailResult.IsFailure)
            {
                return addDetailResult;
            }
        }

        return Result.Success();
    }

    private async Task<Result> AddDetailAsync(CreateDocumentDetailDto createDetailDto,
        DocumentDetailReferences references, 
        CancellationToken cancellationToken = default)
    {
        CreateDocumentDetailDto createDetailWithGlobalDiscountDto =
            createDetailDto with { Discount = createDetailDto.Discount + Discount };

        var detailCreator = new DocumentDetailCreator(createDetailWithGlobalDiscountDto,
            references);

        var createDetailResult = await detailCreator.CreateAsync(cancellationToken);

        if (createDetailResult.IsFailure)
        {
            return createDetailResult;
        }

        var detail = createDetailResult.Value!;
        detail.DocumentId = Id;

        Details ??= new List<DocumentDetail>();

        Details.Add(detail);
        TotalNet += detail.TotalNet;
        TotalDiscount += detail.TotalDiscount;
        TotalTax += detail.TotalTax;

        var articleClassAcronym = references.Lookup?.Articles.TryGetValue(detail.ArticleId, out var article) == true
            ? article.ClassAcronym
            : await references.ArticleRepository.GetArticleClassAcronymAsync(detail.ArticleId, cancellationToken);
        if (articleClassAcronym == "S")
        {
            WithholdingTaxAmount += (6.5M / 100M) * detail.TotalNet;
        }

        return Result.Success();
    }

    public static async Task<Result<Document>> CreatePreviewAsync(CreateDocumentPreviewDto dto,
        DocumentReferences references,
        CancellationToken cancellationToken = default)
    {
        var documentCreator = new DocumentPreviewCreator(dto, references);

        return await documentCreator.CreateAsync(cancellationToken);
    }

    public static async Task<Result<Document>> CreateAsync(CreateDocumentDto dto,
        IDocumentRepository repository,
        DocumentReferences references,
        IDocumentHasher hasher,
        FiscalCountry country,
        Guid? terminalId,
        CancellationToken cancellationToken = default)
    {
        var documentCreator = new DocumentCreator(dto,
            repository,
            references,
            hasher,
            country,
            terminalId);

        return await documentCreator.CreateAsync(cancellationToken);
    }
}