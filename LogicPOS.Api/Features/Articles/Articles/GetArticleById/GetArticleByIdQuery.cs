using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.GetArticleById
{
    public class GetArticleByIdQuery : IRequest<ErrorOr<Article>>
    {
        public Guid Id { get; set; }

        public GetArticleByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
