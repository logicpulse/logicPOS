using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class PrefereneParameterCreator : EntityCreator<PreferenceParameter>
{
    private readonly CreatePreferenceParameterDto _dto;
    private readonly IPreferenceParameterRepository _repository;

    public PrefereneParameterCreator(CreatePreferenceParameterDto dto,
                                     IPreferenceParameterRepository repository) :
        base(new())
    {
        _dto = dto;
        _repository = repository;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Order = await _repository.GetNextOrderAsync(ct);
        _entity.Code = await _repository.GetNextCodeAsync(ct);
        _entity.Token = _dto.Token;
        _entity.Value = _dto.Value;
        _entity.ValueTip = _dto.ValueTip;
        _entity.Required = _dto.Required;
        _entity.RegEx = _dto.RegEx;
        _entity.ResourceString = _dto.ResourceString;
        _entity.ResourceStringInfo = _dto.ResourceStringInfo;
        _entity.FormType = _dto.FormType;
        _entity.FormPageNo = _dto.FormPageNo;
        _entity.InputType = (Enums.PreferenceParameterInputType)_dto.InputType;
        _entity.Code = await _repository.GetNextCodeAsync(ct);
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted ?? false;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if (await _repository.TokenExistsAsync(_entity.Token, ct))
        {
            return Result.Conflict("O token já existe");
        }

        return Result.Success();
    }

    protected override Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }
}