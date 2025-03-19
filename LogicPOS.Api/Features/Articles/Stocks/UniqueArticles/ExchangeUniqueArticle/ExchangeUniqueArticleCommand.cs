using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.StockManagement.ExchangeUniqueArticle
{
    public class ExchangeUniqueArticleCommand: IRequest<ErrorOr<Unit>>
    {
        public Guid ReturnedArticleId { get; set; }
        public Guid ExchangeArticleId { get; set; }
    }
}
