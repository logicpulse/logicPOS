using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Users.CommissionGroups.DeleteCommissionGroup;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class CommissionGroupsPage : Page<CommissionGroup>
    {
        protected override IRequest<ErrorOr<IEnumerable<CommissionGroup>>> GetAllQuery => new GetAllCommissionGroupsQuery();
        public CommissionGroupsPage(Window parent) : base(parent)
        {
            DisableFilterButton();
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new CommissionGroupModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateCommissionColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }
        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddCommissionSorting();
            AddUpdatedAtSorting(3);
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeleteCommissionGroupCommand(SelectedEntity.Id);
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_USERCOMMISSIONGROUP_CREATE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_USERCOMMISSIONGROUP_EDIT");
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_USERCOMMISSIONGROUP_DELETE");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_USERCOMMISSIONGROUP_VIEW");
        }

        #region Singleton
        private static CommissionGroupsPage _instance;
        public static CommissionGroupsPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CommissionGroupsPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }
        #endregion
    }
}
