using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.POS.Enums;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Printing;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (BtnInvoice.Sensitive == true && PaymentMethod == null)
            {
                SimpleAlerts.Information()
                            .WithMessage("Selecione um Método de Pagamento")
                            .ShowAlert();
                Run();
                return;
            }
            if (AllFieldsAreValid() == false)
            {
                Run();
                return;
            }

            var addDocumentCommand = CreateAddDocumentCommand();
            var printingData = DocumentsService.IssueDocumentForPrinting(addDocumentCommand);

            if (printingData == null)
            {
                Run();
                return;
            }

            ProcesPayment();

            ThermalPrintingService.PrintInvoice(printingData.Value);
        }

        private void ProcesPayment()
        {
            if (_paymentMode == PaymentMode.Full)
            {
                ProcessFullPayment();
                return;
            }

            if (_paymentMode == PaymentMode.Splited)
            {
                if (InitialSplittersNumber != SplittersNumber)
                {
                    OrdersService.SavePosTicket(SaleContext.CurrentOrder, SaleContext.CurrentOrder.Tickets.FirstOrDefault());
                }
                UpdateLabels();
                return;
            }

            ProcessPartialPayment();
        }

        private List<SaleItem> SplitTickets(int splittersNumber)
        {
            List<SaleItem> itemsToUpdate = null;
            if (SaleContext.CurrentOrder.Tickets.Any())
            {

                foreach (var ticket in SaleContext.CurrentOrder.Tickets)
                {
                    foreach (var item in ticket.Items)
                    {
                        item.Quantity = (item.Quantity / splittersNumber);


                    }
                    itemsToUpdate = ticket.Items;
                }
            }
            SaleContext.CurrentOrder.SplitTicket(itemsToUpdate, splittersNumber);
            return itemsToUpdate;
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
            paymentMethodDesignation = paymentMethod.Designation;
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

        private void TxtCustomer_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtCustomer.Text))
            {
                Clear();
            }
        }

        private void TxtFiscalNumber_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFiscalNumber.Text))
            {
                Clear();
            }
        }
    }
}
