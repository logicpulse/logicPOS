using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class DocumentSeriesRepository : Repository.WithDesignation<DocumentSeries>, IDocumentSeriesRepository
{
    public DocumentSeriesRepository(LogicPOSDbContext database) : base(database)
    {
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.Documents!.AnyAsync(x => x.SeriesId == id, ct);
    }

    public async Task<DocumentSeries?> GetActiveSeriesByDocumentTypeForDocumentCreationAsync(string documentType,
        Guid? terminalId,
        CancellationToken ct = default)
    {
        if (documentType.Equals("FT", StringComparison.OrdinalIgnoreCase) == false)
        {
            var query1 = Database.DocumentSeries!.Where(x =>
                x.DocumentType!.Acronym == documentType && x.FiscalYear!.IsDeleted == false && x.IsDeleted == false);
            if (terminalId.HasValue)
            {
                query1 = query1.Where(s => s.TerminalId == terminalId);
            }

            return await query1.FirstOrDefaultAsync(ct);
        }

        var query2 = Database.DocumentSeries!.Where(x =>
            x.DocumentType!.Acronym == documentType &&
            x.DocumentType!.Designation.ToLower().Contains("guia") == false && x.FiscalYear!.IsDeleted == false &&
            x.IsDeleted == false);

        if (terminalId.HasValue)
        {
            query2 = query2.Where(s => s.TerminalId == terminalId);
        }

        return await query2.FirstOrDefaultAsync(ct);
    }

    public async Task<DocumentSeries?> GetActiveSeriesForReceiptCreationAsync(Guid? terminalId,
        CancellationToken ct = default)
    {
        var query = Database.DocumentSeries!.Where(s =>
            s.DocumentType!.SaftDocumentType == Domain.Enums.SaftDocumentType.Payments
            && s.FiscalYear!.IsDeleted == false && s.IsDeleted == false);

        if (terminalId.HasValue)
        {
            query = query.Where(s => s.TerminalId == terminalId);
        }

        return await query.FirstOrDefaultAsync(ct);
    }
}