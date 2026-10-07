using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Finance.Documents.Details.Common;

namespace LogicPOS.Domain.Entities;

/// <summary>
/// In-memory catalog for document line creation (avoids N+1 repository hits).
/// </summary>
public sealed class DocumentDetailLookup
{
    public IReadOnlyDictionary<Guid, ArticleDetail> Articles { get; }
    public IReadOnlyDictionary<Guid, VatRate> VatRates { get; }
    public IReadOnlyDictionary<Guid, VatExemptionReason> Exemptions { get; }

    public DocumentDetailLookup(
        IReadOnlyDictionary<Guid, ArticleDetail> articles,
        IReadOnlyDictionary<Guid, VatRate> vatRates,
        IReadOnlyDictionary<Guid, VatExemptionReason> exemptions)
    {
        Articles = articles;
        VatRates = vatRates;
        Exemptions = exemptions;
    }

    public static async Task<DocumentDetailLookup> LoadAsync(
        IEnumerable<CreateDocumentDetailDto> details,
        DocumentDetailReferences references,
        CancellationToken ct = default)
    {
        var detailList = details as IList<CreateDocumentDetailDto> ?? details.ToList();
        var articleIds = detailList.Select(d => d.ArticleId).Distinct().ToList();

        var articles = articleIds.Count == 0
            ? new Dictionary<Guid, ArticleDetail>()
            : await references.ArticleRepository.GetArticleDetailsByIdsAsync(articleIds, ct);

        var vatRateIds = detailList
            .Where(d => d.VatRateId.HasValue)
            .Select(d => d.VatRateId!.Value)
            .Concat(articles.Values.Select(a => a.VatRateId))
            .Distinct()
            .ToList();

        var vatRates = vatRateIds.Count == 0
            ? new Dictionary<Guid, VatRate>()
            : await references.VatRateRepository.GetByIdsAsync(vatRateIds, ct);

        var exemptionIds = detailList
            .Where(d => d.VatExemptionId.HasValue)
            .Select(d => d.VatExemptionId!.Value)
            .Concat(articles.Values
                .Where(a => a.VatExemptionReasonId.HasValue)
                .Select(a => a.VatExemptionReasonId!.Value))
            .Distinct()
            .ToList();

        var exemptions = exemptionIds.Count == 0
            ? new Dictionary<Guid, VatExemptionReason>()
            : await references.VatExemptionRepository.GetByIdsAsync(exemptionIds, ct);

        return new DocumentDetailLookup(articles, vatRates, exemptions);
    }
}
