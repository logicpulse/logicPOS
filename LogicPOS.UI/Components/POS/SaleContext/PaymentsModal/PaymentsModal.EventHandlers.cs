using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Company.GetCompanyInformations;
using LogicPOS.Api.Features.Documents.GetDocumentById;
using LogicPOS.DTOs.Printing;
using LogicPOS.Printing.Documents;
using LogicPOS.Printing.Utility;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.POS.Enums;
using LogicPOS.UI.Components.POS.PrintingContext;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace LogicPOS.UI.Components.POS
{
    public partial class PaymentsModal
    {
        private void AddEventHandlers()
        {
            BtnClearCustomer.Clicked += BtnClearCustomer_Clicked;
            BtnInvoice.Clicked += BtnInvoice_Clicked;
            BtnNewCustomer.Clicked += BtnNewCustomer_Clicked;
            BtnOk.Clicked += BtnOk_Clicked;
            BtnPartialPayment.Clicked += BtnPartialPayment_Clicked;
            BtnFullPayment.Clicked += BtnFullPayment_Clicked;
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
                ErrorHandlingService.HandleApiError(addDocumentResult.FirstError, source: this);
                Run();
                return;
            }

            ProcesPayment();
            PrintingServices.PrintDocument(addDocumentResult.Value);
        }

        private void ProcesPayment()
        {
            if (_paymentMode == PaymentMode.Full)
            {
                ProcessFullPayment();
                return;
            }

            ProcessPartialPayment();
        }

        private void ProcessPartialPayment()
        {
            SaleContext.CurrentOrder.ReduceItems(_partialPaymentItems);
            SaleContext.ReloadCurrentOrder();
        }

        private void ProcessFullPayment()
        {
            SaleContext.ItemsPage.Clear(true);
            SaleContext.CurrentOrder.Close();
            POSWindow.Instance.UpdateUI();
        }

        private void BtnNewCustomer_Clicked(object sender, EventArgs e)
        {
            Clear();
        }

        private void PaymentMethodSelected(PaymentMethod paymentMethod)
        {
            if (paymentMethod.Acronym == "NU")
            {
                InsertMoneyModalResponse result = InsertMoneyModal.RequestDecimalValue(this, TotalFinal);

                if (result.Response == ResponseType.Ok)
                {
                    TotalDelivery = result.Value;
                    TotalChange = TotalDelivery - TotalFinal;
                }
                else
                {
                    TotalDelivery = TotalFinal;
                    TotalChange = 0;
                }
            }

            UncheckInvoiceMode();
            UpdateTotals();
        }

        private void BtnInvoice_Clicked(object sender, EventArgs e)
        {
            if (SelectPaymentCondition() == false)
            {
                _selectedPaymentCondition = null;
                return;
            }

            EnableAllPaymentMethodButtons();
            BtnInvoice.Sensitive = false;
            PaymentMethodsMenu.SelectedEntity = null;

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

        private void BtnPartialPayment_Clicked(object sender, EventArgs e)
        {
            var partialPaymentModal = new PartialPaymentModal(this);
            ResponseType response = (ResponseType)partialPaymentModal.Run();

            if (response == ResponseType.Ok && partialPaymentModal.Page.SelectedItems.Any())
            {
                _partialPaymentItems = partialPaymentModal.Page.SelectedItems;
                TotalDelivery = _partialPaymentItems.Sum(x => x.TotalFinal);
                TotalChange = 0;
                _paymentMode = (TotalDelivery >= TotalFinal) ? PaymentMode.Full : PaymentMode.Partial;
            }
            partialPaymentModal.Destroy();

            UncheckInvoiceMode();
            UpdateButtons();
            UpdateTotals();
        }

        private void BtnFullPayment_Clicked(object sender, EventArgs e)
        {
            _paymentMode = PaymentMode.Full;
            TotalFinal = OrderTotalFinal;
            TotalDelivery = OrderTotalFinal;
            TotalChange = 0;
            UncheckInvoiceMode();
            UpdateButtons();
            UpdateTotals();
        }

        private void UpdateButtons()
        {
            BtnPartialPayment.Sensitive = _paymentMode == PaymentMode.Full;
            BtnFullPayment.Sensitive = _paymentMode == PaymentMode.Partial;
        }
    }
}
