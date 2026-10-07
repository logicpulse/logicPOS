namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftPayments {
    public int NumberOfEntries => Payments.Count;
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public List<SaftPayment> Payments { get; set; } = [];
}