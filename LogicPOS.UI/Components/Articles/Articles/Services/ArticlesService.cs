using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Articles.GetArticleImage;
using LogicPOS.Api.Features.Articles.Articles.GetArticleViewModel;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.GetArticleByCode;
using LogicPOS.Api.Features.Articles.GetArticleById;
using LogicPOS.Api.Features.Articles.GetArticles;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Articles.StockManagement.GetTotalStocks;
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
        private static List<ArticleViewModel> _articles;

        public static List<ArticleViewModel> Articles
        {
            get
            {
                if (_articles == null)
                {
                   _articles = GetAllArticles();
                }
                return _articles;
            }
        }

        private static List<ArticleViewModel> GetAllArticles()
        {
            var query = new GetArticlesQuery
            {
                PageSize = 1000000 // High page size to retrieve all articles
            };
            var articles = DependencyInjection.Mediator.Send(query).Result;

            if (articles.IsError != false)
            {
                ErrorHandlingService.HandleApiError(articles);
                return new List<ArticleViewModel>();
            }

            return articles.Value.Items.ToList();
        }

        public static PaginatedResult<ArticleViewModel> GetArticles(GetArticlesQuery query)
        {
            var articles = DependencyInjection.Mediator.Send(query).Result;

            if (articles.IsError != false)
            {
                ErrorHandlingService.HandleApiError(articles);
                return PaginatedResult<ArticleViewModel>.Empty();
            }

            return articles.Value;
        }

        public static Article GetArticlebById(Guid id)
        {
            var result = DependencyInjection.Mediator.Send(new GetArticleByIdQuery(id)).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            return result.Value;
        }

        public static WarehouseArticle GetWarehouseArticleById(Guid id)
        {
            var result = DependencyInjection.Mediator.Send(new GetWarehouseArticleByIdQuery(id)).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            return result.Value;
        }

        public static ArticleViewModel GetArticleViewModel(Guid id)
        {
            var result = DependencyInjection.Mediator.Send(new GetArticleViewModelQuery(id)).Result;

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
            var result = DependencyInjection.Mediator.Send(new GetArticleByCodeQuery(code)).Result;
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
            var result = DependencyInjection.Mediator.Send(new GetArticlesHistoriesQuery() { PageSize = 100000 }).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }
            var ArticleHistories = result.Value;
            return ArticleHistories.Items.ToList();
        }
       
        public static string GetArticleImage(Guid id)
        {
            var result = DependencyInjection.Mediator.Send(new GetArticleImageQuery(id)).Result;

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

        public static decimal GetArticleTotalStock(Guid articleId)
        {
            
            var query = new GetArticlesTotalStocksQuery(new List<Guid> { articleId });
            var result = DependencyInjection.Mediator.Send(query).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return 0;
            }

            return result.Value.First().Quantity;
        }
        
        public static Dictionary<Guid, decimal> GetArticlesTotalStocks(IEnumerable<Guid> articleIds)
        {
            var query = new GetArticlesTotalStocksQuery(articleIds);
            var result = DependencyInjection.Mediator.Send(query).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return new Dictionary<Guid, decimal>();
            }
            return result.Value.ToDictionary(x => x.ArticleId, x => x.Quantity);
        }
    }
}
