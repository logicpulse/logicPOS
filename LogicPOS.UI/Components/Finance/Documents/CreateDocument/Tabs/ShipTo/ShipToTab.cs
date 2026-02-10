using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Globalization;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class ShipToTab : ModalTab
    {
        public ShipToTab(Window parent) : base(parent,
                                               GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page4"),
                                               AppSettings.Paths.Images + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_4_waybill_to.png",
                                               false)
        {
            Initialize();
            Design();
        }

        private void Initialize()
        {
            InitializeTxtAddress();
            InitializeTxtRegion();
            InitializeTxtZipCode();
            InitializeTxtCity();
            InitializeTxtCountry();
            InitializeTxtDeliveryDate();
            InitializeTxtDeliveryId();
            InitializeTxtWarehouseId();
            InitializeTxtLocationId();
        }

        private void SelectCountry(Api.Entities.Country country)
        {
            TxtCountry.Text = country.Designation;
            TxtCountry.SelectedEntity = country;
        }

        private void SelectCountryByCode2(string code2)
        {
            var country = CountriesService.GetByCode2(code2);
            if (country != null)
            {
                SelectCountry(country);
            }
        }

        public void ImportDataFromDocument(Document document)
        {
            var shipAddress = document.ShipToAddress;

            TxtAddress.Text = shipAddress.AddressDetail;
            TxtRegion.Text = shipAddress.Region;
            TxtZipCode.Text = shipAddress.PostalCode;
            TxtCity.Text = shipAddress.City;
            TxtDeliveryDate.Text = shipAddress.DeliveryDate.HasValue ? shipAddress.DeliveryDate.Value.ToString("yyyy-MM-ddTHH:mm:ss") : "";
            TxtDeliveryId.Text = shipAddress.DeliveryID;
            TxtWarehouseId.Text = shipAddress.WarehouseID;
            TxtLocationId.Text = shipAddress.LocationID;
            SelectCountryByCode2(shipAddress.Country);
        }

        public void ImportCustomerShipAddress(Customer customer)
        {
            TxtAddress.Text = customer.Address;
            TxtZipCode.Text = customer.ZipCode;
            TxtCity.Text = customer.City;
            TxtRegion.Text = customer.Locality;
            SelectCountryByCode2(customer.Country.Code2);
        }

        public void ImportCustomerShipAddress(DocumentCustomer customer)
        {
            TxtAddress.Text = customer.Address;
            TxtZipCode.Text = customer.ZipCode;
            TxtCity.Text = customer.City;
            TxtRegion.Text = customer.Locality;
            SelectCountryByCode2(customer.Country);
        }

        public ShipAddress GetAddress()
        {
            string countryCode2;
            
            if(TxtCountry.SelectedEntity is Api.Entities.Country selectedCountry)
            {
                countryCode2 = selectedCountry.Code2;
            }
            else
            {
                countryCode2 = (TxtCountry.SelectedEntity as Api.Features.Finance.Customers.Customers.Common.Country).Code2;
            }

            return new ShipAddress
            {
                DeliveryID = TxtDeliveryId.Text,
                DeliveryDate = DateTime.ParseExact(TxtDeliveryDate.Text, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                WarehouseID = TxtWarehouseId.Text,
                LocationID = TxtLocationId.Text,
                AddressDetail = TxtAddress.Text,
                PostalCode = TxtZipCode.Text,
                Country = countryCode2,
                City = TxtCity.Text,
                Region = TxtRegion.Text
            };
        }

        public override bool IsValid()
        {
            return TxtAddress.IsValid() &&
                  TxtRegion.IsValid() &&
                  TxtZipCode.IsValid() &&
                  TxtCity.IsValid() &&
                  TxtCountry.IsValid() &&
                  TxtDeliveryDate.IsValid();
        }
    }
}
