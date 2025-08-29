using ErrorOr;
using LogicPOS.Api.Features.Articles.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.Articles.GetArticleViewModel
{
    public class GetArticleViewModelQuery : IRequest<ErrorOr<ArticleViewModel>>
    {
        public Guid Id { get; set; }

        public GetArticleViewModelQuery(Guid id)
        {
            Id = id;
        }
    }
}
