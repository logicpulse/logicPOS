using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("sys_userpermissionprofile")]
public class PermissionProfile : Entity
{
    public bool Granted { get; set; }

    public UserProfile? UserProfile { get; set; }
    public Guid UserProfileId { get; set; }

    public PermissionItem? PermissionItem { get; set; }
    public Guid PermissionItemId { get; set; }

public static Task<Result<PermissionProfile>> CreateAsync(CreatePermissionProfileDto dto,
                                                          PermissionProfileReferences permissionProfileReferences,
                                                          IPermissionProfileRepository permissionProfileRepository,
                                                          CancellationToken cancellationToken = default)
    {
        var creator = new PermissionProfileCreator(dto, permissionProfileRepository, permissionProfileReferences);
        return creator.CreateAsync(cancellationToken);
    }

}
