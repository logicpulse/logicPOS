namespace LogicPOS.ApiServer.DTOs;

public sealed class HolidayResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public uint Order { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public bool Fixed { get; set; }
}

public sealed class AddHolidayRequest
{
    public string Designation { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdateHolidayRequest
{
    public uint Order { get; set; }
    public string? Code { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string? Notes { get; set; }
    public bool Fixed { get; set; }
    public bool IsDeleted { get; set; }
}
