using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.financial;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.Classes.Finance;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;
using System;
using logicpos.shared;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogCustomer : BOBaseDialog
    {
        private ERP_Customer _customer = null;
        private uint _totalNumberOfFinanceDocuments = 0;
        //Helper FiscalNumber
        private Entry _entryFiscalNumber;
        private GenericCRUDWidgetXPO _genericCRUDWidgetXPOFiscalNumber;
        //Helper ZipCode
        private Entry _entryZipCode;
        private GenericCRUDWidgetXPO _genericCRUDWidgetXPOZipCode;
        //Helper Country
        private XPOComboBox _xpoComboBoxCountry;
        private CFG_ConfigurationCountry _configurationCountry;
        //Other
        bool _isFinalConsumerEntity = false;

        public DialogCustomer(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(Resx.window_title_edit_customer);
            SetSize(400, 566);
            InitUI();
            InitNotes();
            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                //Init Local Vars
                _configurationCountry = (_dataSourceRow as ERP_Customer).Country;

                if (_dialogMode != DialogMode.Insert)
                {
                    _customer = (_dataSourceRow as ERP_Customer);
                    string sql = string.Format("SELECT COUNT(*) as Count FROM fin_documentfinancemaster WHERE EntityFiscalNumber = '{0}';", _customer.FiscalNumber);
                    var sqlResult = GlobalFramework.SessionXpo.ExecuteScalar(sql);
                    _totalNumberOfFinanceDocuments = Convert.ToUInt16(sqlResult);
                    if (_customer.Oid == SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity) _isFinalConsumerEntity = true;
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxLabel = new BOWidgetBox(Resx.global_record_order, entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(Resx.global_record_code, entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Name
                Entry entryName = new Entry();
                BOWidgetBox boxName = new BOWidgetBox(Resx.global_name, entryName);
                vboxTab1.PackStart(boxName, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxName, _dataSourceRow, "Name", SettingsApp.RegexAlfaNumericExtended, true));

                //FiscalNumber
                _entryFiscalNumber = new Entry();
                BOWidgetBox boxFiscalNumber = new BOWidgetBox(Resx.global_fiscal_number, _entryFiscalNumber);
                vboxTab1.PackStart(boxFiscalNumber, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxFiscalNumber, _dataSourceRow, "FiscalNumber", _configurationCountry.RegExFiscalNumber, false));

                //Discount
                Entry entryDiscount = new Entry();
                BOWidgetBox boxDiscount = new BOWidgetBox(Resx.global_discount, entryDiscount);
                vboxTab1.PackStart(boxDiscount, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDiscount, _dataSourceRow, "Discount", SettingsApp.RegexPercentage, true));

                //PriceType
                XPOComboBox xpoComboBoxPriceType = new XPOComboBox(DataSourceRow.Session, typeof(FIN_ConfigurationPriceType), (DataSourceRow as ERP_Customer).PriceType, "Designation");
                BOWidgetBox boxPriceType = new BOWidgetBox(Resx.global_price_type, xpoComboBoxPriceType);
                vboxTab1.PackStart(boxPriceType, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPriceType, DataSourceRow, "PriceType", SettingsApp.RegexGuid, true));

                //CustomerType
                XPOComboBox xpoComboBoxCustomerType = new XPOComboBox(DataSourceRow.Session, typeof(ERP_CustomerType), (DataSourceRow as ERP_Customer).CustomerType, "Designation");
                BOWidgetBox boxCustomerType = new BOWidgetBox(Resx.global_customer_types, xpoComboBoxCustomerType);
                vboxTab1.PackStart(boxCustomerType, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCustomerType, DataSourceRow, "CustomerType", SettingsApp.RegexGuid, false));
                
                ////DISABLED : DiscountGroup
                //XPOComboBox xpoComboBoxDiscountGroup = new XPOComboBox(DataSourceRow.Session, typeof(ERP_CustomerDiscountGroup), (DataSourceRow as ERP_Customer).DiscountGroup, "Designation");
                //BOWidgetBox boxDiscountGroup = new BOWidgetBox(Resx.global_discount_group, xpoComboBoxDiscountGroup);
                //vboxTab1.PackStart(boxDiscountGroup, false, false, 0);
                //_crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDiscountGroup, DataSourceRow, "DiscountGroup", SettingsApp.RegexGuid, true));

                //Supplier
                CheckButton checkButtonSupplier = new CheckButton(Resx.global_supplier);
                vboxTab1.PackStart(checkButtonSupplier, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonSupplier, _dataSourceRow, "Supplier"));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(Resx.global_record_disabled);
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = SettingsApp.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(Resx.global_record_main_detail));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //Tab2

                VBox vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Address
                Entry entryAddress = new Entry();
                BOWidgetBox boxAddress = new BOWidgetBox(Resx.global_address, entryAddress);
                vboxTab2.PackStart(boxAddress, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxAddress, _dataSourceRow, "Address", SettingsApp.RegexAlfaNumericExtended, false));

                //Locality
                Entry entryLocality = new Entry();
                BOWidgetBox boxLocality = new BOWidgetBox(Resx.global_locality, entryLocality);
                vboxTab2.PackStart(boxLocality, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLocality, _dataSourceRow, "Locality", SettingsApp.RegexAlfaNumericExtended, false));

                //ZipCode
                _entryZipCode = new Entry() { WidthRequest = 100 };;
                BOWidgetBox boxZipCode = new BOWidgetBox(Resx.global_zipcode, _entryZipCode);
                vboxTab2.PackStart(boxZipCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxZipCode, _dataSourceRow, "ZipCode", _configurationCountry.RegExZipCode, false));

                //City
                Entry entryCity = new Entry();
                BOWidgetBox boxCity = new BOWidgetBox(Resx.global_city, entryCity);
                vboxTab2.PackStart(boxCity, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCity, _dataSourceRow, "City", SettingsApp.RegexAlfaNumericExtended, false));

                //Hbox ZipCode and City
                HBox hboxZipCodeAndCity = new HBox(false, _boxSpacing);
                hboxZipCodeAndCity.PackStart(boxZipCode, true, true, 0);
                hboxZipCodeAndCity.PackStart(boxCity, true, true, 0);
                vboxTab2.PackStart(hboxZipCodeAndCity, false, false, 0);

                //CountrySortProperty
                SortProperty[] sortPropertyCountry = new SortProperty[1];
                sortPropertyCountry[0] = new SortProperty("Designation", SortingDirection.Ascending);
                //Country
                _xpoComboBoxCountry = new XPOComboBox(DataSourceRow.Session, typeof(CFG_ConfigurationCountry), (DataSourceRow as ERP_Customer).Country, "Designation", null, sortPropertyCountry);
                BOWidgetBox boxCountry = new BOWidgetBox(Resx.global_country, _xpoComboBoxCountry);
                vboxTab2.PackStart(boxCountry, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCountry, DataSourceRow, "Country", SettingsApp.RegexGuid, true));

                //Phone
                Entry entryPhone = new Entry();
                BOWidgetBox boxPhone = new BOWidgetBox(Resx.global_phone, entryPhone);
                vboxTab2.PackStart(boxPhone, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPhone, _dataSourceRow, "Phone", SettingsApp.RegexAlfaNumericExtended, false));

                //MobilePhone
                Entry entryMobilePhone = new Entry();
                BOWidgetBox boxMobilePhone = new BOWidgetBox(Resx.global_mobile_phone, entryMobilePhone);
                vboxTab2.PackStart(boxMobilePhone, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxMobilePhone, _dataSourceRow, "MobilePhone", SettingsApp.RegexAlfaNumericExtended, false));

                //Hbox Phone and MobilePhone
                HBox hboxPhoneAndMobilePhone = new HBox(false, _boxSpacing);
                hboxPhoneAndMobilePhone.PackStart(boxPhone, true, true, 0);
                hboxPhoneAndMobilePhone.PackStart(boxMobilePhone, true, true, 0);
                vboxTab2.PackStart(hboxPhoneAndMobilePhone, false, false, 0);

                //Fax
                Entry entryFax = new Entry();
                BOWidgetBox boxFax = new BOWidgetBox(Resx.global_fax, entryFax);
                vboxTab2.PackStart(boxFax, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxFax, _dataSourceRow, "Fax", SettingsApp.RegexAlfaNumericExtended, false));

                //Email
                Entry entryEmail = new Entry();
                BOWidgetBox boxEmail = new BOWidgetBox(Resx.global_email, entryEmail);
                vboxTab2.PackStart(boxEmail, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxEmail, _dataSourceRow, "Email", SettingsApp.RegexEmail, false));

                //Hbox Fax and Email
                HBox hboxFaxAndEmail = new HBox(false, _boxSpacing);
                hboxFaxAndEmail.PackStart(boxFax, true, true, 0);
                hboxFaxAndEmail.PackStart(boxEmail, true, true, 0);
                vboxTab2.PackStart(hboxFaxAndEmail, false, false, 0);

                //Append Tab
                _notebook.AppendPage(vboxTab2, new Label(Resx.global_contacts));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //Tab3

                VBox vboxTab3 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //CardNumber
                Entry entryCardNumber = new Entry();
                BOWidgetBox boxCardNumber = new BOWidgetBox(Resx.global_card_number, entryCardNumber);
                vboxTab3.PackStart(boxCardNumber, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCardNumber, _dataSourceRow, "CardNumber", SettingsApp.RegexAlfaNumericExtended, false));

                //CardCredit
                Entry entryCardCredit = new Entry();
                BOWidgetBox boxCardCredit = new BOWidgetBox(Resx.global_card_credit_amount, entryCardCredit);
                vboxTab3.PackStart(boxCardCredit, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCardCredit, _dataSourceRow, "CardCredit", SettingsApp.RegexDecimalGreaterEqualThanZero, false));

                //DateOfBirth
                Entry entryDateOfBirth = new Entry();
                BOWidgetBox boxDateOfBirth = new BOWidgetBox(Resx.global_dob, entryDateOfBirth);
                vboxTab3.PackStart(boxDateOfBirth, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDateOfBirth, _dataSourceRow, "DateOfBirth", SettingsApp.RegexDate, false));

                //Append Tab
                _notebook.AppendPage(vboxTab3, new Label(Resx.global_others));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Disable Components
                entryName.Sensitive = GetEntryNameSensitiveValue();
                entryCardCredit.Sensitive = false;
                _entryFiscalNumber.Sensitive = GetEntryFiscalNumberSensitiveValue();
                _xpoComboBoxCountry.Sensitive = GetEntryFiscalNumberSensitiveValue();

                //Get References to GenericCRUDWidgetXPO
                _genericCRUDWidgetXPOFiscalNumber = (_crudWidgetList.GetFieldWidget("FiscalNumber") as GenericCRUDWidgetXPO);
                _genericCRUDWidgetXPOZipCode = (_crudWidgetList.GetFieldWidget("ZipCode") as GenericCRUDWidgetXPO);
                //Call Validation
                _genericCRUDWidgetXPOFiscalNumber.ValidateField(ValidateFiscalNumberFunc);

                //Update Components
                UpdateCountryRegExComponents();

                //Events
                _entryFiscalNumber.Changed += delegate { ValidateFiscalNumber(); };
                _xpoComboBoxCountry.Changed += delegate { UpdateCountryRegExComponents(); };
            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void UpdateCountryRegExComponents()
        {
            try
            {
                //Assign member reference to ComboBox Value
                _configurationCountry = (_xpoComboBoxCountry.Value as CFG_ConfigurationCountry);
                //Prepare Validation Rule
                _genericCRUDWidgetXPOZipCode.ValidationRule = (_configurationCountry.RegExZipCode != null) ? _configurationCountry.RegExZipCode : SettingsApp.RegexAlfaNumeric;
                _genericCRUDWidgetXPOFiscalNumber.ValidationRule = (_configurationCountry.RegExFiscalNumber != null) ? _configurationCountry.RegExFiscalNumber : SettingsApp.RegexAlfaNumeric;
                //Call Validate ZipCode
                _genericCRUDWidgetXPOZipCode.ValidateField();
                //Call Validate FiscalNumber
                ValidateFiscalNumber();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private void ValidateFiscalNumber()
        {
            _genericCRUDWidgetXPOFiscalNumber.ValidateField(ValidateFiscalNumberFunc);
        }

        private bool ValidateFiscalNumberFunc()
        {
            bool result = false;

            if (_configurationCountry != null) result = FiscalNumber.IsValidFiscalNumber(_entryFiscalNumber.Text, _configurationCountry.Code2);
            
            return result;
        }

        private bool GetEntryNameSensitiveValue()
        {
            bool result = false;

            if (_dialogMode == DialogMode.Insert)
            {
                result = true;
            }
            else if (_isFinalConsumerEntity)
            {
                result = false;
            }
            else if (_dialogMode == DialogMode.Update && _totalNumberOfFinanceDocuments > 0)
            {
                result = (_customer.FiscalNumber != string.Empty);
            }
            else
            {
                result = true;
            }

            return result;
        }

        private bool GetEntryFiscalNumberSensitiveValue()
        {
            bool result = false;

            if (_dialogMode == DialogMode.Insert)
            {
                result = true;
            }
            else
            {
                if (_isFinalConsumerEntity)
                {
                    result = false;
                }
                else if (_dialogMode == DialogMode.Update && _totalNumberOfFinanceDocuments > 0)
                {
                    result = (_customer.FiscalNumber == string.Empty);
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }
    }
}
