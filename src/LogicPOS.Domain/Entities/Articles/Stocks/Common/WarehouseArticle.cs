using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Domain.Entities;

//[Table("fin_articlewarehouse")]
public class WarehouseArticle : Entity
{
    public Article? Article { get; set; }
    public Guid ArticleId { get; set; }
    public string? SerialNumber { get; set; } 
    public ArticleSerialNumberStatus Status { get; set; } = ArticleSerialNumberStatus.None;
    public WarehouseLocation? WarehouseLocation { get; set; }
    public Guid WarehouseLocationId { get; set; }
    
    [Column(TypeName = "decimal(18,6)")]
    public decimal Quantity { get; set; }
}
