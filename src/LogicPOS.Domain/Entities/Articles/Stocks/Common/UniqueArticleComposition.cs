using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("fin_articlecompositionserialnumber")]
public class UniqueArticleComposition : Entity
{
    public WarehouseArticle? Parent { get; set; }
    public Guid? ParentId { get; set; }

    public WarehouseArticle? Child { get; set; }
    public Guid? ChildId { get; set; }
}
