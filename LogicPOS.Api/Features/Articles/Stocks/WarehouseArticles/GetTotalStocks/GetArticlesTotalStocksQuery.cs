using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetTotalStocks
{
    public class GetArticlesTotalStocksQuery : IRequest<ErrorOr<IEnumerable<TotalStock>>>
    {
        public IEnumerable<Guid> ArticleIds { get; set; }

        public GetArticlesTotalStocksQuery(IEnumerable<Guid> ids)
        {
            ArticleIds = ids;
        }

        public string GetUrlQuery()
        {
            StringBuilder urlBuilder = new StringBuilder("?");
            foreach (var id in ArticleIds)
            {
                urlBuilder.Append($"articleIds={id}&");
            }

            return urlBuilder.ToString();
        }
    }
}
