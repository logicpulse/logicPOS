using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class DocumentShipToTab : ModalTab
    {
        public DocumentShipToTab(Window parent) : base(parent,
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

        private void SelectCountry(Country country)
        {
            TxtCountry.Text = country.Designation;
            TxtCountry.SelectedEntity = country;
        }

        public void ImportDataFromDocument(Document document)
        {
            var shipAddress = document.ShipToAddress;

            TxtAddress.Text = shipAddress.AddressDetail;
            TxtRegion.Text = shipAddress.Region;
            TxtZipCode.Text = shipAddress.PostalCode;
            TxtCity.Text = shipAddress.City;
            TxtCountry.Text = CountriesService.Countries
                                     .Where(c => c.Code2 == shipAddress.Country)
                                     .Select(c => c.Code2)
                                     .FirstOrDefault() ?? shipAddress.Country;

            TxtDeliveryDate.Text = shipAddress.DeliveryDate.ToString();
            TxtDeliveryId.Text = shipAddress.DeliveryID;
            TxtWarehouseId.Text = shipAddress.WarehouseID;
            TxtLocationId.Text = shipAddress.LocationID;
        }

        public void ImportCustomerShipAddress(Customer customer)
        {
            TxtAddress.Text = customer.Address;
            TxtZipCode.Text = customer.ZipCode;
            TxtCity.Text = customer.City;
            TxtRegion.Text = customer.Locality;
            TxtCountry.SelectedEntity = customer.Country;
            TxtCountry.Text = customer.Country.Designation;
        }

        public ShipAddress GetAddress()
        {
            return new ShipAddress
            {
                DeliveryID = TxtDeliveryId.Text,
                DeliveryDate = DateTime.Parse(TxtDeliveryDate.Text),
                WarehouseID = TxtWarehouseId.Text,
                LocationID = TxtLocationId.Text,
                AddressDetail = TxtAddress.Text,
                PostalCode = TxtZipCode.Text,
                Country = (TxtCountry.SelectedEntity as Country).Code2,
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
