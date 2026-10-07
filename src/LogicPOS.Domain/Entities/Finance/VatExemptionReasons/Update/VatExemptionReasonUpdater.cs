using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class VatExemptionReasonUpdater : EntityUpdater<VatExemptionReason>
{
    private readonly UpdateVatExemptionReasonDto _dto;

    public VatExemptionReasonUpdater(VatExemptionReason entity,
                                     UpdateVatExemptionReasonDto dto,
                                     IVatExemptionReasonRepository repository) : base(
            entity, 
            repository)
    {
        _dto = dto;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForCodeConflictAsync(
            _dto.Code, 
            ct);
    }

    protected override Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }

    protected override void SetNewData()
    {
        _entity.Order = _dto.Order;
        _entity.Code = _dto.Code;
        _entity.Designation = _dto.Designation;
        _entity.Acronym = _dto.Acronym;
        _entity.StandardApplicable = _dto.StandardApplicable;
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted;
    }
}