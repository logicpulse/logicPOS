namespace LogicPOS.Application.Features.System.Users;

public interface IUserProfilePermissionsService
{
    public bool UserProfileHasPermission(Guid userProfileId, string permissionToken);
}