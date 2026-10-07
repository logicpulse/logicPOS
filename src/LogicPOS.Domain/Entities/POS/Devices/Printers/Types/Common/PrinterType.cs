using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("sys_configurationprinterstype")]
public class PrinterType : Entity.WithCode.AndOrder.AndDesignation
{
    public string? Token { get; set; }
    public bool ThermalPrinter { get; set; }
    
    public async Task<Result> UpdateAsync(UpdatePrinterTypeDto dto,
                                          IPrinterTypeRepository repository,
                                          CancellationToken cancellationToken = default)
    {
        var updater = new PrinterTypeUpdater(this, dto, repository);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<PrinterType>> CreateAsync(CreatePrinterTypeDto dto,
                                                              IPrinterTypeRepository repository,
                                                              CancellationToken cancellationToken = default)
    {
        var creator = new PrinterTypeCreator(dto, repository);
        return await creator.CreateAsync(cancellationToken);
    }
}
