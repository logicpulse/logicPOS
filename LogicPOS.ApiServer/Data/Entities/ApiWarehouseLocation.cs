namespace LogicPOS.ApiServer.Data.Entities;

public sealed class ApiWarehouseLocation
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
