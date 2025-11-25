using Gtk;
using LogicPOS.Api.Features.Finance.Agt.ListInvoices;
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

        private TreeViewColumn CreateDocumentNumberColumn()
        {
            void Render(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var inv = (AgtInvoice)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = inv.DocumentNumber;
            }

            var title = "Número";
            return Columns.CreateColumn(title, 0, Render);
        }

        private TreeViewColumn CreateDocumentTypeColumn()
        {
            void Render(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var inv = (AgtInvoice)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = inv.DocumentType;
            }

            var title = "Tipo";
            return Columns.CreateColumn(title, 1, Render);
        }

        private TreeViewColumn CreateDocumentDateColumn()
        {
            void Render(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var inv = (AgtInvoice)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = inv.DocumentDate;
            }

            var title = "Data";
            return Columns.CreateColumn(title, 2, Render);
        }

        private TreeViewColumn CreateDocumentStatusColumn()
        {
            void Render(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var inv = (AgtInvoice)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = inv.DocumentStatus;
            }

            var title = "Estado";
            return Columns.CreateColumn(title, 3, Render);
        }

        private TreeViewColumn CreateNetTotalColumn()
        {
            void Render(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var inv = (AgtInvoice)model.GetValue(iter, 0);
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
        }
    }
}
