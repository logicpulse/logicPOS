namespace LogicPOS.ApiServer.Data.Entities;

public enum ApiTableStatus
{
    Free = 0,
    Open = 1,
    Reserved = 2
}

public sealed class ApiTable
{
    public Guid Id { get; set; }
    public uint Order { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public Guid PlaceId { get; set; }
    public string? ButtonImage { get; set; }
    public ApiTableStatus Status { get; set; } = ApiTableStatus.Free;
    public DateTime? OpennedAtUtc { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
