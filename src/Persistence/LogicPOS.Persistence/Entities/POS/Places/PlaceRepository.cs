using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class PlaceRepository : Repository.WithDesignation<Place>, IPlaceRepository
{
    public PlaceRepository(LogicPOSDbContext database) : base(database)
    {
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.Tables!.AnyAsync(x => x.PlaceId == id, ct);
    }
}