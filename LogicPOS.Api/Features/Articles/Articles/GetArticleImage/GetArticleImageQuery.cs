using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.Articles.GetArticleImage
{
    public class GetArticleImageQuery : IRequest<ErrorOr<string>>
    {
        public Guid Id { get; set; }

        public GetArticleImageQuery(Guid id)
        {
            Id = id;
        }
    }
}