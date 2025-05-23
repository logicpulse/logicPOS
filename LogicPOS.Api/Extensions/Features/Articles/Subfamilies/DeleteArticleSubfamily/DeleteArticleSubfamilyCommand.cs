using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Articles.Subfamilies.DeleteArticleSubfamily
{
    public class DeleteArticleSubfamilyCommand : DeleteCommand
    {
        public DeleteArticleSubfamilyCommand(Guid id) : base(id)
        {
        }
    }
}
