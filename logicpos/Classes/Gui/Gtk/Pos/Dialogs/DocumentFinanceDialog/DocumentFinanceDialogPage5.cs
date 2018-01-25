using Gtk;
using System;
using logicpos.Classes.Gui.Gtk.Widgets;
using DevExpress.Xpo;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.datalayer.DataLayer.Xpo;
using DevExpress.Data.Filtering;
using logicpos.financial;
using logicpos.App;
using logicpos.resources.Resources.Localization;

//THIS CLASS is ALMOST EQUAL to DocumentFinanceDialogPage4, but with Search Replace 
//"ShipTo" to "ShipFrom" and "_ship_to_" to ""_ship_from_""

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    class DocumentFinanceDialogPage5 : PagePadPage
    {
        private Session _session;
        private DocumentFinanceDialogPagePad _pagePad;
        private CFG_ConfigurationCountry _intialValueConfigurationCountry;
        //UI
        private EntryBoxValidationDatePickerDialog _entryBoxShipFromDeliveryDate;
        public EntryBoxValidationDatePickerDialog EntryBoxShipFromDeliveryDate
        {
            get { return _entryBoxShipFromDeliveryDate; }
        }

        private EntryBoxValidation _entryBoxShipFromDeliveryID;
        public EntryBoxValidation EntryBoxShipFromDeliveryID
        {
            get { return _entryBoxShipFromDeliveryID; }
        }

        private EntryBoxValidation _entryBoxShipFromWarehouseID;
        public EntryBoxValidation EntryBoxShipFromWarehouseID
        {
            get { return _entryBoxShipFromWarehouseID; }
        }

        private EntryBoxValidation _entryBoxShipFromLocationID;
        public EntryBoxValidation EntryBoxShipFromLocationID
        {
            get { return _entryBoxShipFromLocationID; }
        }

        private EntryBoxValidation _entryBoxShipFromAddressDetail;
        public EntryBoxValidation EntryBoxShipFromAddressDetail
        {
            get { return _entryBoxShipFromAddressDetail; }
        }

        private EntryBoxValidation _entryBoxShipFromRegion;
        public EntryBoxValidation EntryBoxShipFromRegion
        {
            get { return _entryBoxShipFromRegion; }
        }

        private EntryBoxValidation _entryBoxShipFromPostalCode;
        public EntryBoxValidation EntryBoxShipFromPostalCode
        {
            get { return _entryBoxShipFromPostalCode; }
        }

        private EntryBoxValidation _entryBoxShipFromCity;
        public EntryBoxValidation EntryBoxShipFromCity
        {
            get { return _entryBoxShipFromCity; }
        }

        private XPOEntryBoxSelectRecordValidation<CFG_ConfigurationCountry, TreeViewConfigurationCountry> _entryBoxSelectShipFromCountry;
        public XPOEntryBoxSelectRecordValidation<CFG_ConfigurationCountry, TreeViewConfigurationCountry> EntryBoxSelectShipFromCountry
        {
            get { return _entryBoxSelectShipFromCountry; }
        }

        //Constructor
        public DocumentFinanceDialogPage5(Window pSourceWindow, String pPageName) : this(pSourceWindow, pPageName, "", null, true) { }
        public DocumentFinanceDialogPage5(Window pSourceWindow, String pPageName, Widget pWidget) : this(pSourceWindow, pPageName, "", pWidget, true) { }
        public DocumentFinanceDialogPage5(Window pSourceWindow, String pPageName, String pPageIcon, Widget pWidget, bool pEnabled = true)
            : base(pSourceWindow, pPageName, pPageIcon, pWidget, pEnabled)
        {
            //Init private vars
            _pagePad = (_sourceWindow as PosDocumentFinanceDialog).PagePad;
            _session = (_pagePad as DocumentFinanceDialogPagePad).Session;

            //Initials Values
            _intialValueConfigurationCountry = SettingsApp.ConfigurationSystemCountry;

            //ShipFrom Address
            _entryBoxShipFromAddressDetail = new EntryBoxValidation(_sourceWindow, Resx.global_address, KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, true);
            _entryBoxShipFromAddressDetail.EntryValidation.Changed += delegate { Validate(); };

            //ShipFrom Region
            _entryBoxShipFromRegion = new EntryBoxValidation(_sourceWindow, Resx.global_region, KeyboardMode.Alfa, SettingsApp.RegexAlfa, false);
            _entryBoxShipFromRegion.EntryValidation.Changed += delegate { Validate(); };

            //ShipFrom PostalCode
            _entryBoxShipFromPostalCode = new EntryBoxValidation(_sourceWindow, Resx.global_zipcode, KeyboardMode.Alfa, SettingsApp.ConfigurationSystemCountry.RegExZipCode, true);
            _entryBoxShipFromPostalCode.EntryValidation.Changed += delegate { Validate(); };

            //ShipFrom City
            _entryBoxShipFromCity = new EntryBoxValidation(_sourceWindow, Resx.global_city, KeyboardMode.Alfa, SettingsApp.RegexAlfa, true);
            _entryBoxShipFromCity.EntryValidation.Changed += delegate { Validate(); };

            //ShipFrom Country
            CriteriaOperator criteriaOperatorCustomerCountry = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            _entryBoxSelectShipFromCountry = new XPOEntryBoxSelectRecordValidation<CFG_ConfigurationCountry, TreeViewConfigurationCountry>(_sourceWindow, Resx.global_country, "Designation", "Oid", _intialValueConfigurationCountry, criteriaOperatorCustomerCountry, SettingsApp.RegexGuid, true);
            _entryBoxSelectShipFromCountry.EntryValidation.IsEditable = false;
            _entryBoxSelectShipFromCountry.EntryValidation.Changed += delegate { Validate(); };
            _entryBoxSelectShipFromCountry.ClosePopup += delegate
            {
                //Require to Update RegExZipCode
                _entryBoxShipFromPostalCode.EntryValidation.Rule = _entryBoxSelectShipFromCountry.Value.RegExZipCode;
                _entryBoxShipFromPostalCode.EntryValidation.Validate();
            };

            //ShipFromDeliveryDate
            _entryBoxShipFromDeliveryDate = new EntryBoxValidationDatePickerDialog(_sourceWindow, Resx.global_ship_from_delivery_date, _pagePad.DateTimeFormat, _pagePad.InitalDateTime, KeyboardMode.AlfaNumeric, SettingsApp.RegexDateTime, true, _pagePad.DateTimeFormat);
            _entryBoxShipFromDeliveryDate.EntryValidation.Sensitive = true;
            _entryBoxShipFromDeliveryDate.EntryValidation.Text = FrameworkUtils.DateTimeToString(FrameworkUtils.CurrentDateTimeAtomic()).ToString();
            _entryBoxShipFromDeliveryDate.EntryValidation.Validate();
            //Assign Min Date to Validation
            _entryBoxShipFromDeliveryDate.DateTimeMin = FrameworkUtils.CurrentDateTimeAtomic();
            _entryBoxShipFromDeliveryDate.EntryValidation.Changed += _entryBoxShipFromDeliveryDate_ClosePopup;
            _entryBoxShipFromDeliveryDate.ClosePopup += _entryBoxShipFromDeliveryDate_ClosePopup;

            //ShipFromDeliveryID
            _entryBoxShipFromDeliveryID = new EntryBoxValidation(_sourceWindow, Resx.global_ship_from_delivery_id, KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false);
            _entryBoxShipFromDeliveryID.EntryValidation.Changed += delegate { Validate(); };

            //ShipFromWarehouseID
            _entryBoxShipFromWarehouseID = new EntryBoxValidation(_sourceWindow, Resx.global_ship_from_warehouse_id, KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false);
            _entryBoxShipFromWarehouseID.EntryValidation.MaxLength = 50;
            _entryBoxShipFromWarehouseID.EntryValidation.Changed += delegate { Validate(); };

            //ShipFromLocationID
            _entryBoxShipFromLocationID = new EntryBoxValidation(_sourceWindow, Resx.global_ship_from_location_id, KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false);
            _entryBoxShipFromLocationID.EntryValidation.MaxLength = 30;
            _entryBoxShipFromLocationID.EntryValidation.Changed += delegate { Validate(); };

            //HBox hboxDeliveryDate+DeliveryID
            HBox hboxDeliveryDateAndDeliveryID = new HBox(true, 0);
            hboxDeliveryDateAndDeliveryID.PackStart(_entryBoxShipFromDeliveryDate, true, true, 0);
            hboxDeliveryDateAndDeliveryID.PackStart(_entryBoxShipFromDeliveryID, true, true, 0);

            //HBox ZipCode+City+Country
            HBox hboxZipCodeAndCityAndCountry = new HBox(true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(_entryBoxShipFromPostalCode, true, true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(_entryBoxShipFromCity, true, true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(_entryBoxSelectShipFromCountry, true, true, 0);

            //HBox hboxWarehouseID+LocationID
            HBox hboxhboxWarehouseIDAndLocationID = new HBox(true, 0);
            hboxhboxWarehouseIDAndLocationID.PackStart(_entryBoxShipFromWarehouseID, true, true, 0);
            hboxhboxWarehouseIDAndLocationID.PackStart(_entryBoxShipFromLocationID, true, true, 0);

            //Pack VBOX
            VBox vbox = new VBox(false, 2);
            vbox.PackStart(_entryBoxShipFromAddressDetail, false, false, 0);
            vbox.PackStart(_entryBoxShipFromRegion, false, false, 0);
            vbox.PackStart(hboxZipCodeAndCityAndCountry, false, false, 0);
            vbox.PackStart(hboxDeliveryDateAndDeliveryID, false, false, 0);
            vbox.PackStart(hboxhboxWarehouseIDAndLocationID, false, false, 0);
            PackStart(vbox);
        }

        //Override Base Validate
        public override void Validate()
        {
            _validated = (
              _entryBoxShipFromAddressDetail.EntryValidation.Validated &&
              _entryBoxShipFromRegion.EntryValidation.Validated &&
              _entryBoxShipFromPostalCode.EntryValidation.Validated &&
              _entryBoxShipFromCity.EntryValidation.Validated &&
              _entryBoxSelectShipFromCountry.EntryValidation.Validated &&
              _entryBoxShipFromDeliveryDate.EntryValidation.Validated &&
              _entryBoxShipFromDeliveryID.EntryValidation.Validated &&
              _entryBoxShipFromWarehouseID.EntryValidation.Validated &&
              _entryBoxShipFromLocationID.EntryValidation.Validated
            );

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
            _entryBoxShipFromAddressDetail.EntryValidation.Required = pIsRequired;
            //_entryBoxShipFromRegion.EntryValidation.Required = pIsRequired;
            _entryBoxShipFromPostalCode.EntryValidation.Required = pIsRequired;
            _entryBoxShipFromCity.EntryValidation.Required = pIsRequired;
            _entryBoxShipFromDeliveryDate.EntryValidation.Required = pIsRequired;
            _entryBoxSelectShipFromCountry.EntryValidation.Required = pIsRequired;
            //Call Validate
            _entryBoxShipFromAddressDetail.EntryValidation.Validate();
            _entryBoxShipFromRegion.EntryValidation.Validate();
            _entryBoxShipFromPostalCode.EntryValidation.Validate();
            _entryBoxShipFromCity.EntryValidation.Validate();
            _entryBoxShipFromDeliveryDate.EntryValidation.Validate();
            _entryBoxSelectShipFromCountry.EntryValidation.Validate(
                (_entryBoxSelectShipFromCountry.Value != null) ? _entryBoxSelectShipFromCountry.Value.Oid.ToString() : string.Empty
            );
        }

        public void AssignShipFromDefaults()
        {
            //Initials Values
            string initialShipToAddressDetail = FrameworkUtils.GetPreferenceParameter("COMPANY_ADDRESS");
            string initialShipToRegion = FrameworkUtils.GetPreferenceParameter("COMPANY_REGION");
            string initialShipToPostalCode = FrameworkUtils.GetPreferenceParameter("COMPANY_POSTALCODE");
            string initialShipToCity = FrameworkUtils.GetPreferenceParameter("COMPANY_CITY");
            CFG_ConfigurationCountry intialValueConfigurationCountry = SettingsApp.ConfigurationSystemCountry;

            //ShipFrom Address
            _entryBoxShipFromAddressDetail.EntryValidation.Text = initialShipToAddressDetail;
            _entryBoxShipFromAddressDetail.EntryValidation.Validate();
            //ShipFrom Region
            _entryBoxShipFromRegion.EntryValidation.Text = initialShipToRegion;
            _entryBoxShipFromRegion.EntryValidation.Validate();
            //ShipFrom PostalCode
            _entryBoxShipFromPostalCode.EntryValidation.Text = initialShipToPostalCode;
            _entryBoxShipFromPostalCode.EntryValidation.Validate();
            //ShipFrom City
            _entryBoxShipFromCity.EntryValidation.Text = initialShipToCity;
            _entryBoxShipFromCity.EntryValidation.Validate();
            //ShipFrom Country
            _entryBoxSelectShipFromCountry.Value = intialValueConfigurationCountry;
            _entryBoxSelectShipFromCountry.EntryValidation.Validate(_entryBoxSelectShipFromCountry.Value.Oid.ToString());
            //ShipFromDeliveryDate
            //_entryBoxShipFromDeliveryDate.EntryValidation.Text = initialShipFromDeliveryDate;
            //_entryBoxShipFromDeliveryDate.EntryValidation.Validate();
        }

        public void ClearShipFrom()
        {
            //Clear ShipFrom
            if (_entryBoxShipFromDeliveryID.EntryValidation.Text != string.Empty) _entryBoxShipFromDeliveryID.EntryValidation.Text = string.Empty;
            if (_entryBoxShipFromDeliveryDate.EntryValidation.Text != string.Empty) _entryBoxShipFromDeliveryDate.EntryValidation.Text = string.Empty;
            if (_entryBoxShipFromWarehouseID.EntryValidation.Text != string.Empty) _entryBoxShipFromWarehouseID.EntryValidation.Text = string.Empty;
            if (_entryBoxShipFromLocationID.EntryValidation.Text != string.Empty) _entryBoxShipFromLocationID.EntryValidation.Text = string.Empty;
            if (_entryBoxShipFromAddressDetail.EntryValidation.Text != string.Empty) _entryBoxShipFromAddressDetail.EntryValidation.Text = string.Empty;
            if (_entryBoxShipFromCity.EntryValidation.Text != string.Empty) _entryBoxShipFromCity.EntryValidation.Text = string.Empty;
            if (_entryBoxShipFromPostalCode.EntryValidation.Text != string.Empty) _entryBoxShipFromPostalCode.EntryValidation.Text = string.Empty;
            if (_entryBoxShipFromRegion.EntryValidation.Text != string.Empty) _entryBoxShipFromRegion.EntryValidation.Text = string.Empty;
            //Reset to Default Country
            _entryBoxSelectShipFromCountry.Value = _intialValueConfigurationCountry;
        }

        //Equal to ShipTo
        void _entryBoxShipFromDeliveryDate_ClosePopup(object sender, EventArgs e)
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
