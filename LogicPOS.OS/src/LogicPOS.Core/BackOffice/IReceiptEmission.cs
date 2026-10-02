namespace LogicPOS.Core.BackOffice;

public interface IReceiptEmission
{
    Task<IReadOnlyList<ReceiptEmissionRow>> ListAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);

    Task<ReceiptEmissionCatalog> CatalogAsync(CancellationToken cancellationToken = default);

    Task<ReceiptPayResult> PayAsync(ReceiptPayRequest request, CancellationToken cancellationToken = default);
}

public sealed class ReceiptEmissionRow
{
    public Guid Id { get; init; }

    public Guid CustomerId { get; init; }

    public Guid CurrencyId { get; init; }

    public DateTime CreatedAt { get; init; }

    public string Number { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public bool IsDraft { get; init; }

    public bool Paid { get; init; }

    public string CustomerName { get; init; } = string.Empty;

    public string FiscalNumber { get; init; } = string.Empty;

    public string Currency { get; init; } = string.Empty;

    public decimal TotalFinal { get; init; }

    public decimal TotalPaid { get; init; }

    public decimal TotalToPay { get; init; }

    public string RelatedDocuments { get; init; } = string.Empty;
}

public sealed class ReceiptPayOption
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal ExchangeRate { get; init; } = 1m;

    public override string ToString() => Name;
}

public sealed class ReceiptEmissionCatalog
{
    public IReadOnlyList<ReceiptPayOption> PaymentMethods { get; init; } = [];

    public IReadOnlyList<ReceiptPayOption> Currencies { get; init; } = [];
}

public sealed class ReceiptPayRequest
{
    public IReadOnlyList<Guid> DocumentIds { get; init; } = [];

    public Guid PaymentMethodId { get; init; }

    public Guid CurrencyId { get; init; }

    public decimal Amount { get; init; }

    public decimal CurrencyAmount { get; init; }

    public decimal ExchangeRate { get; init; }

    public string Notes { get; init; } = string.Empty;
}

public sealed class ReceiptPayResult
{
    public bool Ok { get; init; }

    public string Message { get; init; } = string.Empty;

    public string? PdfPath { get; init; }

    public static ReceiptPayResult Done(string message, string? pdfPath = null) => new()
    {
        Ok = true,
        Message = message,
        PdfPath = pdfPath
    };

    public static ReceiptPayResult Fail(string message) => new()
    {
        Ok = false,
        Message = message
    };
}

public static class ReceiptSettlement
{
    public static decimal Round(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);

    public static decimal Net(IEnumerable<ReceiptEmissionRow> documents)
    {
        var list = documents as IList<ReceiptEmissionRow> ?? documents.ToList();
        var debit = list.Where(row => row.Type is "FT" or "ND").Sum(row => row.TotalToPay);
        var credit = list.Where(row => row.Type == "NC").Sum(row => row.TotalFinal);
        return Round(debit - credit);
    }
}
