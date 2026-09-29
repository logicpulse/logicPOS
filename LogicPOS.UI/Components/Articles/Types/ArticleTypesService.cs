using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Types.GetAllArticleTypes;
using LogicPOS.UI.Errors;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.ArticlesTypes
{
    public static class ArticleTypesService
    {
        private static List<ArticleType> _articleTypes;
        public static ArticleType DefaultArticleType => ArticleTypes.FirstOrDefault(at => at.Code == "10");

        public static List<ArticleType> ArticleTypes
        {
            get
            {
                if (_articleTypes == null)
                {
                   _articleTypes = GetAllArticleTypes();
                }
                return _articleTypes;
            }
        }


        public static void RefreshArticleTypesCache()
        {
            _articleTypes = GetAllArticleTypes();
        }

        private static List<ArticleType> GetAllArticleTypes()
        {
            var query = new GetAllArticleTypesQuery();
            var articleTypes = DependencyInjection.Mediator.Send(query).Result;

            if (articleTypes.IsError != false)
            {
                ErrorHandlingService.HandleApiError(articleTypes);
                return new List<ArticleType>();
            }

            return articleTypes.Value.ToList();
        }
    }
}
