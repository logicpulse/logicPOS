namespace LogicPOS.Domain.Entities.Finance.Documents.Details.Common;

public record ArticleDetail
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public uint Order { get; set; }
    public string Designation { get; set; } = null!;
    public Guid? VatExemptionReasonId { get; set; }
    public string Unit { get; set; } = null!;
    public decimal Discount { get; set; }
    public decimal Vat { get; set; }
    public string? Notes { get; set; }
    public Guid VatRateId { get; set; }
    public bool PriceWithVat { get; set; }
    public string ClassAcronym { get; set; } = null!;

    public decimal Price5 { get; set; }

    public decimal Price4 { get; set; }

    public decimal Price3 { get; set; }

    public decimal Price2 { get; set; }

    public decimal Price1 { get; set; }
    
    public decimal GetPriceByType(Enums.PriceType priceType)
    {
        switch (priceType)
        {
            case Enums.PriceType.Price1:
                return Price1;
            case Enums.PriceType.Price2:
                return Price2;
            case Enums.PriceType.Price3:
                return Price3;
            case Enums.PriceType.Price4:
                return Price4;
            case Enums.PriceType.Price5:
                return Price5;
            default:
                throw new ArgumentOutOfRangeException(nameof(priceType));
        }
    }
}