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

namespace LogicPOS.UI.Components.Documents.CreateDocumentModal
{
    public class CreateDocumentShipToTab : ModalTab
    {
        private PageTextBox TxtAddress { get; set; }
        private PageTextBox TxtRegion { get; set; }
        private PageTextBox TxtZipCode { get; set; }
        private PageTextBox TxtCity { get; set; }
        private PageTextBox TxtCountry { get; set; }
        private PageTextBox TxtDeliveryDate { get; set; }
        private PageTextBox TxtDeliveryId { get; set; }
        private PageTextBox TxtWarehouseId { get; set; }
        private PageTextBox TxtLocationId { get; set; }

        public CreateDocumentShipToTab(Window parent) : base(parent,
                                                             GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page4"),
                                                             PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_4_waybill_to.png")
        {
            Initialize();
            Design();
        }

        private void Design()
        {
            var verticalLayout = new VBox(false, 2);
            verticalLayout.PackStart(TxtAddress.Component, false, false, 0);
            verticalLayout.PackStart(TxtRegion.Component, false, false, 0);
            verticalLayout.PackStart(PageTextBox.CreateHbox(TxtZipCode,
                                                            TxtCity,
                                                            TxtCountry), false, false, 0);

            verticalLayout.PackStart(PageTextBox.CreateHbox(TxtDeliveryDate,
                                                            TxtDeliveryId), false, false, 0);

            verticalLayout.PackStart(PageTextBox.CreateHbox(TxtWarehouseId,
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
            TxtLocationId = new PageTextBox(SourceWindow,
                                            GeneralUtils.GetResourceByName("global_ship_to_location_id"),
                                            isRequired: true,
                                            isValidatable: false,
                                            includeSelectButton: false,
                                            includeKeyBoardButton: true);
        }

        private void InitializeTxtWarehouseId()
        {
            TxtWarehouseId = new PageTextBox(SourceWindow,
                                             GeneralUtils.GetResourceByName("global_ship_to_warehouse_id"),
                                             isRequired: true,
                                             isValidatable: false,
                                             includeSelectButton: false,
                                             includeKeyBoardButton: true);
        }

        private void InitializeTxtDeliveryId()
        {
            TxtDeliveryId = new PageTextBox(SourceWindow,
                                            GeneralUtils.GetResourceByName("global_ship_to_delivery_id"),
                                            isRequired: true,
                                            isValidatable: false,
                                            includeSelectButton: false,
                                            includeKeyBoardButton: true);
        }

        private void InitializeTxtDeliveryDate()
        {
            TxtDeliveryDate = new PageTextBox(SourceWindow,
                                              GeneralUtils.GetResourceByName("global_ship_to_delivery_date"),
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
            TxtCountry = new PageTextBox(SourceWindow,
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
            }
        }

        private void InitializeTxtCity()
        {
            TxtCity = new PageTextBox(SourceWindow,
                                      GeneralUtils.GetResourceByName("global_city"),
                                      isRequired: true,
                                      isValidatable: false,
                                      includeSelectButton: false,
                                      includeKeyBoardButton: true);
        }

        private void InitializeTxtZipCode()
        {
            TxtZipCode = new PageTextBox(SourceWindow,
                                         GeneralUtils.GetResourceByName("global_zipcode"),
                                         isRequired: true,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true);
        }

        private void InitializeTxtRegion()
        {
            TxtRegion = new PageTextBox(SourceWindow,
                                        GeneralUtils.GetResourceByName("global_region"),
                                        isRequired: true,
                                        isValidatable: false,
                                        includeSelectButton: false,
                                        includeKeyBoardButton: true);
        }

        private void InitializeTxtAddress()
        {
            TxtAddress = new PageTextBox(SourceWindow,
                                         GeneralUtils.GetResourceByName("global_address"),
                                         isRequired: true,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true);
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
                Country = TxtCountry.Text,
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
