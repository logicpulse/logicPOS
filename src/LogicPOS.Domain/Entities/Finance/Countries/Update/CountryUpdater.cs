using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class CountryUpdater : EntityUpdater<Country>
{
    private readonly UpdateCountryDto _dto;
    public CountryUpdater(Country country,
                          UpdateCountryDto dto,
                          ICountryRepository repository) : base(country,repository)
    {
        _dto = dto;
    }

    private async Task<bool> Code2ExistsAsync()
    {
        if (_dto.Code2 is null || _dto.Code2 == _entity.Code2)
        {
            return false;
        }

        return await (_repository as ICountryRepository)!.Code2ExistsAsync(_dto.Code2);
    }

    private async Task<bool> Code3ExistsAsync()
    {
        if (_dto.Code3 is null || _dto.Code3 == _entity.Code3)
        {
            return false;
        }

        return await (_repository as ICountryRepository)!.Code3ExistsAsync(_dto.Code3);
    }

    protected override void SetNewData()
    {
        _entity.Order = _dto.Order;
        _entity.Code = _dto.Code;
        _entity.Designation = _dto.Designation;
        _entity.Code2 = _dto.Code2;
        _entity.Code3 = _dto.Code3;
        _entity.Capital = _dto.Capital;
        _entity.TLD = _dto.TLD;
        _entity.Currency = _dto.Currency;
        _entity.CurrencyCode = _dto.CurrencyCode;
        _entity.FiscalNumberRegex = _dto.FiscalNumberRegex;
        _entity.ZipCodeRegex = _dto.ZipCodeRegex;
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if (await Code2ExistsAsync())
        {
            return Result.Conflict("O Code2 já existe");
        }

        if (await Code3ExistsAsync())
        {
            return Result.Conflict("O Code3 já existe");
        }

        return await CheckForCodeAndDesignationConflictsAsync(
            _dto.Code,
            _dto.Designation, 
            ct);
    }

    protected override Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }
}
