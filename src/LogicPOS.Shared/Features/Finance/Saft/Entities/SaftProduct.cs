namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftProduct
{
    public string Type { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Group { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? NumberCode {get;set;}
}