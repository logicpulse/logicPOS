using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Articles.ExportArticlesToExcel;
using LogicPOS.Api.Features.Articles.Articles.GetArticleImage;
using LogicPOS.Api.Features.Articles.Articles.GetArticleViewModel;
using LogicPOS.Api.Features.Articles.Articles.GetAutoCompleteLines;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.GetArticleByCode;
using LogicPOS.Api.Features.Articles.GetArticleById;
using LogicPOS.Api.Features.Articles.GetArticles;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Articles.StockManagement.GetTotalStocks;
using LogicPOS.Api.Features.Articles.Stocks.WarehouseArticles.GetWarehouseArticleById;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Articles
{
    public static class ArticlesService
    {
        private static readonly Random _random = new Random();
        private static List<AutoCompleteLine> _autocompleteLines;

        public static List<AutoCompleteLine> AutocompleteLines
        {
            get
            {
                if (_autocompleteLines == null)
                {
                    _autocompleteLines = GetAutocompleteLines();
                }
                return _autocompleteLines;
            }
        }

        public static List<AutoCompleteLine> CodeAutocompleteLines => AutocompleteLines.Select(line => new AutoCompleteLine
        {
            Id = line.Id,
            Name = line.Code
        }).ToList();


        public static void RefreshArticlesCache()
        {
            _autocompleteLines = GetAutocompleteLines();
        }

        private static List<AutoCompleteLine> GetAutocompleteLines()
        {

            var articles = DependencyInjection.Mediator.Send(new GetAutoCompleteLinesQuery()).Result;

            if (articles.IsError != false)
            {
                ErrorHandlingService.HandleApiError(articles);
                return new List<AutoCompleteLine>();
            }

            return articles.Value.ToList();
        }

        public static List<AutoCompleteLine> GetUniqueArticlesAutocompleteLines()
        {
            var articles = DependencyInjection.Mediator.Send(new LogicPOS.Api.Features.Articles.Stocks.UniqueArticles.GetAutoCompleteLines.GetAutoCompleteLinesQuery()).Result;

            if (articles.IsError != false)
            {
                ErrorHandlingService.HandleApiError(articles);
                return new List<AutoCompleteLine>();
            }

            return articles.Value.Select(line => new AutoCompleteLine
            {
                Id = line.Id,
                Code = line.Code,
                Name = line.Code
            }).ToList();
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

        public static List<TotalStock> GetArticlesTotalStocks(IEnumerable<Guid> articleIds)
        {
            var query = new GetArticlesTotalStocksQuery(articleIds);
            var result = DependencyInjection.Mediator.Send(query).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }
            return result.Value.ToList();
        }

        public static string GenerateRandomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int length = _random.Next(3, 7);
            char[] result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[_random.Next(chars.Length)];
            }

            return new string(result);
        }

        public static string ExportArticlesToExcel()
        {
            var result = DependencyInjection.Mediator.Send(new ExportArticlesToExcelQuery()).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }
            return result.Value.Path;
        }
    }
}
