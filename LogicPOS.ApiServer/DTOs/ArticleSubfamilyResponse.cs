namespace LogicPOS.ApiServer.DTOs;

public sealed class ArticleSubfamilyResponse
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

    public ArticleFamilyResponse? Family { get; set; }
    public Guid FamilyId { get; set; }

    // CommissionGroups/DiscountGroups catalogs are not built yet, so these nested objects are always null.
    public object? CommissionGroup { get; set; }
    public Guid? CommissionGroupId { get; set; }
    public object? DiscountGroup { get; set; }
    public Guid? DiscountGroupId { get; set; }

    public VatRateResponse? VatOnTable { get; set; }
    public Guid? VatOnTableId { get; set; }
    public VatRateResponse? VatDirectSelling { get; set; }
    public Guid? VatDirectSellingId { get; set; }

    public ButtonResponse? Button { get; set; }
}

public sealed class AddArticleSubfamilyRequest
{
    public string Designation { get; set; } = string.Empty;
    public Guid FamilyId { get; set; }
    public Guid? CommissionGroupId { get; set; }
    public Guid? DiscountGroupId { get; set; }
    public Guid? VatOnTableId { get; set; }
    public Guid? VatDirectSellingId { get; set; }
    public ButtonResponse? Button { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdateArticleSubfamilyRequest
{
    public uint Order { get; set; }
    public string? Code { get; set; }
    public string Designation { get; set; } = string.Empty;
    public Guid FamilyId { get; set; }
    public Guid? CommissionGroupId { get; set; }
    public Guid? DiscountGroupId { get; set; }
    public Guid? VatOnTableId { get; set; }
    public Guid? VatDirectSellingId { get; set; }
    public ButtonResponse? Button { get; set; }
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; }
}
