
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

namespace LogicPOS.Persistence.Repositories;

public class PriceTypeRepository : Repository.WithDesignation<PriceType>, IPriceTypeRepository
{
    public PriceTypeRepository(LogicPOSDbContext context) : base(context)
    {
    }

    public override Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }
}