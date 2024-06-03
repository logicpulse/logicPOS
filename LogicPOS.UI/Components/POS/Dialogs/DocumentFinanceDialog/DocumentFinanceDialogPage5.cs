using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Utility;
using System;

//THIS CLASS is ALMOST EQUAL to DocumentFinanceDialogPage4, but with Search Replace 
//"ShipTo" to "ShipFrom" and "_ship_to_" to ""_ship_from_""

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    internal class DocumentFinanceDialogPage5 : PagePadPage
    {
        private readonly Session _session;
        private readonly DocumentFinanceDialogPagePad _pagePad;
        private readonly cfg_configurationcountry _intialValueConfigurationCountry;

        public EntryBoxValidationDatePickerDialog EntryBoxShipFromDeliveryDate { get; }

        public EntryBoxValidation EntryBoxShipFromDeliveryID { get; }

        public EntryBoxValidation EntryBoxShipFromWarehouseID { get; }

        public EntryBoxValidation EntryBoxShipFromLocationID { get; }

        public EntryBoxValidation EntryBoxShipFromAddressDetail { get; }

        public EntryBoxValidation EntryBoxShipFromRegion { get; }

        public EntryBoxValidation EntryBoxShipFromPostalCode { get; }

        public EntryBoxValidation EntryBoxShipFromCity { get; }

        public XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry> EntryBoxSelectShipFromCountry { get; }

        //Constructor
        public DocumentFinanceDialogPage5(Window pSourceWindow, string pPageName) : this(pSourceWindow, pPageName, "", null, true) { }
        public DocumentFinanceDialogPage5(Window pSourceWindow, string pPageName, Widget pWidget) : this(pSourceWindow, pPageName, "", pWidget, true) { }
        public DocumentFinanceDialogPage5(Window pSourceWindow, string pPageName, string pPageIcon, Widget pWidget, bool pEnabled = true)
            : base(pSourceWindow, pPageName, pPageIcon, pWidget, pEnabled)
        {
            //Init private vars
            _pagePad = (_sourceWindow as PosDocumentFinanceDialog).PagePad;
            _session = (_pagePad as DocumentFinanceDialogPagePad).Session;

            //Initials Values
            _intialValueConfigurationCountry = XPOSettings.ConfigurationSystemCountry;

            //ShipFrom Address
            EntryBoxShipFromAddressDetail = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_address"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericPlus, true);/* IN009253 */
            EntryBoxShipFromAddressDetail.EntryValidation.Changed += delegate { Validate(); };

            //ShipFrom Region
            EntryBoxShipFromRegion = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_region"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericPlus, false);/* IN009253 */
            EntryBoxShipFromRegion.EntryValidation.Changed += delegate { Validate(); };

            //ShipFrom PostalCode
            EntryBoxShipFromPostalCode = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_zipcode"), KeyboardMode.Alfa, XPOSettings.ConfigurationSystemCountry.RegExZipCode, true);
            EntryBoxShipFromPostalCode.EntryValidation.Changed += delegate { Validate(); };

            //ShipFrom City
            EntryBoxShipFromCity = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_city"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericPlus, true);/* IN009253 */
            EntryBoxShipFromCity.EntryValidation.Changed += delegate { Validate(); };

            //ShipFrom Country
            CriteriaOperator criteriaOperatorCustomerCountry = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            EntryBoxSelectShipFromCountry = new XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry>(
                _sourceWindow,
                CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_country"),
                "Designation",
                "Oid",
                _intialValueConfigurationCountry,
                criteriaOperatorCustomerCountry,
                LogicPOS.Utility.RegexUtils.RegexGuid,
                true);

            EntryBoxSelectShipFromCountry.EntryValidation.IsEditable = false;
            EntryBoxSelectShipFromCountry.EntryValidation.Changed += delegate { Validate(); };
            EntryBoxSelectShipFromCountry.ClosePopup += delegate
            {
                //Require to Update RegExZipCode
                EntryBoxShipFromPostalCode.EntryValidation.Rule = EntryBoxSelectShipFromCountry.Value.RegExZipCode;
                EntryBoxShipFromPostalCode.EntryValidation.Validate();
            };

            //ShipFromDeliveryDate
            EntryBoxShipFromDeliveryDate = new EntryBoxValidationDatePickerDialog(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ship_from_delivery_date"), _pagePad.DateTimeFormat, _pagePad.InitalDateTime, KeyboardMode.AlfaNumeric, LogicPOS.Utility.RegexUtils.RegexDateTime, true, _pagePad.DateTimeFormat);
            EntryBoxShipFromDeliveryDate.EntryValidation.Sensitive = true;
            EntryBoxShipFromDeliveryDate.EntryValidation.Text = XPOUtility.DateTimeToString(XPOUtility.CurrentDateTimeAtomic()).ToString();
            EntryBoxShipFromDeliveryDate.EntryValidation.Validate();
            //Assign Min Date to Validation
            EntryBoxShipFromDeliveryDate.DateTimeMin = XPOUtility.CurrentDateTimeAtomic();
            EntryBoxShipFromDeliveryDate.EntryValidation.Changed += _entryBoxShipFromDeliveryDate_ClosePopup;
            EntryBoxShipFromDeliveryDate.ClosePopup += _entryBoxShipFromDeliveryDate_ClosePopup;

            //ShipFromDeliveryID
            EntryBoxShipFromDeliveryID = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ship_from_delivery_id"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false);
            EntryBoxShipFromDeliveryID.EntryValidation.Changed += delegate { Validate(); };

            //ShipFromWarehouseID
            EntryBoxShipFromWarehouseID = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ship_from_warehouse_id"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false);
            EntryBoxShipFromWarehouseID.EntryValidation.MaxLength = 50;
            EntryBoxShipFromWarehouseID.EntryValidation.Changed += delegate { Validate(); };

            //ShipFromLocationID
            EntryBoxShipFromLocationID = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ship_from_location_id"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false);
            EntryBoxShipFromLocationID.EntryValidation.MaxLength = 30;
            EntryBoxShipFromLocationID.EntryValidation.Changed += delegate { Validate(); };

            //HBox hboxDeliveryDate+DeliveryID
            HBox hboxDeliveryDateAndDeliveryID = new HBox(true, 0);
            hboxDeliveryDateAndDeliveryID.PackStart(EntryBoxShipFromDeliveryDate, true, true, 0);
            hboxDeliveryDateAndDeliveryID.PackStart(EntryBoxShipFromDeliveryID, true, true, 0);

            //HBox ZipCode+City+Country
            HBox hboxZipCodeAndCityAndCountry = new HBox(true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(EntryBoxShipFromPostalCode, true, true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(EntryBoxShipFromCity, true, true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(EntryBoxSelectShipFromCountry, true, true, 0);

            //HBox hboxWarehouseID+LocationID
            HBox hboxhboxWarehouseIDAndLocationID = new HBox(true, 0);
            hboxhboxWarehouseIDAndLocationID.PackStart(EntryBoxShipFromWarehouseID, true, true, 0);
            hboxhboxWarehouseIDAndLocationID.PackStart(EntryBoxShipFromLocationID, true, true, 0);

            //Pack VBOX
            VBox vbox = new VBox(false, 2);
            vbox.PackStart(EntryBoxShipFromAddressDetail, false, false, 0);
            vbox.PackStart(EntryBoxShipFromRegion, false, false, 0);
            vbox.PackStart(hboxZipCodeAndCityAndCountry, false, false, 0);
            vbox.PackStart(hboxDeliveryDateAndDeliveryID, false, false, 0);
            vbox.PackStart(hboxhboxWarehouseIDAndLocationID, false, false, 0);
            PackStart(vbox);
        }

        //Override Base Validate
        public override void Validate()
        {
            _validated =
              EntryBoxShipFromAddressDetail.EntryValidation.Validated &&
              EntryBoxShipFromRegion.EntryValidation.Validated &&
              EntryBoxShipFromPostalCode.EntryValidation.Validated &&
              EntryBoxShipFromCity.EntryValidation.Validated &&
              EntryBoxSelectShipFromCountry.EntryValidation.Validated &&
              EntryBoxShipFromDeliveryDate.EntryValidation.Validated &&
              EntryBoxShipFromDeliveryID.EntryValidation.Validated &&
              EntryBoxShipFromWarehouseID.EntryValidation.Validated &&
              EntryBoxShipFromLocationID.EntryValidation.Validated
            ;

            //Enable Next Button, If not In Last Page
            if (_pagePad.CurrentPageIndex < _pagePad.Pages.Count - 1 && _pagePad.CurrentPageIndex == 4)
            {
                _pagePad.ButtonNext.Sensitive = _validated;
            }

            //Validate Dialog (All Pages must be Valid)
            (_sourceWindow as PosDocumentFinanceDialog).Validate();
        }

        public void ToggleValidation(bool pIsRequired)
        {
            //Toggle
            EntryBoxShipFromAddressDetail.EntryValidation.Required = pIsRequired;
            //_entryBoxShipFromRegion.EntryValidation.Required = pIsRequired;
            EntryBoxShipFromPostalCode.EntryValidation.Required = pIsRequired;
            EntryBoxShipFromCity.EntryValidation.Required = pIsRequired;
            EntryBoxShipFromDeliveryDate.EntryValidation.Required = pIsRequired;
            EntryBoxSelectShipFromCountry.EntryValidation.Required = pIsRequired;
            //Call Validate
            EntryBoxShipFromAddressDetail.EntryValidation.Validate();
            EntryBoxShipFromRegion.EntryValidation.Validate();
            EntryBoxShipFromPostalCode.EntryValidation.Validate();
            EntryBoxShipFromCity.EntryValidation.Validate();
            EntryBoxShipFromDeliveryDate.EntryValidation.Validate();
            EntryBoxSelectShipFromCountry.EntryValidation.Validate(
                (EntryBoxSelectShipFromCountry.Value != null) ? EntryBoxSelectShipFromCountry.Value.Oid.ToString() : string.Empty
            );
        }

        public void AssignShipFromDefaults()
        {
            //Initials Values
            /* IN007018 */
            string initialShipFromAddressDetail = PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_ADDRESS");
            string initialShipFromRegion = PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_REGION");
            string initialShipFromPostalCode = PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_POSTALCODE");
            string initialShipFromCity = PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_CITY");

            cfg_configurationcountry intialValueConfigurationCountry = XPOSettings.ConfigurationSystemCountry;
            /* IN007018 
             * There is no checking for installed country x company country, therefore, when registering company it is allowed to register the company for a different country than the deployed one.
             * So, we are seeing address from a country but validation rules from another one.
             */
            //string initialShipFromCountry = FrameworkUtils.GetPreferenceParameter("COMPANY_COUNTRY");

            //ShipFrom Address
            EntryBoxShipFromAddressDetail.EntryValidation.Text = initialShipFromAddressDetail;
            EntryBoxShipFromAddressDetail.EntryValidation.Validate();
            //ShipFrom Region
            EntryBoxShipFromRegion.EntryValidation.Text = initialShipFromRegion;
            EntryBoxShipFromRegion.EntryValidation.Validate();
            //ShipFrom PostalCode
            EntryBoxShipFromPostalCode.EntryValidation.Text = initialShipFromPostalCode;
            /* IN007018 
             * There is no checking for installed country x company country, therefore, when registering company it is allowed to register the company for a different country than the deployed one.
             * So, we are seeing Postal Code from a country but validation rules from another one, hence removing validatation on page defaults.
             */
            //_entryBoxShipFromPostalCode.EntryValidation.Rule = String.Empty;
            EntryBoxShipFromPostalCode.EntryValidation.Validate();
            //_entryBoxShipFromPostalCode.EntryValidation.Validate();
            //ShipFrom City
            EntryBoxShipFromCity.EntryValidation.Text = initialShipFromCity;
            EntryBoxShipFromCity.EntryValidation.Validate();
            //ShipFrom Country
            EntryBoxSelectShipFromCountry.Value = intialValueConfigurationCountry;
            /* IN007018 */
            //_entryBoxSelectShipFromCountry.EntryValidation.Text = initialShipFromCountry; 
            EntryBoxSelectShipFromCountry.EntryValidation.Validate(EntryBoxSelectShipFromCountry.Value.Oid.ToString());
            //ShipFromDeliveryDate
            //_entryBoxShipFromDeliveryDate.EntryValidation.Text = initialShipFromDeliveryDate;
            //_entryBoxShipFromDeliveryDate.EntryValidation.Validate();
        }

        public void ClearShipFrom()
        {
            //Clear ShipFrom
            if (EntryBoxShipFromDeliveryID.EntryValidation.Text != string.Empty) EntryBoxShipFromDeliveryID.EntryValidation.Text = string.Empty;
            if (EntryBoxShipFromDeliveryDate.EntryValidation.Text != string.Empty) EntryBoxShipFromDeliveryDate.EntryValidation.Text = string.Empty;
            if (EntryBoxShipFromWarehouseID.EntryValidation.Text != string.Empty) EntryBoxShipFromWarehouseID.EntryValidation.Text = string.Empty;
            if (EntryBoxShipFromLocationID.EntryValidation.Text != string.Empty) EntryBoxShipFromLocationID.EntryValidation.Text = string.Empty;
            if (EntryBoxShipFromAddressDetail.EntryValidation.Text != string.Empty) EntryBoxShipFromAddressDetail.EntryValidation.Text = string.Empty;
            if (EntryBoxShipFromCity.EntryValidation.Text != string.Empty) EntryBoxShipFromCity.EntryValidation.Text = string.Empty;
            if (EntryBoxShipFromPostalCode.EntryValidation.Text != string.Empty) EntryBoxShipFromPostalCode.EntryValidation.Text = string.Empty;
            if (EntryBoxShipFromRegion.EntryValidation.Text != string.Empty) EntryBoxShipFromRegion.EntryValidation.Text = string.Empty;
            //Reset to Default Country
            EntryBoxSelectShipFromCountry.Value = _intialValueConfigurationCountry;
        }

        //Equal to ShipTo
        private void _entryBoxShipFromDeliveryDate_ClosePopup(object sender, EventArgs e)
        {
            try
            {
                //Now this Extra validation is done in EntryBoxValidationDatePickerDialog

                //EntryBoxValidationDatePickerDialog entryDatePicker = (EntryBoxValidationDatePickerDialog)(sender as EntryValidation).Parent.Parent.Parent;

                //DateTime currentDateTime = FrameworkUtils.CurrentDateTimeAtomic();
                //if (entryDatePicker.EntryValidation.Validated)
                //{
                //    entryDatePicker.EntryValidation.Validated = (entryDatePicker.Value > currentDateTime.Date);
                //    Utils.ValidateUpdateColors(entryDatePicker.EntryValidation, entryDatePicker.EntryValidation.Label, entryDatePicker.EntryValidation.Validated);
                //}

                //Always Validate Dialog
                Validate();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
