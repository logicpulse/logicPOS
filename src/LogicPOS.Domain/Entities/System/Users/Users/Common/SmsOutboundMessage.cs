using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

/// <summary>
/// One outbound SMS counted against the licence quota (all_NumberSMS).
/// </summary>
public class SmsOutboundMessage : Entity
{
    public Guid? UserId { get; set; }
    public string Destination { get; set; } = null!;
    public string Purpose { get; set; } = null!;
}
