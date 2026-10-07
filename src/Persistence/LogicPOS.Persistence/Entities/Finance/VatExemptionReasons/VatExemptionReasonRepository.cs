
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class VatExemptionReasonRepository : Repository.WithDesignation<VatExemptionReason>, IVatExemptionReasonRepository
{
    public VatExemptionReasonRepository(LogicPOSDbContext context) : base(context)
    {
    }

    public async Task<VatExemptionReason?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Database.VatExemptionReasons!
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, VatExemptionReason>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids as IList<Guid> ?? ids.ToList();
        if (idList.Count == 0)
        {
            return new Dictionary<Guid, VatExemptionReason>();
        }

        var reasons = await Database.VatExemptionReasons!
            .AsNoTracking()
            .Where(x => idList.Contains(x.Id))
            .ToListAsync(cancellationToken);

        return reasons.ToDictionary(x => x.Id);
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        var result = await Database.Articles!
                                    .AnyAsync(x => x.VatExemptionReasonId == id, ct);
        
        if (result){
            return true;
        }

        return  await Database.OrderDetails!
                               .AnyAsync(x => x.VatExemptionReasonId == id, ct);
    }
}