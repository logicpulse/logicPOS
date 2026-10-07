using LogicPOS.Domain.Repositories;

namespace LogicPOS.Domain.Entities;

public record WorkSessionMovementReferences(IWorkSessionPeriodRepository WorkSessionPeriodRepository,
                                            IDocumentRepository DocumentRepository,
                                            IPaymentRepository PaymentRepository)
{
    public async Task<bool> PeriodExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await WorkSessionPeriodRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> DocumentExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DocumentRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> PaymentExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await PaymentRepository.ExistsAsync(id, cancellationToken);
    }
}