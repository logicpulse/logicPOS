using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Errors;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class CountryCreator : EntityCreator<Country>
{
    private readonly ICountryRepository _repository;
    private readonly CreateCountryDto _dto;

    public CountryCreator(ICountryRepository repository,
                          CreateCountryDto dto) : base(new Country())
    {
        _repository = repository;
        _dto = dto;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if (await _repository.DesignationExistsAsync(_dto.Designation, ct))
        {
            return Result.Failure(Error.DesignationExistsError);
        }

        if (await _repository.Code2ExistsAsync(_dto.Code2, ct))
        {
            return Result.Conflict("O Code2 já existe");
        }

        if (await _repository.Code3ExistsAsync(_dto.Code3, ct))
        {
            return Result.Conflict("O Code3 já existe");
        }

        return Result.Success();
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Designation = _dto.Designation;
        _entity.Code2 = _dto.Code2;
        _entity.Code3 = _dto.Code3;
        _entity.Capital = _dto.Capital;
        _entity.TLD = _dto.TLD;
        _entity.Currency = _dto.Currency;
        _entity.CurrencyCode = _dto.CurrencyCode;
        _entity.FiscalNumberRegex = _dto.FiscalNumberRegex;
        _entity.ZipCodeRegex = _dto.ZipCodeRegex;
        _entity.Order = await _repository.GetNextOrderAsync(ct);
        _entity.Code = await _repository.GetNextCodeAsync(ct);
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted ?? false;
    }

    protected override Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }
}
