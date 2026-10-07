

using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class CountryRepository : Repository.WithDesignation<Country>, ICountryRepository
{
    public CountryRepository(LogicPOSDbContext context) : base(context)
    {
    }

    public Task<bool> Code2ExistsAsync(string code2,
        CancellationToken cancellationToken = default)
    {
        return Database.Countries!.AnyAsync(x => x.Code2 == code2,
            cancellationToken);
    }

    public Task<bool> Code3ExistsAsync(string code3,
        CancellationToken cancellationToken = default)
    {
        return Database.Countries!.AnyAsync(x => x.Code3 == code3,
            cancellationToken);
    }

    public override Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }
}