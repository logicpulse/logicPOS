using DevExpress.Data.Filtering;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using logicpos.datalayer.Enums;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using DevExpress.Xpo;
using System.Collections;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    class PosDocumentFinanceArticleDialog : PosBaseDialogGenericTreeView<DataRow>
    {
        //Window
        private PosDocumentFinanceDialog _posDocumentFinanceDialog;
        //Action Buttons
        private TouchButtonIconWithText _buttonOk;
        private TouchButtonIconWithText _buttonCancel;
        //UI
        private VBox _vboxEntrys;
        private XPOEntryBoxSelectRecord<fin_article, TreeViewArticle> _entryBoxSelectArticle;
        private XPOEntryBoxSelectRecord<fin_article, TreeViewArticle> _entryBoxSelectArticleCode;
        private XPOEntryBoxSelectRecord<fin_configurationvatrate, TreeViewConfigurationVatRate> _entryBoxSelectVatRate;
        private XPOEntryBoxSelectRecord<fin_configurationvatexemptionreason, TreeViewConfigurationVatExceptionReason> _entryBoxSelectVatExemptionReason;
        private EntryBoxValidation _entryBoxValidationPrice;
        private EntryBoxValidation _entryBoxValidationPriceDisplay;
        private EntryBoxValidation _entryBoxValidationQuantity;
        private EntryBoxValidation _entryBoxValidationDiscount;
        private EntryBoxValidation _entryBoxValidationTotalNet;
        private EntryBoxValidation _entryBoxValidationTotalFinal;
        private EntryBoxValidation _entryBoxValidationToken1;
        private EntryBoxValidation _entryBoxValidationToken2;
        private EntryBoxValidation _entryBoxValidationNotes;
        //CRUDWidgetList
        private GenericCRUDWidgetListDataTable _crudWidgetList;
        //CRUDWidget
        private GenericCRUDWidgetDataTable _crudWidgetSelectArticle;
        private GenericCRUDWidgetDataTable _crudWidgetSelectArticleCode;
        private GenericCRUDWidgetDataTable _crudWidgetPrice;
        private GenericCRUDWidgetDataTable _crudWidgetPriceDisplay;
        private GenericCRUDWidgetDataTable _crudWidgetQuantity;
        private GenericCRUDWidgetDataTable _crudWidgetDiscount;
        private GenericCRUDWidgetDataTable _crudWidgetSelectVatRate;
        private GenericCRUDWidgetDataTable _crudWidgetSelectVatExemptionReason;
        //Document Types
        private fin_documentfinancetype _documentFinanceType;
        private List<string> _listSaftDocumentType = new List<string>();
        //Store Current Price without ExchangeRate, the price used in all Logic, price from Entry is only for Display
        private decimal _articlePrice = 0.0m;
        //Working Currency
        private cfg_configurationcurrency _currencyDefaultSystem;
        private cfg_configurationcurrency _currencyDisplay;
        //Customer Price Type
        private erp_customer _customer;
        //Consignation Invoice Article Default Values
        private fin_configurationvatrate _vatRateConsignationInvoice;
        private fin_configurationvatexemptionreason _vatRateConsignationInvoiceExemptionReason;
        //Logic
        private decimal _discountGlobal = 0.0m;
        public decimal DiscountGlobal
        {
            get { return _discountGlobal; }
            set { _discountGlobal = value; }
        }

        public PosDocumentFinanceArticleDialog(Window pSourceWindow, GenericTreeViewDataTable pTreeView, DialogFlags pDialogFlags, DialogMode pDialogMode, DataRow pDataSourceRow)
                    : base(pSourceWindow, pDialogFlags, pDialogMode, pDataSourceRow)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
            _dialogMode = pDialogMode;
            _dataSourceRow = pDataSourceRow;
            //References
            _posDocumentFinanceDialog = (_sourceWindow as PosDocumentFinanceDialog);
            _currencyDisplay = (_posDocumentFinanceDialog.PagePad.Pages[0] as DocumentFinanceDialogPage1).EntryBoxSelectConfigurationCurrency.Value;
            //Require to Update ExchangeRate after create Database
            _currencyDisplay.Reload();

            //Get Reference for documentFinanceType
            _documentFinanceType = ((_sourceWindow as PosDocumentFinanceDialog).PagePad.Pages[0] as DocumentFinanceDialogPage1).EntryBoxSelectDocumentFinanceType.Value;

            //Init Local Vars
            String windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_insert_articles");
            //Get Default System Currency
            _currencyDefaultSystem = SettingsApp.ConfigurationSystemCurrency;
            //Consignation Invoice default values
            _vatRateConsignationInvoice = (fin_configurationvatrate)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_configurationvatrate), SettingsApp.XpoOidConfigurationVatRateDutyFree);
            _vatRateConsignationInvoiceExemptionReason = (fin_configurationvatexemptionreason)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_configurationvatexemptionreason), SettingsApp.XpoOidConfigurationVatExemptionReasonM99);

            //TODO:THEME
            _windowSize = new Size(760, 360);

            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_finance_article.png");

            //Get Discount from Select Customer
            _discountGlobal = FrameworkUtils.StringToDecimal(((pSourceWindow as PosDocumentFinanceDialog).PagePad.Pages[1] as DocumentFinanceDialogPage2).EntryBoxCustomerDiscount.EntryValidation.Text);
            //Get PriceType from Customer
            var customerObject = ((pSourceWindow as PosDocumentFinanceDialog).PagePad.Pages[1] as DocumentFinanceDialogPage2).EntryBoxSelectCustomerName;
            if (customerObject.Value != null)
            {
                Guid customerOid = customerObject.Value.Oid;
                _customer = (erp_customer)GlobalFramework.SessionXpo.GetObjectByKey(typeof(erp_customer), customerOid);
            }

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(_buttonCancel, ResponseType.Cancel));

            //Init Content
            Fixed fixedContent = new Fixed();

            //Init Transport Documents Lists
            _listSaftDocumentType.Add(SettingsApp.XpoOidDocumentFinanceTypeDeliveryNote.ToString());
            _listSaftDocumentType.Add(SettingsApp.XpoOidDocumentFinanceTypeTransportationGuide.ToString());
            _listSaftDocumentType.Add(SettingsApp.XpoOidDocumentFinanceTypeOwnAssetsDriveGuide.ToString());
            _listSaftDocumentType.Add(SettingsApp.XpoOidDocumentFinanceTypeConsignmentGuide.ToString());
            _listSaftDocumentType.Add(SettingsApp.XpoOidDocumentFinanceTypeReturnGuide.ToString());

            //Init Components
            InitUI();

            //Put
            fixedContent.Put(_vboxEntrys, 0, 0);

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, _windowSize, fixedContent, actionAreaButtons);
        }

        private void InitUI()
        {
            //Init Local Vars

            //Default Values (INSERT)
            fin_article initialValueSelectArticle = (_dataSourceRow["Article.Code"] as fin_article);
            string initialValuePrice = FrameworkUtils.DecimalToString(0);
            string initialValuePriceDisplay = FrameworkUtils.DecimalToString(0);
            string initialValueQuantity = FrameworkUtils.DecimalToString(0);
            string initialValueDiscount = FrameworkUtils.DecimalToString(0);
            string initialValueTotalNet = FrameworkUtils.DecimalToString(0);
            string initialValueTotalFinal = FrameworkUtils.DecimalToString(0);
            string initialValueNotes = string.Empty;
            fin_configurationvatrate initialValueSelectConfigurationVatRate = (fin_configurationvatrate)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_configurationvatrate), SettingsApp.XpoOidArticleDefaultVatDirectSelling);
            fin_configurationvatexemptionreason initialValueSelectConfigurationVatExemptionReason = null;

            //Update Record : Override Default Values
            if (initialValueSelectArticle != null && initialValueSelectArticle.Oid != Guid.Empty)
            {
                //Always display Values from DataRow, for Both INSERT and UPDATE Modes, We Have defaults comming from ColumnProperties
                initialValuePrice = FrameworkUtils.StringToDecimalAndToStringAgain(_dataSourceRow["Price"].ToString());
                initialValuePriceDisplay = FrameworkUtils.StringToDecimalAndToStringAgain(_dataSourceRow["PriceDisplay"].ToString());
                initialValueQuantity = FrameworkUtils.StringToDecimalAndToStringAgain(_dataSourceRow["Quantity"].ToString());
                initialValueDiscount = FrameworkUtils.StringToDecimalAndToStringAgain(_dataSourceRow["Discount"].ToString());
                initialValueTotalNet = FrameworkUtils.StringToDecimalAndToStringAgain(_dataSourceRow["TotalNet"].ToString());
                initialValueTotalFinal = FrameworkUtils.StringToDecimalAndToStringAgain(_dataSourceRow["TotalFinal"].ToString());
                initialValueSelectConfigurationVatRate = (_dataSourceRow["ConfigurationVatRate.Value"] as fin_configurationvatrate);
                initialValueSelectConfigurationVatExemptionReason = (_dataSourceRow["VatExemptionReason.Acronym"] as fin_configurationvatexemptionreason);
                initialValueNotes = _dataSourceRow["Notes"].ToString();
                //Required, Else Wrong Calculation in UPDATES, when Price is not Defined : 
                //Reverse Price if not in default System Currency, else use value from Input
                _articlePrice = (_currencyDefaultSystem == _currencyDisplay)
                    ? FrameworkUtils.StringToDecimal(initialValuePriceDisplay)
                    : (FrameworkUtils.StringToDecimal(initialValuePriceDisplay) / _currencyDisplay.ExchangeRate)
                ;
            }

            //Initialize crudWidgetsList
            _crudWidgetList = new GenericCRUDWidgetListDataTable();
			
			//TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 
            //Select ArticleCode
            fin_article articles = null;
            SortingCollection sortCollection = new SortingCollection();
            sortCollection.Add(new SortProperty("Code", DevExpress.Xpo.DB.SortingDirection.Ascending));
            CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
            ICollection collectionCustomers = GlobalFramework.SessionXpo.GetObjects(GlobalFramework.SessionXpo.GetClassInfo(typeof(fin_article)), criteria, sortCollection, int.MaxValue, false, true);

            foreach (fin_article item in collectionCustomers)
            {
                articles = item;
            }
            articles.Code = "";
            CriteriaOperator criteriaOperatorSelectArticle = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            _entryBoxSelectArticleCode = new XPOEntryBoxSelectRecord<fin_article, TreeViewArticle>(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_article_code"), "Code", "Oid", articles, criteriaOperatorSelectArticle);
            _entryBoxSelectArticleCode.Entry.IsEditable = true;
            _entryBoxSelectArticleCode.WidthRequest = 149;

            //Add to WidgetList
            _crudWidgetSelectArticleCode = new GenericCRUDWidgetDataTable(_entryBoxSelectArticleCode, _entryBoxSelectArticleCode.Label, _dataSourceRow, "Oid", _regexGuid, true);
            _crudWidgetList.Add(_crudWidgetSelectArticleCode);
            //Used only to Update DataRow Column from Widget : Used to force Assign XPGuidObject ChildNames to Columns
            _crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxSelectArticleCode, new Label(), _dataSourceRow, "Article.Code"));
            _crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxSelectArticleCode, new Label(), _dataSourceRow, "Article.Designation"));
            //Events
            _entryBoxSelectArticleCode.ClosePopup += _entryBoxSelectArticleCode_ClosePopup;

            //Select Article Name
            _entryBoxSelectArticle = new XPOEntryBoxSelectRecord<fin_article, TreeViewArticle>(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_article"), "Designation", "Oid", initialValueSelectArticle, criteriaOperatorSelectArticle);
            _entryBoxSelectArticle.Entry.IsEditable = true;

            //Add to WidgetList
            _crudWidgetSelectArticle = new GenericCRUDWidgetDataTable(_entryBoxSelectArticle, _entryBoxSelectArticle.Label, _dataSourceRow, "Oid", _regexGuid, true);
            _crudWidgetList.Add(_crudWidgetSelectArticle);
            //Used only to Update DataRow Column from Widget : Used to force Assign XPGuidObject ChildNames to Columns
            _crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxSelectArticle, new Label(), _dataSourceRow, "Article.Code"));
            _crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxSelectArticle, new Label(), _dataSourceRow, "Article.Designation"));
            //Events
            _entryBoxSelectArticle.ClosePopup += _entryBoxSelectArticle_ClosePopup;

            //Price : Used only in Debug Mode, to Inspect SystemCurrency Values : To View add it to hboxPriceQuantityDiscountAndTotals.PackStart
            //Note #1
            //Removed. this will trigger an error when use a zero Price WayBill to issue an Invoice, better use discount 100%
            //If not Saft Document Type 2, required greater than zero in Price, else we can have zero or greater from Document Type 2 (ex Transportation Guide)
            //string regExPrice = (!_listSaftDocumentType2.Contains(_documentFinanceType.Oid.ToString())) ? _regexDecimalGreaterThanZero : _regexDecimalGreaterEqualThanZero;
            // Now all regExPrice must be greater than Zero
            string regExPrice = _regexDecimalGreaterThanZero;
            _entryBoxValidationPrice = new EntryBoxValidation(this, "Price EUR(*)", KeyboardMode.Money, regExPrice, true);
            _entryBoxValidationPrice.EntryValidation.Text = initialValuePrice;
            _entryBoxValidationPrice.EntryValidation.Sensitive = false;
            //Add to WidgetList
            _crudWidgetPrice = new GenericCRUDWidgetDataTable(_entryBoxValidationPrice, _entryBoxValidationPrice.Label, _dataSourceRow, "Price", regExPrice, true);
            _crudWidgetList.Add(_crudWidgetPrice);

            //PriceDisplay
            //Note #1
            //If not Saft Document Type 2, required greater than zero in Price, else we can have zero or greater from Document Type 2 (ex Transportation Guide)
            _entryBoxValidationPriceDisplay = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_price"), KeyboardMode.Money, regExPrice, true);
            _entryBoxValidationPriceDisplay.EntryValidation.Text = initialValuePriceDisplay;
            //Add to WidgetList
            _crudWidgetPriceDisplay = new GenericCRUDWidgetDataTable(_entryBoxValidationPriceDisplay, _entryBoxValidationPriceDisplay.Label, _dataSourceRow, "PriceDisplay", regExPrice, true);
            _crudWidgetList.Add(_crudWidgetPriceDisplay);
            //Events
            _entryBoxValidationPriceDisplay.EntryValidation.Changed += delegate
            {
                if (_crudWidgetPriceDisplay.Validated)
                {
                    //Reverse Price if not in default System Currency, else use value from Input
                    _articlePrice = (_currencyDefaultSystem == _currencyDisplay)
                        ? FrameworkUtils.StringToDecimal(_entryBoxValidationPriceDisplay.EntryValidation.Text)
                        : (FrameworkUtils.StringToDecimal(_entryBoxValidationPriceDisplay.EntryValidation.Text) / _currencyDisplay.ExchangeRate);
                    //Assign to System Currency Price
                    _entryBoxValidationPrice.EntryValidation.Text = FrameworkUtils.DecimalToString(_articlePrice);
                    UpdatePriceProperties();
                }
            };
            _entryBoxValidationPriceDisplay.EntryValidation.FocusOutEvent += delegate { _entryBoxValidationPriceDisplay.EntryValidation.Text = FrameworkUtils.StringToDecimalAndToStringAgain(_entryBoxValidationPriceDisplay.EntryValidation.Text); };

            //Start with _articlePrice Assigned: DISABLED
            //_articlePrice = (_currencyDefaultSystem == _currencyDisplay) 
            //    ? FrameworkUtils.StringToDecimal(_entryBoxValidationPriceDisplay.EntryValidation.Text) 
            //    : (FrameworkUtils.StringToDecimal(_entryBoxValidationPriceDisplay.EntryValidation.Text) / _currencyDisplay.ExchangeRate)
            //    ;

            //Quantity
            _entryBoxValidationQuantity = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_quantity"), KeyboardMode.Numeric, _regexDecimalGreaterThanZero, true);
            _entryBoxValidationQuantity.EntryValidation.Text = initialValueQuantity;
            //Add to WidgetList
            _crudWidgetQuantity = new GenericCRUDWidgetDataTable(_entryBoxValidationQuantity, _entryBoxValidationQuantity.Label, _dataSourceRow, "Quantity", _regexDecimalGreaterThanZero, true);
            _crudWidgetList.Add(_crudWidgetQuantity);
            //Events
            _entryBoxValidationQuantity.EntryValidation.Changed += delegate { UpdatePriceProperties(); };
            _entryBoxValidationQuantity.EntryValidation.FocusOutEvent += delegate { _entryBoxValidationQuantity.EntryValidation.Text = FrameworkUtils.StringToDecimalAndToStringAgain(_entryBoxValidationQuantity.EntryValidation.Text); };

            //Discount
            _entryBoxValidationDiscount = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_discount"), KeyboardMode.Numeric, _regexPercentage, true);
            _entryBoxValidationDiscount.EntryValidation.Text = initialValueDiscount;
            //Add to WidgetList
            _crudWidgetDiscount = new GenericCRUDWidgetDataTable(_entryBoxValidationDiscount, _entryBoxValidationDiscount.Label, _dataSourceRow, "Discount", _regexPercentage, true);
            _crudWidgetList.Add(_crudWidgetDiscount);
            //Events
            _entryBoxValidationDiscount.EntryValidation.Changed += delegate { UpdatePriceProperties(); };
            _entryBoxValidationDiscount.EntryValidation.FocusOutEvent += delegate { _entryBoxValidationDiscount.EntryValidation.Text = FrameworkUtils.StringToDecimalAndToStringAgain(_entryBoxValidationDiscount.EntryValidation.Text); };

            //TotalNet
            _entryBoxValidationTotalNet = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total_article_tab"), KeyboardMode.None);/* IN009206 */
            _entryBoxValidationTotalNet.EntryValidation.Text = initialValueTotalNet;
            _entryBoxValidationTotalNet.EntryValidation.Sensitive = false;
            //Used only to Update DataRow Column from Widget
            _crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxValidationTotalNet, new Label(), _dataSourceRow, "TotalNet"));

            //TotalFinal
            _entryBoxValidationTotalFinal = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total_per_item_vat"), KeyboardMode.None); /* IN009206 */
            _entryBoxValidationTotalFinal.EntryValidation.Text = initialValueTotalFinal;
            _entryBoxValidationTotalFinal.EntryValidation.Sensitive = false;
            //Used only to Update DataRow Column from Widget
            _crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxValidationTotalFinal, new Label(), _dataSourceRow, "TotalFinal"));

			//TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 
            //HBox ArticleCodeAndArticleName
            HBox ArticleCodeAndArticleName = new HBox(false, 0);
            //Invisible, only used to Debug, to View Values in System Currency
            //hboxPriceQuantityDiscountAndTotals.PackStart(_entryBoxValidationPrice, true, true, 0);
            ArticleCodeAndArticleName.PackStart(_entryBoxSelectArticleCode, false, false, 0);
            ArticleCodeAndArticleName.PackStart(_entryBoxSelectArticle, true, true, 0);


            //HBox PriceQuantityDiscountAndTotals
            HBox hboxPriceQuantityDiscountAndTotals = new HBox(true, 0);
            //Invisible, only used to Debug, to View Values in System Currency
            //hboxPriceQuantityDiscountAndTotals.PackStart(_entryBoxValidationPrice, true, true, 0);
            hboxPriceQuantityDiscountAndTotals.PackStart(_entryBoxValidationPriceDisplay, true, true, 0);
            hboxPriceQuantityDiscountAndTotals.PackStart(_entryBoxValidationQuantity, true, true, 0);
            hboxPriceQuantityDiscountAndTotals.PackStart(_entryBoxValidationDiscount, true, true, 0);
            hboxPriceQuantityDiscountAndTotals.PackStart(_entryBoxValidationTotalNet, true, true, 0);
            hboxPriceQuantityDiscountAndTotals.PackStart(_entryBoxValidationTotalFinal, true, true, 0);

            //SelectVatRate
            CriteriaOperator criteriaOperatorSelectVatRate = CriteriaOperator.Parse("(Disabled = 0 OR Disabled IS NULL)");
            _entryBoxSelectVatRate = new XPOEntryBoxSelectRecord<fin_configurationvatrate, TreeViewConfigurationVatRate>(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_vat_rate"), "Designation", "Oid", initialValueSelectConfigurationVatRate, criteriaOperatorSelectVatRate);
            _entryBoxSelectVatRate.WidthRequest = 149;
            _entryBoxSelectVatRate.Entry.IsEditable = false;
            _entryBoxSelectVatRate.Entry.Changed += _entryBoxSelectVatRate_EntryValidation_Changed;
            //Add to WidgetList
            _crudWidgetSelectVatRate = new GenericCRUDWidgetDataTable(_entryBoxSelectVatRate, _entryBoxSelectVatRate.Label, _dataSourceRow, "ConfigurationVatRate.Value", _regexGuid, true);
            _crudWidgetList.Add(_crudWidgetSelectVatRate);

            //SelectVatExemptionReason
            CriteriaOperator criteriaOperatorSelectVatExemptionReason = CriteriaOperator.Parse("(Disabled = 0 OR Disabled IS NULL)");
            _entryBoxSelectVatExemptionReason = new XPOEntryBoxSelectRecord<fin_configurationvatexemptionreason, TreeViewConfigurationVatExceptionReason>(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_vat_exemption_reason"), "Designation", "Oid", initialValueSelectConfigurationVatExemptionReason, criteriaOperatorSelectVatExemptionReason);
            _entryBoxSelectVatExemptionReason.Entry.IsEditable = false;
            //Add to WidgetList
            _crudWidgetSelectVatExemptionReason = new GenericCRUDWidgetDataTable(_entryBoxSelectVatExemptionReason, _entryBoxSelectVatExemptionReason.Label, _dataSourceRow, "VatExemptionReason.Acronym", _regexGuid, true);
            _crudWidgetList.Add(_crudWidgetSelectVatExemptionReason);
            //ToggleMode: Edit Active/Inactive
            ToggleVatExemptionReasonEditMode();

            //HBox VatRateAndVatExemptionReason
            HBox hboxVatRateAndVatExemptionReason = new HBox(false, 0);
            hboxVatRateAndVatExemptionReason.PackStart(_entryBoxSelectVatRate, false, false, 0);
            hboxVatRateAndVatExemptionReason.PackStart(_entryBoxSelectVatExemptionReason, true, true, 0);

            //Token1
            _entryBoxValidationToken1 = new EntryBoxValidation(this, "Token1", KeyboardMode.None);
            _entryBoxValidationToken1.EntryValidation.Sensitive = false;
            //Used only to Update DataRow Column from Widget
            _crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxValidationToken1, new Label(), _dataSourceRow, "Token1"));
            //Token2
            _entryBoxValidationToken2 = new EntryBoxValidation(this, "Token2", KeyboardMode.None);
            _entryBoxValidationToken2.EntryValidation.Sensitive = false;
            //Used only to Update DataRow Column from Widget
            _crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxValidationToken2, new Label(), _dataSourceRow, "Token2"));

            //Notes
            _entryBoxValidationNotes = new EntryBoxValidation(this, "Notes", KeyboardMode.AlfaNumeric, SettingsApp.RegexAlfaNumericExtended, false);
            _entryBoxValidationNotes.EntryValidation.Text = initialValueNotes;
            _crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxValidationNotes, new Label(), _dataSourceRow, "Notes"));

            //Uncomment to Show Invisible Widgets
            //HBox Token1AndToken2
            //HBox hboxToken1AndToken2 = new HBox(false, 0);
            //hboxToken1AndToken2.PackStart(_entryBoxToken1, false, false, 0);
            //hboxToken1AndToken2.PackStart(_entryBoxToken2, true, true, 0);

            //Pack in VBox
            _vboxEntrys = new VBox(true, 0);
            _vboxEntrys.PackStart(ArticleCodeAndArticleName);
            _vboxEntrys.PackStart(hboxPriceQuantityDiscountAndTotals);
            _vboxEntrys.PackStart(hboxVatRateAndVatExemptionReason);
            //Uncomment to Show Invisible Widgets
            //_vboxEntrys.PackStart(hboxToken1AndToken2);
            _vboxEntrys.PackStart(_entryBoxValidationNotes);
            _vboxEntrys.WidthRequest = _windowSize.Width - 13;

            // CreditNote : Protect all components, only Quantity is Editable in CreditMode
            if (_documentFinanceType.Oid == SettingsApp.XpoOidDocumentFinanceTypeCreditNote)
            {
                //Article
                _entryBoxSelectArticle.Entry.Sensitive = false;
                _entryBoxSelectArticle.ButtonSelectValue.Sensitive = false;
                //PriceDisplay
                _entryBoxValidationPriceDisplay.EntryValidation.Sensitive = false;
                _entryBoxValidationPriceDisplay.ButtonKeyBoard.Sensitive = false;
                //Discount
                _entryBoxValidationDiscount.EntryValidation.Sensitive = false;
                _entryBoxValidationDiscount.ButtonKeyBoard.Sensitive = false;
                //VatRate
                _entryBoxSelectVatRate.Entry.Sensitive = false;
                _entryBoxSelectVatRate.ButtonSelectValue.Sensitive = false;
                //VatExemptionReason
                _entryBoxSelectVatExemptionReason.Entry.Sensitive = false;
                _entryBoxSelectVatExemptionReason.ButtonSelectValue.Sensitive = false;
            }
        }

        protected override void OnResponse(ResponseType pResponse)
        {
            // Call ValidateMaxQuantities before 
            if (pResponse == ResponseType.Ok && !ValidateMaxArticleQuantity())
            {
                this.Run();
            }
            else
            {
                _crudWidgetList.ProcessDialogResponse(this, _dialogMode, pResponse);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events
        private void _entryBoxSelectVatRate_EntryValidation_Changed(object sender, EventArgs e)
        {
            ToggleVatExemptionReasonEditMode();
            //Update Price Properties
            UpdatePriceProperties();
        }
       

		//TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 
        void _entryBoxSelectArticleCode_ClosePopup(object sender, EventArgs e)
        {
            //Prepare Objects
            fin_article article = _entryBoxSelectArticleCode.Value;
            fin_configurationpricetype configurationPriceTypeDefault = (fin_configurationpricetype)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_configurationpricetype), SettingsApp.XpoOidConfigurationPriceTypeDefault);

            _entryBoxSelectArticleCode.Entry.Text = article.Code;
            _entryBoxSelectArticle.Entry.Text = article.Designation;
            //Get PriceType from Customer.PriceType or from default if a New Customer or a Customer without PriceType Defined in BackOffice, always revert to Price1
            PriceType priceType = (_customer != null)
                ? (PriceType)_customer.PriceType.EnumValue
                : (PriceType)configurationPriceTypeDefault.EnumValue
            ;

            //Common changes for MediaNova | Non-MediaNova Articles | Here Prices are always in Retail Mode
            PriceProperties priceProperties = FrameworkUtils.GetArticlePrice(article, priceType, TaxSellType.Normal);

            //Price
            _articlePrice = priceProperties.PriceNet;
            //Display Price
            _entryBoxValidationPrice.EntryValidation.Text = FrameworkUtils.DecimalToString(_articlePrice);
            _entryBoxValidationPriceDisplay.EntryValidation.Text = FrameworkUtils.DecimalToString(_articlePrice * _currencyDisplay.ExchangeRate);
            _entryBoxValidationQuantity.EntryValidation.Text = (article.DefaultQuantity > 0) ? FrameworkUtils.DecimalToString(article.DefaultQuantity) : FrameworkUtils.DecimalToString(1.0m);
            _entryBoxValidationDiscount.EntryValidation.Text = FrameworkUtils.DecimalToString(article.Discount);

            //VatRate
            _entryBoxSelectVatRate.Value = article.VatDirectSelling;
            _entryBoxSelectVatRate.Entry.Text = article.VatDirectSelling.Designation;

            //Default Vat Exception Reason
            if (article.VatExemptionReason != null)
            {
                _entryBoxSelectVatExemptionReason.Value = article.VatExemptionReason;
                _entryBoxSelectVatExemptionReason.Entry.Text = article.VatExemptionReason.Designation;
            }


            //Toggle ToggleVatExemptionReasonEditMode Validation
            ToggleVatExemptionReasonEditMode();
            //Update Price Properties
            UpdatePriceProperties();
        }




        void _entryBoxSelectArticle_ClosePopup(object sender, EventArgs e)
        {
            //Prepare Objects


            fin_article article = _entryBoxSelectArticle.Value;
            fin_configurationpricetype configurationPriceTypeDefault = (fin_configurationpricetype)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_configurationpricetype), SettingsApp.XpoOidConfigurationPriceTypeDefault);


            _entryBoxSelectArticleCode.Entry.Text = article.Code;
            _entryBoxSelectArticle.Entry.Text = article.Designation;
            //Get PriceType from Customer.PriceType or from default if a New Customer or a Customer without PriceType Defined in BackOffice, always revert to Price1
            PriceType priceType = (_customer != null)
                ? (PriceType)_customer.PriceType.EnumValue
                : (PriceType)configurationPriceTypeDefault.EnumValue
            ;

            //Common changes for MediaNova | Non-MediaNova Articles | Here Prices are always in Retail Mode
            PriceProperties priceProperties = FrameworkUtils.GetArticlePrice(article, priceType, TaxSellType.Normal);

            //Price
            _articlePrice = priceProperties.PriceNet;
            //Display Price
            _entryBoxValidationPrice.EntryValidation.Text = FrameworkUtils.DecimalToString(_articlePrice);
            _entryBoxValidationPriceDisplay.EntryValidation.Text = FrameworkUtils.DecimalToString(_articlePrice * _currencyDisplay.ExchangeRate);
            _entryBoxValidationQuantity.EntryValidation.Text = (article.DefaultQuantity > 0) ? FrameworkUtils.DecimalToString(article.DefaultQuantity) : FrameworkUtils.DecimalToString(1.0m);
            _entryBoxValidationDiscount.EntryValidation.Text = FrameworkUtils.DecimalToString(article.Discount);

            //VatRate
            _entryBoxSelectVatRate.Value = article.VatDirectSelling;
            _entryBoxSelectVatRate.Entry.Text = article.VatDirectSelling.Designation;

            //Default Vat Exception Reason
            if (article.VatExemptionReason != null)
            {
                _entryBoxSelectVatExemptionReason.Value = article.VatExemptionReason;
                _entryBoxSelectVatExemptionReason.Entry.Text = article.VatExemptionReason.Designation;
            }

            //Toggle ToggleVatExemptionReasonEditMode Validation
            ToggleVatExemptionReasonEditMode();
            //Update Price Properties
            UpdatePriceProperties();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        private void ToggleVatExemptionReasonEditMode()
        {
            //Default Mode
            if (_documentFinanceType.Oid != SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice)
            {
                if (_entryBoxSelectVatRate.Value != null && _entryBoxSelectVatRate.Value.Oid == SettingsApp.XpoOidConfigurationVatRateDutyFree)
                {
                    _entryBoxSelectVatExemptionReason.Entry.Sensitive = true;
                    _entryBoxSelectVatExemptionReason.ButtonSelectValue.Sensitive = true;
                    //Use Reference to change Validation
                    _crudWidgetSelectVatExemptionReason.Required = true;
                    _crudWidgetSelectVatExemptionReason.ValidationRule = _regexGuid;
                }
                else
                {
                    _entryBoxSelectVatExemptionReason.Value = null;
                    _entryBoxSelectVatExemptionReason.Entry.Text = string.Empty;
                    _entryBoxSelectVatExemptionReason.Entry.Sensitive = false;
                    _entryBoxSelectVatExemptionReason.ButtonSelectValue.Sensitive = false;
                    //Use Reference to change Validation
                    _crudWidgetSelectVatExemptionReason.ValidationRule = string.Empty;
                    _crudWidgetSelectVatExemptionReason.Required = false;
                }
            }
            //Consignation Invoice Mode - Disable _entryBoxSelectVatRate and _entryBoxSelectVatExemptionReason and Assign Default Values
            else
            {
                _entryBoxSelectVatRate.Entry.Sensitive = false;
                _entryBoxSelectVatRate.Entry.Text = _vatRateConsignationInvoice.Designation;
                _entryBoxSelectVatRate.ButtonSelectValue.Sensitive = false;
                _entryBoxSelectVatRate.Value = _vatRateConsignationInvoice;

                _entryBoxSelectVatExemptionReason.Entry.Sensitive = false;
                _entryBoxSelectVatExemptionReason.Entry.Text = _vatRateConsignationInvoiceExemptionReason.Designation;
                _entryBoxSelectVatExemptionReason.ButtonSelectValue.Sensitive = false;
                _entryBoxSelectVatExemptionReason.Value = _vatRateConsignationInvoiceExemptionReason;
            }

            //Use Reference to change Validate
            _crudWidgetSelectVatExemptionReason.ValidateField();
        }

        private void UpdatePriceProperties()
        {
            if (_crudWidgetPrice.Validated && _crudWidgetQuantity.Validated && _crudWidgetDiscount.Validated && _crudWidgetSelectVatRate.Validated)
            {
                PriceProperties priceProperties = PriceProperties.GetPriceProperties(
                    PricePropertiesSourceMode.FromPriceUser,
                    false, //PriceWithVat : Always use PricesWithoutVat in Invoices
                    _articlePrice,
                    FrameworkUtils.StringToDecimal(_entryBoxValidationQuantity.EntryValidation.Text),
                    FrameworkUtils.StringToDecimal(_entryBoxValidationDiscount.EntryValidation.Text),
                    _discountGlobal,
                    _entryBoxSelectVatRate.Value.Value
                );
                //priceProperties.SendToLog(article.Designation);

                //Update UI / Display with ExchangeRate
				/* IN009235 */
                _entryBoxValidationTotalNet.EntryValidation.Text = FrameworkUtils.DecimalToString(priceProperties.TotalNetBeforeDiscountGlobal * _currencyDisplay.ExchangeRate);
                _entryBoxValidationTotalFinal.EntryValidation.Text = FrameworkUtils.DecimalToString(priceProperties.TotalFinalBeforeDiscountGlobal * _currencyDisplay.ExchangeRate);
            }
            else
            {
                _entryBoxValidationTotalNet.EntryValidation.Text = FrameworkUtils.DecimalToString(0.0m);
                _entryBoxValidationTotalFinal.EntryValidation.Text = FrameworkUtils.DecimalToString(0.0m);
            }
        }

        private bool ValidateMaxArticleQuantity()
        {
            bool result = true;
            string invalidArticleQuantity = string.Empty;

            try
            {
                /* IN009171 - when receiving a '.' on it, converter throws "System.FormatException" */
                string quantity = _entryBoxValidationQuantity.EntryValidation.Text;
                if (!string.IsNullOrEmpty(quantity))
                {
                    quantity = FrameworkUtils.DecimalToString(FrameworkUtils.StringToDecimal(quantity));
                    quantity = quantity.Replace('.', ',');
                }
                decimal currentQuantity = Convert.ToDecimal(quantity);
                // Validate Max Quantities only if ValidateMaxQuantities is defined
                if (_posDocumentFinanceDialog.ValidateMaxQuantities != null && _posDocumentFinanceDialog.ValidateMaxQuantities.Count > 0)
                {
                    decimal maxPossibleQuantity = _posDocumentFinanceDialog.ValidateMaxQuantities[_entryBoxSelectArticle.Value.Oid];

                    if (currentQuantity > maxPossibleQuantity)
                    {
                        _log.Debug(string.Format("CurrentQuantity: [{0}] is Greater than MaxPossibleQuantity: [{1}]", currentQuantity, maxPossibleQuantity));
                        result = false;
                    }

                    // Check if has Errrors, and Show Error Message
                    if (!result)
                    {
                        // Show Message
                        Utils.ShowMessageTouchErrorTryToIssueACreditNoteExceedingSourceDocumentArticleQuantities(this, currentQuantity, maxPossibleQuantity);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("bool ValidateMaxArticleQuantity() :: _entryBoxValidationQuantity.EntryValidation.Text [ " + _entryBoxValidationQuantity.EntryValidation.Text + " ]: " + ex.Message, ex);
            }

            return result;
        }
    }
}
