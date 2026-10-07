using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class PreferenceParameterUpdater : EntityUpdater<PreferenceParameter>
{
    private readonly UpdatePreferenceParameterDto _dto;

    public PreferenceParameterUpdater(PreferenceParameter entity,
                                      UpdatePreferenceParameterDto dto,
                                      IPreferenceParameterRepository repository) : base(entity,repository)
    {
        _dto = dto;
    }



    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        var codeResult  = await CheckForCodeConflictAsync(_dto.Code,ct);
        if (codeResult.IsFailure)
        {
            return codeResult;
        }

        return await CheckForTokenConflictAsync(ct);
    }

    private Task<Result> CheckForTokenConflictAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Success());
    }

    protected override Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }

    protected override void SetNewData()
    {
        _entity.Code = _dto.Code;
        _entity.Order = _dto.Order;
        _entity.Value = _dto.Value;
        _entity.Notes = _dto.Notes;
    }
}