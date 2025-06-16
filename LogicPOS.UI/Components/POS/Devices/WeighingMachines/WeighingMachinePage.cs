using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.WeighingMachines.DeleteWeighingMachine;
using LogicPOS.Api.Features.WeighingMachines.GetAllWeighingMachines;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using MediatR;
using System.Collections.Generic;


namespace LogicPOS.UI.Components.Pages
{
    public class WeighingMachinesPage : Page<WeighingMachine>
    {
        protected override IRequest<ErrorOr<IEnumerable<WeighingMachine>>> GetAllQuery => new GetAllWeighingMachinesQuery();
        public WeighingMachinesPage(Window parent) : base(parent)
        {
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new WeighingMachineModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(2));
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
            return new DeleteWeighingMachineCommand(SelectedEntity.Id);
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONWEIGHINGMACHINE_CREATE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONWEIGHINGMACHINE_EDIT");
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONWEIGHINGMACHINE_DELETE");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONWEIGHINGMACHINE_VIEW");
        }

        #region Singleton
        private static WeighingMachinesPage _instance;
        public static WeighingMachinesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WeighingMachinesPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }
        #endregion

    }
}
