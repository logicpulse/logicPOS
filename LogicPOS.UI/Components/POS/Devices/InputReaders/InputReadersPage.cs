using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.InputReaders.DeleteInputReader;
using LogicPOS.Api.Features.InputReaders.GetAllInputReaders;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System.Collections.Generic;


namespace LogicPOS.UI.Components.Pages
{
    public class InputReadersPage : Page<InputReader>
    {
       
        protected override IRequest<ErrorOr<IEnumerable<InputReader>>> GetAllQuery => new GetAllInputReadersQuery();
        public InputReadersPage(Window parent) : base(parent)
        {
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new InputReaderModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddUpdatedAtSorting(2);
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeleteInputReaderCommand(SelectedEntity.Id);
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONINPUTREADER_CREATE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONINPUTREADER_EDIT");
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONINPUTREADER_DELETE");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONINPUTREADER_VIEW");
        }

        #region Singleton
        private static InputReadersPage _instance;
        public static InputReadersPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InputReadersPage(null);
                }
                return _instance;
            }
        }
        #endregion
    }
}
