using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Customers.Types.DeleteCustomerType;
using LogicPOS.Api.Features.Customers.Types.GetAllCustomerTypes;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class CustomerTypePage : Page<CustomerType>
    {
        protected override IRequest<ErrorOr<IEnumerable<CustomerType>>> GetAllQuery => new GetAllCustomerTypesQuery();

        public CustomerTypePage(Window parent) : base(parent)
        {
            DisableFilterButton();
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

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new CustomerTypeModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeleteCustomerTypeCommand(SelectedEntity.Id);
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CUSTOMERTYPE_CREATE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CUSTOMERTYPE_EDIT");
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CUSTOMERTYPE_DELETE");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CUSTOMERTYPE_VIEW");
        }

        #region Singleton
        private static CustomerTypePage _instance;
        public static CustomerTypePage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CustomerTypePage(null);
                }
                return _instance;
            }
        }
        #endregion
    }
}
