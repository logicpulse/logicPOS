using Gtk;
using LogicPOS.Api.Features.Finance.Agt.ListSeries;
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

        private TreeViewColumn CreateCodeColumn()
        {
            void RenderCode(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var series = (AgtSeriesInfo)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = series.Code;
            }

            var title = "Código";
            return Columns.CreateColumn(title, 0, RenderCode);
        }

        private TreeViewColumn CreateDocumentTypeColumn()
        {
            void RenderDocType(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var series = (AgtSeriesInfo)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = series.DocumentType;
            }

            var title = "Tipo";
            return Columns.CreateColumn(title, 1, RenderDocType);
        }

        private TreeViewColumn CreateStatusColumn()
        {
            void RenderStatus(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var series = (AgtSeriesInfo)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = series.Status;
            }

            var title = "Estado";
            return Columns.CreateColumn(title, 2, RenderStatus);
        }

        private TreeViewColumn CreateYearColumn()
        {
            void RenderYear(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var series = (AgtSeriesInfo)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = series.Year;
            }

            var title = "Ano";
            return Columns.CreateColumn(title, 3, RenderYear);
        }

        private TreeViewColumn CreateCreationDateColumn()
        {
            void RenderYear(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var series = (AgtSeriesInfo)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = series.SeriesCreationDate;
            }

            var title = "Data de Criação";
            return Columns.CreateColumn(title, 4, RenderYear);
        }

        private TreeViewColumn CreateInvoicingMethodColumn()
        {
            void RenderYear(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var series = (AgtSeriesInfo)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = series.InvoicingMethod;
            }

            var title = "Método de Facturação";
            return Columns.CreateColumn(title, 5, RenderYear);
        }

        private TreeViewColumn CreateJoiningDateColumn()
        {
            void RenderYear(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var series = (AgtSeriesInfo)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = series.JoiningDate;
                column.Alignment = 1;
                cell.Xalign = 1;
            }

            var title = "Data de Adesão";
            return Columns.CreateColumn(title, 6, RenderYear);
        }

        private TreeViewColumn CreateJoiningTypeColumn()
        {
            void RenderYear(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var series = (AgtSeriesInfo)model.GetValue(iter, 0);
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
        }
    }
}
