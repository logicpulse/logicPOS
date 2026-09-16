namespace LogicPOS.ApiServer.Data.Entities;

public sealed class ApiPlace
{
    public Guid Id { get; set; }
    public uint Order { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;

    // The client's Place.PriceType is an enum, but AddPlaceCommand/UpdatePlaceCommand send a
    // PriceTypeId (Guid) instead — there is no PriceTypes catalog wired up client-side yet.
    // We keep the raw id for round-tripping and always report PriceType as Undefined (0).
    public Guid? PriceTypeId { get; set; }

    public Guid MovementTypeId { get; set; }
    public string? ButtonImage { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
