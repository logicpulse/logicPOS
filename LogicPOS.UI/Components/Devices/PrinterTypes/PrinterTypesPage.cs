using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.PrinterTypes.DeletePrinterType;
using LogicPOS.Api.Features.PrinterTypes.GetAllPrinterTypes;
using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;


namespace LogicPOS.UI.Components.Pages
{
    public class PrinterTypesPage : Page<PrinterType>
    {

        protected override IRequest<ErrorOr<IEnumerable<PrinterType>>> GetAllQuery => new GetAllPrinterTypesQuery();
        public PrinterTypesPage(Window parent) : base(parent)
        {
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new PrinterTypeModal(mode, SelectedEntity as PrinterType);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateThermalPrinterColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }

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

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddThermalPrinterSorting();
            AddUpdatedAtSorting(3);
        }

        private void AddThermalPrinterSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftPrinterType = (PrinterType)model.GetValue(left, 0);
                var rightPrinterType = (PrinterType)model.GetValue(right, 0);

                if (leftPrinterType == null || rightPrinterType == null)
                {
                    return 0;
                }

                return leftPrinterType.ThermalPrinter.CompareTo(rightPrinterType.ThermalPrinter);
            });
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeletePrinterTypeCommand(SelectedEntity.Id);
        }

        #region Signleton
        private static PrinterTypesPage _instance;
        public static PrinterTypesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PrinterTypesPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }
        #endregion
    }
}
