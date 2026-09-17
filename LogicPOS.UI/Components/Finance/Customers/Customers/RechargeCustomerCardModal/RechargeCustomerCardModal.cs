using Gtk;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public class RechargeCustomerCardModal : Modal
    {
        private readonly Customer _customer;

        public IconButtonWithText BtnOk { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        public IconButtonWithText BtnCancel { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);

        private TextBox _txtCustomerName;
        private TextBox _txtCardNumber;
        private TextBox _txtCardCredit;
        private TextBox _txtAmount;

        public RechargeCustomerCardModal(Window parent, Customer customer)
            : base(parent,
                   LocalizedString.Instance["pos_button_label_payment_type_customer_card"],
                   new Size(500, 370),
                   AppSettings.Paths.Images + @"Icons\Windows\icon_window_users.png")
        {
            _customer = customer ?? throw new ArgumentNullException(nameof(customer));
            PresentCustomerData();
        }

        private void PresentCustomerData()
        {
            _txtCustomerName.Text = _customer.Name;
            _txtCardNumber.Text = _customer.CardNumber;
            _txtCardCredit.Text = _customer.CardCredit.ToString();
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            BtnOk.Sensitive = false;
            BtnOk.Clicked += BtnOk_Clicked;

            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        protected override Widget CreateBody()
        {
            InitializeFields();

            var layout = new VBox(false, 2);
            layout.PackStart(_txtCustomerName.Component, false, false, 0);
            layout.PackStart(_txtCardNumber.Component, false, false, 0);
            layout.PackStart(_txtCardCredit.Component, false, false, 0);
            layout.PackStart(_txtAmount.Component, false, false, 0);
            return layout;
        }

        private void InitializeFields()
        {
            _txtCustomerName = CreateReadOnlyField("global_name", "?");
            _txtCardNumber = CreateReadOnlyField("global_card_number", "?");
            _txtCardCredit = CreateReadOnlyField("global_card_credit_amount","?");

            _txtAmount = new TextBox(WindowSettings.Source,
                                     LocalizedString.Instance["global_value"],
                                     isRequired: true,
                                     isValidatable: true,
                                     includeSelectButton: false,
                                     includeKeyBoardButton: true,
                                     regex: RegularExpressions.DecimalNumber);

            _txtAmount.Entry.Changed += (_, __) => UpdateOkSensitivity();
        }

        private TextBox CreateReadOnlyField(string labelKey, string value)
        {
            var field = new TextBox(WindowSettings.Source,
                                    LocalizedString.Instance[labelKey],
                                    isRequired: false,
                                    isValidatable: false,
                                    includeSelectButton: false,
                                    includeKeyBoardButton: false);
            field.Text = value;
            field.Entry.Sensitive = false;
            return field;
        }

        private void UpdateOkSensitivity()
        {
            BtnOk.Sensitive = _txtAmount.IsValid()
                && decimal.TryParse(_txtAmount.Text, out var amount)
                && amount > 0;
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (_txtAmount.IsValid() == false)
            {
                ValidationUtilities.ShowValidationErrors(new[] { _txtAmount }, this);
                Run();
                return;
            }

            var amount = decimal.Parse(_txtAmount.Text);

            var confirm = CustomAlerts.Question(this)
                .WithTitle(LocalizedString.Instance["pos_button_label_payment_type_customer_card"])
                .WithMessage($"{LocalizedString.Instance["global_value"]}: {amount:0.00}")
                .WithSize(new Size(300,300))
                .ShowAlert();

            if (confirm != ResponseType.Yes)
            {
                Run();
                return;
            }

            if (CustomersService.RechargeCard(_customer.Id, amount) == false)
            {
                Run();
                return;
            }

            Respond(ResponseType.Ok);
        }
    }
}
