using Gtk;
using LogicPOS.Api.Features.Finance.Agt.ListOnlineSeries;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.UI.Components.Pages.GridViews;

namespace LogicPOS.UI.Components.Pages
{
    public partial class AgtSeriesPage
    {
        protected override void AddColumns()
        {
            GridView.AppendColumn(CreateCodeColumn());
            GridView.AppendColumn(CreateDocumentTypeColumn());
            GridView.AppendColumn(CreateStatusColumn());
            GridView.AppendColumn(CreateYearColumn());
            GridView.AppendColumn(CreateCreationDateColumn());
            GridView.AppendColumn(CreateInvoicingMethodColumn());
            GridView.AppendColumn(CreateJoiningDateColumn());
            GridView.AppendColumn(CreateJoiningTypeColumn());
        }


        private void AddDocumentTypeSorting()
        {
            GridViewSettings.Sort.SetSortFunc(1, (model, left, right) =>
            {
                var a = (OnlineSeriesInfo)model.GetValue(left, 0);
                var b = (OnlineSeriesInfo)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.DocumentType.CompareTo(b.DocumentType);
            });
        }


        private void AddStatusSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var a = (OnlineSeriesInfo)model.GetValue(left, 0);
                var b = (OnlineSeriesInfo)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.Status.CompareTo(b.Status);
            });
        }

        private void AddYearSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var a = (OnlineSeriesInfo)model.GetValue(left, 0);
                var b = (OnlineSeriesInfo)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.Year.CompareTo(b.Year);
            });
        }

        private void AddSeriesCreationDateSorting()
        {
            GridViewSettings.Sort.SetSortFunc(4, (model, left, right) =>
            {
                var a = (OnlineSeriesInfo)model.GetValue(left, 0);
                var b = (OnlineSeriesInfo)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.SeriesCreationDate.CompareTo(b.SeriesCreationDate);
            });
        }

        private void AddInvoicingMethodSorting()
        {
            GridViewSettings.Sort.SetSortFunc(5, (model, left, right) =>
            {
                var a = (OnlineSeriesInfo)model.GetValue(left, 0);
                var b = (OnlineSeriesInfo)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.InvoicingMethod.CompareTo(b.InvoicingMethod);
            });
        }

        private void AddJoiningDateSorting()
        {
            GridViewSettings.Sort.SetSortFunc(6, (model, left, right) =>
            {
                var a = (OnlineSeriesInfo)model.GetValue(left, 0);
                var b = (OnlineSeriesInfo)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.JoiningDate.CompareTo(b.JoiningDate);
            });
        }

        private void AddJoiningTypeSorting()
        {
            GridViewSettings.Sort.SetSortFunc(7, (model, left, right) =>
            {
                var a = (OnlineSeriesInfo)model.GetValue(left, 0);
                var b = (OnlineSeriesInfo)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.JoiningType.CompareTo(b.JoiningType);
            });
        }


        private TreeViewColumn CreateCodeColumn()
        {
            void RenderCode(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var series = (OnlineSeriesInfo)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = series.Code;
                (cell as CellRendererText).Foreground = "red";
            }

            var title = "Código";
            return Columns.CreateColumn(title, 0, RenderCode);
        }

        private TreeViewColumn CreateDocumentTypeColumn()
        {
            void RenderDocType(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                cell.Xpad = 10;
                var series = (OnlineSeriesInfo)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = series.DocumentType;
            }

            var title = "Tipo";
            return Columns.CreateColumn(title, 1, RenderDocType);
        }

        private TreeViewColumn CreateStatusColumn()
        {
            void RenderStatus(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                cell.Xpad = 10;
                var series = (OnlineSeriesInfo)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = series.Status;
            }

            var title = "Estado";
            return Columns.CreateColumn(title, 2, RenderStatus);
        }

        private TreeViewColumn CreateYearColumn()
        {
            void RenderYear(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                cell.Xpad = 10;
                var series = (OnlineSeriesInfo)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = series.Year;
                column.Alignment = 0.5f;
            }

            var title = "Ano";
            return Columns.CreateColumn(title, 3, RenderYear);
        }

        private TreeViewColumn CreateCreationDateColumn()
        {
            void RenderYear(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                cell.Xpad = 10;
                var series = (OnlineSeriesInfo)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = series.SeriesCreationDate;
                column.Alignment = 0.5f;
            }

            var title = "Data de Criação";
            return Columns.CreateColumn(title, 4, RenderYear);
        }

        private TreeViewColumn CreateInvoicingMethodColumn()
        {
            void RenderYear(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                cell.Xpad = 10;
                var series = (OnlineSeriesInfo)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = series.InvoicingMethod;
            }

            var title = "Método de Facturação";
            return Columns.CreateColumn(title, 5, RenderYear);
        }

        private TreeViewColumn CreateJoiningDateColumn()
        {
            void RenderYear(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                cell.Xpad = 10;
                var series = (OnlineSeriesInfo)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = series.JoiningDate;
                column.Alignment = 0.5f;
                cell.Xalign = 1;
            }

            var title = "Data de Adesão";
            return Columns.CreateColumn(title, 6, RenderYear);
        }

        private TreeViewColumn CreateJoiningTypeColumn()
        {
            void RenderYear(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                cell.Xpad = 10;
                var series = (OnlineSeriesInfo)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = series.JoiningType;
                column.Alignment = 1;
                cell.Xalign = 1;
            }

            var title = "Tipo de Adesão";
            return Columns.CreateColumn(title, 7, RenderYear);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);
            AddDocumentTypeSorting();
            AddStatusSorting();
            AddYearSorting();
            AddSeriesCreationDateSorting();
            AddInvoicingMethodSorting();
            AddJoiningDateSorting();
            AddJoiningTypeSorting();
        }
    }
}
