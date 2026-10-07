using LogicPOS.Domain.Repositories;

namespace LogicPOS.Domain.Entities;

public record ArticleReferences(IArticleClassRepository ArticleClassRepository,
                                IArticleSubfamilyRepository ArticleSubfamilyRepository,
                                IArticleTypeRepository ArticleTypeRepository,
                                IMeasurementUnitRepository MeasurementUnitRepository,
                                ISizeUnitRepository SizeUnitRepository,
                                ICommissionGroupRepository CommissionGroupRepository,
                                IVatRateRepository VatRateRepository,
                                IVatExemptionReasonRepository VatExemptionReasonRepository,
                                IDiscountGroupRepository DiscountGroupRepository)
{
    public async Task<bool> CommissionGroupExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await CommissionGroupRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> ClassExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await ArticleClassRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> SubfamilyExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await ArticleSubfamilyRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> TypeExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await ArticleTypeRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> MeasurementUnitExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await MeasurementUnitRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> SizeUnitExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await SizeUnitRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> VatRateExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await VatRateRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> VatExemptionReasonExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await VatExemptionReasonRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> DiscountGroupExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DiscountGroupRepository.ExistsAsync(id, cancellationToken);
    }

}