using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class CurrencyCreator : EntityCreator<Currency>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly CreateCurrencyDto _dto;

    public CurrencyCreator(CreateCurrencyDto dto,
                           ICurrencyRepository currencyRepository) : base( new())
    {
        _currencyRepository = currencyRepository;
        _dto = dto;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Designation = _dto.Designation;
        _entity.Acronym = _dto.Acronym;
        _entity.Symbol = _dto.Symbol;
        _entity.Entity = _dto.Entity;
        _entity.ExchangeRate = _dto.ExchangeRate;
        _entity.Code = await _currencyRepository.GetNextCodeAsync(ct);
        _entity.Order = await _currencyRepository.GetNextOrderAsync(ct);
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted ?? false;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForDesignationConflictAsync(_currencyRepository, _dto.Designation, ct);
    }

    protected override Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }
}