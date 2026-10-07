using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class TerminalCreator : EntityCreator<Terminal>
{
    private readonly CreateTerminalDto _dto;
    private readonly ITerminalRepository _repository;
    private readonly TerminalReferences _references;

    public TerminalCreator(CreateTerminalDto dto,
                           ITerminalRepository repository,
                           TerminalReferences references) : base(new())
    {
        _dto = dto;
        _repository = repository;
        _references = references;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Code = _dto.Imported ? "?" : await _repository.GetNextCodeAsync(ct);
        _entity.Order = _dto.Imported ? 0 : await _repository.GetNextOrderAsync(ct);
        _entity.Designation = _dto.Designation;
        _entity.HardwareId = _dto.HardwareId;
        _entity.TimerInterval = _dto.TimerInterval;
        _entity.Notes = _dto.Notes;
        _entity.PlaceId = _dto.PlaceId;
        _entity.PrinterId = _dto.PrinterId;
        _entity.ThermalPrinterId = _dto.ThermalPrinterId;
        _entity.PoleDisplayId = _dto.PoleDisplayId;
        _entity.WeighingMachineId = _dto.WeighingMachineId;
        _entity.BarcodeReaderId = _dto.BarcodeReaderId;
        _entity.CardReaderId = _dto.CardReaderId;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForDesignationConflictAsync(_repository,
                                                      _entity.Designation,
                                                      ct);
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
       if (_dto.PlaceId.HasValue &&
            !await _references.PlaceExistsAsync(_dto.PlaceId.Value, ct))
        {
            return Result.NotFound(nameof(Place), _dto.PlaceId.ToString());
        }

        if (_dto.PrinterId.HasValue &&
            !await _references.PrinterExistsAsync(_dto.PrinterId.Value, ct))
        {
            return Result.NotFound(nameof(Printer), _dto.PrinterId.ToString());
        }

        if (_dto.ThermalPrinterId.HasValue &&
            !await _references.PrinterExistsAsync(_dto.ThermalPrinterId.Value, ct))
        {
            return Result.NotFound(nameof(Printer), _dto.ThermalPrinterId.ToString());
        }

        if (_dto.PoleDisplayId.HasValue &&
            !await _references.PoleDisplayExistsAsync(_dto.PoleDisplayId.Value, ct))
        {
            return Result.NotFound(nameof(PoleDisplay), _dto.PoleDisplayId.ToString());
        }

        if (_dto.WeighingMachineId.HasValue &&
            !await _references.WeighingMachineExistsAsync(_dto.WeighingMachineId.Value, ct))
        {
            return Result.NotFound(nameof(WeighingMachine), _dto.WeighingMachineId.ToString());
        }

        if (_dto.BarcodeReaderId.HasValue &&
            !await _references.InputReaderExistsAsync(_dto.BarcodeReaderId.Value, ct))
        {
            return Result.NotFound(nameof(InputReader), _dto.BarcodeReaderId.ToString());
        }

        if (_dto.CardReaderId.HasValue &&
            !await _references.InputReaderExistsAsync(_dto.CardReaderId.Value, ct))
        {
            return Result.NotFound(nameof(InputReader), _dto.CardReaderId.ToString());
        }

        return Result.Success();
    }
}