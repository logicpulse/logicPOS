using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.Types.GetAllArticleTypes
{
    public class GetAllArticleTypesQuery : IRequest<ErrorOr<IEnumerable<ArticleType>>>
    {

    }
}
