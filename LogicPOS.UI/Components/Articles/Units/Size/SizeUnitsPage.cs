using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.SizeUnits.DeleteSizeUnit;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.SizeUnits.GetAllSizeUnits;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System.Collections.Generic;


namespace LogicPOS.UI.Components.Pages
{
    public class SizeUnitsPage : Page<SizeUnit>
    {
        public SizeUnitsPage(Window parent) : base(parent)
        {
            DisableFilterButton();
        }


        protected override IRequest<ErrorOr<IEnumerable<SizeUnit>>> GetAllQuery => new GetAllSizeUnitsQuery();
      
        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new SizeUnitModal(mode, SelectedEntity);
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
            return new DeleteSizeUnitCommand(SelectedEntity.Id);
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONUNITSIZE_CREATE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONUNITSIZE_EDIT");
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONUNITSIZE_DELETE");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONUNITSIZE_VIEW");
        }

        #region Singleton
        private static SizeUnitsPage _instance;
        public static SizeUnitsPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SizeUnitsPage(null);
                }
                return _instance;
            }
        }
        #endregion
    }

}
