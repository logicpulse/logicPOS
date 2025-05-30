using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.GetArticleByCode;
using LogicPOS.Api.Features.Articles.GetArticleById;
using LogicPOS.Api.Features.Articles.GetArticles;
using LogicPOS.Api.Features.Articles.Stocks.WarehouseArticles.GetWarehouseArticleById;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.UI.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LogicPOS.UI.Components.Articles
{
    public static class ArticlesService
    {
        private static readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();

        public static PaginatedResult<ArticleViewModel> GetArticles(GetArticlesQuery query)
        {
            var articles = _mediator.Send(query).Result;

            if (articles.IsError != false)
            {
                ErrorHandlingService.HandleApiError(articles);
                return PaginatedResult<ArticleViewModel>.Empty();
            }

            return articles.Value;
        }

        public static Article GetArticlebById(Guid id)
        {
            var result = _mediator.Send(new GetArticleByIdQuery(id)).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            return result.Value;
        }

        public static WarehouseArticle GetWarehouseArticleById(Guid id)
        {
            var result = _mediator.Send(new GetWarehouseArticleByIdQuery(id)).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            return result.Value;
        }

        public static Article GetArticleByCode(string code)
        {
            var result = _mediator.Send(new GetArticleByCodeQuery(code)).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }
            return result.Value;
        }
    }
}
