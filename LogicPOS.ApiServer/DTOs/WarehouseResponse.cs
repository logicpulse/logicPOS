namespace LogicPOS.ApiServer.DTOs;

public sealed class WarehouseLocationResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public WarehouseResponse? Warehouse { get; set; }
    public Guid WarehouseId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public sealed class WarehouseResponse
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
    public bool IsDefault { get; set; }
    public List<WarehouseLocationResponse> Locations { get; set; } = [];
}

public sealed class AddWarehouseRequest
{
    public string Designation { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public List<string>? Locations { get; set; }
}

public sealed class UpdateWarehouseRequest
{
    public uint Order { get; set; }
    public string? Code { get; set; }
    public string Designation { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsDeleted { get; set; }
    public string? Notes { get; set; }
}

public sealed class AddWarehouseLocationsRequest
{
    public List<string> Locations { get; set; } = [];
}

public sealed class UpdateWarehouseLocationRequest
{
    public string Designation { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
