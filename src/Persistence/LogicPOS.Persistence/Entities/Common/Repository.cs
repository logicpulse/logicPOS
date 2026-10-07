
using LogicPOS.Domain.Entities.Common;
using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories.Common;

public abstract class Repository<TEntity> : IRepository where TEntity : Entity.WithCode.AndOrder
{
    protected readonly LogicPOSDbContext Database;
    protected Repository(LogicPOSDbContext database)
    {
        Database = database;
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
    {
        return await Database.Set<TEntity>().AnyAsync(
             x => x.Code == code,
             ct);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.Set<TEntity>().AnyAsync(
            x => x.Id == id,
            ct);
    }

    public async Task<string> GetNextCodeAsync(CancellationToken ct = default)
    {
        var hasEntities = await Database.Set<TEntity>().AnyAsync(ct);

        if (hasEntities == false)
        {
            return CodeGenerator.DefaultFirstCode();
        }
        
        var currentCodes = await Database.Set<TEntity>()
            .Select(x => x.Code)
            .ToListAsync(ct);

        return CodeGenerator.Generate(currentCodes);
    }

    public async Task<uint> GetNextOrderAsync(CancellationToken ct = default)
    {
        var hasEntities = await Database.Set<TEntity>().AnyAsync(ct);

        if (hasEntities == false)
        {
            return 10;
        }

        return await Database.Set<TEntity>()
            .MaxAsync(x => x.Order, ct) + 10;
    }

    public abstract Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default);

}
