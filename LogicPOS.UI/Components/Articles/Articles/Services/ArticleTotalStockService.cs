using LogicPOS.Api.Features.Articles.StockManagement.GetTotalStocks;
using LogicPOS.UI.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Articles
{
    public static class ArticleTotalStockService
    {
        private static readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        private static readonly Dictionary<Guid, decimal> _totals = new Dictionary<Guid, decimal>();

        public static void LoadTotals(IEnumerable<Guid> articles)
        {
            var filteredIds = articles.Where(id => !_totals.ContainsKey(id));

            if (!filteredIds.Any())
            {
                return;
            }

            var query = new GetArticlesTotalStocksQuery(filteredIds);
            var result = _mediator.Send(query).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return;
            }

            foreach (var total in result.Value)
            {
                if (_totals.ContainsKey(total.ArticleId))
                {
                    _totals[total.ArticleId] = total.Quantity;
                }
                else
                {
                    _totals.Add(total.ArticleId, total.Quantity);
                }
            }
        }

        public static decimal GetArticleTotalStock(Guid articleId)
        {
            if (_totals.ContainsKey(articleId))
            {
                return _totals[articleId];
            }

            var query = new GetArticlesTotalStocksQuery(new List<Guid> { articleId });
            var result = _mediator.Send(query).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return 0;
            }


            if (result.Value.Any())
            {
                _totals[articleId] = result.Value.First().Quantity;
                return _totals[articleId];
            }

            return 0;
        }

    }
}
