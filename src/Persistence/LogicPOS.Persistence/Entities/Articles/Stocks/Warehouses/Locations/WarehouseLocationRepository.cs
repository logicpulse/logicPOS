using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class WarehouseLocationRepository  : IWarehouseLocationRepository
{
    private readonly LogicPOSDbContext _database;
    public WarehouseLocationRepository(LogicPOSDbContext context) 
    {
        _database = context;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _database.WarehouseLocations!
            .AnyAsync(wl => wl.Id == id, cancellationToken);
    }

    public Task<WarehouseLocation?> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        return _database.WarehouseLocations!
            .FirstOrDefaultAsync(wl => wl.IsDefault, cancellationToken);
    }

    public async Task<bool> IsReferencedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result =  await _database.StockMovements.AnyAsync(aw => aw.WarehouseLocationId == id, cancellationToken);
        return result && await _database.WarehouseArticles.AnyAsync(aw => aw.WarehouseLocationId == id, cancellationToken);
    }

    public async Task<bool> LocationHasArticleAsync(Guid locationId,
        Guid articleId,
        CancellationToken cancellationToken = default)
    {
        return await _database.WarehouseArticles!
            .AnyAsync(aw => aw.WarehouseLocationId == locationId && 
                            aw.ArticleId == articleId,
                cancellationToken);
    }

    public async Task MakeDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var location = await _database.WarehouseLocations!.AsTracking()
            .FirstOrDefaultAsync(wl => wl.Id == id, cancellationToken);
       
        if (location == null)
        {
            return;
        }

        var allLocations = await _database.WarehouseLocations!.AsTracking().ToListAsync(cancellationToken);

        foreach (var loc in allLocations)
        {
            loc.IsDefault = false;
        }

        location.IsDefault = true;
    }
}