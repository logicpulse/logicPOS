using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class TerminalUpdater : EntityUpdater<Terminal>
{
    private readonly UpdateTerminalDto _dto;
    private readonly TerminalReferences _references;

    public TerminalUpdater(Terminal entity,
                           UpdateTerminalDto dto,
                           ITerminalRepository repository,
                           TerminalReferences references) : base(entity,repository)
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
         if (_dto.PrinterId.HasValue && _dto.PrinterId != Guid.Empty &&
            _dto.PrinterId != _entity.PrinterId)
        {
            if (!await _references.PrinterRepository.ExistsAsync(_dto.PrinterId.Value, ct))
            {
                return Result.NotFound<Printer>( _dto.PrinterId.ToString()!);
            }
        }

        if (_dto.WeighingMachineId.HasValue && _dto.WeighingMachineId != Guid.Empty &&
            _dto.WeighingMachineId != _entity.WeighingMachineId)
        {
            if (!await _references.WeighingMachineRepository.ExistsAsync(_dto.WeighingMachineId.Value, ct))
            {
                return Result.NotFound<WeighingMachine>(_dto.WeighingMachineId.ToString()!);
            }
        }

        if (_dto.PlaceId.HasValue && _dto.PlaceId != Guid.Empty &&
            _dto.PlaceId != _entity.PlaceId)
        {
            if (!await _references.PlaceRepository.ExistsAsync(_dto.PlaceId.Value, ct))
            {
                return Result.NotFound<Place>(_dto.PlaceId.ToString()!);
            }
        }

        if (_dto.ThermalPrinterId.HasValue && _dto.ThermalPrinterId != Guid.Empty &&
            _dto.ThermalPrinterId != _entity.ThermalPrinterId)
        {
            if (!await _references.PrinterRepository.ExistsAsync(_dto.ThermalPrinterId.Value, ct))
            {
                return Result.NotFound<Printer>(_dto.ThermalPrinterId.ToString()!);
            }
        }

        if (_dto.BarcodeReaderId.HasValue && _dto.BarcodeReaderId != Guid.Empty &&
            _dto.BarcodeReaderId != _entity.BarcodeReaderId)
        {
            if (!await _references.InputReaderRepository.ExistsAsync(_dto.BarcodeReaderId.Value, ct))
            {
                return Result.NotFound<InputReader>(_dto.BarcodeReaderId.ToString()!);
            }
        }

        if (_dto.CardReaderId.HasValue && _dto.CardReaderId != Guid.Empty &&
            _dto.CardReaderId != _entity.CardReaderId)
        {
            if (!await _references.InputReaderRepository.ExistsAsync(_dto.CardReaderId.Value, ct))
            {
                return Result.NotFound<InputReader>(_dto.CardReaderId.ToString()!);
            }
        }

        if (_dto.PoleDisplayId.HasValue && _dto.PoleDisplayId != Guid.Empty &&
            _dto.PoleDisplayId != _entity.PoleDisplayId)
        {
            if (!await _references.PoleDisplayRepository.ExistsAsync(_dto.PoleDisplayId.Value, ct))
            {
                return Result.NotFound<PoleDisplay>(_dto.PoleDisplayId.ToString()!);
            }
        }

        return Result.Success();
    }

    protected override void SetNewData()
    {
        _entity.Order = _dto.Order;
        _entity.Code = _dto.Code;
        _entity.Designation = _dto.Designation;
        _entity.TimerInterval = _dto.TimerInterval;
        _entity.PrinterId = _dto.PrinterId == Guid.Empty ? null : _dto.PrinterId;
        _entity.WeighingMachineId = _dto.WeighingMachineId == Guid.Empty ? null : _dto.WeighingMachineId;
        _entity.PlaceId = _dto.PlaceId == Guid.Empty ? null : _dto.PlaceId;
        _entity.ThermalPrinterId = _dto.ThermalPrinterId == Guid.Empty ? null : _dto.ThermalPrinterId;
        _entity.BarcodeReaderId = _dto.BarcodeReaderId == Guid.Empty ? null : _dto.BarcodeReaderId;
        _entity.CardReaderId = _dto.CardReaderId == Guid.Empty ? null : _dto.CardReaderId;
        _entity.PoleDisplayId = _dto.PoleDisplayId == Guid.Empty ? null : _dto.PoleDisplayId;
        _entity.Notes = _dto.Notes;
        _entity.IsDefault = _dto.IsDefault;
    }

}