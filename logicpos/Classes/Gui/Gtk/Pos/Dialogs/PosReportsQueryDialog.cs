using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Reports;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosReportsQueryDialog : PosBaseDialog
    {
        // ScrolledWindow
        private ScrolledWindow _scrolledWindow;
        // EntryBoxValidationDatePickerDialog
        private EntryBoxValidationDatePickerDialog _entryBoxDateStart;
        private EntryBoxValidationDatePickerDialog _entryBoxDateEnd;
        // XPOEntryBoxSelectRecordValidation
        private XPOEntryBoxSelectRecordValidation<fin_documentfinancetype, TreeViewDocumentFinanceType> _entryBoxSelectDocumentFinanceType;
        private XPOEntryBoxSelectRecordValidation<pos_configurationplaceterminal, TreeViewConfigurationPlaceTerminal> _entryBoxSelectConfigurationPlaceTerminal;
        private XPOEntryBoxSelectRecordValidation<sys_userdetail, TreeViewUser> _entryBoxSelectUserDetail;
        private XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _entryBoxSelectCustomer;
        private XPOEntryBoxSelectRecordValidation<fin_configurationpaymentmethod, TreeViewConfigurationPaymentMethod> _entryBoxSelectConfigurationPaymentMethod;
        private XPOEntryBoxSelectRecordValidation<fin_configurationpaymentcondition, TreeViewConfigurationPaymentCondition> _entryBoxSelectConfigurationPaymentCondition;
        private XPOEntryBoxSelectRecordValidation<cfg_configurationcurrency, TreeViewConfigurationCurrency> _entryBoxSelectConfigurationCurrency;
        private XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry> _entryBoxSelectShipFromCountry;
        private XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle> _entryBoxSelectArticle;
        private XPOEntryBoxSelectRecordValidation<fin_articlefamily, TreeViewArticleFamily> _entryBoxSelectArticleFamily;
        private XPOEntryBoxSelectRecordValidation<fin_articlesubfamily, TreeViewArticleSubFamily> _entryBoxSelectArticleSubFamily;
        private XPOEntryBoxSelectRecordValidation<pos_configurationplace, TreeViewConfigurationPlace> _entryBoxSelectPlace;
        private XPOEntryBoxSelectRecordValidation<pos_configurationplacetable, TreeViewConfigurationPlaceTable> _entryBoxSelectPlaceTable;
        private XPOEntryBoxSelectRecordValidation<sys_systemaudittype, TreeViewSystemAuditType> _entryBoxSelectSystemAuditType;
        // Dictionaries
        private Dictionary<string, object> _selectionBoxs = new Dictionary<string, object>();
        // Strore components for mode, this is used to Add or Not component to UI based on ReportsQueryDialogMode
        private Dictionary<ReportsQueryDialogMode, Dictionary<string, string>> _fieldsModeComponents = new Dictionary<ReportsQueryDialogMode, Dictionary<string, string>>();
        // Dialog Buttons
        private TouchButtonIconWithText _buttonOk;
        private TouchButtonIconWithText _buttonCancel;
		// IN009223 IN009227
        private TouchButtonIconWithText _buttonCleanFilter;
        private ResponseType _responseTypeCleanFilter = (ResponseType) DialogResponseType.CleanFilter;

        // Export to pdf/excel
        private TouchButtonIconWithText _buttonExportPdf;
        private ResponseType _responseTypeExportPdf = (ResponseType)DialogResponseType.ExportPdf;
        private TouchButtonIconWithText _buttonExportXls;
        private ResponseType _responseTypeExportXls = (ResponseType)DialogResponseType.ExportXls;

        // Parameters
        private ReportsQueryDialogMode _reportsQueryDialogMode;
        private string _databaseSourceObject;
        // Public Properties
        private string _filterValue;
        private string _filterValueHumanReadble;
        public string FilterValue { get => _filterValue; set => _filterValue = value; }
        public string FilterValueHumanReadble { get => _filterValueHumanReadble; set => _filterValueHumanReadble = value; }
        private DateTime _dateStart;
        public DateTime DateStart { get => _dateStart; set => _dateStart = value; }
        private DateTime _dateEnd;
        public DateTime DateEnd { get => _dateEnd; set => _dateEnd = value; }

        //Overload : Default Dates Start: 1st Day of Month, End Last Day Of Month
        public PosReportsQueryDialog(Window pSourceWindow, DialogFlags pDialogFlags, ReportsQueryDialogMode pReportsQueryDialogMode, string pDatabaseSourceObject)
            : base(pSourceWindow, pDialogFlags)
        {
            // Private Properties Parameters
            _reportsQueryDialogMode = pReportsQueryDialogMode;
            // Corresponds to Database Entity ex fin_documentfinancemaster, view_documentfinance, view_documentfinancesellgroup
            _databaseSourceObject = pDatabaseSourceObject;
            //pastMonths=0 to Work in Curent Month Range, pastMonths=1 Works in Past Month, pastMonths=2 Two months Ago etc
            int pastMonths = 0;
            DateTime workingDate = FrameworkUtils.CurrentDateTimeAtomic().AddMonths(-pastMonths);

            /* IN005974 */
            //DateTime firstDayOfYear = new DateTime(workingDate.Year, 1, 1);
            DateTime firstDayOfMonth = new DateTime(workingDate.Year, workingDate.Month, 1);
            //DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            DateTime dateTimeStart = firstDayOfMonth;
            //DateTime dateTimeEnd = lastDayOfMonth.AddHours(23).AddMinutes(59).AddSeconds(59);
            DateTime dateTimeEnd = workingDate;

            InitUI(pDialogFlags, dateTimeStart, dateTimeEnd);
        }

        public PosReportsQueryDialog(Window pSourceWindow, DialogFlags pDialogFlags, DateTime pDateStart, DateTime pDateEnd)
            : base(pSourceWindow, pDialogFlags)
        {
            //Call Init UI
            InitUI(pDialogFlags, pDateStart, pDateEnd);
        }

        private void InitUI(DialogFlags pDialogFlags, DateTime pDateStart, DateTime pDateEnd)
        {

            //Init Local Vars
            String windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_report_filter");
            Size windowSize = new Size(540, 568);
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_date_picker.png");
			
			/* IN009010 */
            if (!ReportsQueryDialogMode.CUSTOMER_BALANCE_SUMMARY.Equals(_reportsQueryDialogMode))
            {
                //Parameters
                _dateStart = pDateStart;
                _dateEnd = pDateEnd;

                //Init DateEntry Start
                _entryBoxDateStart = new EntryBoxValidationDatePickerDialog(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date_start"), _dateStart, SettingsApp.RegexDate, true);
                _entryBoxDateStart.EntryValidation.Text = _dateStart.ToString(SettingsApp.DateFormat);
                _entryBoxDateStart.EntryValidation.Validate();
                _entryBoxDateStart.ClosePopup += entryBoxDateStart_ClosePopup;
                /* IN005974 - now, date field also accepts text */ // _entryBoxDateStart.KeyReleaseEvent += entryBoxDateStart_Text;
                _entryBoxDateStart.EntryValidation.Changed += entryBoxDateStart_Changed;
                //Init DateEntry End
                _entryBoxDateEnd = new EntryBoxValidationDatePickerDialog(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date_end"), _dateEnd, SettingsApp.RegexDate, true);
                _entryBoxDateEnd.EntryValidation.Text = _dateEnd.ToString(SettingsApp.DateFormat);
                _entryBoxDateEnd.EntryValidation.Validate();
                _entryBoxDateEnd.ClosePopup += entryBoxDateEnd_ClosePopup;
                /* IN005974 - now, date field also accepts text */ // _entryBoxDateEnd.KeyReleaseEvent += entryBoxDateEnd_Text;
                _entryBoxDateEnd.EntryValidation.Changed += entryBoxDateEnd_Changed;
            }

            //InitFieldsModeComponents
            InitFieldsModeComponents();

            VBox vbox = new VBox(false, 0);
            vbox.PackStart(_entryBoxDateStart, false, false, 2);
            vbox.PackStart(_entryBoxDateEnd, false, false, 2);

            // Loop selectionBoxs and Place it in VBox
            foreach (var item in _selectionBoxs)
            {
                vbox.PackStart((item.Value as Widget), false, false, 2);
            }

            //ViewPort
            Viewport viewport = new Viewport() { ShadowType = ShadowType.None };
            viewport.Add(vbox);
            viewport.ResizeMode = ResizeMode.Parent;
            //ScrolledWindow
            _scrolledWindow = new ScrolledWindow();
            _scrolledWindow.ShadowType = ShadowType.EtchedIn;
            _scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            _scrolledWindow.Add(viewport);
            _scrolledWindow.ResizeMode = ResizeMode.Parent;
            //ActionArea Buttons
			// IN009223 IN009227
            string fileActionFilter = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_clean_filter.png");
            string fileActionExportPdf = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_export_pdf.png");
            string fileActionExportXls = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_export_xls.png");

            _buttonCleanFilter = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.CleanFilter,"Clean Filter", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_filter_clear"), fileActionFilter);
            //Export to Xls/pdf
            _buttonExportPdf = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.ExportPdf, "touchButtonPosToolbarFinanceDocuments_Red", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_export_pdf"), fileActionExportPdf);
            _buttonExportXls = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.ExportXls, "touchButtonPosToolbarFinanceDocuments_Green", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_export_xls"), fileActionExportXls);

            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
            // Used to get GetComposedFilter
            _buttonOk.Clicked += _buttonOk_Clicked;
            _buttonExportXls.Clicked += _buttonExportXls_Clicked;
            _buttonExportPdf.Clicked += _buttonExportPdf_Clicked;

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
                        
            // IN009223 IN009227
            if (_reportsQueryDialogMode == ReportsQueryDialogMode.FILTER_DOCUMENTS_UNPAYED || _reportsQueryDialogMode == ReportsQueryDialogMode.FILTER_DOCUMENTS_PAGINATION || _reportsQueryDialogMode == ReportsQueryDialogMode.FILTER_PAYMENT_DOCUMENTS)
            {
                actionAreaButtons.Add(new ActionAreaButton(_buttonCleanFilter, _responseTypeCleanFilter));
                actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
                //actionAreaButtons.Add(_actionAreaButtonCleanFilter);
                windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_filter");
            }
            else if (Utils.IsLinux) {
                actionAreaButtons.Add(new ActionAreaButton(_buttonExportXls, _responseTypeExportXls));
                actionAreaButtons.Add(new ActionAreaButton(_buttonExportPdf, _responseTypeExportPdf));
            }
            else
            {
                actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
            }
            
            actionAreaButtons.Add(new ActionAreaButton(_buttonCancel, ResponseType.Cancel));

            //Start Validated
            Validate();

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _scrolledWindow, actionAreaButtons);
        }

        // UI Query Input Components
        private void InitFieldsModeComponents()
        {
            try
            {
                // FINANCIAL
                _fieldsModeComponents.Add(ReportsQueryDialogMode.FINANCIAL, new Dictionary<string, string>());
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(DateTime).Name, "Date");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(fin_documentfinancetype).Name, "DocumentType");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(pos_configurationplaceterminal).Name, "CreatedWhere");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(sys_userdetail).Name, "CreatedBy");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(erp_customer).Name, "EntityOid");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(fin_configurationpaymentmethod).Name, "PaymentMethod");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(fin_configurationpaymentcondition).Name, "PaymentCondition");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(cfg_configurationcurrency).Name, "Currency");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(cfg_configurationcountry).Name, "EntityCountryOid");

                // FINANCIAL_DETAIL
                _fieldsModeComponents.Add(ReportsQueryDialogMode.FINANCIAL_DETAIL, new Dictionary<string, string>());
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(DateTime).Name, "fmDate");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(fin_documentfinancetype).Name, "ftOid");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(pos_configurationplaceterminal).Name, "trTerminal");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(sys_userdetail).Name, "udUserDetail");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(erp_customer).Name, "fmEntity");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(fin_configurationpaymentmethod).Name, "fmPaymentMethod");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(fin_configurationpaymentcondition).Name, "fmPaymentCondition");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(cfg_configurationcurrency).Name, "fmCurrency");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(cfg_configurationcountry).Name, "ccCountry");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(fin_articlefamily).Name, "afFamily");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(fin_articlesubfamily).Name, "sfSubFamily");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(fin_article).Name, "fdArticle");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(pos_configurationplace).Name, "cpPlace");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(pos_configurationplacetable).Name, "dmPlaceTable");

                // FINANCIAL_DETAIL_GROUP : Equalt to FINANCIAL_DETAIL
                _fieldsModeComponents.Add(ReportsQueryDialogMode.FINANCIAL_DETAIL_GROUP, _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL]);

                // ARTICLE_STOCK_MOVEMENTS
                _fieldsModeComponents.Add(ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS, new Dictionary<string, string>());
                _fieldsModeComponents[ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS].Add(typeof(DateTime).Name, "stkDate");
                _fieldsModeComponents[ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS].Add(typeof(fin_documentfinancetype).Name, "fdmDocumentType");
                _fieldsModeComponents[ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS].Add(typeof(pos_configurationplaceterminal).Name, "afaTerminal");
                _fieldsModeComponents[ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS].Add(typeof(sys_userdetail).Name, "afaUserDetail");
                _fieldsModeComponents[ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS].Add(typeof(fin_articlefamily).Name, "afaOid");
                _fieldsModeComponents[ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS].Add(typeof(fin_articlesubfamily).Name, "asfOid");

                // SYSTEM_AUDIT
                _fieldsModeComponents.Add(ReportsQueryDialogMode.SYSTEM_AUDIT, new Dictionary<string, string>());
                _fieldsModeComponents[ReportsQueryDialogMode.SYSTEM_AUDIT].Add(typeof(DateTime).Name, "sauDate");
                _fieldsModeComponents[ReportsQueryDialogMode.SYSTEM_AUDIT].Add(typeof(sys_systemaudittype).Name, "satOid");
                _fieldsModeComponents[ReportsQueryDialogMode.SYSTEM_AUDIT].Add(typeof(sys_userdetail).Name, "usdOid");
                _fieldsModeComponents[ReportsQueryDialogMode.SYSTEM_AUDIT].Add(typeof(pos_configurationplaceterminal).Name, "cptOid");

                // CURRENT_ACCOUNT
                _fieldsModeComponents.Add(ReportsQueryDialogMode.CURRENT_ACCOUNT, new Dictionary<string, string>());
                _fieldsModeComponents[ReportsQueryDialogMode.CURRENT_ACCOUNT].Add(typeof(DateTime).Name, "Date");
                _fieldsModeComponents[ReportsQueryDialogMode.CURRENT_ACCOUNT].Add(typeof(erp_customer).Name, "EntityOid");

                // FILTER_DOCUMENTS_PAGINATION IN009223 IN009227
                _fieldsModeComponents.Add(ReportsQueryDialogMode.FILTER_DOCUMENTS_PAGINATION, new Dictionary<string, string>());
                _fieldsModeComponents[ReportsQueryDialogMode.FILTER_DOCUMENTS_PAGINATION].Add(typeof(DateTime).Name, "Date");
                _fieldsModeComponents[ReportsQueryDialogMode.FILTER_DOCUMENTS_PAGINATION].Add(typeof(fin_documentfinancetype).Name, "DocumentType");               
                _fieldsModeComponents[ReportsQueryDialogMode.FILTER_DOCUMENTS_PAGINATION].Add(typeof(erp_customer).Name, "EntityOid");
                _fieldsModeComponents[ReportsQueryDialogMode.FILTER_DOCUMENTS_PAGINATION].Add(typeof(fin_configurationpaymentmethod).Name, "PaymentMethod");
                _fieldsModeComponents[ReportsQueryDialogMode.FILTER_DOCUMENTS_PAGINATION].Add(typeof(fin_configurationpaymentcondition).Name, "PaymentCondition");

                // FILTER_DOCUMENTS_UNPAYED IN009223 IN009227
                _fieldsModeComponents.Add(ReportsQueryDialogMode.FILTER_DOCUMENTS_UNPAYED, new Dictionary<string, string>());
                _fieldsModeComponents[ReportsQueryDialogMode.FILTER_DOCUMENTS_UNPAYED].Add(typeof(DateTime).Name, "Date");
                _fieldsModeComponents[ReportsQueryDialogMode.FILTER_DOCUMENTS_UNPAYED].Add(typeof(fin_documentfinancetype).Name, "DocumentType");
                _fieldsModeComponents[ReportsQueryDialogMode.FILTER_DOCUMENTS_UNPAYED].Add(typeof(erp_customer).Name, "EntityOid");
                _fieldsModeComponents[ReportsQueryDialogMode.FILTER_DOCUMENTS_UNPAYED].Add(typeof(fin_configurationpaymentcondition).Name, "PaymentCondition");

                // FILTER_PAYMENT_DOCUMENTS IN009223 IN009227
                _fieldsModeComponents.Add(ReportsQueryDialogMode.FILTER_PAYMENT_DOCUMENTS, new Dictionary<string, string>());
                _fieldsModeComponents[ReportsQueryDialogMode.FILTER_PAYMENT_DOCUMENTS].Add(typeof(DateTime).Name, "PaymentDate");
                _fieldsModeComponents[ReportsQueryDialogMode.FILTER_PAYMENT_DOCUMENTS].Add(typeof(erp_customer).Name, "EntityOid");
                _fieldsModeComponents[ReportsQueryDialogMode.FILTER_PAYMENT_DOCUMENTS].Add(typeof(fin_configurationpaymentmethod).Name, "PaymentMethod");

                /* 
                 * IN008018: CUSTOMER_BALANCE_DETAILS
                 */
                _fieldsModeComponents.Add(ReportsQueryDialogMode.CUSTOMER_BALANCE_DETAILS, new Dictionary<string, string>());
                _fieldsModeComponents[ReportsQueryDialogMode.CUSTOMER_BALANCE_DETAILS].Add(typeof(DateTime).Name, "Date");
                _fieldsModeComponents[ReportsQueryDialogMode.CUSTOMER_BALANCE_DETAILS].Add(typeof(erp_customer).Name, "EntityOid");
                /* 
                 * IN009010: CUSTOMER_BALANCE_SUMMARY
                 */
                _fieldsModeComponents.Add(ReportsQueryDialogMode.CUSTOMER_BALANCE_SUMMARY, new Dictionary<string, string>());
                //_fieldsModeComponents[ReportsQueryDialogMode.CUSTOMER_BALANCE_SUMMARY].Add(typeof(DateTime).Name, "Date");
                _fieldsModeComponents[ReportsQueryDialogMode.CUSTOMER_BALANCE_SUMMARY].Add(typeof(erp_customer).Name, "EntityOid");
                /*
                 * IN009204: COMPANY_BILLING
                 */
                _fieldsModeComponents.Add(ReportsQueryDialogMode.COMPANY_BILLING, new Dictionary<string, string>());
                _fieldsModeComponents[ReportsQueryDialogMode.COMPANY_BILLING].Add(typeof(DateTime).Name, "Date");
                // USER_COMMISSION
                _fieldsModeComponents.Add(ReportsQueryDialogMode.USER_COMMISSION, new Dictionary<string, string>());
                _fieldsModeComponents[ReportsQueryDialogMode.USER_COMMISSION].Add(typeof(DateTime).Name, "DateDay");
                _fieldsModeComponents[ReportsQueryDialogMode.USER_COMMISSION].Add(typeof(sys_userdetail).Name, "UserOid");
                _fieldsModeComponents[ReportsQueryDialogMode.USER_COMMISSION].Add(typeof(fin_article).Name, "ArticleOid");

                // Create SelectionBox References / Fill Selection Boxs Dictionary to be used in Dynamic Dialog
                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(fin_documentfinancetype)))
                {
                    // For filter Area in Documents: IN009223 IN009227 - Begin
                    if (_reportsQueryDialogMode == ReportsQueryDialogMode.FILTER_DOCUMENTS_UNPAYED)
                    {
                        string extraFilter = $@" AND (
Oid = '{SettingsApp.XpoOidUndefinedRecord}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeInvoice}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeCreditNote}'
)".Replace(Environment.NewLine, string.Empty);
                        _entryBoxSelectDocumentFinanceType = SelectionBoxFactory<fin_documentfinancetype, TreeViewDocumentFinanceType>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinanceseries_documenttype"), "Designation", extraFilter);
                        _selectionBoxs.Add(typeof(fin_documentfinancetype).Name, _entryBoxSelectDocumentFinanceType);
                        
                    }
                    else if (_reportsQueryDialogMode == ReportsQueryDialogMode.FILTER_DOCUMENTS_PAGINATION)
                    {
                        string extraFilter = $@" AND (
Oid = '{SettingsApp.XpoOidUndefinedRecord}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeInvoice}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeConsignmentGuide}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeProformaInvoice}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeTransportationGuide}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeBudget}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeOwnAssetsDriveGuide}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeCreditNote}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeReturnGuide}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeDeliveryNote}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice}'
)".Replace(Environment.NewLine, string.Empty);
                    _entryBoxSelectDocumentFinanceType = SelectionBoxFactory<fin_documentfinancetype, TreeViewDocumentFinanceType>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinanceseries_documenttype"), "Designation", extraFilter);
                    _selectionBoxs.Add(typeof(fin_documentfinancetype).Name, _entryBoxSelectDocumentFinanceType);

                    }// IN009223 IN009227 - End
                    else {
                        // Leave Indentation, this will be converted to inline
                        string extraFilter = $@" AND (
Oid = '{SettingsApp.XpoOidUndefinedRecord}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeInvoice}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeDebitNote}' OR
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice}'
)".Replace(Environment.NewLine, string.Empty);
                        _entryBoxSelectDocumentFinanceType = SelectionBoxFactory<fin_documentfinancetype, TreeViewDocumentFinanceType>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinanceseries_documenttype"), "Designation", extraFilter);
                        _selectionBoxs.Add(typeof(fin_documentfinancetype).Name, _entryBoxSelectDocumentFinanceType);
                    }
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(pos_configurationplaceterminal)))
                {
                    _entryBoxSelectConfigurationPlaceTerminal = SelectionBoxFactory<pos_configurationplaceterminal, TreeViewConfigurationPlaceTerminal>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_configurationplaceterminal"));
                    _selectionBoxs.Add(typeof(pos_configurationplaceterminal).Name, _entryBoxSelectConfigurationPlaceTerminal);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(sys_userdetail)))
                {
                    _entryBoxSelectUserDetail = SelectionBoxFactory<sys_userdetail, TreeViewUser>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_user"), "Name");
                    _selectionBoxs.Add(typeof(sys_userdetail).Name, _entryBoxSelectUserDetail);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(erp_customer)))
                {
                    _entryBoxSelectCustomer = SelectionBoxFactory<erp_customer, TreeViewCustomer>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_customer"), "Name");
                    _selectionBoxs.Add(typeof(erp_customer).Name, _entryBoxSelectCustomer);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(fin_configurationpaymentmethod)))
                {
                    _entryBoxSelectConfigurationPaymentMethod = SelectionBoxFactory<fin_configurationpaymentmethod, TreeViewConfigurationPaymentMethod>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_payment_method"));
                    _selectionBoxs.Add(typeof(fin_configurationpaymentmethod).Name, _entryBoxSelectConfigurationPaymentMethod);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(fin_configurationpaymentcondition)))
                {
                    _entryBoxSelectConfigurationPaymentCondition = SelectionBoxFactory<fin_configurationpaymentcondition, TreeViewConfigurationPaymentCondition>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_payment_condition"));
                    _selectionBoxs.Add(typeof(fin_configurationpaymentcondition).Name, _entryBoxSelectConfigurationPaymentCondition);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(cfg_configurationcurrency)))
                {
                    _entryBoxSelectConfigurationCurrency = SelectionBoxFactory<cfg_configurationcurrency, TreeViewConfigurationCurrency>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_ConfigurationCurrency"));
                    _selectionBoxs.Add(typeof(cfg_configurationcurrency).Name, _entryBoxSelectConfigurationCurrency);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(cfg_configurationcountry)))
                {
                    _entryBoxSelectShipFromCountry = SelectionBoxFactory<cfg_configurationcountry, TreeViewConfigurationCountry>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_country"));
                    _selectionBoxs.Add(typeof(cfg_configurationcountry).Name, _entryBoxSelectShipFromCountry);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(fin_article)))
                {
                    _entryBoxSelectArticle = SelectionBoxFactory<fin_article, TreeViewArticle>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_articles"));
                    _selectionBoxs.Add(typeof(fin_article).Name, _entryBoxSelectArticle);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(fin_articlefamily)))
                {
                    _entryBoxSelectArticleFamily = SelectionBoxFactory<fin_articlefamily, TreeViewArticleFamily>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_families"));
                    _selectionBoxs.Add(typeof(fin_articlefamily).Name, _entryBoxSelectArticleFamily);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(fin_articlesubfamily)))
                {
                    _entryBoxSelectArticleSubFamily = SelectionBoxFactory<fin_articlesubfamily, TreeViewArticleSubFamily>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_subfamilies"));
                    _selectionBoxs.Add(typeof(fin_articlesubfamily).Name, _entryBoxSelectArticleSubFamily);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(pos_configurationplace)))
                {
                    _entryBoxSelectPlace = SelectionBoxFactory<pos_configurationplace, TreeViewConfigurationPlace>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_places"));
                    _selectionBoxs.Add(typeof(pos_configurationplace).Name, _entryBoxSelectPlace);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(pos_configurationplacetable)))
                {
                    _entryBoxSelectPlaceTable = SelectionBoxFactory<pos_configurationplacetable, TreeViewConfigurationPlaceTable>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_place_tables"));
                    _selectionBoxs.Add(typeof(pos_configurationplacetable).Name, _entryBoxSelectPlaceTable);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(sys_systemaudittype)))
                {
                    _entryBoxSelectSystemAuditType = SelectionBoxFactory<sys_systemaudittype, TreeViewSystemAuditType>(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_audit_type"));
                    _selectionBoxs.Add(typeof(sys_systemaudittype).Name, _entryBoxSelectSystemAuditType);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private bool ComponentExistsInQueryDialogMode(ReportsQueryDialogMode key, Type type)
        {
            return _fieldsModeComponents[_reportsQueryDialogMode].ContainsKey(type.Name);
        }
    }
}
