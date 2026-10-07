using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

namespace LogicPOS.Persistence.Repositories;

public class WarehouseRepository : Repository.WithDesignation<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(LogicPOSDbContext context) : base(context)
    {
    }

    public  override Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }
}