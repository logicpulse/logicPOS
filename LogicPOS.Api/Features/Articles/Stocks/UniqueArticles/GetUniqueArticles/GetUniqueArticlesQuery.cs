using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetUniqueArticles
{
    public class GetUniqueArticlesQuery: IRequest<ErrorOr<IEnumerable<WarehouseArticle>>>
    {
        public Guid Id { get; set; }

        public GetUniqueArticlesQuery(Guid id)
        {
            Id = id;
        }
    }
}
