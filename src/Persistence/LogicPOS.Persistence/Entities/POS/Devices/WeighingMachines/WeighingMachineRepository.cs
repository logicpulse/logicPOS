
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class WeighingMachineRepository : Repository.WithDesignation<WeighingMachine>, IWeighingMachineRepository
{
    public WeighingMachineRepository(LogicPOSDbContext context) : base(context)
    {
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.Terminals!.AnyAsync(x => x.WeighingMachineId == id, ct);
    }
}