using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using LogicPOS.Globalization;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PrinterTypesPage
    {
        private TreeViewColumn CreateThermalPrinterColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var printerType = (PrinterType)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = printerType.ThermalPrinter ? LocalizedString.Instance["global_treeview_true"] : LocalizedString.Instance["global_treeview_false"];
            }

            var title = LocalizedString.Instance["global_printer_thermal_printer"];
            return Columns.CreateColumn(title, 2, RenderValue);
        }
    }
}
