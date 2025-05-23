using LogicPOS.Api.Features.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Articles.Classes.DeleteArticleClass
{
    public class DeleteArticleClassCommand : DeleteCommand
    {
        public DeleteArticleClassCommand(Guid id) : base(id)
        { }
    }
}
