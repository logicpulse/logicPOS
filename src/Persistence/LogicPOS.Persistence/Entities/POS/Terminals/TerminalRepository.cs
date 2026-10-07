using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

namespace LogicPOS.Persistence.Repositories;

public class TerminalRepository : Repository.WithDesignation<Terminal>, ITerminalRepository
{
    public TerminalRepository(LogicPOSDbContext database) : base(database)
    {
    }

    public override Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }
}