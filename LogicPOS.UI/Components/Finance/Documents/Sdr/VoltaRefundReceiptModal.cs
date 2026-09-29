using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.Api.Features.Finance.Documents.Documents.Sdr;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.Finance.PaymentMethods;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using System.Drawing;

namespace LogicPOS.UI.Components.Finance.Documents.Sdr
{
    public partial class VoltaRefundReceiptModal : Modal
    {
        public VoltaRefundReceiptModal(Window parent)
            : base(parent,
                   TitleBase,
                   new Size(620, 360),
                   AppSettings.Paths.Images + @"Icons\Windows\icon_window_payments.png")
        {
        }

        public static void Show(Window parent)
        {
            if (FiscalYearsService.HasActiveFiscalYear() == false)
            {
                FiscalYearsService.ShowOpenFiscalYearAlert(parent);
                return;
            }

            if (TrvDocumentUiRules.ResolveDepositArticle() == null)
            {
                CustomAlerts.Warning(parent)
                    .WithTitle("Artigo SDR em falta")
                    .WithMessage("Crie o artigo de depósito SDRVDEP antes de emitir o Talão Reembolso Volta.")
                    .ShowAlert();
                return;
            }

            if (TrvDocumentUiRules.ResolvePaymentMethod() == null)
            {
                CustomAlerts.Warning(parent)
                    .WithTitle("Método de pagamento em falta")
                    .WithMessage("Configure o método de pagamento OU (Outros) antes de emitir o Talão Reembolso Volta.")
                    .ShowAlert();
                return;
            }

            var modal = new VoltaRefundReceiptModal(parent);
            modal.Run();
            modal.Destroy();
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        protected override Widget CreateBody()
        {
            InitializeFields();
            AddEventHandlers();
            SetDefaultCustomer();
            SelectDefaultPaymentMethod();
            UpdateTotal();

            var layout = new VBox(false, 0);
            layout.PackStart(TxtCustomer.Component, false, false, 0);
            layout.PackStart(TxtQuantity.Component, false, false, 0);
            layout.PackStart(TxtPaymentMethod.Component, false, false, 0);
            layout.PackStart(TxtOriginDocument.Component, false, false, 0);

            return layout;
        }

        private void InitializeFields()
        {
            TxtCustomer = new TextBox(this,
                LocalizedString.Instance["global_customer"],
                isRequired: true,
                isValidatable: false,
                includeSelectButton: true,
                includeKeyBoardButton: true,
                includeClearButton: false);

            TxtCustomer.WithAutoCompletion(CustomersService.AutocompleteLines, id => CustomersService.GetById(id));
            TxtCustomer.OnCompletionSelected += customer => SelectCustomer(customer as Customer);

            TxtQuantity = new TextBox(this,
                LocalizedString.Instance["global_quantity"],
                isRequired: true,
                isValidatable: true,
                includeSelectButton: false,
                includeKeyBoardButton: true,
                regex: RegularExpressions.DecimalGreaterEqualThanZeroFinancial,
                includeClearButton: false);

            TxtQuantity.Text = "1";

            TxtPaymentMethod = new TextBox(this,
                LocalizedString.Instance["global_payment_method"],
                isRequired: true,
                isValidatable: false,
                includeSelectButton: true,
                includeKeyBoardButton: false);

            TxtPaymentMethod.Entry.IsEditable = true;
            TxtPaymentMethod.WithAutoCompletion(PaymentMethodsService.AutocompleteLines, id => PaymentMethodsService.GetBydId(id));
            TxtPaymentMethod.Component.Sensitive = false;

            InitializeTxtOriginDocument();
        }

        private void InitializeTxtOriginDocument()
        {
            TxtOriginDocument = new TextBox(this,
                                                LocalizedString.Instance["global_source_finance_document"],
                                                isRequired: false,
                                                isValidatable: false,
                                                includeSelectButton: true,
                                                includeKeyBoardButton: false);
            TxtOriginDocument.Entry.IsEditable = false;
            TxtOriginDocument.SelectEntityClicked += BtnSelectOriginDocument_Clicked;
        }

        private void UpdateTotal()
        {
            decimal quantity = 0;
            decimal.TryParse(TxtQuantity?.Text, out quantity);

            var depositArticle = TrvDocumentUiRules.ResolveDepositArticle();
            var total = depositArticle == null
                ? 0
                : SdrDepositAmountCalculator.CalculateLineTotal(quantity, depositArticle);

            WindowSettings.Title.Text = $"{TitleBase} — Total a reembolsar: {total:N2}{PreferenceParametersService.SystemCurrency}";
        }

        private void SetDefaultCustomer()
        {
            if (CustomersService.Default != null)
            {
                SelectCustomer(CustomersService.Default);
            }
        }

        private void SelectDefaultPaymentMethod()
        {
            SelectPaymentMethod(TrvDocumentUiRules.ResolvePaymentMethod());
        }

        private void SelectCustomer(Customer customer)
        {
            if (customer == null)
            {
                return;
            }

            _selectedCustomer = customer;
            TxtCustomer.Text = customer.Name;
            TxtCustomer.SelectedEntity = customer;
        }

        private void SelectPaymentMethod(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
            {
                return;
            }

            TxtPaymentMethod.Text = paymentMethod.Designation;
            TxtPaymentMethod.SelectedEntity = paymentMethod;
        }

        private decimal GetQuantity()
        {
            decimal quantity = 0;
            decimal.TryParse(TxtQuantity.Text, out quantity);
            return quantity;
        }
    }
}
