using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("sys_userpermissionitem")]
public class PermissionItem : Entity.WithCode.AndOrder.AndDesignation
{
    public string Token { get; set; } = null!;
    public PermissionGroup? PermissionGroup { get; set; }
    public Guid PermissionGroupId { get; set; }
}
