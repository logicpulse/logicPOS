using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Pages.GridViews;

namespace LogicPOS.UI.Components.Pages
{
    public partial class WarehouseArticlesPage
    {
        #region Creators
        private TreeViewColumn CreateWarehouseColumn()
        {
            void RenderWarehouse(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var article = (WarehouseArticle)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = article.WarehouseLocation.Warehouse.Designation;
            }

            var title = LocalizedString.Instance["global_warehouse"];
            return Columns.CreateColumn(title, 1, RenderWarehouse);
        }

        private TreeViewColumn CreateLocationColumn()
        {
            void RenderLocation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var article = (WarehouseArticle)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = article.WarehouseLocation.Designation;
            }
            var title = LocalizedString.Instance["global_ConfigurationDevice_PlaceTerminal"];
            return Columns.CreateColumn(title, 2, RenderLocation);
        }

        private TreeViewColumn CreateDesignationColumn()
        {
            void RenderDesignation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var article = (WarehouseArticle)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = article.Article.Designation;
            }
            var title = LocalizedString.Instance["global_designation"];
            return Columns.CreateColumn(title, 3, RenderDesignation);
        }

        private TreeViewColumn CreateSerialNumberColumn()
        {
            void RenderSerialNumber(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var article = (WarehouseArticle)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = article.SerialNumber;
            }
            var title = LocalizedString.Instance["global_serial_number"];
            return Columns.CreateColumn(title, 4, RenderSerialNumber);
        }

        private TreeViewColumn CreateQuantityColumn()
        {
            void RenderQuantity(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var article = (WarehouseArticle)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = article.Quantity.ToString();
            }
            var title = LocalizedString.Instance["global_quantity"];
            return Columns.CreateColumn(title, 5, RenderQuantity);
        }

        #endregion

        #region Sorting
        private void AddWarehouseSorting()
        {
            GridViewSettings.Sort.SetSortFunc(1, (model, left, right) =>
            {
                var a = (WarehouseArticle)model.GetValue(left, 0);
                var b = (WarehouseArticle)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.WarehouseLocation.Warehouse.Designation.CompareTo(b.WarehouseLocation.Warehouse.Designation);
            });
        }

        private void AddLocationSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var a = (WarehouseArticle)model.GetValue(left, 0);
                var b = (WarehouseArticle)model.GetValue(right, 0);
                if (a == null || b == null)
                {
                    return 0;
                }
                return a.WarehouseLocation.Designation.CompareTo(b.WarehouseLocation.Designation);
            });
        }

        private void AddDesignationSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var a = (WarehouseArticle)model.GetValue(left, 0);
                var b = (WarehouseArticle)model.GetValue(right, 0);
                if (a == null || b == null)
                {
                    return 0;
                }
                return a.Article.Designation.CompareTo(b.Article.Designation);
            });
        }

        private void AddSerialNumberSorting()
        {
            GridViewSettings.Sort.SetSortFunc(4, (model, left, right) =>
            {
                var a = (WarehouseArticle)model.GetValue(left, 0);
                var b = (WarehouseArticle)model.GetValue(right, 0);
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
                var a = (WarehouseArticle)model.GetValue(left, 0);
                var b = (WarehouseArticle)model.GetValue(right, 0);
                if (a == null || b == null)
                {
                    return 0;
                }
                return a.Quantity.CompareTo(b.Quantity);
            });
        }

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

                var entity = model.GetValue(iterator, 0) as WarehouseArticle;

                if (entity != null && entity.Article.Designation.ToLower().Contains(search))
                {
                    return true;
                }

                return false;
            };
        }
        #endregion
    }
}
