using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents.Series.GetAllDocumentSeries;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Services;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentSeriesPage : Page<DocumentSeries>
    {
        protected override IRequest<ErrorOr<IEnumerable<DocumentSeries>>> GetAllQuery => new GetActiveDocumentSeriesQuery { AllTerminals = true };
        public DocumentSeriesPage(Window parent) : base(parent)
        {
            Navigator.BtnDelete.Visible = false;
            Navigator.BtnUpdate.Visible = false;
            Navigator.BtnInsert.Visible = !(SystemInformationService.UseAgtFe);
            DisableFilterButton();
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            if (mode == EntityEditionModalMode.Update && CountriesService.Default.Code2 != "PT")
            {
                mode = EntityEditionModalMode.View;
            }

            var modal = new DocumentSerieModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

  
        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddFiscalYearSorting();
            AddDocumentTypeSorting();
            AddDesignationSorting(3);
            AddUpdatedAtSorting(4);
        }


        protected override DeleteCommand GetDeleteCommand()
        {
            return null;
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCESERIES_EDIT");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCESERIES_VIEW");
        }

        #region Singleton
        private static DocumentSeriesPage _instance;
        public static DocumentSeriesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DocumentSeriesPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }
        #endregion
    }
}
