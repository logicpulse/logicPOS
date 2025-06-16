using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Customers.DeleteCustomer;
using LogicPOS.Api.Features.Customers.GetAllCustomers;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class CustomersPage : Page<Customer>
    {
        protected override IRequest<ErrorOr<IEnumerable<Customer>>> GetAllQuery => new GetAllCustomersQuery();

        public CustomersPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new CustomerModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(CreateNameColumn());
            GridView.AppendColumn(CreateFiscalNumberColumn());
            GridView.AppendColumn(CreateCardNumberColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeleteCustomerCommand(SelectedEntity.Id);
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CUSTOMER_CREATE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CUSTOMER_EDIT");
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CUSTOMER_DELETE");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CUSTOMER_VIEW");
        }

        #region Singleton
        private static CustomersPage _instance;

        public static CustomersPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CustomersPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }

        #endregion
    }
}
