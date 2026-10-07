using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class FiscalYearCreator : EntityCreator<FiscalYear>
{
    private readonly CreateFiscalYearDto _dto;
    private readonly IFiscalYearRepository _repository;

    public FiscalYearCreator(CreateFiscalYearDto dto,
                             IFiscalYearRepository repository) : base( new())
    {
        _dto = dto;
        _repository = repository;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Order = await _repository.GetNextOrderAsync(ct);
        _entity.Designation  = _dto.Designation;
        _entity.Acronym = _dto.Acronym;
        _entity.Year = _dto.Year;
        _entity.SeriesForEachTerminal = _dto.SeriesForEachTerminal;
        _entity.Code = await _repository.GetNextCodeAsync(ct);
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = false;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if (await _repository.AcronymExistsAsync(_dto.Acronym, ct))
        {
            return Result.Conflict($"Acrônimo '{_dto.Acronym}' já existe.");
        }
        return await CheckForDesignationConflictAsync(_repository, _dto.Designation, ct);
    }

    protected override Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }
}