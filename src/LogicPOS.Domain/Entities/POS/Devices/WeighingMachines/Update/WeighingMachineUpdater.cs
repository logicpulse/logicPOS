using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class WeighingMachineUpdater : EntityUpdater<WeighingMachine>
{
    private readonly UpdateWeighingMachineDto _dto;

    public WeighingMachineUpdater(WeighingMachine weighingMachine,
                                  UpdateWeighingMachineDto dto,
                                  IWeighingMachineRepository repository) : base(weighingMachine,
                                                                                repository)
    {
        _dto = dto;
    }

    protected async override Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
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
        _entity.Parity = _dto.Parity;
        _entity.PortName = _dto.PortName;
        _entity.BaudRate = _dto.BaudRate;
        _entity.DataBits = _dto.DataBits;
        _entity.StopBits = _dto.StopBits;
        _entity.Parity = _dto.Parity;
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted;
    }
}