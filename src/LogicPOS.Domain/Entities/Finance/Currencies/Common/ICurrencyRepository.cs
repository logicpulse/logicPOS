
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Common;

namespace LogicPOS.Domain.Repositories;

public interface ICurrencyRepository : IRepository.IWithDesignation
{
    Task<decimal> GetExchangeRateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<string> GetAcronymAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> GetCompanyCurrencyIdAsync(CancellationToken cancellationToken  = default);
    Task<Currency?> GetByIdAsync(Guid id, CancellationToken ct = default);
}