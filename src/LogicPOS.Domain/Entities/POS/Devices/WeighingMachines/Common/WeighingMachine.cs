using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("sys_configurationweighingmachine")]
public class WeighingMachine : Entity.WithCode.AndOrder.AndDesignation
{
    public string PortName { get; set; } = null!;
    public uint BaudRate { get; set; }
    public string Parity { get; set; } = null!;
    public string StopBits { get; set; } = null!;
    public uint DataBits { get; set; }

    public Task<Result> UpdateAsync(UpdateWeighingMachineDto dto,
                                    IWeighingMachineRepository repository,
                                    CancellationToken cancellationToken = default)
    {
        var updater = new WeighingMachineUpdater(this, dto, repository);
        return updater.UpdateAsync(cancellationToken);
    }

    public static Task<Result<WeighingMachine>> CreateAsync(IWeighingMachineRepository repository,
                                                            CreateWeighingMachineDto dto,
                                                            CancellationToken cancellationToken = default)
    {
        var creator = new WeighingMachineCreator(dto, repository);
        return creator.CreateAsync(cancellationToken);
    }
}