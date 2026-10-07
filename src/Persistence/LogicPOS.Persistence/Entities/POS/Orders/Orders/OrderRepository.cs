
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly LogicPOSDbContext _database;

    public OrderRepository(LogicPOSDbContext database)
    {
        _database = database;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _database.Orders!.AnyAsync(x => x.Id == id, cancellationToken);
    }
}