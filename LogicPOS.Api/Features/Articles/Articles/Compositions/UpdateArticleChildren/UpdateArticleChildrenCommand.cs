using ErrorOr;
using LogicPOS.Api.Features.Articles.Common;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.UpdateArticleChildren
{
    public class UpdateArticleChildrenCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public IEnumerable<ArticleChild> Children { get; set; }
    }
}
