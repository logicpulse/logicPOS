using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class VatRateCreator : EntityCreator<VatRate>
{
    private readonly CreateVatRateDto _dto;
    private readonly IVatRateRepository _repository;

    public VatRateCreator(CreateVatRateDto dto,
                          IVatRateRepository repository) : base(new())
    {
        _dto = dto;
        _repository = repository;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Order = await _repository.GetNextOrderAsync(ct);
        _entity.Designation = _dto.Designation;
        _entity.TaxCode = _dto.TaxCode;
        _entity.TaxType = _dto.TaxType;
        _entity.Description = _dto.Description;
        _entity.ExpirationDate = _dto.ExpirationDate;
        _entity.CountryRegion = _dto.CountryRegion;
        _entity.Value = _dto.Value;
        _entity.ReasonCode = _dto.ReasonCode;
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