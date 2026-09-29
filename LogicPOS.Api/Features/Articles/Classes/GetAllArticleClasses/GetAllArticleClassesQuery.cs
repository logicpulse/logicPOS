using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.Classes.GetAllArticleClasses
{
    public class GetAllArticleClassesQuery : IRequest<ErrorOr<IEnumerable<ArticleClass>>>
    {
    }
}
