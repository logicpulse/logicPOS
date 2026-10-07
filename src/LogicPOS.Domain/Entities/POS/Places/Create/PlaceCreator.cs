using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class PlaceCreator : EntityCreator<Place>
{
    private readonly CreatePlaceDto _dto;
    private readonly IPlaceRepository _repository;
    private readonly PlaceReferences _references;

    public PlaceCreator(CreatePlaceDto dto,
                        IPlaceRepository repository,
                        PlaceReferences references) : base(new())
    {
        _dto = dto;
        _repository = repository;
        _references = references;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Order = _dto.Imported ? 0 : await _repository.GetNextOrderAsync(ct);
        _entity.Designation = _dto.Designation;
        _entity.Code = _dto.Imported ? "?" : await _repository.GetNextCodeAsync(ct);
        _entity.Notes = _dto.Notes;
        _entity.PriceTypeId = _dto.PriceTypeId;
        _entity.MovementTypeId = _dto.MovementTypeId;
        _entity.AccountType = _dto.AccountType;
        _entity.ButtonImage = _dto.ButtonImage;
        _entity.TypeSubtotal = _dto.TypeSubtotal;
        _entity.OrderPrintMode = (Enums.OrderPrintMode?)_dto.OrderPrintMode;
        _entity.IsDeleted = _dto.IsDeleted ?? false;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if (_dto.Imported)
        {
            return Result.Success();
        }
        return await CheckForDesignationConflictAsync(_repository, _dto.Designation, ct);
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (!await _references.PriceTypeExistsAsync(_dto.PriceTypeId, ct))
        {
            return Result.NotFound(nameof(PriceType), _dto.PriceTypeId.ToString());
        }

        if (_dto.MovementTypeId.HasValue &&
             !await _references.MovementTypeExistsAsync(_dto.MovementTypeId.Value, ct))
        {
            return Result.NotFound(nameof(MovementType), _dto.MovementTypeId.ToString());
        }

        return Result.Success();
    }
}