using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.GetArticleChildren
{
    public class GetArticleChildrenQuery : IRequest<ErrorOr<IEnumerable<ArticleChild>>>
    {
        public Guid Id { get; set; }

        public GetArticleChildrenQuery(Guid id)
        {
            Id = id;
        }
    }
}
