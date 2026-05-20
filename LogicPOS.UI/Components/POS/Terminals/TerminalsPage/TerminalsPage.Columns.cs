using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using LogicPOS.Globalization;

namespace LogicPOS.UI.Components.Pages
{
    public partial class TerminalsPage
    {
        private TreeViewColumn CreateHardwareIdColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var terminal = (Terminal)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = terminal.HardwareId;
            }

            var title = LocalizedString.Instance["global_hardware_id"];
            return Columns.CreateColumn(title, 2, RenderValue);
        }
    }
}
