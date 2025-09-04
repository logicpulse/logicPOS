using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CreateDocumentCustomerTab : ModalTab
    {

        public CreateDocumentCustomerTab(Window parent) : base(parent: parent,
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
            TxtDiscount.Text = document.Discount.ToString();
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
            TxtPhone.Clear();
            TxtEmail.Clear();
            TxtNotes.Clear();
        }
    }
}
