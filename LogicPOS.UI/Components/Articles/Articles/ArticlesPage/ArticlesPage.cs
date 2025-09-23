using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.DeleteArticle;
using LogicPOS.Api.Features.Articles.GetArticles;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ArticlesPage : Page<ArticleViewModel>
    {
        public GetArticlesQuery CurrentQuery { get; private set; } = GetDefaultQuery();
        public PaginatedResult<ArticleViewModel> Articles { get; private set; }
        private Dictionary<Guid, decimal> _articleStocks = new Dictionary<Guid, decimal>();

        public ArticlesPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
            AddEventHandlers();
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

            LoadCurrentArticlesStocks();
        }

        private void LoadCurrentArticlesStocks()
        {
            ArticlesService.GetArticlesTotalStocks(Articles.Items.Select(a => a.Id)).ForEach(ts =>
            {
                _articleStocks[ts.ArticleId] =  ts.Quantity; 
            });
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
            CurrentQuery = new GetArticlesQuery { Search = searchText };
            Refresh();
        }

        private static GetArticlesQuery GetDefaultQuery()
        {
            return new GetArticlesQuery();

        }

        protected override DeleteCommand GetDeleteCommand()
        {
            var result = new DeleteArticleCommand(SelectedEntity.Id);
            ArticlesService.RefreshArticlesCache();
            return result;
        }

        public override void UpdateButtonPrevileges()
        {

            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLE_CREATE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLE_EDIT");
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLE_DELETE");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLE_VIEW");

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
