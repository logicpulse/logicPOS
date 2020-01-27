using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Enums.Reports;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Classes.Stocks;
using logicpos.resources.Resources.Localization;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using logicpos.documentviewer;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosDocumentFinanceSelectRecordDialog
    {
        //Reference to selecte Printer (Print | PrintAs Response)
        private sys_configurationprinters _printerChoosed;
        //Store Reference to Generic Printer
        private sys_configurationprinters _printerGeneric;
        //ResponseType (Above 10)
        //DialogFinanceMaster ResponseTypes
        /* IN009223 and IN009227: related to "DialogResponseType" */
        private ResponseType _responseTypePrint = (ResponseType)DialogResponseType.Print;
        private ResponseType _responseTypePrintAs = (ResponseType)DialogResponseType.PrintAs;
        private ResponseType _responseTypePayCurrentAcountsDocument = (ResponseType)DialogResponseType.PayCurrentAcountsDocument;
        private ResponseType _responseTypeNewDocument = (ResponseType)DialogResponseType.NewDocument;
        private ResponseType _responseTypePayInvoice = (ResponseType)DialogResponseType.PayInvoice;
        private ResponseType _responseTypeCancelDocument = (ResponseType)DialogResponseType.CancelDocument;
        private ResponseType _responseTypeOpenDocument = (ResponseType)DialogResponseType.OpenDocument;
        private ResponseType _responseTypeCloneDocument = (ResponseType)DialogResponseType.CloneDocument;
        private ResponseType _responseTypeSendEmailDocument = (ResponseType)DialogResponseType.SendEmailDocument;

        //Store list of Master and PaymentDocuments, created in TreeModelForEachTask_ActionGetFinanceDocumentsList/ActionGetPaymentDocumentsList
        private List<fin_documentfinancemaster> _listSelectFinanceMasterDocuments;
        private List<fin_documentfinancepayment> _listSelectFinancePaymentDocuments;
        private List<fin_documentfinancemaster> _listMarkedFinanceMasterDocuments = new List<fin_documentfinancemaster>();
        private List<fin_documentfinancepayment> _listMarkedFinancePaymentDocuments = new List<fin_documentfinancepayment>();
        private string _selectRecordWindowTitle;
        string _afterFilterTitle = null;
        //Permissions
        private bool permissionFinanceDocumentCancelDocument = FrameworkUtils.HasPermissionTo("FINANCE_DOCUMENT_CANCEL_DOCUMENT");
        //Require reference to use in TransientFor inside the TreeModelForEachTask_ActionPrintDocuments
        private PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> _dialogFinanceDocumentsResponse;
        //Used to Store Button that Call dialogFinanceMaster, usefull for ex to get buttonToken :)
        private TouchButtonIconWithText _dialogFinanceMasterCallerButton;
        //Used to store Checked Documents Customer Oid, to prevent choose Diferent Customers for Payments
        Guid _selectedDocumentEntityOid = new Guid();
        //UI
        //Reference for dialogDocumentFinanceMaster 
        PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> _dialogDocumentFinanceMaster;
        //Shared/Common for all Modes

        /// <summary>
        /// Button test 4/7/19: IN009223 IN009227
        /// </summary>
        ActionAreaButton _actionAreaButtonMore;
        string windowTitleDefault;
        
        CriteriaOperator criteriaOperatorFilter = null;

        // IN009223 IN009227
        public XPCollection DataSourceBeforeFilter { get; private set; }
        private CriteriaOperator CriteriaOperatorBase = null;
        ActionAreaButton _actionAreaButtonFilter;

        ActionAreaButton _actionAreaButtonPrintDocument;
        ActionAreaButton _actionAreaButtonPrintDocumentAs;
        ActionAreaButton _actionAreaButtonClose;
        //FinanceMasterDocuments
        ActionAreaButton _actionAreaButtonPayCurrentAcountsDocument;
        ActionAreaButton _actionAreaButtonNewDocument;
        ActionAreaButton _actionAreaButtonPayInvoice;
        ActionAreaButton _actionAreaButtonCancelDocument;
        //FinancePaymentDocuments
        ActionAreaButton _actionAreaButtonPrintPayment;
        ActionAreaButton _actionAreaButtonPrintPaymentAs;
        ActionAreaButton _actionAreaButtonOpenDocument;
        ActionAreaButton _actionAreaButtonCloneDocument;
        ActionAreaButton _actionAreaButtonSendEmailDocument;
        ActionAreaButton _actionAreaButtonCancelPayment;
        //Public Members
        decimal _totalDialogFinanceMasterDocuments = 0;
		
		// IN009223 IN009227
        private ReportsQueryDialogMode _filterChoice;

        public decimal TotalDialogFinanceMasterDocuments
        {
            get { return _totalDialogFinanceMasterDocuments; }
            set { _totalDialogFinanceMasterDocuments = value; }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: Click Event

        void touchButtonPosToolbarFinanceDocuments_Clicked(object sender, EventArgs e)
        {
            _dialogFinanceMasterCallerButton = (sender as TouchButtonIconWithText);

            //Settings

			// IN009223 IN009227
            string fileActionMore = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_more.png");
            string fileActionFilter = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_filter.png");
            
			string fileActionClose = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_close.png");
            string fileActionPrint = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_print.png");
            string fileActionNewDocument = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_toolbar_finance_new_document.png");
            string fileActionPayInvoice = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_payment_full.png");
            string fileActionCancel = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_cancel_document.png");
            bool generatePdfDocuments = Convert.ToBoolean(GlobalFramework.Settings["generatePdfDocuments"]);

            //Default/Shared ActionArea Buttons
            // IN009223 IN009227
            TouchButtonIconWithText buttonMore = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.More, "touchButtonMore_Grey", string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_more") ,SettingsApp.PaginationRowsPerPage), fileActionMore);
            TouchButtonIconWithText buttonFilter = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.Filter, "touchButtonFilter_Green", "Filter", fileActionFilter);            
			TouchButtonIconWithText buttonClose = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.Close);
            TouchButtonIconWithText buttonPrintDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.Print, "touchButtonPrintDocument_Green");
            TouchButtonIconWithText buttonPrintDocumentAs = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.PrintAs, "touchButtonPrintDocumentAs_Green");
            TouchButtonIconWithText buttonCancelDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButtonCancelDocument_Green", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_cancel_document"), _fileActionCancel);
            TouchButtonIconWithText buttonOpenDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.OpenDocument, "touchButtonOpenDocument_Green");
            TouchButtonIconWithText buttonSendEmailDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.SendEmailDocument, "touchButtonSendEmailDocument_Green");
            buttonPrintDocument.Sensitive = false;
            buttonPrintDocumentAs.Sensitive = false;
            buttonCancelDocument.Sensitive = false;
            buttonOpenDocument.Sensitive = false;
            buttonSendEmailDocument.Sensitive = false;
            //ActionArea Buttons
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();

            //Prepare Diferences for Diferent Type of Documents
            _selectRecordWindowTitle = string.Empty;
            CriteriaOperator criteriaOperator = null;
            _actionAreaButtonNewDocument = null;
            _actionAreaButtonPayInvoice = null;
            _actionAreaButtonPayCurrentAcountsDocument = null;
            _actionAreaButtonCancelDocument = new ActionAreaButton(buttonCancelDocument, _responseTypeCancelDocument);
            _actionAreaButtonSendEmailDocument = new ActionAreaButton(buttonSendEmailDocument, _responseTypeSendEmailDocument);
                        
            // IN009223 IN009227
            _actionAreaButtonMore = new ActionAreaButton(buttonMore, (ResponseType)DialogResponseType.LoadMore);
            _actionAreaButtonFilter = new ActionAreaButton(buttonFilter, (ResponseType)DialogResponseType.Filter);
            //actionAreaButtons.Add(_actionAreaButtonMore);
            //actionAreaButtons.Add(_actionAreaButtonFilter);

            //SHARED for All Modes : Add to Criteria Operator
            string criteriaOperatorShared = "(Disabled IS NULL OR Disabled  <> 1) AND ";
            //System.Windows.Forms.Application.EnableVisualStyles();
            //System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            string sqlCountTopResults = "0";
            string sqlCountResultTopResults = "0";

            switch (_dialogFinanceMasterCallerButton.Token)
            {
                //All
                case "ALL":
                    criteriaOperator = CriteriaOperator.Parse(string.Format("{0} DocumentType <> '{1}'", criteriaOperatorShared, SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput));
                    CriteriaOperatorBase = criteriaOperator;// IN009223 IN009227

                    var countResult = GlobalFramework.SessionXpo.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperatorBase);

                    if (GlobalFramework.DatabaseType.ToString() == "MySql" || GlobalFramework.DatabaseType.ToString() == "SQLite")
                    {
                        string filterCriteriaOperatorBase = CriteriaOperatorBase.ToString().Replace("[", "").Replace("]", "");
                        sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT * FROM fin_documentfinancemaster WHERE {1}) AS Total LIMIT {0};", SettingsApp.PaginationRowsPerPage, filterCriteriaOperatorBase);
                        sqlCountResultTopResults = GlobalFramework.SessionXpo.ExecuteScalar(sqlCountTopResults).ToString();
                    }
                    else if (GlobalFramework.DatabaseType.ToString() == "MSSqlServer")
                    {
                        sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT TOP {0} * FROM fin_documentfinancemaster WHERE {1}) AS Total;", SettingsApp.PaginationRowsPerPage, CriteriaOperatorBase);
                        sqlCountResultTopResults = GlobalFramework.SessionXpo.ExecuteScalar(sqlCountTopResults).ToString();
                    }
                    //if count > Pagination Row/Page 
                    if (Convert.ToInt32(sqlCountResultTopResults) > Convert.ToInt32(SettingsApp.PaginationRowsPerPage))
                    {
                        sqlCountResultTopResults = SettingsApp.PaginationRowsPerPage.ToString();
                    }
                    windowTitleDefault = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_select_finance_document");
                    string showResults = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_show_results"), sqlCountResultTopResults, countResult.ToString());
                    _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);

                    //Add aditional Buttons to ActionArea
                    TouchButtonIconWithText buttonNewDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButtonNewDocument_Green", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_new_financial_document"), fileActionNewDocument);
                    _actionAreaButtonNewDocument = new ActionAreaButton(buttonNewDocument, _responseTypeNewDocument);
                    TouchButtonIconWithText buttonCloneDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.CloneDocument, "touchButtonCloneDocument_Green");
                    buttonCloneDocument.Sensitive = false;
                    _actionAreaButtonCloneDocument = new ActionAreaButton(buttonCloneDocument, _responseTypeCloneDocument);
                    actionAreaButtons.Add(_actionAreaButtonNewDocument);
                    actionAreaButtons.Add(_actionAreaButtonCloneDocument);
                    //Start Enabled if has Open WorkSessionPeriodTerminal, checked events are Updated in event "dialogDocumentFinanceMaster_CheckBoxToggled"
                    buttonNewDocument.Sensitive = (
                        GlobalFramework.WorkSessionPeriodTerminal != null
                        && GlobalFramework.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open
                    );
                    //Add Shared Buttons
                    //_actionAreaButtonCancelDocument = new ActionAreaButton(buttonCancelDocument, _responseTypeCancelDocument);
                    actionAreaButtons.Add(_actionAreaButtonCancelDocument);
                    _filterChoice = ReportsQueryDialogMode.FILTER_DOCUMENTS_PAGINATION;// IN009223 IN009227
                    break;
                // Payments
                case "FT_UNPAYED":
                    /* IN009067 - NC removed from this window */
                    /* IN009182 - option #1 */
                    // string criteriaOperatorString = string.Format("{0} (DocumentType = '{1}' OR DocumentType = '{2}' OR DocumentType = '{3}') AND Payed = 0 AND DocumentStatusStatus <> 'A'", criteriaOperatorShared, SettingsApp.XpoOidDocumentFinanceTypeInvoice, SettingsApp.XpoOidDocumentFinanceTypeCreditNote, SettingsApp.XpoOidDocumentFinanceTypeDebitNote);
                    /* IN009182 - option #2 */
					
					//[IN:009337] Documentos - Não apresenta documentos a liquidar [SQLite] 
                    string payed = " ";
                    if (GlobalFramework.DatabaseType.ToString() == "SQLite")
                    {
                        payed = "0";
                    }
                    else { payed = "False"; }
                    string criteriaOperatorString = string.Format("{0} DocumentType = '{1}' AND DocumentStatusStatus <> 'A' AND Payed = '{3}'", criteriaOperatorShared, SettingsApp.XpoOidDocumentFinanceTypeInvoice, SettingsApp.XpoOidDocumentFinanceTypeCreditNote, payed);
                    criteriaOperator = CriteriaOperator.Parse(criteriaOperatorString);

                    /* IN009067 - This query will retrieve all documents pendent of payment, partial or total */
                    string sql = @"
SELECT
    DocFinMaster.Oid
FROM
    fin_documentfinancemaster DocFinMaster
LEFT JOIN
    viewdocumentfinancepaymenttotals AS DocFinPayTotals ON(DocFinPayTotals.DocumentOid = DocFinMaster.Oid)
WHERE
    (
        (
            (SELECT A.Total FROM viewdocumentfinancepaymenttotals AS A WHERE A.DocumentOid = DocFinMaster.Oid) < DocFinMaster.TotalFinal
            OR 
            (SELECT A.Total FROM viewdocumentfinancepaymenttotals AS A WHERE A.DocumentOid = DocFinMaster.Oid) IS NULL            
        )
        AND {0}
    )
";


                    //                string sql2 = string.Format(@"
                    //SELECT 
                    // (
                    //  DocFinMaster.TotalFinal - (
                    //   SELECT 
                    //       SUM(CreditAmount) + SUM(CreditInvoiceTotal) as CreditTotals
                    //   FROM 
                    //       view_documentfinancepaymentdocumenttotal 
                    //   WHERE 
                    //       DocumentPayed = '{0}'
                    //  )
                    // ) AS CreditTotal
                    //FROM 
                    //    fin_documentfinancemaster DocFinMaster
                    //WHERE 
                    //    DocFinMaster.Oid = '{0}';", documentFinanceMaster.Oid);

                    //documentDebit = Convert.ToDecimal(GlobalFramework.SessionXpo.ExecuteScalar(sql));
                    /* IN009067 - creates a list of documents not payed, total or partial either */

     

                    var sqlResult = GlobalFramework.SessionXpo.ExecuteQuery(string.Format(sql, criteriaOperatorString));
                    List<Guid> resultList = new List<Guid>();
                    string inClause = string.Empty;
                    foreach (var item in sqlResult.ResultSet[0].Rows)
                    {
                        resultList.Add(new Guid(Convert.ToString(item.Values[0])));
                        inClause += "'" + (Convert.ToString(item.Values[0])) + "', ";
                    }
                    string countQuery = string.Empty;            

                    //List<Guid> resultListfilter = new List<Guid>();
                    //SortingCollection sortCollection = new SortingCollection();
                    //sortCollection.Add(new SortProperty("Oid", DevExpress.Xpo.DB.SortingDirection.Ascending));
                    //CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
                    //ICollection collectionCustomers = GlobalFramework.SessionXpo.GetObjects(GlobalFramework.SessionXpo.GetClassInfo(typeof(fin_documentfinancemaster)), criteria, sortCollection, int.MaxValue, false, true);
                    //inClause = string.Empty;
                    //foreach (fin_documentfinancemaster item in collectionCustomers)
                    //{
                    //    for (int i = 0; i < resultList.Count; i++)
                    //        if (item.Oid.ToString() == resultList[i].ToString())
                    //        {
                    //            if (item.DocumentParent != null && item.DocumentType.Oid == SettingsApp.XpoOidDocumentFinanceTypeCreditNote)
                    //            {
                    //                if (item.TotalFinal < item.DocumentParent.TotalFinal)
                    //                {
                    //                    resultListfilter.Add(new Guid(Convert.ToString(item.Oid)));
                    //                    inClause += "'" + (Convert.ToString(item.Oid)) + "', ";
                    //                }
                    //            }
                    //            else
                    //            {
                    //                resultListfilter.Add(new Guid(Convert.ToString(item.Oid)));
                    //                inClause += "'" + (Convert.ToString(item.Oid)) + "', ";
                    //            }
                    //        }
                    //}

                    //if (!string.IsNullOrEmpty(inClause))
                    //{
                    //    inClause = inClause.Remove(inClause.Length - 2);
                    //    countQuery = string.Format(" And Oid In ( {0} )", inClause);
                    //}



                    countResult = GlobalFramework.SessionXpo.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse(string.Format("{0} {1}", criteriaOperatorString, countQuery)));

                    countResult = resultList.Count.ToString();

                    if (GlobalFramework.DatabaseType.ToString() == "MySql" || GlobalFramework.DatabaseType.ToString() == "SQLite")
                    {
                        string filterCriteriaOperatorBase = criteriaOperatorString.ToString().Replace("[", "").Replace("]", "");
                        sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT * FROM fin_documentfinancemaster WHERE {1} {2}) AS Total LIMIT {0};", SettingsApp.PaginationRowsPerPage, filterCriteriaOperatorBase, countQuery);
                        sqlCountResultTopResults = GlobalFramework.SessionXpo.ExecuteScalar(sqlCountTopResults).ToString();
                    }
                    else if (GlobalFramework.DatabaseType.ToString() == "MSSqlServer")
                    {
                        sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT TOP {0} * FROM fin_documentfinancemaster WHERE {1} {2}) AS Total;", SettingsApp.PaginationRowsPerPage, criteriaOperatorString, countQuery);
                        sqlCountResultTopResults = GlobalFramework.SessionXpo.ExecuteScalar(sqlCountTopResults).ToString();
                    }


                    criteriaOperator = GroupOperator.And(criteriaOperator, new InOperator("Oid", resultList));
                    CriteriaOperatorBase = criteriaOperator;// IN009223 IN009227

                    windowTitleDefault = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_select_finance_document_ft_unpaid");
                    showResults = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_show_results"), sqlCountResultTopResults, countResult);
                    if (!string.IsNullOrEmpty(showResults))
                    {
                        _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);
                    }
                    else
                    {
                        _selectRecordWindowTitle = string.Format("{0} ::", windowTitleDefault);
                    }

                    //Add aditional Buttons to ActionArea
                    TouchButtonIconWithText buttonPayInvoice = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButtonPayInvoice_Green", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_pay_invoice"), fileActionPayInvoice);
                    buttonPayInvoice.Sensitive = false;
                    _actionAreaButtonPayInvoice = new ActionAreaButton(buttonPayInvoice, _responseTypePayInvoice);
                    actionAreaButtons.Add(_actionAreaButtonPayInvoice);
                    //Add Shared Buttons
                    //_actionAreaButtonCancelDocument = new ActionAreaButton(buttonCancelDocument, _responseTypeCancelDocument);
                    actionAreaButtons.Add(_actionAreaButtonCancelDocument);
                    _filterChoice = Enums.Reports.ReportsQueryDialogMode.FILTER_DOCUMENTS_UNPAYED;// IN009223 IN009227
                    break;
                // CurrentAccount
                case "CC":
                    criteriaOperator = CriteriaOperator.Parse(string.Format("{0} DocumentType = '{1}' AND Payed = 0", criteriaOperatorShared, SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput));
                    CriteriaOperatorBase = criteriaOperator;// IN009223 IN009227

                    countResult = GlobalFramework.SessionXpo.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperatorBase);

                    if (GlobalFramework.DatabaseType.ToString() == "MySql" || GlobalFramework.DatabaseType.ToString() == "SQLite")
                    {
                        string filterCriteriaOperatorBase = CriteriaOperatorBase.ToString().Replace("[", "").Replace("]", "");
                        sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT * FROM fin_documentfinancemaster WHERE {1}) AS Total LIMIT {0};", SettingsApp.PaginationRowsPerPage, filterCriteriaOperatorBase);
                        sqlCountResultTopResults = GlobalFramework.SessionXpo.ExecuteScalar(sqlCountTopResults).ToString();
                    }
                    else if (GlobalFramework.DatabaseType.ToString() == "MSSqlServer")
                    {
                        sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT TOP {0} * FROM fin_documentfinancemaster WHERE {1}) AS Total;", SettingsApp.PaginationRowsPerPage, CriteriaOperatorBase);
                        sqlCountResultTopResults = GlobalFramework.SessionXpo.ExecuteScalar(sqlCountTopResults).ToString();
                    }



                    windowTitleDefault = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_select_finance_document_cc");
                    showResults = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_show_results"), sqlCountResultTopResults, countResult);
                    _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);

                    //Add aditional Buttons to ActionArea
                    TouchButtonIconWithText buttonPayCurrentAcountDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButtonPayCurrentAcountDocument_Green", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_pay"), fileActionPayInvoice);
                    buttonPayCurrentAcountDocument.Sensitive = false;
                    _actionAreaButtonPayCurrentAcountsDocument = new ActionAreaButton(buttonPayCurrentAcountDocument, _responseTypePayCurrentAcountsDocument);
                    actionAreaButtons.Add(_actionAreaButtonPayCurrentAcountsDocument);
                    //Add Shared Buttons
                    //_actionAreaButtonCancelDocument = new ActionAreaButton(buttonCancelDocument, _responseTypeCancelDocument);
                    actionAreaButtons.Add(_actionAreaButtonCancelDocument);
                    _filterChoice = Enums.Reports.ReportsQueryDialogMode.FILTER_PAYMENT_DOCUMENTS;// IN009223 IN009227
                    break;
                default:
                    break;
            }

            //Add references to Send to Event CursorChanged
            _actionAreaButtonPrintDocument = new ActionAreaButton(buttonPrintDocument, _responseTypePrint);
            _actionAreaButtonPrintDocumentAs = new ActionAreaButton(buttonPrintDocumentAs, _responseTypePrintAs);
            _actionAreaButtonOpenDocument = new ActionAreaButton(buttonOpenDocument, _responseTypeOpenDocument);
            _actionAreaButtonClose = new ActionAreaButton(buttonClose, ResponseType.Close);
            actionAreaButtons.Add(_actionAreaButtonPrintDocument);
            actionAreaButtons.Add(_actionAreaButtonPrintDocumentAs);
            if (generatePdfDocuments) actionAreaButtons.Add(_actionAreaButtonOpenDocument);
            actionAreaButtons.Add(_actionAreaButtonSendEmailDocument);
            actionAreaButtons.Add(_actionAreaButtonClose);
            //Reset totalDialogFinanceMasterDocuments
            _totalDialogFinanceMasterDocuments = 0;
            _dialogDocumentFinanceMaster = new PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster>(
                this,
                DialogFlags.DestroyWithParent,
                _selectRecordWindowTitle,
                //TODO:THEME
                GlobalApp.MaxWindowSize,
                null, //XpoDefaultValue
                criteriaOperator,
                GenericTreeViewMode.CheckBox,
                actionAreaButtons
              );

            //Events
            _dialogDocumentFinanceMaster.Response += dialogFinanceMasterDocuments_Response;
            //CheckBox Capture CursorChanged/CheckBoxToggled Event, And enable/disable Buttons based on Valid Selection, Must be Here, Where we have a refence to Buttons
            _dialogDocumentFinanceMaster.CheckBoxToggled += dialogDocumentFinanceMaster_CheckBoxToggled;

            //Call Dialog
            int response = _dialogDocumentFinanceMaster.Run();

            _dialogDocumentFinanceMaster.Destroy();
        }

        void dialogDocumentFinanceMaster_CheckBoxToggled(object sender, EventArgs e)
        {
            bool itemChecked;
            decimal documentValue = 0.0m;
            decimal documentDebit = 0.0m;
            fin_documentfinancemaster documentFinanceMaster = null;
            bool validMarkedDocumentTypesForCloneSensitive = false;
            bool validMarkedDocumentTypesForSendEmailSensitive = false;

            try
            {
                //WorkStation : Store Session Open/Close Status, some operations must be disable when Session is Closed, like Payment Operations
                bool hasOpenWorkStation = (GlobalFramework.WorkSessionPeriodTerminal != null && GlobalFramework.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open);

                //Default Mode 
                if (_dialogDocumentFinanceMaster.GenericTreeViewMode == GenericTreeViewMode.Default)
                {
                    //XPOMode
                    if (_dialogDocumentFinanceMaster.GenericTreeView.DataSourceRow != null)
                    {
                        documentFinanceMaster = (fin_documentfinancemaster)_dialogDocumentFinanceMaster.GenericTreeView.DataSourceRow;
                        _totalDialogFinanceMasterDocuments = documentFinanceMaster.TotalFinal;
                        //Enable/Disable Buttons
                        _actionAreaButtonPrintDocument.Button.Sensitive = hasOpenWorkStation;
                        _actionAreaButtonPrintDocumentAs.Button.Sensitive = hasOpenWorkStation;
                        if (_actionAreaButtonPayInvoice != null) _actionAreaButtonPayInvoice.Button.Sensitive = hasOpenWorkStation;
                        if (_actionAreaButtonPayCurrentAcountsDocument != null) _actionAreaButtonPayCurrentAcountsDocument.Button.Sensitive = hasOpenWorkStation;
                        if (_actionAreaButtonCancelDocument != null) _actionAreaButtonCancelDocument.Button.Sensitive = permissionFinanceDocumentCancelDocument;
                    }
                    else
                    {
                        //Reset Total
                        _totalDialogFinanceMasterDocuments = 0.0m;
                        //Enable/Disable Buttons
                        _actionAreaButtonPrintDocument.Button.Sensitive = false;
                        _actionAreaButtonPrintDocumentAs.Button.Sensitive = false;
                        if (_actionAreaButtonPayInvoice != null) _actionAreaButtonPayInvoice.Button.Sensitive = false;
                        if (_actionAreaButtonPayCurrentAcountsDocument != null) _actionAreaButtonPayCurrentAcountsDocument.Button.Sensitive = false;
                        if (_actionAreaButtonCancelDocument != null) _actionAreaButtonCancelDocument.Button.Sensitive = false;
                    }
                }
                //CheckBox Mode
                else if (_dialogDocumentFinanceMaster.GenericTreeViewMode == GenericTreeViewMode.CheckBox)
                {
                    if (_dialogDocumentFinanceMaster.GenericTreeView.MarkedCheckBoxs > 0)
                    {
                        //Get value from Model, its Outside XPGuidObject Scope
                        itemChecked = (bool)_dialogDocumentFinanceMaster.GenericTreeView.GetCurrentModelCheckBoxValue();
                        documentFinanceMaster = (fin_documentfinancemaster)_dialogDocumentFinanceMaster.GenericTreeView.DataSourceRow;

                        // Add/Remove MarkedFinanceMasterDocuments on click/mark Document
                        if (itemChecked)
                        {
                            _listMarkedFinanceMasterDocuments.Add(documentFinanceMaster);
                            //_log.Debug(string.Format("_listMarkedFinanceMasterDocuments count: [{0}], Added: [{1}]", _listMarkedFinanceMasterDocuments.Count, documentFinanceMaster.DocumentNumber));
                        }
                        else
                        {
                            _listMarkedFinanceMasterDocuments.Remove(documentFinanceMaster);
                            //_log.Debug(string.Format("_listMarkedFinanceMasterDocuments count: [{0}], Removed: [{1}]", _listMarkedFinanceMasterDocuments.Count, documentFinanceMaster.DocumentNumber));
                        }

                        // Get Sensitive for Clone : Required for actionAreaButtonCloneDocument.Button.Sensitive
                        validMarkedDocumentTypesForCloneSensitive = GetSensitiveForCloneDocuments(_listMarkedFinanceMasterDocuments);

                        // Get Sensitive for SendEmail
                        validMarkedDocumentTypesForSendEmailSensitive = GetSensitiveForSendEmailDocuments(_listMarkedFinanceMasterDocuments);

                        //Customer Protection (Payments) to prevent Choosing Diferent Customers in MultiSelect
                        if (_dialogFinanceMasterCallerButton.Token == "FT_UNPAYED")
                        {
                            if (_selectedDocumentEntityOid == new Guid())
                            {
                                _selectedDocumentEntityOid = documentFinanceMaster.EntityOid;
                            }
                            else if (_selectedDocumentEntityOid != documentFinanceMaster.EntityOid)
                            {
                                //Reset Checked to Uncheck when choose Diferent Customer
                                _dialogDocumentFinanceMaster.GenericTreeView.MarkedCheckBoxs--;
                                _dialogDocumentFinanceMaster.GenericTreeView.ListStoreModel.SetValue(_dialogDocumentFinanceMaster.GenericTreeView.TreeIterModel, 1, false);
                                return;
                            }
                        }
                        /* IN009166 - as defined, only non-canceled documents must be part of the calculus for selected documents */
                        if (!"A".Equals(documentFinanceMaster.DocumentStatusStatus))
                        {
                            //Get documentValue from Document.TotalFinal to Increment/Decrement base on Credit Bool, ex Credit=True Value=100 else if Credit=False Value=-100
                            if (documentFinanceMaster.DocumentType.Credit)
                            {
                                /* IN009166 - Making Total calculation based on window option selected 
                                 * "FT_UNPAYED": Payment Documents
                                 */
                                if ("FT_UNPAYED".Equals(_dialogFinanceMasterCallerButton.Token))
                                {

                                    /* IN009166 - removing all old implementations */
                                    //This Query Exists 3 Locations, Find it and change in all Locations - Required "GROUP BY fmaOid,fmaTotalFinal" to work with SQLServer
                                    //string sql = string.Format("SELECT fmaTotalFinal - SUM(fmpCreditAmount) as Result FROM view_documentfinancepayment WHERE fmaOid = '{0}' AND fpaPaymentStatus <> 'A' GROUP BY fmaOid,fmaTotalFinal;", documentFinanceMaster.Oid);
                                    /* IN009067 - Fixing "Total entregue" default value when paying financial documents */
                                    /* IN009152 - fix for selected documents total */
                                    string sql = string.Format(@"
    SELECT 
	    (
		    DocFinMaster.TotalFinal - (
			    SELECT 
			        SUM(CreditAmount) + SUM(CreditInvoiceTotal) as CreditTotals
			    FROM 
			        view_documentfinancepaymentdocumenttotal 
			    WHERE 
			        DocumentPayed = '{0}'
		    )
	    ) AS CreditTotal
    FROM 
        fin_documentfinancemaster DocFinMaster
    WHERE 
        DocFinMaster.Oid = '{0}';", documentFinanceMaster.Oid);

                                    documentDebit = Convert.ToDecimal(GlobalFramework.SessionXpo.ExecuteScalar(sql));
                                    /* IN009166 - forced to use "TotalFinalRound" and not "TotalFinal" instead */
                                    documentValue = (documentDebit != 0) ? documentDebit : documentFinanceMaster.TotalFinal;
                                }
                                else
                                {
                                    /* IN009166 - Total of Documents selected
                                     * When getting TotalFinal, the calculation returns a value different than the one is being shown.
                                     * Then, we selected TotalFinalRound as show on Documents window for each register.
                                     */
                                    // documentDebit = Convert.ToDecimal(GlobalFramework.SessionXpo.ExecuteScalar(sql));
                                    // documentValue = (documentDebit != 0) ? documentDebit : documentFinanceMaster.TotalFinal;
                                    documentValue = documentFinanceMaster.TotalFinal;
                                }
                            }
                            else
                            {
                                /* IN009166 */
                                documentValue = -documentFinanceMaster.TotalFinal;
                            };
                            _totalDialogFinanceMasterDocuments += (itemChecked) ? documentValue : -documentValue;
                        }

                        //Enable/Disable Buttons for all Modes
                        _actionAreaButtonPrintDocument.Button.Sensitive = true;
                        _actionAreaButtonPrintDocumentAs.Button.Sensitive = true;
                        if (!GlobalFramework.AppUseParkingTicketModule)
                        {
                            _actionAreaButtonOpenDocument.Button.Sensitive = true;
                        }
                        _actionAreaButtonFilter.Button.Sensitive = true;
                        _actionAreaButtonMore.Button.Sensitive = true;
                        if (_actionAreaButtonCloneDocument != null) _actionAreaButtonCloneDocument.Button.Sensitive = validMarkedDocumentTypesForCloneSensitive;
                        _actionAreaButtonSendEmailDocument.Button.Sensitive = validMarkedDocumentTypesForSendEmailSensitive;
                        if (_actionAreaButtonPayCurrentAcountsDocument != null && _dialogFinanceMasterCallerButton.Token == "CC") _actionAreaButtonPayCurrentAcountsDocument.Button.Sensitive = hasOpenWorkStation;
                        //Must Greater than zero to Pay Something or Zero to Issue Zero Payment Document and Set Payed to Documents
                        if (_actionAreaButtonPayInvoice != null && _dialogFinanceMasterCallerButton.Token == "FT_UNPAYED") _actionAreaButtonPayInvoice.Button.Sensitive = (hasOpenWorkStation && _totalDialogFinanceMasterDocuments >= 0);
                        //Cancel Documents must me in ALL, CC, or FT_UNPAYED Mode 
                        if (_actionAreaButtonCancelDocument != null && (
                                _dialogFinanceMasterCallerButton.Token == "ALL" ||
                                _dialogFinanceMasterCallerButton.Token == "FT_UNPAYED" ||
                                _dialogFinanceMasterCallerButton.Token == "CC"
                                )
                                && permissionFinanceDocumentCancelDocument
                            )
                            _actionAreaButtonCancelDocument.Button.Sensitive = true;
                    }
                    else
                    {
                        //Reset Helper Vars
                        _totalDialogFinanceMasterDocuments = 0.0m;
                        //Reset Selected Customer to Blank
                        _selectedDocumentEntityOid = new Guid();
                        //Enable/Disable Buttons for all Modes
                        _actionAreaButtonPrintDocument.Button.Sensitive = false;
                        _actionAreaButtonPrintDocumentAs.Button.Sensitive = false;
                        _actionAreaButtonOpenDocument.Button.Sensitive = false;
                        _actionAreaButtonSendEmailDocument.Button.Sensitive = false;
                        /* IN009067 - solving "NullReferenceException" issue */
                        if (_actionAreaButtonCloneDocument != null) { _actionAreaButtonCloneDocument.Button.Sensitive = false; }
                        if (_actionAreaButtonPayCurrentAcountsDocument != null) _actionAreaButtonPayCurrentAcountsDocument.Button.Sensitive = false;
                        if (_actionAreaButtonPayInvoice != null) _actionAreaButtonPayInvoice.Button.Sensitive = false;
                        if (_actionAreaButtonCancelDocument != null) _actionAreaButtonCancelDocument.Button.Sensitive = false;
                        // Require to Empty listMarkedFinanceMasterDocuments (MarkedCheckBoxs > 0)
                        _listMarkedFinanceMasterDocuments.Clear();
                    }
                }
                CriteriaOperator criteriaCount = null;

                if (ReferenceEquals(criteriaOperatorFilter, null))
                {
                    criteriaCount = CriteriaOperatorBase;
                }
                else
                {
                    criteriaCount = criteriaOperatorFilter;
                }

                var countResult = GlobalFramework.SessionXpo.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), criteriaCount);
                string nDocs = _dialogDocumentFinanceMaster.GenericTreeView.DataSource.Count.ToString();
                string showResults = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_show_results"), nDocs, countResult);

                //Finish Updating Title
                _dialogDocumentFinanceMaster.WindowTitle = (_totalDialogFinanceMasterDocuments != 0) ? string.Format("{0} :: {1} - {2}", windowTitleDefault, FrameworkUtils.DecimalToStringCurrency(_totalDialogFinanceMasterDocuments), showResults) : string.Format("{0} :: {1}", windowTitleDefault, showResults);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: DialogDocumentFinanceMaster Response

        void dialogFinanceMasterDocuments_Response(object o, ResponseArgs args)
        {
            //Get Sender Reference : require for use Transient
            _dialogFinanceDocumentsResponse = (PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster>)o;

            fin_documentfinancemaster documentFinanceMaster = (fin_documentfinancemaster)_dialogFinanceDocumentsResponse.GenericTreeView.DataSourceRow;

            if (args.ResponseId != ResponseType.Close)
            {
                if (
                        (args.ResponseId == _responseTypePrint && !Utils.ShowMessageTouchRequiredValidPrinter(_dialogFinanceDocumentsResponse))
                        || args.ResponseId == _responseTypePrintAs
                    )
                {
                    //Assign Choosed Printer based on user ResponseType
                    //TK016249 - Impressoras - Diferenciação entre Tipos (helio)
                    if (documentFinanceMaster.SourceOrderMain == null) { _printerChoosed = (args.ResponseId == _responseTypePrint) ? GlobalFramework.LoggedTerminal.Printer : _printerGeneric; }
                    else { _printerChoosed = (args.ResponseId == _responseTypePrint) ? GlobalFramework.LoggedTerminal.ThermalPrinter : _printerGeneric; }


                    //Single Record Mode - Default - USED HERE ONLY TO TEST Both Dialogs Modes (Default and CheckBox)
                    if (_dialogFinanceDocumentsResponse.GenericTreeViewMode == GenericTreeViewMode.Default)
                    {
                        FrameworkCalls.PrintFinanceDocument(this, _printerChoosed, documentFinanceMaster);
                    }
                    //Multi Record Mode - CheckBox - ACTIVE MODE
                    else if (_dialogFinanceDocumentsResponse.GenericTreeViewMode == GenericTreeViewMode.CheckBox)
                    {
                        //Required to use ListStoreModel and not ListStoreModelFilterSort, we only loop the visible filtered rows, and not The hidden Checked Rows
                        _dialogFinanceDocumentsResponse.GenericTreeView.ListStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask_ActionPrintDocuments));
                        //UnCheck all Marked CheckBoxs
                        UnCheckAll_FinanceMasterDocuments(_dialogFinanceDocumentsResponse, false);
                    }
                }
                //Shared for all Modes that required a _listSelectFinanceMasterDocuments
                else if (
                    args.ResponseId == _responseTypePayCurrentAcountsDocument ||
                    args.ResponseId == _responseTypePayInvoice ||
                    args.ResponseId == _responseTypeCancelDocument ||
                    args.ResponseId == _responseTypeOpenDocument ||
                    args.ResponseId == _responseTypeCloneDocument ||
                    args.ResponseId == _responseTypeSendEmailDocument
                )
                {
                    _listSelectFinanceMasterDocuments = new List<fin_documentfinancemaster>();

                    //Single Record Mode - Default - USED HERE ONLY TO TEST Both Dialogs Modes (Default and CheckBox)
                    if (_dialogFinanceDocumentsResponse.GenericTreeViewMode == GenericTreeViewMode.Default)
                    {
                        _listSelectFinanceMasterDocuments.Add(documentFinanceMaster);
                    }
                    //Multi Record Mode - CheckBox - ACTIVE MODE
                    else if (_dialogFinanceDocumentsResponse.GenericTreeViewMode == GenericTreeViewMode.CheckBox)
                    {
                        //Fill _listPayDocuments in ForEachFunc
                        //Required to use ListStoreModel and not ListStoreModelFilterSort, we only loop the visible filtered rows, and not The hidden Checked Rows
                        _dialogFinanceDocumentsResponse.GenericTreeView.ListStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask_ActionGetFinanceDocumentsList));
                    }

                    //Used to Check if Methods return a Valid Return Document, ex when Dialog OK Pressed
                    object resultDocument = null;

                    //Send to Method based on Response
                    if (args.ResponseId == _responseTypePayCurrentAcountsDocument)
                    {
                        //Start Processing Documents
                        resultDocument = PayCurrentAcountDocuments(_dialogFinanceDocumentsResponse, _listSelectFinanceMasterDocuments);
                        //UnCheck all Marked CheckBoxs
                        if (resultDocument != null) UnCheckAll_FinanceMasterDocuments(_dialogFinanceDocumentsResponse, false);
                    }
                    //Pay Invoices
                    else if (args.ResponseId == _responseTypePayInvoice)
                    {
                        //Start Processing Documents
                        resultDocument = CallPayInvoicesDialog(_dialogFinanceDocumentsResponse, _totalDialogFinanceMasterDocuments);
                        //UnCheck all Marked CheckBoxs
                        if (resultDocument != null) UnCheckAll_FinanceMasterDocuments(_dialogFinanceDocumentsResponse, false);

                        //Update Window title and reset filter if zero results in filter after pay invoices
                        _dialogFinanceDocumentsResponse.GenericTreeView.DataSource.Criteria = CriteriaOperatorBase;
                        _dialogFinanceDocumentsResponse.GenericTreeView.DataSource.TopReturnedObjects = SettingsApp.PaginationRowsPerPage * _dialogFinanceDocumentsResponse.GenericTreeView.CurrentPageNumber;
                        _dialogFinanceDocumentsResponse.GenericTreeView.Refresh();
                        string nDocs = _dialogDocumentFinanceMaster.GenericTreeView.DataSource.Count.ToString();

                        if (nDocs == "0")
                        {
                            _dialogFinanceDocumentsResponse.GenericTreeView.DataSource.Criteria = CriteriaOperatorBase;
                            _dialogFinanceDocumentsResponse.GenericTreeView.DataSource.TopReturnedObjects = SettingsApp.PaginationRowsPerPage * _dialogFinanceDocumentsResponse.GenericTreeView.CurrentPageNumber;
                            _dialogFinanceDocumentsResponse.GenericTreeView.Refresh();
                            criteriaOperatorFilter = null;
                            _afterFilterTitle = null;

                            var countResult = GlobalFramework.SessionXpo.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperatorBase);
                            nDocs = _dialogDocumentFinanceMaster.GenericTreeView.DataSource.Count.ToString();
                            var showResults = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_show_results"), nDocs, countResult);
                            _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);
                            _dialogDocumentFinanceMaster.WindowTitle = _selectRecordWindowTitle;
                        }
                        else
                        {
                            var countResult = GlobalFramework.SessionXpo.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), criteriaOperatorFilter);
                            string showResults = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_show_results"), nDocs, countResult);
                            _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);
                            _dialogDocumentFinanceMaster.WindowTitle = _selectRecordWindowTitle;
                            _afterFilterTitle = null;
                        }

                        //end update
                    }
                    //Cancel Documents
                    else if (args.ResponseId == _responseTypeCancelDocument)
                    {
                        //Start Processing Documents
                        CallCancelFinanceMasterDocumentsDialog(_dialogFinanceDocumentsResponse, _listSelectFinanceMasterDocuments);
                        //UnCheck all Marked CheckBoxs, After call CallCancelDocumentsDialog
                        UnCheckAll_FinanceMasterDocuments(_dialogFinanceDocumentsResponse, true);
                    }
                    //Open Documents
                    else if (args.ResponseId == _responseTypeOpenDocument)
                    {
                        //Start Open Documents
                        OpenFinanceMasterDocuments(_listSelectFinanceMasterDocuments);
                        //UnCheck all Marked CheckBoxs, After call CallCancelDocumentsDialog
                        UnCheckAll_FinanceMasterDocuments(_dialogFinanceDocumentsResponse, true);
                    }
                    //Clone Documents
                    else if (args.ResponseId == _responseTypeCloneDocument)
                    {
                        //Compose document list
                        string documentList = string.Empty;
                        int i = 0;
                        foreach (fin_documentfinancemaster item in _listSelectFinanceMasterDocuments)
                        {
                            i++;
                            documentList += string.Format("- {0}{1}", item.DocumentNumber, (i < _listSelectFinanceMasterDocuments.Count) ? Environment.NewLine : string.Empty);
                        }

                        // Call Dialog
                        ResponseType dialogResponse = Utils.ShowMessageTouch(_dialogFinanceDocumentsResponse, DialogFlags.Modal, new Size(700, 440), MessageType.Question, ButtonsType.OkCancel, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_question"),
                            string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_clone_documents_confirmation"), documentList)
                        );

                        if (dialogResponse == ResponseType.Ok)
                        {
                            //Start Open Documents
                            CallCloneFinanceMasterDocuments(_dialogFinanceDocumentsResponse, _listSelectFinanceMasterDocuments);

                            //UnCheck all Marked CheckBoxs, After call CallCancelDocumentsDialog
                            UnCheckAll_FinanceMasterDocuments(_dialogFinanceDocumentsResponse, true);

                            //IN009255 Usabilidade - Opção de Copiar Documentos não apresenta mensagem de sucesso ao usuário. 
                            //Mensagem de sucesso clone
                            Utils.ShowMessageTouch(this, DialogFlags.DestroyWithParent | DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_clone_document"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_operation_successfully"));
                        }
                    }
                    //SendEmail Documents
                    else if (args.ResponseId == _responseTypeSendEmailDocument)
                    {
                        //Start Send Email Documents
                        ResponseType responseType = CallSendEmailFinanceMasterDocuments(_dialogFinanceDocumentsResponse, _listSelectFinanceMasterDocuments);
                        if (responseType == ResponseType.Ok)
                        {
                            //UnCheck all Marked CheckBoxs, After call CallCancelDocumentsDialog
                            UnCheckAll_FinanceMasterDocuments(_dialogFinanceDocumentsResponse, true);
                        }
                    }

                    //COMMON : Always refresh TreeView, IF Dialog Returns a Valid Document/OK
                    if (resultDocument != null)
                    {
                        //Refresh Treeview
                        _dialogFinanceDocumentsResponse.GenericTreeView.Refresh();
                        //Reset CheckBoxs
                        _dialogFinanceDocumentsResponse.GenericTreeView.MarkedCheckBoxs = 0;
                        //Update Total
                        _totalDialogFinanceMasterDocuments = 0.0m;
                        //Reset Customer
                        _selectedDocumentEntityOid = new Guid();
                        //Finish Updating Title
                        string nDocs = _dialogDocumentFinanceMaster.GenericTreeView.DataSource.Count.ToString();
                        string nDocsTotal = _dialogDocumentFinanceMaster.GenericTreeView.DataSource.Count.ToString();

                        _dialogDocumentFinanceMaster.WindowTitle = string.Format("{0} - Numero de Documentos: {1}", _selectRecordWindowTitle, nDocs);
                        _dialogFinanceDocumentsResponse.WindowTitle = _selectRecordWindowTitle;
                        //Disable Buttons
                        if (_actionAreaButtonPayCurrentAcountsDocument != null) _actionAreaButtonPayCurrentAcountsDocument.Button.Sensitive = false;
                        if (_actionAreaButtonPayInvoice != null) _actionAreaButtonPayInvoice.Button.Sensitive = false;
                    }
                }
                else if (args.ResponseId == _responseTypeNewDocument)
                {
                    //Call New DocumentFinance Dialog
                    PosDocumentFinanceDialog dialogNewDocument = new PosDocumentFinanceDialog(_dialogFinanceDocumentsResponse, DialogFlags.DestroyWithParent);
                    ResponseType responseNewDocument = (ResponseType)dialogNewDocument.Run();

                    if (responseNewDocument == ResponseType.Ok)
                    {
                        //Always refresh TreeView, After Valid Payment
                        _dialogFinanceDocumentsResponse.GenericTreeView.Refresh();
                    }
                    dialogNewDocument.Destroy();
                }
                else if (args.ResponseId == _responseTypePayInvoice)
                {
                    //USE PAYMENTS DIALOG and IF RETURNS OK REFRESH TREE
                    _log.Debug("ResponseId == _responseTypePayInvoice");
                }// IN009223 IN009227 - Begin
                else if (args.ResponseId == (ResponseType)DialogResponseType.LoadMore)
                {

                    _dialogFinanceDocumentsResponse.GenericTreeView.DataSource.TopReturnedObjects = (SettingsApp.PaginationRowsPerPage * _dialogFinanceDocumentsResponse.GenericTreeView.CurrentPageNumber);
                    _dialogFinanceDocumentsResponse.GenericTreeView.Refresh();

                    CriteriaOperator criteriaCount = null;

                    if (ReferenceEquals(criteriaOperatorFilter, null))
                    {
                        criteriaCount = CriteriaOperatorBase;
                    }
                    else
                    {
                        criteriaCount = criteriaOperatorFilter;
                    }

                    var countResult = GlobalFramework.SessionXpo.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), criteriaCount);
                    string nDocs = _dialogDocumentFinanceMaster.GenericTreeView.DataSource.Count.ToString();
                    string showResults = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_show_results"), nDocs, countResult);

                    _dialogDocumentFinanceMaster.WindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);
                    //_dialogDocumentFinanceMaster.WindowTitle = _selectRecordWindowTitle;
                }
                else if (args.ResponseId == (ResponseType)DialogResponseType.Filter)
                {
                    //Reset current page to 1 ( Pagination go to defined initialy )
                    _dialogFinanceDocumentsResponse.GenericTreeView.CurrentPageNumber = 1;

                    // Filter SellDocuments
                    string filterField = string.Empty;
                    string statusField = string.Empty;
                    string extraFilter = string.Empty;

                    List<string> result = new List<string>();

                    PosReportsQueryDialog dialog = new PosReportsQueryDialog(_dialogFinanceDocumentsResponse, DialogFlags.Modal, _filterChoice, "fin_documentfinancemaster");
                    DialogResponseType response = (DialogResponseType)dialog.Run();

                    //IF button Clean Filter Clicked
                    if (DialogResponseType.CleanFilter.Equals(response))
                    {
                        _dialogFinanceDocumentsResponse.GenericTreeView.DataSource.Criteria = CriteriaOperatorBase;
                        _dialogFinanceDocumentsResponse.GenericTreeView.DataSource.TopReturnedObjects = SettingsApp.PaginationRowsPerPage * _dialogFinanceDocumentsResponse.GenericTreeView.CurrentPageNumber;
                        _dialogFinanceDocumentsResponse.GenericTreeView.Refresh();
                        criteriaOperatorFilter = null;
                        _afterFilterTitle = null;

                        var countResult = GlobalFramework.SessionXpo.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperatorBase);
                        string nDocs = _dialogDocumentFinanceMaster.GenericTreeView.DataSource.Count.ToString();
                        string showResults = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_show_results"), nDocs, countResult);

                        _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);
                        _dialogDocumentFinanceMaster.WindowTitle = _selectRecordWindowTitle;
                    }
                    else if (DialogResponseType.Ok.Equals(response))
                    {
                        filterField = "DocumentType";
                        statusField = "DocumentStatusStatus";

                        /* IN009066 - FS and NC added to reports */
                        extraFilter = $@" AND ({statusField} <> 'A') AND (
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeInvoice}' OR 
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice}' OR 
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment}' OR 
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice}' OR 
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeDebitNote}' OR 
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeCreditNote}' OR 
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypePayment}' 
			                    OR 
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput}'
			                    )".Replace(Environment.NewLine, string.Empty);
                        /* IN009089 - # TO DO: above, we need to check with business this condition:  {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput}' */

                        // Assign Dialog FilterValue to Method Result Value
                        result.Add($"{dialog.FilterValue}{extraFilter}");
                        result.Add(dialog.FilterValueHumanReadble);
                        string addFilter = dialog.FilterValue;

                        criteriaOperatorFilter = GroupOperator.And(CriteriaOperatorBase, CriteriaOperator.Parse(addFilter));

                        _dialogFinanceDocumentsResponse.GenericTreeView.DataSource.Criteria = criteriaOperatorFilter;
                        _dialogFinanceDocumentsResponse.GenericTreeView.DataSource.TopReturnedObjects = SettingsApp.PaginationRowsPerPage * _dialogFinanceDocumentsResponse.GenericTreeView.CurrentPageNumber;
                        _dialogFinanceDocumentsResponse.GenericTreeView.Refresh();

                        var countResult = GlobalFramework.SessionXpo.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), criteriaOperatorFilter);
                        string nDocs = _dialogDocumentFinanceMaster.GenericTreeView.DataSource.Count.ToString();
                        string showResults = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_show_results"), nDocs, countResult);

                        _dialogDocumentFinanceMaster.WindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);
                        _afterFilterTitle = _dialogDocumentFinanceMaster.WindowTitle;
                        //criteriaOperatorFilter = null;

                        //_selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);
                        //_dialogDocumentFinanceMaster.WindowTitle = _selectRecordWindowTitle;

                    }
                    else
                    {
                        //CriteriaOperatorLastFilter = null;
                        // ESSE ELSE
                        criteriaOperatorFilter = null;
                        // Destroy Dialog on Cancel
                        dialog.Destroy();
                        // Assign Result
                        result = null;
                    }

                    dialog.Destroy();

                    _dialogFinanceDocumentsResponse.GenericTreeView.Refresh();
                }// IN009223 IN009227 - End
                 //_dialogFinanceDocumentsResponse.GenericTreeView.Refresh();
                if (!string.IsNullOrEmpty(_afterFilterTitle)) { _dialogDocumentFinanceMaster.WindowTitle = _afterFilterTitle; }

                _dialogFinanceDocumentsResponse.Run();
            }
            else
            {
                //Reset listMarkedFinanceMasterDocuments
                _listMarkedFinanceMasterDocuments.Clear();
                /* IN009223 and IN009227 */
                criteriaOperatorFilter = null;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: Action Print Finance Master Documents

        private bool TreeModelForEachTask_ActionPrintDocuments(TreeModel model, TreePath path, TreeIter iter)
        {
            int columnIndexCheckBox = 1;
            int columnIndexGuid = 2;
            try
            {
                bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));
                Guid itemGuid = new Guid(model.GetValue(iter, columnIndexGuid).ToString());

                if (itemChecked)
                {
                    fin_documentfinancemaster documentFinanceMaster = (fin_documentfinancemaster)FrameworkUtils.GetXPGuidObject(typeof(fin_documentfinancemaster), itemGuid);
                    //Required to use _dialogFinanceDocumentsResponse to Fix TransientFor, ALT+TAB
                    FrameworkCalls.PrintFinanceDocument(_dialogFinanceDocumentsResponse, _printerChoosed, documentFinanceMaster);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return false;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: Pay CC Documents

        /// <summary>
        /// Persist FinanceDocument From List of Documents. Usefull to Receive a List of Documents and Persist its Documents
        /// </summary>
        /// <param name="SourceWindow"></param>
        /// <param name="FinanceDocuments"></param>
        /// <returns></returns>

        private fin_documentfinancemaster PayCurrentAcountDocuments(
          PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> pSourceWindow,
          List<fin_documentfinancemaster> pFinanceDocuments
        )
        {
            //Local Vars
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> parentDialog = (PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster>)pSourceWindow;
            fin_documentfinancemaster resultDocument = null;

            try
            {
                //Init Global ArticleBag
                ArticleBag articleBag = new ArticleBag();
                ArticleBagKey articleBagKey;
                ArticleBagProperties articleBagProps;

                foreach (fin_documentfinancemaster document in pFinanceDocuments)
                {
                    foreach (fin_documentfinancedetail detail in document.DocumentDetail)
                    {
                        //Prepare articleBag Key and Props
                        articleBagKey = new ArticleBagKey(
                          detail.Article.Oid,
                          detail.Designation,
                          detail.Price,
                          detail.Discount,
                          detail.Vat
                        );
                        articleBagProps = new ArticleBagProperties(
                          detail.DocumentMaster.SourceOrderMain.PlaceTable.Place.Oid,
                          detail.DocumentMaster.SourceOrderMain.PlaceTable.Oid,
                          (PriceType)detail.DocumentMaster.SourceOrderMain.PlaceTable.Place.PriceType.EnumValue,
                          detail.Code,
                          detail.Quantity,
                          detail.UnitMeasure
                        );
                        //Send to Bag
                        articleBag.Add(articleBagKey, articleBagProps);
                    }
                }
                //IN009284 POS - Pagamento conta-corrente - Cliente por defeito 
                ProcessFinanceDocumentParameter ccCustomerOid = new ProcessFinanceDocumentParameter(pFinanceDocuments[0].EntityOid, articleBag);
                ccCustomerOid.Customer = pFinanceDocuments[0].EntityOid;
                /* Please see ERR201810#14 */
                // Call to overloaded constructor method, where "pSkipPersistFinanceDocument" parameter was settled to false when calling main constructor method.
                //
                // We are now calling main class constructor method, setting "pSkipPersistFinanceDocument" parameter to "true" instead, 
                // avoiding double invoice creation issue for "Conta Corrente" payments.
                PosPaymentsDialog dialog = new PosPaymentsDialog(pSourceWindow, DialogFlags.DestroyWithParent, articleBag, false, false, true, ccCustomerOid, null);

                int response = dialog.Run();
                if (response == (int)ResponseType.Ok)
                {
                    //Prepare ProcessFinanceDocumentParameter
                    ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(
                      SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice, dialog.ArticleBagFullPayment)
                    {
                        SourceMode = PersistFinanceDocumentSourceMode.CurrentAcountDocuments,
                        FinanceDocuments = pFinanceDocuments,
                        PaymentMethod = dialog.PaymentMethod.Oid,
                        Customer = dialog.Customer.Oid,
                        TotalDelivery = dialog.TotalDelivery,
                        TotalChange = dialog.TotalChange
                    };
                    resultDocument = FrameworkCalls.PersistFinanceDocument(dialog, processFinanceDocumentParameter);

                    //Always refresh TreeView, After Valid Payment
                    parentDialog.GenericTreeView.Refresh();
                    parentDialog.WindowTitle = _selectRecordWindowTitle;// resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_select_finance_document_cc;
                                                                        //Reset Totals
                    _totalDialogFinanceMasterDocuments = 0.0m;
                    //Disable Action Buttons Liquidar(0)/Print(0) after Refresh
                    parentDialog.ActionAreaButtons[0].Button.Sensitive = false;
                    parentDialog.ActionAreaButtons[1].Button.Sensitive = false;

                    dialog.Destroy();
                    return resultDocument;
                }
                else
                {
                    dialog.Destroy();
                    return null;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return resultDocument;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: Call PayInvoicesDialog

        private fin_documentfinancepayment CallPayInvoicesDialog(
          PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> pSourceWindow,
          decimal pPaymentAmount
        )
        {
            //Initialize local Vars
            fin_documentfinancepayment resultDocumentFinancePayment = null;
            int noOfInvoices = 0;

            //Count NoOfInvoices
            foreach (fin_documentfinancemaster document in _listSelectFinanceMasterDocuments)
            {
                if (document.DocumentType.Credit) noOfInvoices++;
            }

            PosPayInvoicesDialog dialogPayInvoices = new PosPayInvoicesDialog(pSourceWindow, DialogFlags.DestroyWithParent, pPaymentAmount, noOfInvoices);
            ResponseType response = (ResponseType)dialogPayInvoices.Run();
            if (response == ResponseType.Ok)
            {
                //Start Processing Documents
                resultDocumentFinancePayment = PayInvoices(
                    pSourceWindow,
                    _listSelectFinanceMasterDocuments,
                    _selectedDocumentEntityOid,
                    dialogPayInvoices.EntryBoxSelectConfigurationPaymentMethod.Value.Oid,
                    dialogPayInvoices.EntryBoxSelectConfigurationCurrency.Value.Oid,
                    dialogPayInvoices.PayedAmount,
                    dialogPayInvoices.EntryBoxDocumentPaymentNotes.EntryValidation.Text
                );
            }
            dialogPayInvoices.Destroy();

            return resultDocumentFinancePayment;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: Pay Invoices

        /// <summary>
        /// Split FinanceDocuments into Invoices and CreditNotes, Ready to send to PersistFinanceDocumentPayment
        /// </summary>
        /// <param name="SourceWindow"></param>
        /// <param name="FinanceDocuments"></param>
        /// <returns>Payment Document from PersistFinanceDocumentPayment Method</returns>
        private fin_documentfinancepayment PayInvoices(
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> pSourceWindow,
            List<fin_documentfinancemaster> pFinanceDocuments,
            Guid pCustomer,
            Guid pPaymentMethod,
            Guid pConfigurationCurrency,
            decimal pPaymentAmount,
            string pPaymentNotes = ""
        )
        {
            //Local Vars
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> parentDialog = (PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster>)pSourceWindow;
            fin_documentfinancepayment resultDocument = null;
            List<fin_documentfinancemaster> listInvoices = new List<fin_documentfinancemaster>();
            List<fin_documentfinancemaster> listCreditNotes = new List<fin_documentfinancemaster>();

            try
            {
                foreach (fin_documentfinancemaster document in pFinanceDocuments)
                {
                    if (document.DocumentType.Credit)
                    {
                        listInvoices.Add(document);
                    }
                    else
                    {
                        listCreditNotes.Add(document);
                    }
                }
                return FrameworkCalls.PersistFinanceDocumentPayment(pSourceWindow, listInvoices, listCreditNotes, pCustomer, pPaymentMethod, pConfigurationCurrency, pPaymentAmount, pPaymentNotes);
            }
            catch (ProcessFinanceDocumentValidationException ex)
            {
                _log.Error(ex.Message, ex);
                return resultDocument;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: Open Finance Documents PDFs

        /// <summary>
        /// Open Documents, or generate Documents and Open Documents
        /// </summary>
        /// <param name="FinanceDocuments"></param>
        /// <returns></returns>
        private void OpenFinanceMasterDocuments(
            List<fin_documentfinancemaster> pFinanceDocuments
        )
        {
            List<string> documents = new List<string>();

            try
            {
                // Call GenerateDocument and add it to List
                foreach (fin_documentfinancemaster document in pFinanceDocuments)
                {
                    if (document.SourceOrderMain != null)
                    {
                        Utils.ShowMessageTouch(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Ok, string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_warning_cant_open_title")), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_dialog_cant_open_document"), document.DocumentNumber));
                    }
                    else if (!LicenceManagement.IsLicensed || !LicenceManagement.CanPrint)
                    {
                        Utils.ShowMessageTouchErrorUnlicencedFunctionDisabled(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printing_function_disabled"));
                    }
                    else
                    {
                        documents.Add(ProcessFinanceDocument.GenerateDocumentFinanceMasterPDFIfNotExists(document));
                    }
                }

                foreach (var item in documents)
                {
                    if (!LicenceManagement.IsLicensed || !LicenceManagement.CanPrint)
                    {
                        Utils.ShowMessageTouchErrorUnlicencedFunctionDisabled(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printing_function_disabled"));
                    }
                    else if (File.Exists(item))
                    {
                        if(Utils.IsLinux)
                        {
                            System.Diagnostics.Process.Start(FrameworkUtils.OSSlash(string.Format(@"{0}\{1}", Environment.CurrentDirectory, item)));
                        }
                        else if (Utils.UsePosPDFViewer() == true && !Utils.IsLinux)
                        {
                            string docPath = FrameworkUtils.OSSlash(string.Format(@"{0}\{1}", Environment.CurrentDirectory, item));
                            var ScreenSizePDF = GlobalApp.ScreenSize;
                            int widthPDF = ScreenSizePDF.Width;
                            int heightPDF = ScreenSizePDF.Height;
                            System.Windows.Forms.Application.Run(new startDocumentViewer(docPath, widthPDF - 50, heightPDF - 25, false));
                        }
                
                    }
                }
            }
            catch (ProcessFinanceDocumentValidationException ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: Send Email Finance Documents PDFs

        /// <summary>
        /// Send Email Documents, or generate Documents and Send Email Documents
        /// </summary>
        /// <param name="Documents"></param>
        private ResponseType CallSendEmailFinanceMasterDocuments(
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> pSourceWindow,
            List<fin_documentfinancemaster> pDocuments
        )
        {
            Dictionary<fin_documentfinancemaster, string> documents = new Dictionary<fin_documentfinancemaster, string>();
            List<string> attachmentFileNames = new List<string>();
            // Get Customer from first Document
            erp_customer customer = (erp_customer)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(erp_customer), pDocuments[0].EntityOid);
            string customerEmail = (customer.Email != null) ? customer.Email : string.Empty;
            string subject = string.Empty;
            string mailBody = string.Empty;
            string documentList = string.Empty;

            try
            {
                // Call GenerateDocument and add it to List
                foreach (fin_documentfinancemaster document in pDocuments)
                {
                    documents.Add(document, ProcessFinanceDocument.GenerateDocumentFinanceMasterPDFIfNotExists(document));
                }

                foreach (var item in documents)
                {
                    if (File.Exists(item.Value))
                    {
                        // Compose documentList
                        documentList += string.Format("- {1}{0}", Environment.NewLine, item.Key.DocumentNumber);
                        // Add to attachmentFileNames
                        attachmentFileNames.Add(item.Value);
                    }
                }
                // Always remove last NewLine
                documentList = documentList.TrimEnd('\n').TrimEnd('\r');

                // Do the same for Payments
                // Dont forget ResponseType responseType
                subject = GlobalFramework.PreferenceParameters["SEND_MAIL_FINANCE_DOCUMENTS_SUBJECT"];
                string mailBodyTemplate = GlobalFramework.PreferenceParameters["SEND_MAIL_FINANCE_DOCUMENTS_BODY"];

                Dictionary<string, string> customTokensDictionary = new Dictionary<string, string>();
                customTokensDictionary.Add("DOCUMENT_LIST", documentList);
                // Prepare List of Dictionaries to send to replaceTextTokens
                List<Dictionary<string, string>> tokensDictionaryList = new List<Dictionary<string, string>>();
                tokensDictionaryList.Add(GlobalFramework.PreferenceParameters);
                tokensDictionaryList.Add(customTokensDictionary);
                mailBody = Utils.replaceTextTokens(mailBodyTemplate, tokensDictionaryList);

                PosSendEmailDialog dialog = new PosSendEmailDialog(
                    pSourceWindow,
                    DialogFlags.Modal,
                    new System.Drawing.Size(800, 640),
                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_send_email"),
                    subject,
                    customerEmail,
                    mailBody,
                    attachmentFileNames
                    );

                ResponseType responseType = (ResponseType)dialog.Run();
                dialog.Destroy();

                return responseType;
            }
            catch (ProcessFinanceDocumentValidationException ex)
            {
                _log.Error(ex.Message, ex);
                return ResponseType.None;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinancePayment: Send Email Finance Documents PDFs

        /// <summary>
        /// Send Email Documents, or generate Documents and Send Email Documents, 
        /// SAME has Above Version only changed type fin_documentfinancemaster with fin_documentfinancepayment
        /// </summary>
        /// <param name="Documents"></param>
        private void CallSendEmailFinancePaymentDocuments(
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinancePayment> pSourceWindow,
            List<fin_documentfinancepayment> pDocuments
        )
        {
            Dictionary<fin_documentfinancepayment, string> documents = new Dictionary<fin_documentfinancepayment, string>();
            List<string> attachmentFileNames = new List<string>();
            // Get Customer from first Document
            erp_customer customer = (erp_customer)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(erp_customer), pDocuments[0].EntityOid);
            string customerEmail = (customer.Email != null) ? customer.Email : string.Empty;
            string mailBody = string.Empty;

            try
            {
                // Call GenerateDocument and add it to List
                foreach (fin_documentfinancepayment document in pDocuments)
                {
                    documents.Add(document, ProcessFinanceDocument.GenerateDocumentFinancePaymentPDFIfNotExists(document));
                }

                foreach (var item in documents)
                {
                    if (File.Exists(item.Value))
                    {
                        mailBody += string.Format("- {1}{0}", Environment.NewLine, item.Key.PaymentRefNo);
                        attachmentFileNames.Add(item.Value);
                    }
                }

                PosSendEmailDialog dialog = new PosSendEmailDialog(
                    pSourceWindow,
                    DialogFlags.Modal,
                    new System.Drawing.Size(800, 640),
                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_send_email"),
                    "Subject",
                    customerEmail,
                    mailBody,
                    attachmentFileNames
                    );

                ResponseType responseType = (ResponseType)dialog.Run();
                dialog.Destroy();
            }
            catch (ProcessFinanceDocumentValidationException ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: Pay Invoices

        /// <summary>
        /// Clone Documents
        /// </summary>
        /// <param name="SourceWindow"></param>
        /// <param name="FinanceDocuments"></param>
        /// <returns></returns>
        private void CallCloneFinanceMasterDocuments(
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> pSourceWindow,
            List<fin_documentfinancemaster> pFinanceDocuments
        )
        {
            //Local Vars
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> parentDialog = (PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster>)pSourceWindow;
            List<fin_documentfinancemaster> resultDocument = new List<fin_documentfinancemaster>();

            SortingCollection articleDetailSortingCollection = new SortingCollection();
            articleDetailSortingCollection.Add(new SortProperty("Ord", DevExpress.Xpo.DB.SortingDirection.Ascending));

            try
            {
                foreach (fin_documentfinancemaster document in pFinanceDocuments)
                {
                    // Apply Sorting, this way we respect same Order
                    document.DocumentDetail.Sorting = articleDetailSortingCollection;
                    // Init ArticleBag with Discount
                    ArticleBag articleBag = new ArticleBag(document.Discount);

                    foreach (fin_documentfinancedetail articleDetail in document.DocumentDetail)
                    {
                        //Prepare articleBag Key and Props
                        ArticleBagKey articleBagKey = new ArticleBagKey(
                            articleDetail.Article.Oid,
                            articleDetail.Designation,
                            articleDetail.Price,
                            articleDetail.Discount,
                            articleDetail.Vat,
                            //If has a Valid ConfigurationVatExemptionReason use it Else send New Guid
                            (articleDetail.VatExemptionReason != null) ? articleDetail.VatExemptionReason.Oid : new Guid()
                        );
                        ArticleBagProperties articleBagProps = new ArticleBagProperties(
                            new Guid(),                 //pPlaceOid,
                            new Guid(),                 //pTableOid,
                            articleDetail.PriceType,    //pPriceType : PriceType.Price1
                            articleDetail.Code,
                            articleDetail.Quantity,
                            articleDetail.UnitMeasure
                        );

                        // Notes
                        if (!string.IsNullOrEmpty(articleDetail.Notes))
                        {
                            articleBagProps.Notes = articleDetail.Notes;
                        }
                        articleBag.Add(articleBagKey, articleBagProps);
                    }

                    // Init ProcessFinanceDocumentParameter
                    ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(document.DocumentType.Oid, articleBag)
                    {
                        Customer = document.EntityOid,
                        SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag
                    };

                    // PaymentCondition
                    if (document.PaymentCondition != null)
                    {
                        processFinanceDocumentParameter.PaymentCondition = document.PaymentCondition.Oid;
                    }
                    // PaymentMethod
                    if (document.PaymentMethod != null)
                    {
                        processFinanceDocumentParameter.PaymentMethod = document.PaymentMethod.Oid;
                    }
                    // Notes
                    if (document.Notes != null)
                    {
                        processFinanceDocumentParameter.Notes = document.Notes;
                    }

                    // PersistFinanceDocument
                    ProcessFinanceDocument.PersistFinanceDocument(processFinanceDocumentParameter);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: Cancel Documents

        /// <summary>
        /// Responsible for cancel documents based on some business rules
        /// </summary>
        /// <param name="pDialog"></param>
        /// <param name="pListSelectDocuments"></param>
        private void CallCancelFinanceMasterDocumentsDialog(Window pDialog, List<fin_documentfinancemaster> pListSelectDocuments)
        {

            _log.Debug("void PosDocumentFinanceSelectRecordDialog.CallCancelFinanceMasterDocumentsDialog(Window pDialog, List<fin_documentfinancemaster> pListSelectDocuments)");

            //SAF-T Notes
            //4.1: Documentos comerciais a clientes (SalesInvoices);
            //  4.1.4.2.1. * Estado atual do documento (InvoiceStatus) : “A” — Documento anulado
            //4.2: Documentos de movimentação de mercadorias (MovementOfGoods); 
            //  4.2.3.2.1. * Estado atual do documento (Movement-Status) : “A” — Documento anulado
            //4.3: Documentos de conferência de entrega de mercadorias ou da prestação de serviços (WorkingDocuments).
            //  4.3.4.2.1. * Estado atual do documento (WorkStatus) : “A” — Documento anulado

            try
            {
                logicpos.Utils.ResponseText dialogResponse;
                DateTime currentDateTime;
                List<string> ignoredDocuments = new List<string>();

                foreach (var documentMaster in pListSelectDocuments)
                {
                    //Check if Can Cancell Document
                    if (CanCancelFinanceMasterDocument(documentMaster))
                    {
                        string fileWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_input_text_default.png");

                        //Call Request Motive Dialog
                        dialogResponse = Utils.GetInputText(pDialog, DialogFlags.Modal, fileWindowIcon, string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_cancel_document_input_text_label"), documentMaster.DocumentNumber), string.Empty, SettingsApp.RegexAlfaNumericExtended, true);

                        if (dialogResponse.ResponseType == ResponseType.Ok)
                        {
                            //_log.Debug(string.Format("DocumentNumber:[{0}], DocumentStatusStatus:[{1}], reason:[{2}]", document.DocumentNumber, document.DocumentStatusStatus, dialogResponse.InputText));
                            currentDateTime = FrameworkUtils.CurrentDateTimeAtomic();
                            documentMaster.DocumentStatusStatus = "A";
                            documentMaster.DocumentStatusDate = currentDateTime.ToString(SettingsApp.DateTimeFormatCombinedDateTime);
                            documentMaster.DocumentStatusReason = dialogResponse.Text;
                            documentMaster.DocumentStatusUser = GlobalFramework.LoggedUser.CodeInternal;
                            /* IN009083 - uncommented */
                            //ATWS: Check if Sent Resend Document to AT WebServices                                                 //WIP: CancellWayBills : 
                            bool sendDocumentToAT = false;                                                                          //WIP: CancellWayBills : 
                            if (                                                                                                    //WIP: CancellWayBills : 
                                SettingsApp.ServiceATSendDocumentsWayBill                                                           //WIP: CancellWayBills : 
                                && SettingsApp.ConfigurationSystemCountry.Oid == SettingsApp.XpoOidConfigurationCountryPortugal     //WIP: CancellWayBills : 
                                && documentMaster.DocumentType.WsAtDocument                                                         //WIP: CancellWayBills : 
                                && documentMaster.DocumentType.WayBill                                                              //WIP: CancellWayBills : 
                                && documentMaster.DocumentType.Oid != SettingsApp.XpoOidDocumentFinanceTypeInvoiceWayBill           //WIP: CancellWayBills : 
                                )                                                                                                   //WIP: CancellWayBills : 
                            {                                                                                                       //WIP: CancellWayBills : 
                                sendDocumentToAT = true;                                                                            //WIP: CancellWayBills : 
                            }                                                                                                       //WIP: CancellWayBills : 

                            /* IN009083 - case document is not "DocumentType.WayBill", we will send it to AT if able to */
                            if (
                                SettingsApp.ServiceATSendDocuments
                                && SettingsApp.XpoOidConfigurationCountryPortugal.Equals(SettingsApp.ConfigurationSystemCountry.Oid)
                                && documentMaster.DocumentType.WsAtDocument
                                && !documentMaster.DocumentType.WayBill
                                )
                            {
                                sendDocumentToAT = true;
                            }/* IN009083 - end */

                            /* 
                                See "logicpos.financial.service.Objects.Utils"
                                Method "string GetDocumentsQuery(bool pWayBillMode, Guid pDocumentMaster)":

                                    (fin_documentfinancetype.WsAtDocument = 1):
                                        Fatura (Guia)
                                        Fatura Simplificada
                                        Fatura
                                        Fatura-Recibo
                                        Nota de Crédito
                                        Nota de Débito
                                AND (
                                        fin_documentfinancemaster.ATValidAuditResult IS NULL 
                                        OR 
                                        fin_documentfinancemaster.ATResendDocument = 1
                                    ) 
                            */
                            //if ( SettingsApp.ConfigurationSystemCountry.Oid == SettingsApp.XpoOidConfigurationCountryPortugal)
                            {
                                /* IN009032 and then removed by IN009083 */
                                //documentMaster.ATResendDocument = true;
                            }

                            /* IN009083 */
                            bool isCanceledByUser = false;
                            /* IN009083 - uncommented */
                            //ATWS: Call ResendDocument to At WebService
                            if (sendDocumentToAT)                                                                                   //WIP: CancellWayBills : 
                            {                                                                                                       //WIP: CancellWayBills :
                                                                                                                                    /* IN009083 - we expect user to cancel the communication with AT in case of any error, then for WayBill, task scheduler will be responsible for it.
                                                                                                                                     * #TODO: "ShipFromDeliveryDate" when AT is receiving the cancellation request */
                                isCanceledByUser = FrameworkCalls.SendDocumentToATWSDialog(pDialog, documentMaster);                //WIP: CancellWayBills : 
                            }                                                                                                       //WIP: CancellWayBills : 

                            /* IN009083 - when AT WS call fails and user wish to do not retry the document sending to AT,
                             * we inform user that real time process failed and depends on "timer service" (Utils.ServiceSendPendentDocuments()) */
                            if (isCanceledByUser)
                            {
                                /* "timer service" process will run and, for WayBill, "ATResendDocument" must be settled */
                                if (documentMaster.DocumentType.WayBill)
                                {
                                    //Assign ResendDocument to Document                                                             //WIP: CancellWayBills : 
                                    documentMaster.ATResendDocument = true;                                                         //WIP: CancellWayBills : 
                                }

                                _log.Debug("void PosDocumentFinanceSelectRecordDialog.CallCancelFinanceMasterDocumentsDialog(Window pDialog, List<fin_documentfinancemaster> pListSelectDocuments) :: AT Document Cancellation Process: real time process cancelled by user [" + documentMaster.DocumentNumber + "]");
                                ignoredDocuments.Add(string.Format("{0} [{1}]", documentMaster.DocumentNumber, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_error_in_at_ws_call_status")));
                            }

                            /* Restore stock items */
                            if (!ProcessArticleStockMode.None.Equals(documentMaster.DocumentType.StockMode))
                            {
                                //Process Stock : Restore Stock after Cancelling Documents
                                _log.Debug("void PosDocumentFinanceSelectRecordDialog.CallCancelFinanceMasterDocumentsDialog(Window pDialog, List<fin_documentfinancemaster> pListSelectDocuments) :: Process Stock : restoring stock after cancelling [" + documentMaster.DocumentNumber + "] document");
                                ProcessArticleStock.Add(documentMaster, true);
                            }

                            documentMaster.Save();

                            //Audit
                            FrameworkUtils.Audit("FINANCE_DOCUMENT_CANCELLED", string.Format("{0} {1}: {2}", documentMaster.DocumentType.Designation, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_document_cancelled"), documentMaster.DocumentNumber));
                        }
                        else
                        {
                            //Add to Ignored Documents if User Cancel
                            ignoredDocuments.Add(string.Format("{0} [{1}]", documentMaster.DocumentNumber, documentMaster.DocumentStatusStatus));
                        }
                    }
                    else
                    {
                        //Add to Ignored Documents if CanCancelFinanceMasterDocument result false
                        ignoredDocuments.Add(string.Format("{0} [{1}]", documentMaster.DocumentNumber, documentMaster.DocumentStatusStatus));
                    }
                }

                //Show Ignored Documents
                if (ignoredDocuments.Count > 0) ShowIgnoredDocuments(pDialog, ignoredDocuments);
            }
            catch (Exception ex)
            {
                _log.Error("void PosDocumentFinanceSelectRecordDialog.CallCancelFinanceMasterDocumentsDialog(Window pDialog, List<fin_documentfinancemaster> pListSelectDocuments) :: " + ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: Open Documents

        /// <summary>
        /// Split FinanceDocuments into Invoices and CreditNotes, Ready to send to PersistFinanceDocumentPayment
        /// </summary>
        /// <param name="SourceWindow"></param>
        /// <param name="FinanceDocuments"></param>
        /// <returns>Payment Document from PersistFinanceDocumentPayment Method</returns>
        private fin_documentfinancepayment OpenDocument(
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> pSourceWindow,
            List<fin_documentfinancemaster> pFinanceDocuments,
            Guid pCustomer,
            Guid pPaymentMethod,
            Guid pConfigurationCurrency,
            decimal pPaymentAmount,
            string pPaymentNotes = ""
        )
        {
            //Local Vars
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> parentDialog = (PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster>)pSourceWindow;
            fin_documentfinancepayment resultDocument = null;
            List<fin_documentfinancemaster> listInvoices = new List<fin_documentfinancemaster>();
            List<fin_documentfinancemaster> listCreditNotes = new List<fin_documentfinancemaster>();

            try
            {
                foreach (fin_documentfinancemaster document in pFinanceDocuments)
                {
                    if (document.DocumentType.Credit)
                    {
                        listInvoices.Add(document);
                    }
                    else
                    {
                        listCreditNotes.Add(document);
                    }
                }
                return FrameworkCalls.PersistFinanceDocumentPayment(pSourceWindow, listInvoices, listCreditNotes, pCustomer, pPaymentMethod, pConfigurationCurrency, pPaymentAmount, pPaymentNotes);
            }
            catch (ProcessFinanceDocumentValidationException ex)
            {
                _log.Error(ex.Message, ex);
                return resultDocument;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: Get Finance Master Document List (Checked Items)

        /// <summary>
        /// Checks if a cancelation request is valid, based on "DocumentStatusStatus", "Date" among other rules.
        /// </summary>
        /// <param name="pDocumentFinanceMaster"></param>
        /// <returns>It returns "bool isCancellable"</returns>
        private bool CanCancelFinanceMasterDocument(fin_documentfinancemaster pDocumentFinanceMaster)
        {
            _log.Debug("bool PosDocumentFinanceSelectRecordDialog.CanCancelFinanceMasterDocument(fin_documentfinancemaster pDocumentFinanceMaster)");

            bool isCancellable = false;
            DateTime currentDateDay = FrameworkUtils.CurrentDateTimeAtomicMidnight();
            DateTime documentDateDay = FrameworkUtils.DateTimeToMidnightDate(pDocumentFinanceMaster.Date);

            /* IN009083 - document is not cancelled nor invoiced and Date of document is Today */
            if (pDocumentFinanceMaster.DocumentStatusStatus != "A" && pDocumentFinanceMaster.DocumentStatusStatus != "F" && currentDateDay == documentDateDay)
            {
                isCancellable = true;

                if (!pDocumentFinanceMaster.DocumentType.WayBill && IsAlreadyReceivedByAT(pDocumentFinanceMaster.Oid))
                {/* IN009083 - if fiscal document was already received by AT */
                    _log.Debug("CanCancelFinanceMasterDocument() :: document was already received successfully by AT: " + pDocumentFinanceMaster.DocumentNumber);
                    return false;
                }
                else if (pDocumentFinanceMaster.DocumentType.WayBill && pDocumentFinanceMaster.ShipFromDeliveryDate <= DateTime.Now)
                {/* IN009083 - if invoice products are already in transport */
                    _log.Debug("CanCancelFinanceMasterDocument() :: products are already in transport: " + pDocumentFinanceMaster.DocumentNumber);
                    return false;
                }

                /* IN009083 */
                {//Check if Document have dependent non Cancelled Child FinanceDocuments
                    string sqlFinanceMaster = string.Format("SELECT DocumentNumber FROM fin_documentfinancemaster WHERE DocumentParent = '{0}' AND DocumentStatusStatus <> 'A' ORDER BY CreatedAt;", pDocumentFinanceMaster.Oid);
                    XPSelectData xPSelectDataFinanceMaster = FrameworkUtils.GetSelectedDataFromQuery(sqlFinanceMaster);

                    /* IN009083 - if found one simple dependent, returns */
                    if (xPSelectDataFinanceMaster.Data.Length > 0)
                    {
                        _log.Debug("CanCancelFinanceMasterDocument() :: linked documents found: " + xPSelectDataFinanceMaster.Data.Length);
                        return false;
                    }
                }
                /* IN009083 */
                {//Check if Document have dependent non Cancelled Payments
                    string sqlFinancePayment = string.Format("SELECT fmaDocumentNumber AS DocumentNumber FROM view_documentfinancepayment WHERE fmaOid = '{0}' AND fpaPaymentStatus <> 'A' ORDER BY fmaCreatedAt;", pDocumentFinanceMaster.Oid);
                    XPSelectData xPSelectDataFinancePayment = FrameworkUtils.GetSelectedDataFromQuery(sqlFinancePayment);

                    /* IN009083 - has payments */
                    if (xPSelectDataFinancePayment.Data.Length > 0)
                    {
                        _log.Debug("CanCancelFinanceMasterDocument() :: document has payments: " + xPSelectDataFinancePayment.Data.Length);
                        return false;
                    }
                }
            }
            return isCancellable;
        }

        /// <summary>
        /// When a document has a register in sys_systemauditat with a ReturnCode equals 0, then this document has been received by AT successful.
        /// If any other status, returns false.
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        private bool IsAlreadyReceivedByAT(Guid oid)
        {
            bool result = false;

            try
            {
                /* IN009083 - we are considering here that we do not cancel Sales Documents if already received by AT */
                string sql = string.Format("SELECT COUNT(*) FROM sys_systemauditat WHERE DocumentMaster = '{0}' AND ReturnCode = '0';", oid);
                var sqlResult = GlobalFramework.SessionXpo.ExecuteScalar(sql);
                int countResult = Convert.ToUInt16(sqlResult);
                /* if document already received successfully by AT */
                if (countResult > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _log.Error("bool PosDocumentFinanceSelectRecordDialog.IsAlreadyReceivedByAT(Guid oid) :: " + ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: Get Finance Master Document List (Checked Items)

        private bool TreeModelForEachTask_ActionGetFinanceDocumentsList(TreeModel model, TreePath path, TreeIter iter)
        {
            int columnIndexCheckBox = 1;
            int columnIndexGuid = 2;
            try
            {
                bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));
                Guid itemGuid = new Guid(model.GetValue(iter, columnIndexGuid).ToString());

                if (itemChecked)
                {
                    //Add to FinanceMasterDocuments
                    _listSelectFinanceMasterDocuments.Add((fin_documentfinancemaster)FrameworkUtils.GetXPGuidObject(typeof(fin_documentfinancemaster), itemGuid));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return false;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: WorkSession Periods Event

        private void _touchButtonPosToolbarWorkSessionPeriods_Clicked(object sender, EventArgs e)
        {
            //Default ActionArea Buttons
            TouchButtonIconWithText buttonPrintDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.Print, "touchButtonPrintDocument_Green");
            TouchButtonIconWithText buttonClose = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.Close);
            buttonPrintDocument.Sensitive = false;

            //ActionArea Buttons
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            //Add references to Send to Event CursorChanged
            ActionAreaButton actionAreaButtonPrint = new ActionAreaButton(buttonPrintDocument, _responseTypePrint);
            ActionAreaButton actionAreaButtonClose = new ActionAreaButton(buttonClose, ResponseType.Close);
            actionAreaButtons.Add(actionAreaButtonPrint);
            actionAreaButtons.Add(actionAreaButtonClose);

            //Define Criteria
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse("PeriodType = 0 AND SessionStatus = 1");

            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewWorkSessionPeriod>
              dialogWorkSessionPeriods = new PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewWorkSessionPeriod>(
                this,
                DialogFlags.DestroyWithParent,
                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_select_worksession_period_day"),
                GlobalApp.MaxWindowSize,
                null, //XpoDefaultValue
                criteriaOperator,
                GenericTreeViewMode.CheckBox,
                actionAreaButtons
              );

            //CheckBox Capture CursorChanged/CheckBoxToggled Event, And enable/disable Buttons based on Valid Selection, Must be Here, Where we have a refence to Buttons
            dialogWorkSessionPeriods.CheckBoxToggled += delegate
            {
        //Use inside delegate to have accesss to local references, ex dialogPartialPayment, actionAreaButtonOk
        if (dialogWorkSessionPeriods.GenericTreeViewMode == GenericTreeViewMode.Default)
                {
            //DataTableMode else use XPGuidObject
            if (dialogWorkSessionPeriods.GenericTreeView.DataSourceRow != null) actionAreaButtonPrint.Button.Sensitive = true;
                }
                else if (dialogWorkSessionPeriods.GenericTreeViewMode == GenericTreeViewMode.CheckBox)
                {
                    actionAreaButtonPrint.Button.Sensitive = (dialogWorkSessionPeriods.GenericTreeView.MarkedCheckBoxs > 0) ? true : false;
                }
            };

            //Events
            dialogWorkSessionPeriods.Response += dialogWorkSessionPeriods_Response;

            //Call Dialog
            int response = dialogWorkSessionPeriods.Run();
            dialogWorkSessionPeriods.Destroy();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: WorkSession Periods Response

        private void dialogWorkSessionPeriods_Response(object o, ResponseArgs args)
        {
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewWorkSessionPeriod>
              dialog = (PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewWorkSessionPeriod>)o;

            if (args.ResponseId != ResponseType.Close)
            {
                if (args.ResponseId == _responseTypePrint)
                {
                    //Single Record Mode - Default - USED HERE ONLY TO TEST Both Dialogs Modes (Default and CheckBox)
                    if (dialog.GenericTreeViewMode == GenericTreeViewMode.Default)
                    {
                        //use dialog.GenericTreeView.DataTableRow.ItemArray
                    }
                    //Multi Record Mode - CheckBox - ACTIVE MODE
                    else if (dialog.GenericTreeViewMode == GenericTreeViewMode.CheckBox)
                    {
                        //Required to use ListStoreModel and not ListStoreModelFilterSort, we only loop the visible filtered rows, and not The hidden Checked Rows
                        dialog.GenericTreeView.ListStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask_ActionPrintPeriod));
                    }
                }
                dialog.Run();
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: WorkSession Periods TreeModelForEachTask

        private bool TreeModelForEachTask_ActionPrintPeriod(TreeModel model, TreePath path, TreeIter iter)
        {
            int columnIndexCheckBox = 1;
            int columnIndexGuid = 2;
            try
            {
                bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));
                Guid itemGuid = new Guid(model.GetValue(iter, columnIndexGuid).ToString());

                if (itemChecked)
                {
                    pos_worksessionperiod workSessionPeriodParent = (pos_worksessionperiod)FrameworkUtils.GetXPGuidObject(typeof(pos_worksessionperiod), itemGuid);
                    pos_worksessionperiod workSessionPeriodChild;
                    //Print Parent Session : PrintWorkSessionMovement
                    FrameworkCalls.PrintWorkSessionMovement(this, GlobalFramework.LoggedTerminal.ThermalPrinter, workSessionPeriodParent);

                    //Get Child Sessions
                    string sql = string.Format(@"SELECT Oid FROM pos_worksessionperiod WHERE Parent = '{0}' ORDER BY DateStart;", workSessionPeriodParent.Oid);
                    XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sql);
                    foreach (DevExpress.Xpo.DB.SelectStatementResultRow row in xPSelectData.Data)
                    {
                        //Print Child Sessions
                        workSessionPeriodChild = (pos_worksessionperiod)FrameworkUtils.GetXPGuidObject(typeof(pos_worksessionperiod), new Guid(row.Values[xPSelectData.GetFieldIndex("Oid")].ToString()));
                        //PrintWorkSessionMovement
                        FrameworkCalls.PrintWorkSessionMovement(this, GlobalFramework.LoggedTerminal.ThermalPrinter, workSessionPeriodChild);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return false;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceMaster: Uncheck FinanceMasterDocuments

        private void UnCheckAll_FinanceMasterDocuments(PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> pDialog, bool pRefreshTree)
        {
            //UnCheck all Marked CheckBoxs
            pDialog.GenericTreeView.UnCheckAll();
            //Refresh Tree
            if (pRefreshTree) pDialog.GenericTreeView.Refresh();
            //Restore Title, without Totals
            pDialog.WindowTitle = _selectRecordWindowTitle;
            //Reset Totals
            _totalDialogFinanceMasterDocuments = 0.0m;

            //Dont Disable First and Last button (New and Close)
            for (int i = 2; i < pDialog.ActionAreaButtons.Count - 1; i++)
            {
                //_log.Debug(string.Format("index[{0}], Label[{1}]", i, pDialog.ActionAreaButtons[i].Button.Name));
                pDialog.ActionAreaButtons[i].Button.Sensitive = false;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Payments : Clicked Event

        private void _toolbarFinanceDocumentsPayments_Clicked(object sender, EventArgs e)
        {
            bool validMarkedDocumentTypesForSendEmailSensitive = false;
            bool itemChecked = false;
            fin_documentfinancepayment documentFinancePayment;
            // IN009223 IN009227
            //string fileActionMore = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_more.png");
            //string fileActionFilter = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_filter.png");

            //Default ActionArea Buttons
            // IN009223 IN009227
            //TouchButtonIconWithText buttonMore = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.More, "touchButtonMore_Grey", string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_more, SettingsApp.PaginationRowsPerPage), fileActionMore);
            //TouchButtonIconWithText buttonFilter = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Filter, "touchButtonFilter_Green", "Filter", fileActionFilter);
            TouchButtonIconWithText buttonClose = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.Close);
            TouchButtonIconWithText buttonPrintDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.Print, "touchButtonPrintDocument_Green");
            TouchButtonIconWithText buttonPrintDocumentAs = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.PrintAs, "touchButtonPrintDocumentAs_Green");
            TouchButtonIconWithText buttonCancelDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButtonCancelDocument_Green", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_cancel_document"), _fileActionCancel);
            TouchButtonIconWithText buttonOpenDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.OpenDocument, "touchButtonOpenDocument_Green");
            TouchButtonIconWithText buttonSendEmailDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.SendEmailDocument, "touchButtonSendEmailDocument_Green");
            buttonPrintDocument.Sensitive = false;
            buttonPrintDocumentAs.Sensitive = false;
            buttonCancelDocument.Sensitive = false;
            buttonOpenDocument.Sensitive = false;
            buttonSendEmailDocument.Sensitive = false;
            //ActionArea Buttons
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            //Add references to Send to Event CursorChanged
            // IN009223 IN009227
            //_actionAreaButtonMore = new ActionAreaButton(buttonMore, _responseTypeLoadMoreDocuments);
            //_actionAreaButtonFilter = new ActionAreaButton(buttonFilter, _responseTypeFilter);
            ActionAreaButton actionAreaButtonClose = new ActionAreaButton(buttonClose, ResponseType.Close);
            _actionAreaButtonPrintPayment = new ActionAreaButton(buttonPrintDocument, _responseTypePrint);
            _actionAreaButtonPrintPaymentAs = new ActionAreaButton(buttonPrintDocumentAs, _responseTypePrintAs);
            _actionAreaButtonCancelPayment = new ActionAreaButton(buttonCancelDocument, _responseTypeCancelDocument);
            _actionAreaButtonOpenDocument = new ActionAreaButton(buttonOpenDocument, _responseTypeOpenDocument);
            _actionAreaButtonSendEmailDocument = new ActionAreaButton(buttonSendEmailDocument, _responseTypeSendEmailDocument);
            // IN009223 IN009227            
            //actionAreaButtons.Add(_actionAreaButtonMore);
            //actionAreaButtons.Add(_actionAreaButtonFilter);
            actionAreaButtons.Add(_actionAreaButtonCancelPayment);
            actionAreaButtons.Add(_actionAreaButtonPrintPayment);
            actionAreaButtons.Add(_actionAreaButtonPrintPaymentAs);
            actionAreaButtons.Add(_actionAreaButtonOpenDocument);
            actionAreaButtons.Add(_actionAreaButtonSendEmailDocument);
            actionAreaButtons.Add(actionAreaButtonClose);

            // IN009223 IN009227
            _filterChoice = Enums.Reports.ReportsQueryDialogMode.FILTER_PAYMENT_DOCUMENTS;

            //Define Criteria
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            CriteriaOperatorBase = criteriaOperator;// IN009223 IN009227

            var countResult = GlobalFramework.SessionXpo.Evaluate(typeof(fin_documentfinancepayment), CriteriaOperator.Parse("Count()"), CriteriaOperatorBase);
            string sqlCountTopResults = "0";
            string sqlCountResultTopResults = "0";

            if (GlobalFramework.DatabaseType.ToString() == "MySql" || GlobalFramework.DatabaseType.ToString() == "SQLite")
            {
                string filterCriteriaOperatorBase = CriteriaOperatorBase.ToString().Replace("[", "").Replace("]", "");
                sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT * FROM fin_documentfinancepayment WHERE {1}) AS Total LIMIT {0};", SettingsApp.PaginationRowsPerPage, filterCriteriaOperatorBase);
                sqlCountResultTopResults = GlobalFramework.SessionXpo.ExecuteScalar(sqlCountTopResults).ToString();
            }
            else if (GlobalFramework.DatabaseType.ToString() == "MSSqlServer")
            {
                sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT TOP {0} * FROM fin_documentfinancepayment WHERE {1}) AS Total;", SettingsApp.PaginationRowsPerPage, CriteriaOperatorBase);
                sqlCountResultTopResults = GlobalFramework.SessionXpo.ExecuteScalar(sqlCountTopResults).ToString();
            }


            windowTitleDefault = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_document_finance_payment");
            //NOT IN USE string nDocs = _dialogDocumentFinanceMaster.GenericTreeView.DataSource.Count.ToString();
            string showResults = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_show_results"), sqlCountResultTopResults, countResult);
            _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);

            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinancePayment>
              dialogPayments = new PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinancePayment>(
                this,
                DialogFlags.DestroyWithParent,
                _selectRecordWindowTitle,
                GlobalApp.MaxWindowSize,
                null, //XpoDefaultValue
                criteriaOperator,
                GenericTreeViewMode.CheckBox,
                actionAreaButtons
              );


            //CheckBox Capture CursorChanged/CheckBoxToggled Event, And enable/disable Buttons based on Valid Selection, Must be Here, Where we have a refence to Buttons
            dialogPayments.CheckBoxToggled += delegate
            {
        //Use inside delegate to have accesss to local references, ex dialogPartialPayment, actionAreaButtonOk
        if (dialogPayments.GenericTreeViewMode == GenericTreeViewMode.Default)
                {
            //DataTableMode else use XPGuidObject
            if (dialogPayments.GenericTreeView.DataSourceRow != null)
                    {
                        _actionAreaButtonPrintPayment.Button.Sensitive = true;
                        _actionAreaButtonPrintPaymentAs.Button.Sensitive = true;
                        _actionAreaButtonCancelPayment.Button.Sensitive = true;
                        _actionAreaButtonOpenDocument.Button.Sensitive = true;
                        _actionAreaButtonFilter.Button.Sensitive = true;
                    }
                }
                else if (dialogPayments.GenericTreeViewMode == GenericTreeViewMode.CheckBox)
                {

            //Get value from Model, its Outside XPGuidObject Scope
            itemChecked = (bool)dialogPayments.GenericTreeView.GetCurrentModelCheckBoxValue();
                    documentFinancePayment = (fin_documentfinancepayment)dialogPayments.GenericTreeView.DataSourceRow;
            // Add/Remove MarkedFinanceMasterDocuments on click/mark Document
            if (itemChecked)
                    {
                        _listMarkedFinancePaymentDocuments.Add(documentFinancePayment);
                //_log.Debug(string.Format("_listMarkedFinancePaymentDocuments count: [{0}], Added: [{1}]", _listMarkedFinancePaymentDocuments.Count, documentFinancePayment.PaymentRefNo));
            }
                    else
                    {
                        _listMarkedFinancePaymentDocuments.Remove(documentFinancePayment);
                //_log.Debug(string.Format("_listMarkedFinancePaymentDocuments count: [{0}], Removed: [{1}]", _listMarkedFinancePaymentDocuments.Count, documentFinancePayment.PaymentRefNo));
            }
            // Get Sensitive for SendEmail
            validMarkedDocumentTypesForSendEmailSensitive = GetSensitiveForSendEmailDocuments(_listMarkedFinancePaymentDocuments);

                    if (dialogPayments.GenericTreeView.MarkedCheckBoxs > 0)
                    {
                        _actionAreaButtonPrintPayment.Button.Sensitive = true;
                        _actionAreaButtonPrintPaymentAs.Button.Sensitive = true;
                        _actionAreaButtonCancelPayment.Button.Sensitive = (permissionFinanceDocumentCancelDocument) ? true : false;
                        _actionAreaButtonOpenDocument.Button.Sensitive = (permissionFinanceDocumentCancelDocument) ? true : false;
                        _actionAreaButtonSendEmailDocument.Button.Sensitive = (validMarkedDocumentTypesForSendEmailSensitive) ? true : false;
                    }
                    else
                    {
                        _actionAreaButtonPrintPayment.Button.Sensitive = false;
                        _actionAreaButtonPrintPaymentAs.Button.Sensitive = false;
                        _actionAreaButtonCancelPayment.Button.Sensitive = false;
                        _actionAreaButtonOpenDocument.Button.Sensitive = false;
                        _actionAreaButtonSendEmailDocument.Button.Sensitive = false;
                // Require to Empty listMarkedFinancePaymentDocuments (MarkedCheckBoxs > 0)
                _listMarkedFinancePaymentDocuments.Clear();
                    }
                }
            };

            //Events
            dialogPayments.Response += dialogFinancePaymentDocuments_Response;

            //Call Dialog
            int response = dialogPayments.Run();
            dialogPayments.Destroy();
        }

        private void dialogFinancePaymentDocuments_Response(object o, ResponseArgs args)
        {
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinancePayment>
              dialog = (PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinancePayment>)o;

            fin_documentfinancepayment documentFinancePayment = (fin_documentfinancepayment)dialog.GenericTreeView.DataSourceRow;

            if (args.ResponseId != ResponseType.Close)
            {
                if (
                        (args.ResponseId == _responseTypePrint && !Utils.ShowMessageTouchRequiredValidPrinter(_dialogFinanceDocumentsResponse))
                        || args.ResponseId == _responseTypePrintAs
                    )
                {
                    //Assign Choosed Printer based on user ResponseType
                    _printerChoosed = (args.ResponseId == _responseTypePrint) ? GlobalFramework.LoggedTerminal.Printer : _printerGeneric;

                    //Single Record Mode - Default - USED HERE ONLY TO TEST Both Dialogs Modes (Default and CheckBox)
                    if (dialog.GenericTreeViewMode == GenericTreeViewMode.Default)
                    {
                        FrameworkCalls.PrintFinanceDocumentPayment(this, _printerChoosed, documentFinancePayment);
                    }
                    //Multi Record Mode - CheckBox - ACTIVE MODE
                    else if (dialog.GenericTreeViewMode == GenericTreeViewMode.CheckBox)
                    {
                        //Required to use ListStoreModel and not ListStoreModelFilterSort, we only loop the visible filtered rows, and not The hidden Checked Rows
                        dialog.GenericTreeView.ListStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask_ActionPrintPayments));
                        //UnCheck all Marked CheckBoxs
                        UnCheckAll_FinancePaymentDocuments(dialog, false);
                    }
                }
                //Shared for all Modes that required a _listSelectFinancePaymentDocuments
                else if (// IN009223 IN009227
                    args.ResponseId == _responseTypeCancelDocument ||
                    args.ResponseId == _responseTypeOpenDocument ||
                    args.ResponseId == (ResponseType)DialogResponseType.LoadMore ||
                    args.ResponseId == (ResponseType)DialogResponseType.Filter ||
                    args.ResponseId == _responseTypeSendEmailDocument
                )
                {
                    _listSelectFinancePaymentDocuments = new List<fin_documentfinancepayment>();

                    //Single Record Mode - Default - USED HERE ONLY TO TEST Both Dialogs Modes (Default and CheckBox)
                    if (dialog.GenericTreeViewMode == GenericTreeViewMode.Default)
                    {
                        _listSelectFinancePaymentDocuments.Add(documentFinancePayment);
                    }
                    //Multi Record Mode - CheckBox - ACTIVE MODE
                    else if (dialog.GenericTreeViewMode == GenericTreeViewMode.CheckBox)
                    {
                        //Fill _listSelectFinancePaymentDocuments in ForEachFunc
                        //Required to use ListStoreModel and not ListStoreModelFilterSort, we only loop the visible filtered rows, and not The hidden Checked Rows
                        dialog.GenericTreeView.ListStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask_ActionGetPaymentDocumentsList));
                    }

                    //Used to Check if Methods return a Valid Return Document, ex when Dialog OK Pressed
                    object resultDocument = null;

                    //Send to Method based on Response
                    //Cancel Documents
                    if (args.ResponseId == _responseTypeCancelDocument)
                    {
                        //Start Processing Documents
                        CallCancelFinancePaymentDocumentsDialog(dialog, _listSelectFinancePaymentDocuments);
                        //UnCheck all Marked CheckBoxs, After call CallCancelDocumentsDialog
                        UnCheckAll_FinancePaymentDocuments(dialog, true);
                    }
                    else if (args.ResponseId == _responseTypeOpenDocument)
                    {
                        //Start Processing Documents
                        OpenFinancePaymentDocuments(dialog, _listSelectFinancePaymentDocuments);
                        //UnCheck all Marked CheckBoxs, After call CallCancelDocumentsDialog
                        UnCheckAll_FinancePaymentDocuments(dialog, true);
                    }
                    //SendEmail Documents
                    else if (args.ResponseId == _responseTypeSendEmailDocument)
                    {
                        //Start Send Email Documents
                        CallSendEmailFinancePaymentDocuments(dialog, _listSelectFinancePaymentDocuments);
                        //UnCheck all Marked CheckBoxs, After call CallCancelDocumentsDialog
                        UnCheckAll_FinancePaymentDocuments(dialog, true);

                    }// IN009223 IN009227 - Begin
                    else if (args.ResponseId == (ResponseType)DialogResponseType.LoadMore)
                    {
                        dialog.GenericTreeView.DataSource.TopReturnedObjects = (SettingsApp.PaginationRowsPerPage * dialog.GenericTreeView.CurrentPageNumber);
                        dialog.GenericTreeView.Refresh();

                        CriteriaOperator criteriaCount = null;

                        if (ReferenceEquals(criteriaOperatorFilter, null))
                        {
                            criteriaCount = CriteriaOperatorBase;
                        }
                        else
                        {
                            criteriaCount = criteriaOperatorFilter;
                        }

                        var countResult = GlobalFramework.SessionXpo.Evaluate(typeof(fin_documentfinancepayment), CriteriaOperator.Parse("Count()"), criteriaCount);
                        string nDocs = dialog.GenericTreeView.DataSource.Count.ToString();
                        string showResults = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_show_results"), nDocs, countResult);

                        _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);
                        dialog.WindowTitle = _selectRecordWindowTitle;
                    }
                    else if (args.ResponseId == (ResponseType)DialogResponseType.Filter)
                    {
                        //Reset current page to 1 ( Pagination go to defined initialy )
                        dialog.GenericTreeView.CurrentPageNumber = 1;

                        PosReportsQueryDialog dialogPayedDocuments = new PosReportsQueryDialog(dialog, DialogFlags.Modal, ReportsQueryDialogMode.FILTER_PAYMENT_DOCUMENTS, typeof(fin_documentfinancepayment).Name);
                        DialogResponseType response = (DialogResponseType)dialogPayedDocuments.Run();
                        List<string> result = new List<string>();

                        // Filter SellDocuments
                        string filterField = string.Empty;
                        string statusField = string.Empty;
                        string extraFilter = string.Empty;

                        if (DialogResponseType.CleanFilter.Equals(response))
                        {
                            dialog.GenericTreeView.DataSource.Criteria = CriteriaOperatorBase;
                            dialog.GenericTreeView.DataSource.TopReturnedObjects = SettingsApp.PaginationRowsPerPage * dialog.GenericTreeView.CurrentPageNumber;

                            dialog.GenericTreeView.Refresh();
                            criteriaOperatorFilter = null;

                            var countResult = GlobalFramework.SessionXpo.Evaluate(typeof(fin_documentfinancepayment), CriteriaOperator.Parse("Count()"), CriteriaOperatorBase);
                            string nDocs = dialog.GenericTreeView.DataSource.Count.ToString();
                            string showResults = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_show_results"), nDocs, countResult);
                            //Finish Updating Title
                            dialog.WindowTitle = string.Format("{0} :: {1} ", windowTitleDefault, showResults);
                        }
                        else if (DialogResponseType.Ok.Equals(response))
                        {
                            filterField = "DocumentType";
                            statusField = "DocumentStatusStatus";

                            /* IN009066 - FS and NC added to reports */
                            extraFilter = $@" AND ({statusField} <> 'A') AND (
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeInvoice}' OR 
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice}' OR 
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment}' OR 
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice}' OR 
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeDebitNote}' OR 
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeCreditNote}' OR 
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypePayment}' 
			                    OR 
			                    {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput}'
			                    )".Replace(Environment.NewLine, string.Empty);
                            /* IN009089 - # TO DO: above, we need to check with business this condition:  {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput}' */

                            // Assign Dialog FilterValue to Method Result Value
                            result.Add($"{dialogPayedDocuments.FilterValue}{extraFilter}");
                            result.Add(dialogPayedDocuments.FilterValueHumanReadble);

                            string addFilter = dialogPayedDocuments.FilterValue;
                            string criteriaOperatorShared = "(Disabled IS NULL OR Disabled  <> 1) AND";

                            //Reset current page to 1 ( Pagination go to defined initialy )
                            dialog.GenericTreeView.CurrentPageNumber = 1;

                            CriteriaOperator criteriaOperator = CriteriaOperator.Parse(string.Format("{0} {1}", criteriaOperatorShared, addFilter));
                            dialog.GenericTreeView.DataSource.Criteria = criteriaOperator;
                            dialog.GenericTreeView.DataSource.TopReturnedObjects = SettingsApp.PaginationRowsPerPage * dialog.GenericTreeView.CurrentPageNumber;

                            criteriaOperatorFilter = criteriaOperator;
                            dialog.GenericTreeView.Refresh();

                            var countResult = GlobalFramework.SessionXpo.Evaluate(typeof(fin_documentfinancepayment), CriteriaOperator.Parse("Count()"), criteriaOperatorFilter);
                            string nDocs = dialog.GenericTreeView.DataSource.Count.ToString();
                            string showResults = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_show_results"), nDocs, countResult);
                            //Finish Updating Title
                            dialog.WindowTitle = string.Format("{0} :: {1} ", windowTitleDefault, showResults);
                        }
                        else
                        {
                            // Destroy Dialog on Cancel
                            dialogPayedDocuments.Destroy();
                            dialog.GenericTreeView.Refresh();
                            // Assign Result
                            result = null;
                        }
                        dialogPayedDocuments.Destroy();
                        dialog.GenericTreeView.Refresh();
                    }
                    //COMMON : Always refresh TreeView, IF Dialog Returns a Valid Document/OK
                    if (resultDocument != null)
                    { // IN009223 IN009227 - End
                      //Refresh Treeview
                        dialog.GenericTreeView.Refresh();
                        //Reset CheckBoxs
                        dialog.GenericTreeView.MarkedCheckBoxs = 0;
                        //Reset Customer
                        _selectedDocumentEntityOid = new Guid();
                        //Disable Buttons
                        if (_actionAreaButtonPrintPayment != null) _actionAreaButtonPrintPayment.Button.Sensitive = false;
                        if (_actionAreaButtonPrintPaymentAs != null) _actionAreaButtonPrintPaymentAs.Button.Sensitive = false;
                        if (_actionAreaButtonCancelPayment != null) _actionAreaButtonCancelPayment.Button.Sensitive = false;
                    }
                }
                dialog.Run();
            }
            else
            {
                //Reset listMarkedFinancePaymentDocuments
                _listMarkedFinancePaymentDocuments.Clear();
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Payments : Get Finance Payment Document List (Checked Items)

        private bool TreeModelForEachTask_ActionGetPaymentDocumentsList(TreeModel model, TreePath path, TreeIter iter)
        {
            int columnIndexCheckBox = 1;
            int columnIndexGuid = 2;
            try
            {
                bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));
                Guid itemGuid = new Guid(model.GetValue(iter, columnIndexGuid).ToString());

                if (itemChecked)
                {
                    //Add to FinancePaymentDocuments
                    _listSelectFinancePaymentDocuments.Add((fin_documentfinancepayment)FrameworkUtils.GetXPGuidObject(typeof(fin_documentfinancepayment), itemGuid));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return false;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Payments : UncheckAll FinancePaymentDocuments

        private void UnCheckAll_FinancePaymentDocuments(PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinancePayment> pDialog, bool pRefreshTree)
        {
            //UnCheck all Marked CheckBoxs
            pDialog.GenericTreeView.UnCheckAll();
            //Refresh Tree
            if (pRefreshTree) pDialog.GenericTreeView.Refresh();

            //Dont Disable First and Last button (New and Close)
            for (int i = 1; i < pDialog.ActionAreaButtons.Count - 1; i++)
            {
                //_log.Debug(string.Format("index[{0}], Label[{1}]", i, pDialog.ActionAreaButtons[i].Button.Name));
                pDialog.ActionAreaButtons[i].Button.Sensitive = false;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Payments : Action Print Finance Payment Documents

        private bool TreeModelForEachTask_ActionPrintPayments(TreeModel model, TreePath path, TreeIter iter)
        {
            int columnIndexCheckBox = 1;
            int columnIndexGuid = 2;
            try
            {
                bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));
                Guid itemGuid = new Guid(model.GetValue(iter, columnIndexGuid).ToString());

                if (itemChecked)
                {
                    fin_documentfinancepayment documentFinancePayment = (fin_documentfinancepayment)FrameworkUtils.GetXPGuidObject(typeof(fin_documentfinancepayment), itemGuid);

                    FrameworkCalls.PrintFinanceDocumentPayment(this, _printerChoosed, documentFinancePayment);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return false;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Payments : Call Cancel Finance Payment Documents Dialog

        private void CallCancelFinancePaymentDocumentsDialog(Window pDialog, List<fin_documentfinancepayment> pListSelectDocuments)
        {
            //SAF-T Notes
            //4.4: Documentos de recibos emitidos
            //  4.4.4.8.1. * Estado atual do recibo (PaymentStatus) : “A” — Recibo anulado

            try
            {
                logicpos.Utils.ResponseText dialogResponse;
                DateTime currentDateTime;
                List<string> ignoredDocuments = new List<string>();

                foreach (var document in pListSelectDocuments)
                {
                    if (CanCancelFinancePaymentDocument(document))
                    {
                        string fileWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_input_text_default.png");

                        dialogResponse = Utils.GetInputText(pDialog, DialogFlags.Modal, fileWindowIcon, string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_cancel_document_input_text_label"), document.PaymentRefNo), string.Empty, SettingsApp.RegexAlfaNumericExtended, true);
                        if (dialogResponse.ResponseType == ResponseType.Ok)
                        {
                            //_log.Debug(string.Format("PaymentRefNo:[{0}], DocumentStatusStatus:[{1}], reason:[{2}]", document.PaymentRefNo, document.PaymentStatus, dialogResponse.InputText));
                            currentDateTime = FrameworkUtils.CurrentDateTimeAtomic();
                            document.PaymentStatus = "A";
                            document.PaymentStatusDate = currentDateTime.ToString(SettingsApp.DateTimeFormatCombinedDateTime);
                            document.Reason = dialogResponse.Text;
                            document.SourceID = GlobalFramework.LoggedUser.CodeInternal;
                            document.Save();

                            //Audit
                            FrameworkUtils.Audit("FINANCE_DOCUMENT_CANCELLED", string.Format("{0} {1}: {2}", document.DocumentType.Designation, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_document_cancelled"), document.PaymentRefNo));

                            //Removed Payed BIT to all DocumentPayment Documents (FT and NC)
                            foreach (fin_documentfinancemasterpayment documentPayment in document.DocumentPayment)
                            {
                                _log.Debug(string.Format("{0} : Set Payed to false: [{0}]", documentPayment.DocumentFinanceMaster.DocumentNumber));
                                documentPayment.DocumentFinanceMaster.Payed = false;
                                documentPayment.DocumentFinanceMaster.Save();
                            }
                        }
                        else
                        {
                            //Add to Ignored Documents
                            ignoredDocuments.Add(string.Format("[{0}] : {1}", document.PaymentStatus, document.PaymentRefNo));
                        }
                    }
                    else
                    {
                        //Add to Ignored Documents
                        ignoredDocuments.Add(string.Format("[{0}] : {1}", document.PaymentStatus, document.PaymentRefNo));
                    }
                }

                //Show Ignored Documents
                if (ignoredDocuments.Count > 0) ShowIgnoredDocuments(pDialog, ignoredDocuments);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Check if the DocumentFinancePayment Document can be Cancelled
        /// </summary>
        private bool CanCancelFinancePaymentDocument(fin_documentfinancepayment pDocumentFinancePayment)
        {
            bool result = false;

            if (pDocumentFinancePayment.PaymentStatus != "A")
            {
                result = true;
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Payments : Call Open Finance Payment Documents

        private void OpenFinancePaymentDocuments(Window pDialog, List<fin_documentfinancepayment> pListSelectDocuments)
        {
            List<string> documents = new List<string>();

            try
            {
                foreach (fin_documentfinancepayment document in pListSelectDocuments)
                {
                    if (!LicenceManagement.IsLicensed || !LicenceManagement.CanPrint)
                    {
                        Utils.ShowMessageTouchErrorUnlicencedFunctionDisabled(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printing_function_disabled"));
                    }
                    else documents.Add(ProcessFinanceDocument.GenerateDocumentFinancePaymentPDFIfNotExists(document));
                }

                foreach (var item in documents)
                {
                    if (!LicenceManagement.IsLicensed || !LicenceManagement.CanPrint)
                    {
                        Utils.ShowMessageTouchErrorUnlicencedFunctionDisabled(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printing_function_disabled"));
                    }
                    else if (File.Exists(item))
                    {
                        if (Utils.UsePosPDFViewer() == true && !Utils.IsLinux)
                        {
                            string docPath = FrameworkUtils.OSSlash(string.Format(@"{0}\{1}", Environment.CurrentDirectory, item));
                            var ScreenSizePDF = GlobalApp.ScreenSize;
                            int widthPDF = ScreenSizePDF.Width;
                            int heightPDF = ScreenSizePDF.Height;
                            System.Windows.Forms.Application.Run(new startDocumentViewer(docPath, widthPDF - 50, heightPDF - 25, false));
                        }
                        else
                        {
                            System.Diagnostics.Process.Start(FrameworkUtils.OSSlash(string.Format(@"{0}\{1}", Environment.CurrentDirectory, item)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Shared for FinanceMasterDocuments and FinancePaymentDocuments

        private void ShowIgnoredDocuments(Window pSourceWindow, List<string> pIgnoredDocuments)
        {
            //Show Ignored Documents
            string ignoredDocumentsMessage = string.Empty;
            if (pIgnoredDocuments.Count > 0)
            {
                //Sort Documents List
                pIgnoredDocuments.Sort();

                for (int i = 0; i < pIgnoredDocuments.Count; i++)
                {
                    ignoredDocumentsMessage += string.Format("{0}{1}", Environment.NewLine, pIgnoredDocuments[i]);
                }

                string infoMessage = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "app_info_show_ignored_cancelled_documents"), ignoredDocumentsMessage);
                Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, new Size(600, 400), MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), infoMessage);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //MerchandiseEntry

        private void _touchButtonPosToolbarMerchandiseEntry_Clicked(object sender, EventArgs e)
        {
            ProcessArticleStockParameter response = PosArticleStockDialog.GetProcessArticleStockParameter(this);

            if (response != null)
            {
                ProcessArticleStock.Add(ProcessArticleStockMode.In, response);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Get Sensitive for CloneDocuments, Check if Checked/Marked Documents can be Cloned, returns true if all Documents can be Cloned, 
        //else if one or more cant be Cloned, ex WayBills
        private bool GetSensitiveForCloneDocuments(List<fin_documentfinancemaster> documents)
        {
            bool result = true;
            // Valid DocumentTypes
            Guid[] validDocumentTypes = new Guid[] {
                SettingsApp.XpoOidDocumentFinanceTypeInvoice,
                SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice,
                SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment,
                SettingsApp.XpoOidDocumentFinanceTypeBudget,
                SettingsApp.XpoOidDocumentFinanceTypeProformaInvoice
            };

            try
            {
                foreach (fin_documentfinancemaster item in documents)
                {
                    // Check if is an invalid DocumentType (Outside of validDocumentTypes Array)
                    if (Array.IndexOf(validDocumentTypes, item.DocumentType.Oid) < 0)
                    {
                        // Detected invalid DocumentType, return false
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Get Sensitive for SendEmailDocuments, Check if Checked/Marked Documents has the same Customer

        // fin_documentfinancemaster Overload
        private bool GetSensitiveForSendEmailDocuments(List<fin_documentfinancemaster> documents)
        {
            bool result = true;

            ArrayList customerList = new ArrayList();

            try
            {
                foreach (fin_documentfinancemaster item in documents)
                {
                    if (!customerList.Contains(item.EntityOid))
                    {
                        customerList.Add(item.EntityOid);
                    }
                }

                result = (customerList.Count > 1) ? false : true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        // fin_documentfinancepayment Overload
        private bool GetSensitiveForSendEmailDocuments(List<fin_documentfinancepayment> documents)
        {
            bool result = true;

            ArrayList customerList = new ArrayList();

            try
            {
                foreach (fin_documentfinancepayment item in documents)
                {
                    if (!customerList.Contains(item.EntityOid))
                    {
                        customerList.Add(item.EntityOid);
                    }
                }

                result = (customerList.Count > 1) ? false : true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
