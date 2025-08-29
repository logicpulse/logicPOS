using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Articles.GetArticleViewModel;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.GetArticleByCode;
using LogicPOS.Api.Features.Articles.GetArticleById;
using LogicPOS.Api.Features.Articles.GetArticles;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Articles.Stocks.WarehouseArticles.GetWarehouseArticleById;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.UI.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Articles
{
    public static class ArticlesService
    {
        private static readonly ISender _mediator = DependencyInjection.Mediator;

        public static List<ArticleViewModel> GetAllArticles()
        {
            var query = new GetArticlesQuery
            {
                PageSize = 1000000 // High page size to retrieve all articles
            };
            var articles = _mediator.Send(query).Result;

            if (articles.IsError != false)
            {
                ErrorHandlingService.HandleApiError(articles);
                return new List<ArticleViewModel>();
            }

            return articles.Value.Items.ToList();
        }

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

        public static ArticleViewModel GetArticleViewModel(Guid id)
        {
            var result = _mediator.Send(new GetArticleViewModelQuery(id)).Result;
           
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            if (result.Value == null)
            {
                return null;
            }

            return result.Value;
        }

        public static ArticleViewModel GetArticleByCode(string code)
        {
            var result = _mediator.Send(new GetArticleByCodeQuery(code)).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            if (result.Value == null)
            {
                return null;
            }

            return result.Value;
        }

        public static List<ArticleHistory> GetAllArticleHistories()
        {
            var result = _mediator.Send(new GetArticlesHistoriesQuery() { PageSize = 100000 }).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }
            var ArticleHistories = result.Value;
            return ArticleHistories.Items.ToList();
        }
    }
}
