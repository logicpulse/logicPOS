using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Enums;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class TableUpdater : EntityUpdater<Table>
{
    private readonly UpdateTableDto _dto;
    private readonly IPlaceRepository _placeRepository;

    public TableUpdater(Table entity,
                        UpdateTableDto dto,
                        ITableRepository repository,
                        IPlaceRepository placeRepository) :
        base(entity,repository)
    {
        _dto = dto;
        _placeRepository = placeRepository;
    }


    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForCodeAndDesignationConflictsAsync(_dto.Code, _dto.Designation, ct);
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (_dto.PlaceId != _entity.PlaceId &&
            !await _placeRepository.ExistsAsync(_dto.PlaceId, ct))
        {
            return Result.NotFound(nameof(Place), _dto.PlaceId.ToString());
        }

        return Result.Success();
    }

    protected override void SetNewData()
    {
        _entity.Code = _dto.Code;
        _entity.Order = _dto.Order;
        _entity.PlaceId = _dto.PlaceId;
        _entity.Designation = _dto.Designation;
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted;
    }
}