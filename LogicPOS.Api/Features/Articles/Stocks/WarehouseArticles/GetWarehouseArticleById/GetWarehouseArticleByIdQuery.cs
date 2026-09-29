using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.Stocks.WarehouseArticles.GetWarehouseArticleById
{
    public class GetWarehouseArticleByIdQuery : IRequest<ErrorOr<WarehouseArticle>>
    {
        public Guid Id { get; set; }

        public GetWarehouseArticleByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
