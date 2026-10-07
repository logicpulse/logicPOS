using LogicPOS.Shared.Features.Finance.Saft.Entities;

namespace LogicPOS.Application.Features.Finance.Saft.Helpers;

public interface ISalesInvoicesHelper
{
    public Task<SaftSalesInvoices> GetSalesInvoicesAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
}