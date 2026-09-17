using Gtk;
using LogicPOS.Api.Features.Articles.Stocks.WarehouseArticles.Common;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Pages.GridViews;

namespace LogicPOS.UI.Components.Pages
{
    public partial class WarehouseArticlesPage
    {
        protected override void AddColumns()
        {
            GridView.AppendColumn(CreateWarehouseColumn());
            GridView.AppendColumn(CreateLocationColumn());
            GridView.AppendColumn(CreateDesignationColumn());
            GridView.AppendColumn(CreateSerialNumberColumn());
            GridView.AppendColumn(CreateQuantityColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(6));
        }

        #region Creators
        private TreeViewColumn CreateWarehouseColumn()
        {
            void RenderWarehouse(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var article = (WarehouseArticleViewModel)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = article.Warehouse;
            }

            var title = LocalizedString.Instance["global_warehouse"];
            return Columns.CreateColumn(title, 1, RenderWarehouse);
        }

        private TreeViewColumn CreateLocationColumn()
        {
            void RenderLocation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var article = (WarehouseArticleViewModel)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = article.Location;
            }
            var title = LocalizedString.Instance["global_ConfigurationDevice_PlaceTerminal"];
            return Columns.CreateColumn(title, 2, RenderLocation);
        }

        private TreeViewColumn CreateDesignationColumn()
        {
            void RenderDesignation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var article = (WarehouseArticleViewModel)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = article.Article;
            }
            var title = LocalizedString.Instance["global_designation"];
            return Columns.CreateColumn(title, 3, RenderDesignation);
        }

        private TreeViewColumn CreateSerialNumberColumn()
        {
            void RenderSerialNumber(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var article = (WarehouseArticleViewModel)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = article.SerialNumber;
            }
            var title = LocalizedString.Instance["global_serial_number"];
            return Columns.CreateColumn(title, 4, RenderSerialNumber);
        }

        private TreeViewColumn CreateQuantityColumn()
        {
            void RenderQuantity(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var article = (WarehouseArticleViewModel)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = article.Quantity.ToString("F2");
            }
            var title = LocalizedString.Instance["global_quantity"];
            return Columns.CreateColumn(title, 5, RenderQuantity);
        }

        #endregion


        #region Sorting
        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddWarehouseSorting();
            AddLocationSorting();
            AddDesignationSorting();
            AddSerialNumberSorting();
            AddQuantitySorting();
        }

        private void AddWarehouseSorting()
        {
            GridViewSettings.Sort.SetSortFunc(1, (model, left, right) =>
            {
                var a = (WarehouseArticleViewModel)model.GetValue(left, 0);
                var b = (WarehouseArticleViewModel)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.Warehouse.CompareTo(b.Warehouse);
            });
        }

        private void AddLocationSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var a = (WarehouseArticleViewModel)model.GetValue(left, 0);
                var b = (WarehouseArticleViewModel)model.GetValue(right, 0);
                if (a == null || b == null)
                {
                    return 0;
                }
                return a.Location.CompareTo(b.Location);
            });
        }

        private void AddDesignationSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var a = (WarehouseArticleViewModel)model.GetValue(left, 0);
                var b = (WarehouseArticleViewModel)model.GetValue(right, 0);
                if (a == null || b == null)
                {
                    return 0;
                }
                return a.Article.CompareTo(b.Article);
            });
        }

        private void AddSerialNumberSorting()
        {
            GridViewSettings.Sort.SetSortFunc(4, (model, left, right) =>
            {
                var a = (WarehouseArticleViewModel)model.GetValue(left, 0);
                var b = (WarehouseArticleViewModel)model.GetValue(right, 0);
                if (a == null || b == null || a.SerialNumber == null || b.SerialNumber == null)
                {
                    return 0;
                }
                return a.SerialNumber.CompareTo(b.SerialNumber);
            });
        }

        private void AddQuantitySorting()
        {
            GridViewSettings.Sort.SetSortFunc(5, (model, left, right) =>
            {
                var a = (WarehouseArticleViewModel)model.GetValue(left, 0);
                var b = (WarehouseArticleViewModel)model.GetValue(right, 0);
                if (a == null || b == null)
                {
                    return 0;
                }
                return a.Quantity.CompareTo(b.Quantity);
            });
        }

        #endregion
    }
}
