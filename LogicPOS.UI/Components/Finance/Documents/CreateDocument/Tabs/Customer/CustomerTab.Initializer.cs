using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;
using System.Linq;
using LogicPOS.Globalization;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CustomerTab
    {
        private void InitializeTxtEmail()
        {
            TxtEmail = new TextBox(SourceWindow,
                                       LocalizedString.Instance["global_email"],
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
        }

        private void InitializeTxtPhone()
        {
            TxtPhone = new TextBox(SourceWindow,
                                       LocalizedString.Instance["global_phone"],
                                       isRequired: false,
                                       isValidatable: true,
                                       regex: RegularExpressions.IntegerNumber,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
        }

        private void InitializeTxtCountry()
        {
            TxtCountry = new TextBox(SourceWindow,
                                         LocalizedString.Instance["global_country"],
                                         isRequired: true,
                                         isValidatable: false,
                                         includeSelectButton: true,
                                         includeKeyBoardButton: false);

            TxtCountry.Entry.IsEditable = false;
            TxtCountry.SelectEntityClicked += BtnSelectCountry_Clicked;
            SelectDefaultCountry();
        }

        private void SelectDefaultCountry()
        {
            var defaultCountry = CountriesService.Default;
            if (defaultCountry == null)
            {
                return;
            }

            TxtCountry.SelectedEntity = defaultCountry;
            TxtCountry.Text = defaultCountry.Designation;
        }

        private void InitializeTxtCity()
        {
            TxtCity = new TextBox(SourceWindow,
                                      LocalizedString.Instance["global_city"],
                                      isRequired: false,
                                      isValidatable: false,
                                      includeSelectButton: false,
                                      includeKeyBoardButton: true);
        }

        private void InitializeTxtZipCode()
        {
            TxtZipCode = new TextBox(SourceWindow,
                                         LocalizedString.Instance["global_zipcode"],
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true);
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new TextBox(SourceWindow,
                                       LocalizedString.Instance["global_notes"],
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);

            TxtNotes.Entry.IsEditable = true;
        }

        private void InitializeTxtLocality()
        {
            TxtLocality = new TextBox(SourceWindow,
                                          LocalizedString.Instance["global_locality"],
                                          isRequired: false,
                                          isValidatable: false,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true);
        }

        private void InitializeTxtAddress()
        {
            TxtAddress = new TextBox(SourceWindow,
                                         LocalizedString.Instance["global_address"],
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true);
        }

        private void InitializeTxtDiscount()
        {
            TxtDiscount = new TextBox(SourceWindow,
                                          LocalizedString.Instance["global_discount"],
                                          isRequired: true,
                                          isValidatable: true,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true,
                                          regex: RegularExpressions.NullableMoney);

            TxtDiscount.IsValidFunction = ValidationFunctions.IsValidDiscount;
            TxtDiscount.WithText("0");
            TxtDiscount.Entry.Changed += TxtDiscount_Changed;
        }

  

        private void InitializeTxtCardNumber()
        {
            TxtCardNumber = new TextBox(SourceWindow,
                                            LocalizedString.Instance["global_card_number"],
                                            isRequired: false,
                                            isValidatable: false,
                                            includeSelectButton: false,
                                            includeKeyBoardButton: true);
            TxtCardNumber.WithAutoCompletion(CustomersService.CardNumberAutocompleteLines, id => CustomersService.GetById(id));
            TxtCardNumber.OnCompletionSelected += c => SelectCustomer(c as Customer);
            TxtCardNumber.Entry.Activated += OnTxtCardNumberEnterPressed;
            TxtCardNumber.Entry.Changed += TxtCardNumber_Changed;
        }

        private void InitializeTxtFiscalNumber()
        {
            TxtFiscalNumber = new TextBox(SourceWindow,
                                              LocalizedString.Instance["global_fiscal_number"],
                                              isRequired: true,
                                              isValidatable: true,
                                              regex: RegularExpressions.GetFiscalNumberRegexForCountry(SystemInformationService.SystemInformation.CountryCode2),
                                              includeSelectButton: false,
                                              includeKeyBoardButton: true);
            
            var customers = CustomersService.GetAllCustomers().Select(c => (c as object, c.Name)).ToList();
            TxtFiscalNumber.WithAutoCompletion(CustomersService.FiscalNumberAutocompleteLines, id => CustomersService.GetById(id));
            TxtFiscalNumber.OnCompletionSelected += c => SelectCustomer(c as Customer);
            TxtFiscalNumber.Entry.Changed += TxtFiscalNumber_Changed;
        }

        private void InitializeTxtCustomer()
        {
            TxtCustomer = new TextBox(SourceWindow,
                                          LocalizedString.Instance["global_customer"],
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: true);

            TxtCustomer.SelectEntityClicked += BtnSelectCustomer_Clicked;
            var customers = CustomersService.GetAllCustomers().Select(c => (c as object, c.Name)).ToList();
            TxtCustomer.WithAutoCompletion(CustomersService.AutocompleteLines, id => CustomersService.GetById(id));
            TxtCustomer.OnCompletionSelected += c => SelectCustomer(c as Customer);
            TxtCustomer.Entry.Changed += TxtCustomer_Changed;
            TxtCustomer.Entry.ClipboardPasted += TxtCustomer_ClipboardPasted;
        }


    }
}
