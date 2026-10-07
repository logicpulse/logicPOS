namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftPaymentLine
{
    public string CustomerId { get; init; } = null!;
    public string InvoiceNumber { get; init; } = null!;
    public string InvoiceDate { get; init; } = null!;
    public decimal CreditAmount { get; set; }
    public decimal DebitAmount { get; set; }
}