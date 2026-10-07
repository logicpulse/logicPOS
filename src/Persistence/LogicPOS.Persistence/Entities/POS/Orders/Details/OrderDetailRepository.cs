using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

namespace LogicPOS.Persistence.Repositories;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    public OrderDetailRepository(LogicPOSDbContext database) : base(database)
    {
    }

    public override Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }
}