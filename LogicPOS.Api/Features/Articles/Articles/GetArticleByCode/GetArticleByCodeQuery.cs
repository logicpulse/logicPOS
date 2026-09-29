using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using MediatR;

namespace LogicPOS.Api.Features.Articles.GetArticleByCode
{
    public class GetArticleByCodeQuery : IRequest<ErrorOr<ArticleViewModel>>
    {
        public string Code { get; set; }

        public GetArticleByCodeQuery(string code)
        {
            Code = code;
        }
    }
}
