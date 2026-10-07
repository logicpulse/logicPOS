using LogicPOS.Domain.Repositories;

namespace LogicPOS.Domain.Entities;

public record DocumentSeriesReferences(IFiscalYearRepository FiscalYearRepository,
                                       IDocumentTypeRepository DocumentTypeRepository)
{
    public async Task<bool> FiscalYearExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await FiscalYearRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> DocumentTypeExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DocumentTypeRepository.ExistsAsync(id, cancellationToken);
    }
}