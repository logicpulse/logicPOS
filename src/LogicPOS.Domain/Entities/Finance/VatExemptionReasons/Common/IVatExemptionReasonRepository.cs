using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Common;

namespace LogicPOS.Domain.Repositories;

public interface IVatExemptionReasonRepository : IRepository.IWithDesignation
{
    Task<VatExemptionReason?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<Guid, VatExemptionReason>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);
}
