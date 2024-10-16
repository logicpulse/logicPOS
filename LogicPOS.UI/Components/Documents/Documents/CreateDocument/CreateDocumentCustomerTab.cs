using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents;
using LogicPOS.Settings;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public class CreateDocumentCustomerTab : ModalTab
    {
        public PageTextBox TxtCustomer { get; set; }
        public PageTextBox TxtFiscalNumber { get; set; }
        public PageTextBox TxtCardNumber { get; set; }
        public PageTextBox TxtDiscount { get; set; }
        public PageTextBox TxtAddress { get; set; }
        public PageTextBox TxtLocality { get; set; }
        public PageTextBox TxtZipCode { get; set; }
        public PageTextBox TxtCity { get; set; }
        public PageTextBox TxtCountry { get; set; }
        public PageTextBox TxtPhone { get; set; }
        public PageTextBox TxtEmail { get; set; }
        public PageTextBox TxtNotes { get; set; }
        public Guid? CustomerId { get; set; }
        private Guid _countryId;

        public CreateDocumentCustomerTab(Window parent) : base(parent: parent,
                                                  name: GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page2"),
                                                  icon: PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_2_customer.png")
        {
            Initialize();
            Design();
        }

        private void Initialize()
        {
            InitializeTxtCustomer();
            InitializeTxtFiscalNumber();
            InitializeTxtCardNumber();
            InitializeTxtDiscount();
            InitializeTxtAddress();
            InitializeTxtLocality();
            InitializeTxtZipCode();
            InitializeTxtCity();
            InitializeTxtCountry();
            InitializeTxtPhone();
            InitializeTxtEmail();
            InitializeTxtNotes();
        }

        private void InitializeTxtEmail()
        {
            TxtEmail = new PageTextBox(SourceWindow,
                                       GeneralUtils.GetResourceByName("global_email"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
        }

        private void InitializeTxtPhone()
        {
            TxtPhone = new PageTextBox(SourceWindow,
                                       GeneralUtils.GetResourceByName("global_phone"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
        }

        private void InitializeTxtCountry()
        {
            TxtCountry = new PageTextBox(SourceWindow,
                                         GeneralUtils.GetResourceByName("global_country"),
                                         isRequired: true,
                                         isValidatable: false,
                                         includeSelectButton: true,
                                         includeKeyBoardButton: false);

            TxtCountry.Entry.IsEditable = false;

            TxtCountry.SelectEntityClicked += BtnSelectCountry_Clicked;
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

        private void InitializeTxtCity()
        {
            TxtCity = new PageTextBox(SourceWindow,
                                      GeneralUtils.GetResourceByName("global_city"),
                                      isRequired: false,
                                      isValidatable: false,
                                      includeSelectButton: false,
                                      includeKeyBoardButton: true);
        }

        private void InitializeTxtZipCode()
        {
            TxtZipCode = new PageTextBox(SourceWindow,
                                         GeneralUtils.GetResourceByName("global_zipcode"),
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true);
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new PageTextBox(SourceWindow,
                                       GeneralUtils.GetResourceByName("global_notes"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);

            TxtNotes.Entry.IsEditable = true;
        }

        private void InitializeTxtLocality()
        {
            TxtLocality = new PageTextBox(SourceWindow,
                                          GeneralUtils.GetResourceByName("global_locality"),
                                          isRequired: false,
                                          isValidatable: false,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true);
        }

        private void InitializeTxtAddress()
        {
            TxtAddress = new PageTextBox(SourceWindow,
                                         GeneralUtils.GetResourceByName("global_address"),
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true);
        }

        private void InitializeTxtDiscount()
        {
            TxtDiscount = new PageTextBox(SourceWindow,
                                          GeneralUtils.GetResourceByName("global_discount"),
                                          isRequired: true,
                                          isValidatable: true,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true,
                                          regex: RegularExpressions.DecimalNumber);
            TxtDiscount.Text = 0.00M.ToString("");
        }

        private void BtnSelectCurrency_Clicked(object sender, EventArgs e)
        {
            var page = new CurrenciesPage(null, PageOptions.SelectionPageOptions);
            var selectCurrencyModal = new EntitySelectionModal<Currency>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectCurrencyModal.Run();
            selectCurrencyModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtDiscount.Text = page.SelectedEntity.Designation;
            }
        }

        private void InitializeTxtCardNumber()
        {
            TxtCardNumber = new PageTextBox(SourceWindow,
                                            GeneralUtils.GetResourceByName("global_card_number"),
                                            isRequired: false,
                                            isValidatable: false,
                                            includeSelectButton: false,
                                            includeKeyBoardButton: true);
        }

        private void InitializeTxtFiscalNumber()
        {
            TxtFiscalNumber = new PageTextBox(SourceWindow,
                                              GeneralUtils.GetResourceByName("global_fiscal_number"),
                                              isRequired: true,
                                              isValidatable: false,
                                              includeSelectButton: false,
                                              includeKeyBoardButton: true);
        }

        private void InitializeTxtCustomer()
        {
            TxtCustomer = new PageTextBox(SourceWindow,
                                          GeneralUtils.GetResourceByName("global_customer"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: true);

            TxtCustomer.SelectEntityClicked += BtnSelectCustomer_Clicked;
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
                CustomerId = page.SelectedEntity.Id;
                ShowCustomerData(page.SelectedEntity);
            }    
        }

        public void ShowCustomerData(Customer customer)
        {
            TxtFiscalNumber.Text = customer.FiscalNumber;
            TxtCardNumber.Text = customer.CardNumber;
            TxtDiscount.Text = customer.Discount?.ToString();
            TxtAddress.Text = customer.Address;
            TxtLocality.Text = customer.Locality;
            TxtZipCode.Text = customer.ZipCode;
            TxtCity.Text = customer.City;
            TxtCountry.Text = customer.Country.Designation;
            TxtCountry.SelectedEntity = customer.Country;
            TxtPhone.Text = customer.Phone;
            TxtEmail.Text = customer.Email;
            TxtNotes.Text = customer.Notes;
        }

        public void ImportDataFromDocument(Document document)
        {
            CustomerId = document.CustomerId;
            TxtCustomer.Text = document.Customer.Name;
            TxtFiscalNumber.Text = document.Customer.FiscalNumber;
            TxtAddress.Text = document.Customer.Address;
            TxtLocality.Text = document.Customer.Locality;
            TxtZipCode.Text = document.Customer.ZipCode;
            TxtCity.Text = document.Customer.City;
            TxtCountry.Text = document.Customer.Country;
            TxtDiscount.Text = document.Discount.ToString();
            TxtPhone.Text = document.Customer.Phone;
            TxtEmail.Text = document.Customer.Email;
            _countryId = document.Customer.CountryId;
        }

        private void Design()
        {
            var verticalLayout = new VBox(false, 2);
            verticalLayout.PackStart(TxtCustomer.Component, false, false, 0);

            verticalLayout.PackStart(PageTextBox.CreateHbox(TxtFiscalNumber,
                                                            TxtCardNumber,
                                                            TxtDiscount), false, false, 0);

            verticalLayout.PackStart(PageTextBox.CreateHbox(TxtAddress,
                                                            TxtLocality), false, false, 0);


            verticalLayout.PackStart(PageTextBox.CreateHbox(TxtZipCode,
                                                            TxtCity,
                                                            TxtCountry), false, false, 0);


            verticalLayout.PackStart(PageTextBox.CreateHbox(TxtPhone,
                                                            TxtEmail), false, false, 0);



            verticalLayout.PackStart(TxtNotes.Component, false, false, 0);

            PackStart(verticalLayout);
        }

        public Customer GetCustomer()
        {
            return TxtCustomer.SelectedEntity as Customer;
        }

        public DocumentCustomer GetDocumentCustomer()
        {
            return new DocumentCustomer
            {
                Name = TxtCustomer.Text,
                FiscalNumber = TxtFiscalNumber.Text,
                Address = TxtAddress.Text,
                Locality = TxtLocality.Text,
                ZipCode = TxtZipCode.Text,
                City = TxtCity.Text,
                Country = TxtCountry.Text,
                CountryId = (TxtCountry.SelectedEntity as Country)?.Id ?? _countryId,
                Email = TxtEmail.Text,
                Phone = TxtPhone.Text
            };
        }

        public override bool IsValid()
        {
            return TxtCustomer.IsValid() &&
                   TxtFiscalNumber.IsValid() &&
                   TxtDiscount.IsValid() &&
                   TxtCountry.IsValid();
        }
    }
}
