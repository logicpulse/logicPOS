namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftTax
{
    public string Type { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Percentage { get; set; } 
    public string CountryRegion { get; set; } = null!;
}