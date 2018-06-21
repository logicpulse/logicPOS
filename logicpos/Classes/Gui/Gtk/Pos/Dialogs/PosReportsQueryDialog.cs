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
        private XPOEntryBoxSelectRecordValidation<FIN_DocumentFinanceType, TreeViewDocumentFinanceType> _entryBoxSelectDocumentFinanceType;
        private XPOEntryBoxSelectRecordValidation<POS_ConfigurationPlaceTerminal, TreeViewConfigurationPlaceTerminal> _entryBoxSelectConfigurationPlaceTerminal;
        private XPOEntryBoxSelectRecordValidation<SYS_UserDetail, TreeViewUser> _entryBoxSelectUserDetail;
        private XPOEntryBoxSelectRecordValidation<ERP_Customer, TreeViewCustomer> _entryBoxSelectCustomer;
        private XPOEntryBoxSelectRecordValidation<FIN_ConfigurationPaymentMethod, TreeViewConfigurationPaymentMethod> _entryBoxSelectConfigurationPaymentMethod;
        private XPOEntryBoxSelectRecordValidation<FIN_ConfigurationPaymentCondition, TreeViewConfigurationPaymentCondition> _entryBoxSelectConfigurationPaymentCondition;
        private XPOEntryBoxSelectRecordValidation<CFG_ConfigurationCurrency, TreeViewConfigurationCurrency> _entryBoxSelectConfigurationCurrency;
        private XPOEntryBoxSelectRecordValidation<CFG_ConfigurationCountry, TreeViewConfigurationCountry> _entryBoxSelectShipFromCountry;
        private XPOEntryBoxSelectRecordValidation<FIN_Article, TreeViewArticle> _entryBoxSelectArticle;
        private XPOEntryBoxSelectRecordValidation<FIN_ArticleFamily, TreeViewArticleFamily> _entryBoxSelectArticleFamily;
        private XPOEntryBoxSelectRecordValidation<FIN_ArticleSubFamily, TreeViewArticleSubFamily> _entryBoxSelectArticleSubFamily;
        private XPOEntryBoxSelectRecordValidation<POS_ConfigurationPlace, TreeViewConfigurationPlace> _entryBoxSelectPlace;
        private XPOEntryBoxSelectRecordValidation<POS_ConfigurationPlaceTable, TreeViewConfigurationPlaceTable> _entryBoxSelectPlaceTable;
        private XPOEntryBoxSelectRecordValidation<SYS_SystemAuditType, TreeViewSystemAuditType> _entryBoxSelectSystemAuditType;
        // Dictionaries
        private Dictionary<string, object> _selectionBoxs = new Dictionary<string, object>();
        // Strore components for mode, this is used to Add or Not component to UI based on ReportsQueryDialogMode
        private Dictionary<ReportsQueryDialogMode, Dictionary<string, string>> _fieldsModeComponents = new Dictionary<ReportsQueryDialogMode, Dictionary<string, string>>();
        // Dialog Buttons
        private TouchButtonIconWithText _buttonOk;
        private TouchButtonIconWithText _buttonCancel;
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
            DateTime firstDayOfMonth = new DateTime(workingDate.Year, workingDate.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            DateTime dateTimeStart = firstDayOfMonth;
            DateTime dateTimeEnd = lastDayOfMonth.AddHours(23).AddMinutes(59).AddSeconds(59);

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
            //Parameters
            _dateStart = pDateStart;
            _dateEnd = pDateEnd;

            //Init Local Vars
            String windowTitle = Resx.window_title_dialog_report_filter;
            Size windowSize = new Size(540, 568);
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_date_picker.png");

            //Init DateEntry Start
            _entryBoxDateStart = new EntryBoxValidationDatePickerDialog(this, Resx.global_date_start, _dateStart, SettingsApp.RegexDate, true);
            _entryBoxDateStart.EntryValidation.Text = _dateStart.ToString(SettingsApp.DateFormat);
            _entryBoxDateStart.EntryValidation.Validate();
            _entryBoxDateStart.ClosePopup += entryBoxDateStart_ClosePopup;
            //Init DateEntry End
            _entryBoxDateEnd = new EntryBoxValidationDatePickerDialog(this, Resx.global_date_end, _dateEnd, SettingsApp.RegexDate, true);
            _entryBoxDateEnd.EntryValidation.Text = _dateEnd.ToString(SettingsApp.DateFormat);
            _entryBoxDateEnd.EntryValidation.Validate();
            _entryBoxDateEnd.ClosePopup += entryBoxDateEnd_ClosePopup;

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
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
            // Used to get GetComposedFilter
            _buttonOk.Clicked += _buttonOk_Clicked;

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(_buttonCancel, ResponseType.Cancel));

            //Start Validated
            Validate();

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _scrolledWindow, actionAreaButtons);
        }

        private void InitFieldsModeComponents()
        {
            try
            {
                // FINANCIAL
                _fieldsModeComponents.Add(ReportsQueryDialogMode.FINANCIAL, new Dictionary<string, string>());
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(DateTime).Name, "Date");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(FIN_DocumentFinanceType).Name, "DocumentType");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(POS_ConfigurationPlaceTerminal).Name, "CreatedWhere");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(SYS_UserDetail).Name, "CreatedBy");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(ERP_Customer).Name, "EntityOid");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(FIN_ConfigurationPaymentMethod).Name, "PaymentMethod");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(FIN_ConfigurationPaymentCondition).Name, "PaymentCondition");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(CFG_ConfigurationCurrency).Name, "Currency");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL].Add(typeof(CFG_ConfigurationCountry).Name, "EntityCountryOid");

                // FINANCIAL_DETAIL
                _fieldsModeComponents.Add(ReportsQueryDialogMode.FINANCIAL_DETAIL, new Dictionary<string, string>());
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(DateTime).Name, "fmDate");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(FIN_DocumentFinanceType).Name, "ftOid");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(POS_ConfigurationPlaceTerminal).Name, "trTerminal");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(SYS_UserDetail).Name, "udUserDetail");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(ERP_Customer).Name, "fmEntity");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(FIN_ConfigurationPaymentMethod).Name, "fmPaymentMethod");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(FIN_ConfigurationPaymentCondition).Name, "fmPaymentCondition");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(CFG_ConfigurationCurrency).Name, "fmCurrency");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(CFG_ConfigurationCountry).Name, "ccCountry");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(FIN_ArticleFamily).Name, "afFamily");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(FIN_ArticleSubFamily).Name, "sfSubFamily");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(FIN_Article).Name, "fdArticle");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(POS_ConfigurationPlace).Name, "cpPlace");
                _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL].Add(typeof(POS_ConfigurationPlaceTable).Name, "dmPlaceTable");

                // FINANCIAL_DETAIL_GROUP : Equalt to FINANCIAL_DETAIL
                _fieldsModeComponents.Add(ReportsQueryDialogMode.FINANCIAL_DETAIL_GROUP, _fieldsModeComponents[ReportsQueryDialogMode.FINANCIAL_DETAIL]);

                // ARTICLE_STOCK_MOVEMENTS
                _fieldsModeComponents.Add(ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS, new Dictionary<string, string>());
                _fieldsModeComponents[ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS].Add(typeof(DateTime).Name, "stkDate");
                _fieldsModeComponents[ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS].Add(typeof(FIN_DocumentFinanceType).Name, "fdmDocumentType");
                _fieldsModeComponents[ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS].Add(typeof(POS_ConfigurationPlaceTerminal).Name, "afaTerminal");
                _fieldsModeComponents[ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS].Add(typeof(SYS_UserDetail).Name, "afaUserDetail");
                _fieldsModeComponents[ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS].Add(typeof(FIN_ArticleFamily).Name, "afaOid");
                _fieldsModeComponents[ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS].Add(typeof(FIN_ArticleSubFamily).Name, "asfOid");

                // SYSTEM_AUDIT
                _fieldsModeComponents.Add(ReportsQueryDialogMode.SYSTEM_AUDIT, new Dictionary<string, string>());
                _fieldsModeComponents[ReportsQueryDialogMode.SYSTEM_AUDIT].Add(typeof(DateTime).Name, "sauDate");
                _fieldsModeComponents[ReportsQueryDialogMode.SYSTEM_AUDIT].Add(typeof(SYS_SystemAuditType).Name, "satOid");
                _fieldsModeComponents[ReportsQueryDialogMode.SYSTEM_AUDIT].Add(typeof(SYS_UserDetail).Name, "usdOid");
                _fieldsModeComponents[ReportsQueryDialogMode.SYSTEM_AUDIT].Add(typeof(POS_ConfigurationPlaceTerminal).Name, "cptOid");

                // Create SelectionBox References / Fill Selection Boxs Dictionary to be used in Dynamic Dialog
                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(FIN_DocumentFinanceType)))
                {
                    string extraFilter = $@" AND (
Oid = '{SettingsApp.XpoOidUndefinedRecord}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeInvoice}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment}' OR 
Oid = '{SettingsApp.XpoOidDocumentFinanceTypeDebitNote}'
)".Replace(Environment.NewLine, string.Empty);
                    _entryBoxSelectDocumentFinanceType = SelectionBoxFactory<FIN_DocumentFinanceType, TreeViewDocumentFinanceType>(Resx.global_documentfinanceseries_documenttype, "Designation", extraFilter);
                    _selectionBoxs.Add(typeof(FIN_DocumentFinanceType).Name, _entryBoxSelectDocumentFinanceType);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(POS_ConfigurationPlaceTerminal)))
                {
                    _entryBoxSelectConfigurationPlaceTerminal = SelectionBoxFactory<POS_ConfigurationPlaceTerminal, TreeViewConfigurationPlaceTerminal>(Resx.global_configurationplaceterminal);
                    _selectionBoxs.Add(typeof(POS_ConfigurationPlaceTerminal).Name, _entryBoxSelectConfigurationPlaceTerminal);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(SYS_UserDetail)))
                {
                    _entryBoxSelectUserDetail = SelectionBoxFactory<SYS_UserDetail, TreeViewUser>(Resx.global_user, "Name");
                    _selectionBoxs.Add(typeof(SYS_UserDetail).Name, _entryBoxSelectUserDetail);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(ERP_Customer)))
                {
                    _entryBoxSelectCustomer = SelectionBoxFactory<ERP_Customer, TreeViewCustomer>(Resx.global_customer, "Name");
                    _selectionBoxs.Add(typeof(ERP_Customer).Name, _entryBoxSelectCustomer);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(FIN_ConfigurationPaymentMethod)))
                {
                    _entryBoxSelectConfigurationPaymentMethod = SelectionBoxFactory<FIN_ConfigurationPaymentMethod, TreeViewConfigurationPaymentMethod>(Resx.global_payment_method);
                    _selectionBoxs.Add(typeof(FIN_ConfigurationPaymentMethod).Name, _entryBoxSelectConfigurationPaymentMethod);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(FIN_ConfigurationPaymentCondition)))
                {
                    _entryBoxSelectConfigurationPaymentCondition = SelectionBoxFactory<FIN_ConfigurationPaymentCondition, TreeViewConfigurationPaymentCondition>(Resx.global_payment_condition);
                    _selectionBoxs.Add(typeof(FIN_ConfigurationPaymentCondition).Name, _entryBoxSelectConfigurationPaymentCondition);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(CFG_ConfigurationCurrency)))
                {
                    _entryBoxSelectConfigurationCurrency = SelectionBoxFactory<CFG_ConfigurationCurrency, TreeViewConfigurationCurrency>(Resx.global_ConfigurationCurrency);
                    _selectionBoxs.Add(typeof(CFG_ConfigurationCurrency).Name, _entryBoxSelectConfigurationCurrency);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(CFG_ConfigurationCountry)))
                {
                    _entryBoxSelectShipFromCountry = SelectionBoxFactory<CFG_ConfigurationCountry, TreeViewConfigurationCountry>(Resx.global_country);
                    _selectionBoxs.Add(typeof(CFG_ConfigurationCountry).Name, _entryBoxSelectShipFromCountry);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(FIN_Article)))
                {
                    _entryBoxSelectArticle = SelectionBoxFactory<FIN_Article, TreeViewArticle>(Resx.global_articles);
                    _selectionBoxs.Add(typeof(FIN_Article).Name, _entryBoxSelectArticle);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(FIN_ArticleFamily)))
                {
                    _entryBoxSelectArticleFamily = SelectionBoxFactory<FIN_ArticleFamily, TreeViewArticleFamily>(Resx.global_families);
                    _selectionBoxs.Add(typeof(FIN_ArticleFamily).Name, _entryBoxSelectArticleFamily);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(FIN_ArticleSubFamily)))
                {
                    _entryBoxSelectArticleSubFamily = SelectionBoxFactory<FIN_ArticleSubFamily, TreeViewArticleSubFamily>(Resx.global_subfamilies);
                    _selectionBoxs.Add(typeof(FIN_ArticleSubFamily).Name, _entryBoxSelectArticleSubFamily);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(POS_ConfigurationPlace)))
                {
                    _entryBoxSelectPlace = SelectionBoxFactory<POS_ConfigurationPlace, TreeViewConfigurationPlace>(Resx.global_places);
                    _selectionBoxs.Add(typeof(POS_ConfigurationPlace).Name, _entryBoxSelectPlace);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(POS_ConfigurationPlaceTable)))
                {
                    _entryBoxSelectPlaceTable = SelectionBoxFactory<POS_ConfigurationPlaceTable, TreeViewConfigurationPlaceTable>(Resx.global_place_tables);
                    _selectionBoxs.Add(typeof(POS_ConfigurationPlaceTable).Name, _entryBoxSelectPlaceTable);
                }

                if (ComponentExistsInQueryDialogMode(_reportsQueryDialogMode, typeof(SYS_SystemAuditType)))
                {
                    _entryBoxSelectSystemAuditType = SelectionBoxFactory<SYS_SystemAuditType, TreeViewSystemAuditType>(Resx.global_audit_type);
                    _selectionBoxs.Add(typeof(SYS_SystemAuditType).Name, _entryBoxSelectSystemAuditType);
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
