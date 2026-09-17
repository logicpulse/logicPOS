using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetUniqueArticleChildren
{
    public class GetUniqueArticleChildrenQuery : IRequest<ErrorOr<IEnumerable<WarehouseArticle>>>
    {
        public Guid Id { get; set; }

        public GetUniqueArticleChildrenQuery(Guid id)
        {
            Id = id;
        }
    }
}
