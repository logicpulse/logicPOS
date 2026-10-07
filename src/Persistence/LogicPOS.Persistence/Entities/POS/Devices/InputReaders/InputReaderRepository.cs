using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class InputReaderRepository : Repository.WithDesignation<InputReader>, IInputReaderRepository
{
    public InputReaderRepository(LogicPOSDbContext database) : base(database)
    {
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.Terminals!.AnyAsync(x => x.BarcodeReaderId == id || x.CardReaderId == id, ct);
    }
}