
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class PreferenceParameterRepository : Repository<PreferenceParameter> ,IPreferenceParameterRepository
{
    public PreferenceParameterRepository(LogicPOSDbContext database) : base(database)
    {
    }

    public override Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }

    public async Task<bool> TokenExistsAsync(string token, CancellationToken ct = default)
    {
        return await Database.PreferenceParameters!.AnyAsync(x => x.Token == token,
                                                              ct);
    }

    public async Task<string?> GetValueByTokenAsync(string token, CancellationToken ct = default)
    {
        return await Database.PreferenceParameters.Where(pp => pp.Token == token).Select(pp => pp.Value).FirstOrDefaultAsync(ct);
    }
}