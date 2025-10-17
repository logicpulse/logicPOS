using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.Agt;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CustomerTab : ModalTab
    {
        public event Action<Api.Entities.Customer> CustomerSelected;
        public event Action<decimal> DiscountChanged;

        public CustomerTab(Window parent) : base(parent: parent,
                                                               name: GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page2"),
                                                               icon: AppSettings.Paths.Images + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_2_customer.png")
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

        public void ShowCustomerData(Api.Entities.Customer customer)
        {
            TxtFiscalNumber.Text = customer.FiscalNumber;
            TxtCardNumber.Text = customer.CardNumber;
            TxtDiscount.Text = customer.Discount.ToString("F2");
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
            TxtDiscount.Text = document.Discount.ToString("0.00");
            TxtPhone.Text = document.Customer.Phone;
            TxtEmail.Text = document.Customer.Email;

            var country = CountriesService.GetCountry(document.Customer.CountryId);
            TxtCountry.Text = country?.Designation;
            TxtCountry.SelectedEntity = country;
        }

        public Api.Entities.Customer GetCustomer()
        {
            return TxtCustomer.SelectedEntity as Api.Entities.Customer;
        }

        public string FiscalNumber => TxtFiscalNumber.Text;
        
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
                Country = (TxtCountry.SelectedEntity as Country)?.Code2,
                CountryId = (TxtCountry.SelectedEntity as Country).Id,
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

        private void SelectCustomer(Api.Entities.Customer customer)
        {
            TxtCustomer.Text = customer.Name;
            TxtCustomer.SelectedEntity = customer;
            CustomerId = customer.Id;
            ShowCustomerData(customer);
            FreezeEditableFields(customer.IsFinalConsumer);
            CustomerSelected?.Invoke(customer);
        }

        private void FreezeEditableFields(bool freeze = true)
        {
            TxtFiscalNumber.BtnKeyboard.Sensitive = TxtFiscalNumber.Entry.Sensitive = !freeze;
            TxtCardNumber.BtnKeyboard.Sensitive = TxtCardNumber.Entry.Sensitive = !freeze;
            TxtCustomer.BtnKeyboard.Sensitive = TxtCustomer.Entry.Sensitive = !freeze;
            TxtAddress.BtnKeyboard.Sensitive = TxtAddress.Entry.Sensitive = !freeze;
            TxtAddress.BtnKeyboard.Sensitive = TxtLocality.Entry.Sensitive = !freeze;
            TxtCountry.BtnKeyboard.Sensitive = TxtCountry.Entry.Sensitive = !freeze;
            TxtCity.BtnKeyboard.Sensitive = TxtCity.Entry.Sensitive = !freeze;
            TxtNotes.BtnKeyboard.Sensitive = TxtNotes.Entry.Sensitive = !freeze;
            TxtZipCode.BtnKeyboard.Sensitive = TxtZipCode.Entry.Sensitive = !freeze;
            TxtPhone.BtnKeyboard.Sensitive = TxtPhone.Entry.Sensitive = !freeze;
            TxtEmail.BtnKeyboard.Sensitive = TxtEmail.Entry.Sensitive = !freeze;
        }

        public void Clear()
        {
            CustomerId = null;
            TxtFiscalNumber.Clear();
            TxtCustomer.Clear();
            TxtCardNumber.Clear();
            TxtDiscount.Text = "0";
            TxtAddress.Clear();
            TxtLocality.Clear();
            TxtZipCode.Clear();
            TxtCity.Clear();
            TxtCountry.Clear();
            SelectDefaultCountry();
            TxtPhone.Clear();
            TxtEmail.Clear();
            TxtNotes.Clear();
        }

        public void FillWithAgtInfo()
        {
            if (TxtFiscalNumber.IsValid() == false || string.IsNullOrWhiteSpace(TxtFiscalNumber.Text))
            {
                CustomAlerts.Warning(this.SourceWindow).WithMessage("Informe o NIF").ShowAlert();
                return;
            }

            var contributor = AgtService.GetAgtContributorInfo(TxtFiscalNumber.Text);

            if (contributor == null)
            {
                CustomAlerts.Warning(this.SourceWindow).WithMessage("Não foi possível retornar os dados online do contribuinte.").ShowAlert();
                return;
            }

            Clear();

             TxtCustomer.Text = contributor.GetCustomerName() ?? "";
             TxtCustomer.SelectedEntity = null;

             TxtLocality.Text = contributor.GetLocality() ?? "";
             TxtAddress.Text = contributor.GetAddress() ?? "";
             TxtCity.Text = contributor.GetCity() ?? "";
             TxtPhone.Text = contributor.Phone ?? "";
             TxtEmail.Text = contributor.Email ?? "";
             TxtZipCode.Text = contributor.PostalCode ?? "";
        }
    }
}
