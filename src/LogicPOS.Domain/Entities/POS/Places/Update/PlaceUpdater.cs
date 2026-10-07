using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class PlaceUpdater : EntityUpdater<Place>
{
    private readonly UpdatePlaceDto _dto;
    private readonly PlaceReferences _references;

    public PlaceUpdater(Place entity,
                        UpdatePlaceDto dto,
                        IPlaceRepository repository,
                        PlaceReferences references) : base(entity,repository)
    {
        _dto = dto;
        _references = references;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForCodeAndDesignationConflictsAsync(_dto.Code,
                                                              _dto.Designation,
                                                              ct);
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (_dto.MovementTypeId != Guid.Empty &&
            _dto.MovementTypeId != _entity.MovementTypeId &&
            !await _references.MovementTypeExistsAsync(_dto.MovementTypeId, ct))
        {
            return Result.NotFound(nameof(MovementType), _dto.MovementTypeId.ToString());
        }

        if (_dto.PriceTypeId != _entity.PriceTypeId &&
            !await _references.PriceTypeExistsAsync(_dto.PriceTypeId, ct))
        {
            return Result.NotFound(nameof(PriceType), _dto.PriceTypeId.ToString());
        }

        return Result.Success();
    }

    protected override void SetNewData()
    {
        _entity.Code = _dto.Code;
        _entity.Order = _dto.Order;
        _entity.Designation = _dto.Designation;
       
        _entity.MovementTypeId = _dto.MovementTypeId == Guid.Empty ? null : _dto.MovementTypeId;

        _entity.PriceTypeId = _dto.PriceTypeId;
        _entity.Notes = _dto.Notes;
        _entity.ButtonImage = _dto.ButtonImage;
        _entity.TypeSubtotal = _dto.TypeSubtotal;
        _entity.AccountType = _dto.AccountType;
        _entity.OrderPrintMode = (Enums.OrderPrintMode?) _dto.OrderPrintMode;
        _entity.IsDeleted = _dto.IsDeleted;
    }
}