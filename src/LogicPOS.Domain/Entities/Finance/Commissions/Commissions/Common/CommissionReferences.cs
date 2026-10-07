using LogicPOS.Domain.Repositories;

namespace LogicPOS.Domain.Entities;

public record CommissionReferences(IUserRepository UserRepository,
                                   ICommissionGroupRepository CommissionGroupRepository,
                                   IDocumentDetailRepository DocumentDetailRepository)
{
    public async Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await UserRepository.ExistsAsync(userId, cancellationToken);
    }

    public async Task<bool> CommissionGroupExistsAsync(Guid commissionGroupId, CancellationToken cancellationToken = default)
    {
        return await CommissionGroupRepository.ExistsAsync(commissionGroupId, cancellationToken);
    }

    public async Task<bool> DocumentDetailExistsAsync(Guid documentDetailId, CancellationToken cancellationToken = default)
    {
        return await DocumentDetailRepository.ExistsAsync(documentDetailId, cancellationToken);
    }
}