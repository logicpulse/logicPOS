using LogicPOS.Domain.Repositories;

namespace LogicPOS.Domain.Entities;

public record ReceiptReferences(
    IDocumentSeriesRepository SeriesRepository,
    ICurrencyRepository CurrencyRepository,
    IPaymentMethodRepository PaymentMethodRepository
)
{
    public async Task<bool> CurrencyExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await CurrencyRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> PaymentMethodExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await PaymentMethodRepository.ExistsAsync(id, cancellationToken);
    }

}