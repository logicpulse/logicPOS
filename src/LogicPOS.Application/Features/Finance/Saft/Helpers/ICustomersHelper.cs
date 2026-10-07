using LogicPOS.Shared.Features.Finance.Saft.Entities;

namespace LogicPOS.Application.Features.Finance.Saft.Helpers;

public interface ICustomersHelper
{
    public Task<IEnumerable<SaftCustomer>> GetSaftCustomers(DateTime startDate, DateTime endDate, CancellationToken ct);
}
