using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Classes.GetAllArticleClasses;
using LogicPOS.UI.Errors;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.ArticleClasses
{
    public static class ArticleClassesService
    {
        private static List<ArticleClass> _articleClasses;
        public static ArticleClass DefaultArticleClass => ArticleClasses.FirstOrDefault(ac => ac.Acronym == "P");
        public static List<ArticleClass> ArticleClasses
        {
            get
            {
                if (_articleClasses == null)
                {
                   _articleClasses = GetAllArticleClasses();
                }
                return _articleClasses;
            }
        }
        public static void RefreshArticleClassesCache()
        {
            _articleClasses = GetAllArticleClasses();
        }

        private static List<ArticleClass> GetAllArticleClasses()
        {
            var query = new GetAllArticleClassesQuery();
            var articleClasses = DependencyInjection.Mediator.Send(query).Result;

            if (articleClasses.IsError != false)
            {
                ErrorHandlingService.HandleApiError(articleClasses);
                return new List<ArticleClass>();
            }

            return articleClasses.Value.ToList();
        }
    }
}
