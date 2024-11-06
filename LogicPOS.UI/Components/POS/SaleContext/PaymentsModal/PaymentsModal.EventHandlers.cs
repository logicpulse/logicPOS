using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.GetDocumentById;
using LogicPOS.Api.Features.Terminals.GetTerminalById;
using LogicPOS.DTOs.Printing;
using LogicPOS.Printing.Documents;
using LogicPOS.Printing.Utility;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

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
            PrintDocument(addDocumentResult.Value);
        }



        private void PrintDocument(Guid id)
        {
            var document = DependencyInjection.Services.GetRequiredService<ISender>().Send(new GetDocumentByIdQuery(id)).Result.Value;

            List<int> docCopyName = new List<int>();
            docCopyName.Add(0);
            PrinterDto printer = GetTerminalThermalPrinter(TerminalService.Terminal);
            if (printer == null) 
            {
                return;
            }
            CompanyInformationsDto companyInformationsDto = GetCompanyInformation();
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

        private CompanyInformationsDto GetCompanyInformation()
        {
            var companyInformations = new CompanyInformations();
            return new CompanyInformationsDto()
            {
                Address = companyInformations.Address,
                Name = companyInformations.Name,
                BusinessName = companyInformations.BusinessName,
                ComercialName = companyInformations.ComercialName,
                City = companyInformations.City,
                Logo = companyInformations.Logo,

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
