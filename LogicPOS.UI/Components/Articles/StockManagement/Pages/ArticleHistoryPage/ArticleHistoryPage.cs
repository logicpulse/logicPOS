using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.StockManagement.GetAllWarehouseArticles;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ArticleHistoryPage : Page<ArticleHistory>
    {
        protected override IRequest<ErrorOr<IEnumerable<ArticleHistory>>> GetAllQuery => new GetArticlesHistoriesQuery();

        public ArticleHistoryPage(Window parent) : base(parent)
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
            throw new System.NotImplementedException();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(CreateDesignationColumn());
            GridView.AppendColumn(CreateSerialNumberColumn());
            GridView.AppendColumn(CreateStatusColumn());
            GridView.AppendColumn(CreateIsComposedColumn());
            GridView.AppendColumn(CreatePurchaseDateColumn());
            GridView.AppendColumn(CreateProviderColumn());
            GridView.AppendColumn(CreatePurchasePriceColumn());
            GridView.AppendColumn(CreateOriginDocumentColumn());
            GridView.AppendColumn(CreateSaleDocumentColumn());
            GridView.AppendColumn(CreateWarehouseColumn());
            GridView.AppendColumn(CreateLocationColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(12));
        }

        protected override DeleteCommand GetDeleteCommand() => null;

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);
        }
    }
}
