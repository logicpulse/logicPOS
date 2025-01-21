using ErrorOr;
using LogicPOS.Api.Features.Articles.Common;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.AddArticleChildren
{
    public class AddArticleChildrenCommand : IRequest<ErrorOr<Guid>>
    {
        public Guid Id { get; set; }
        public IEnumerable<ArticleChild> Children { get; set; }
    }
}
