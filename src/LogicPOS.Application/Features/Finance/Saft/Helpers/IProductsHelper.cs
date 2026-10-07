using LogicPOS.Shared.Features.Finance.Saft.Entities;

namespace LogicPOS.Application.Features.Finance.Saft.Helpers;

public interface IProductsHelper
{
    public Task<IEnumerable<SaftProduct>> GetProductsAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
}