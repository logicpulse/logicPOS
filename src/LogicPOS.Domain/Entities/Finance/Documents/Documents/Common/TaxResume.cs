namespace LogicPOS.Domain.ValueObjects;

public record TaxResume
{
    public string Code { get; set; } = null!;
    public string Designation { get; set; } = null!;
    public decimal Base { get; set; }
    public decimal Rate { get; set; }
    public decimal Total { get; set; }
}