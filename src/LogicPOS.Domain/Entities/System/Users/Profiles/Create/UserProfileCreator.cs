using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class UserProfileCreator : EntityCreator<UserProfile>
{
    private readonly CreateUserProfileDto _dto;
    private readonly IUserProfileRepository _repository;

    public UserProfileCreator(CreateUserProfileDto dto,
                              IUserProfileRepository repository) : base(new UserProfile())
    {
        _repository = repository;
        _dto = dto;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Order = await _repository.GetNextOrderAsync(ct);
        _entity.Designation = _dto.Designation;
        _entity.Code = await _repository.GetNextCodeAsync(ct);
        _entity.AccessPassword = _dto.AccessPassword;
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted ?? false;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForDesignationConflictAsync(_repository, _dto.Designation, ct);
    }

    protected override Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }
}