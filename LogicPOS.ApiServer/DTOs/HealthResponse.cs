namespace LogicPOS.ApiServer.DTOs;

public sealed class HealthResponse
{
    public string Status { get; set; } = "Healthy";
    public string Environment { get; set; } = string.Empty;
    public bool DatabaseAvailable { get; set; }
    public DateTime TimestampUtc { get; set; }
    public string Version { get; set; } = string.Empty;
}
