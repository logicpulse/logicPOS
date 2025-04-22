using ErrorOr;
using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Printers.DeletePrinter;
using LogicPOS.Api.Features.Printers.GetAllPrinters;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using MediatR;
using System.Collections.Generic;
using Printer = LogicPOS.Api.Entities.Printer;

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

            var title = GeneralUtils.GetResourceByName("global_printer_type");
            return Columns.CreateColumn(title, 3, RenderPlace);
        }

        
    }
}
