using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetAllWarehouseArticles
{
    public class GetAllWarehouseArticlesQuery : IRequest<ErrorOr<IEnumerable<WarehouseArticle>>>
    {
    }
}
