using LogicPOS.Domain.Entities.Dtos;

namespace LogicPOS.Domain.Entities.Finance.Documents.Documents.Details.Sdr;

public static class DocumentDetailsSdrEnricher
{
    public static IReadOnlyList<CreateDocumentDetailDto> Enrich(
        IReadOnlyList<CreateDocumentDetailDto> details,
        IReadOnlyDictionary<Guid, bool> packagingByArticleId,
        SdrDepositArticleSnapshot? depositArticle)
    {
        var withoutDeposit = RemoveDepositLines(details, depositArticle?.Id);

        if (depositArticle is null)
        {
            return withoutDeposit;
        }

        var packagingUnits = SdrPackagingQuantityCalculator.CountUnits(
            withoutDeposit,
            packagingByArticleId,
            depositArticle.Id);

        if (packagingUnits <= 0)
        {
            return withoutDeposit;
        }

        var countryCode = ResolveCountryCode(withoutDeposit);
        var depositLine = SdrDepositDetailFactory.Create(depositArticle, packagingUnits, countryCode);

        return withoutDeposit.Append(depositLine).ToList();
    }

    private static List<CreateDocumentDetailDto> RemoveDepositLines(
        IReadOnlyList<CreateDocumentDetailDto> details,
        Guid? depositArticleId)
    {
        if (depositArticleId is null)
        {
            return details.ToList();
        }

        return details.Where(d => d.ArticleId != depositArticleId.Value).ToList();
    }

    private static string ResolveCountryCode(IReadOnlyList<CreateDocumentDetailDto> details)
        => details.FirstOrDefault()?.Country2Code ?? string.Empty;
}
