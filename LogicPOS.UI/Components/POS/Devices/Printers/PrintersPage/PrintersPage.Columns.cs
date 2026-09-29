using Gtk;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using Printer = LogicPOS.Api.Entities.Printer;
using LogicPOS.Globalization;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PrintersPage : Page<Printer>
    {
        
        private TreeViewColumn CreatePrinterTypeColumn()
        {
            void RenderPlace(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var table = (Printer)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = table.Type.Designation;
            }

            var title = LocalizedString.Instance["global_printer_type"];
            return Columns.CreateColumn(title, 3, RenderPlace);
        }

        
    }
}
