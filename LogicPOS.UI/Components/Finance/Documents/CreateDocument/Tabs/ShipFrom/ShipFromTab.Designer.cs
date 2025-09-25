using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Company;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class ShipFromTab
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
            TxtLocationId.Text=GeneralUtils.GetResourceByName("xml_value_unknown"); 
        }

        private void InitializeTxtWarehouseId()
        {
            TxtWarehouseId = new TextBox(SourceWindow,
                                             GeneralUtils.GetResourceByName("global_ship_from_warehouse_id"),
                                             isRequired: false,
                                             isValidatable: false,
                                             includeSelectButton: false,
                                             includeKeyBoardButton: true);
            TxtWarehouseId.Text = GeneralUtils.GetResourceByName("xml_value_unknown");
        }

        private void InitializeTxtDeliveryId()
        {
            TxtDeliveryId = new TextBox(SourceWindow,
                                            GeneralUtils.GetResourceByName("global_ship_from_delivery_id"),
                                            isRequired: false,
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

            TxtDeliveryDate.Entry.IsEditable = true;
            TxtDeliveryDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

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

            TxtCountry.Entry.IsEditable = true;
            var country = CountriesService.Default;
            var countries = CountriesService.Countries.Select(c => (c as object, c.Designation)).ToList();
            if (country != null) 
            { 
                TxtCountry.Text = country.Designation;
                TxtCountry.SelectedEntity = country;
             }

            TxtCountry.WithAutoCompletion(countries);
            TxtCountry.OnCompletionSelected += c => SelectCountry(c as Country);
            TxtCountry.Entry.Changed += TxtCoutry_Changed;
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

            TxtCity.Text = CompanyDetailsService.CompanyInformation.City;
        }

        private void InitializeTxtZipCode()
        {
            TxtZipCode = new TextBox(SourceWindow,
                                         GeneralUtils.GetResourceByName("global_zipcode"),
                                         isRequired: false,
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
            TxtRegion.Text = GeneralUtils.GetResourceByName("xml_value_unknown");
        }

        private void InitializeTxtAddress()
        {
            TxtAddress = new TextBox(SourceWindow,
                                         GeneralUtils.GetResourceByName("global_address"),
                                         isRequired: true,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true);

            TxtAddress.Text = CompanyDetailsService.CompanyInformation.Address;
        }

    }
}
