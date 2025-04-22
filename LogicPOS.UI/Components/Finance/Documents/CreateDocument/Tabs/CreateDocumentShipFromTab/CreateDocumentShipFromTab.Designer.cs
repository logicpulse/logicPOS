using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CreateDocumentShipFromTab
    {
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

    }
}
