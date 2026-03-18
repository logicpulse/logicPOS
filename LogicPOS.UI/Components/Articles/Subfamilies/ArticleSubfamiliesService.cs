using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Subfamilies.GetAllArticleSubfamilies;
using LogicPOS.UI.Errors;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Articles.Subfamilies
{
    public static class ArticleSubfamiliesService
    {
        private static List<ArticleSubfamily> _articleSubfamilies;
        public static List<ArticleSubfamily> ArticleSubfamilies
        {
            get
            {
                if (_articleSubfamilies == null)
                {
                    _articleSubfamilies = GetAllArticleSubfamilies();
                }

                return _articleSubfamilies;
            }
        }
        private static List<ArticleSubfamily> GetAllArticleSubfamilies()
        {

            var articles = DependencyInjection.Mediator.Send(new GetAllArticleSubfamiliesQuery()).Result;

            if (articles.IsError != false)
            {
                ErrorHandlingService.HandleApiError(articles);
                return new List<ArticleSubfamily>();
            }

            return articles.Value.ToList();
        }
    }
}
