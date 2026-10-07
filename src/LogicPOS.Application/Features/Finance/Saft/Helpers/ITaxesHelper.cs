using LogicPOS.Shared.Features.Finance.Saft.Entities;

namespace LogicPOS.Application.Features.Finance.Saft.Helpers;

public interface ITaxesHelper
{
    public Task<IEnumerable<SaftTax>> GetTaxesAsync(CancellationToken ct);
  
}