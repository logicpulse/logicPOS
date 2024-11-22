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
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
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
            BtnMoney.Clicked += BtnMoney_Clicked;
            BtnCheck.Clicked += BtnCheck_Clicked;
            BtnMB.Clicked += BtnMB_Clicked;
            BtnCreditCard.Clicked += BtnCreditCard_Clicked;
            BtnDebitCard.Clicked += BtnDebitCard_Clicked;
            BtnVisa.Clicked += BtnVisa_Clicked;
            BtnCustomerCard.Clicked += BtnCustomerCard_Clicked;
            BtnCurrentAccount.Clicked += BtnCurrentAccount_Clicked;
            PaymentMethodButtons.ForEach(button => { button.Clicked += BtnPaymentMethod_Clicked; });
            BtnInvoice.Clicked += BtnInvoice_Clicked;
            BtnNewCustomer.Clicked += BtnNewCustomer_Clicked;
            BtnOk.Clicked += BtnOk_Clicked;
            BtnPartialPayment.Clicked += BtnPartialPayment_Clicked;
            BtnFullPayment.Clicked += BtnFullPayment_Clicked;
        }

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
                ErrorHandlingService.HandleApiError(addDocumentResult.FirstError, source: this);
                Run();
                return;
            }

            ProcesPayment();

            PrintDocument(addDocumentResult.Value);
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
            SaleContext.ItemsPage.Clear(true);
            SaleContext.CurrentOrder.ReduceItems(_partialPaymentItems);
            SaleContext.UpdatePOSLabels();
        }

        private void ProcessFullPayment()
        {
            SaleContext.ItemsPage.Clear(true);
            SaleContext.CurrentOrder.Close();
            SaleContext.UpdatePOSLabels();
        }

        private void PrintDocument(Guid id)
        {
            var result = DependencyInjection.Services.GetRequiredService<ISender>().Send(new GetDocumentByIdQuery(id)).Result;
            if (result.IsError)
            {
                CustomAlerts.ShowApiErrorAlert(this, result.FirstError);
            }
            var document = result.Value;
            List<int> docCopyName = new List<int>();
            docCopyName.Add(0);
            PrinterDto printer = GetTerminalThermalPrinter(TerminalService.Terminal);
            if (printer == null)
            {
                return;
            }
            CompanyPrintingInformationsDto companyInformationsDto = GetCompanyPrintingInformation();
            new ThermalPrinting(printer, companyInformationsDto, docCopyName, document, TerminalService.Terminal.Designation, AuthenticationService.User.Name);
        }

        protected PrinterDto GetTerminalThermalPrinter(Terminal terminal)
        {

            if (terminal.ThermalPrinter != null)
            {
                return new PrinterDto()
                {
                    Designation = terminal.ThermalPrinter.Designation,
                    Token = terminal.ThermalPrinter.Type.Token,
                    IsThermal = terminal.ThermalPrinter.Type.ThermalPrinter,
                    CutCommand = "0x42,0x00"
                };
            }
            else
            {
                return null;
            }
        }

        private CompanyPrintingInformationsDto GetCompanyPrintingInformation()
        {
            var result = DependencyInjection.Services.GetRequiredService<ISender>().Send(new GetCompanyInformationsQuery()).Result;
            if (result.IsError)
            {
                CustomAlerts.ShowApiErrorAlert(this, result.FirstError);
            }
            var companyInformations = result.Value;
            return new CompanyPrintingInformationsDto()
            {
                Address = companyInformations.Address,
                Name = companyInformations.Name,
                BusinessName = companyInformations.BussinessName,
                ComercialName = companyInformations.ComercialName,
                City = companyInformations.City,
                Logo = companyInformations.LogoBmp,
                Email = companyInformations.Email,
                MobilePhone = companyInformations.MobilePhone,
                FiscalNumber = companyInformations.FiscalNumber,
                Phone = companyInformations.Phone,
                StockCapital = companyInformations.StockCapital,
                Website = companyInformations.Website,
                DocumentFinalLine1 = companyInformations.DocumentFinalLine1,
                DocumentFinalLine2 = companyInformations.DocumentFinalLine2,
                TicketFinalLine1 = companyInformations.TicketFinalLine1,
                TicketFinalLine2 = companyInformations.TicketFinalLine2,
            };
        }

        private void BtnNewCustomer_Clicked(object sender, EventArgs e)
        {
            Clear();
        }

        private void BtnPaymentMethod_Clicked(object sender, EventArgs e)
        {
            EnableAllPaymentMethodButtons();
            (sender as IconButtonWithText).Sensitive = false;
            UncheckInvoiceMode();
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
