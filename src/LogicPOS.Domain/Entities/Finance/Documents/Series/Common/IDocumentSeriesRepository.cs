using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Common;

namespace LogicPOS.Domain.Repositories;

public interface IDocumentSeriesRepository : IRepository.IWithDesignation
{
    public Task<DocumentSeries?> GetActiveSeriesByDocumentTypeForDocumentCreationAsync(string documentType,
        Guid? terminalId, CancellationToken ct = default);

    public Task<DocumentSeries?>
        GetActiveSeriesForReceiptCreationAsync(Guid? terminalId, CancellationToken ct = default);
}