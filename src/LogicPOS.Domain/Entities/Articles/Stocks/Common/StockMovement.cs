using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("fin_articlestock")]
public class StockMovement : Entity
{
    public Customer? Customer { get; set; }
    public Guid? CustomerId { get; set; }

    public Article? Article { get; set; }
    public Guid ArticleId { get; set; }
    public string? SerialNumber { get; set; }
    public WarehouseLocation? WarehouseLocation { get; set; }
    public Guid? WarehouseLocationId { get; set; }
    public string? DocumentNumber { get; set; } 

    [Column(TypeName = "decimal(18,6)")]
    public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal Price { get; set; }
    public DateTime Date { get; set; }
    public DocumentDetail? Detail { get; set; }
    public Guid? DetailId { get; set; }
    public byte[]? ExternalDocument { get; set; }
}
