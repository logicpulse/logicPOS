using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.POS
{
    public partial class PaymentsModal
    {
        private void BtnCurrentAccount_Clicked(object sender, EventArgs e)
        {
            SelectPaymentMethodByToken("CURRENT_ACCOUNT");
        }

        private void BtnCustomerCard_Clicked(object sender, EventArgs e)
        {
            SelectPaymentMethodByToken("CUSTOMER_CARD");
        }

        private void BtnVisa_Clicked(object sender, EventArgs e)
        {
            SelectPaymentMethodByToken("VISA");
        }

        private void BtnDebitCard_Clicked(object sender, EventArgs e)
        {
            SelectPaymentMethodByToken("DEBIT_CARD");
        }

        private void BtnCreditCard_Clicked(object sender, EventArgs e)
        {
            SelectPaymentMethodByToken("CREDIT_CARD");
        }

        private void BtnMB_Clicked(object sender, EventArgs e)
        {
            SelectPaymentMethodByToken("CASH_MACHINE");
        }

        private void BtnCheck_Clicked(object sender, EventArgs e)
        {
            SelectPaymentMethodByToken("BANK_CHECK");
        }

        private void BtnMoney_Clicked(object sender, EventArgs e)
        {
            InsertMoneyModalResponse result = InsertMoneyModal.RequestDecimalValue(this, TotalFinal);

            if (result.Response != ResponseType.Ok)
            {
                return;
            }

            SelectPaymentMethodByToken("MONEY");

            TotalDelivery = result.Value;
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            Validate();

            if (AllFieldsAreValid() == false)
            {
                Run();
                return;
            }

            var addDocumentCommand = CreateAddDocumentCommand();
            var addDocumentResult = _mediator.Send(addDocumentCommand).Result;

            if (addDocumentResult.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(this, addDocumentResult.FirstError);
                Run();
                return;
            }
        }

        private void BtnNewCustomer_Clicked(object sender, EventArgs e)
        {
            Clear();
        }

        private void BtnPaymentMethod_Clicked(object sender, EventArgs e)
        {
            EnableAllPaymentMethodButtons();
            BtnInvoice.Sensitive = true;
            (sender as IconButtonWithText).Sensitive = false;
            UpdateTotals();
        }

        private void BtnInvoice_Clicked(object sender, EventArgs e)
        {
            //logicpos.Utils.ShowMessageTouch(this,
            //                                DialogFlags.Modal,
            //                                MessageType.Error,
            //                                ButtonsType.Ok,
            //                                CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_error"),
            //                                CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_cant_create_cc_document_with_default_entity"));

            if (SelectPaymentCondition() == false)
            {
                _selectedPaymentCondition = null;
                return;
            }

            EnableAllPaymentMethodButtons();
            BtnInvoice.Sensitive = false;
            _selectedPaymentMethod = null;

            UpdateTotals();
        }
        
        private void BtnClearCustomer_Clicked(object sender, EventArgs e)
        {
            Clear();
        }

        private void BtnSelectCustomer_Clicked(object sender, System.EventArgs e)
        {
            var page = new CustomersPage(null, PageOptions.SelectionPageOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<Customer>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCustomer.Text = page.SelectedEntity.Name;
                TxtCustomer.SelectedEntity = page.SelectedEntity;
                ShowCustomerData(page.SelectedEntity);
            }
        }

        private void BtnSelectCountry_Clicked(object sender, EventArgs e)
        {
            var page = new CountriesPage(null, PageOptions.SelectionPageOptions);
            var selectCountryModal = new EntitySelectionModal<Country>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectCountryModal.Run();
            selectCountryModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCountry.Text = page.SelectedEntity.Designation;
                TxtCountry.SelectedEntity = page.SelectedEntity;
            }
        }

        protected override void OnResponse(ResponseType response)
        {
            if (response != ResponseType.Ok && response != ResponseType.Cancel)
            {
                Run();
                return;
            }

            base.OnResponse(response);
        }

    }
}
