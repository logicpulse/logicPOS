using LogicPOS.Domain.Results;

namespace LogicPOS.Application.Features.Finance.Agt.Services;

public interface IAgtDocumentValidationStatusService
{
    /// <summary>
    /// Asks AGT for the validation status of a registered document or receipt and stores the result.
    /// Shared by the bulk validation-status endpoint and the background validation sync worker.
    /// </summary>
    Task<Result> UpdateValidationStatusAsync(Guid documentId, CancellationToken ct = default);
}
