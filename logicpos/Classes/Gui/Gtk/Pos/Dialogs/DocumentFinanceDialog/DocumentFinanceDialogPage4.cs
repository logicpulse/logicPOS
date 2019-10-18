using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;
using System;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    class DocumentFinanceDialogPage4 : PagePadPage
    {
        private Session _session;
        private DocumentFinanceDialogPagePad _pagePad;
        private cfg_configurationcountry _intialValueConfigurationCountry;
        //UI
        private EntryBoxValidationDatePickerDialog _entryBoxShipToDeliveryDate;
        public EntryBoxValidationDatePickerDialog EntryBoxShipToDeliveryDate
        {
            get { return _entryBoxShipToDeliveryDate; }
        }

        private EntryBoxValidation _entryBoxShipToDeliveryID;
        public EntryBoxValidation EntryBoxShipToDeliveryID
        {
            get { return _entryBoxShipToDeliveryID; }
        }

        private EntryBoxValidation _entryBoxShipToWarehouseID;
        public EntryBoxValidation EntryBoxShipToWarehouseID
        {
            get { return _entryBoxShipToWarehouseID; }
        }

        private EntryBoxValidation _entryBoxShipToLocationID;
        public EntryBoxValidation EntryBoxShipToLocationID
        {
            get { return _entryBoxShipToLocationID; }
        }

        private EntryBoxValidation _entryBoxShipToAddressDetail;
        public EntryBoxValidation EntryBoxShipToAddressDetail
        {
            get { return _entryBoxShipToAddressDetail; }
        }

        private EntryBoxValidation _entryBoxShipToRegion;
        public EntryBoxValidation EntryBoxShipToRegion
        {
            get { return _entryBoxShipToRegion; }
        }

        private EntryBoxValidation _entryBoxShipToPostalCode;
        public EntryBoxValidation EntryBoxShipToPostalCode
        {
            get { return _entryBoxShipToPostalCode; }
        }

        private EntryBoxValidation _entryBoxShipToCity;
        public EntryBoxValidation EntryBoxShipToCity
        {
            get { return _entryBoxShipToCity; }
        }

        private XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry> _entryBoxSelectShipToCountry;
        public XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry> EntryBoxSelectShipToCountry
        {
            get { return _entryBoxSelectShipToCountry; }
        }

        //Constructor
        public DocumentFinanceDialogPage4(Window pSourceWindow, String pPageName) 
            : this(pSourceWindow, pPageName, "", null, true) { }
        public DocumentFinanceDialogPage4(Window pSourceWindow, String pPageName, Widget pWidget) 
            : this(pSourceWindow, pPageName, "", pWidget, true) { }
        public DocumentFinanceDialogPage4(Window pSourceWindow, String pPageName, String pPageIcon, Widget pWidget, bool pEnabled = true)
            : base(pSourceWindow, pPageName, pPageIcon, pWidget, pEnabled)
        {
            //Init private vars
            _pagePad = (_sourceWindow as PosDocumentFinanceDialog).PagePad;
            _session = (_pagePad as DocumentFinanceDialogPagePad).Session;

            //Initial Values
            _intialValueConfigurationCountry = SettingsApp.ConfigurationSystemCountry;

            //ShipTo Address
            _entryBoxShipToAddressDetail = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_address"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericPlus, true);/* IN009253 */
            _entryBoxShipToAddressDetail.EntryValidation.Changed += delegate { Validate(); };

            //ShipTo Region
            _entryBoxShipToRegion = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_region"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericPlus, false);/* IN009253 */
            _entryBoxShipToRegion.EntryValidation.Changed += delegate { Validate(); };

            //ShipTo PostalCode
            _entryBoxShipToPostalCode = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_zipcode"), KeyboardMode.Alfa, SettingsApp.ConfigurationSystemCountry.RegExZipCode, true);
            _entryBoxShipToPostalCode.EntryValidation.Changed += delegate { Validate(); };

            //ShipTo City
            _entryBoxShipToCity = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_city"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericPlus, true);/* IN009253 */
            _entryBoxShipToCity.EntryValidation.Changed += delegate { Validate(); };

            //ShipTo Country
            CriteriaOperator criteriaOperatorCustomerCountry = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            _entryBoxSelectShipToCountry = new XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry>(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_country"), "Designation", "Oid", _intialValueConfigurationCountry, criteriaOperatorCustomerCountry, SettingsApp.RegexGuid, true);
            _entryBoxSelectShipToCountry.EntryValidation.Validate(_entryBoxSelectShipToCountry.Value.Oid.ToString());
            _entryBoxSelectShipToCountry.EntryValidation.IsEditable = false;
            _entryBoxSelectShipToCountry.EntryValidation.Changed += delegate { Validate(); };
            _entryBoxSelectShipToCountry.ClosePopup += delegate
            {
                //Require to Update RegExZipCode
                _entryBoxShipToPostalCode.EntryValidation.Rule = _entryBoxSelectShipToCountry.Value.RegExZipCode;
                _entryBoxShipToPostalCode.EntryValidation.Validate();
            };

            //ShipToDeliveryDate
            _entryBoxShipToDeliveryDate = new EntryBoxValidationDatePickerDialog(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_ship_to_delivery_date"), _pagePad.DateTimeFormat, _pagePad.InitalDateTime, KeyboardMode.AlfaNumeric, SettingsApp.RegexDateTime, true, _pagePad.DateTimeFormat);
            _entryBoxShipToDeliveryDate.EntryValidation.Sensitive = true;
            //_entryBoxShipToDeliveryDate.EntryValidation.Text = _pagePad.InitalDateTime.ToString(_pagePad.DateTimeFormat);
            _entryBoxShipToDeliveryDate.EntryValidation.Validate();
            //Assign Min Date to Validation
            _entryBoxShipToDeliveryDate.DateTimeMin = FrameworkUtils.CurrentDateTimeAtomic();
            _entryBoxShipToDeliveryDate.EntryValidation.Changed += _entryBoxShipToDeliveryDate_Changed;
            _entryBoxShipToDeliveryDate.ClosePopup += _entryBoxShipToDeliveryDate_Changed;

            //ShipToDeliveryID
            _entryBoxShipToDeliveryID = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_ship_to_delivery_id"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false);
            _entryBoxShipToDeliveryID.EntryValidation.Changed += delegate { Validate(); };

            //ShipToWarehouseID
            _entryBoxShipToWarehouseID = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_ship_to_warehouse_id"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false);
            _entryBoxShipToWarehouseID.EntryValidation.MaxLength = 50;
            _entryBoxShipToWarehouseID.EntryValidation.Changed += delegate { Validate(); };

            //ShipToLocationID
            _entryBoxShipToLocationID = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_ship_to_location_id"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false);
            _entryBoxShipToLocationID.EntryValidation.MaxLength = 30;
            _entryBoxShipToLocationID.EntryValidation.Changed += delegate { Validate(); };

            //HBox ZipCode+City+Country
            HBox hboxZipCodeAndCityAndCountry = new HBox(true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(_entryBoxShipToPostalCode, true, true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(_entryBoxShipToCity, true, true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(_entryBoxSelectShipToCountry, true, true, 0);

            //HBox hboxDeliveryDate+DeliveryID
            HBox hboxDeliveryDateAndDeliveryID = new HBox(true, 0);
            hboxDeliveryDateAndDeliveryID.PackStart(_entryBoxShipToDeliveryDate, true, true, 0);
            hboxDeliveryDateAndDeliveryID.PackStart(_entryBoxShipToDeliveryID, true, true, 0);

            //HBox hboxWarehouseID+LocationID
            HBox hboxhboxWarehouseIDAndLocationID = new HBox(true, 0);
            hboxhboxWarehouseIDAndLocationID.PackStart(_entryBoxShipToWarehouseID, true, true, 0);
            hboxhboxWarehouseIDAndLocationID.PackStart(_entryBoxShipToLocationID, true, true, 0);

            //Pack VBOX
            VBox vbox = new VBox(false, 2);

            vbox.PackStart(_entryBoxShipToAddressDetail, false, false, 0);
            vbox.PackStart(_entryBoxShipToRegion, false, false, 0);
            vbox.PackStart(hboxZipCodeAndCityAndCountry, false, false, 0);
            vbox.PackStart(hboxDeliveryDateAndDeliveryID, false, false, 0);
            vbox.PackStart(hboxhboxWarehouseIDAndLocationID, false, false, 0);
            PackStart(vbox);
        }

        //Override Base Validate
        public override void Validate()
        {
            _validated = (
              _entryBoxShipToAddressDetail.EntryValidation.Validated &&
              _entryBoxShipToRegion.EntryValidation.Validated &&
              _entryBoxShipToPostalCode.EntryValidation.Validated &&
              _entryBoxShipToCity.EntryValidation.Validated &&
              _entryBoxSelectShipToCountry.EntryValidation.Validated &&
              _entryBoxShipToDeliveryDate.EntryValidation.Validated &&
              _entryBoxShipToDeliveryID.EntryValidation.Validated &&
              _entryBoxShipToWarehouseID.EntryValidation.Validated &&
              _entryBoxShipToLocationID.EntryValidation.Validated
            );

            //Enable Next Button, If not In Last Page
            if (_pagePad.CurrentPageIndex < _pagePad.Pages.Count - 1 && _pagePad.CurrentPageIndex == 3)
            {
                _pagePad.ButtonNext.Sensitive = _validated;
            }

            //Validate Dialog (All Pages must be Valid)
            (_sourceWindow as PosDocumentFinanceDialog).Validate();
        }

        public void ToggleValidation(bool pIsRequired)
        {
            //Toggle
            _entryBoxShipToAddressDetail.EntryValidation.Required = pIsRequired;
            //_entryBoxShipToRegion.EntryValidation.Required = pIsRequired;
            _entryBoxShipToPostalCode.EntryValidation.Required = pIsRequired;
            _entryBoxShipToCity.EntryValidation.Required = pIsRequired;
            _entryBoxShipToDeliveryDate.EntryValidation.Required = pIsRequired;
            _entryBoxSelectShipToCountry.EntryValidation.Required = pIsRequired;
            //Call Validate
            _entryBoxShipToAddressDetail.EntryValidation.Validate();
            _entryBoxShipToRegion.EntryValidation.Validate();
            _entryBoxShipToPostalCode.EntryValidation.Validate();
            _entryBoxShipToCity.EntryValidation.Validate();
            _entryBoxShipToDeliveryDate.EntryValidation.Validate();
            _entryBoxSelectShipToCountry.EntryValidation.Validate(
                (_entryBoxSelectShipToCountry.Value != null) ? _entryBoxSelectShipToCountry.Value.Oid.ToString() : string.Empty
            );
        }

        public void ClearShipTo()
        {
            //Clear ShipTo
            if (_entryBoxShipToDeliveryID.EntryValidation.Text != string.Empty) _entryBoxShipToDeliveryID.EntryValidation.Text = string.Empty;
            if (_entryBoxShipToDeliveryDate.EntryValidation.Text != string.Empty) _entryBoxShipToDeliveryDate.EntryValidation.Text = string.Empty;
            if (_entryBoxShipToWarehouseID.EntryValidation.Text != string.Empty) _entryBoxShipToWarehouseID.EntryValidation.Text = string.Empty;
            if (_entryBoxShipToLocationID.EntryValidation.Text != string.Empty) _entryBoxShipToLocationID.EntryValidation.Text = string.Empty;
            if (_entryBoxShipToAddressDetail.EntryValidation.Text != string.Empty) _entryBoxShipToAddressDetail.EntryValidation.Text = string.Empty;
            if (_entryBoxShipToCity.EntryValidation.Text != string.Empty) _entryBoxShipToCity.EntryValidation.Text = string.Empty;
            if (_entryBoxShipToPostalCode.EntryValidation.Text != string.Empty) _entryBoxShipToPostalCode.EntryValidation.Text = string.Empty;
            if (_entryBoxShipToRegion.EntryValidation.Text != string.Empty) _entryBoxShipToRegion.EntryValidation.Text = string.Empty;
            //Reset to Default Country
            _entryBoxSelectShipToCountry.Value = _intialValueConfigurationCountry;
        }

        //Equal to ShipFrom
        void _entryBoxShipToDeliveryDate_Changed(object sender, EventArgs e)
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
                _log.Error(ex.Message, ex);
            }
        }
    }
}
