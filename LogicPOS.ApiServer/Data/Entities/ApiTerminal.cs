namespace LogicPOS.ApiServer.Data.Entities;

public sealed class ApiTerminal
{
    public Guid Id { get; set; }
    public uint Order { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string HardwareId { get; set; } = string.Empty;
    public uint TimerInterval { get; set; } = 100;
    public bool IsDefault { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
