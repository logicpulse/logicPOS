using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Common;

namespace LogicPOS.Domain.Repositories;

public interface IVatRateRepository : IRepository.IWithDesignation
{
    Task<VatRate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<Guid, VatRate>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);
}
