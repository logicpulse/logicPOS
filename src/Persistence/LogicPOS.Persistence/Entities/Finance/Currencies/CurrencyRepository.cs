

using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class CurrencyRepository : Repository.WithDesignation<Currency>, ICurrencyRepository
{
    public CurrencyRepository(LogicPOSDbContext context) : base(context)
    {
    }

    public async Task<string> GetAcronymAsync(Guid id,
                                              CancellationToken cancellationToken)
    {
        return await Database.Currencies!.Where(x => x.Id == id)
            .Select(x => x.Acronym)
            .FirstAsync(cancellationToken);
    }

    public async Task<Guid> GetCompanyCurrencyIdAsync(CancellationToken cancellationToken = default)
    {
        var currencyAcronym = await Database.PreferenceParameters!.Where(x => x.Token == "SYSTEM_CURRENCY")
                                                                  .Select(x => x.Value)
                                                                  .FirstAsync(cancellationToken);

        return await Database.Currencies!.Where(c => c.Acronym == currencyAcronym).Select(c => c.Id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Currency?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.Currencies!.AsNoTracking().Where(c => c.Id == id).FirstOrDefaultAsync(ct);
    }

    public async Task<decimal> GetExchangeRateAsync(Guid id,
                                                    CancellationToken cancellationToken = default)
    {
        return await Database.Currencies!.Where(x => x.Id == id)
            .Select(x => x.ExchangeRate)
            .FirstAsync(cancellationToken);
    }

    public override Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }
}