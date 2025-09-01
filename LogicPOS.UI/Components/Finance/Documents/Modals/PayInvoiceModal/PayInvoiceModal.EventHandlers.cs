using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PayInvoiceModal
    {
        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (AllFieldsAreValid() == false)
            {
                ShowValidationErrors();
                Run();
                return;
            }

            var result = _mediator.Send(CreateCommand()).Result;

            if (result.IsError)
            {
                CustomAlerts.ShowApiErrorAlert(this, result.FirstError);
                return;
            }

            DocumentPdfUtils.ViewReceiptPdf(this, result.Value);
        }

        private void TxtTotalPaid_Changed(object sender, EventArgs e)
        {
            if (!TxtTotalPaid.IsValid() || !TxtExchangeRate.IsValid())
            {
                return;
            }

            var totalPaid = CalculateSystemCurrencyTotalPaid();

            if (totalPaid > _invoicesTotalFinal)
            {
                var exchangeRate = decimal.Parse(TxtExchangeRate.Text);
                TxtTotalPaid.Text = (_invoicesTotalFinal / exchangeRate).ToString("0.00");
            }

            UpdateTitle();
            UpdateSystemCurrencyTotalPaid();
        }

        private void BtnSelectCurrency_Clicked(object sender, EventArgs e)
        {
            var page = new CurrenciesPage(null, PageOptions.SelectionPageOptions);
            var selectCurrencyModal = new EntitySelectionModal<Currency>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectCurrencyModal.Run();
            selectCurrencyModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCurrency.Text = page.SelectedEntity.Designation;
                TxtCurrency.SelectedEntity = page.SelectedEntity;
                TxtExchangeRate.Text = page.SelectedEntity.ExchangeRate.ToString("0.00");
            }
        }

        private void BtnSelectPaymentMethod_Clicked(object sender, EventArgs e)
        {
            var page = new PaymentMethodsPage(null, PageOptions.SelectionPageOptions);
            var selectPaymentMethodModal = new EntitySelectionModal<PaymentMethod>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectPaymentMethodModal.Run();
            selectPaymentMethodModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtPaymentMethod.Text = page.SelectedEntity.Designation;
                TxtPaymentMethod.SelectedEntity = page.SelectedEntity;
            }
        }
        private void TxtExchangeRate_Changed(object sender, EventArgs e)
        {
            if (TxtExchangeRate.IsValid())
            {
                UpdateTitle();
                UpdateSystemCurrencyTotalPaid();
            }
        }
        private void UpdateSystemCurrencyTotalPaid()
        {
            TxtSystemCurrencyTotalPaid.Text = CalculateSystemCurrencyTotalPaid().ToString();
        }


        private void UpdateTitle()
        {
            var totalPaidPercentage = (CalculateSystemCurrencyTotalPaid() / _invoicesTotalFinal) * 100;
            WindowSettings.Title.Text = $"{TitleBase} ({Invoices.Count} = {_invoicesTotalFinal:0.00}) - {totalPaidPercentage:0.00}%";
        }

        private decimal CalculateSystemCurrencyTotalPaid()
        {
            if (TxtTotalPaid.IsValid() == false || TxtExchangeRate.IsValid() == false)
            {
                return 0;
            }

            return decimal.Parse(TxtTotalPaid.Entry.Text) * decimal.Parse(TxtExchangeRate.Text);
        }

        private void SetDefaultCurrency()
        {
            TxtCurrency.SelectedEntity = Invoices.First().Invoice.Currency;
            TxtCurrency.Text = (TxtCurrency.SelectedEntity as Currency).Designation;
        }
    }
}
