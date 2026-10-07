
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

namespace LogicPOS.Persistence.Repositories;

public class HolidayRepository : Repository.WithDesignation<Holiday>, IHolidayRepository
{
    public HolidayRepository(LogicPOSDbContext context) : base(context)
    {
    }

    public override Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult(false);
    }
}