using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Articles.Families.DeleteArticleFamily
{
    public class DeleteArticleFamilyCommand : DeleteCommand
    {
        public DeleteArticleFamilyCommand(Guid id) : base(id)
        {
        }
    }

}
