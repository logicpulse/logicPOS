namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftPaymentMechanism
{
    public string Code { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}