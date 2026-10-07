using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("sys_systemnotification")]
public class SystemNotification : Entity
{
    public Guid TypeId { get; set; }
    public SystemNotificationType? Type { get; set; }

    public uint Ord { get; set; }
    public string? Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime ReadingDate { get; set; }

    public User? TargetUser { get; set; }
    public Guid? TargetUserId { get; set; }

    public Terminal? TargetTerminal { get; set; }
    public Guid? TargetTerminalId { get; set; }

    public User? LastReadUser { get; set; }
    public Guid? LastReadUserId { get; set; }

    public Terminal? LastReadTerminal { get; set; }
    public Guid? LastReadTerminalId { get; set; }

}
