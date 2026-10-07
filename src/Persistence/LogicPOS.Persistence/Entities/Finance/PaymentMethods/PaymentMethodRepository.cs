using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class PaymentMethodRepository : Repository.WithDesignation<PaymentMethod>, IPaymentMethodRepository
{
    public PaymentMethodRepository(LogicPOSDbContext database) : base(database)
    {
    }

    public async Task<string> GetAcronymAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.PaymentMethods!.Where(x => x.Id == id)
            .Select(x => x.Acronym)
            .FirstAsync(ct);
    }

    public async Task<Guid?> GetCustomerCardIdAsync(CancellationToken ct = default)
    {
        return await Database.PaymentMethods.Where(pm => pm.Acronym == "CA" || pm.Token == "CUSTOMER_CARD")
            .Select(pm => pm.Id).FirstOrDefaultAsync(ct);
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        var result = await Database.DocumentPaymentMethods!.AnyAsync(x => x.PaymentMethodId == id, ct);
        if (result)
        {
            return true;
        }

        return await Database.Receipts!.AnyAsync(x => x.PaymentMethodId == id, ct);
    }
}