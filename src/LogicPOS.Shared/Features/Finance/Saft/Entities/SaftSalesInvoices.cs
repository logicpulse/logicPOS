namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftSalesInvoices
{
    public int NumberOfEntries => Invoices.Count;
    public decimal TotalCredit { get; set; }
    public decimal TotalDebit { get; set; }
    public List<SaftInvoice> Invoices { get; set; } = null!;
    
}