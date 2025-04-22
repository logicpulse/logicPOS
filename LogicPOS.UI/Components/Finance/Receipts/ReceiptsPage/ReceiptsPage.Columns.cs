using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ReceiptsPage
    {
        private TreeViewColumn CreateRelatedReceiptsColumn()
        {
            void RenderRelatedReceipts(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var receipt = ((Receipt)model.GetValue(iter, 0));
                (cell as CellRendererText).Text = receipt.Notes;
            }

            var title = GeneralUtils.GetResourceByName("window_title_dialog_document_finance_column_related_doc");
            return Columns.CreateColumn(title, 8, RenderRelatedReceipts);
        }

        private TreeViewColumn CreateTotalColumn()
        {
            void RenderAmountColumn(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var receipt = ((Receipt)model.GetValue(iter, 0));
                (cell as CellRendererText).Text = receipt.Amount.ToString("0.00");
            }

            var title = GeneralUtils.GetResourceByName("global_total");
            return Columns.CreateColumn(title, 7, RenderAmountColumn);
        }

        private TreeViewColumn CreateFiscalNumberColumn()
        {
            void RenderFiscalNumber(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var receipt = (Receipt)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = receipt.CustomerFiscalNumber;
            }

            var title = GeneralUtils.GetResourceByName("global_fiscal_number");
            return Columns.CreateColumn(title, 6, RenderFiscalNumber);
        }

        private TreeViewColumn CreateEntityColumn()
        {
            void RenderEntity(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var receipt = (Receipt)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = receipt.Customer;
            }

            var title = GeneralUtils.GetResourceByName("global_entity");
            return Columns.CreateColumn(title, 5, RenderEntity);
        }

        private TreeViewColumn CreateStatusColumn()
        {
            void RenderStatus(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var receipt = (Receipt)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = receipt.Status;
            }

            var title = GeneralUtils.GetResourceByName("global_document_status");
            return Columns.CreateColumn(title, 4, RenderStatus);
        }

        private TreeViewColumn CreateDateColumn()
        {
            void RenderDate(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var receipt = (Receipt)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = receipt.CreatedAt.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_document_date");
            return Columns.CreateColumn(title, 2, RenderDate);
        }

        private TreeViewColumn CreateNumberColumn()
        {
            void RenderNumber(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var receipt = (Receipt)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = receipt.RefNo;
            }

            var title = GeneralUtils.GetResourceByName("global_document_number");
            return Columns.CreateColumn(title, 3, RenderNumber);
        }

        private TreeViewColumn CreateSelectColumn()
        {
            TreeViewColumn selectColumn = new TreeViewColumn();

            var selectCellRenderer = new CellRendererToggle();
            selectColumn.PackStart(selectCellRenderer, true);

            selectCellRenderer.Toggled += CheckBox_Clicked;

            void RenderSelect(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var receipt = (Receipt)model.GetValue(iter, 0);
                (cell as CellRendererToggle).Active = SelectedReceipts.Contains(receipt);
            }

            selectColumn.SetCellDataFunc(selectCellRenderer, RenderSelect);

            return selectColumn;
        }
    }
}
