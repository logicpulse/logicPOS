namespace LogicPOS.ApiServer.DTOs;

public sealed class TableResponse
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
    public PlaceResponse? Place { get; set; }
    public Guid PlaceId { get; set; }
    public string? ButtonImage { get; set; }
    public int Status { get; set; }
    public DateTime? OpennedAt { get; set; }
}

public sealed class TableViewModelResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int PriceTypeEnum { get; set; }
    public string Designation { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTime? OpennedAt { get; set; }
    public Guid PlaceId { get; set; }
}

public sealed class AddTableRequest
{
    public string Designation { get; set; } = string.Empty;
    public Guid PlaceId { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdateTableRequest
{
    public uint Order { get; set; }
    public string? Code { get; set; }
    public string Designation { get; set; } = string.Empty;
    public Guid PlaceId { get; set; }
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; }
}
