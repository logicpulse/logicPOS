namespace LogicPOS.ApiServer.DTOs;

public sealed class ButtonResponse
{
    public string? Label { get; set; }
    public string? Image { get; set; }
    public string ImageExtension { get; set; } = "png";
}

public sealed class ArticleFamilyResponse
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

    // CommissionGroups/DiscountGroups catalogs are not built yet, so these nested objects are always null.
    public object? CommissionGroup { get; set; }
    public Guid? CommissionGroupId { get; set; }
    public object? DiscountGroup { get; set; }
    public Guid? DiscountGroupId { get; set; }

    public ButtonResponse? Button { get; set; }
}

public sealed class AddArticleFamilyRequest
{
    public string Designation { get; set; } = string.Empty;
    public Guid? CommissionGroupId { get; set; }
    public Guid? DiscountGroupId { get; set; }
    public ButtonResponse? Button { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdateArticleFamilyRequest
{
    public uint? Order { get; set; }
    public string? Code { get; set; }
    public string Designation { get; set; } = string.Empty;
    public Guid? CommissionGroupId { get; set; }
    public Guid? DiscountGroupId { get; set; }
    public ButtonResponse? Button { get; set; }
    public string? Notes { get; set; }
    public bool? IsDeleted { get; set; }
}
