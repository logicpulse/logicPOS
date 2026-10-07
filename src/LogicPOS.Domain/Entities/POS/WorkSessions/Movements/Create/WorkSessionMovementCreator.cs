using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class WorkSessionMovementCreator : EntityCreator<WorkSessionMovement>
{
    private readonly CreateWorkSessionMovementDto _dto;
    private readonly WorkSessionMovementReferences _references;
    public WorkSessionMovementCreator(CreateWorkSessionMovementDto dto,
                                      WorkSessionMovementReferences references) : base(new())
    {
        _dto = dto;
        _references = references;
    }

    protected override Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Type = _dto.Type;
        _entity.PeriodId = _dto.PeriodId;
        _entity.Amount = _dto.Amount;
        _entity.DocumentId = _dto.DocumentId;
        _entity.PaymentId = _dto.PaymentId;
        _entity.Notes = _dto.Notes;

        return Task.CompletedTask;
    }

    protected override Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {

        if (!await _references.PeriodExistsAsync(_dto.PeriodId, ct))
        {
            return Result.NotFound<WorkSessionPeriod>(_dto.PeriodId.ToString());
        }

        if (_dto.DocumentId.HasValue && !await _references.DocumentExistsAsync(_dto.DocumentId.Value, ct))
        {
            return Result.NotFound<Document>(_dto.DocumentId.Value.ToString());
        }

        if (_dto.PaymentId.HasValue && !await _references.PaymentExistsAsync(_dto.PaymentId.Value, ct))
        {
            return Result.NotFound<Payment>(_dto.PaymentId.Value.ToString());
        }

        return Result.Success();
    }
}