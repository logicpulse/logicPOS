using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("sys_systemnotificationdocumentmaster")]
public class DocumentNotification : Entity
{
    public Guid NotificationId {  get; set; }
    public Guid DocumentId { get; set; }
}
