using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("sys_systemaudittype")]
public class SystemAuditType : Entity.WithCode.AndOrder.AndDesignation
{
    public string? Token { get; set; }
    public string? ResourceString { get; set; }
}
