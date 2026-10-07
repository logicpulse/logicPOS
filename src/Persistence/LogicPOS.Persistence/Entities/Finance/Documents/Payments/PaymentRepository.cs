using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly LogicPOSDbContext _database;

    public PaymentRepository(LogicPOSDbContext database)
    {
        _database = database;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _database.Payments!.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<decimal> GetTotalPaidForDocumentAsync(Guid documentId,
        CancellationToken ct = default)
    {
        // Sum in memory: decimal Sum in SQL fails with SQLite.
        var rows = await _database.Payments!
            .Where(x => x.DocumentId == documentId && x.Receipt!.Status == "N")
            .Select(x => new { x.CreditAmount, x.DebitAmount })
            .ToListAsync(ct);

        return rows.Sum(x => x.CreditAmount + x.DebitAmount);
    }
}