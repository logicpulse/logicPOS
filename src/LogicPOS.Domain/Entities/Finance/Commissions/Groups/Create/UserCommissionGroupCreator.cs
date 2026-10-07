using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class CommissionGroupCreator : EntityCreator<CommissionGroup>
{
    private readonly ICommissionGroupRepository _repository;
    private readonly CreateUserCommissionGroupDto _dto;

    public CommissionGroupCreator(ICommissionGroupRepository repository,
                                  CreateUserCommissionGroupDto dto) : base(new CommissionGroup())
    {
        _repository = repository;
        _dto = dto;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Order = await _repository.GetNextOrderAsync(ct);
        _entity.Designation = _dto.Designation;
        _entity.Commission = _dto.Commission;
        _entity.Code = await _repository.GetNextCodeAsync(ct);
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