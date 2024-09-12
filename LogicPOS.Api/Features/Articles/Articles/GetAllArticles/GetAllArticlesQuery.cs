using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.GetAllArticles
{
    public class GetAllArticlesQuery : IRequest<ErrorOr<IEnumerable<Article>>>
    {

    }
}
