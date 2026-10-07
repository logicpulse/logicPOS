using LogicPOS.Domain.Repositories;

namespace LogicPOS.Domain.Entities;

public record UserReferences(IUserProfileRepository UserProfileRepository,
                             ICommissionGroupRepository CommissionGroupRepository)
{
    public async Task<bool> UserProfileExistsAsync(Guid id,
                                                   CancellationToken cancellationToken = default)
    {
        return await UserProfileRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> CommissionGroupExistsAsync(Guid id,
                                                       CancellationToken cancellationToken = default)
    {
        return await CommissionGroupRepository.ExistsAsync(id, cancellationToken);
    }
}   
