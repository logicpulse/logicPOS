using System.ComponentModel.DataAnnotations.Schema;

namespace LogicPOS.Domain.Entities.Common;

[ComplexType]
public record Button
{
    public string? Label { get; set; }
    public string? Image { get; set; }
    public string? ImageExtension { get; set; }
}