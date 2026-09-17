using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using LogicPOS.Globalization;

namespace LogicPOS.UI.Components.Pages
{
    public partial class UsersPage
    {
        private TreeViewColumn CreateNameColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var user = (User)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = user.Name;
            }

            var title = LocalizedString.Instance["global_users"];
            return Columns.CreateColumn(title, 1, RenderValue);
        }

        private TreeViewColumn CreateProfileColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var user = (User)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = user.Profile.Designation;
            }

            var title = LocalizedString.Instance["global_profile"];
            return Columns.CreateColumn(title, 2, RenderValue);
        }

        private TreeViewColumn CreateFiscalNumberColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var user = (User)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = user.FiscalNumber;
            }

            var title = LocalizedString.Instance["global_fiscal_number"];
            return Columns.CreateColumn(title, 3, RenderValue);
        }
    }
}
