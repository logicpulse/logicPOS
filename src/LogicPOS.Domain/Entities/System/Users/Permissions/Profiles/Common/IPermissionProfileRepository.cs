namespace LogicPOS.Domain.Repositories;

public interface IPermissionProfileRepository
{
    public Task<bool> ProfileExistsAsync(Guid userProfileId, Guid permissionItemId, CancellationToken cancellationToken = default);
}
