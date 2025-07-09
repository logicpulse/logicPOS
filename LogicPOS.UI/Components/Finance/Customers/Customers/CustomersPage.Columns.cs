using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Pages
{
    public partial class CustomersPage
    {
        private TreeViewColumn CreateNameColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var user = (Customer)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = user.Name;
            }

            var title = GeneralUtils.GetResourceByName("global_users");
            return Columns.CreateColumn(title, 1, RenderValue);
        }

        private TreeViewColumn CreateCardNumberColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var customer = (Customer)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = customer.CardNumber;
            }

            var title = GeneralUtils.GetResourceByName("global_card_number");
            return Columns.CreateColumn(title, 3, RenderValue);
        }

        private TreeViewColumn CreateFiscalNumberColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var user = (Customer)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = user.FiscalNumber;
            }

            var title = GeneralUtils.GetResourceByName("global_fiscal_number");
            return Columns.CreateColumn(title, 2, RenderValue);
        }

    }
}
