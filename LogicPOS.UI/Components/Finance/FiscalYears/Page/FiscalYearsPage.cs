using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.FiscalYears.GetAllFiscalYears;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using MediatR;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public partial class FiscalYearsPage : Page<FiscalYear>
    {
        protected override IRequest<ErrorOr<IEnumerable<FiscalYear>>> GetAllQuery => new GetAllFiscalYearsQuery();

        public FiscalYearsPage(Window parent) : base(parent)
        {
            Navigator.BtnUpdate.Visible = false;
            Navigator.BtnDelete.Visible = false;
            DisableFilterButton();
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            if (mode == EntityEditionModalMode.Update)
            {
                mode = EntityEditionModalMode.View;
            }

            var modal = new FiscalYearModal(modalMode: mode,
                                            entity: SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }
        
        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateAcronymColumn());
            GridView.AppendColumn(CreateYearColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
        }
       
        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddAcronymSorting();
            AddYearSorting();
            AddUpdatedAtSorting(4);
        }
      
        protected override DeleteCommand GetDeleteCommand() => null;

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_CREATE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_EDIT");
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_DELETE");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_VIEW");
        }
    }
}
