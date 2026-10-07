using LogicPOS.Domain.Repositories;

namespace LogicPOS.Domain.Entities.Dtos;

public record ArticleSubfamilyReferences(IArticleFamilyRepository FamilyRepository,
                                         ICommissionGroupRepository CommissionGroupRepository,
                                         IDiscountGroupRepository DiscountGroupRepository,
                                         IVatRateRepository VatRateRepository)
{
    public async Task<bool> FamilyExistsAsync(Guid id,
                                              CancellationToken cancellationToken = default)
    {
        return await FamilyRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> CommissionGroupExistsAsync(Guid id,
                                                       CancellationToken cancellationToken = default)
    {
        return await CommissionGroupRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> DiscountGroupExistsAsync(Guid id,
                                                     CancellationToken cancellationToken = default)
    {
        return await DiscountGroupRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> VatRateExistsAsync(Guid id,
                                               CancellationToken cancellationToken = default)
    {
        return await VatRateRepository.ExistsAsync(id, cancellationToken);
    }
}