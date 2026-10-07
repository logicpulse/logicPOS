namespace LogicPOS.Core.FrontOffice;

public sealed class PosArticle
{
    public PosArticle(
        Guid id,
        Guid subfamilyId,
        string text,
        string designation,
        string code,
        string? barcode,
        bool favorite,
        decimal catalogPrice,
        bool priceIncludesVat,
        decimal vatPercentage,
        decimal defaultQuantity,
        decimal discount,
        Guid vatRateId)
    {
        Id = id;
        SubfamilyId = subfamilyId;
        Text = text;
        Designation = designation;
        Code = code;
        Barcode = barcode;
        Favorite = favorite;
        CatalogPrice = catalogPrice;
        PriceIncludesVat = priceIncludesVat;
        VatPercentage = vatPercentage;
        DefaultQuantity = defaultQuantity;
        Discount = discount;
        VatRateId = vatRateId;
    }

    public Guid Id { get; }

    public Guid SubfamilyId { get; }

    public string Text { get; }

    public string Designation { get; }

    public string Code { get; }

    public string? Barcode { get; }

    public bool Favorite { get; }

    public decimal CatalogPrice { get; }

    public bool PriceIncludesVat { get; }

    public decimal VatPercentage { get; }

    public decimal DefaultQuantity { get; }

    public decimal Discount { get; }

    public Guid VatRateId { get; }

    public decimal QuantityStep => DefaultQuantity > 0 ? DefaultQuantity : 1m;
}
