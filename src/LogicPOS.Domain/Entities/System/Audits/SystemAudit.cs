using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("sys_systemaudit")]
public class SystemAudit : Entity
{
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public SystemAuditType? Type { get; set; }
    public Guid TypeId { get; set; }
}
