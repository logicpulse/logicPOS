using LogicPOS.Domain.Repositories;

namespace LogicPOS.Domain.Entities;

public record OrderReferences(IArticleRepository ArticleRepository,
                              IVatExemptionReasonRepository VatExemptionReasonRepository,
                              IOrderRepository OrderRepository,
                              ITableRepository TableRepository,
                              ITicketRepository TicketRepository)
{
    public async Task<bool> VatExemptionReasonExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await VatExemptionReasonRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> ArticleExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await ArticleRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> OrderExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await OrderRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> TableExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await TableRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> TicketExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await TicketRepository.ExistsAsync(id, cancellationToken);
    }
}