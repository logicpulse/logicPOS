using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.PrinterTypes.DeletePrinterType;
using LogicPOS.Api.Features.PrinterTypes.GetAllPrinterTypes;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using MediatR;
using System.Collections.Generic;


namespace LogicPOS.UI.Components.Pages
{
    public partial class PrinterTypesPage : Page<PrinterType>
    {

        protected override IRequest<ErrorOr<IEnumerable<PrinterType>>> GetAllQuery => new GetAllPrinterTypesQuery();
        public PrinterTypesPage(Window parent) : base(parent)
        {
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new PrinterTypeModal(mode, SelectedEntity);
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

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddThermalPrinterSorting();
            AddUpdatedAtSorting(3);
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
