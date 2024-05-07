using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.Classes.Finance;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.Enums;
using logicpos.shared.Classes.Finance;
using System;
using logicpos.Classes.Enums.Keyboard;
using System.Collections;
using logicpos.shared.App;
using logicpos.datalayer.App;
using logicpos.financial.library.App;
using logicpos.datalayer.Xpo;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    internal class DocumentFinanceDialogPage2 : PagePadPage
    {
        private readonly Session _session;
        private readonly DocumentFinanceDialogPagePad _pagePad;
        private readonly PosDocumentFinanceDialog _posDocumentFinanceDialog;
        private readonly cfg_configurationcountry _intialValueConfigurationCountry;

        //Used to store Customer PriceType before change it by user, used to compare PriceType after select other (old and new)
        private PriceType _currentCustomerPriceType = PriceType.Price1;
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

        public erp_customer Erp_customer { get; set; }
        public TreeViewCustomer treeViewCustomer { get; set; }

        public bool EnableGetCustomerDetails { get; set; } = true;

        public XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> EntryBoxSelectCustomerName { get; }

        public EntryBoxValidation EntryBoxCustomerAddress { get; }

        public EntryBoxValidation EntryBoxCustomerLocality { get; }

        public EntryBoxValidation EntryBoxCustomerZipCode { get; }

        public EntryBoxValidation EntryBoxCustomerCity { get; }

        public XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry> EntryBoxSelectCustomerCountry { get; }

        public XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> EntryBoxSelectCustomerFiscalNumber { get; }

        public XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> EntryBoxSelectCustomerCardNumber { get; }

        public EntryBoxValidation EntryBoxCustomerDiscount { get; }

        public EntryBoxValidation EntryBoxCustomerPhone { get; }

        public EntryBoxValidation EntryBoxCustomerEmail { get; }

        public EntryBoxValidation EntryBoxCustomerNotes { get; }

        //Constructor
        [Obsolete]
        public DocumentFinanceDialogPage2(Window pSourceWindow, string pPageName)
            : this(pSourceWindow, pPageName, "", null, true) { }

        [Obsolete]
        public DocumentFinanceDialogPage2(Window pSourceWindow, string pPageName, Widget pWidget)
            : this(pSourceWindow, pPageName, "", pWidget, true) { }

        [Obsolete]
        public DocumentFinanceDialogPage2(Window pSourceWindow, string pPageName, string pPageIcon, Widget pWidget, bool pEnabled = true)
            : base(pSourceWindow, pPageName, pPageIcon, pWidget, pEnabled)
        {
            //Init private vars
            _pagePad = (_sourceWindow as PosDocumentFinanceDialog).PagePad;
            _session = (_pagePad as DocumentFinanceDialogPagePad).Session;
            _posDocumentFinanceDialog = (_sourceWindow as PosDocumentFinanceDialog);

            //Initials Values
            _intialValueConfigurationCountry = DataLayerSettings.ConfigurationSystemCountry;            
            //Customer Name
            CriteriaOperator criteriaOperatorCustomerName = null;
			//TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 
            treeViewCustomer = new TreeViewCustomer(
              pSourceWindow,
              null,//DefaultValue 
              null,//DialogType
              null         
            );

         
            erp_customer customer = null;
            SortingCollection sortCollection = new SortingCollection
            {
                new SortProperty("Name", DevExpress.Xpo.DB.SortingDirection.Ascending)
            };
            CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
            ICollection collectionCustomers = XPOSettings.Session.GetObjects(XPOSettings.Session.GetClassInfo(typeof(erp_customer)), criteria, sortCollection, int.MaxValue, false, true);

            foreach (erp_customer item in collectionCustomers)
            {
                customer = item;
            }
            customer.Name = "";
            customer.FiscalNumber = "";
            EntryBoxSelectCustomerName = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(
                _sourceWindow, 
                CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_customer"), 
                "Name", 
                "Name", 
                customer, 
                criteriaOperatorCustomerName, 
                KeyboardMode.Alfa, 
                LogicPOS.Utility.RegexUtils.RegexAlfaNumericPlus, 
                true);
            
            EntryBoxSelectCustomerName.ClosePopup += delegate
            {
                GetCustomerDetails("Oid", EntryBoxSelectCustomerName.Value.Oid.ToString());
                Validate();
            };
			//TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 
            EntryBoxSelectCustomerName.EntryValidation.Changed += delegate
            {
                _entryBoxSelectCustomerName_Changed(EntryBoxSelectCustomerName.EntryValidation, null);
                if(EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text == string.Empty)
                {
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Validated = false;
                    _pagePad.ButtonNext.Sensitive = false;
                }
            };
            EntryBoxSelectCustomerName.EntryValidation.IsEditable = true;

            //Customer Address
            EntryBoxCustomerAddress = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_address"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericPlus, false);/* IN009253 */
            EntryBoxCustomerAddress.EntryValidation.Changed += delegate { Validate(); };

            //Customer Locality
            EntryBoxCustomerLocality = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_locality"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericPlus, false);/* IN009253 */
            EntryBoxCustomerLocality.EntryValidation.Changed += delegate { Validate(); };

            //Customer ZipCode
            EntryBoxCustomerZipCode = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_zipcode"), KeyboardMode.Alfa, DataLayerSettings.ConfigurationSystemCountry.RegExZipCode, false);
            EntryBoxCustomerZipCode.EntryValidation.Changed += delegate { Validate(); };

            //Customer City
            EntryBoxCustomerCity = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_city"), KeyboardMode.AlfaNumeric, LogicPOS.Utility.RegexUtils.RegexAlfaNumericPlus, false);/* IN009253 */
            EntryBoxCustomerCity.EntryValidation.Changed += delegate { Validate(); };

            //Customer Country
            CriteriaOperator criteriaOperatorCustomerCountry = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1) AND (RegExFiscalNumber IS NOT NULL AND RegExZipCode IS NOT NULL)");
            EntryBoxSelectCustomerCountry = new XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry>(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_country"), "Designation", "Oid", _intialValueConfigurationCountry, criteriaOperatorCustomerCountry, LogicPOS.Utility.RegexUtils.RegexGuid, true);
            EntryBoxSelectCustomerCountry.EntryValidation.IsEditable = true;
            EntryBoxSelectCustomerCountry.EntryValidation.Validate(EntryBoxSelectCustomerCountry.Value.Oid.ToString());
            EntryBoxSelectCustomerCountry.ClosePopup += delegate
            {
                //Require to Update RegEx and Criteria to filter Country Clients Only
                EntryBoxSelectCustomerFiscalNumber.EntryValidation.Rule = EntryBoxSelectCustomerCountry.Value.RegExFiscalNumber;
                EntryBoxCustomerZipCode.EntryValidation.Rule = EntryBoxSelectCustomerCountry.Value.RegExZipCode;
                //Clear Customer Fields, Except Country
                ClearCustomerAndWayBill(false);
                //Apply Criteria Operators
                ApplyCriteriaToCustomerInputs();
                //Call Main Validate
                Validate();
            };

            //Customer FiscalNumber
            CriteriaOperator criteriaOperatorCustomerFiscalNumber = null;
            EntryBoxSelectCustomerFiscalNumber = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_fiscal_number"), "FiscalNumber", "FiscalNumber", null, criteriaOperatorCustomerFiscalNumber, KeyboardMode.AlfaNumeric, EntryBoxSelectCustomerCountry.Value.RegExFiscalNumber, true);
            EntryBoxSelectCustomerFiscalNumber.ClosePopup += delegate
            {
                if (EntryBoxSelectCustomerFiscalNumber.EntryValidation.Validated) GetCustomerDetails("FiscalNumber", EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text);
                Validate();
            };
            EntryBoxSelectCustomerFiscalNumber.EntryValidation.Changed += _entryBoxSelectCustomerFiscalNumber_Changed;
            EntryBoxSelectCustomerFiscalNumber.EntryValidation.Required = true;
            //Customer CardNumber
            CriteriaOperator criteriaOperatorCustomerCardNumber = null;
            EntryBoxSelectCustomerCardNumber = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(
                _sourceWindow, 
                CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_card_number"), 
                "CardNumber", 
                "CardNumber", 
                null, 
                criteriaOperatorCustomerCardNumber, 
                KeyboardMode.AlfaNumeric, 
                LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, 
                false);

            EntryBoxSelectCustomerCardNumber.ClosePopup += delegate
            {
                if (EntryBoxSelectCustomerCardNumber.EntryValidation.Validated) GetCustomerDetails("CardNumber", EntryBoxSelectCustomerCardNumber.EntryValidation.Text);
                Validate();
            };

            //Customer Discount
            EntryBoxCustomerDiscount = new EntryBoxValidation(
                _sourceWindow, 
                CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_discount"),
                KeyboardMode.Alfa, 
                LogicPOS.Utility.RegexUtils.RegexPercentage, 
                true);

            EntryBoxCustomerDiscount.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(0.0m);
            EntryBoxCustomerDiscount.EntryValidation.Changed += _entryBoxCustomerDiscount_Changed;
            EntryBoxCustomerDiscount.EntryValidation.FocusOutEvent += delegate
            {
                EntryBoxCustomerDiscount.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.StringToDecimalAndToStringAgain(EntryBoxCustomerDiscount.EntryValidation.Text);
            };

            //Customer Phone
            EntryBoxCustomerPhone = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_phone"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false);
            EntryBoxCustomerPhone.EntryValidation.Changed += delegate { Validate(); };

            //Customer Email
            EntryBoxCustomerEmail = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_email"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexEmail, false);
            EntryBoxCustomerEmail.EntryValidation.Changed += delegate { Validate(); };

            //Customer Notes
            EntryBoxCustomerNotes = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_notes"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false);
            EntryBoxCustomerNotes.EntryValidation.Changed += delegate { Validate(); };

            //HBox ZipCode+City+Country
            HBox hboxZipCodeAndCityAndCountry = new HBox(true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(EntryBoxCustomerZipCode, true, true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(EntryBoxCustomerCity, true, true, 0);
            hboxZipCodeAndCityAndCountry.PackStart(EntryBoxSelectCustomerCountry, true, true, 0);

            //HBox Discount+FiscalNumber+CardNumber
            HBox hboxDiscountAndFiscalNumberAndCardNumber = new HBox(true, 0);
            hboxDiscountAndFiscalNumberAndCardNumber.PackStart(EntryBoxSelectCustomerFiscalNumber, true, true, 0);
            hboxDiscountAndFiscalNumberAndCardNumber.PackStart(EntryBoxSelectCustomerCardNumber, true, true, 0);
            hboxDiscountAndFiscalNumberAndCardNumber.PackStart(EntryBoxCustomerDiscount, true, true, 0);

            //HBox Address+Locality
            HBox hboxAddressLocality = new HBox(true, 0);
            hboxAddressLocality.PackStart(EntryBoxCustomerAddress, true, true, 0);
            hboxAddressLocality.PackStart(EntryBoxCustomerLocality, true, true, 0);

            //HBox PhoneEmail
            HBox hboxPhoneEmail = new HBox(true, 0);
            hboxPhoneEmail.PackStart(EntryBoxCustomerPhone, true, true, 0);
            hboxPhoneEmail.PackStart(EntryBoxCustomerEmail, true, true, 0);

            //Pack VBOX
            VBox vbox = new VBox(false, 2);
            vbox.PackStart(EntryBoxSelectCustomerName, false, false, 0);
            vbox.PackStart(hboxDiscountAndFiscalNumberAndCardNumber, false, false, 0);
            vbox.PackStart(hboxAddressLocality, false, false, 0);
            vbox.PackStart(hboxZipCodeAndCityAndCountry, false, false, 0);
            vbox.PackStart(hboxPhoneEmail, false, false, 0);
            vbox.PackStart(EntryBoxCustomerNotes, false, false, 0);
            PackStart(vbox);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        private void _entryBoxSelectCustomerFiscalNumber_Changed(object sender, EventArgs e)
        {
            if (EnableGetCustomerDetails)
            {
                UpdateCustomerAddressAndFiscalNumberRequireFields();

                //Extra Protection to only use FinanceFinalConsumerFiscalNumber on FS and FT else is Invalid
                bool isFinalConsumerEntity = FinancialLibraryUtils.IsFinalConsumerEntity(EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text);
                if (
                    isFinalConsumerEntity &&
                    (
                        _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid != SharedSettings.XpoOidDocumentFinanceTypeInvoice &&
                        _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid != SharedSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice
                    )
                   )
                {
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Validated = false;
                }

                if (EntryBoxSelectCustomerFiscalNumber.EntryValidation.Validated)
                {
                    bool isValidFiscalNumber = FiscalNumber.IsValidFiscalNumber(EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text, EntryBoxSelectCustomerCountry.Value.Code2);
                    //Get Customer from Fiscal Number
                    if (isValidFiscalNumber)
                    {
                        //Replaced without FiscalNumber.ExtractFiscalNumber Method else we can get From FiscanNumber when dont have Country Prefix
                        //GetCustomerDetails("FiscalNumber", FiscalNumber.ExtractFiscalNumber(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text, _entryBoxSelectCustomerCountry.Value.Code2));
                        GetCustomerDetails("FiscalNumber", EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text);
                    }
                    else
                    {
                        if (EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text != string.Empty) EntryBoxSelectCustomerFiscalNumber.EntryValidation.Validated = false;
                    }
                }
                Validate();
            }
        }

        private void _entryBoxSelectCustomerName_Changed(object sender, EventArgs e)
        {
            if (EnableGetCustomerDetails)
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
                    _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid != SharedSettings.XpoOidDocumentFinanceTypeInvoice &&
                    _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid != SharedSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice
                ) ? string.Format("AND (Oid <> '{0}')", SharedSettings.FinalConsumerId) : string.Empty;
            EntryBoxSelectCustomerName.CriteriaOperator = CriteriaOperator.Parse(string.Format("{0} AND (Country = '{1}') {2}", filterBase, EntryBoxSelectCustomerCountry.Value.Oid, filterExtra));
            EntryBoxSelectCustomerFiscalNumber.CriteriaOperator = CriteriaOperator.Parse(string.Format("{0 } AND (Country = '{1}') AND (FiscalNumber IS NOT NULL AND FiscalNumber <> '') {2}", filterBase, EntryBoxSelectCustomerCountry.Value.Oid, filterExtra));
            EntryBoxSelectCustomerCardNumber.CriteriaOperator = CriteriaOperator.Parse(string.Format("{0} AND (Country = '{1}') AND (CardNumber IS NOT NULL AND CardNumber <> '') {2}", filterBase, EntryBoxSelectCustomerCountry.Value.Oid, filterExtra));
        }

        //Override Base Validate
        public override void Validate()
        {
            _validated = (
                EntryBoxSelectCustomerName.EntryValidation.Validated &&
                EntryBoxCustomerAddress.EntryValidation.Validated &&
                EntryBoxCustomerLocality.EntryValidation.Validated &&
                EntryBoxCustomerZipCode.EntryValidation.Validated &&
                EntryBoxCustomerCity.EntryValidation.Validated &&
                EntryBoxCustomerPhone.EntryValidation.Validated &&
                EntryBoxCustomerEmail.EntryValidation.Validated &&
                EntryBoxCustomerDiscount.EntryValidation.Validated &&
                EntryBoxCustomerNotes.EntryValidation.Validated &&
                EntryBoxSelectCustomerCountry.EntryValidation.Validated &&
                EntryBoxSelectCustomerFiscalNumber.EntryValidation.Validated &&
                EntryBoxSelectCustomerCardNumber.EntryValidation.Validated
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
                EnableGetCustomerDetails = false;

                // Encrypt pFieldValue to use in Sql Filter
                if (LogicPOS.Settings.PluginSettings.PluginSoftwareVendor != null)
                {
                    // Only Encrypt Encrypted Fields
                    if (pFieldName == nameof(Erp_customer.FiscalNumber) || pFieldName == nameof(Erp_customer.CardNumber))
                    {
                        pFieldValue = LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.Encrypt(pFieldValue);
                    }
                }

                Guid customerGuid = new Guid();
                string sql = string.Format("SELECT Oid FROM erp_customer WHERE {0} = '{1}' AND (Hidden IS NULL OR Hidden = 0);", pFieldName, pFieldValue);

                if (pFieldValue != string.Empty)
                {
                    customerGuid = XPOHelper.GetGuidFromQuery(sql);
                }

                //Assign pagePad.Customer Reference
                if (customerGuid != Guid.Empty)
                {
                    _pagePad.Customer = (erp_customer)XPOHelper.GetXPGuidObject(typeof(erp_customer), customerGuid);
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
                        //_logger.Debug(String.Format("Diferent Customer PriceTypes Detected: [{0}], Current:[{1}], Last: [{2}]", _pagePad.Customer.Name, (PriceType)_pagePad.Customer.PriceType.EnumValue, _currentCustomerPriceType));
                        //Call UpdateCurrentCustomerPriceType to Update Article Prices
                        ChangedCustomerPriceType();
                    }
                    //Call StoreCurrentCustomerPriceType() after Compare 
                    StoreCurrentCustomerPriceType();

                    //Assign Values to UI Components
                    EntryBoxCustomerAddress.EntryValidation.Text = (_pagePad.Customer.Address == null) ? string.Empty : _pagePad.Customer.Address;
                    EntryBoxCustomerLocality.EntryValidation.Text = (_pagePad.Customer.Locality == null) ? string.Empty : _pagePad.Customer.Locality;
                    EntryBoxCustomerZipCode.EntryValidation.Text = (_pagePad.Customer.ZipCode == null) ? string.Empty : _pagePad.Customer.ZipCode;
                    EntryBoxCustomerCity.EntryValidation.Text = (_pagePad.Customer.City == null) ? string.Empty : _pagePad.Customer.City;
                    EntryBoxCustomerPhone.EntryValidation.Text = (_pagePad.Customer.Phone == null) ? string.Empty : _pagePad.Customer.Phone;
                    EntryBoxCustomerEmail.EntryValidation.Text = (_pagePad.Customer.Email == null) ? string.Empty : _pagePad.Customer.Email;
                    EntryBoxSelectCustomerCountry.Value = _pagePad.Customer.Country;
                    EntryBoxSelectCustomerCountry.EntryValidation.Text = (_pagePad.Customer.Country == null) ? string.Empty : _pagePad.Customer.Country.Designation;
                    EntryBoxSelectCustomerCountry.EntryValidation.Validate(EntryBoxSelectCustomerCountry.Value.Oid.ToString());
                    EntryBoxCustomerDiscount.EntryValidation.Text = (_pagePad.Customer.Discount <= 0.0m) ? LogicPOS.Utility.DataConversionUtils.DecimalToString(0.0m) : LogicPOS.Utility.DataConversionUtils.DecimalToString(_pagePad.Customer.Discount);
                    //Require to Update RegEx
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Rule = _pagePad.Customer.Country.RegExFiscalNumber;

                    switch (pFieldName)
                    {
                        case "Oid":
                            EntryBoxSelectCustomerName.Value = _pagePad.Customer;
                            EntryBoxSelectCustomerCardNumber.Value = _pagePad.Customer;
                            EntryBoxSelectCustomerName.EntryValidation.Text = (_pagePad.Customer.Name == null) ? string.Empty : _pagePad.Customer.Name;
                            EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text = (_pagePad.Customer.FiscalNumber == null) ? string.Empty : _pagePad.Customer.FiscalNumber;
                            EntryBoxSelectCustomerCardNumber.EntryValidation.Text = (_pagePad.Customer.CardNumber == null) ? string.Empty : _pagePad.Customer.CardNumber;
                            break;
                        case "FiscalNumber":
                            EntryBoxSelectCustomerName.Value = _pagePad.Customer;
                            EntryBoxSelectCustomerCardNumber.Value = _pagePad.Customer;
                            EntryBoxSelectCustomerName.EntryValidation.Text = (_pagePad.Customer.Name == null) ? string.Empty : _pagePad.Customer.Name;
                            EntryBoxSelectCustomerCardNumber.EntryValidation.Text = (_pagePad.Customer.CardNumber == null) ? string.Empty : _pagePad.Customer.CardNumber;
                            break;
                        case "CardNumber":
                            EntryBoxSelectCustomerName.Value = _pagePad.Customer;
                            EntryBoxSelectCustomerName.EntryValidation.Text = (_pagePad.Customer.Name == null) ? string.Empty : _pagePad.Customer.Name;
                            EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text = (_pagePad.Customer.FiscalNumber == null) ? string.Empty : _pagePad.Customer.FiscalNumber;
                            break;
                        default:
                            break;
                    }
                    EntryBoxCustomerNotes.EntryValidation.Text = (_pagePad.Customer.Notes == null) ? string.Empty : _pagePad.Customer.Notes;
                }
                //IN:009275 Use Euro VAT Info 
                else if (logicpos.Utils.UseVatAutocomplete())
                {
                    string cod_FiscalNumber = string.Format("{0}{1}", cfg_configurationpreferenceparameter.GetCountryCode2, EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text);
                    var address = EuropeanVatInformation.Get(cod_FiscalNumber).Address.Split('\n');
                    if (address != null)
                    {
                        string zip = address[2].Substring(0, address[2].IndexOf(' '));
                        string city = address[2].Substring(address[2].IndexOf(' ') + 1);
                        EntryBoxCustomerAddress.EntryValidation.Text = address[0];
                        EntryBoxCustomerLocality.EntryValidation.Text = address[1];
                        EntryBoxCustomerZipCode.EntryValidation.Text = zip;
                        EntryBoxCustomerCity.EntryValidation.Text = city;
                        EntryBoxSelectCustomerName.EntryValidation.Text = EuropeanVatInformation.Get(cod_FiscalNumber).Name;

                        switch (pFieldName)
                        {
                            case "Oid":
                                EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text = string.Empty;
                                EntryBoxSelectCustomerCardNumber.EntryValidation.Text = string.Empty;
                                break;
                            case "FiscalNumber":
                                EntryBoxSelectCustomerCardNumber.EntryValidation.Text = string.Empty;
                                break;
                            case "CardNumber":
                                EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text = string.Empty;
                                break;
                            default:
                                break;
                        }
                        EntryBoxCustomerNotes.EntryValidation.Text = string.Empty;

                    }
                }
                ////IN:009275 ENDS

                else
                {
                    EntryBoxSelectCustomerName.EntryValidation.Text = string.Empty;
                    EntryBoxCustomerAddress.EntryValidation.Text = string.Empty;
                    EntryBoxCustomerLocality.EntryValidation.Text = string.Empty;
                    EntryBoxCustomerZipCode.EntryValidation.Text = string.Empty;
                    EntryBoxCustomerCity.EntryValidation.Text = string.Empty;
                    EntryBoxCustomerPhone.EntryValidation.Text = string.Empty;
                    EntryBoxCustomerEmail.EntryValidation.Text = string.Empty;
                    //Never Change Country
                    //_entryBoxSelectCustomerCountry.Value = _intialValueConfigurationCountry;
                    //_entryBoxSelectCustomerCountry.EntryValidation.Text = _intialValueConfigurationCountry.Designation;
                    EntryBoxSelectCustomerCountry.EntryValidation.Validate(EntryBoxSelectCustomerCountry.Value.Oid.ToString());
                    EntryBoxCustomerDiscount.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(0.0m);

                    switch (pFieldName)
                    {
                        case "Oid":
                            EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text = string.Empty;
                            EntryBoxSelectCustomerCardNumber.EntryValidation.Text = string.Empty;
                            break;
                        case "FiscalNumber":
                            EntryBoxSelectCustomerName.EntryValidation.Text = string.Empty;
                            EntryBoxSelectCustomerCardNumber.EntryValidation.Text = string.Empty;
                            break;
                        case "CardNumber":
                            EntryBoxSelectCustomerName.EntryValidation.Text = string.Empty;
                            EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text = string.Empty;
                            break;
                        default:
                            break;
                    }
                    EntryBoxCustomerNotes.EntryValidation.Text = string.Empty;

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
                _logger.Error(ex.Message, ex);
            }
            finally
            {
                //Re Enable GetCustomerDetails
                EnableGetCustomerDetails = true;
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
                EnableGetCustomerDetails = false;

                //Restore Default PriceType
                if (_currentCustomerPriceType != PriceType.Price1)
                {
                    _currentCustomerPriceType = PriceType.Price1;
                    //if (_pagePad.Customer != null) _logger.Debug(String.Format("PriceTypes Restored to Defaults: [{0}], Current:[{1}], Last: [{2}]", _pagePad.Customer.Name, (PriceType)_pagePad.Customer.PriceType.EnumValue, _currentCustomerPriceType));
                }

                //Clear Reference
                //_pagePad.Customer = null;

                //Clear Fields
                //_entryBoxSelectCustomerName.Value = null;
                EntryBoxSelectCustomerName.EntryValidation.Text = string.Empty;
                EntryBoxCustomerAddress.EntryValidation.Text = string.Empty;
                EntryBoxCustomerLocality.EntryValidation.Text = string.Empty;
                EntryBoxCustomerZipCode.EntryValidation.Text = string.Empty;
                EntryBoxCustomerCity.EntryValidation.Text = string.Empty;
                EntryBoxCustomerPhone.EntryValidation.Text = string.Empty;
                EntryBoxCustomerEmail.EntryValidation.Text = string.Empty;
                if (pClearCountry)
                {
                    EntryBoxSelectCustomerCountry.Value = _intialValueConfigurationCountry;
                    EntryBoxSelectCustomerCountry.EntryValidation.Text = _intialValueConfigurationCountry.Designation;
                }
                EntryBoxCustomerDiscount.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(0.0m);
                EntryBoxSelectCustomerFiscalNumber.Value = null;
                EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text = string.Empty;
                EntryBoxSelectCustomerCardNumber.Value = null;
                EntryBoxSelectCustomerCardNumber.EntryValidation.Text = string.Empty;
                EntryBoxCustomerNotes.EntryValidation.Text = string.Empty;
                //Validate
                EntryBoxSelectCustomerName.EntryValidation.Validate();
                EntryBoxSelectCustomerFiscalNumber.EntryValidation.Validate();
                EntryBoxSelectCustomerCardNumber.EntryValidation.Validate();
                EntryBoxSelectCustomerCountry.EntryValidation.Validate(EntryBoxSelectCustomerCountry.Value.Oid.ToString());
                //Enabled FiscalNumber
                EntryBoxSelectCustomerFiscalNumber.EntryValidation.Sensitive = true;
                EntryBoxSelectCustomerFiscalNumber.ButtonKeyBoard.Sensitive = true;

                //Clear ShipTo/From
                _pagePad4.ClearShipTo();
                //Disabled - Always have ShipFrom from Defaults
                //_pagePad5.ClearShipFrom();

                //Call UpdateCustomerEditMode
                UpdateCustomerEditMode();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            finally
            {
                //Re Enable GetCustomerDetails
                EnableGetCustomerDetails = true;
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
                bool isFinalConsumerEntity = (_pagePad.Customer != null && _pagePad.Customer.Oid == SharedSettings.FinalConsumerId);
                bool isHiddenConsumerEntity = (_pagePad.Customer != null && _pagePad.Customer.FiscalNumber == SharedSettings.FinanceFinalConsumerFiscalNumber);
                bool isSingularEntity = (isFinalConsumerEntity || FiscalNumber.IsSingularEntity(EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text, EntryBoxSelectCustomerCountry.Value.Code2));
                bool isInvoice = false;
                bool isSimplifiedInvoice = false;
                bool isConferenceDocument = false;
                bool isWayBill = false;
                //Used To Disable FiscalNumber Edits
                // Encrypt pFieldValue to use in Sql Filter
                string fiscalNumberFilterValue = string.Empty;
                if (LogicPOS.Settings.PluginSettings.PluginSoftwareVendor != null)
                {
                    fiscalNumberFilterValue = LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.Encrypt(EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text);
                }
                string sql = string.Format("SELECT Oid FROM erp_customer WHERE FiscalNumber = '{0}' AND (Hidden IS NULL OR Hidden = 0);", fiscalNumberFilterValue);
                Guid customerGuid = XPOHelper.GetGuidFromQuery(sql);
                erp_customer customer = (customerGuid != Guid.Empty) ? (erp_customer)XPOHelper.GetXPGuidObject(typeof(erp_customer), customerGuid) : null;

                if (_pagePad1.EntryBoxSelectDocumentFinanceType.Value != null)
                {
                    isInvoice = (_pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid == SharedSettings.XpoOidDocumentFinanceTypeInvoice);
                    isSimplifiedInvoice = (_pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid == SharedSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice);
                    isConferenceDocument = (_pagePad1.EntryBoxSelectSourceDocumentFinance.Value != null && _pagePad1.EntryBoxSelectSourceDocumentFinance.Value.DocumentType.Oid == SharedSettings.XpoOidDocumentFinanceTypeConferenceDocument);
                    isWayBill = _pagePad1.EntryBoxSelectDocumentFinanceType.Value.WayBill;
                }
				// Moçambique - Pedidos da reunião 13/10/2020 + Faturas no Front-Office [IN:014327]
                //Disable/Enable ButtonClearCustomer based on SourceDocument, if has SourceDocument Disable ClearButton
                _posDocumentFinanceDialog.ButtonClearCustomer.Sensitive = (_pagePad1.EntryBoxSelectSourceDocumentFinance.Value == null);

                if(_pagePad1.EntryBoxSelectSourceDocumentFinance.Value != null && SharedSettings.XpoOidConfigurationCountryMozambique.Equals(DataLayerSettings.ConfigurationSystemCountry.Oid) 
                    && _pagePad1.EntryBoxSelectSourceDocumentFinance.Value != null 
                    && (_pagePad1.EntryBoxSelectSourceDocumentFinance.Value.DocumentType.Oid == SharedSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice || _pagePad1.EntryBoxSelectSourceDocumentFinance.Value.DocumentType.Oid == SharedSettings.XpoOidDocumentFinanceTypeInvoice))
                {

                    _posDocumentFinanceDialog.ButtonClearCustomer.Sensitive = true;
                    EntryBoxCustomerDiscount.EntryValidation.Sensitive = true;
                    EntryBoxCustomerDiscount.ButtonKeyBoard.Sensitive = true;

                    //EntryBoxSelect
                    EntryBoxSelectCustomerName.EntryValidation.Sensitive = true;
                    EntryBoxSelectCustomerName.ButtonKeyBoard.Sensitive = true;
                    EntryBoxSelectCustomerName.ButtonSelectValue.Sensitive = true;
                    EntryBoxSelectCustomerCountry.EntryValidation.Sensitive = true;
                    EntryBoxSelectCustomerCountry.ButtonSelectValue.Sensitive = true;
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Sensitive = true;
                    EntryBoxSelectCustomerFiscalNumber.ButtonKeyBoard.Sensitive = true;
                    EntryBoxSelectCustomerFiscalNumber.ButtonSelectValue.Sensitive = true;
                    EntryBoxSelectCustomerCardNumber.EntryValidation.Sensitive = true;
                    EntryBoxSelectCustomerCardNumber.ButtonKeyBoard.Sensitive = true;
                    EntryBoxSelectCustomerCardNumber.ButtonSelectValue.Sensitive = true;

                    //Disable Required Fields after Source Document, usefull to create NC for Hidden Customers
                    //EntryBox
                    EntryBoxCustomerAddress.EntryValidation.Required = true;
                    //_entryBoxCustomerLocality.EntryValidation.Required = false;
                    EntryBoxCustomerZipCode.EntryValidation.Required = true;
                    EntryBoxCustomerCity.EntryValidation.Required = true;
                    EntryBoxCustomerNotes.EntryValidation.Required = true;
                    EntryBoxCustomerDiscount.EntryValidation.Required = true;
                    //EntryBoxSelect
                    EntryBoxSelectCustomerName.EntryValidation.Required = true;
                    EntryBoxSelectCustomerCountry.EntryValidation.Required = true;
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Required = true;
                    EntryBoxSelectCustomerCardNumber.EntryValidation.Required = false;
                }

                //If has SourceDocument and not a ConferenceDocument put all Edits and Validation to ReadOnly, and Put Validation Required Fields to False
                //if ((_pagePad1.EntryBoxSelectSourceDocumentFinance.Value != null && !isConferenceDocument) && (! isSingularEntity && isHiddenConsumerEntity) && ! isWayBill)
                //Used || in TestCollapseRowArgs, Was &&
                //if ((_pagePad1.EntryBoxSelectSourceDocumentFinance.Value != null && !isConferenceDocument) || (!isSingularEntity && isHiddenConsumerEntity) && !isWayBill)
                else if ((_pagePad1.EntryBoxSelectSourceDocumentFinance.Value != null && !isConferenceDocument) || (!isSingularEntity && isHiddenConsumerEntity) && !isWayBill)
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
                    EntryBoxCustomerDiscount.EntryValidation.Sensitive = false;
                    EntryBoxCustomerDiscount.ButtonKeyBoard.Sensitive = false;

                    //EntryBoxSelect
                    EntryBoxSelectCustomerName.EntryValidation.Sensitive = false;
                    EntryBoxSelectCustomerName.ButtonKeyBoard.Sensitive = false;
                    EntryBoxSelectCustomerName.ButtonSelectValue.Sensitive = false;
                    EntryBoxSelectCustomerCountry.EntryValidation.Sensitive = false;
                    EntryBoxSelectCustomerCountry.ButtonSelectValue.Sensitive = false;
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Sensitive = false;
                    EntryBoxSelectCustomerFiscalNumber.ButtonKeyBoard.Sensitive = false;
                    EntryBoxSelectCustomerFiscalNumber.ButtonSelectValue.Sensitive = false;
                    EntryBoxSelectCustomerCardNumber.EntryValidation.Sensitive = false;
                    EntryBoxSelectCustomerCardNumber.ButtonKeyBoard.Sensitive = false;
                    EntryBoxSelectCustomerCardNumber.ButtonSelectValue.Sensitive = false;

                    //Disable Required Fields after Source Document, usefull to create NC for Hidden Customers
                    //EntryBox
                    EntryBoxCustomerAddress.EntryValidation.Required = false;
                    //_entryBoxCustomerLocality.EntryValidation.Required = false;
                    EntryBoxCustomerZipCode.EntryValidation.Required = false;
                    EntryBoxCustomerCity.EntryValidation.Required = false;
                    EntryBoxCustomerNotes.EntryValidation.Required = false;
                    EntryBoxCustomerDiscount.EntryValidation.Required = false;
                    //EntryBoxSelect
                    EntryBoxSelectCustomerName.EntryValidation.Required = false;
                    EntryBoxSelectCustomerCountry.EntryValidation.Required = false;
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Required = false;
                    EntryBoxSelectCustomerCardNumber.EntryValidation.Required = false;
                }
                else if (
                    //Required Minimal Fields Edit
                    (totalDocument < SharedSettings.FinanceRuleRequiredCustomerDetailsAboveValue && (isInvoice || isSimplifiedInvoice) && isSingularEntity)
                )
                {
                    //Enable edit User details, usefull to edit Name, Address etc
                    bool enableEditCustomerDetails = !isFinalConsumerEntity;

                    //Address
                    EntryBoxCustomerAddress.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxCustomerAddress.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //Locality
                    EntryBoxCustomerLocality.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxCustomerLocality.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //ZipCode
                    EntryBoxCustomerZipCode.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxCustomerZipCode.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //City
                    EntryBoxCustomerCity.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxCustomerCity.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //Phone
                    EntryBoxCustomerPhone.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxCustomerPhone.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //Email
                    EntryBoxCustomerEmail.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxCustomerEmail.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //Notes
                    EntryBoxCustomerNotes.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxCustomerNotes.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //Discount
                    EntryBoxCustomerDiscount.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxCustomerDiscount.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //Name
                    EntryBoxSelectCustomerName.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxSelectCustomerName.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    EntryBoxSelectCustomerName.ButtonSelectValue.Sensitive = true;
                    //Country
                    EntryBoxSelectCustomerCountry.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxSelectCustomerCountry.ButtonSelectValue.Sensitive = enableEditCustomerDetails;
                    //Always Disabled/Only Enabled in New Customer
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Sensitive = (customer == null);
                    EntryBoxSelectCustomerFiscalNumber.ButtonKeyBoard.Sensitive = (customer == null);
                    //CardNumber
                    EntryBoxSelectCustomerCardNumber.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxSelectCustomerCardNumber.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    EntryBoxSelectCustomerCardNumber.ButtonSelectValue.Sensitive = true;
                    //Validation

                    //EntryBox
                    //_entryBoxSelectCustomerName.EntryValidation.Required = true;//Always Required
                    EntryBoxSelectCustomerName.EntryValidation.Required = false;
                    EntryBoxCustomerAddress.EntryValidation.Required = false;
                    //_entryBoxCustomerLocality.EntryValidation.Required = false;
                    EntryBoxCustomerZipCode.EntryValidation.Required = false;
                    EntryBoxCustomerCity.EntryValidation.Required = false;
                    //EntryBoxSelect
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Required = false;
                }
                else
                {
                    //Enable edit User details, usefull to edit Name, Address etc
                    bool enableEditCustomerDetails = !isFinalConsumerEntity;
                    bool requiredCustomerDetails = (totalDocument >= SharedSettings.FinanceRuleRequiredCustomerDetailsAboveValue || isWayBill);

                    //EntryBox
                    EntryBoxCustomerAddress.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxCustomerLocality.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxCustomerZipCode.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxCustomerCity.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxCustomerPhone.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxCustomerEmail.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxCustomerNotes.EntryValidation.Sensitive = enableEditCustomerDetails;
                    //EntryBoxSelect
                    EntryBoxSelectCustomerName.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxSelectCustomerName.ButtonSelectValue.Sensitive = true;
                    EntryBoxSelectCustomerCountry.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxSelectCustomerCountry.ButtonSelectValue.Sensitive = true;
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Sensitive = true;
                    EntryBoxSelectCustomerFiscalNumber.ButtonSelectValue.Sensitive = true;
                    EntryBoxSelectCustomerCardNumber.EntryValidation.Sensitive = enableEditCustomerDetails;
                    EntryBoxSelectCustomerCardNumber.ButtonKeyBoard.Sensitive = true;
                    EntryBoxSelectCustomerCardNumber.ButtonSelectValue.Sensitive = true;

                    //Validation
                    //EntryBoxSelect
                    EntryBoxSelectCustomerName.EntryValidation.Required = true;
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Required = true;
                    //EntryBox
                    EntryBoxCustomerAddress.EntryValidation.Required = (!isSingularEntity || requiredCustomerDetails);
                    EntryBoxCustomerZipCode.EntryValidation.Required = (!isSingularEntity || requiredCustomerDetails);
                    EntryBoxCustomerCity.EntryValidation.Required = (!isSingularEntity || requiredCustomerDetails);
                }

                // Update Rules
                EntryBoxCustomerZipCode.EntryValidation.Rule = EntryBoxSelectCustomerCountry.Value.RegExZipCode;
                EntryBoxSelectCustomerFiscalNumber.EntryValidation.Rule = EntryBoxSelectCustomerCountry.Value.RegExFiscalNumber;

                //Always Validate All Fields

                //EntryBox
                EntryBoxSelectCustomerName.EntryValidation.Validate();
                EntryBoxCustomerAddress.EntryValidation.Validate();
                EntryBoxCustomerLocality.EntryValidation.Validate();
                EntryBoxCustomerZipCode.EntryValidation.Validate();
                EntryBoxCustomerCity.EntryValidation.Validate();
                EntryBoxCustomerPhone.EntryValidation.Validate();
                EntryBoxCustomerEmail.EntryValidation.Validate();
                EntryBoxCustomerNotes.EntryValidation.Sensitive = true;
                //EntryBoxSelect
                EntryBoxSelectCustomerName.EntryValidation.Validate();
                EntryBoxSelectCustomerCountry.EntryValidation.Validate(EntryBoxSelectCustomerCountry.Value.Oid.ToString());
                EntryBoxSelectCustomerFiscalNumber.EntryValidation.Validate();
                EntryBoxSelectCustomerCardNumber.EntryValidation.Validate();

                //Call Before ReCheck if FiscalNumber is Valid
                UpdateCustomerAddressAndFiscalNumberRequireFields();

                //ReCheck if FiscalNumber is Valid
                if (EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text != string.Empty && EntryBoxSelectCustomerFiscalNumber.EntryValidation.Validated)
                {
                    bool isValidFiscalNumber = FiscalNumber.IsValidFiscalNumber(EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text, EntryBoxSelectCustomerCountry.Value.Code2);
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Validated = isValidFiscalNumber;
                    //Disable FiscalNumber Entry
                    //_entryBoxSelectCustomerFiscalNumber.EntryValidation.Sensitive = !isValidFiscalNumber;
                    //_entryBoxSelectCustomerFiscalNumber.ButtonKeyBoard.Sensitive = !isValidFiscalNumber;
                }

                //Shared

                //If Default Customer always disable Discount
                if (isFinalConsumerEntity && EntryBoxCustomerDiscount.EntryValidation.Sensitive == true)
                {
                    EntryBoxCustomerDiscount.EntryValidation.Sensitive = false;
                }

                //Require Validate
                Validate();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //Almost Equal to PosPaymentDialog Method : Both methods have same Name
        //Update Address And FiscalNumber Require Fields
        private void UpdateCustomerAddressAndFiscalNumberRequireFields()
        {
            try
            {
                bool isRequiredAllCustomerDetails = (_pagePad3.ArticleBag != null && _pagePad3.ArticleBag.TotalFinal > SharedSettings.FinanceRuleRequiredCustomerDetailsAboveValue);
                bool isFinalConsumerEntity = (EntryBoxSelectCustomerName.Value != null && EntryBoxSelectCustomerName.Value.Oid == SharedSettings.FinalConsumerId);
                bool isSingularEntity = (
                    isFinalConsumerEntity ||
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Validated &&
                    FiscalNumber.IsSingularEntity(EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text, EntryBoxSelectCustomerCountry.Value.Code2)
                );

                //If is a SingularEntity and Not isRequiredAllCustomerDetails Disable Address, ZipCode and City
                //InTest : Added WayBill to force Address for At WebServices
                if (isSingularEntity && !_pagePad1.EntryBoxSelectDocumentFinanceType.Value.WayBill && !isRequiredAllCustomerDetails)
                {
                    EntryBoxCustomerAddress.EntryValidation.Required = false;
                    EntryBoxCustomerZipCode.EntryValidation.Required = false;
                    EntryBoxCustomerCity.EntryValidation.Required = false;
                    EntryBoxCustomerAddress.EntryValidation.Validate();
                    EntryBoxCustomerZipCode.EntryValidation.Validate();
                    EntryBoxCustomerCity.EntryValidation.Validate();
                    //_entryBoxCustomerPhone.EntryValidation.Validate();
                    //_entryBoxCustomerEmail.EntryValidation.Validate();
                }
                //if TotalFinal > SettingsApp.FinanceRuleRequiredCustomerDetailsAboveValue Required Address, ZipCode and City
                else if (isRequiredAllCustomerDetails)
                {
                    EntryBoxCustomerAddress.EntryValidation.Required = true;
                    EntryBoxCustomerZipCode.EntryValidation.Required = true;
                    EntryBoxCustomerCity.EntryValidation.Required = true;
                    EntryBoxCustomerAddress.EntryValidation.Validate();
                    EntryBoxCustomerZipCode.EntryValidation.Validate();
                    EntryBoxCustomerCity.EntryValidation.Validate();
                    //_entryBoxCustomerPhone.EntryValidation.Validate();
                    //_entryBoxCustomerEmail.EntryValidation.Validate();
                }

                //Always Required NIF or Client Name, or Both if none has Filled or ! isSingularEntity
                if (
                        (
                            EntryBoxSelectCustomerName.EntryValidation.Text == string.Empty &&
                            (
                                EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text == string.Empty
                                || !EntryBoxSelectCustomerFiscalNumber.EntryValidation.Validated
                            )
                        )
                        ||
                        (
                            !isSingularEntity
                            && EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text != string.Empty
                        )
                        ||
                        (
                            _pagePad3.ArticleBag != null && _pagePad3.ArticleBag.TotalFinal > SharedSettings.FinanceRuleRequiredCustomerDetailsAboveValue
                        )
                    )
                {
                    EntryBoxSelectCustomerName.EntryValidation.Required = true;
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Required = true;
                    EntryBoxSelectCustomerName.EntryValidation.Validate();
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Validate();
                }
                else if (EntryBoxSelectCustomerName.EntryValidation.Text == string.Empty)
                {
                    EntryBoxSelectCustomerName.EntryValidation.Required = false;
                    EntryBoxSelectCustomerName.EntryValidation.Validate();
                }
                else if (EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text == string.Empty)
                {
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Required = false;
                    EntryBoxSelectCustomerFiscalNumber.EntryValidation.Validate();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
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
                    pagePad4.EntryBoxShipToAddressDetail.EntryValidation.Text = EntryBoxCustomerAddress.EntryValidation.Text;
                    pagePad4.EntryBoxShipToRegion.EntryValidation.Text = EntryBoxCustomerLocality.EntryValidation.Text;
                    pagePad4.EntryBoxShipToPostalCode.EntryValidation.Text = EntryBoxCustomerZipCode.EntryValidation.Text;
                    pagePad4.EntryBoxShipToCity.EntryValidation.Text = EntryBoxCustomerCity.EntryValidation.Text;
                    pagePad4.EntryBoxSelectShipToCountry.EntryValidation.Text = EntryBoxSelectCustomerCountry.EntryValidation.Text;
                    pagePad4.EntryBoxSelectShipToCountry.Value = EntryBoxSelectCustomerCountry.Value;
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
                _logger.Error(ex.Message, ex);
            }
        }

        //Shared Method to store customer before change it by user, this way we compare old and news customer and clean articles
        public void StoreCurrentCustomerPriceType()
        {
            if (EntryBoxSelectCustomerName.Value != null)
            {
                _currentCustomerPriceType = (PriceType)EntryBoxSelectCustomerName.Value.PriceType.EnumValue;
            }
            else
            {
                _currentCustomerPriceType = PriceType.Price1;
            }
            //_logger.Debug(String.Format("_currentCustomerPriceType: [{0}]", _currentCustomerPriceType));
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
