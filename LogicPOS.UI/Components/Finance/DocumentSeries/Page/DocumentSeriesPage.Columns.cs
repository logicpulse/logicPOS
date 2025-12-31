using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentSeriesPage
    {
        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(CreateFiscalYearColumn());
            GridView.AppendColumn(CreateDocumentTypeColumn());
            GridView.AppendColumn(Columns.CreateDesignationColumn(3));
            GridView.AppendColumn(CreateTerminalColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(5));
        }

        private TreeViewColumn CreateFiscalYearColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var terminal = (DocumentSeries)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = terminal.FiscalYear.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_fiscal_year");
            return Columns.CreateColumn(title, 1, RenderValue);
        }

        private TreeViewColumn CreateTerminalColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var terminal = (DocumentSeries)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = terminal.Terminal?.Designation ?? "-";
            }

            var title = GeneralUtils.GetResourceByName("global_terminal");
            return Columns.CreateColumn(title, 4, RenderValue);
        }

        private TreeViewColumn CreateDocumentTypeColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var terminal = (DocumentSeries)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = terminal.DocumentType.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_documentfinance_type");
            return Columns.CreateColumn(title, 2, RenderValue);
        }
    }
}
