using ErrorOr;
using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Customers.DeleteCustomer;
using LogicPOS.Api.Features.Customers.GetAllCustomers;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Errors;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public partial class CustomersPage : Page<Customer>
    {
        private const string ProvidersOnlyOption = "providers-only";
        private const string CustomersOnlyOption = "customers-only";
        public static readonly Dictionary<string, string> SupplierSelectionOptions = new Dictionary<string, string> { { "selection-page", "true" }, { ProvidersOnlyOption, "true" } };
        public static readonly Dictionary<string, string> CustomerSelectionOptions = new Dictionary<string, string> { { "selection-page", "true" }, { CustomersOnlyOption, "true" } };

        protected override IRequest<ErrorOr<IEnumerable<Customer>>> GetAllQuery => new GetAllCustomersQuery();

        public CustomersPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
        }

        protected override void LoadEntities()
        {
            var getEntitiesResult = _mediator.Send(GetAllQuery).Result;

            if (getEntitiesResult.IsError)
            {
                HandleErrorResult(getEntitiesResult);
                return;
            }

            _entities.Clear();

            var customers = getEntitiesResult.Value;

            if (Options != null && Options.ContainsKey(ProvidersOnlyOption))
            {
                customers = customers.Where(c => c.Supplier);
            }
            else if (Options != null && Options.ContainsKey(CustomersOnlyOption))
            {
                customers = customers.Where(c => c.Supplier == false);
            }

            _entities.AddRange(customers);
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            if (CustomersService.Default != null && mode == EntityEditionModalMode.Update && SelectedEntity.Id == CustomersService.Default.Id)
            {
                mode = EntityEditionModalMode.View;
            }

            var modal = new CustomerModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            CustomersService.RefreshCustomersCache();
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

        public override bool DeleteEntity()
        {
            var command = GetDeleteCommand();

            if (command == null)
            {
                return false;
            }

            var result = _mediator.Send(GetDeleteCommand()).Result;
            CustomersService.RefreshCustomersCache();

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result, source: SourceWindow);
                return false;
            }

            if (result.Value == false)
            {
                CustomAlerts.ShowCannotDeleteEntityErrorAlert(SourceWindow);
            }
            return result.Value;
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            var result = new DeleteCustomerCommand(SelectedEntity.Id);
            return result;
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
