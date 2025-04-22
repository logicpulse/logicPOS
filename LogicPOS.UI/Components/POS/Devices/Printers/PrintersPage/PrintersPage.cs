using ErrorOr;
using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Printers.DeletePrinter;
using LogicPOS.Api.Features.Printers.GetAllPrinters;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using MediatR;
using System.Collections.Generic;
using Printer = LogicPOS.Api.Entities.Printer;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PrintersPage : Page<Printer>
    {
        public PrintersPage(Window parent) : base(parent)
        {
        }

        protected override IRequest<ErrorOr<IEnumerable<Printer>>> GetAllQuery => new GetAllPrintersQuery();

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new PrinterModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(2));
            GridView.AppendColumn(CreatePrinterTypeColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
        }
        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(2);
            AddPrinterTypeSorting();
            AddUpdatedAtSorting(4);
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeletePrinterCommand(SelectedEntity.Id);
        }

        #region Signleton
        private static PrintersPage _instance;
        public static PrintersPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PrintersPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }
        #endregion
    }
}
