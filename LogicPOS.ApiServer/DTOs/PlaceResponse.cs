namespace LogicPOS.ApiServer.DTOs;

public sealed class PlaceResponse
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

    // See ApiPlace.PriceTypeId: no PriceTypes catalog exists client-side yet, so this is always Undefined (0).
    public int PriceType { get; set; }

    public MovementTypeResponse? MovementType { get; set; }
    public string? ButtonImage { get; set; }
}

public sealed class AddPlaceRequest
{
    public string Designation { get; set; } = string.Empty;
    public string? ButtonImage { get; set; }
    public Guid? PriceTypeId { get; set; }
    public Guid MovementTypeId { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdatePlaceRequest
{
    public uint Order { get; set; }
    public string? Code { get; set; }
    public string Designation { get; set; } = string.Empty;
    public Guid? PriceTypeId { get; set; }
    public Guid MovementTypeId { get; set; }
    public string? ButtonImage { get; set; }
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; }
}
