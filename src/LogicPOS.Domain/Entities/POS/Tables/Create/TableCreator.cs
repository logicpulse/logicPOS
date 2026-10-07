using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class TableCreator : EntityCreator<Table>
{
    private readonly CreateTableDto _dto;
    private readonly ITableRepository _tableRepository;
    private readonly IPlaceRepository _placeRepository;

    public TableCreator(CreateTableDto dto,
        ITableRepository tableRepository,
        IPlaceRepository placeRepository) : base(new())
    {
        _dto = dto;
        _tableRepository = tableRepository;
        _placeRepository = placeRepository;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Order = _dto.Imported ? 0 : await _tableRepository.GetNextOrderAsync(ct);
        _entity.PlaceId = _dto.PlaceId;
        _entity.Designation = _dto.Designation;
        _entity.ButtonImage = _dto.ButtonImage;
        _entity.Discount = _dto.Discount ?? 0;
        _entity.Status = Enums.TableStatus.Free;
        _entity.Code = _dto.Imported ? "?" : await _tableRepository.GetNextCodeAsync(ct);
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted ?? false;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if (_dto.Imported)
        {
            return Result.Success();
        }
        return await CheckForDesignationConflictAsync(_tableRepository, _dto.Designation, ct);
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (!await _placeRepository.ExistsAsync(_dto.PlaceId, ct))
        {
            return Result.NotFound(nameof(Place), _dto.PlaceId.ToString());
        }

        return Result.Success();
    }
}