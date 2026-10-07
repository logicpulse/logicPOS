using LogicPOS.Domain.Repositories;

namespace LogicPOS.Domain.Entities;

public record ArticleFamilyReferences(
    ICommissionGroupRepository CommissionGroupRepository,
    IDiscountGroupRepository DiscountGroupRepository)
{
    public async Task<bool> CommissionGroupExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await CommissionGroupRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> DiscountGroupExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DiscountGroupRepository.ExistsAsync(id, cancellationToken);
    }

}