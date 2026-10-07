namespace LogicPOS.Application.Features.System
{
    public interface ISystemNotificationService
    {
         Task CreateSystemNotificationAsync(List<NotificationItem> values);
    }

    public record NotificationItem
    {
        public Guid DocumentId { get; set; }
        public string Number { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public Guid NotificationTypeId { get; set; }
    }
}
