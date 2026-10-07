using System.ComponentModel.DataAnnotations.Schema;

namespace LogicPOS.Domain.ValueObjects;

[ComplexType]
public sealed class DocumentDetailTax
{
    public Guid TaxId { get; set; }
    public string Designation { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    [Column(TypeName = "decimal(18,6)")] public decimal Percentage { get; set; }
    public string CountryRegion { get; set; } = null!;
}