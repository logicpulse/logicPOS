namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftCurrency
{
    public string CurrencyCode { get; set; } = null!;
    public string CurrencyAmount { get; set; } = null!;
    public string ExchangeRate { get; set; } = null!;
}