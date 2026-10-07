using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class PaymentConditionRepository : Repository.WithDesignation<PaymentCondition>, IPaymentConditionRepository
{
    public PaymentConditionRepository(LogicPOSDbContext database) : base(database)
    {
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.PaymentConditions!.AnyAsync(x => x.Id == id, ct);
    }
}