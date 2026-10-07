using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class PoleDisplayUpdater : EntityUpdater<PoleDisplay>
{
    private readonly UpdatePoleDisplayDto _dto;

    public PoleDisplayUpdater(PoleDisplay entity,
                              UpdatePoleDisplayDto dto,
                              IPoleDisplayRepository repository) :
        base(entity,repository)
    {
        _dto = dto;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForDesignationConflictAsync(_dto.Designation, ct);
    }

    protected override Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }

    protected override void SetNewData()
    {
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
        _entity.IsDeleted = _dto.IsDeleted;
    }
}