using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Domain.Results;

namespace LogicPOS.Application.Features.Articles.Stocks;

public interface IStockMovementProcessor
{
    public Task<Result> ProcessStockMovementAsync(
        StockMovement movement, 
        IEnumerable<Guid>? childUniqueArticles,
        ArticleSerialNumberStatus returnStatus = ArticleSerialNumberStatus.Available,
        CancellationToken ct = default);

}