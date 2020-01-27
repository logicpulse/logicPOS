using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial;
using logicpos.financial.library.Classes.Finance;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;
using logicpos.datalayer.Enums;
using logicpos.shared.Classes.Finance;
using System;
using logicpos.Classes.Enums.Keyboard;
using System.Collections;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    class DocumentFinanceDialogPage2 : PagePadPage
    {
        private Session _session;
        private DocumentFinanceDialogPagePad _pagePad;
        private PosDocumentFinanceDialog _posDocumentFinanceDialog;
        private cfg_configurationcountry _intialValueConfigurationCountry;
        
        //Used to store Customer PriceType before change it by user, used to compare PriceType after select other (old and new)
        PriceType _currentCustomerPriceType = PriceType.Price1;
        //Articles TreeView Reference
        private TreeViewDocumentFinanceArticle _treeViewArticles;
        //UI Object References from other pages
        //Required PagePage1 to be public to be assigned in PosDocumentFinanceDialog InitPages
        private DocumentFinanceDialogPage1 _pagePad1;
        public DocumentFinanceDialogPage1 PagePad1
        {
            set { _pagePad1 = value; }
        }
        //Required PagePage3 to be public to be assigned in PosDocumentFinanceDialog InitPages
        private DocumentFinanceDialogPage3 _pagePad3;
        public DocumentFinanceDialogPage3 PagePad3
        {
            set { _pagePad3 = value; }
        }
        //Required PagePage4 to be public to be assigned in PosDocumentFinanceDialog InitPages
        private DocumentFinanceDialogPage4 _pagePad4;
        public DocumentFinanceDialogPage4 PagePad4
        {
            set { _pagePad4 = value; }
        }
        //Required PagePage5 to be public to be assigned in PosDocumentFinanceDialog InitPages
        private DocumentFinanceDialogPage5 _pagePad5;
        public DocumentFinanceDialogPage5 PagePad5
        {
            set { _pagePad5 = value; }
        }
        //Required to be accessed from other Pages, usefull to block. Change events when we change Entry.Text from code, and prevent to recursivly call Change Events
        private bool _enableGetCustomerDetails = true;

        private erp_customer _erpCustomer;
        public erp_customer erp_customer
        {
            get { return _erpCustomer; }
            set { _erpCustomer = value; }
        }
        private TreeViewCustomer _treeViewCustomer;
        public TreeViewCustomer treeViewCustomer
        {
            get { return _treeViewCustomer; }
            set { _treeViewCustomer = value; }
        }

        public bool EnableGetCustomerDetails
        {
            get { return _enableGetCustomerDetails; }
            set { _enableGetCustomerDetails = value; }
        }
        //UI
        private XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _entryBoxSelectCustomerName;
        public XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> EntryBoxSelectCustomerName
        {
            get { return _entryBoxSelectCustomerName; }
        }

        private EntryBoxValidation _entryBoxCustomerAddress;
        public EntryBoxValidation EntryBoxCustomerAddress
        {
            get { return _entryBoxCustomerAddress; }
        }

        private EntryBoxValidation _entryBoxCustomerLocality;
        public EntryBoxValidation EntryBoxCustomerLocality
        {
            get { return _entryBoxCustomerLocality; }
        }

        private EntryBoxValidation _entryBoxCustomerZipCode;
        public EntryBoxValidation EntryBoxCustomerZipCode
        {
            get { return _entryBoxCustomerZipCode; }
        }

        private EntryBoxValidation _entryBoxCustomerCity;
        public EntryBoxValidation EntryBoxCustomerCity
        {
            get { return _entryBoxCustomerCity; }
        }

        private XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry> _entryBoxSelectCustomerCountry;
        public XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry> EntryBoxSelectCustomerCountry
        {
            get { return _entryBoxSelectCustomerCountry; }
        }

        private XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _entryBoxSelectCustomerFiscalNumber;
        public XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> EntryBoxSelectCustomerFiscalNumber
        {
            get { return _entryBoxSelectCustomerFiscalNumber; }
        }

        private XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _entryBoxSelectCustomerCardNumber;
        public XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> EntryBoxSelectCustomerCardNumber
        {
            get { return _entryBoxSelectCustomerCardNumber; }
        }

        private EntryBoxValidation _entryBoxCustomerDiscount;
        public EntryBoxValidation EntryBoxCustomerDiscount
        {
            get { return _entryBoxCustomerDiscount; }
        }

        private EntryBoxValidation _entryBoxCustomerPhone;
        public EntryBoxValidation EntryBoxCustomerPhone
        {
            get { return _entryBoxCustomerPhone; }
        }

        private EntryBoxValidation _entryBoxCustomerEmail;
        public EntryBoxValidation EntryBoxCustomerEmail
        {
            get { return _entryBoxCustomerEmail; }
        }

        private EntryBoxValidation _entryBoxCustomerNotes;
        public EntryBoxValidation EntryBoxCustomerNotes
        {
            get { return _entryBoxCustomerNotes; }
        }

        //Constructor
        public DocumentFinanceDialogPage2(Window pSourceWindow, String pPageName)
            : this(pSourceWindow, pPageName, "", null, true) { }
        public DocumentFinanceDialogPage2(Window pSourceWindow, String pPageName, Widget pWidget)
            : this(pSourceWindow, pPageName, "", pWidget, true) { }
        public DocumentFinanceDialogPage2(Window pSourceWindow, String pPageName, String pPageIcon, Widget pWidget, bool pEnabled = true)
            : base(pSourceWindow, pPageName, pPageIcon, pWidget, pEnabled)
        {
            //Init private vars
            _pagePad = (_sourceWindow as PosDocumentFinanceDialog).PagePad;
            _session = (_pagePad as DocumentFinanceDialogPagePad).Session;
            _posDocumentFinanceDialog = (_sourceWindow as PosDocumentFinanceDialog);

            //Initials Values
            _intialValueConfigurationCountry = SettingsApp.ConfigurationSystemCountry;            
            //Customer Name
            CriteriaOperator criteriaOperatorCustomerName = null;
			//TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 
            _treeViewCustomer = new TreeViewCustomer(
              pSourceWindow,
              null,//DefaultValue 
              null,//DialogType
              null         
            );

         
            erp_customer customer = null;
            SortingCollection sortCollection = new SortingCollection();
            sortCollection.Add(new SortProperty("Name", DevExpress.Xpo.DB.SortingDirection.Ascending));
            CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
            ICollection collectionCustomers = GlobalFramework.SessionXpo.GetObjects(GlobalFramework.SessionXpo.GetClassInfo(typeof(erp_customer)), criteria, sortCollection, int.MaxValue, false, true);

            foreach (erp_customer item in collectionCustomers)
            {
                customer = item;
            }
            customer.Name = "";
            customer.FiscalNumber = "";
            _entryBoxSelectCustomerName = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_customer"), "Name", "Name", customer, criteriaOperatorCustomerName, KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericPlus, true);/* IN009253 */
            
            _entryBoxSelectCustomerName.ClosePopup += delegate
            {
                GetCustomerDetails("Oid", _entryBoxSelectCustomerName.Value.Oid.ToString());
                Validate();
            };
			//TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 
            _entryBoxSelectCustomerName.EntryValidation.Changed += delegate
            {
                _entryBoxSelectCustomerName_Changed(_entryBoxSelectCustomerName.EntryValidation, null);
                if(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text == string.Empty)
                {
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated = false;
                    _pagePad.ButtonNext.Sensitive = false;
                }
            };
            _entryBoxSelectCustomerName.EntryValidation.IsEditable = true;

            //Customer Address
            _entryBoxCustomerAddress = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_address"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericPlus, false);/* IN009253 */
            _entryBoxCustomerAddress.EntryValidation.Changed += delegate { Validate(); };

            //Customer Locality
            _entryBoxCustomerLocality = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_locality"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericPlus, false);/* IN009253 */
            _entryBoxCustomerLocality.EntryValidation.Changed += delegate { Validate(); };

            //Customer ZipCode
            _entryBoxCustomerZipCode = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_zipcode"), KeyboardMode.Alfa, SettingsApp.ConfigurationSystemCountry.RegExZipCode, false);
            _entryBoxCustomerZipCode.EntryValidation.Changed += delegate { Validate(); };

            //Customer City
            _entryBoxCustomerCity = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_city"), KeyboardMode.AlfaNumeric, SettingsApp.RegexAlfaNumericPlus, false);/* IN009253 */
            _entryBoxCustomerCity.EntryValidation.Changed += delegate { Validate(); };

            //Customer Country
            CriteriaOperator criteriaOperatorCustomerCountry = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1) AND (RegExFiscalNumber IS NOT NULL AND RegExZipCode IS NOT NULL)");
            _entryBoxSelectCustomerCountry = new XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry>(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_country"), "Designation", "Oid", _intialValueConfigurationCountry, criteriaOperatorCustomerCountry, SettingsApp.RegexGuid, true);
            _entryBoxSelectCustomerCountry.EntryValidation.IsEditable = true;
            _entryBoxSelectCustomerCountry.EntryValidation.Validate(_entryBoxSelectCustomerCountry.Value.Oid.ToString());
            _entryBoxSelectCustomerCountry.ClosePopup += delegate
            {
                //Require to Update RegEx and Criteria to filter Country Clients Only
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Rule = _entryBoxSelectCustomerCountry.Value.RegExFiscalNumber;
                _entryBoxCustomerZipCode.EntryValidation.Rule = _entryBoxSelectCustomerCountry.Value.RegExZipCode;
                //Clear Customer Fields, Except Country
                ClearCustomerAndWayBill(false);
                //Apply Criteria Operators
                ApplyCriteriaToCustomerInputs();
                //Call Main Validate
                Validate();
            };

            //Customer FiscalNumber
            CriteriaOperator criteriaOperatorCustomerFiscalNumber = null;
            _entryBoxSelectCustomerFiscalNumber = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_fiscal_number"), "FiscalNumber", "FiscalNumber", null, criteriaOperatorCustomerFiscalNumber, KeyboardMode.AlfaNumeric, _entryBoxSelectCustomerCountry.Value.RegExFiscalNumber, true);
            _entryBoxSelectCustomerFiscalNumber.ClosePopup += delegate
            {
                if (_entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated) GetCustomerDetails("FiscalNumber", _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text);
                Validate();
            };
            _entryBoxSelectCustomerFiscalNumber.EntryValidation.Changed += _entryBoxSelectCustomerFiscalNumber_Changed;
            _entryBoxSelectCustomerFiscalNumber.EntryValidation.Required = true;
            //Customer CardNumber
            CriteriaOperator criteriaOperatorCustomerCardNumber = null;
            _entryBoxSelectCustomerCardNumber = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_card_number"), "CardNumber", "CardNumber", null, criteriaOperatorCustomerCardNumber, KeyboardMode.AlfaNumeric, SettingsApp.RegexAlfaNumericExtended, false);
            _entryBoxSelectCustomerCardNumber.ClosePopup += delegate
            {
                if (_entryBoxSelectCustomerCardNumber.EntryValidation.Validated) GetCustomerDetails("CardNumber", _entryBoxSelectCustomerCardNumber.EntryValidation.Text);
                Validate();
            };

            //Customer Discount
            _entryBoxCustomerDiscount = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_discount"), KeyboardMode.Alfa, SettingsApp.RegexPercentage, true);
            _entryBoxCustomerDiscount.EntryValidation.Text = FrameworkUtils.DecimalToString(0.0m);
            _entryBoxCustomerDiscount.EntryValidation.Changed += _entryBoxCustomerDiscount_Changed;
            _entryBoxCustomerDiscount.EntryValidation.FocusOutEvent += delegate
            {
                _entryBoxCustomerDiscount.EntryValidation.Text = FrameworkUtils.StringToDecimalAndToStringAgain(_entryBoxCustomerDiscount.EntryValidation.Text);
            };

            //Customer Phone
            _entryBoxCustomerPhone = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_phone"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false);
            _entryBoxCustomerPhone.EntryValidation.Changed += delegate { Validate(); };

            //Customer Email
            _entryBoxCustomerEmail = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_email"), KeyboardMode.Alfa, SettingsApp.RegexEmail, false);
            _entryBoxCustomerEmail.EntryValidation.Changed += delegate { Validate(); };

            //Customer Notes
            _entryBoxCustomerNotes = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_notes"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false);
            _entryBoxCustomerNotes.EntryValidation.Changed += delegate { Validate(); };

            //HBox ZipCode+City+Country
            HBox hboxZipCodeAndCityAndCountry = new HBox(true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(_entryBoxCustomerZipCode, true, true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(_entryBoxCustomerCity, true, true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(_entryBoxSelectCustomerCountry, true, true, 0);

            //HBox Discount+FiscalNumber+CardNumber
            HBox hboxDiscountAndFiscalNumberAndCardNumber = new HBox(true, 0);
            hboxDiscountAndFiscalNumberAndCardNumber.PackStart(_entryBoxSelectCustomerFiscalNumber, true, true, 0);
            hboxDiscountAndFiscalNumberAndCardNumber.PackStart(_entryBoxSelectCustomerCardNumber, true, true, 0);
            hboxDiscountAndFiscalNumberAndCardNumber.PackStart(_entryBoxCustomerDiscount, true, true, 0);

            //HBox Address+Locality
            HBox hboxAddressLocality = new HBox(true, 0);
            hboxAddressLocality.PackStart(_entryBoxCustomerAddress, true, true, 0);
            hboxAddressLocality.PackStart(_entryBoxCustomerLocality, true, true, 0);

            //HBox PhoneEmail
            HBox hboxPhoneEmail = new HBox(true, 0);
            hboxPhoneEmail.PackStart(_entryBoxCustomerPhone, true, true, 0);
            hboxPhoneEmail.PackStart(_entryBoxCustomerEmail, true, true, 0);

            //Pack VBOX
            VBox vbox = new VBox(false, 2);
            vbox.PackStart(_entryBoxSelectCustomerName, false, false, 0);
            vbox.PackStart(hboxDiscountAndFiscalNumberAndCardNumber, false, false, 0);
            vbox.PackStart(hboxAddressLocality, false, false, 0);
            vbox.PackStart(hboxZipCodeAndCityAndCountry, false, false, 0);
            vbox.PackStart(hboxPhoneEmail, false, false, 0);
            vbox.PackStart(_entryBoxCustomerNotes, false, false, 0);
            PackStart(vbox);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        void _entryBoxSelectCustomerFiscalNumber_Changed(object sender, EventArgs e)
        {
            if (_enableGetCustomerDetails)
            {
                UpdateCustomerAddressAndFiscalNumberRequireFields();

                //Extra Protection to only use FinanceFinalConsumerFiscalNumber on FS and FT else is Invalid
                bool isFinalConsumerEntity = FrameworkUtils.IsFinalConsumerEntity(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text);
                if (
                    isFinalConsumerEntity &&
                    (
                        _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid != SettingsApp.XpoOidDocumentFinanceTypeInvoice &&
                        _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid != SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice
                    )
                   )
                {
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated = false;
                }

                if (_entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated)
                {
                    bool isValidFiscalNumber = FiscalNumber.IsValidFiscalNumber(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text, _entryBoxSelectCustomerCountry.Value.Code2);
                    //Get Customer from Fiscal Number
                    if (isValidFiscalNumber)
                    {
                        //Replaced without FiscalNumber.ExtractFiscalNumber Method else we can get From FiscanNumber when dont have Country Prefix
                        //GetCustomerDetails("FiscalNumber", FiscalNumber.ExtractFiscalNumber(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text, _entryBoxSelectCustomerCountry.Value.Code2));
                        GetCustomerDetails("FiscalNumber", _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text);
                    }
                    else
                    {
                        if (_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text != string.Empty) _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated = false;
                    }
                }
                Validate();
            }
        }

        private void _entryBoxSelectCustomerName_Changed(object sender, EventArgs e)
        {
            if (_enableGetCustomerDetails)
            {
                UpdateCustomerAddressAndFiscalNumberRequireFields();
                Validate();
            }
        }

        private void _entryBoxCustomerDiscount_Changed(object sender, EventArgs e)
        {
            //Update Dialog Title with Total, usefull when we change Customer and Have Diferent Discounts, Results and Diferent Totals
            ArticleBag articleBag = (_pagePad.Pages[2] as DocumentFinanceDialogPage3).ArticleBag;
            if (articleBag != null)
            {
                //Update TreeView With Changed Discount
                (_pagePad.Pages[2] as DocumentFinanceDialogPage3).UpdateTotalFinal();
                _posDocumentFinanceDialog.WindowTitle = _posDocumentFinanceDialog.GetPageTitle(_pagePad.CurrentPageIndex);
            }
            Validate();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Public Methods

        //Apply Country CriteriaOperator to Related Inputs
        public void ApplyCriteriaToCustomerInputs()
        {
            string filterBase = "(Disabled IS NULL OR Disabled  <> 1) AND (Hidden IS NULL OR Hidden = 0)";
            string filterExtra =
                (
                    _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid != SettingsApp.XpoOidDocumentFinanceTypeInvoice &&
                    _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid != SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice
                ) ? string.Format("AND (Oid <> '{0}')", SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity) : string.Empty;
            _entryBoxSelectCustomerName.CriteriaOperator = CriteriaOperator.Parse(string.Format("{0} AND (Country = '{1}') {2}", filterBase, _entryBoxSelectCustomerCountry.Value.Oid, filterExtra));
            _entryBoxSelectCustomerFiscalNumber.CriteriaOperator = CriteriaOperator.Parse(string.Format("{0 } AND (Country = '{1}') AND (FiscalNumber IS NOT NULL AND FiscalNumber <> '') {2}", filterBase, _entryBoxSelectCustomerCountry.Value.Oid, filterExtra));
            _entryBoxSelectCustomerCardNumber.CriteriaOperator = CriteriaOperator.Parse(string.Format("{0} AND (Country = '{1}') AND (CardNumber IS NOT NULL AND CardNumber <> '') {2}", filterBase, _entryBoxSelectCustomerCountry.Value.Oid, filterExtra));
        }

        //Override Base Validate
        public override void Validate()
        {
            _validated = (
                _entryBoxSelectCustomerName.EntryValidation.Validated &&
                _entryBoxCustomerAddress.EntryValidation.Validated &&
                _entryBoxCustomerLocality.EntryValidation.Validated &&
                _entryBoxCustomerZipCode.EntryValidation.Validated &&
                _entryBoxCustomerCity.EntryValidation.Validated &&
                _entryBoxCustomerPhone.EntryValidation.Validated &&
                _entryBoxCustomerEmail.EntryValidation.Validated &&
                _entryBoxCustomerDiscount.EntryValidation.Validated &&
                _entryBoxCustomerNotes.EntryValidation.Validated &&
                _entryBoxSelectCustomerCountry.EntryValidation.Validated &&
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated &&
                _entryBoxSelectCustomerCardNumber.EntryValidation.Validated
            );

            //Enable Next Button, If not In Last Page
            if (_pagePad.CurrentPageIndex < _pagePad.Pages.Count - 1 && _pagePad.CurrentPageIndex == 1)
            {
                _pagePad.ButtonNext.Sensitive = _validated;
            }

            //Validate Dialog (All Pages must be Valid)
            _posDocumentFinanceDialog.Validate();
        }

        public void GetCustomerDetails(string pFieldName, string pFieldValue)
        {
            try
            {
                //Disable calls to this function when we trigger .Changed events, creating recursive calls to this function
                _enableGetCustomerDetails = false;

                // Encrypt pFieldValue to use in Sql Filter
                if (GlobalFramework.PluginSoftwareVendor != null)
                {
                    // Only Encrypt Encrypted Fields
                    if (pFieldName == nameof(erp_customer.FiscalNumber) || pFieldName == nameof(erp_customer.CardNumber))
                    {
                        pFieldValue = GlobalFramework.PluginSoftwareVendor.Encrypt(pFieldValue);
                    }
                }

                Guid customerGuid = new Guid();
                string sql = string.Format("SELECT Oid FROM erp_customer WHERE {0} = '{1}' AND (Hidden IS NULL OR Hidden = 0);", pFieldName, pFieldValue);

                if (pFieldValue != string.Empty)
                {
                    customerGuid = FrameworkUtils.GetGuidFromQuery(sql);
                }

                //Assign pagePad.Customer Reference
                if (customerGuid != Guid.Empty)
                {
                    _pagePad.Customer = (erp_customer)FrameworkUtils.GetXPGuidObject(typeof(erp_customer), customerGuid);
                }
                else
                {
                    _pagePad.Customer = null;
                }

                //If Valid Customer, and not Not SimplifiedInvoice, and ! isSingularEntity
                if (
                    _pagePad.Customer != null
                )
                {
                    //Call StoreCurrentCustomer to Store CurrentCustomer before change _entryBoxSelectCustomerName.Value
                    if (_currentCustomerPriceType != (PriceType)_pagePad.Customer.PriceType.EnumValue)
                    {
                        //_log.Debug(String.Format("Diferent Customer PriceTypes Detected: [{0}], Current:[{1}], Last: [{2}]", _pagePad.Customer.Name, (PriceType)_pagePad.Customer.PriceType.EnumValue, _currentCustomerPriceType));
                        //Call UpdateCurrentCustomerPriceType to Update Article Prices
                        ChangedCustomerPriceType();
                    }
                    //Call StoreCurrentCustomerPriceType() after Compare 
                    StoreCurrentCustomerPriceType();

                    //Assign Values to UI Components
                    _entryBoxCustomerAddress.EntryValidation.Text = (_pagePad.Customer.Address == null) ? string.Empty : _pagePad.Customer.Address;
                    _entryBoxCustomerLocality.EntryValidation.Text = (_pagePad.Customer.Locality == null) ? string.Empty : _pagePad.Customer.Locality;
                    _entryBoxCustomerZipCode.EntryValidation.Text = (_pagePad.Customer.ZipCode == null) ? string.Empty : _pagePad.Customer.ZipCode;
                    _entryBoxCustomerCity.EntryValidation.Text = (_pagePad.Customer.City == null) ? string.Empty : _pagePad.Customer.City;
                    _entryBoxCustomerPhone.EntryValidation.Text = (_pagePad.Customer.Phone == null) ? string.Empty : _pagePad.Customer.Phone;
                    _entryBoxCustomerEmail.EntryValidation.Text = (_pagePad.Customer.Email == null) ? string.Empty : _pagePad.Customer.Email;
                    _entryBoxSelectCustomerCountry.Value = _pagePad.Customer.Country;
                    _entryBoxSelectCustomerCountry.EntryValidation.Text = (_pagePad.Customer.Country == null) ? string.Empty : _pagePad.Customer.Country.Designation;
                    _entryBoxSelectCustomerCountry.EntryValidation.Validate(_entryBoxSelectCustomerCountry.Value.Oid.ToString());
                    _entryBoxCustomerDiscount.EntryValidation.Text = (_pagePad.Customer.Discount <= 0.0m) ? FrameworkUtils.DecimalToString(0.0m) : FrameworkUtils.DecimalToString(_pagePad.Customer.Discount);
                    //Require to Update RegEx
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Rule = _pagePad.Customer.Country.RegExFiscalNumber;

                    switch (pFieldName)
                    {
                        case "Oid":
                            _entryBoxSelectCustomerName.Value = _pagePad.Customer;
                            _entryBoxSelectCustomerCardNumber.Value = _pagePad.Customer;
                            _entryBoxSelectCustomerName.EntryValidation.Text = (_pagePad.Customer.Name == null) ? string.Empty : _pagePad.Customer.Name;
                            _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text = (_pagePad.Customer.FiscalNumber == null) ? string.Empty : _pagePad.Customer.FiscalNumber;
                            _entryBoxSelectCustomerCardNumber.EntryValidation.Text = (_pagePad.Customer.CardNumber == null) ? string.Empty : _pagePad.Customer.CardNumber;
                            break;
                        case "FiscalNumber":
                            _entryBoxSelectCustomerName.Value = _pagePad.Customer;
                            _entryBoxSelectCustomerCardNumber.Value = _pagePad.Customer;
                            _entryBoxSelectCustomerName.EntryValidation.Text = (_pagePad.Customer.Name == null) ? string.Empty : _pagePad.Customer.Name;
                            _entryBoxSelectCustomerCardNumber.EntryValidation.Text = (_pagePad.Customer.CardNumber == null) ? string.Empty : _pagePad.Customer.CardNumber;
                            break;
                        case "CardNumber":
                            _entryBoxSelectCustomerName.Value = _pagePad.Customer;
                            _entryBoxSelectCustomerName.EntryValidation.Text = (_pagePad.Customer.Name == null) ? string.Empty : _pagePad.Customer.Name;
                            _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text = (_pagePad.Customer.FiscalNumber == null) ? string.Empty : _pagePad.Customer.FiscalNumber;
                            break;
                        default:
                            break;
                    }
                    _entryBoxCustomerNotes.EntryValidation.Text = (_pagePad.Customer.Notes == null) ? string.Empty : _pagePad.Customer.Notes;
                }
                //IN:009275 Use Euro VAT Info 
                else if (Utils.UseVatAutocomplete())
                {
                    string cod_FiscalNumber = string.Format("{0}{1}", cfg_configurationpreferenceparameter.GetCountryCode2, _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text);
                    var address = EuropeanVatInformation.Get(cod_FiscalNumber).Address.Split('\n');
                    if (address != null)
                    {
                        string zip = address[2].Substring(0, address[2].IndexOf(' '));
                        string city = address[2].Substring(address[2].IndexOf(' ') + 1);
                        _entryBoxCustomerAddress.EntryValidation.Text = address[0];
                        _entryBoxCustomerLocality.EntryValidation.Text = address[1];
                        _entryBoxCustomerZipCode.EntryValidation.Text = zip;
                        _entryBoxCustomerCity.EntryValidation.Text = city;
                        _entryBoxSelectCustomerName.EntryValidation.Text = EuropeanVatInformation.Get(cod_FiscalNumber).Name;

                        switch (pFieldName)
                        {
                            case "Oid":
                                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text = string.Empty;
                                _entryBoxSelectCustomerCardNumber.EntryValidation.Text = string.Empty;
                                break;
                            case "FiscalNumber":
                                _entryBoxSelectCustomerCardNumber.EntryValidation.Text = string.Empty;
                                break;
                            case "CardNumber":
                                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text = string.Empty;
                                break;
                            default:
                                break;
                        }
                        _entryBoxCustomerNotes.EntryValidation.Text = string.Empty;

                    }
                }
                ////IN:009275 ENDS

                else
                {
                    _entryBoxSelectCustomerName.EntryValidation.Text = string.Empty;
                    _entryBoxCustomerAddress.EntryValidation.Text = string.Empty;
                    _entryBoxCustomerLocality.EntryValidation.Text = string.Empty;
                    _entryBoxCustomerZipCode.EntryValidation.Text = string.Empty;
                    _entryBoxCustomerCity.EntryValidation.Text = string.Empty;
                    _entryBoxCustomerPhone.EntryValidation.Text = string.Empty;
                    _entryBoxCustomerEmail.EntryValidation.Text = string.Empty;
                    //Never Change Country
                    //_entryBoxSelectCustomerCountry.Value = _intialValueConfigurationCountry;
                    //_entryBoxSelectCustomerCountry.EntryValidation.Text = _intialValueConfigurationCountry.Designation;
                    _entryBoxSelectCustomerCountry.EntryValidation.Validate(_entryBoxSelectCustomerCountry.Value.Oid.ToString());
                    _entryBoxCustomerDiscount.EntryValidation.Text = FrameworkUtils.DecimalToString(0.0m);

                    switch (pFieldName)
                    {
                        case "Oid":
                            _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text = string.Empty;
                            _entryBoxSelectCustomerCardNumber.EntryValidation.Text = string.Empty;
                            break;
                        case "FiscalNumber":
                            _entryBoxSelectCustomerName.EntryValidation.Text = string.Empty;
                            _entryBoxSelectCustomerCardNumber.EntryValidation.Text = string.Empty;
                            break;
                        case "CardNumber":
                            _entryBoxSelectCustomerName.EntryValidation.Text = string.Empty;
                            _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text = string.Empty;
                            break;
                        default:
                            break;
                    }
                    _entryBoxCustomerNotes.EntryValidation.Text = string.Empty;

                    //Call pagePad4 ClearShipTo
                    _pagePad4.ClearShipTo();
                }

                //Call
                Validate();

                //Call UpdateCustomerEditMode
                UpdateCustomerEditMode();

                //Update ShipTo
                AssignShipToDetails();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            finally
            {
                //Re Enable GetCustomerDetails
                _enableGetCustomerDetails = true;
            }
        }

        public void ClearCustomerAndWayBill()
        {
            ClearCustomerAndWayBill(true);
        }

        public void ClearCustomerAndWayBill(bool pClearCountry)
        {
            try
            {
                //Disable calls to this function when we trigger .Changed events, creating recursive calls to this function
                _enableGetCustomerDetails = false;

                //Restore Default PriceType
                if (_currentCustomerPriceType != PriceType.Price1)
                {
                    _currentCustomerPriceType = PriceType.Price1;
                    //if (_pagePad.Customer != null) _log.Debug(String.Format("PriceTypes Restored to Defaults: [{0}], Current:[{1}], Last: [{2}]", _pagePad.Customer.Name, (PriceType)_pagePad.Customer.PriceType.EnumValue, _currentCustomerPriceType));
                }

                //Clear Reference
                //_pagePad.Customer = null;

                //Clear Fields
                //_entryBoxSelectCustomerName.Value = null;
                _entryBoxSelectCustomerName.EntryValidation.Text = string.Empty;
                _entryBoxCustomerAddress.EntryValidation.Text = string.Empty;
                _entryBoxCustomerLocality.EntryValidation.Text = string.Empty;
                _entryBoxCustomerZipCode.EntryValidation.Text = string.Empty;
                _entryBoxCustomerCity.EntryValidation.Text = string.Empty;
                _entryBoxCustomerPhone.EntryValidation.Text = string.Empty;
                _entryBoxCustomerEmail.EntryValidation.Text = string.Empty;
                if (pClearCountry)
                {
                    _entryBoxSelectCustomerCountry.Value = _intialValueConfigurationCountry;
                    _entryBoxSelectCustomerCountry.EntryValidation.Text = _intialValueConfigurationCountry.Designation;
                }
                _entryBoxCustomerDiscount.EntryValidation.Text = FrameworkUtils.DecimalToString(0.0m);
                _entryBoxSelectCustomerFiscalNumber.Value = null;
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text = string.Empty;
                _entryBoxSelectCustomerCardNumber.Value = null;
                _entryBoxSelectCustomerCardNumber.EntryValidation.Text = string.Empty;
                _entryBoxCustomerNotes.EntryValidation.Text = string.Empty;
                //Validate
                _entryBoxSelectCustomerName.EntryValidation.Validate();
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validate();
                _entryBoxSelectCustomerCardNumber.EntryValidation.Validate();
                _entryBoxSelectCustomerCountry.EntryValidation.Validate(_entryBoxSelectCustomerCountry.Value.Oid.ToString());
                //Enabled FiscalNumber
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Sensitive = true;
                _entryBoxSelectCustomerFiscalNumber.ButtonKeyBoard.Sensitive = true;

                //Clear ShipTo/From
                _pagePad4.ClearShipTo();
                //Disabled - Always have ShipFrom from Defaults
                //_pagePad5.ClearShipFrom();

                //Call UpdateCustomerEditMode
                UpdateCustomerEditMode();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            finally
            {
                //Re Enable GetCustomerDetails
                _enableGetCustomerDetails = true;
            }
        }

        //Almost Equal to PosPaymentDialog Method
        //Method to enable/disable edit components based on current document type, customer type and many other combinations
        public void UpdateCustomerEditMode()
        {
            try
            {
                //Init Variables
                decimal totalDocument = (_pagePad3.ArticleBag != null && _pagePad3.ArticleBag.TotalFinal > 0) ? _pagePad3.ArticleBag.TotalFinal : 0.0m;
                bool isFinalConsumerEntity = (_pagePad.Customer != null && _pagePad.Customer.Oid == SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity) ? true : false;
                bool isHiddenConsumerEntity = (_pagePad.Customer != null && _pagePad.Customer.FiscalNumber == SettingsApp.FinanceFinalConsumerFiscalNumber) ? true : false;
                bool isSingularEntity = (isFinalConsumerEntity || FiscalNumber.IsSingularEntity(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text, _entryBoxSelectCustomerCountry.Value.Code2));
                bool isInvoice = false;
                bool isSimplifiedInvoice = false;
                bool isConferenceDocument = false;
                bool isWayBill = false;
                //Used To Disable FiscalNumber Edits
                // Encrypt pFieldValue to use in Sql Filter
                string fiscalNumberFilterValue = string.Empty;
                if (GlobalFramework.PluginSoftwareVendor != null)
                {
                    fiscalNumberFilterValue = GlobalFramework.PluginSoftwareVendor.Encrypt(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text);
                }
                string sql = string.Format("SELECT Oid FROM erp_customer WHERE FiscalNumber = '{0}' AND (Hidden IS NULL OR Hidden = 0);", fiscalNumberFilterValue);
                Guid customerGuid = FrameworkUtils.GetGuidFromQuery(sql);
                erp_customer customer = (customerGuid != Guid.Empty) ? (erp_customer)FrameworkUtils.GetXPGuidObject(typeof(erp_customer), customerGuid) : null;

                if (_pagePad1.EntryBoxSelectDocumentFinanceType.Value != null)
                {
                    isInvoice = (_pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid == SettingsApp.XpoOidDocumentFinanceTypeInvoice);
                    isSimplifiedInvoice = (_pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid == SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice);
                    isConferenceDocument = (_pagePad1.EntryBoxSelectSourceDocumentFinance.Value != null && _pagePad1.EntryBoxSelectSourceDocumentFinance.Value.DocumentType.Oid == SettingsApp.XpoOidDocumentFinanceTypeConferenceDocument);
                    isWayBill = _pagePad1.EntryBoxSelectDocumentFinanceType.Value.WayBill;
                }

                //Disable/Enable ButtonClearCustomer based on SourceDocument, if has SourceDocument Disable ClearButton
                _posDocumentFinanceDialog.ButtonClearCustomer.Sensitive = (_pagePad1.EntryBoxSelectSourceDocumentFinance.Value == null);

                //If has SourceDocument and not a ConferenceDocument put all Edits and Validation to ReadOnly, and Put Validation Required Fields to False
                //if ((_pagePad1.EntryBoxSelectSourceDocumentFinance.Value != null && !isConferenceDocument) && (! isSingularEntity && isHiddenConsumerEntity) && ! isWayBill)
                //Used || in TestCollapseRowArgs, Was &&
                //if ((_pagePad1.EntryBoxSelectSourceDocumentFinance.Value != null && !isConferenceDocument) || (!isSingularEntity && isHiddenConsumerEntity) && !isWayBill)
                if ((_pagePad1.EntryBoxSelectSourceDocumentFinance.Value != null && !isConferenceDocument) || (!isSingularEntity && isHiddenConsumerEntity) && !isWayBill)
                {
                    // Commented now we can Edit some values to prevent source documents like invoice with totals > 1000 and alter missing values like ZipCode

                    //EntryBox
                    //_entryBoxCustomerAddress.EntryValidation.Sensitive = false;
                    //_entryBoxCustomerAddress.ButtonKeyBoard.Sensitive = false;
                    //_entryBoxCustomerLocality.EntryValidation.Sensitive = false;
                    //_entryBoxCustomerLocality.ButtonKeyBoard.Sensitive = false;
                    //_entryBoxCustomerZipCode.EntryValidation.Sensitive = false;
                    //_entryBoxCustomerZipCode.ButtonKeyBoard.Sensitive = false;
                    //_entryBoxCustomerCity.EntryValidation.Sensitive = false;
                    //_entryBoxCustomerCity.ButtonKeyBoard.Sensitive = false;
                    //_entryBoxCustomerPhone.EntryValidation.Sensitive = false;
                    //_entryBoxCustomerPhone.ButtonKeyBoard.Sensitive = false;
                    //_entryBoxCustomerEmail.EntryValidation.Sensitive = false;
                    //_entryBoxCustomerEmail.ButtonKeyBoard.Sensitive = false;
                    //_entryBoxCustomerNotes.EntryValidation.Sensitive = false;
                    //_entryBoxCustomerNotes.ButtonKeyBoard.Sensitive = false;
                    _entryBoxCustomerDiscount.EntryValidation.Sensitive = false;
                    _entryBoxCustomerDiscount.ButtonKeyBoard.Sensitive = false;

                    //EntryBoxSelect
                    _entryBoxSelectCustomerName.EntryValidation.Sensitive = false;
                    _entryBoxSelectCustomerName.ButtonKeyBoard.Sensitive = false;
                    _entryBoxSelectCustomerName.ButtonSelectValue.Sensitive = false;
                    _entryBoxSelectCustomerCountry.EntryValidation.Sensitive = false;
                    _entryBoxSelectCustomerCountry.ButtonSelectValue.Sensitive = false;
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Sensitive = false;
                    _entryBoxSelectCustomerFiscalNumber.ButtonKeyBoard.Sensitive = false;
                    _entryBoxSelectCustomerFiscalNumber.ButtonSelectValue.Sensitive = false;
                    _entryBoxSelectCustomerCardNumber.EntryValidation.Sensitive = false;
                    _entryBoxSelectCustomerCardNumber.ButtonKeyBoard.Sensitive = false;
                    _entryBoxSelectCustomerCardNumber.ButtonSelectValue.Sensitive = false;

                    //Disable Required Fields after Source Document, usefull to create NC for Hidden Customers
                    //EntryBox
                    _entryBoxCustomerAddress.EntryValidation.Required = false;
                    //_entryBoxCustomerLocality.EntryValidation.Required = false;
                    _entryBoxCustomerZipCode.EntryValidation.Required = false;
                    _entryBoxCustomerCity.EntryValidation.Required = false;
                    _entryBoxCustomerNotes.EntryValidation.Required = false;
                    _entryBoxCustomerDiscount.EntryValidation.Required = false;
                    //EntryBoxSelect
                    _entryBoxSelectCustomerName.EntryValidation.Required = false;
                    _entryBoxSelectCustomerCountry.EntryValidation.Required = false;
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Required = false;
                    _entryBoxSelectCustomerCardNumber.EntryValidation.Required = false;
                }
                else if (
                    //Required Minimal Fields Edit
                    (totalDocument < SettingsApp.FinanceRuleRequiredCustomerDetailsAboveValue && (isInvoice || isSimplifiedInvoice) && isSingularEntity)
                )
                {
                    //Enable edit User details, usefull to edit Name, Address etc
                    bool enableEditCustomerDetails = !isFinalConsumerEntity;

                    //Address
                    _entryBoxCustomerAddress.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerAddress.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //Locality
                    _entryBoxCustomerLocality.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerLocality.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //ZipCode
                    _entryBoxCustomerZipCode.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerZipCode.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //City
                    _entryBoxCustomerCity.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerCity.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //Phone
                    _entryBoxCustomerPhone.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerPhone.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //Email
                    _entryBoxCustomerEmail.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerEmail.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //Notes
                    _entryBoxCustomerNotes.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerNotes.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //Discount
                    _entryBoxCustomerDiscount.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerDiscount.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //Name
                    _entryBoxSelectCustomerName.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxSelectCustomerName.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    _entryBoxSelectCustomerName.ButtonSelectValue.Sensitive = true;
                    //Country
                    _entryBoxSelectCustomerCountry.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxSelectCustomerCountry.ButtonSelectValue.Sensitive = enableEditCustomerDetails;
                    //Always Disabled/Only Enabled in New Customer
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Sensitive = (customer == null);
                    _entryBoxSelectCustomerFiscalNumber.ButtonKeyBoard.Sensitive = (customer == null);
                    //CardNumber
                    _entryBoxSelectCustomerCardNumber.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxSelectCustomerCardNumber.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    _entryBoxSelectCustomerCardNumber.ButtonSelectValue.Sensitive = true;
                    //Validation

                    //EntryBox
                    //_entryBoxSelectCustomerName.EntryValidation.Required = true;//Always Required
                    _entryBoxSelectCustomerName.EntryValidation.Required = false;
                    _entryBoxCustomerAddress.EntryValidation.Required = false;
                    //_entryBoxCustomerLocality.EntryValidation.Required = false;
                    _entryBoxCustomerZipCode.EntryValidation.Required = false;
                    _entryBoxCustomerCity.EntryValidation.Required = false;
                    //EntryBoxSelect
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Required = false;
                }
                else
                {
                    //Enable edit User details, usefull to edit Name, Address etc
                    bool enableEditCustomerDetails = !isFinalConsumerEntity;
                    bool requiredCustomerDetails = (totalDocument >= SettingsApp.FinanceRuleRequiredCustomerDetailsAboveValue || isWayBill);

                    //EntryBox
                    _entryBoxCustomerAddress.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerLocality.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerZipCode.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerCity.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerPhone.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerEmail.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerNotes.EntryValidation.Sensitive = enableEditCustomerDetails;
                    //EntryBoxSelect
                    _entryBoxSelectCustomerName.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxSelectCustomerName.ButtonSelectValue.Sensitive = true;
                    _entryBoxSelectCustomerCountry.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxSelectCustomerCountry.ButtonSelectValue.Sensitive = true;
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Sensitive = true;
                    _entryBoxSelectCustomerFiscalNumber.ButtonSelectValue.Sensitive = true;
                    _entryBoxSelectCustomerCardNumber.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxSelectCustomerCardNumber.ButtonKeyBoard.Sensitive = true;
                    _entryBoxSelectCustomerCardNumber.ButtonSelectValue.Sensitive = true;

                    //Validation
                    //EntryBoxSelect
                    _entryBoxSelectCustomerName.EntryValidation.Required = true;
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Required = true;
                    //EntryBox
                    _entryBoxCustomerAddress.EntryValidation.Required = (!isSingularEntity || requiredCustomerDetails);
                    _entryBoxCustomerZipCode.EntryValidation.Required = (!isSingularEntity || requiredCustomerDetails);
                    _entryBoxCustomerCity.EntryValidation.Required = (!isSingularEntity || requiredCustomerDetails);
                }

                // Update Rules
                _entryBoxCustomerZipCode.EntryValidation.Rule = _entryBoxSelectCustomerCountry.Value.RegExZipCode;
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Rule = _entryBoxSelectCustomerCountry.Value.RegExFiscalNumber;

                //Always Validate All Fields

                //EntryBox
                _entryBoxSelectCustomerName.EntryValidation.Validate();
                _entryBoxCustomerAddress.EntryValidation.Validate();
                _entryBoxCustomerLocality.EntryValidation.Validate();
                _entryBoxCustomerZipCode.EntryValidation.Validate();
                _entryBoxCustomerCity.EntryValidation.Validate();
                _entryBoxCustomerPhone.EntryValidation.Validate();
                _entryBoxCustomerEmail.EntryValidation.Validate();
                _entryBoxCustomerNotes.EntryValidation.Sensitive = true;
                //EntryBoxSelect
                _entryBoxSelectCustomerName.EntryValidation.Validate();
                _entryBoxSelectCustomerCountry.EntryValidation.Validate(_entryBoxSelectCustomerCountry.Value.Oid.ToString());
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validate();
                _entryBoxSelectCustomerCardNumber.EntryValidation.Validate();

                //Call Before ReCheck if FiscalNumber is Valid
                UpdateCustomerAddressAndFiscalNumberRequireFields();

                //ReCheck if FiscalNumber is Valid
                if (_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text != string.Empty && _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated)
                {
                    bool isValidFiscalNumber = FiscalNumber.IsValidFiscalNumber(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text, _entryBoxSelectCustomerCountry.Value.Code2);
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated = isValidFiscalNumber;
                    //Disable FiscalNumber Entry
                    //_entryBoxSelectCustomerFiscalNumber.EntryValidation.Sensitive = !isValidFiscalNumber;
                    //_entryBoxSelectCustomerFiscalNumber.ButtonKeyBoard.Sensitive = !isValidFiscalNumber;
                }

                //Shared

                //If Default Customer always disable Discount
                if (isFinalConsumerEntity && _entryBoxCustomerDiscount.EntryValidation.Sensitive == true)
                {
                    _entryBoxCustomerDiscount.EntryValidation.Sensitive = false;
                }

                //Require Validate
                Validate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //Almost Equal to PosPaymentDialog Method : Both methods have same Name
        //Update Address And FiscalNumber Require Fields
        private void UpdateCustomerAddressAndFiscalNumberRequireFields()
        {
            try
            {
                bool isRequiredAllCustomerDetails = (_pagePad3.ArticleBag != null && _pagePad3.ArticleBag.TotalFinal > SettingsApp.FinanceRuleRequiredCustomerDetailsAboveValue);
                bool isFinalConsumerEntity = (_entryBoxSelectCustomerName.Value != null && _entryBoxSelectCustomerName.Value.Oid == SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity) ? true : false;
                bool isSingularEntity = (
                    isFinalConsumerEntity ||
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated &&
                    FiscalNumber.IsSingularEntity(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text, _entryBoxSelectCustomerCountry.Value.Code2)
                );

                //If is a SingularEntity and Not isRequiredAllCustomerDetails Disable Address, ZipCode and City
                //InTest : Added WayBill to force Address for At WebServices
                if (isSingularEntity && !_pagePad1.EntryBoxSelectDocumentFinanceType.Value.WayBill && !isRequiredAllCustomerDetails)
                {
                    _entryBoxCustomerAddress.EntryValidation.Required = false;
                    _entryBoxCustomerZipCode.EntryValidation.Required = false;
                    _entryBoxCustomerCity.EntryValidation.Required = false;
                    _entryBoxCustomerAddress.EntryValidation.Validate();
                    _entryBoxCustomerZipCode.EntryValidation.Validate();
                    _entryBoxCustomerCity.EntryValidation.Validate();
                    //_entryBoxCustomerPhone.EntryValidation.Validate();
                    //_entryBoxCustomerEmail.EntryValidation.Validate();
                }
                //if TotalFinal > SettingsApp.FinanceRuleRequiredCustomerDetailsAboveValue Required Address, ZipCode and City
                else if (isRequiredAllCustomerDetails)
                {
                    _entryBoxCustomerAddress.EntryValidation.Required = true;
                    _entryBoxCustomerZipCode.EntryValidation.Required = true;
                    _entryBoxCustomerCity.EntryValidation.Required = true;
                    _entryBoxCustomerAddress.EntryValidation.Validate();
                    _entryBoxCustomerZipCode.EntryValidation.Validate();
                    _entryBoxCustomerCity.EntryValidation.Validate();
                    //_entryBoxCustomerPhone.EntryValidation.Validate();
                    //_entryBoxCustomerEmail.EntryValidation.Validate();
                }

                //Always Required NIF or Client Name, or Both if none has Filled or ! isSingularEntity
                if (
                        (
                            _entryBoxSelectCustomerName.EntryValidation.Text == string.Empty &&
                            (
                                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text == string.Empty
                                || !_entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated
                            )
                        )
                        ||
                        (
                            !isSingularEntity
                            && _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text != string.Empty
                        )
                        ||
                        (
                            _pagePad3.ArticleBag != null && _pagePad3.ArticleBag.TotalFinal > SettingsApp.FinanceRuleRequiredCustomerDetailsAboveValue
                        )
                    )
                {
                    _entryBoxSelectCustomerName.EntryValidation.Required = true;
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Required = true;
                    _entryBoxSelectCustomerName.EntryValidation.Validate();
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validate();
                }
                else if (_entryBoxSelectCustomerName.EntryValidation.Text == string.Empty)
                {
                    _entryBoxSelectCustomerName.EntryValidation.Required = false;
                    _entryBoxSelectCustomerName.EntryValidation.Validate();
                }
                else if (_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text == string.Empty)
                {
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Required = false;
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validate();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public void AssignShipToDetails()
        {
            try
            {
                //Get and Update WayBill Mode
                bool wayBillMode = _pagePad1.GetAndUpdateUIWayBillMode();

                if (wayBillMode)
                {
                    DocumentFinanceDialogPage4 pagePad4 = (DocumentFinanceDialogPage4)_pagePad.Pages[3];
                    pagePad4.EntryBoxShipToAddressDetail.EntryValidation.Text = _entryBoxCustomerAddress.EntryValidation.Text;
                    pagePad4.EntryBoxShipToRegion.EntryValidation.Text = _entryBoxCustomerLocality.EntryValidation.Text;
                    pagePad4.EntryBoxShipToPostalCode.EntryValidation.Text = _entryBoxCustomerZipCode.EntryValidation.Text;
                    pagePad4.EntryBoxShipToCity.EntryValidation.Text = _entryBoxCustomerCity.EntryValidation.Text;
                    pagePad4.EntryBoxSelectShipToCountry.EntryValidation.Text = _entryBoxSelectCustomerCountry.EntryValidation.Text;
                    pagePad4.EntryBoxSelectShipToCountry.Value = _entryBoxSelectCustomerCountry.Value;
                    //Clean Entrys
                    //pagePad4.EntryBoxShipToDeliveryDate.EntryValidation.Text = FrameworkUtils.CurrentDateTimeAtomic().ToString(SettingsApp.DateTimeFormat);
                    pagePad4.EntryBoxShipToDeliveryID.EntryValidation.Text = string.Empty;
                    pagePad4.EntryBoxShipToWarehouseID.EntryValidation.Text = string.Empty;
                    pagePad4.EntryBoxShipToLocationID.EntryValidation.Text = string.Empty;
                    //Validate
                    if (pagePad4.EntryBoxSelectShipToCountry.Value != null)
                    {
                        pagePad4.EntryBoxSelectShipToCountry.EntryValidation.Validate(pagePad4.EntryBoxSelectShipToCountry.Value.Oid.ToString());
                        /* IN007018: Require to Update RegExZipCode on Page 4 */
                        pagePad4.EntryBoxShipToPostalCode.EntryValidation.Rule = pagePad4.EntryBoxSelectShipToCountry.Value.RegExZipCode;
                        pagePad4.EntryBoxShipToPostalCode.EntryValidation.Validate();
                    }
                    else
                    {
                        pagePad4.EntryBoxSelectShipToCountry.EntryValidation.Validate(new Guid().ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //Shared Method to store customer before change it by user, this way we compare old and news customer and clean articles
        public void StoreCurrentCustomerPriceType()
        {
            if (_entryBoxSelectCustomerName.Value != null)
            {
                _currentCustomerPriceType = (PriceType)_entryBoxSelectCustomerName.Value.PriceType.EnumValue;
            }
            else
            {
                _currentCustomerPriceType = PriceType.Price1;
            }
            //_log.Debug(String.Format("_currentCustomerPriceType: [{0}]", _currentCustomerPriceType));
        }

        //Detect PriceType Change
        public void ChangedCustomerPriceType()
        {
            //Get Required Object References : TreeViewArticles
            if (_treeViewArticles == null) { _treeViewArticles = (_pagePad.Pages[2] as DocumentFinanceDialogPage3).TreeViewArticles; }
            //Require to delete all Articles, we cant know if user change its orginal price when insert, and we cant recreate it from article prices ex convert from Price1 to Price2
            //Another problem is when we change exchange rate
            _treeViewArticles.DeleteRecords();

            //Clear ArticleBag Before Update Title
            (_pagePad.Pages[2] as DocumentFinanceDialogPage3).ArticleBag = new ArticleBag();

            //Update TitleBar
            _posDocumentFinanceDialog.WindowTitle = _posDocumentFinanceDialog.GetPageTitle(_pagePad.CurrentPageIndex);
        }
    }
}
