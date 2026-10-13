using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

/// <summary>
/// Concurrent licence seat for one full API session (Avalonia or Web).
/// </summary>
public class LicenseSession : Entity
{
    public Guid UserId { get; set; }
    public string ClientKind { get; set; } = null!; // "Avalonia" | "Web"
    public Guid SessionKey { get; set; }
    public DateTime LastHeartbeatAt { get; set; } = DateTime.UtcNow;
    public Guid? TerminalId { get; set; }
}

public static class LicenseSessionClientKinds
{
    public const string Avalonia = "Avalonia";
    public const string Web = "Web";
}