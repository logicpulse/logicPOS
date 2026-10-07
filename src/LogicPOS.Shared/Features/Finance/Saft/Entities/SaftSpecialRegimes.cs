namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftSpecialRegimes {
    public string SelfBillingIndicator { get; set; } = null!;
    public string CashVATSchemeIndicator { get; set; } = null!;
    public string ThirdPartiesBillingIndicator { get; set; } = null!;
}