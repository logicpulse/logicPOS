using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.PoleDisplays.DeletePoleDisplay;
using LogicPOS.Api.Features.PoleDisplays.GetAllPoleDisplays;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class PoleDisplaysPage : Page<PoleDisplay>
    {
        public PoleDisplaysPage(Window parent) : base(parent)
        {
        }

        protected override IRequest<ErrorOr<IEnumerable<PoleDisplay>>> GetAllQuery => new GetAllPoleDisplaysQuery();

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new PoleDisplayModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(4));
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(5));
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
            return new DeletePoleDisplayCommand(SelectedEntity.Id);
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPOLEDISPLAY_CREATE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPOLEDISPLAY_EDIT");
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPOLEDISPLAY_DELETE");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPOLEDISPLAY_VIEW");
        }


        #region Singleton
        private static PoleDisplaysPage _instance;
        public static PoleDisplaysPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PoleDisplaysPage(null);
                }
                return _instance;
            }
        }
        #endregion
    }
}
