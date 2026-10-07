using System;

using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Entities.POS.Devices.Printers.Printer;

public class PrinterAssociationRepository :  IPrinterAssociationRepository
{
    private readonly LogicPOSDbContext _database;

    public PrinterAssociationRepository(LogicPOSDbContext database)
    {
        _database = database;
    }
  
    public async Task<Guid> AssociatePrinterAsync(Guid printerId, Guid entityId, CancellationToken cancellationToken = default)
    {
        var association = new PrinterAssociation
        {
            PrinterId = printerId,
            EntityId = entityId
        };

        await _database.PrinterAssociations!.AddAsync(association, cancellationToken);
        await _database.SaveChangesAsync(cancellationToken);

        return association.Id;
    }

    public async Task<bool> AssociationExistsAsync(Guid printerId, Guid entityId, CancellationToken cancellationToken = default)
    {
        return await _database.PrinterAssociations!.AnyAsync(x => x.PrinterId == printerId && x.EntityId == entityId,
                                                             cancellationToken);
    }

    public async Task<bool> EntityExistsAsync(Guid entityId, CancellationToken cancellationToken = default)
    {
        return await _database.PrinterAssociations!.AnyAsync(x => x.EntityId == entityId, cancellationToken);
    }

    public async Task RemoveEntityAssociationAsync(Guid entityId, CancellationToken cancellationToken = default)
    {
        await _database.PrinterAssociations!.Where(x => x.EntityId == entityId).ExecuteDeleteAsync(cancellationToken);
    }

}
