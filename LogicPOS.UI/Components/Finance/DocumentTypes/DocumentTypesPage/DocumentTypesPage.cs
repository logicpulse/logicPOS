using ErrorOr;
using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.DocumentTypes.GetAllDocumentTypes;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Errors;
using MediatR;
using System.Collections;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;


namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentTypesPage : Page<DocumentType>
    {
        public DocumentTypesPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
            Navigator.BtnInsert.Visible = false;
            Navigator.BtnDelete.Visible = false;
            Navigator.BtnUpdate.Visible = false;
        }

        protected override void LoadEntities()
        {
            var result = _mediator.Send(new GetAllDocumentTypesQuery()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result,
                                                    source: SourceWindow);
                return;
            }
            var entities = result.Value;

            if (IsSelectionPage())
            {
                entities = result.Value.Where(dc => dc.SaftDocumentType != SaftDocumentType.Payments);
            }

            _entities.Clear();
            _entities.AddRange(entities);
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            if (mode == EntityEditionModalMode.Insert)
            {
                return (int)ResponseType.None;
            }

            var modal = new DocumentTypeModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateAcronymColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddAcronymSorting();
            AddUpdatedAtSorting(3);
        }

        protected override DeleteCommand GetDeleteCommand() => null;

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_CREATE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_EDIT");
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_DELETE");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_VIEW");
        }

        #region Singleton
        private static DocumentTypesPage _instance;
        public static DocumentTypesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DocumentTypesPage(BackOfficeWindow.Instance);
                }

                return _instance;
            }
        }
        #endregion
    }

}
