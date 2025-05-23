using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Articles.Types.DeleteArticleType
{
    public class DeleteArticleTypeCommand : DeleteCommand
    {
        public DeleteArticleTypeCommand(Guid id) : base(id)
        {
        }
    }
}
