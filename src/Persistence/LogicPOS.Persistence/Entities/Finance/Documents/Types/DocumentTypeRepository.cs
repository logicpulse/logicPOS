using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

namespace LogicPOS.Persistence.Repositories;

public class DocumentTypeRepository : Repository.WithDesignation<DocumentType>, IDocumentTypeRepository
{
    public DocumentTypeRepository(LogicPOSDbContext database) : base(database)
    {
    }

    public override Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult(true); 
    }
}