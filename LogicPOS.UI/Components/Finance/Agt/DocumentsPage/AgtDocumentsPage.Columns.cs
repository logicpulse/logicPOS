using Gtk;
using LogicPOS.Api.Features.Finance.Agt.ListOnlineDocuments;
using LogicPOS.UI.Components.Pages.GridViews;

namespace LogicPOS.UI.Components.Pages
{
    public partial class AgtDocumentsPage
    {
        protected override void AddColumns()
        {
            GridView.AppendColumn(CreateDocumentNumberColumn());
            GridView.AppendColumn(CreateDocumentTypeColumn());
            GridView.AppendColumn(CreateDocumentDateColumn());
            GridView.AppendColumn(CreateDocumentStatusColumn());
            GridView.AppendColumn(CreateNetTotalColumn());
        }

        private void AddDocumentNumberSorting()
        {
            GridViewSettings.Sort.SetSortFunc(0, (model, left, right) =>
            {
                var a = (OnlineDocument)model.GetValue(left, 0);
                var b = (OnlineDocument)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.Number.CompareTo(b.Number);
            });
        }
        private void AddDocumentTypeSorting()
        {
            GridViewSettings.Sort.SetSortFunc(1, (model, left, right) =>
            {
                var a = (OnlineDocument)model.GetValue(left, 0);
                var b = (OnlineDocument)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.Type.CompareTo(b.Type);
            });
        }
        private void AddDocumentDateSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var a = (OnlineDocument)model.GetValue(left, 0);
                var b = (OnlineDocument)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.Date.CompareTo(b.Date);
            });
        }
        private void AddDocumentStatusSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var a = (OnlineDocument)model.GetValue(left, 0);
                var b = (OnlineDocument)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.Status.CompareTo(b.Status);
            });
        }
        private void AddDocumentTotalSorting()
        {
            GridViewSettings.Sort.SetSortFunc(4, (model, left, right) =>
            {
                var a = (OnlineDocument)model.GetValue(left, 0);
                var b = (OnlineDocument)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.NetTotal.CompareTo(b.NetTotal);
            });
        }
        private TreeViewColumn CreateDocumentNumberColumn()
        {
            void Render(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var inv = (OnlineDocument)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = inv.Number;
            }

            var title = "Número";
            return Columns.CreateColumn(title, 0, Render);
        }
        private TreeViewColumn CreateDocumentTypeColumn()
        {
            void Render(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var inv = (OnlineDocument)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = inv.Type;
            }

            var title = "Tipo";
            return Columns.CreateColumn(title, 1, Render);
        }
        private TreeViewColumn CreateDocumentDateColumn()
        {
            void Render(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var inv = (OnlineDocument)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = inv.Date;
            }

            var title = "Data";
            return Columns.CreateColumn(title, 2, Render);
        }
        private TreeViewColumn CreateDocumentStatusColumn()
        {
            void Render(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var inv = (OnlineDocument)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = inv.Status;
            }

            var title = "Estado";
            return Columns.CreateColumn(title, 3, Render);
        }
        private TreeViewColumn CreateNetTotalColumn()
        {
            void Render(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var inv = (OnlineDocument)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = inv.NetTotal;
                column.Alignment = 1;
                cell.Xalign = 1;
            }

            var title = "Total";
            return Columns.CreateColumn(title, 4, Render);
        }
        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);
            AddDocumentNumberSorting();
            AddDocumentTypeSorting();
            AddDocumentDateSorting();
            AddDocumentStatusSorting();
            AddDocumentTotalSorting();
        }
    }
}
