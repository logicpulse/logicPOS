namespace LogicPOS.ApiServer.Data.Entities;

public sealed class ApiHoliday
{
    public Guid Id { get; set; }
    public uint Order { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public bool IsFixed { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
