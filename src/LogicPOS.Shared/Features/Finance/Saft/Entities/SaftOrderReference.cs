namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftOrderReference
{
    public string OriginatingOn { get; set; } = null!;
    public string OrderDate { get; set; } = null!;
}