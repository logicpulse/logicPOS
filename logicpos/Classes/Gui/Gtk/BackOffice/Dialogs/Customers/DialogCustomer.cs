using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.Classes.Finance;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogCustomer : BOBaseDialog
    {
        private erp_customer _customer = null;
        private uint _totalNumberOfFinanceDocuments = 0;
        //Helper FiscalNumber
        private Entry _entryFiscalNumber;
        private Entry _entryName;
        private Entry _entryAddress;
        private Entry _entryLocality;
        private Entry _entryCity;

        private XPOComboBox _xpoComboBoxCustomerType;

        private GenericCRUDWidgetXPO _genericCRUDWidgetXPOFiscalNumber;
        //Helper ZipCode
        private Entry _entryZipCode;
        private GenericCRUDWidgetXPO _genericCRUDWidgetXPOZipCode;
        //Helper Country
        private XPOComboBox _xpoComboBoxCountry;
        private cfg_configurationcountry _configurationCountry;
        //Other
        bool _isFinalConsumerEntity = false;

        public DialogCustomer(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_edit_customer"));
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
                _configurationCountry = (_dataSourceRow as erp_customer).Country;
                /* When customers already have a document issued to them, edit Fiscal Number field is not allowed */
                if (_dialogMode != DialogMode.Insert)
                {
                    _customer = (_dataSourceRow as erp_customer);

                    /* IN009249 - begin */
                    string customerFiscalNumberCrypto = GlobalFramework.PluginSoftwareVendor.Encrypt(_customer.FiscalNumber);
                    string countSQL = string.Format("EntityFiscalNumber = '{0}'", customerFiscalNumberCrypto);

                    var countResult = GlobalFramework.SessionXpo.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse(countSQL));
                    _totalNumberOfFinanceDocuments = Convert.ToUInt16(countResult);
                    /* IN009249 - end */
                    
                    if (_customer.Oid == SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity) _isFinalConsumerEntity = true;
                }

                //erp_customer customers = null;
                SortingCollection sortCollection = new SortingCollection();
                sortCollection.Add(new SortProperty("Code", DevExpress.Xpo.DB.SortingDirection.Ascending));
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
                ICollection collectionCustomers = GlobalFramework.SessionXpo.GetObjects(GlobalFramework.SessionXpo.GetClassInfo(typeof(erp_customer)), criteria, sortCollection, int.MaxValue, false, true);
                
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxLabel = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", SettingsApp.RegexIntegerGreaterThanZero, true));

                //FiscalNumber
                _entryFiscalNumber = new Entry();
                BOWidgetBox boxFiscalNumber = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_fiscal_number"), _entryFiscalNumber);
                vboxTab1.PackStart(boxFiscalNumber, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxFiscalNumber, _dataSourceRow, "FiscalNumber", _configurationCountry.RegExFiscalNumber, true));/* IN009061 */

                //Name
                _entryName = new Entry();
                BOWidgetBox boxName = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_name"), _entryName);
                vboxTab1.PackStart(boxName, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxName, _dataSourceRow, "Name", SettingsApp.RegexAlfaNumericPlus, true));/* IN009253 */
                               
                //Discount
                Entry entryDiscount = new Entry();
                BOWidgetBox boxDiscount = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_discount"), entryDiscount);
                vboxTab1.PackStart(boxDiscount, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDiscount, _dataSourceRow, "Discount", SettingsApp.RegexPercentage, true));

                //PriceType IN:009261
                XPOComboBox xpoComboBoxPriceType = new XPOComboBox(DataSourceRow.Session, typeof(fin_configurationpricetype), (DataSourceRow as erp_customer).PriceType, "Designation", null, null, 1);
                BOWidgetBox boxPriceType = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_price_type"), xpoComboBoxPriceType);
                vboxTab1.PackStart(boxPriceType, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPriceType, DataSourceRow, "PriceType", SettingsApp.RegexGuid, true));

                //CustomerType IN009261
                SortProperty[] sortPropertyCostumerType = new SortProperty[1];
                sortPropertyCostumerType[0] = new SortProperty("Designation", SortingDirection.Descending);

                _xpoComboBoxCustomerType = new XPOComboBox(DataSourceRow.Session, typeof(erp_customertype), (DataSourceRow as erp_customer).CustomerType, "Designation", null, sortPropertyCostumerType, 1);
                BOWidgetBox boxCustomerType = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_customer_types"), _xpoComboBoxCustomerType);
                vboxTab1.PackStart(boxCustomerType, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCustomerType, DataSourceRow, "CustomerType", SettingsApp.RegexGuid, true));
                
                ////DISABLED : DiscountGroup
                //XPOComboBox xpoComboBoxDiscountGroup = new XPOComboBox(DataSourceRow.Session, typeof(erp_customerdiscountgroup), (DataSourceRow as erp_customer).DiscountGroup, "Designation");
                //BOWidgetBox boxDiscountGroup = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_discount_group, xpoComboBoxDiscountGroup);
                //vboxTab1.PackStart(boxDiscountGroup, false, false, 0);
                //_crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDiscountGroup, DataSourceRow, "DiscountGroup", SettingsApp.RegexGuid, true));

                //Supplier
                CheckButton checkButtonSupplier = new CheckButton(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_supplier"));
                vboxTab1.PackStart(checkButtonSupplier, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonSupplier, _dataSourceRow, "Supplier"));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = SettingsApp.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //Tab2

                VBox vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Address
                _entryAddress = new Entry();
                BOWidgetBox boxAddress = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_address"), _entryAddress);
                vboxTab2.PackStart(boxAddress, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxAddress, _dataSourceRow, "Address", SettingsApp.RegexAlfaNumericPlus, false));/* IN009253 */

                //Locality
                _entryLocality = new Entry();
                BOWidgetBox boxLocality = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_locality"), _entryLocality);
                vboxTab2.PackStart(boxLocality, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLocality, _dataSourceRow, "Locality", SettingsApp.RegexAlfaNumericPlus, false));/* IN009253 */

                //ZipCode
                _entryZipCode = new Entry() { WidthRequest = 100 }; ;
                BOWidgetBox boxZipCode = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_zipcode"), _entryZipCode);
                vboxTab2.PackStart(boxZipCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxZipCode, _dataSourceRow, "ZipCode", _configurationCountry.RegExZipCode, false));

                //City
                _entryCity = new Entry();
                BOWidgetBox boxCity = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_city"), _entryCity);
                vboxTab2.PackStart(boxCity, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCity, _dataSourceRow, "City", SettingsApp.RegexAlfaNumericPlus, false)); /* IN009176, IN009253 */

                //Hbox ZipCode and City
                HBox hboxZipCodeAndCity = new HBox(false, _boxSpacing);
                hboxZipCodeAndCity.PackStart(boxZipCode, true, true, 0);
                hboxZipCodeAndCity.PackStart(boxCity, true, true, 0);
                vboxTab2.PackStart(hboxZipCodeAndCity, false, false, 0);

                //CountrySortProperty
                SortProperty[] sortPropertyCountry = new SortProperty[1];
                sortPropertyCountry[0] = new SortProperty("Designation", SortingDirection.Ascending);
                //Country
                _xpoComboBoxCountry = new XPOComboBox(DataSourceRow.Session, typeof(cfg_configurationcountry), (DataSourceRow as erp_customer).Country, "Designation", null, sortPropertyCountry);
                BOWidgetBox boxCountry = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_country"), _xpoComboBoxCountry);
                vboxTab2.PackStart(boxCountry, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCountry, DataSourceRow, "Country", SettingsApp.RegexGuid, true));

                //Phone
                Entry entryPhone = new Entry();
                BOWidgetBox boxPhone = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_phone"), entryPhone);
                vboxTab2.PackStart(boxPhone, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPhone, _dataSourceRow, "Phone", SettingsApp.RegexAlfaNumericExtended, false));

                //MobilePhone
                Entry entryMobilePhone = new Entry();
                BOWidgetBox boxMobilePhone = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_mobile_phone"), entryMobilePhone);
                vboxTab2.PackStart(boxMobilePhone, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxMobilePhone, _dataSourceRow, "MobilePhone", SettingsApp.RegexAlfaNumericExtended, false));

                //Hbox Phone and MobilePhone
                HBox hboxPhoneAndMobilePhone = new HBox(false, _boxSpacing);
                hboxPhoneAndMobilePhone.PackStart(boxPhone, true, true, 0);
                hboxPhoneAndMobilePhone.PackStart(boxMobilePhone, true, true, 0);
                vboxTab2.PackStart(hboxPhoneAndMobilePhone, false, false, 0);

                //Fax
                Entry entryFax = new Entry();
                BOWidgetBox boxFax = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_fax"), entryFax);
                vboxTab2.PackStart(boxFax, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxFax, _dataSourceRow, "Fax", SettingsApp.RegexAlfaNumericExtended, false));

                //Email
                Entry entryEmail = new Entry();
                BOWidgetBox boxEmail = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_email"), entryEmail);
                vboxTab2.PackStart(boxEmail, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxEmail, _dataSourceRow, "Email", SettingsApp.RegexEmail, false));

                //Hbox Fax and Email
                HBox hboxFaxAndEmail = new HBox(false, _boxSpacing);
                hboxFaxAndEmail.PackStart(boxFax, true, true, 0);
                hboxFaxAndEmail.PackStart(boxEmail, true, true, 0);
                vboxTab2.PackStart(hboxFaxAndEmail, false, false, 0);

                //Append Tab
                _notebook.AppendPage(vboxTab2, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_contacts")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //Tab3

                VBox vboxTab3 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //CardNumber
                Entry entryCardNumber = new Entry();
                BOWidgetBox boxCardNumber = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_card_number"), entryCardNumber);
                vboxTab3.PackStart(boxCardNumber, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCardNumber, _dataSourceRow, "CardNumber", SettingsApp.RegexAlfaNumericExtended, false));

                //CardCredit
                Entry entryCardCredit = new Entry();
                BOWidgetBox boxCardCredit = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_card_credit_amount"), entryCardCredit);
                vboxTab3.PackStart(boxCardCredit, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCardCredit, _dataSourceRow, "CardCredit", SettingsApp.RegexDecimalGreaterEqualThanZero, false));

                //DateOfBirth
                Entry entryDateOfBirth = new Entry();
                BOWidgetBox boxDateOfBirth = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_dob"), entryDateOfBirth);
                vboxTab3.PackStart(boxDateOfBirth, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDateOfBirth, _dataSourceRow, "DateOfBirth", SettingsApp.RegexDate, false));

                //Append Tab
                _notebook.AppendPage(vboxTab3, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_others")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Disable Components
                _entryName.Sensitive = GetEntryNameSensitiveValue();
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
                //string teste = (DataSourceRow as erp_customer).CustomerType.Designation;
                //Events
                _entryFiscalNumber.Changed += delegate { ValidateFiscalNumber(); };
                _xpoComboBoxCountry.Changed += delegate { UpdateCountryRegExComponents(); };
                //IN009260 Inserir Cliente permite código já inserido 
                entryCode.FocusOutEvent += delegate
                {
                    foreach (erp_customer item in collectionCustomers)
                    {
                        if (entryCode.Text == item.Code.ToString())
                        {
                            Utils.ShowMessageTouch(GlobalApp.WindowBackOffice, DialogFlags.DestroyWithParent | DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_validation_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_code_number_exists"));
                            entryCode.Text = "";
                        }
                    }
                };

                entryOrd.FocusOutEvent += delegate
                {
                    foreach (erp_customer item in collectionCustomers)
                    {
                        if (entryOrd.Text == item.Code.ToString())
                        {
                            Utils.ShowMessageTouch(GlobalApp.WindowBackOffice, DialogFlags.DestroyWithParent | DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_validation_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_code_number_exists"));
                            entryOrd.Text = "";
                        }
                    }
                };
            }
            catch (System.Exception ex)
            {
                _log.Error("void DialogCustomer.InitUI(): " + ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void UpdateCountryRegExComponents()
        {
            try
            {
                //Assign member reference to ComboBox Value
                _configurationCountry = (_xpoComboBoxCountry.Value as cfg_configurationcountry);
                //Prepare Validation Rule
                _genericCRUDWidgetXPOZipCode.ValidationRule = (_configurationCountry.RegExZipCode != null) ? _configurationCountry.RegExZipCode : SettingsApp.RegexAlfaNumeric;
                _genericCRUDWidgetXPOFiscalNumber.ValidationRule = (_configurationCountry.RegExFiscalNumber != null) ? _configurationCountry.RegExFiscalNumber : SettingsApp.RegexAlfaNumeric;
                //Call Validate ZipCode
                _genericCRUDWidgetXPOZipCode.ValidateField();
                //Call Validate FiscalNumber
                /* IN009061 - this was calling validation when don't need */
                //ValidateFiscalNumber();
            }
            catch (Exception ex)
            {
                _log.Error("void DialogCustomer.UpdateCountryRegExComponents(): " + ex.Message, ex);
            }
        }

        private void ValidateFiscalNumber()
        {
            _genericCRUDWidgetXPOFiscalNumber.ValidateField(ValidateFiscalNumberFunc);
        }

        private bool ValidateFiscalNumberFunc()
        {
            bool result = false;
            //IN:009268 BackOffice - NIF auto-complete Clean fields on insert different VAT

            if (_configurationCountry != null) result = FiscalNumber.IsValidFiscalNumber(_entryFiscalNumber.Text, _configurationCountry.Code2);
            /* IN009061 - used to force field to be shown in error or not */
            _genericCRUDWidgetXPOFiscalNumber.Validated = result;

            /* IN009061 - avoid duplicated FiscalNumber */
            if (DialogMode.Insert.Equals(_dialogMode) && result)
            {
                FiscalNumberAlreadyExists(_entryFiscalNumber.Text);
            }
            else if (DialogMode.Update.Equals(_dialogMode) && result)
            {
                if (!_entryFiscalNumber.Text.Equals(_customer.FiscalNumber))
                {
                    FiscalNumberAlreadyExists(_entryFiscalNumber.Text);
                }
            }

            return result;
        }

        /// <summary>
        /// Checks for Fiscal Number duplicates.
        /// See IN009061 for further information.
        /// </summary>
        /// <param name="fiscalNumber"></param>
        /// <returns></returns>
        private bool FiscalNumberAlreadyExists(string fiscalNumber)
        {
            /* Initially, settled to false, because database will restrict any attempt to insert duplicated value in case of error */
            bool fiscalNumberAlreadyExists = false;
            try
            {
                string encryptedFiscalNumber = GlobalFramework.PluginSoftwareVendor.Encrypt(fiscalNumber);

                string sqlForFiscalNumberCount = string.Format("SELECT COUNT(*) as Count FROM erp_customer WHERE FiscalNumber = '{0}';", encryptedFiscalNumber);
                var sqlResult = GlobalFramework.SessionXpo.ExecuteScalar(sqlForFiscalNumberCount);
                int count = Convert.ToUInt16(sqlResult);

                if (count > 0)
                {
                    fiscalNumberAlreadyExists = true;
                    _genericCRUDWidgetXPOFiscalNumber.Validated = false;
                    Utils.ShowMessageTouch(GlobalApp.WindowBackOffice, DialogFlags.DestroyWithParent | DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_validation_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_customer_fiscal_number_exists"));

                }
                //IN:009268 BackOffice - NIF auto-complete 
                else if (Utils.UseVatAutocomplete())
                {

                    string cod_FiscalNumber = string.Format("{0}{1}", cfg_configurationpreferenceparameter.GetCountryCode2, fiscalNumber);
                   
                    if (EuropeanVatInformation.Get(cod_FiscalNumber) != null)
                    {
                        var address = (EuropeanVatInformation.Get(cod_FiscalNumber).Address.Split('\n'));
                        string zip = address[2].Substring(0, address[2].IndexOf(' '));
                        string city = address[2].Substring(address[2].IndexOf(' ') + 1);
                        _entryAddress.Text = address[0];
                        _entryCity.Text = address[1];
                        _entryZipCode.Text = zip;
                        _entryLocality.Text = city;
                        _entryName.Text = EuropeanVatInformation.Get(cod_FiscalNumber).Name;
                    }
                    else
                    {
                        _entryAddress.Text = "";
                        _entryCity.Text = "";
                        _entryZipCode.Text = "";
                        _entryLocality.Text = "";
                        _entryName.Text = "";
                    }
                }
                //IN:009268 ENDS
            }
            catch (Exception ex)
            {
                _log.Error("bool DialogCustomer.FiscalNumberAlreadyExists(): " + ex.Message, ex);
            }

            return fiscalNumberAlreadyExists;
        }

        private bool EntryCodeAlreadyExists(int code)
        {
            /* Initially, settled to false, because database will restrict any attempt to insert duplicated value in case of error */
            bool entryCodeAlreadyExists = false;
            try
            {
                string sqlForFiscalNumberCount = string.Format("SELECT COUNT(*) as Count FROM erp_customer WHERE Code = '{0}';", code.ToString());
                var sqlResult = GlobalFramework.SessionXpo.ExecuteScalar(sqlForFiscalNumberCount);
                int count = Convert.ToUInt16(sqlResult);

                if (count > 0)
                {
                    entryCodeAlreadyExists = true;
                    _genericCRUDWidgetXPOFiscalNumber.Validated = false;
                    Utils.ShowMessageTouch(GlobalApp.WindowBackOffice, DialogFlags.DestroyWithParent | DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_validation_error"), "Já existe o Codigo");
                }
            }
            catch (Exception ex)
            {
                _log.Error("bool DialogCustomer.EntryCodeAlreadyExists(): " + ex.Message, ex);
            }

            return entryCodeAlreadyExists;
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
