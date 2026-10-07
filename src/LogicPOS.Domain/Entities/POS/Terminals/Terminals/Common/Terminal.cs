using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("pos_configurationplaceterminal")]
public class Terminal : Entity.WithCode.AndOrder.AndDesignation
{
    #region  Devices
    public Printer? Printer { get; set; }
    public Guid? PrinterId { get; set; }
    
    public WeighingMachine? WeighingMachine { get; set; }
    public Guid? WeighingMachineId { get; set; }

    public Place? Place { get; set; }
    public Guid? PlaceId { get; set; }

    public Printer? ThermalPrinter { get; set; }
    public Guid? ThermalPrinterId { get; set; }

    public InputReader? BarcodeReader { get; set; }
    public Guid? BarcodeReaderId { get; set; }

    public InputReader? CardReader { get; set; }
    public Guid? CardReaderId { get; set; }

    public PoleDisplay? PoleDisplay { get; set; }
    public Guid? PoleDisplayId { get; set; }
    #endregion

    public string HardwareId { get; set; } = null!;
    public uint TimerInterval { get; set; }
    public bool IsDefault { get; set; }

    public async Task<Result> UpdateAsync(UpdateTerminalDto dto,
        ITerminalRepository repository,
        TerminalReferences dependencies,
        CancellationToken cancellationToken = default)
    {
        var updater = new TerminalUpdater(this, dto, repository, dependencies);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<Terminal>> CreateAsync(CreateTerminalDto dto,
        ITerminalRepository repository,
        TerminalReferences references,
        CancellationToken cancellationToken = default)
    {
        var creator = new TerminalCreator(dto, repository,references);
        return await creator.CreateAsync(cancellationToken);
    }
}