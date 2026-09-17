using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Articles.Stocks.WarehouseArticles.DeleteWarehouseArticle
{
    public class DeleteWarehouseArticleCommand : DeleteCommand
    {
        public DeleteWarehouseArticleCommand(Guid id) : base(id)
        {
        }
    }
}
