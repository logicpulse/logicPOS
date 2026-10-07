
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class CustomerTypeRepository : Repository.WithDesignation<CustomerType>, ICustomerTypeRepository
{
    public CustomerTypeRepository(LogicPOSDbContext context) : base(context)
    {
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.Customers!.AnyAsync(x => x.CustomerTypeId == id, ct);
    }
}