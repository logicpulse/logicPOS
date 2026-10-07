
using LogicPOS.Domain.Entities;

namespace LogicPOS.Domain.Repositories;

public interface IWarehouseLocationRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<bool> IsReferencedAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> LocationHasArticleAsync(Guid locationId,
                                       Guid articleId,
                                       CancellationToken cancellationToken = default);

    Task<WarehouseLocation?> GetDefaultAsync(CancellationToken cancellationToken = default);

    Task MakeDefaultAsync(Guid id, CancellationToken cancellationToken = default);
}

