using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;

namespace LogicPOS.Api.Features.Articles.GetArticleByCode
{
    public class GetArticleByCodeQuery : IRequest<ErrorOr<Article>>
    {
        public string Code { get; set; }

        public GetArticleByCodeQuery(string code)
        {
            Code = code;
        }
    }
}
