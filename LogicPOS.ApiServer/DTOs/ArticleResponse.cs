namespace LogicPOS.ApiServer.DTOs;

public sealed class ArticlePriceRequest
{
    public decimal Value { get; set; }
    public decimal PromotionValue { get; set; }
    public bool UsePromotion { get; set; }
}

public sealed class ArticlePriceResponse
{
    public decimal Value { get; set; }
    public decimal PromotionValue { get; set; }
    public bool UsePromotion { get; set; }
}

public sealed class ArticleResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    public string Code { get; set; } = string.Empty;
    public uint Order { get; set; }
    public string Designation { get; set; } = string.Empty;

    public Guid ClassId { get; set; }
    public ArticleClassResponse? Class { get; set; }
    public ArticleSubfamilyResponse? Subfamily { get; set; }
    public Guid SubfamilyId { get; set; }
    public ArticleTypeResponse? Type { get; set; }
    public MeasurementUnitResponse? MeasurementUnit { get; set; }
    public SizeUnitResponse? SizeUnit { get; set; }

    // CommissionGroups/DiscountGroups/VatExemptionReasons catalogs are not built yet: always null.
    public object? CommissionGroup { get; set; }
    public object? DiscountGroup { get; set; }
    public VatRateResponse? VatDirectSelling { get; set; }
    public object? VatExemptionReason { get; set; }

    public ButtonResponse? Button { get; set; }
    public ArticlePriceResponse Price1 { get; set; } = new();
    public ArticlePriceResponse Price2 { get; set; } = new();
    public ArticlePriceResponse Price3 { get; set; } = new();
    public ArticlePriceResponse Price4 { get; set; } = new();
    public ArticlePriceResponse Price5 { get; set; } = new();
    public bool PriceWithVat { get; set; }
    public decimal Discount { get; set; }
    public decimal DefaultQuantity { get; set; }
    public decimal MinimumStock { get; set; }
    public decimal Tare { get; set; }
    public float Weight { get; set; }
    public string? Barcode { get; set; }
    public bool PVPVariable { get; set; }
    public bool Favorite { get; set; }
    public bool UseWeighingBalance { get; set; }
    public bool IsComposed { get; set; }
    public bool UniqueArticles { get; set; }
    public bool IsSdrPackaging { get; set; }
    public string? BarcodeLabelPrintModel { get; set; }
}

public sealed class ArticleViewModelResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    public uint Order { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsComposed { get; set; }
    public bool IsSdrPackaging { get; set; }
    public string Family { get; set; } = string.Empty;
    public string Subfamily { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal DefaultQuantity { get; set; }
    public decimal MinimumStock { get; set; }
    public decimal Price1 { get; set; }
    public decimal Price2 { get; set; }
    public decimal Price3 { get; set; }
    public decimal Price4 { get; set; }
    public decimal Price5 { get; set; }
    public decimal Price1PromotionValue { get; set; }
    public decimal Price2PromotionValue { get; set; }
    public decimal Price3PromotionValue { get; set; }
    public decimal Price4PromotionValue { get; set; }
    public decimal Price5PromotionValue { get; set; }
    public bool Price1UsePromotion { get; set; }
    public bool Price2UsePromotion { get; set; }
    public bool Price3UsePromotion { get; set; }
    public bool Price4UsePromotion { get; set; }
    public bool Price5UsePromotion { get; set; }
    public decimal? VatDirectSelling { get; set; }
    public decimal Discount { get; set; }
    public string Unit { get; set; } = string.Empty;
    public Guid SubfamilyId { get; set; }
    public Guid FamilyId { get; set; }
    public Guid VatRateId { get; set; }
    public Guid? VatExemptionReasonId { get; set; }
    public ButtonResponse? Button { get; set; }
    public bool PriceWithVat { get; set; }
    public string ClassAcronym { get; set; } = string.Empty;
    public string? BarcodeLabelPrintModel { get; set; }
}

public sealed class ArticlesPaginatedResponse
{
    public IEnumerable<ArticleViewModelResponse> Items { get; set; } = [];
    public int ItemsCount { get; set; }
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public sealed class AddArticleRequest
{
    public string? Code { get; set; }
    public string? CodeDealer { get; set; }
    public string Designation { get; set; } = string.Empty;
    public ButtonResponse? Button { get; set; }
    public ArticlePriceRequest? Price1 { get; set; }
    public ArticlePriceRequest? Price2 { get; set; }
    public ArticlePriceRequest? Price3 { get; set; }
    public ArticlePriceRequest? Price4 { get; set; }
    public ArticlePriceRequest? Price5 { get; set; }
    public bool PriceWithVat { get; set; }
    public decimal Discount { get; set; }
    public decimal DefaultQuantity { get; set; }
    public decimal TotalStock { get; set; }
    public decimal MinimumStock { get; set; }
    public decimal Tare { get; set; }
    public float Weight { get; set; }
    public string? Barcode { get; set; }
    public bool PVPVariable { get; set; }
    public bool Favorite { get; set; }
    public bool UseWeighingBalance { get; set; }
    public Guid SubfamilyId { get; set; }
    public Guid TypeId { get; set; }
    public Guid ClassId { get; set; }
    public Guid MeasurementUnitId { get; set; }
    public Guid SizeUnitId { get; set; }
    public Guid? CommissionGroupId { get; set; }
    public Guid? DiscountGroupId { get; set; }
    public Guid? VatOnTableId { get; set; }
    public Guid VatDirectSellingId { get; set; }
    public Guid? VatExemptionReasonId { get; set; }
    public bool IsComposed { get; set; }
    public bool UniqueArticles { get; set; }
    public bool IsSdrPackaging { get; set; }
    public string? Notes { get; set; }
    public string? BarcodeLabelPrintModel { get; set; }
}

public sealed class UpdateArticleRequest
{
    public uint Order { get; set; }
    public string? Code { get; set; }
    public string? CodeDealer { get; set; }
    public string Designation { get; set; } = string.Empty;
    public ButtonResponse? Button { get; set; }
    public ArticlePriceRequest? Price1 { get; set; }
    public ArticlePriceRequest? Price2 { get; set; }
    public ArticlePriceRequest? Price3 { get; set; }
    public ArticlePriceRequest? Price4 { get; set; }
    public ArticlePriceRequest? Price5 { get; set; }
    public bool PriceWithVat { get; set; }
    public decimal Discount { get; set; }
    public decimal DefaultQuantity { get; set; }
    public decimal TotalStock { get; set; }
    public decimal MinimumStock { get; set; }
    public decimal Tare { get; set; }
    public float Weight { get; set; }
    public string? Barcode { get; set; }
    public bool PVPVariable { get; set; }
    public bool Favorite { get; set; }
    public bool UseWeighingBalance { get; set; }
    public Guid? SubfamilyId { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? MeasurementUnitId { get; set; }
    public Guid? SizeUnitId { get; set; }
    public Guid? CommissionGroupId { get; set; }
    public Guid? DiscountGroupId { get; set; }
    public Guid? VatOnTableId { get; set; }
    public Guid? VatDirectSellingId { get; set; }
    public Guid? VatExemptionReasonId { get; set; }
    public bool IsComposed { get; set; }
    public bool UniqueArticles { get; set; }
    public bool IsSdrPackaging { get; set; }
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; }
    public string? BarcodeLabelPrintModel { get; set; }
}
