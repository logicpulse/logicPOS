using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class ReceiptRepository : IReceiptRepository
{
    private readonly LogicPOSDbContext _database;

    public ReceiptRepository(LogicPOSDbContext database)
    {
        _database = database;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await _database.Receipts!.AnyAsync(x => x.Id == id, ct);
    }

    public async Task<string> GetDocumentType(CancellationToken ct = default)
    {
        var documentType = await _database.DocumentTypes!
            .Where(t => t.SaftDocumentType == Domain.Enums.SaftDocumentType.Payments)
            .Select(t => t.Acronym)
            .FirstAsync(ct);

        return documentType;
    }
}