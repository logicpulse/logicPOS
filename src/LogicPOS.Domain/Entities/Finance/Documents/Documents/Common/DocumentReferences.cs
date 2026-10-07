using LogicPOS.Domain.Repositories;

namespace LogicPOS.Domain.Entities;

public record DocumentReferences(IDocumentRepository DocumentRepository,
                                 IPaymentMethodRepository PaymentMethodRepository,
                                 IDocumentSeriesRepository SeriesRepository,
                                 ICurrencyRepository CurrencyRepository,
                                 ICustomerRepository CustomerRepository,
                                 IPaymentConditionRepository PaymentConditionRepository,
                                 IArticleRepository ArticleRepository,
                                 IVatRateRepository VatRateRepository,
                                 IVatExemptionReasonRepository VatExemptionRepository)
{
    public async Task<bool> DocumentExistsAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return await DocumentRepository.ExistsAsync(documentId, cancellationToken);
    }

    public async Task<bool> PaymentMethodExistsAsync(Guid paymentMethodId, CancellationToken cancellationToken = default)
    {
        return await PaymentMethodRepository.ExistsAsync(paymentMethodId, cancellationToken);
    }

    public async Task<bool> CurrencyExistsAsync(Guid currencyId, CancellationToken cancellationToken = default)
    {
        return await CurrencyRepository.ExistsAsync(currencyId, cancellationToken);
    }

    public async Task<bool> PaymentConditionExistsAsync(Guid paymentConditionId, CancellationToken cancellationToken = default)
    {
        return await PaymentConditionRepository.ExistsAsync(paymentConditionId, cancellationToken);
    }

}