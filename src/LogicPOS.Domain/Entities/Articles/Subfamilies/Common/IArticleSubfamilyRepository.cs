using LogicPOS.Domain.Entities.Common;

namespace LogicPOS.Domain.Repositories;

public interface IArticleSubfamilyRepository : IRepository.IWithDesignation
{
    public Task<bool> DesignationExistsAsync(Guid familyId, string designation, CancellationToken ct);
}