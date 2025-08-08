using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.GetArticleByCode;
using LogicPOS.Api.Features.Articles.GetArticleById;
using LogicPOS.Api.Features.Articles.GetArticles;
using LogicPOS.Api.Features.Articles.Stocks.WarehouseArticles.GetWarehouseArticleById;
using LogicPOS.Api.Features.Articles.UpdateArticle;
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

        public static ArticleViewModel GetArticleViewModelById(Guid id)
        {
            var article = GetArticlebById(id);
            return ArticleToViewModel(article);
        }

        public static ArticleViewModel GetArticleByCode(string code)
        {
            var result = _mediator.Send(new GetArticleByCodeQuery(code)).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            if(result.Value == null)
            {
                return null;
            }

            return ArticleToViewModel(result.Value);
        }

        private static ArticleViewModel ArticleToViewModel(Article article)
        {
            return new ArticleViewModel
            {
                Id = article.Id,
                Code = article.Code,
                Designation = article.Designation,
                Family = article.Subfamily?.Family?.Designation,
                Subfamily = article.Subfamily?.Designation,
                Type = article.Type.Designation,
                ButtonLabel = article.Button.Label,
                DefaultQuantity = article.DefaultQuantity,
                MinimumStock = article.MinimumStock,
                Price = article.Price1.Value,
                VatDirectSelling = article.VatDirectSelling?.Value,
                Discount = article.Discount,
                IsComposed = article.IsComposed,
                Unit = article.MeasurementUnit?.Acronym,
                SubfamilyId = article.SubfamilyId,
                FamilyId = article.Subfamily?.FamilyId ?? Guid.Empty,
            };
        }

        public static void UpdateArticleNotes(Guid articleId, string Notes)
        {
            var updateCommand = new UpdateArticleCommand();

            updateCommand.Id = articleId;
            updateCommand.NewNotes = Notes;

        
            var result = _mediator.Send( updateCommand).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
            
            }
            
        }
    }
}
