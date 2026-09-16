namespace LogicPOS.ApiServer.Data.Entities;

public sealed class ApiArticleFamily
{
    public Guid Id { get; set; }
    public uint Order { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;

    // CommissionGroups/DiscountGroups catalogs are not built yet; the ids are stored
    // for round-tripping but never validated or resolved to a nested object.
    public Guid? CommissionGroupId { get; set; }
    public Guid? DiscountGroupId { get; set; }

    public string? ButtonLabel { get; set; }
    public string? ButtonImage { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
