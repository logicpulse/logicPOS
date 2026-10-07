using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class PermissionProfileCreator : EntityCreator<PermissionProfile>
{
    private readonly CreatePermissionProfileDto _dto;
    private readonly IPermissionProfileRepository _repository;
    private readonly PermissionProfileReferences _references;

    public PermissionProfileCreator(CreatePermissionProfileDto dto,
                                    IPermissionProfileRepository repository,
                                    PermissionProfileReferences references) : base(new())
    {
        _dto = dto;
        _repository = repository;
        _references = references;
    }


    protected override Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.PermissionItemId = _dto.PermissionItemId;
        _entity.UserProfileId = _dto.UserProfileId;
        _entity.Granted = true;
        return Task.CompletedTask;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if(await _repository.ProfileExistsAsync(_dto.UserProfileId, _dto.PermissionItemId, ct))
        {
            return Result.Conflict("Perfil de permissão já existe");
        }

        return Result.Success();
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        var userProfileExists = await _references.UserProfileExistsAsync(_dto.UserProfileId, ct);
        if(userProfileExists == false)
        {
            return Result.NotFound(nameof(UserProfile),_dto.UserProfileId.ToString());
        }

        var permissionItemExists = await _references.PermissionItemExistsAsync(_dto.PermissionItemId, ct);
        if (permissionItemExists == false)
        {
            return Result.NotFound(nameof(PermissionItem), _dto.PermissionItemId.ToString());
        }

        return Result.Success();
    }
}