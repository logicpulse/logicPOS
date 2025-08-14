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

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CreateDocumentShipToTab : ModalTab
    {
        public CreateDocumentShipToTab(Window parent) : base(parent,
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
        private List<Country> InitializeCountriesForCompletion()
        {
            _countriesForCompletion = CountryService.GetCountries();
            return _countriesForCompletion;
        }

        private void SelectCountry(Country country)
        {
            TxtCountry.Text = country.Designation;
            TxtCountry.SelectedEntity = country;
        }
        public void ImportDataFromDocument(Document document)
        {
            var shipAddress = document.ShipToAdress;

            TxtAddress.Text = shipAddress.AddressDetail;
            TxtRegion.Text = shipAddress.Region;
            TxtZipCode.Text = shipAddress.PostalCode;
            TxtCity.Text = shipAddress.City;
            TxtCountry.Text = CreateDocumentModal.GetCountries()
                                     .Where(c => c.Code2 == shipAddress.Country)
                                     .Select(c => c.Code2)
                                     .FirstOrDefault() ?? shipAddress.Country;

            TxtDeliveryDate.Text = shipAddress.DeliveryDate.ToString();
            TxtDeliveryId.Text = shipAddress.DeliveryID;
            TxtWarehouseId.Text = shipAddress.WarehouseID;
            TxtLocationId.Text = shipAddress.LocationID;
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
