using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("sys_configurationprinters")]
public class Printer : Entity.WithCode.AndOrder.AndDesignation
{
    public PrinterType? Type { get; set; }
    public Guid TypeId { get; set; }

    public string? NetworkName { get; set; }
    public string? ThermalEncoding { get; set; }
    public bool? ThermalPrintLogo { get; set; }
    public string? ThermalImageCompanyLogo { get; set; }
    public int? ThermalMaxCharsPerLineNormal { get; set; }
    public int? ThermalMaxCharsPerLineNormalBold { get; set; }
    public int? ThermalMaxCharsPerLineSmall { get; set; }
    public string? ThermalCutCommand { get; set; }
    public int? ThermalOpenDrawerValueM { get; set; }
    public int? ThermalOpenDrawerValueT1 { get; set; }
    public int? ThermalOpenDrawerValueT2 { get; set; }
    public bool? ShowInDialog { get; set; }

    public async Task<Result> UpdateAsync(UpdatePrinterDto dto,
        IPrinterRepository repository,
        IPrinterTypeRepository printerTypeRepository,
        CancellationToken cancellationToken = default)
    {
        var updater = new PrinterUpdater(this, dto, repository, printerTypeRepository);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<Printer>> CreateAsync(CreatePrinterDto dto,
        IPrinterRepository repository,
        IPrinterTypeRepository printerTypeRepository,
        CancellationToken cancellationToken = default)
    {
        var creator = new PrinterCreator(dto, repository, printerTypeRepository);
        return await creator.CreateAsync(cancellationToken);
    }
}