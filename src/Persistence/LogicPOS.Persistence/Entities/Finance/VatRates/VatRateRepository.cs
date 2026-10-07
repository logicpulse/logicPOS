

using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class VatRateRepository : Repository.WithDesignation<VatRate>, IVatRateRepository
{
    public VatRateRepository(LogicPOSDbContext context) : base(context)
    {

    }

    public Task<VatRate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Database.VatRates!
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, VatRate>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids as IList<Guid> ?? ids.ToList();
        if (idList.Count == 0)
        {
            return new Dictionary<Guid, VatRate>();
        }

        var rates = await Database.VatRates!
            .AsNoTracking()
            .Where(x => idList.Contains(x.Id))
            .ToListAsync(cancellationToken);

        return rates.ToDictionary(x => x.Id);
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        var result = await Database.Articles!
                                    .AnyAsync(x => x.VatOnTableId == id || x.VatDirectSellingId == id, ct);

        if (result)
        {
            return true;
        }

        result = await Database.ArticleSubfamilies!
                                .AnyAsync(x => x.VatOnTableId == id || x.VatDirectSellingId == id, ct);

        if (result)
        {
            return true;
        }

        return  await Database.DocumentDetails!
                                .AnyAsync(x => x.Tax.TaxId == id, ct);
        

    }
}