using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class CommissionCreator : EntityCreator<Commission>
{
    private readonly CreateCommissionDto _dto;
    private readonly CommissionReferences _references;

    public CommissionCreator(CreateCommissionDto dto,
                             CommissionReferences references) : base(new())
    {
        _dto = dto;
        _references = references;
    }

    protected override Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.UserId = _dto.UserId;
        _entity.CommissionGroupId = _dto.CommissionGroupId;
        _entity.DocumentDetailId = _dto.DocumentDetailId;
        _entity.CommissionValue = _dto.CommissionValue;
        _entity.Total = _dto.Total;
        _entity.Notes = _dto.Notes;
        return Task.CompletedTask;
    }

    protected override Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (!await _references.UserExistsAsync(_dto.UserId, ct))
        {
            return Result.NotFound(nameof(SizeUnit), _dto.UserId.ToString());
        }

        if (!await _references.CommissionGroupExistsAsync(_dto.CommissionGroupId, ct))
        {
            return Result.NotFound(nameof(CommissionGroup), _dto.CommissionGroupId.ToString());
        }

        if (!await _references.DocumentDetailExistsAsync(_dto.DocumentDetailId, ct))
        {
            return Result.NotFound(nameof(DocumentDetail), _dto.DocumentDetailId.ToString());
        }

        return Result.Success();
    }
}