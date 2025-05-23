using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.Subfamilies.GetAllArticleSubfamilies
{
    public class GetAllArticleSubfamiliesQuery : IRequest<ErrorOr<IEnumerable<ArticleSubfamily>>>
    {
    }
}
