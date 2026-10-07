using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class WarehouseUpdater : EntityUpdater<Warehouse>
{
    private readonly UpdateWarehouseDto _dto;
    public WarehouseUpdater(Warehouse warehouse,
                            IWarehouseRepository repository,
                            UpdateWarehouseDto dto) : base( entity: warehouse, repository: repository)
    {
        _dto = dto;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForCodeAndDesignationConflictsAsync(_dto.Code,
                                                              _dto.Designation,
                                                              ct);
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
        _entity.IsDefault = _dto.IsDefault;
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted;
    }
}
