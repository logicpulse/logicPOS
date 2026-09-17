using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.Families.GetAllArticleFamilies
{
    public class GetAllArticleFamiliesQuery : IRequest<ErrorOr<IEnumerable<ArticleFamily>>>
    {
    }
}
