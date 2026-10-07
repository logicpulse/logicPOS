using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly LogicPOSDbContext _database;
    public TicketRepository(LogicPOSDbContext database)
    {
        _database = database;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _database.Tickets!.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<int> GetNextTicketIdForOrderAsync(Guid orderId,
                                                        CancellationToken cancellationToken = default)
    {
        var maxTicketId = await _database.Tickets!
            .Where(x => x.OrderId == orderId)
            .MaxAsync(x => (int?)x.TicketId, cancellationToken);

        return (maxTicketId ?? 0) + 1;
    }
}