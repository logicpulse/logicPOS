using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.StockManagement.GetAllWarehouseArticles;
using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class WarehouseArticlesPage : Page<WarehouseArticle>
    {
        protected override IRequest<ErrorOr<IEnumerable<WarehouseArticle>>> GetAllQuery => new GetAllWarehouseArticlesQuery();
        public WarehouseArticlesPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
            RemoveForbiddenButtons();
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

        protected override void AddColumns()
        {
            GridView.AppendColumn(CreateWarehouseColumn());
            GridView.AppendColumn(CreateLocationColumn());
            GridView.AppendColumn(CreateDesignationColumn());
            GridView.AppendColumn(CreateSerialNumberColumn());
            GridView.AppendColumn(CreateQuantityColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(6));
        }

        protected override DeleteCommand GetDeleteCommand() => null;

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);
            AddWarehouseSorting();
            AddLocationSorting();
            AddDesignationSorting();
            AddSerialNumberSorting();
            AddQuantitySorting();
        }

        public void ApplyFilter(Predicate<WarehouseArticle> predicate)
        {
            LoadEntities();
            _entities.RemoveAll(e => !predicate(e));
            var model = (ListStore)GridViewSettings.Model;
            model.Clear();
            AddEntitiesToModel();
        }
    }
}
