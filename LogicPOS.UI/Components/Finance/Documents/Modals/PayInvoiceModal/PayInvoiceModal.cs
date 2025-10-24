using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents.PayDocuments;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PayInvoiceModal : Modal
    {
        public List<DocumentViewModel> Invoices { get; } = new List<DocumentViewModel>();
        private readonly decimal _invoicesTotalFinal;
        public PayInvoiceModal(Window parent,
                               IEnumerable<DocumentViewModel> invoices) : base(parent,
                                                     LocalizedString.Instance["window_title_dialog_pay_invoices"],
                                                     new Size(500, 500),
                                                     AppSettings.Paths.Images + @"Icons\Windows\icon_window_pay_invoice.png")
        {
            TitleBase = WindowSettings.Title.Text;
            Invoices.AddRange(invoices);
            SetDefaultCurrency();
            _invoicesTotalFinal = Invoices.Sum(x => x.TotalToPay);
            TxtTotalPaid.Text = _invoicesTotalFinal.ToString("0.00");
            TxtSystemCurrencyTotalPaid.Text = _invoicesTotalFinal.ToString("0.00");
            UpdateTitle();
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
            Initialize();

            var body = new VBox(false, 2);
            body.PackStart(TxtPaymentMethod.Component, false, false, 0);
            body.PackStart(TxtCurrency.Component, false, false, 0);
            body.PackStart(TxtExchangeRate.Component, false, false, 0);
            body.PackStart(TextBox.CreateHbox(TxtTotalPaid, TxtSystemCurrencyTotalPaid), false, false, 0);
            body.PackStart(TxtDateTime.Component, false, false, 0);
            body.PackStart(TxtNotes.Component, false, false, 0);

            return body;
        }

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
        }

        private PayDocumentsCommand CreateCommand()
        {
            var command = new PayDocumentsCommand
            {
                PaymentMethodId = (TxtPaymentMethod.SelectedEntity as ApiEntity).Id,
                CurrencyId = (TxtCurrency.SelectedEntity as ApiEntity).Id,
                ExchangeRate = decimal.Parse(TxtExchangeRate.Text),
                CurrencyAmount = decimal.Parse(TxtTotalPaid.Text),
                Amount = CalculateSystemCurrencyTotalPaid(),
                Documents = Invoices.Select(x => x.Id).ToList()
            };

            return command;
        }

        public bool AllFieldsAreValid() => ValidatableFields.All(tab => tab.IsValid());

        protected void ShowValidationErrors() => ValidationUtilities.ShowValidationErrors(ValidatableFields);

    }
}
