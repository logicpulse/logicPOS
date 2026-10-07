using LogicPOS.Domain.Repositories;

namespace LogicPOS.Domain.Entities;

public record DocumentDetailReferences(
    IArticleRepository ArticleRepository,
    IVatRateRepository VatRateRepository,
    IVatExemptionReasonRepository VatExemptionRepository,
    DocumentDetailLookup? Lookup = null)
{
    public async Task<bool> ArticleExistsAsync(Guid articleId, CancellationToken cancellationToken = default)
    {
        if (Lookup is not null)
        {
            return Lookup.Articles.ContainsKey(articleId);
        }

        return await ArticleRepository.ExistsAsync(articleId, cancellationToken);
    }

    public async Task<bool> VatRateExistsAsync(Guid vatRateId, CancellationToken cancellationToken = default)
    {
        if (Lookup is not null)
        {
            return Lookup.VatRates.ContainsKey(vatRateId);
        }

        return await VatRateRepository.ExistsAsync(vatRateId, cancellationToken);
    }

    public async Task<bool> VatExemptionExistsAsync(Guid vatExemptionId, CancellationToken cancellationToken = default)
    {
        if (Lookup is not null)
        {
            return Lookup.Exemptions.ContainsKey(vatExemptionId);
        }

        return await VatExemptionRepository.ExistsAsync(vatExemptionId, cancellationToken);
    }
}
