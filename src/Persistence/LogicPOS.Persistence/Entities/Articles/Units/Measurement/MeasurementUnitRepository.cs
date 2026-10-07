using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class MeasurementUnitRepository : Repository.WithDesignation<MeasurementUnit>, IMeasurementUnitRepository
{
    public MeasurementUnitRepository(LogicPOSDbContext database) : base(database)
    {
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.Articles!.AnyAsync(x => x.MeasurementUnitId == id, ct);
    }
}