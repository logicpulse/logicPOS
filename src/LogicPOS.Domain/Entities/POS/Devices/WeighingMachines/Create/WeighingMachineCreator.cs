using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class WeighingMachineCreator : EntityCreator<WeighingMachine>
{
    private readonly CreateWeighingMachineDto _dto;
    private readonly IWeighingMachineRepository _repository;

    public WeighingMachineCreator(CreateWeighingMachineDto dto,
                                  IWeighingMachineRepository repository) : 
        base(new())
    {
        _dto = dto;
        _repository = repository;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Order = await _repository.GetNextOrderAsync(ct);
        _entity.Designation = _dto.Designation;
        _entity.Code = await _repository.GetNextCodeAsync(ct);
        _entity.Parity = _dto.Parity;
        _entity.PortName = _dto.PortName;
        _entity.BaudRate = _dto.BaudRate;
        _entity.DataBits = _dto.DataBits;
        _entity.StopBits = _dto.StopBits;
        _entity.Parity = _dto.Parity;
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