using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Common;
using LogicPOS.Domain.Entities.Finance.Documents.Details.Common;

namespace LogicPOS.Domain.Repositories;

public interface IArticleRepository : IRepository.IWithDesignation
{
    Task<string> GetArticleClassAcronymAsync(Guid id, CancellationToken ct = default);
    Task<ArticleDetail> GetArticleDetailAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyDictionary<Guid, ArticleDetail>> GetArticleDetailsByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken ct = default);
    Task<bool> DesignationExistsAsync(Guid subfamilyId, string designation, CancellationToken ct);
    Task<bool> IsPriceWithVatArticleAsync(Guid id, CancellationToken ct);
    Task<Article?> GetSdrDepositArticleAsync(CancellationToken ct);
}
