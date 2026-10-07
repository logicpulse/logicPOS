using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class PoleDisplayCreator : EntityCreator<PoleDisplay>
{
    private readonly CreatePoleDisplayDto _dto;
    private readonly IPoleDisplayRepository _repository;

    public PoleDisplayCreator(CreatePoleDisplayDto dto,
                              IPoleDisplayRepository repository) :
        base( new())
    {
        _dto = dto;
        _repository = repository;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Code = await _repository.GetNextCodeAsync(ct);
        _entity.Order = await _repository.GetNextOrderAsync(ct);
        _entity.Designation = _dto.Designation;
        _entity.VendorId = _dto.VendorId;
        _entity.ProductId = _dto.ProductId;
        _entity.COMPort = _dto.COMPort;
        _entity.EndPoint = _dto.EndPoint;
        _entity.CodeTable = _dto.CodeTable;
        _entity.CharactersPerLine = _dto.CharactersPerLine;
        _entity.GoToStandByInSeconds = _dto.GoToStandByInSeconds;
        _entity.StandByLine1 = _dto.StandByLine1;
        _entity.StandByLine2 = _dto.StandByLine2;
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