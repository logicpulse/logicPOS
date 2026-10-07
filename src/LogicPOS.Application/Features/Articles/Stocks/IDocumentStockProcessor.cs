using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Domain.Results;

namespace LogicPOS.Application.Features.Articles.Stocks;

public interface IDocumentStockProcessor
{
    public Task<Result> ProcessIssuedDocumentAsync(Document document, ProcessArticleStockMode stockMode,
        ArticleSerialNumberStatus returnStatus = ArticleSerialNumberStatus.Available, CancellationToken ct = default);

    public Task<Result> ProcessCancelledDocumentAsync(Document document, ProcessArticleStockMode stockMode,
        ArticleSerialNumberStatus returnStatus = ArticleSerialNumberStatus.Available, CancellationToken ct = default);
}