using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class ShipFromTab : ModalTab
    {
        public ShipFromTab(Window parent) : base(parent,
                                                 GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page5"),
                                                 AppSettings.Paths.Images + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_5_waybill_from.png",
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
            var shipAddress = document.ShipFromAddress;

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

        public ShipAddress GetAddress()
        {
            return new ShipAddress
            {
                DeliveryID = TxtDeliveryId.Text,
                DeliveryDate = DateTime.ParseExact(TxtDeliveryDate.Text, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
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
                   TxtDeliveryDate.IsValid() &&
                   TxtDeliveryId.IsValid() &&
                   TxtWarehouseId.IsValid() &&
                   TxtLocationId.IsValid();
        }
    }
}
