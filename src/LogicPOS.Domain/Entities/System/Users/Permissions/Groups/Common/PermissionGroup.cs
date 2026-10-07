using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("sys_userpermissiongroup")]
public class PermissionGroup : Entity.WithCode.AndOrder.AndDesignation
{
    public async Task<Result> UpdateAsync(UpdatePermissionGroupDto dto,
                                          IPermissionGroupRepository repository,
                                          CancellationToken cancellationToken = default)
    {
        var updater = new PermissionGroupUpdater(this, dto, repository);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<PermissionGroup>> CreateAsync(CreatePermissionGroupDto dto,
                                                                  IPermissionGroupRepository repository,
                                                                  CancellationToken cancellationToken = default)
    {
        var creator = new PermissionGroupCreator(dto, repository);
        return await  creator.CreateAsync(cancellationToken);
    }
}
