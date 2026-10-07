using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("sys_systemnotificationtype")]
public class SystemNotificationType : Entity.WithCode.AndOrder.AndDesignation
{
    public string? Message { get; set; }
    public int WarnDaysBefore { get; set; }
    public Guid? TargetUserId { get; set; }
    public Guid? TargetTerminalId { get; set; }
}
