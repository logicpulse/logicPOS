using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Printers.GetAllPrinters;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;
using Printer = LogicPOS.Api.Entities.Printer;

namespace LogicPOS.UI.Components.Pages
{
    public class PrintersPage : Page<Printer>
    {
        public PrintersPage(Window parent) : base(parent)
        {
        }

        protected override IRequest<ErrorOr<IEnumerable<Printer>>> GetAllQuery => new GetAllPrintersQuery();

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new PrinterModal(mode, SelectedEntity);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(2));
            GridView.AppendColumn(CreatePrinterTypeColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
        }

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


        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(2);
            AddPrinterTypeSorting();
            AddUpdatedAtSorting(4);
        }

        private void AddPrinterTypeSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftPrinter = (Printer)model.GetValue(left, 0);
                var rightPrinter = (Printer)model.GetValue(right, 0);

                if (leftPrinter == null || rightPrinter == null)
                {
                    return 0;
                }

                return leftPrinter.Type.Designation.CompareTo(rightPrinter.Type.Designation);
            });
        }
    }
}
