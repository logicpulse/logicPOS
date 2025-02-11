using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Enums;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Pages.GridViews;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ArticleHistoryPage
    {
        #region Creators
        private TreeViewColumn CreateDesignationColumn()
        {
            void RenderDesignation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.WarehouseArticle.Article.Designation;
            }
            var title = LocalizedString.Instance["global_designation"];
            return Columns.CreateColumn(title, 1, RenderDesignation);
        }

        private TreeViewColumn CreateSerialNumberColumn()
        {
            void RenderSerialNumber(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.WarehouseArticle.SerialNumber;
            }
            var title = LocalizedString.Instance["global_serial_number"];
            return Columns.CreateColumn(title, 2, RenderSerialNumber);
        }

        private TreeViewColumn CreateStatusColumn()
        {
            void RenderStatus(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.WarehouseArticle.Status.ToFriendlyString();
            }
            var title = "Estado";
            return Columns.CreateColumn(title, 3, RenderStatus);
        }

        private TreeViewColumn CreateIsComposedColumn()
        {
            void RenderIsComposed(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.WarehouseArticle.Article.IsComposed ? "Sim" : "Não";
            }
            var title = "Artigo Composto";
            return Columns.CreateColumn(title, 4, RenderIsComposed);
        }

        private TreeViewColumn CreatePurchaseDateColumn()
        {
            void RenderPurchaseDate(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.OutStockMovement?.Date.ToString();
            }
            var title = "Data de Compra";
            return Columns.CreateColumn(title, 5, RenderPurchaseDate);
        }

        private TreeViewColumn CreateProviderColumn()
        {
            void RenderProvider(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.InStockMovement.Customer.Name;
            }
            var title = "Fornecedor";
            return Columns.CreateColumn(title, 6, RenderProvider);
        }

        private TreeViewColumn CreatePurchasePriceColumn()
        {
            void RenderPurchasePrice(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.InStockMovement.Price.ToString();
            }
            var title = LocalizedString.Instance["global_purchase_price"];
            return Columns.CreateColumn(title, 7, RenderPurchasePrice);
        }

        private TreeViewColumn CreateOriginDocumentColumn()
        {
            void RenderOriginDocument(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.InStockMovement.DocumentNumber;
            }
            var title = "Documento de Origem";
            return Columns.CreateColumn(title, 8, RenderOriginDocument);
        }

        private TreeViewColumn CreateSaleDocumentColumn()
        {
            void RenderSaleDocument(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.OutStockMovement?.DocumentNumber;
            }
            var title = "Documento de Venda";
            return Columns.CreateColumn(title, 9, RenderSaleDocument);
        }

        private TreeViewColumn CreateWarehouseColumn()
        {
            void RenderWarehouse(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.WarehouseArticle.WarehouseLocation.Warehouse.Designation;
            }

            var title = LocalizedString.Instance["global_warehouse"];
            return Columns.CreateColumn(title, 10, RenderWarehouse);
        }

        private TreeViewColumn CreateLocationColumn()
        {
            void RenderLocation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var history = (ArticleHistory)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = history.WarehouseArticle.WarehouseLocation.Designation;
            }
            var title = LocalizedString.Instance["global_ConfigurationDevice_PlaceTerminal"];
            return Columns.CreateColumn(title, 11, RenderLocation);
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

                if (entity != null && entity.WarehouseArticle.Article.Designation.ToLower().Contains(search))
                {
                    return true;
                }

                return false;
            };
        }
        #endregion
    }
}
