using LogicPOS.Domain.Repositories;

namespace LogicPOS.Domain.Entities;

public record PermissionProfileReferences(IUserProfileRepository UserProfileRepository,
    IPermissionItemRepository PermissionItemRepository)
{
    public async Task<bool> UserProfileExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await UserProfileRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> PermissionItemExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await PermissionItemRepository.ExistsAsync(id, cancellationToken);
    }
}