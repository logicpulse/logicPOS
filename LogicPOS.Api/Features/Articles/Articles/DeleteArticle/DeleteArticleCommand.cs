using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Articles.DeleteArticle
{
    public class DeleteArticleCommand : DeleteCommand
    {
        public DeleteArticleCommand(Guid id) : base(id)
        {
        }
    }
}
