using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.DeleteArticle;
using LogicPOS.Api.Features.Articles.GetArticles;
using LogicPOS.Api.Features.Articles.StockManagement.GetTotalStocks;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Errors;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ArticlesPage : Page<ArticleViewModel>
    {
        public GetArticlesQuery CurrentQuery { get; private set; } = GetDefaultQuery();
        public PaginatedResult<ArticleViewModel> Articles { get; private set; }

        public ArticlesPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
        }

        protected override void LoadEntities()
        {
            var getArticles = _mediator.Send(CurrentQuery).Result;

            if (getArticles.IsError)
            {
                ErrorHandlingService.HandleApiError(getArticles,
                                                    source: SourceWindow);
                return;
            }

            Articles = getArticles.Value;

            _entities.Clear();

            if (Articles.Items.Any())
            {
                _entities.AddRange(Articles.Items);
            }

            ArticleTotalStockService.LoadTotals(_entities.Select(x => x.Id));
        }

        public Article GetSelectedArticle()
        {
            if (SelectedEntity == null)
            {
                return null;
            }

            var article = ArticlesService.GetArticlebById(SelectedEntity.Id);
            return article;
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new ArticleModal(mode, GetSelectedArticle());
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        public override void Search(string searchText)
        {
            CurrentQuery = new GetArticlesQuery { Search =  searchText };
            Refresh();
        }

        private static GetArticlesQuery GetDefaultQuery()
        {
            return new GetArticlesQuery();

        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeleteArticleCommand(SelectedEntity.Id);
        }

        #region Singleton
        private static ArticlesPage _instance;

        public static ArticlesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ArticlesPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }

        #endregion
    }
}
