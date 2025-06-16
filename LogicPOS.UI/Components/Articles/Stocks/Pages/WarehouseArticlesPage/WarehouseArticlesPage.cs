using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.StockManagement.GetWarehouseArticles;
using LogicPOS.Api.Features.Articles.Stocks.WarehouseArticles.Common;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public partial class WarehouseArticlesPage : Page<WarehouseArticleViewModel>
    {
        public GetWarehouseArticlesQuery CurrentQuery { get; private set; } = GetDefaultQuery();
        public PaginatedResult<WarehouseArticleViewModel> Articles { get; private set; }

        public WarehouseArticlesPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
            RemoveForbiddenButtons();
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
        }

        private void RemoveForbiddenButtons()
        {
            Navigator.RightButtons.Remove(Navigator.BtnView);
            Navigator.RightButtons.Remove(Navigator.BtnDelete);
            Navigator.RightButtons.Remove(Navigator.BtnInsert);
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new ChangeArticleLocationModal(SelectedEntity);
            int response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override DeleteCommand GetDeleteCommand() => null;

        public void FilterByArticleId(Guid articleId)
        {
            CurrentQuery = new GetWarehouseArticlesQuery { ArticleId = articleId };
            Refresh();
        }

        public override void Search(string searchText)
        {
            CurrentQuery = new GetWarehouseArticlesQuery { Search = searchText };
            Refresh();
        }

        protected override void InitializeFilter()
        {
            GridViewSettings.Filter = new TreeModelFilter(GridViewSettings.Model, null);
            GridViewSettings.Filter.VisibleFunc = (model, iterator) => true;

        }
        private static GetWarehouseArticlesQuery GetDefaultQuery()
        {
            return new GetWarehouseArticlesQuery();

        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLEWAREHOUSE_CREATE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLEWAREHOUSE_EDIT");
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLEWAREHOUSE_DELETE");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLEWAREHOUSE_VIEW");
            
        }
    }
}