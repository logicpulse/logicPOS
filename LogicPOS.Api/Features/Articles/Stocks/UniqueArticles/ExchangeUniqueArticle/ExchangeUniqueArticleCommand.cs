using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.StockManagement.ExchangeUniqueArticle
{
    public class ExchangeUniqueArticleCommand: IRequest<ErrorOr<Success>>
    {
        public Guid ReturnedArticleId { get; set; }
        public Guid ExchangeArticleId { get; set; }
    }
}
