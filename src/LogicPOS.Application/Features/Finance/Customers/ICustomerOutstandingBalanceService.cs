namespace LogicPOS.Application.Features.Finance.Customers;

public interface ICustomerOutstandingBalanceService
{
    Task<CustomerOutstandingBalance> GetLifetimeBalanceAsync(Guid customerId, CancellationToken ct = default);

    Task<IReadOnlyDictionary<Guid, decimal>> GetLifetimeBalancesAsync(
        IEnumerable<Guid> customerIds,
        CancellationToken ct = default);
}
