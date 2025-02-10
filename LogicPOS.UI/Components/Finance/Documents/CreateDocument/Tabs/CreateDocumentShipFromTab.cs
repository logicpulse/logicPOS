using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents;
using LogicPOS.Settings;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public class CreateDocumentShipFromTab : ModalTab
    {
        private TextBox TxtAddress { get; set; }
        private TextBox TxtRegion { get; set; }
        private TextBox TxtZipCode { get; set; }
        private TextBox TxtCity { get; set; }
        private TextBox TxtCountry { get; set; }
        private TextBox TxtDeliveryDate { get; set; }
        private TextBox TxtDeliveryId { get; set; }
        private TextBox TxtWarehouseId { get; set; }
        private TextBox TxtLocationId { get; set; }

        public CreateDocumentShipFromTab(Window parent) : base(parent,
                                                               GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page5"),
                                                               PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_5_waybill_from.png",
                                                               false)
        {
            Initialize();
            Design();
        }

        private void Design()
        {
            var verticalLayout = new VBox(false, 2);
            verticalLayout.PackStart(TxtAddress.Component, false, false, 0);
            verticalLayout.PackStart(TxtRegion.Component, false, false, 0);
            verticalLayout.PackStart(TextBox.CreateHbox(TxtZipCode,
                                                            TxtCity,
                                                            TxtCountry), false, false, 0);

            verticalLayout.PackStart(TextBox.CreateHbox(TxtDeliveryDate,
                                                            TxtDeliveryId), false, false, 0);

            verticalLayout.PackStart(TextBox.CreateHbox(TxtWarehouseId,
                                                            TxtLocationId), false, false, 0);


            PackStart(verticalLayout);
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

        private void InitializeTxtLocationId()
        {
            TxtLocationId = new TextBox(SourceWindow,
                                            GeneralUtils.GetResourceByName("global_ship_to_location_id"),
                                            isRequired: true,
                                            isValidatable: false,
                                            includeSelectButton: false,
                                            includeKeyBoardButton: true);
        }

        private void InitializeTxtWarehouseId()
        {
            TxtWarehouseId = new TextBox(SourceWindow,
                                             GeneralUtils.GetResourceByName("global_ship_from_warehouse_id"),
                                             isRequired: true,
                                             isValidatable: false,
                                             includeSelectButton: false,
                                             includeKeyBoardButton: true);
        }

        private void InitializeTxtDeliveryId()
        {
            TxtDeliveryId = new TextBox(SourceWindow,
                                            GeneralUtils.GetResourceByName("global_ship_from_delivery_id"),
                                            isRequired: true,
                                            isValidatable: false,
                                            includeSelectButton: false,
                                            includeKeyBoardButton: true);
        }

        private void InitializeTxtDeliveryDate()
        {
            TxtDeliveryDate = new TextBox(SourceWindow,
                                              GeneralUtils.GetResourceByName("global_ship_from_delivery_date"),
                                              isRequired: true,
                                              isValidatable: false,
                                              includeSelectButton: true,
                                              includeKeyBoardButton: false);

            TxtDeliveryDate.Entry.IsEditable = false;

            TxtDeliveryDate.SelectEntityClicked += TxtDeliveryDate_SelectEntityClicked;
        }

        private void TxtDeliveryDate_SelectEntityClicked(object sender, EventArgs e)
        {
            var dateTimePicker = new DateTimePicker(SourceWindow);
            ResponseType response = (ResponseType)dateTimePicker.Run();
            dateTimePicker.Destroy();

            if (response == ResponseType.Ok)
            {
                TxtDeliveryDate.Text = dateTimePicker.Calendar.Date.ToString();
            }
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

            TxtCountry.SelectEntityClicked += TxtCountry_SelectEntityClicked;
        }

        private void TxtCountry_SelectEntityClicked(object sender, EventArgs e)
        {
            var page = new CountriesPage(null, PageOptions.SelectionPageOptions);
            var selectModal = new EntitySelectionModal<Country>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectModal.Run();
            selectModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCountry.Text = page.SelectedEntity.Designation;
                TxtCountry.SelectedEntity = page.SelectedEntity;
            }
        }

        private void InitializeTxtCity()
        {
            TxtCity = new TextBox(SourceWindow,
                                      GeneralUtils.GetResourceByName("global_city"),
                                      isRequired: true,
                                      isValidatable: false,
                                      includeSelectButton: false,
                                      includeKeyBoardButton: true);
        }

        private void InitializeTxtZipCode()
        {
            TxtZipCode = new TextBox(SourceWindow,
                                         GeneralUtils.GetResourceByName("global_zipcode"),
                                         isRequired: true,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true);
        }

        private void InitializeTxtRegion()
        {
            TxtRegion = new TextBox(SourceWindow,
                                        GeneralUtils.GetResourceByName("global_region"),
                                        isRequired: true,
                                        isValidatable: false,
                                        includeSelectButton: false,
                                        includeKeyBoardButton: true);
        }

        private void InitializeTxtAddress()
        {
            TxtAddress = new TextBox(SourceWindow,
                                         GeneralUtils.GetResourceByName("global_address"),
                                         isRequired: true,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true);
        }

        public void ImportDataFromDocument(Document document)
        {
            var shipAddress = document.ShipFromAdress;

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
                   TxtDeliveryDate.IsValid() &&
                   TxtDeliveryId.IsValid() &&
                   TxtWarehouseId.IsValid() &&
                   TxtLocationId.IsValid();
        }
    }
}
