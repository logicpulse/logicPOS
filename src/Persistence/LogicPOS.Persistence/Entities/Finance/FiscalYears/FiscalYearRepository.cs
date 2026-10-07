
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class FiscalYearRepository : Repository.WithDesignation<FiscalYear>, IFiscalYearRepository
{
    public FiscalYearRepository(LogicPOSDbContext database) : base(database)
    {
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.DocumentSeries!.AnyAsync(x => x.FiscalYearId == id, ct);
    }

    public async Task<bool> AcronymExistsAsync(string acronym, CancellationToken ct)
    {
        return await Database.FiscalYears.AnyAsync(x => x.Acronym == acronym, ct);
    }
}