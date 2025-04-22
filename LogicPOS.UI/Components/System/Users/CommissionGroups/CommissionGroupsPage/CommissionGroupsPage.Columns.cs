using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Pages
{
    public partial class CommissionGroupsPage
    {
        private TreeViewColumn CreateCommissionColumn()
        {
            void RenderCommission(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var commissionGroup = (CommissionGroup)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = commissionGroup.Commission.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_commission");
            return Columns.CreateColumn(title, 2, RenderCommission);

        }
    }
}
