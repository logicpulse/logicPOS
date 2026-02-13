using Gtk;
using LogicPOS.Api.Enums;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Pages.GridViews;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ArticleHistoryPage
    {

        protected override void AddColumns()
        {
            if (!IsSelectionPage())
            {
                GridView.AppendColumn(CreateSelectColumn());
            }
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
            GridView.AppendColumn(CreateUpdatedAtColumn());
        }

        #region Creators
        private TreeViewColumn CreateDesignationColumn()
        {
            void RenderDesignation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.Article;
            }
            var title = LocalizedString.Instance["global_designation"];
            return Columns.CreateColumn(title, 1, RenderDesignation);
        }

        private TreeViewColumn CreateSerialNumberColumn()
        {
            void RenderSerialNumber(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.SerialNumber;
            }
            var title = LocalizedString.Instance["global_serial_number"];
            return Columns.CreateColumn(title, 2, RenderSerialNumber);
        }

        private TreeViewColumn CreateStatusColumn()
        {
            void RenderStatus(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.Status.ToFriendlyString();
            }
            var title = "Estado";
            return Columns.CreateColumn(title, 3, RenderStatus);
        }

        private TreeViewColumn CreateIsComposedColumn()
        {
            void RenderIsComposed(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.IsComposed ? "Sim" : "Não";
            }
            var title = "Artigo Composto";
            return Columns.CreateColumn(title, 4, RenderIsComposed);
        }

        private TreeViewColumn CreatePurchaseDateColumn()
        {
            void RenderPurchaseDate(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.PurchaseDate?.Date.ToShortDateString();
            }
            var title = "Data de Compra";
            return Columns.CreateColumn(title, 5, RenderPurchaseDate);
        }

        private TreeViewColumn CreateProviderColumn()
        {
            void RenderProvider(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.Supplier;
            }
            var title = "Fornecedor";
            return Columns.CreateColumn(title, 6, RenderProvider);
        }

        private TreeViewColumn CreatePurchasePriceColumn()
        {
            void RenderPurchasePrice(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.PurchasePrice.ToString("F2");
            }
            var title = LocalizedString.Instance["global_purchase_price"];
            return Columns.CreateColumn(title, 7, RenderPurchasePrice);
        }

        private TreeViewColumn CreateOriginDocumentColumn()
        {
            void RenderOriginDocument(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.OriginDocument;
            }
            var title = "Documento de Origem";
            return Columns.CreateColumn(title, 8, RenderOriginDocument);
        }

        private TreeViewColumn CreateSaleDocumentColumn()
        {
            void RenderSaleDocument(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.SaleDocument;
            }
            var title = "Documento de Venda";
            return Columns.CreateColumn(title, 9, RenderSaleDocument);
        }

        private TreeViewColumn CreateWarehouseColumn()
        {
            void RenderWarehouse(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.Warehouse;
            }

            var title = LocalizedString.Instance["global_warehouse"];
            return Columns.CreateColumn(title, 10, RenderWarehouse);
        }

        private TreeViewColumn CreateLocationColumn()
        {
            void RenderLocation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.WarehouseLocation;
            }
            var title = LocalizedString.Instance["global_ConfigurationDevice_PlaceTerminal"];
            return Columns.CreateColumn(title, 11, RenderLocation);
        }

        private TreeViewColumn CreateUpdatedAtColumn()
        {
            void RenderUpdatedAt(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.UpdatedAt.ToLocalTime().ToString();
            }
            var title = LocalizedString.Instance["global_record_date_updated"];
            return Columns.CreateColumn(title, 12, RenderUpdatedAt);
        }

        private TreeViewColumn CreateSelectColumn()
        {
            TreeViewColumn selectColumn = new TreeViewColumn();

            var selectCellRenderer = new CellRendererToggle();
            selectColumn.PackStart(selectCellRenderer, true);

            selectCellRenderer.Toggled += CheckBox_Clicked;

            void RenderSelect(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererToggle).Active = SelectedHistories.Contains(history);
            }

            selectColumn.SetCellDataFunc(selectCellRenderer, RenderSelect);

            return selectColumn;
        }

        #endregion

        #region Sorting
        //private void AddWarehouseSorting()
        //{
        //    GridViewSettings.Sort.SetSortFunc(1, (model, left, right) =>
        //    {
        //        var a = (ArticleHistory)model.GetValue(left, 0);
        //        var b = (ArticleHistory)model.GetValue(right, 0);

        //        if (a == null || b == null)
        //        {
        //            return 0;
        //        }

        //        return a.WarehouseLocation.Warehouse.Designation.CompareTo(b.WarehouseLocation.Warehouse.Designation);
        //    });
        //}

        //private void AddLocationSorting()
        //{
        //    GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
        //    {
        //        var a = (ArticleHistory)model.GetValue(left, 0);
        //        var b = (ArticleHistory)model.GetValue(right, 0);
        //        if (a == null || b == null)
        //        {
        //            return 0;
        //        }
        //        return a.WarehouseLocation.Designation.CompareTo(b.WarehouseLocation.Designation);
        //    });
        //}

        //private void AddDesignationSorting()
        //{
        //    GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
        //    {
        //        var a = (ArticleHistory)model.GetValue(left, 0);
        //        var b = (ArticleHistory)model.GetValue(right, 0);
        //        if (a == null || b == null)
        //        {
        //            return 0;
        //        }
        //        return a.Article.Designation.CompareTo(b.Article.Designation);
        //    });
        //}

        //private void AddSerialNumberSorting()
        //{
        //    GridViewSettings.Sort.SetSortFunc(4, (model, left, right) =>
        //    {
        //        var a = (ArticleHistory)model.GetValue(left, 0);
        //        var b = (ArticleHistory)model.GetValue(right, 0);
        //        if (a == null || b == null || a.SerialNumber == null || b.SerialNumber == null)
        //        {
        //            return 0;
        //        }
        //        return a.SerialNumber.CompareTo(b.SerialNumber);
        //    });
        //}

        //private void AddQuantitySorting()
        //{
        //    GridViewSettings.Sort.SetSortFunc(5, (model, left, right) =>
        //    {
        //        var a = (ArticleHistory)model.GetValue(left, 0);
        //        var b = (ArticleHistory)model.GetValue(right, 0);
        //        if (a == null || b == null)
        //        {
        //            return 0;
        //        }
        //        return a.Quantity.CompareTo(b.Quantity);
        //    });
        //}

        #endregion

        #region Filter
        protected override void InitializeFilter()
        {
            GridViewSettings.Filter = new TreeModelFilter(GridViewSettings.Model, null);
            GridViewSettings.Filter.VisibleFunc = (model, iterator) =>
            {
                var search = Navigator.SearchBox.SearchText.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(search))
                {
                    return true;
                }

                var entity = model.GetValue(iterator, 0) as ArticleHistory;

                if (entity != null && entity.Article.ToLower().Contains(search))
                {
                    return true;
                }

                return false;
            };
        }
        #endregion
    }
}
