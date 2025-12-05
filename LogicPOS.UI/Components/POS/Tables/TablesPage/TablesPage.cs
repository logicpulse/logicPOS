using ErrorOr;
using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.POS.Tables.Common;
using LogicPOS.Api.Features.Tables.DeleteTable;
using LogicPOS.Api.Features.Tables.GetAllTables;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Services;
using MediatR;
using System.Collections.Generic;
using Table = LogicPOS.Api.Features.POS.Tables.Common.Table;

namespace LogicPOS.UI.Components.Pages
{
    public partial class TablesPage : Page<TableViewModel>
    {
        public TablesPage(Window parent) : base(parent)
        {
            DisableFilterButton();
        }

        protected override IRequest<ErrorOr<IEnumerable<TableViewModel>>> GetAllQuery => new GetTablesQuery();

        public override int RunModal(EntityEditionModalMode mode)
        {
            Table table = null;
            if (SelectedEntity != null)
            {
                table = TablesService.GetTable(SelectedEntity.Id);
            }
            var modal = new TableModal(mode, table);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(2));
            GridView.AppendColumn(CreatePlaceColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
        }
        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);
            AddCodeSorting(0);
            AddDesignationSorting(2);
            AddPlaceSorting();
            AddUpdatedAtSorting(4);
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeleteTableCommand(SelectedEntity.Id);
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_CREATE");
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_DELETE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_EDIT");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_VIEW");
        }

        #region Signleton
        private static TablesPage _instance;
        public static TablesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TablesPage(BackOfficeWindow.Instance);
                }

                return _instance;
            }
        }
        #endregion
    }
}
