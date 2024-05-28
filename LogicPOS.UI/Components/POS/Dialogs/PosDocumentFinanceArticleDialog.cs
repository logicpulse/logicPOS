using DevExpress.Data.Filtering;
using Gtk;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.Enums;
using logicpos.shared.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using DevExpress.Xpo;
using System.Collections;
using logicpos.datalayer.Xpo;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Shared.CustomDocument;
using LogicPOS.Shared.Article;
using LogicPOS.Data.XPO.Settings;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosDocumentFinanceArticleDialog : PosBaseDialogGenericTreeView<DataRow>
    {
        //Window
        private readonly PosDocumentFinanceDialog _posDocumentFinanceDialog;
        //Action Buttons
        private readonly TouchButtonIconWithText _buttonOk;
        private readonly TouchButtonIconWithText _buttonCancel;
        private readonly TouchButtonIconWithText _buttonClear;
        //UI
        private VBox _vboxEntrys;
        private XPOEntryBoxSelectRecord<fin_article, TreeViewArticle> _entryBoxSelectArticle;
        private XPOEntryBoxSelectRecord<fin_article, TreeViewArticle> _entryBoxSelectArticleCode;
        private XPOEntryBoxSelectRecord<fin_articlewarehouse, TreeViewArticleWarehouse> _entryBoxSelectArticleWarehouse;
        private XPOEntryBoxSelectRecord<fin_articlefamily, TreeViewArticleFamily> _entryBoxSelectArticleFamily;
        private XPOEntryBoxSelectRecord<fin_articlesubfamily, TreeViewArticleSubFamily> _entryBoxSelectArticleSubFamily;
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
        private GenericCRUDWidgetDataTable _crudWidgetSelectArticleSubFamily;
        private GenericCRUDWidgetDataTable _crudWidgetSelectArticleFamily;

        //Document Types
        private readonly fin_documentfinancetype _documentFinanceType;
        private readonly List<string> _listSaftDocumentType = new List<string>();
        //Store Current Price without ExchangeRate, the price used in all Logic, price from Entry is only for Display
        private decimal _articlePrice = 0.0m;

        //Working Currency
        private readonly cfg_configurationcurrency _currencyDefaultSystem;
        private readonly cfg_configurationcurrency _currencyDisplay;
        //Customer Price Type
        private readonly erp_customer _customer;

        //New Article
        private fin_article _article;
        private fin_articlewarehouse _articleWarehouse;

        //Consignation Invoice Article Default Values
        private readonly fin_configurationvatrate _vatRateConsignationInvoice;
        private readonly fin_configurationvatexemptionreason _vatRateConsignationInvoiceExemptionReason;

        public decimal DiscountGlobal { get; set; } = 0.0m;

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
            string windowTitle = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_insert_articles");
            //Get Default System Currency
            _currencyDefaultSystem = XPOSettings.ConfigurationSystemCurrency;
            //Consignation Invoice default values
            _vatRateConsignationInvoice = (fin_configurationvatrate)XPOSettings.Session.GetObjectByKey(typeof(fin_configurationvatrate), InvoiceSettings.XpoOidConfigurationVatRateDutyFree);
            _vatRateConsignationInvoiceExemptionReason = (fin_configurationvatexemptionreason)XPOSettings.Session.GetObjectByKey(typeof(fin_configurationvatexemptionreason), InvoiceSettings.XpoOidConfigurationVatExemptionReasonM99);

            //TODO:THEME
            _windowSize = new Size(900, 360);

            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_finance_article.png";

            //Get Discount from Select Customer
            DiscountGlobal = LogicPOS.Utility.DataConversionUtils.StringToDecimal(((pSourceWindow as PosDocumentFinanceDialog).PagePad.Pages[1] as DocumentFinanceDialogPage2).EntryBoxCustomerDiscount.EntryValidation.Text);
            //Get PriceType from Customer
            var customerObject = ((pSourceWindow as PosDocumentFinanceDialog).PagePad.Pages[1] as DocumentFinanceDialogPage2).EntryBoxSelectCustomerName;
            if (customerObject.Value != null)
            {
                Guid customerOid = customerObject.Value.Oid;
                _customer = (erp_customer)XPOSettings.Session.GetObjectByKey(typeof(erp_customer), customerOid);
            }

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
            _buttonClear = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.CleanFilter);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(_buttonOk, ResponseType.Ok),
                new ActionAreaButton(_buttonCancel, ResponseType.Cancel),
                new ActionAreaButton(_buttonClear, ResponseType.DeleteEvent)
            };

            //Init Content
            Fixed fixedContent = new Fixed();

            //Init Transport Documents Lists
            _listSaftDocumentType.Add(CustomDocumentSettings.DeliveryNoteDocumentTypeId.ToString());
            _listSaftDocumentType.Add(CustomDocumentSettings.TransportDocumentTypeId.ToString());
            _listSaftDocumentType.Add(DocumentSettings.XpoOidDocumentFinanceTypeOwnAssetsDriveGuide.ToString());
            _listSaftDocumentType.Add(DocumentSettings.XpoOidDocumentFinanceTypeConsignmentGuide.ToString());
            _listSaftDocumentType.Add(DocumentSettings.XpoOidDocumentFinanceTypeReturnGuide.ToString());

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

            string initialValuePrice = LogicPOS.Utility.DataConversionUtils.DecimalToString(0);
            string initialValuePriceDisplay = LogicPOS.Utility.DataConversionUtils.DecimalToString(0);
            string initialValueQuantity = LogicPOS.Utility.DataConversionUtils.DecimalToString(0);
            string initialValueDiscount = LogicPOS.Utility.DataConversionUtils.DecimalToString(0);
            string initialValueTotalNet = LogicPOS.Utility.DataConversionUtils.DecimalToString(0);
            string initialValueTotalFinal = LogicPOS.Utility.DataConversionUtils.DecimalToString(0);
            string initialValueNotes = string.Empty;
            fin_configurationvatrate initialValueSelectConfigurationVatRate = (fin_configurationvatrate)XPOSettings.Session.GetObjectByKey(typeof(fin_configurationvatrate), XPOSettings.XpoOidArticleDefaultVatDirectSelling);
            fin_configurationvatexemptionreason initialValueSelectConfigurationVatExemptionReason = null;

            //Update Record : Override Default Values
            if (initialValueSelectArticle != null && initialValueSelectArticle.Oid != Guid.Empty)
            {
                //Always display Values from DataRow, for Both INSERT and UPDATE Modes, We Have defaults comming from ColumnProperties
                initialValuePrice = LogicPOS.Utility.DataConversionUtils.StringToDecimalAndToStringAgain(_dataSourceRow["Price"].ToString());
                initialValuePriceDisplay = LogicPOS.Utility.DataConversionUtils.StringToDecimalAndToStringAgain(_dataSourceRow["PriceDisplay"].ToString());
                initialValueQuantity = LogicPOS.Utility.DataConversionUtils.StringToDecimalAndToStringAgain(_dataSourceRow["Quantity"].ToString());
                initialValueDiscount = LogicPOS.Utility.DataConversionUtils.StringToDecimalAndToStringAgain(_dataSourceRow["Discount"].ToString());
                initialValueTotalNet = LogicPOS.Utility.DataConversionUtils.StringToDecimalAndToStringAgain(_dataSourceRow["TotalNet"].ToString());
                initialValueTotalFinal = LogicPOS.Utility.DataConversionUtils.StringToDecimalAndToStringAgain(_dataSourceRow["TotalFinal"].ToString());
                initialValueSelectConfigurationVatRate = (_dataSourceRow["ConfigurationVatRate.Value"] as fin_configurationvatrate);
                initialValueSelectConfigurationVatExemptionReason = (_dataSourceRow["VatExemptionReason.Acronym"] as fin_configurationvatexemptionreason);
                initialValueNotes = _dataSourceRow["Notes"].ToString();
                //Required, Else Wrong Calculation in UPDATES, when Price is not Defined : 
                //Reverse Price if not in default System Currency, else use value from Input
                _articlePrice = (_currencyDefaultSystem == _currencyDisplay)
                    ? LogicPOS.Utility.DataConversionUtils.StringToDecimal(initialValuePriceDisplay)
                    : (LogicPOS.Utility.DataConversionUtils.StringToDecimal(initialValuePriceDisplay) / _currencyDisplay.ExchangeRate)
                ;
            }

            //Front-End - Adicionar artigos na criação de Documentos [IN:010335]

            //Initialize crudWidgetsList
            _crudWidgetList = new GenericCRUDWidgetListDataTable();

            //TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 
            //Select ArticleCode

            SortingCollection sortCollection = new SortingCollection
            {
                new SortProperty("Code", DevExpress.Xpo.DB.SortingDirection.Ascending)
            };
            CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
            ICollection collectionCustomers = XPOSettings.Session.GetObjects(XPOSettings.Session.GetClassInfo(typeof(fin_article)), criteria, sortCollection, int.MaxValue, false, true);
            //_article = new fin_article(XPOSettings.Session);
            ////foreach (fin_article item in collectionCustomers)
            ////{
            ////    articles = item;
            ////}
            //_article.Code = "";
            if (initialValueSelectArticle != null) _article = initialValueSelectArticle;
            if(_dataSourceRow["Warehouse"] != null && _dataSourceRow["SerialNumber"] != null)
            {
                var sql = string.Format("SELECT ArticleWarehouse FROM fin_articleserialnumber WHERE SerialNumber = '{0}';", _dataSourceRow["SerialNumber"].ToString());
                var Oid = _article.Session.ExecuteScalar(sql);
                if (Oid != null) _articleWarehouse = (fin_articlewarehouse)_article.Session.GetObjectByKey(typeof(fin_articlewarehouse), Guid.Parse(Oid.ToString()));
            }
            else if(_dataSourceRow["Warehouse"] != null)
            {
                var sql = string.Format("SELECT Oid FROM ArticleWarehouse WHERE Location = '{0}' AND Article = '{1}';", _dataSourceRow["Warehouse"].ToString(), _article);
                var Oid = _article.Session.ExecuteScalar(sql);
                if (Oid != null) _articleWarehouse = (fin_articlewarehouse)_article.Session.GetObjectByKey(typeof(fin_articlewarehouse), Guid.Parse(Oid.ToString()));
            }
            
            if(_articleWarehouse == null) _articleWarehouse = new fin_articlewarehouse(_article.Session);

            CriteriaOperator criteriaOperatorSelectArticle = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            _entryBoxSelectArticleCode = new XPOEntryBoxSelectRecord<fin_article, TreeViewArticle>(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_code"), "Code", "Oid", initialValueSelectArticle, criteriaOperatorSelectArticle);
            _entryBoxSelectArticleCode.Entry.IsEditable = true;
            _entryBoxSelectArticleCode.WidthRequest = 149;

            _crudWidgetSelectArticleCode = new GenericCRUDWidgetDataTable(_entryBoxSelectArticleCode, _entryBoxSelectArticleCode.Label, _dataSourceRow, "article.Code", _regexAlfaNumeric, true);
            _crudWidgetList.Add(_crudWidgetSelectArticleCode);

            CriteriaOperator criteriaOperatorSelectArticleWarehouse = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1) AND (Quantity > 0)");
            _entryBoxSelectArticleWarehouse = new XPOEntryBoxSelectRecord<fin_articlewarehouse, TreeViewArticleWarehouse>(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_serial_number"), "ArticleSerialNumber", "Oid", _articleWarehouse, criteriaOperatorSelectArticleWarehouse);
            _entryBoxSelectArticleWarehouse.Entry.IsEditable = true;            
            _entryBoxSelectArticleWarehouse.WidthRequest = 149;
            //_crudWidgetSelectArticleSerialNumber = new GenericCRUDWidgetDataTable(_entryBoxSelectArticleSerialNumber, _entryBoxSelectArticleSerialNumber.Label, _dataSourceRow, "SerialNumber", _regexAlfaNumericExtended, false);
            //_crudWidgetList.Add(_crudWidgetSelectArticleSerialNumber);

            CriteriaOperator criteriaOperatorSelectArticleFamily = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            _entryBoxSelectArticleFamily = new XPOEntryBoxSelectRecord<fin_articlefamily, TreeViewArticleFamily>(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_family"), "Designation", "Oid", initialValueSelectArticle.Family, criteriaOperatorSelectArticle);
            _entryBoxSelectArticleFamily.Entry.IsEditable = false;
            _entryBoxSelectArticleFamily.Entry.Sensitive = false;
            _entryBoxSelectArticleFamily.WidthRequest = 160;
            _entryBoxSelectArticleFamily.ClosePopup += _entryBoxSelectArticleFamily_ClosePopup;
                        
            _crudWidgetSelectArticleFamily = new GenericCRUDWidgetDataTable(_entryBoxSelectArticleFamily, _entryBoxSelectArticleFamily.Label, _dataSourceRow, "article.Family", _regexAlfaNumericExtended, true);
            _crudWidgetList.Add(_crudWidgetSelectArticleFamily);

            fin_articlesubfamily initialValueSelectArticleSubFamily = (_dataSourceRow["Article.SubFamily"] as fin_articlesubfamily);
            CriteriaOperator criteriaOperatorSelectArticleSubFamily = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");            
            _entryBoxSelectArticleSubFamily = new XPOEntryBoxSelectRecord<fin_articlesubfamily, TreeViewArticleSubFamily>(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_subfamily"), "Designation", "Oid", initialValueSelectArticle.SubFamily, criteriaOperatorSelectArticle);
            _entryBoxSelectArticleSubFamily.Entry.IsEditable = false;
            _entryBoxSelectArticleSubFamily.Entry.Sensitive = false;
            _entryBoxSelectArticleSubFamily.WidthRequest = 160;
            _entryBoxSelectArticleSubFamily.ClosePopup += _entryBoxSelectArticleSubFamily_ClosePopup;
            _entryBoxSelectArticleSubFamily.ButtonSelectValue.Sensitive = false;
            
            _crudWidgetSelectArticleSubFamily = new GenericCRUDWidgetDataTable(_entryBoxSelectArticleSubFamily, _entryBoxSelectArticleSubFamily.Label, _dataSourceRow, "article.Subfamily", _regexAlfaNumericExtended, true);
            _crudWidgetList.Add(_crudWidgetSelectArticleSubFamily);

            if(initialValueSelectArticle.Family != null && initialValueSelectArticle.SubFamily != null)
            {
                _entryBoxSelectArticleFamily.Entry.Text = initialValueSelectArticle.Family.Designation;
                _entryBoxSelectArticleSubFamily.Entry.Text = initialValueSelectArticle.SubFamily.Designation;

                _entryBoxSelectArticleFamily.ButtonSelectValue.Sensitive = false;
            }

            //Add to WidgetList
            //_crudWidgetSelectArticleCode = new GenericCRUDWidgetDataTable(_entryBoxSelectArticleCode, _entryBoxSelectArticleCode.Label, _dataSourceRow, "Oid", _regexGuid, true);
            //_crudWidgetList.Add(_crudWidgetSelectArticleCode);

                //Events
                _entryBoxSelectArticleCode.ClosePopup += _entryBoxSelectArticleCode_ClosePopup;
            _entryBoxSelectArticleWarehouse.ClosePopup += _entryBoxSelectArticleCode_ClosePopup;

            //Select Article Name
            _entryBoxSelectArticle = new XPOEntryBoxSelectRecord<fin_article, TreeViewArticle>(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article"), "Designation", "Oid", initialValueSelectArticle, criteriaOperatorSelectArticle);
            _entryBoxSelectArticle.Entry.IsEditable = true;

            //Add to WidgetList
            _crudWidgetSelectArticle = new GenericCRUDWidgetDataTable(_entryBoxSelectArticle, _entryBoxSelectArticle.Label, _dataSourceRow, "article.Designation", _regexAlfaNumericExtended, true);
            _crudWidgetList.Add(_crudWidgetSelectArticle);


            ////Used only to Update DataRow Column from Widget : Used to force Assign XPGuidObject ChildNames to Columns
            //_crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxSelectArticleCode, new Label(), _dataSourceRow, "Article.Code"));
            //_crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxSelectArticle, new Label(), _dataSourceRow, "Article.Designation"));
            //_crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxSelectArticleFamily, new Label(), _dataSourceRow, "Article.Family"));
            //_crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxSelectArticleSubFamily, new Label(), _dataSourceRow, "Article.SubFamily"));

            ////Used only to Update DataRow Column from Widget : Used to force Assign XPGuidObject ChildNames to Columns
            //_crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxSelectArticle, new Label(), _dataSourceRow, "Article.Code"));
            //_crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxSelectArticle, new Label(), _dataSourceRow, "Article.Designation"));
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
            _entryBoxValidationPriceDisplay = new EntryBoxValidation(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_price"), KeyboardMode.Money, regExPrice, true);
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
                        ? LogicPOS.Utility.DataConversionUtils.StringToDecimal(_entryBoxValidationPriceDisplay.EntryValidation.Text)
                        : (LogicPOS.Utility.DataConversionUtils.StringToDecimal(_entryBoxValidationPriceDisplay.EntryValidation.Text) / _currencyDisplay.ExchangeRate);
                    //Assign to System Currency Price
                    _entryBoxValidationPrice.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(_articlePrice);
                    UpdatePriceProperties();
                }
            };
            _entryBoxValidationPriceDisplay.EntryValidation.FocusOutEvent += delegate { _entryBoxValidationPriceDisplay.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.StringToDecimalAndToStringAgain(_entryBoxValidationPriceDisplay.EntryValidation.Text); };

            //Start with _articlePrice Assigned: DISABLED
            //_articlePrice = (_currencyDefaultSystem == _currencyDisplay) 
            //    ? FrameworkUtils.StringToDecimal(_entryBoxValidationPriceDisplay.EntryValidation.Text) 
            //    : (FrameworkUtils.StringToDecimal(_entryBoxValidationPriceDisplay.EntryValidation.Text) / _currencyDisplay.ExchangeRate)
            //    ;

            //Quantity
            _entryBoxValidationQuantity = new EntryBoxValidation(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity"), KeyboardMode.Numeric, _regexDecimalGreaterThanZero, true);
            _entryBoxValidationQuantity.EntryValidation.Text = initialValueQuantity;
            //Add to WidgetList
            _crudWidgetQuantity = new GenericCRUDWidgetDataTable(_entryBoxValidationQuantity, _entryBoxValidationQuantity.Label, _dataSourceRow, "Quantity", _regexDecimalGreaterThanZero, true);
            _crudWidgetList.Add(_crudWidgetQuantity);
            //Events
            _entryBoxValidationQuantity.EntryValidation.Changed += delegate
            {
                try
                {                    
                    UpdatePriceProperties();
                }
                catch (Exception Ex)
                {
                    _logger.Error("Error updating article quantity :" + Ex.Message);
                }
            };
            _entryBoxValidationQuantity.EntryValidation.FocusOutEvent += delegate
            {
                try
                {
                    _entryBoxValidationQuantity.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.StringToDecimalAndToStringAgain(_entryBoxValidationQuantity.EntryValidation.Text);
                }
                catch (Exception Ex)
                {
                    _logger.Error("Error updating article quantity :" + Ex.Message);
                }
            };

            //Discount
            _entryBoxValidationDiscount = new EntryBoxValidation(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_discount"), KeyboardMode.Numeric, _regexPercentage, true);
            _entryBoxValidationDiscount.EntryValidation.Text = initialValueDiscount;
            //Add to WidgetList
            _crudWidgetDiscount = new GenericCRUDWidgetDataTable(_entryBoxValidationDiscount, _entryBoxValidationDiscount.Label, _dataSourceRow, "Discount", _regexPercentage, true);
            _crudWidgetList.Add(_crudWidgetDiscount);
            //Events
            _entryBoxValidationDiscount.EntryValidation.Changed += delegate { UpdatePriceProperties(); };
            _entryBoxValidationDiscount.EntryValidation.FocusOutEvent += delegate { _entryBoxValidationDiscount.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.StringToDecimalAndToStringAgain(_entryBoxValidationDiscount.EntryValidation.Text); };

            //TotalNet
            _entryBoxValidationTotalNet = new EntryBoxValidation(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_article_tab"), KeyboardMode.None);/* IN009206 */
            _entryBoxValidationTotalNet.EntryValidation.Text = initialValueTotalNet;
            _entryBoxValidationTotalNet.EntryValidation.Sensitive = false;
            //Used only to Update DataRow Column from Widget
            _crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxValidationTotalNet, new Label(), _dataSourceRow, "TotalNet"));

            //TotalFinal
            _entryBoxValidationTotalFinal = new EntryBoxValidation(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_per_item_vat"), KeyboardMode.None); /* IN009206 */
            _entryBoxValidationTotalFinal.EntryValidation.Text = initialValueTotalFinal;
            _entryBoxValidationTotalFinal.EntryValidation.Sensitive = false;
            //Used only to Update DataRow Column from Widget
            _crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxValidationTotalFinal, new Label(), _dataSourceRow, "TotalFinal"));

            //TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 
            //HBox ArticleCodeAndArticleName
            HBox ArticleCodeAndArticleName = new HBox(false, 0);
            //Invisible, only used to Debug, to View Values in System Currency
            //hboxPriceQuantityDiscountAndTotals.PackStart(_entryBoxValidationPrice, true, true, 0);
            if(GeneralSettings.AppUseBackOfficeMode && LicenseSettings.LicenseModuleStocks) ArticleCodeAndArticleName.PackStart(_entryBoxSelectArticleWarehouse, false, false, 0); 
            ArticleCodeAndArticleName.PackStart(_entryBoxSelectArticleCode, false, false, 0);
            ArticleCodeAndArticleName.PackStart(_entryBoxSelectArticle, true, true, 0);
            ArticleCodeAndArticleName.PackStart(_entryBoxSelectArticleFamily, false, false, 0);
            ArticleCodeAndArticleName.PackStart(_entryBoxSelectArticleSubFamily, false, false, 0);


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
            _entryBoxSelectVatRate = new XPOEntryBoxSelectRecord<fin_configurationvatrate, TreeViewConfigurationVatRate>(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_vat_rate"), "Designation", "Oid", initialValueSelectConfigurationVatRate, criteriaOperatorSelectVatRate);
            _entryBoxSelectVatRate.WidthRequest = 149;
            _entryBoxSelectVatRate.Entry.Changed += _entryBoxSelectVatRate_EntryValidation_Changed;
            //Add to WidgetList
            _crudWidgetSelectVatRate = new GenericCRUDWidgetDataTable(_entryBoxSelectVatRate, _entryBoxSelectVatRate.Label, _dataSourceRow, "ConfigurationVatRate.Value", _regexGuid, true);
            _crudWidgetList.Add(_crudWidgetSelectVatRate);

            //SelectVatExemptionReason
            CriteriaOperator criteriaOperatorSelectVatExemptionReason = CriteriaOperator.Parse("(Disabled = 0 OR Disabled IS NULL)");
            _entryBoxSelectVatExemptionReason = new XPOEntryBoxSelectRecord<fin_configurationvatexemptionreason, TreeViewConfigurationVatExceptionReason>(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_vat_exemption_reason"), "Designation", "Oid", initialValueSelectConfigurationVatExemptionReason, criteriaOperatorSelectVatExemptionReason);
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
            _entryBoxValidationNotes = new EntryBoxValidation(this, "Notes", KeyboardMode.AlfaNumeric, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false);
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
            if (_documentFinanceType.Oid == CustomDocumentSettings.CreditNoteDocumentTypeId)
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


        private void _entryBoxSelectArticleSubFamily_ClosePopup(object sender, EventArgs e)
        {

        }

        private void _entryBoxSelectArticleFamily_ClosePopup(object sender, EventArgs e)
        {
            if (_entryBoxSelectArticleFamily.Value != null)
            {
                _entryBoxSelectArticleSubFamily.Entry.Text = string.Empty;
                _entryBoxSelectArticleSubFamily.ButtonSelectValue.Sensitive = true;
                _entryBoxSelectArticleSubFamily.CriteriaOperator = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND Family == '{0}'", _entryBoxSelectArticleFamily.Value.Oid.ToString()));
                _entryBoxSelectArticle.CriteriaOperator = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND Family == '{0}'", _entryBoxSelectArticleFamily.Value.Oid.ToString()));
                _entryBoxSelectArticleCode.CriteriaOperator = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND Family == '{0}'", _entryBoxSelectArticleFamily.Value.Oid.ToString()));
            }
        }

        private bool ResetValidationFields()
        {
            return false;
        }

        //Front-End - Adicionar artigos na criação de Documentos [IN:010335]
        protected override void OnResponse(ResponseType pResponse)
        {
            if (pResponse == ResponseType.Cancel)
            {
                //CANCELED
            }

            else if ((pResponse == ResponseType.Ok && (string.IsNullOrEmpty(_entryBoxSelectArticleCode.Entry.Text) || string.IsNullOrEmpty(_entryBoxSelectArticle.Entry.Text))))
            {
                logicpos.Utils.ShowMessageTouch(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Ok, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_insert_articles"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_insert_new_article_code_error"));
                this.Run();
            }

            // Call ValidateMaxQuantities before 
            else if (pResponse == ResponseType.Ok && !ValidateMaxArticleQuantity())
            {     
                this.Run();
            }
            else if (pResponse == ResponseType.Ok && !ValidateSerialNumber())
            {
                this.Run();
            }
            else if (pResponse == ResponseType.Ok && !ValidateMinArticleStockQuantity())
            {
                this.Run();
            }
            else if (pResponse == ResponseType.DeleteEvent)
            {
                // Clean article fields
                CleanArticleFields(true);

                this.Run();
            }
            else if (pResponse == ResponseType.Ok)
            {
                _crudWidgetList.ProcessDialogResponse(this, _dialogMode, pResponse);

                //Save new article
                try
                {
                    if (_article == null || _article.Oid == Guid.Empty)
                    {
                        //Verifica se artigo já existe
                        fin_configurationpricetype configurationPriceTypeDefault = (fin_configurationpricetype)XPOSettings.Session.GetObjectByKey(typeof(fin_configurationpricetype), XPOSettings.XpoOidConfigurationPriceTypeDefault);

                        _article = new fin_article(XPOSettings.Session);
                        //Prepare Objects
                        Guid haveCode = new Guid();

                        string sql = string.Format(@"SELECT Oid FROM fin_article WHERE Code = '{0}' ORDER BY Code; ", _entryBoxSelectArticleCode.Entry.Text);
                        var result = XPOSettings.Session.ExecuteQuery(sql);
                        if (result != null && result.ResultSet[0].Rows.Length > 0)
                        {
                            haveCode = Guid.Parse(result.ResultSet[0].Rows[0].Values[0].ToString());
                        }

                        //Code don't exists on database
                        if ((result.ResultSet[0].Rows.Length == 0) && (!string.IsNullOrEmpty(_entryBoxSelectArticleCode.Entry.Text) || !string.IsNullOrEmpty(_entryBoxSelectArticle.Entry.Text)))
                        {
                            //Guid vatExemptionReasonOid = Guid.Parse((_dataSourceRow["VatExemptionReason.value"].ToString()).ToString());
                            _article.Code = _entryBoxSelectArticleCode.Entry.Text;
                            _article.Designation = _entryBoxSelectArticle.Entry.Text;
                            _article.Price1 = LogicPOS.Utility.DataConversionUtils.StringToDecimal(_dataSourceRow["Price"].ToString());
                            _article.PriceWithVat = false;
                            _article.Discount = LogicPOS.Utility.DataConversionUtils.StringToDecimal(_dataSourceRow["Discount"].ToString());
                            _article.VatDirectSelling = (fin_configurationvatrate)XPOSettings.Session.GetObjectByKey(typeof(fin_configurationvatrate), _entryBoxSelectVatRate.Value.Oid);
                            _article.VatOnTable = _article.VatDirectSelling;
                            if (_entryBoxSelectVatExemptionReason.Value != null)
                            {
                                _article.VatExemptionReason = (fin_configurationvatexemptionreason)XPOSettings.Session.GetObjectByKey(typeof(fin_configurationvatexemptionreason), _entryBoxSelectVatExemptionReason.Value.Oid);
                            }
                            _article.Notes = _entryBoxValidationNotes.EntryValidation.Text;
                            _article.Family = (fin_articlefamily)XPOSettings.Session.GetObjectByKey(typeof(fin_articlefamily), _entryBoxSelectArticleFamily.Value.Oid);
                            _article.SubFamily = (fin_articlesubfamily)XPOSettings.Session.GetObjectByKey(typeof(fin_articlesubfamily), _entryBoxSelectArticleSubFamily.Value.Oid);
                            _article.Type = (fin_articletype)XPOSettings.Session.GetObjectByKey(typeof(fin_articletype), XPOSettings.XpoOidArticleDefaultType);

                            _article.Save();

                            _entryBoxSelectArticle.Value = _article;
                            _entryBoxSelectArticle_ClosePopup(null, null);

                            logicpos.Utils.ShowMessageTouch(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_insert_articles"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_insert_new_article"));
                        }
                        else
                        {
                            logicpos.Utils.ShowMessageTouch(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Ok, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_insert_articles"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_insert_new_article_code_error"));
                            this.Run();
                        }

                    }
                    //Codigo ou designação vazias
                    if (string.IsNullOrEmpty(_entryBoxSelectArticleCode.Entry.Text) || string.IsNullOrEmpty(_entryBoxSelectArticle.Entry.Text))
                    {
                        logicpos.Utils.ShowMessageTouch(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Ok, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_insert_articles"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_insert_new_article_code_error"));
                        this.Run();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("Error saving new article: {0}", ex.Message));
                }

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
        private void _entryBoxSelectArticleCode_ClosePopup(object sender, EventArgs e)
        {
            try
            {
                //Prepare Objects
                Guid haveCode = new Guid();
                if ((sender as XPOEntryBoxSelectRecord<fin_articlewarehouse, TreeViewArticleWarehouse>) == _entryBoxSelectArticleWarehouse)
                {
                    haveCode = _entryBoxSelectArticleWarehouse.Value.Article.Oid;                    
                }

                if(haveCode == Guid.Empty)
                {
                    string sql = string.Format(@"SELECT Oid FROM fin_article WHERE Code = '{0}' ORDER BY Code; ", _entryBoxSelectArticleCode.Entry.Text);

                    var result = XPOSettings.Session.ExecuteQuery(sql);
                    if (result != null && result.ResultSet[0].Rows.Length > 0)
                    {
                        haveCode = Guid.Parse(result.ResultSet[0].Rows[0].Values[0].ToString());
                    }
                }

                if (haveCode != null && haveCode != Guid.Empty)
                {
                    fin_article article = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), haveCode);
                    fin_configurationpricetype configurationPriceTypeDefault = (fin_configurationpricetype)XPOSettings.Session.GetObjectByKey(typeof(fin_configurationpricetype), XPOSettings.XpoOidConfigurationPriceTypeDefault);

                    if (article != null && article.Type.HavePrice && article.Oid != Guid.Empty)
                    {
                        this.WindowTitle = string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_insert_articles"));
                        if (string.IsNullOrEmpty(_entryBoxSelectArticle.Entry.Text) || _entryBoxSelectArticle.Entry.Text != article.Designation)
                        {
                            _entryBoxSelectArticle.Value = article;
                            _entryBoxSelectArticle.Entry.Text = article.Designation;
                        }

                        if (article.SubFamily != null && article.Family != null)
                        {
                            _entryBoxSelectArticleSubFamily.Entry.Text = article.SubFamily.Designation;
                            _entryBoxSelectArticleSubFamily.Value = article.SubFamily;
                            _entryBoxSelectArticleSubFamily.ButtonSelectValue.Sensitive = false;

                            _entryBoxSelectArticleFamily.Entry.Text = article.Family.Designation;
                            _entryBoxSelectArticleFamily.Value = article.Family;
                            _entryBoxSelectArticleFamily.ButtonSelectValue.Sensitive = false;

                            _crudWidgetSelectArticleFamily.Validated = true;
                            _crudWidgetSelectArticleFamily.ValidateField();

                            _crudWidgetSelectArticleSubFamily.Validated = true;
                            _crudWidgetSelectArticleSubFamily.ValidateField();
                        }
                        //Get PriceType from Customer.PriceType or from default if a New Customer or a Customer without PriceType Defined in BackOffice, always revert to Price1
                        PriceType priceType = (_customer != null)
                            ? (PriceType)_customer.PriceType.EnumValue
                            : (PriceType)configurationPriceTypeDefault.EnumValue
                        ;

                        //Common changes for MediaNova | Non-MediaNova Articles | Here Prices are always in Retail Mode
                        PriceProperties priceProperties = ArticleUtils.GetArticlePrice(article, priceType, TaxSellType.Normal);


                        //Price
                        _articlePrice = priceProperties.PriceNet;
                        _dataSourceRow["PriceFinal"] = priceProperties.PriceNet;
                        //Display Price
                        _entryBoxValidationPrice.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(_articlePrice);
                        _entryBoxValidationPriceDisplay.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(_articlePrice * _currencyDisplay.ExchangeRate);
                        _entryBoxValidationQuantity.EntryValidation.Text = (article.DefaultQuantity > 0) ? LogicPOS.Utility.DataConversionUtils.DecimalToString(article.DefaultQuantity) : LogicPOS.Utility.DataConversionUtils.DecimalToString(1.0m);
                        _entryBoxValidationDiscount.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(article.Discount);

                        //VatRate
                        _entryBoxSelectVatRate.Value = article.VatDirectSelling;
                        _entryBoxSelectVatRate.Entry.Text = article.VatDirectSelling.Designation;


                        //Price
                        _articlePrice = priceProperties.PriceNet;
                        _dataSourceRow["PriceFinal"] = priceProperties.PriceNet;
                        //Display Price
                        _entryBoxValidationPrice.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(_articlePrice);
                        _entryBoxValidationPriceDisplay.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(_articlePrice * _currencyDisplay.ExchangeRate);
                        _entryBoxValidationQuantity.EntryValidation.Text = (article.DefaultQuantity > 0) ? LogicPOS.Utility.DataConversionUtils.DecimalToString(article.DefaultQuantity) : LogicPOS.Utility.DataConversionUtils.DecimalToString(1.0m);
                        _entryBoxValidationDiscount.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(article.Discount);


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

                        //Assign to DataRow
                        _dataSourceRow["Article.Code"] = article;
                        _dataSourceRow[3] = article;
                        _dataSourceRow[1] = article.Oid;

                        _dataSourceRow["Price"] = _dataSourceRow["PriceFinal"];
                        _article = article;
                        if(_entryBoxSelectArticleWarehouse.Value != null) { _dataSourceRow["Warehouse"] = (_entryBoxSelectArticleWarehouse.Value as fin_articlewarehouse).Oid.ToString(); }
                        
                        if (_article.ArticleSerialNumber.Count > 0)
                        {
                            _entryBoxSelectArticleWarehouse.Entry.IsEditable = true;
                            _entryBoxSelectArticleWarehouse.Entry.Sensitive = true;
                            if(_entryBoxSelectArticleWarehouse.Value.ArticleSerialNumber != null)
                            {
                                _dataSourceRow["SerialNumber"] = _entryBoxSelectArticleWarehouse.Value.ArticleSerialNumber.SerialNumber;
                                _entryBoxValidationQuantity.EntryValidation.Sensitive = false;
                            }
                            else
                            {
                                _dataSourceRow["SerialNumber"] = "";
                                _entryBoxValidationQuantity.EntryValidation.Sensitive = true;
                            }
                        }
                        else
                        {
                            _entryBoxSelectArticleWarehouse.Entry.IsEditable = false;
                            _entryBoxSelectArticleWarehouse.Entry.Sensitive = false;
                            _entryBoxSelectArticleWarehouse.Value = null;
                            _entryBoxValidationQuantity.EntryValidation.Sensitive = true;
                            _entryBoxSelectArticleWarehouse.Entry.Text = "";
                            _dataSourceRow["SerialNumber"] = "";
                        }
                    }

                    else
                    {
                        logicpos.Utils.ShowMessageBox(_sourceWindow, DialogFlags.DestroyWithParent, new Size(450, 350), MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_already_exited"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "article_type_without_price"));
                    }

                }
                else
                {
                    this.WindowTitle = string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_insert_articles") + " :: " + CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_new_article"));
                    CleanArticleFields(false);
                }


            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private void _entryBoxSelectArticle_ClosePopup(object sender, EventArgs e)
        {
            try
            {
                //Prepare Objects
                Guid haveCode = new Guid();

                string sql = string.Format(@"SELECT Oid FROM fin_article WHERE Designation = '{0}' ORDER BY Code; ", _entryBoxSelectArticle.Entry.Text);
                var result = XPOSettings.Session.ExecuteQuery(sql);

                if (result != null && result.ResultSet[0].Rows.Length > 0)
                {
                    haveCode = Guid.Parse(result.ResultSet[0].Rows[0].Values[0].ToString());
                }

                if (haveCode != null && haveCode != Guid.Empty)
                {
                    fin_article article = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), haveCode);
                    fin_configurationpricetype configurationPriceTypeDefault = (fin_configurationpricetype)XPOSettings.Session.GetObjectByKey(typeof(fin_configurationpricetype), XPOSettings.XpoOidConfigurationPriceTypeDefault);

                    if (article != null && article.Type.HavePrice && article.Oid != Guid.Empty)
                    {
                        //bool showMessage = false;
                        //if (GlobalFramework.CheckStocks)
                        //{
                        //    if (!Utils.ShowMessageMinimumStock(this, haveCode, article.DefaultQuantity, out showMessage))
                        //    {
                        //        if (showMessage) { return; }                                
                        //    }
                        //}

                        this.WindowTitle = string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_insert_articles"));

                        if (string.IsNullOrEmpty(_entryBoxSelectArticleCode.Entry.Text) || _entryBoxSelectArticleCode.Entry.Text != article.Code)
                        {
                            _entryBoxSelectArticleCode.Value = article;
                            _entryBoxSelectArticleCode.Entry.Text = article.Code;
                        }

                        if (article.SubFamily != null && article.Family != null)
                        {
                            _entryBoxSelectArticleSubFamily.Entry.Text = article.SubFamily.Designation;
                            _entryBoxSelectArticleSubFamily.Value = article.SubFamily;
                            _entryBoxSelectArticleSubFamily.ButtonSelectValue.Sensitive = false;

                            _entryBoxSelectArticleFamily.Entry.Text = article.Family.Designation;
                            _entryBoxSelectArticleFamily.Value = article.Family;
                            _entryBoxSelectArticleFamily.ButtonSelectValue.Sensitive = false;

                            _crudWidgetSelectArticleFamily.Validated = true;
                            _crudWidgetSelectArticleFamily.ValidateField();

                            _crudWidgetSelectArticleSubFamily.Validated = true;
                            _crudWidgetSelectArticleSubFamily.ValidateField();
                        }
                        //Get PriceType from Customer.PriceType or from default if a New Customer or a Customer without PriceType Defined in BackOffice, always revert to Price1
                        PriceType priceType = (_customer != null)
                            ? (PriceType)_customer.PriceType.EnumValue
                            : (PriceType)configurationPriceTypeDefault.EnumValue
                        ;

                        //Common changes for MediaNova | Non-MediaNova Articles | Here Prices are always in Retail Mode
                        PriceProperties priceProperties = ArticleUtils.GetArticlePrice(article, priceType, TaxSellType.Normal);

                        //Price
                        _articlePrice = priceProperties.PriceNet;
                        _dataSourceRow["PriceFinal"] = priceProperties.PriceNet;
                        //Display Price
                        _entryBoxValidationPrice.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(_articlePrice);
                        _entryBoxValidationPriceDisplay.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(_articlePrice * _currencyDisplay.ExchangeRate);
                        _entryBoxValidationQuantity.EntryValidation.Text = (article.DefaultQuantity > 0) ? LogicPOS.Utility.DataConversionUtils.DecimalToString(article.DefaultQuantity) : LogicPOS.Utility.DataConversionUtils.DecimalToString(1.0m);
                        _entryBoxValidationDiscount.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(article.Discount);


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

                        //Assign to DataRow
                        _dataSourceRow["Article.Code"] = article;
                        _dataSourceRow[3] = article;
                        _dataSourceRow[1] = article.Oid;
                        _dataSourceRow["Price"] = _dataSourceRow["PriceFinal"];
                        _article = article;
                        if (_entryBoxSelectArticleWarehouse.Value != null) { _dataSourceRow["Warehouse"] = (_entryBoxSelectArticleWarehouse.Value as fin_articlewarehouse).Oid.ToString(); }
                        if (_article.ArticleSerialNumber.Count > 0)
                        {
                            _entryBoxSelectArticleWarehouse.Entry.IsEditable = true;
                            _entryBoxSelectArticleWarehouse.Entry.Sensitive = true;
                            if (_entryBoxSelectArticleWarehouse.Value.ArticleSerialNumber != null)
                            {
                                _dataSourceRow["SerialNumber"] = _entryBoxSelectArticleWarehouse.Value.ArticleSerialNumber.SerialNumber;
                                _entryBoxValidationQuantity.EntryValidation.Sensitive = false;
                            }
                            else
                            {
                                _dataSourceRow["SerialNumber"] = "";
                                _entryBoxValidationQuantity.EntryValidation.Sensitive = true;
                            }
                        }
                        else
                        {
                            _entryBoxSelectArticleWarehouse.Entry.IsEditable = false;
                            _entryBoxSelectArticleWarehouse.Entry.Sensitive = false;
                            _entryBoxValidationQuantity.EntryValidation.Sensitive = true;
                            _entryBoxSelectArticleWarehouse.Value = null;
                            _entryBoxSelectArticleWarehouse.Entry.Text = "";
                            _dataSourceRow["SerialNumber"] = "";
                        }
                    }

                    else
                    {
                        logicpos.Utils.ShowMessageBox(_sourceWindow, DialogFlags.DestroyWithParent, new Size(450, 350), MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_already_exited"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "article_type_without_price"));
                    }

                }
                else
                {
                    this.WindowTitle = string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_insert_articles") + " :: " + CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_new_article"));
                    CleanArticleFields(false);
                }


            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }


        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        private void CleanArticleFields(bool cleanCodeAndDesignation)
        {
            fin_configurationvatrate initialValueSelectConfigurationVatRate = (fin_configurationvatrate)XPOSettings.Session.GetObjectByKey(typeof(fin_configurationvatrate), XPOSettings.XpoOidArticleDefaultVatDirectSelling);
            fin_configurationvatexemptionreason initialValueSelectConfigurationVatExemptionReason = null;
            CriteriaOperator criteriaOperatorSelectVatRate = CriteriaOperator.Parse("(Disabled = 0 OR Disabled IS NULL)");

            _article = new fin_article();
            //Adicionar artigos na criação de Documentos - Verificar  [IN015510]
            if (cleanCodeAndDesignation)
            {
                _entryBoxSelectArticle.Entry.Text = string.Empty;
                _entryBoxSelectArticleCode.Entry.Text = string.Empty;
                _crudWidgetSelectArticle.Validated = false;
                _crudWidgetSelectArticle.ValidateField(ResetValidationFields);
                _crudWidgetSelectArticle.Required = true;
                _crudWidgetSelectArticleCode.Validated = false;
                _crudWidgetSelectArticleCode.ValidateField(ResetValidationFields);

                _entryBoxSelectVatRate.Value = initialValueSelectConfigurationVatRate;
                _entryBoxSelectVatRate.Entry.Text = _entryBoxSelectVatRate.Value.Designation;
                _entryBoxSelectVatExemptionReason.Value = initialValueSelectConfigurationVatExemptionReason;
                _entryBoxSelectArticle.Value = _article;
                _entryBoxSelectArticleCode.Value = _article;
                _entryBoxSelectArticleFamily.Value = _article.Family;
                _entryBoxSelectArticleSubFamily.Value = _article.SubFamily;

                _entryBoxSelectArticleFamily.Entry.Text = string.Empty;
                _entryBoxSelectArticleSubFamily.Entry.Text = string.Empty;
                _entryBoxSelectArticleSubFamily.ButtonSelectValue.Sensitive = false;


                _entryBoxValidationPrice.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(0.00m);
                _entryBoxValidationPriceDisplay.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(0.00m);
                _entryBoxValidationQuantity.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(0.00m);
                _entryBoxValidationTotalNet.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(0.00m);
                _entryBoxValidationTotalFinal.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(0.00m);
                _entryBoxValidationToken1.EntryValidation.Text = string.Empty;
                _entryBoxValidationToken2.EntryValidation.Text = string.Empty;
                _entryBoxValidationNotes.EntryValidation.Text = string.Empty;

                _crudWidgetSelectArticleFamily.Validated = false;
                _crudWidgetSelectArticleFamily.ValidateField(ResetValidationFields);
                _crudWidgetSelectArticleSubFamily.Validated = false;
                _crudWidgetSelectArticleSubFamily.ValidateField(ResetValidationFields);

                _entryBoxSelectArticleFamily.ButtonSelectValue.Sensitive = true;
            }



            ////Used only to Update DataRow Column from Widget : Used to force Assign XPGuidObject ChildNames to Columns
            _crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxSelectArticleCode, new Label(), _dataSourceRow, "Article.Code"));
            _crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxSelectArticle, new Label(), _dataSourceRow, "Article.Designation"));
            _crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxSelectArticleFamily, new Label(), _dataSourceRow, "Article.Family"));
            _crudWidgetList.Add(new GenericCRUDWidgetDataTable(_entryBoxSelectArticleSubFamily, new Label(), _dataSourceRow, "Article.SubFamily"));

        }

        private void ToggleVatExemptionReasonEditMode()
        {
            //Default Mode
            if (_documentFinanceType.Oid != DocumentSettings.XpoOidDocumentFinanceTypeConsignationInvoice)
            {
                if (_entryBoxSelectVatRate.Value != null && _entryBoxSelectVatRate.Value.Oid == InvoiceSettings.XpoOidConfigurationVatRateDutyFree)
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
                    LogicPOS.Utility.DataConversionUtils.StringToDecimal(_entryBoxValidationQuantity.EntryValidation.Text),
                    LogicPOS.Utility.DataConversionUtils.StringToDecimal(_entryBoxValidationDiscount.EntryValidation.Text),
                    DiscountGlobal,
                    _entryBoxSelectVatRate.Value.Value
                );
                _dataSourceRow["PriceFinal"] = priceProperties.PriceNet;
                _dataSourceRow["Price"] = _dataSourceRow["PriceFinal"];
                //priceProperties.SendToLog(article.Designation);

                //Update UI / Display with ExchangeRate
                /* IN009235 */
                _entryBoxValidationTotalNet.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(priceProperties.TotalNetBeforeDiscountGlobal * _currencyDisplay.ExchangeRate);
                _entryBoxValidationTotalFinal.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(priceProperties.TotalFinalBeforeDiscountGlobal * _currencyDisplay.ExchangeRate);
            }
            else
            {
                _entryBoxValidationTotalNet.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(0.0m);
                _entryBoxValidationTotalFinal.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(0.0m);
            }
        }

        private bool ValidateSerialNumber()
        {
            if((_posDocumentFinanceDialog.PagePad.Pages[2] as DocumentFinanceDialogPage3).ArticleBag != null)
            {
                try
                {
                    foreach (var item in (_posDocumentFinanceDialog.PagePad.Pages[2] as DocumentFinanceDialogPage3).ArticleBag)
                    {
                        if ((item.Key as ArticleBagKey).ArticleId == Guid.Parse(_dataSourceRow[1].ToString()))
                        {
                            if (!string.IsNullOrEmpty(_dataSourceRow["SerialNumber"].ToString()) && (item.Value as ArticleBagProperties).SerialNumber.Contains(_dataSourceRow["SerialNumber"].ToString()) && _dialogMode != DialogMode.Update)
                            {
                                logicpos.Utils.ShowMessageTouch(this, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_error"), "Artigo com o nº série: " + _dataSourceRow["SerialNumber"].ToString() + " Já foi inserido");
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("bool ValidateSerialNumber() :: " + ex.Message, ex);
                }
            }
            return true;
        }

        private bool ValidateMaxArticleQuantity()
        {
            bool result = true;
            try
            {
                /* IN009171 - when receiving a '.' on it, converter throws "System.FormatException" */
                string quantity = _entryBoxValidationQuantity.EntryValidation.Text;
                if (!string.IsNullOrEmpty(quantity))
                {
                    quantity = LogicPOS.Utility.DataConversionUtils.DecimalToString(LogicPOS.Utility.DataConversionUtils.StringToDecimal(quantity));
                    quantity = quantity.Replace('.', ',');
                }
                decimal currentQuantity = Convert.ToDecimal(quantity);
                // Validate Max Quantities only if ValidateMaxQuantities is defined
                if (_posDocumentFinanceDialog.ValidateMaxQuantities != null && _posDocumentFinanceDialog.ValidateMaxQuantities.Count > 0)
                {
                    decimal maxPossibleQuantity = _posDocumentFinanceDialog.ValidateMaxQuantities[_entryBoxSelectArticle.Value.Oid];

                    if (currentQuantity > maxPossibleQuantity)
                    {
                        _logger.Debug(string.Format("CurrentQuantity: [{0}] is Greater than MaxPossibleQuantity: [{1}]", currentQuantity, maxPossibleQuantity));
                        result = false;
                    }

                    // Check if has Errrors, and Show Error Message
                    if (!result)
                    {
                        // Show Message
                        logicpos.Utils.ShowMessageTouchErrorTryToIssueACreditNoteExceedingSourceDocumentArticleQuantities(this, currentQuantity, maxPossibleQuantity);
                    }
                }                
            }
            catch (Exception ex)
            {
                _logger.Error("bool ValidateMaxArticleQuantity() :: _entryBoxValidationQuantity.EntryValidation.Text [ " + _entryBoxValidationQuantity.EntryValidation.Text + " ]: " + ex.Message, ex);
            }

            return result;
        }

        //Vvalidate Minimum stock
        private bool ValidateMinArticleStockQuantity()
        {
            bool result = true;
            try
            {
                /* IN009171 - when receiving a '.' on it, converter throws "System.FormatException" */
                string quantity = _entryBoxValidationQuantity.EntryValidation.Text;
                if (!string.IsNullOrEmpty(quantity))
                {
                    quantity = LogicPOS.Utility.DataConversionUtils.DecimalToString(LogicPOS.Utility.DataConversionUtils.StringToDecimal(quantity));
                    quantity = quantity.Replace('.', ',');
                }
                decimal currentQuantity = Convert.ToDecimal(quantity);
  
                //Check for Minimum stock
                bool showMessage;
                if (logicpos.Utils.CheckStocks())
                {

                    if (!logicpos.Utils.ShowMessageMinimumStock(this, _article.Oid, currentQuantity, out showMessage))
                    {
                        if (showMessage)
                        {
                            result = false;
                        }else
                        result = true;
                    }
                    else { result = true; }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("bool ValidateMinArticleStockQuantity() :: _entryBoxValidationQuantity.EntryValidation.Text [ " + _entryBoxValidationQuantity.EntryValidation.Text + " ]: " + ex.Message, ex);
            }

            return result;
        }
    }
}
