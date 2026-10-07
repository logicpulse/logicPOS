using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;


internal class HolidayUpdater : EntityUpdater<Holiday>
{
    private readonly UpdateHolidayDto _dto;

    public HolidayUpdater(Holiday entity,
                          UpdateHolidayDto dto,
                          IHolidayRepository repository) : base(entity,repository)
    {
        _dto = dto;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForCodeAndDesignationConflictsAsync(_dto.Code, _dto.Designation, ct);
    }

    protected override Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }

    protected override void SetNewData()
    {
        _entity.Code = _dto.Code;
        _entity.Order = _dto.Order;
        _entity.Designation = _dto.Designation;
        _entity.Year = _dto.Year;
        _entity.Month = _dto.Month;
        _entity.Day = _dto.Day;
        _entity.Fixed = _dto.Fixed;
        _entity.Description = _dto.Description;
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted;
    }
}