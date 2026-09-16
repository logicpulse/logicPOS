namespace LogicPOS.ApiServer.Data.Entities;

public sealed class ApiArticle
{
    public Guid Id { get; set; }
    public uint Order { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? CodeDealer { get; set; }
    public string Designation { get; set; } = string.Empty;

    public Guid ClassId { get; set; }
    public Guid SubfamilyId { get; set; }
    public Guid TypeId { get; set; }
    public Guid MeasurementUnitId { get; set; }
    public Guid SizeUnitId { get; set; }
    public Guid VatDirectSellingId { get; set; }

    // CommissionGroups/DiscountGroups/VatExemptionReasons catalogs are not built yet; stored but not validated/resolved.
    public Guid? CommissionGroupId { get; set; }
    public Guid? DiscountGroupId { get; set; }
    public Guid? VatOnTableId { get; set; }
    public Guid? VatExemptionReasonId { get; set; }

    public string? ButtonLabel { get; set; }
    public string? ButtonImage { get; set; }

    public decimal Price1Value { get; set; }
    public decimal Price1PromotionValue { get; set; }
    public bool Price1UsePromotion { get; set; }
    public decimal Price2Value { get; set; }
    public decimal Price2PromotionValue { get; set; }
    public bool Price2UsePromotion { get; set; }
    public decimal Price3Value { get; set; }
    public decimal Price3PromotionValue { get; set; }
    public bool Price3UsePromotion { get; set; }
    public decimal Price4Value { get; set; }
    public decimal Price4PromotionValue { get; set; }
    public bool Price4UsePromotion { get; set; }
    public decimal Price5Value { get; set; }
    public decimal Price5PromotionValue { get; set; }
    public bool Price5UsePromotion { get; set; }

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

    public string Notes { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
