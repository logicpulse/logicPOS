using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.DataLayer.Xpo;
using System;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using logicpos.datalayer.Xpo;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    internal class DocumentFinanceDialogPage4 : PagePadPage
    {
        private readonly Session _session;
        private readonly DocumentFinanceDialogPagePad _pagePad;
        private readonly cfg_configurationcountry _intialValueConfigurationCountry;

        public EntryBoxValidationDatePickerDialog EntryBoxShipToDeliveryDate { get; }

        public EntryBoxValidation EntryBoxShipToDeliveryID { get; }

        public EntryBoxValidation EntryBoxShipToWarehouseID { get; }

        public EntryBoxValidation EntryBoxShipToLocationID { get; }

        public EntryBoxValidation EntryBoxShipToAddressDetail { get; }

        public EntryBoxValidation EntryBoxShipToRegion { get; }

        public EntryBoxValidation EntryBoxShipToPostalCode { get; }

        public EntryBoxValidation EntryBoxShipToCity { get; }

        public XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry> EntryBoxSelectShipToCountry { get; }

        //Constructor
        public DocumentFinanceDialogPage4(Window pSourceWindow, string pPageName) 
            : this(pSourceWindow, pPageName, "", null, true) { }
        public DocumentFinanceDialogPage4(Window pSourceWindow, string pPageName, Widget pWidget) 
            : this(pSourceWindow, pPageName, "", pWidget, true) { }
        public DocumentFinanceDialogPage4(Window pSourceWindow, string pPageName, string pPageIcon, Widget pWidget, bool pEnabled = true)
            : base(pSourceWindow, pPageName, pPageIcon, pWidget, pEnabled)
        {
            //Init private vars
            _pagePad = (_sourceWindow as PosDocumentFinanceDialog).PagePad;
            _session = (_pagePad as DocumentFinanceDialogPagePad).Session;

            //Initial Values
            _intialValueConfigurationCountry = XPOSettings.ConfigurationSystemCountry;

            //ShipTo Address
            EntryBoxShipToAddressDetail = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_address"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericPlus, true);/* IN009253 */
            EntryBoxShipToAddressDetail.EntryValidation.Changed += delegate { Validate(); };

            //ShipTo Region
            EntryBoxShipToRegion = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_region"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericPlus, false);/* IN009253 */
            EntryBoxShipToRegion.EntryValidation.Changed += delegate { Validate(); };

            //ShipTo PostalCode
            EntryBoxShipToPostalCode = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_zipcode"), KeyboardMode.Alfa, XPOSettings.ConfigurationSystemCountry.RegExZipCode, true);
            EntryBoxShipToPostalCode.EntryValidation.Changed += delegate { Validate(); };

            //ShipTo City
            EntryBoxShipToCity = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_city"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericPlus, true);/* IN009253 */
            EntryBoxShipToCity.EntryValidation.Changed += delegate { Validate(); };

            //ShipTo Country
            CriteriaOperator criteriaOperatorCustomerCountry = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            EntryBoxSelectShipToCountry = new XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry>(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_country"), "Designation", "Oid", _intialValueConfigurationCountry, criteriaOperatorCustomerCountry, LogicPOS.Utility.RegexUtils.RegexGuid, true);
            EntryBoxSelectShipToCountry.EntryValidation.Validate(EntryBoxSelectShipToCountry.Value.Oid.ToString());
            EntryBoxSelectShipToCountry.EntryValidation.IsEditable = false;
            EntryBoxSelectShipToCountry.EntryValidation.Changed += delegate { Validate(); };
            EntryBoxSelectShipToCountry.ClosePopup += delegate
            {
                //Require to Update RegExZipCode
                EntryBoxShipToPostalCode.EntryValidation.Rule = EntryBoxSelectShipToCountry.Value.RegExZipCode;
                EntryBoxShipToPostalCode.EntryValidation.Validate();
            };

            //ShipToDeliveryDate
            EntryBoxShipToDeliveryDate = new EntryBoxValidationDatePickerDialog(
                _sourceWindow, 
                CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ship_to_delivery_date"), 
                _pagePad.DateTimeFormat, 
                _pagePad.InitalDateTime, 
                KeyboardMode.AlfaNumeric, 
                LogicPOS.Utility.RegexUtils.RegexDateTime, 
                true, 
                _pagePad.DateTimeFormat);

            EntryBoxShipToDeliveryDate.EntryValidation.Sensitive = true;
            //_entryBoxShipToDeliveryDate.EntryValidation.Text = _pagePad.InitalDateTime.ToString(_pagePad.DateTimeFormat);
            EntryBoxShipToDeliveryDate.EntryValidation.Validate();
            //Assign Min Date to Validation
            EntryBoxShipToDeliveryDate.DateTimeMin = XPOHelper.CurrentDateTimeAtomic();
            EntryBoxShipToDeliveryDate.EntryValidation.Changed += _entryBoxShipToDeliveryDate_Changed;
            EntryBoxShipToDeliveryDate.ClosePopup += _entryBoxShipToDeliveryDate_Changed;

            //ShipToDeliveryID
            EntryBoxShipToDeliveryID = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ship_to_delivery_id"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false);
            EntryBoxShipToDeliveryID.EntryValidation.Changed += delegate { Validate(); };

            //ShipToWarehouseID
            EntryBoxShipToWarehouseID = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ship_to_warehouse_id"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false);
            EntryBoxShipToWarehouseID.EntryValidation.MaxLength = 50;
            EntryBoxShipToWarehouseID.EntryValidation.Changed += delegate { Validate(); };

            //ShipToLocationID
            EntryBoxShipToLocationID = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ship_to_location_id"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false);
            EntryBoxShipToLocationID.EntryValidation.MaxLength = 30;
            EntryBoxShipToLocationID.EntryValidation.Changed += delegate { Validate(); };

            //HBox ZipCode+City+Country
            HBox hboxZipCodeAndCityAndCountry = new HBox(true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(EntryBoxShipToPostalCode, true, true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(EntryBoxShipToCity, true, true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(EntryBoxSelectShipToCountry, true, true, 0);

            //HBox hboxDeliveryDate+DeliveryID
            HBox hboxDeliveryDateAndDeliveryID = new HBox(true, 0);
            hboxDeliveryDateAndDeliveryID.PackStart(EntryBoxShipToDeliveryDate, true, true, 0);
            hboxDeliveryDateAndDeliveryID.PackStart(EntryBoxShipToDeliveryID, true, true, 0);

            //HBox hboxWarehouseID+LocationID
            HBox hboxhboxWarehouseIDAndLocationID = new HBox(true, 0);
            hboxhboxWarehouseIDAndLocationID.PackStart(EntryBoxShipToWarehouseID, true, true, 0);
            hboxhboxWarehouseIDAndLocationID.PackStart(EntryBoxShipToLocationID, true, true, 0);

            //Pack VBOX
            VBox vbox = new VBox(false, 2);

            vbox.PackStart(EntryBoxShipToAddressDetail, false, false, 0);
            vbox.PackStart(EntryBoxShipToRegion, false, false, 0);
            vbox.PackStart(hboxZipCodeAndCityAndCountry, false, false, 0);
            vbox.PackStart(hboxDeliveryDateAndDeliveryID, false, false, 0);
            vbox.PackStart(hboxhboxWarehouseIDAndLocationID, false, false, 0);
            PackStart(vbox);
        }

        //Override Base Validate
        public override void Validate()
        {
            _validated = (
              EntryBoxShipToAddressDetail.EntryValidation.Validated &&
              EntryBoxShipToRegion.EntryValidation.Validated &&
              EntryBoxShipToPostalCode.EntryValidation.Validated &&
              EntryBoxShipToCity.EntryValidation.Validated &&
              EntryBoxSelectShipToCountry.EntryValidation.Validated &&
              EntryBoxShipToDeliveryDate.EntryValidation.Validated &&
              EntryBoxShipToDeliveryID.EntryValidation.Validated &&
              EntryBoxShipToWarehouseID.EntryValidation.Validated &&
              EntryBoxShipToLocationID.EntryValidation.Validated
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
            EntryBoxShipToAddressDetail.EntryValidation.Required = pIsRequired;
            //_entryBoxShipToRegion.EntryValidation.Required = pIsRequired;
            EntryBoxShipToPostalCode.EntryValidation.Required = pIsRequired;
            EntryBoxShipToCity.EntryValidation.Required = pIsRequired;
            EntryBoxShipToDeliveryDate.EntryValidation.Required = pIsRequired;
            EntryBoxSelectShipToCountry.EntryValidation.Required = pIsRequired;
            //Call Validate
            EntryBoxShipToAddressDetail.EntryValidation.Validate();
            EntryBoxShipToRegion.EntryValidation.Validate();
            EntryBoxShipToPostalCode.EntryValidation.Validate();
            EntryBoxShipToCity.EntryValidation.Validate();
            EntryBoxShipToDeliveryDate.EntryValidation.Validate();
            EntryBoxSelectShipToCountry.EntryValidation.Validate(
                (EntryBoxSelectShipToCountry.Value != null) ? EntryBoxSelectShipToCountry.Value.Oid.ToString() : string.Empty
            );
        }

        public void ClearShipTo()
        {
            //Clear ShipTo
            if (EntryBoxShipToDeliveryID.EntryValidation.Text != string.Empty) EntryBoxShipToDeliveryID.EntryValidation.Text = string.Empty;
            if (EntryBoxShipToDeliveryDate.EntryValidation.Text != string.Empty) EntryBoxShipToDeliveryDate.EntryValidation.Text = string.Empty;
            if (EntryBoxShipToWarehouseID.EntryValidation.Text != string.Empty) EntryBoxShipToWarehouseID.EntryValidation.Text = string.Empty;
            if (EntryBoxShipToLocationID.EntryValidation.Text != string.Empty) EntryBoxShipToLocationID.EntryValidation.Text = string.Empty;
            if (EntryBoxShipToAddressDetail.EntryValidation.Text != string.Empty) EntryBoxShipToAddressDetail.EntryValidation.Text = string.Empty;
            if (EntryBoxShipToCity.EntryValidation.Text != string.Empty) EntryBoxShipToCity.EntryValidation.Text = string.Empty;
            if (EntryBoxShipToPostalCode.EntryValidation.Text != string.Empty) EntryBoxShipToPostalCode.EntryValidation.Text = string.Empty;
            if (EntryBoxShipToRegion.EntryValidation.Text != string.Empty) EntryBoxShipToRegion.EntryValidation.Text = string.Empty;
            //Reset to Default Country
            EntryBoxSelectShipToCountry.Value = _intialValueConfigurationCountry;
        }

        //Equal to ShipFrom
        private void _entryBoxShipToDeliveryDate_Changed(object sender, EventArgs e)
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
