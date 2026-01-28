using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CustomerTab
    {
        private void InitializeTxtEmail()
        {
            TxtEmail = new TextBox(SourceWindow,
                                       GeneralUtils.GetResourceByName("global_email"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
        }

        private void InitializeTxtPhone()
        {
            TxtPhone = new TextBox(SourceWindow,
                                       GeneralUtils.GetResourceByName("global_phone"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
        }

        private void InitializeTxtCountry()
        {
            TxtCountry = new TextBox(SourceWindow,
                                         GeneralUtils.GetResourceByName("global_country"),
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
            TxtCountry.SelectedEntity = CountriesService.Default;
            TxtCountry.Text = CountriesService.Default.Designation;
        }

        private void InitializeTxtCity()
        {
            TxtCity = new TextBox(SourceWindow,
                                      GeneralUtils.GetResourceByName("global_city"),
                                      isRequired: false,
                                      isValidatable: false,
                                      includeSelectButton: false,
                                      includeKeyBoardButton: true);
        }

        private void InitializeTxtZipCode()
        {
            TxtZipCode = new TextBox(SourceWindow,
                                         GeneralUtils.GetResourceByName("global_zipcode"),
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true);
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new TextBox(SourceWindow,
                                       GeneralUtils.GetResourceByName("global_notes"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);

            TxtNotes.Entry.IsEditable = true;
        }

        private void InitializeTxtLocality()
        {
            TxtLocality = new TextBox(SourceWindow,
                                          GeneralUtils.GetResourceByName("global_locality"),
                                          isRequired: false,
                                          isValidatable: false,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true);
        }

        private void InitializeTxtAddress()
        {
            TxtAddress = new TextBox(SourceWindow,
                                         GeneralUtils.GetResourceByName("global_address"),
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true);
        }

        private void InitializeTxtDiscount()
        {
            TxtDiscount = new TextBox(SourceWindow,
                                          GeneralUtils.GetResourceByName("global_discount"),
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
                                            GeneralUtils.GetResourceByName("global_card_number"),
                                            isRequired: false,
                                            isValidatable: false,
                                            includeSelectButton: false,
                                            includeKeyBoardButton: true);
        }

        private void InitializeTxtFiscalNumber()
        {
            TxtFiscalNumber = new TextBox(SourceWindow,
                                              GeneralUtils.GetResourceByName("global_fiscal_number"),
                                              isRequired: true,
                                              isValidatable: true,
                                              regex: RegularExpressions.FiscalNumber,
                                              includeSelectButton: false,
                                              includeKeyBoardButton: true);

            var customers = CustomersService.Customers.Select(c => (c as object, c.FiscalNumber)).ToList();
            TxtFiscalNumber.WithAutoCompletion(CustomersService.FiscalNumberAutocompleteLines, id => CustomersService.GetById(id));
            TxtFiscalNumber.OnCompletionSelected += c => SelectCustomer(c as Customer);
            TxtFiscalNumber.Entry.Changed += TxtFiscalNumber_Changed;
        }

        private void InitializeTxtCustomer()
        {
            TxtCustomer = new TextBox(SourceWindow,
                                          GeneralUtils.GetResourceByName("global_customer"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: true);

            TxtCustomer.SelectEntityClicked += BtnSelectCustomer_Clicked;
            var customers = CustomersService.Customers.Select(c => (c as object, c.Name)).ToList();
            TxtCustomer.WithAutoCompletion(CustomersService.AutocompleteLines, id => CustomersService.GetById(id));
            TxtCustomer.OnCompletionSelected += c => SelectCustomer(c as Customer);
            TxtCustomer.Entry.Changed += TxtCustomer_Changed;
        }

    }
}
