
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class DocumentDetailRepository : IDocumentDetailRepository
{
    private readonly LogicPOSDbContext _database;

    public DocumentDetailRepository(LogicPOSDbContext database)
    {
        _database = database;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _database.DocumentDetails!.AnyAsync(x => x.Id == id,
                                                         cancellationToken);
    }
}