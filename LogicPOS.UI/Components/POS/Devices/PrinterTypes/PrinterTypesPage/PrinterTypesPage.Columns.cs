using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PrinterTypesPage
    {
        private TreeViewColumn CreateThermalPrinterColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var printerType = (PrinterType)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = printerType.ThermalPrinter ? GeneralUtils.GetResourceByName("global_treeview_true") : GeneralUtils.GetResourceByName("global_treeview_false");
            }

            var title = GeneralUtils.GetResourceByName("global_printer_thermal_printer");
            return Columns.CreateColumn(title, 2, RenderValue);
        }
    }
}
