using LogicPOS.Domain.Entities.Dtos;

namespace LogicPOS.Domain.Entities.Finance.Documents.Documents.Details.Sdr;

public static class SdrDepositDetailFactory
{
    public static CreateDocumentDetailDto Create(
        SdrDepositArticleSnapshot depositArticle,
        decimal quantity,
        string country2Code)
        => new(
            ArticleId: depositArticle.Id,
            Quantity: quantity,
            Price: depositArticle.UnitPrice,
            VatExemptionId: depositArticle.VatExemptionId,
            VatRateId: depositArticle.VatRateId,
            Discount: 0,
            PriceType: null,
            Notes: null,
            Country2Code: country2Code,
            SerialNumber: null);
}
