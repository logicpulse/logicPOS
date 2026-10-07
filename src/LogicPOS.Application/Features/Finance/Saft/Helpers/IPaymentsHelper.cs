using LogicPOS.Shared.Features.Finance.Saft.Entities;

namespace LogicPOS.Application.Features.Finance.Saft.Helpers;

public interface IPaymentsHelper
{
    public  Task<SaftPayments> GetPaymentsAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
}