using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly LogicPOSDbContext _database;

    public DocumentRepository(LogicPOSDbContext database)
    {
        _database = database;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await _database.Documents!.AnyAsync(document => document.Id == id, ct);
    }

    public async Task<bool> NumberExistsAsync(string number, CancellationToken ct = default)
    {
        return await _database.Documents!.AnyAsync(document => document.Number == number,
            ct);
    }

    public async Task<decimal> GetDocumentTotalPaidAsync(Guid documentId, CancellationToken ct = default)
    {
        var totals = await GetDocumentsTotalPaidAsync([documentId], ct);
        return totals.GetValueOrDefault(documentId);
    }

    /// <summary>
    /// Paid total per document from allocation lines, not <see cref="Receipt.Amount"/>.
    /// FT/ND: sum of CreditAmount + DebitAmount on payment rows plus TotalFinal of active child NC documents.
    /// NC: payment rows only (DebitAmount when settled on a receipt; CreditAmount is normally zero).
    /// </summary>
    public async Task<IReadOnlyDictionary<Guid, decimal>> GetDocumentsTotalPaidAsync(
        IEnumerable<Guid> documentIds,
        CancellationToken ct = default)
    {
        var ids = documentIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return new Dictionary<Guid, decimal>();
        }

        // Sum in memory: decimal Sum/GroupBy in SQL fails with SQLite.
        var paymentRows = await _database.Payments!
            .Where(p => p.Receipt!.Status == "N" && ids.Contains(p.DocumentId))
            .Select(p => new { p.DocumentId, p.CreditAmount, p.DebitAmount })
            .ToListAsync(ct);

        var creditNoteRows = await _database.Documents
            .Where(doc => doc.Type == "NC" && doc.Status == "N" && doc.ParentId != null &&
                          ids.Contains(doc.ParentId.Value))
            .Select(doc => new { ParentId = doc.ParentId!.Value, doc.TotalFinal })
            .ToListAsync(ct);

        var paymentByDocument = paymentRows
            .GroupBy(p => p.DocumentId)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.CreditAmount + p.DebitAmount));

        var creditNotesByDocument = creditNoteRows
            .GroupBy(c => c.ParentId)
            .ToDictionary(g => g.Key, g => g.Sum(c => c.TotalFinal));

        return ids.ToDictionary(
            id => id,
            id => paymentByDocument.GetValueOrDefault(id) + creditNotesByDocument.GetValueOrDefault(id));
    }
}