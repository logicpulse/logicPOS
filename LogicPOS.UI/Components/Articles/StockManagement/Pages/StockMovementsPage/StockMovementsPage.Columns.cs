using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Pages.GridViews;

namespace LogicPOS.UI.Components.Pages
{
    public partial class StockMovementsPage 
    {
        #region Creators
        private TreeViewColumn CreateMovementTypeColumn()
        {
            void RenderMovementType(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var movement = (StockMovement)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = movement.Quantity >= 0 ? "Entrada" : "Saída";
            }

            var title = LocalizedString.Instance["global_stock_movement"];
            return Columns.CreateColumn(title, 1, RenderMovementType);
        }

        private TreeViewColumn CreateDateColumn()
        {
            void RenderDate(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var movement = (StockMovement)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = movement.Date.ToShortDateString();
            }
            var title = LocalizedString.Instance["global_date"];
            return Columns.CreateColumn(title, 2, RenderDate);
        }

        private TreeViewColumn CreateEntityColumn()
        {
            void RenderEntity(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var movement = (StockMovement)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = movement.Customer?.Name;
            }
            var title = LocalizedString.Instance["global_entity"];
            return Columns.CreateColumn(title, 3, RenderEntity);
        }

        private TreeViewColumn CreateDocumentNumberColumn()
        {
            void RenderDocumetNumber(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var movement = (StockMovement)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = movement.DocumentNumber;
            }
            var title = LocalizedString.Instance["global_document_number"];
            return Columns.CreateColumn(title, 4, RenderDocumetNumber);
        }

        private TreeViewColumn CreateArticleColumn()
        {
            void RenderArticle(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var movement = (StockMovement)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = movement.Article?.Designation ?? movement.Detail?.Designation;
            }
            var title = LocalizedString.Instance["global_article"];
            return Columns.CreateColumn(title, 5, RenderArticle);
        }

        private TreeViewColumn CreateQuantityColumn()
        {
            void RenderQuantity(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var movement = (StockMovement)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = movement.Quantity.ToString();
            }
            var title = LocalizedString.Instance["global_quantity"];
            return Columns.CreateColumn(title, 6, RenderQuantity);
        }

        #endregion

        #region Sorting
        private void AddMovementTypeSorting()
        {
            GridViewSettings.Sort.SetSortFunc(1, (model, left, right) =>
            {
                var a = (StockMovement)model.GetValue(left, 0);
                var b = (StockMovement)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.Quantity.CompareTo(b.Quantity);
            });
        }

        private void AddDateSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var a = (StockMovement)model.GetValue(left, 0);
                var b = (StockMovement)model.GetValue(right, 0);
                if (a == null || b == null)
                {
                    return 0;
                }
                return a.Date.CompareTo(b.Date);
            });
        }

        private void AddEntitySorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var a = (StockMovement)model.GetValue(left, 0);
                var b = (StockMovement)model.GetValue(right, 0);
                if (a == null || b == null || a.Customer == null || b.Customer == null)
                {
                    return 0;
                }
                return a.Customer.Name.CompareTo(b.Customer.Name);
            });
        }

        private void AddDocumentNumberSorting()
        {
            GridViewSettings.Sort.SetSortFunc(4, (model, left, right) =>
            {
                var a = (StockMovement)model.GetValue(left, 0);
                var b = (StockMovement)model.GetValue(right, 0);
                if (a == null || b == null || a.DocumentNumber == null || b.DocumentNumber == null)
                {
                    return 0;
                }
                return a.DocumentNumber.CompareTo(b.DocumentNumber);
            });
        }

        private void AddArticleSorting()
        {
            GridViewSettings.Sort.SetSortFunc(5, (model, left, right) =>
            {
                var a = (StockMovement)model.GetValue(left, 0);
                var b = (StockMovement)model.GetValue(right, 0);
                if (a == null || b == null || a.Article == null || b.Article == null)
                {
                    return 0;
                }
                return a.Article.Designation.CompareTo(b.Article.Designation);
            });
        }

        private void AddQuantitySorting()
        {
            GridViewSettings.Sort.SetSortFunc(6, (model, left, right) =>
            {
                var a = (StockMovement)model.GetValue(left, 0);
                var b = (StockMovement)model.GetValue(right, 0);
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

                var entity = model.GetValue(iterator, 0) as StockMovement;

                if (entity != null && entity.Customer.Name.ToLower().Contains(search))
                {
                    return true;
                }

                return false;
            };
        }
        #endregion
    }
}
