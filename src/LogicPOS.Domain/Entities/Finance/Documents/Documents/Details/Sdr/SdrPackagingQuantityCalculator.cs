using LogicPOS.Domain.Entities.Dtos;

namespace LogicPOS.Domain.Entities.Finance.Documents.Documents.Details.Sdr;

public static class SdrPackagingQuantityCalculator
{
    public static decimal CountUnits(
        IEnumerable<CreateDocumentDetailDto> details,
        IReadOnlyDictionary<Guid, bool> packagingByArticleId,
        Guid depositArticleId)
    {
        decimal total = 0;

        foreach (var detail in details)
        {
            if (detail.ArticleId == depositArticleId)
            {
                continue;
            }

            if (!packagingByArticleId.TryGetValue(detail.ArticleId, out var isPackaging) || !isPackaging)
            {
                continue;
            }

            total += detail.Quantity;
        }

        return total;
    }
}
