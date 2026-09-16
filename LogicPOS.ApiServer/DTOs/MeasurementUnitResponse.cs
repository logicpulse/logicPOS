namespace LogicPOS.ApiServer.DTOs;

public sealed class MeasurementUnitResponse
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
    public string Acronym { get; set; } = string.Empty;
}

public sealed class AddMeasurementUnitRequest
{
    public string Designation { get; set; } = string.Empty;
    public string? Acronym { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdateMeasurementUnitRequest
{
    public uint Order { get; set; }
    public string? Code { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string? Acronym { get; set; }
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; }
}
