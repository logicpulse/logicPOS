﻿using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Reports;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.shared.Enums;
using LogicPOS.Data.XPO;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Finance.DocumentProcessing;
using LogicPOS.Modules;
using LogicPOS.Modules.StockManagement;
using LogicPOS.Settings;
using LogicPOS.Settings.Extensions;
using LogicPOS.Shared.Article;
using LogicPOS.Shared.CustomDocument;
using LogicPOS.UI;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosDocumentFinanceSelectRecordDialog
    {
        private sys_configurationprinters _printerChoosed;
        private readonly sys_configurationprinters _printerGeneric;

        private readonly ResponseType _responseTypePrint = (ResponseType)DialogResponseType.Print;
        private readonly ResponseType _responseTypePrintAs = (ResponseType)DialogResponseType.PrintAs;
        private readonly ResponseType _responseTypePayCurrentAcountsDocument = (ResponseType)DialogResponseType.PayCurrentAcountsDocument;
        private readonly ResponseType _responseTypeNewDocument = (ResponseType)DialogResponseType.NewDocument;
        private readonly ResponseType _responseTypePayInvoice = (ResponseType)DialogResponseType.PayInvoice;
        private readonly ResponseType _responseTypeCancelDocument = (ResponseType)DialogResponseType.CancelDocument;
        private readonly ResponseType _responseTypeOpenDocument = (ResponseType)DialogResponseType.OpenDocument;
        private readonly ResponseType _responseTypeCloneDocument = (ResponseType)DialogResponseType.CloneDocument;
        private readonly ResponseType _responseTypeSendEmailDocument = (ResponseType)DialogResponseType.SendEmailDocument;

        private List<fin_documentfinancemaster> _listSelectFinanceMasterDocuments;
        private List<fin_documentfinancepayment> _listSelectFinancePaymentDocuments;
        private readonly List<fin_documentfinancemaster> _listMarkedFinanceMasterDocuments = new List<fin_documentfinancemaster>();
        private readonly List<fin_documentfinancepayment> _listMarkedFinancePaymentDocuments = new List<fin_documentfinancepayment>();
        private string _selectRecordWindowTitle;
        private string _afterFilterTitle = null;
        private readonly bool permissionFinanceDocumentCancelDocument = GeneralSettings.LoggedUserHasPermissionTo("FINANCE_DOCUMENT_CANCEL_DOCUMENT");
        private PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinanceMaster> _dialogFinanceDocumentsResponse;
        private IconButtonWithText _btnCaller;

        private Guid _selectedDocumentEntityOid = new Guid();

        private PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinanceMaster> _dialogDocumentFinanceMaster;

        private ActionAreaButton _actionAreaButtonMore;
        private string windowTitleDefault;
        private CriteriaOperator criteriaOperatorFilter = null;

        public XPCollection DataSourceBeforeFilter { get; private set; }
        private CriteriaOperator CriteriaOperatorBase = null;
        private ActionAreaButton _actionAreaButtonFilter;
        private ActionAreaButton _actionAreaButtonPrintDocument;
        private ActionAreaButton _actionAreaButtonPrintDocumentAs;
        private ActionAreaButton _actionAreaButtonClose;

        private ActionAreaButton _actionAreaButtonPayCurrentAcountsDocument;
        private ActionAreaButton _actionAreaButtonNewDocument;
        private ActionAreaButton _actionAreaButtonPayInvoice;
        private ActionAreaButton _actionAreaButtonCancelDocument;

        private ActionAreaButton _actionAreaButtonPrintPayment;
        private ActionAreaButton _actionAreaButtonPrintPaymentAs;
        private ActionAreaButton _actionAreaButtonOpenDocument;
        private ActionAreaButton _actionAreaButtonCloneDocument;
        private ActionAreaButton _actionAreaButtonSendEmailDocument;
        private ActionAreaButton _actionAreaButtonCancelPayment;

        private ReportsQueryDialogMode _filterChoice;

        public decimal TotalDialogFinanceMasterDocuments { get; set; } = 0;

        private void BtnDocuments_Clicked(object sender, EventArgs e)
        {
            var documentsModal = new DocumentsModal(this);
            documentsModal.Run();
            documentsModal.Destroy();

            _btnCaller = (sender as IconButtonWithText);

            string fileActionMore = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_more.png";
            string fileActionFilter = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_filter.png";
            string fileActionClose = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_close.png";
            string fileActionPrint = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_print.png";
            string fileActionNewDocument = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_finance_new_document.png";
            string fileActionPayInvoice = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_full.png";
            string fileActionCancel = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_cancel_document.png";
            bool generatePdfDocuments = AppSettings.Instance.generatePdfDocuments;

            IconButtonWithText buttonMore = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.More, "touchButtonMore_Grey", string.Format(GeneralUtils.GetResourceByName("global_button_label_more"), POSSettings.PaginationRowsPerPage), fileActionMore);
            IconButtonWithText buttonFilter = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Filter, "touchButtonFilter_Green", "Filter", fileActionFilter);
            IconButtonWithText buttonClose = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);
            IconButtonWithText buttonPrintDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Print, "touchButtonPrintDocument_Green");
            IconButtonWithText buttonPrintDocumentAs = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.PrintAs, "touchButtonPrintDocumentAs_Green");
            IconButtonWithText buttonCancelDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButtonCancelDocument_Green", GeneralUtils.GetResourceByName("global_button_label_cancel_document"), IconSettings.ActionCancel);
            IconButtonWithText buttonOpenDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument, "touchButtonOpenDocument_Green");
            IconButtonWithText buttonSendEmailDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.SendEmailDocument, "touchButtonSendEmailDocument_Green");
            buttonPrintDocument.Sensitive = false;
            buttonPrintDocumentAs.Sensitive = false;
            buttonCancelDocument.Sensitive = false;
            buttonOpenDocument.Sensitive = false;
            buttonSendEmailDocument.Sensitive = false;

            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();

            _selectRecordWindowTitle = string.Empty;
            CriteriaOperator criteriaOperator = null;
            _actionAreaButtonNewDocument = null;
            _actionAreaButtonPayInvoice = null;
            _actionAreaButtonPayCurrentAcountsDocument = null;
            _actionAreaButtonCancelDocument = new ActionAreaButton(buttonCancelDocument, _responseTypeCancelDocument);
            _actionAreaButtonSendEmailDocument = new ActionAreaButton(buttonSendEmailDocument, _responseTypeSendEmailDocument);

            _actionAreaButtonMore = new ActionAreaButton(buttonMore, (ResponseType)DialogResponseType.LoadMore);
            _actionAreaButtonFilter = new ActionAreaButton(buttonFilter, (ResponseType)DialogResponseType.Filter);

            string criteriaOperatorShared = "(Disabled IS NULL OR Disabled  <> 1) AND ";
            string sqlCountTopResults = "0";
            string sqlCountResultTopResults = "0";

            switch (_btnCaller.Token)
            {
                case "ALL":
                    criteriaOperator = CriteriaOperator.Parse(string.Format("{0} DocumentType <> '{1}'", criteriaOperatorShared, DocumentSettings.CurrentAccountInputId));
                    CriteriaOperatorBase = criteriaOperator;// IN009223 IN009227

                    var countResult = XPOSettings.Session.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperatorBase);

                    if (DatabaseSettings.DatabaseType.ToString() == "MySql" || DatabaseSettings.DatabaseType.ToString() == "SQLite")
                    {
                        string filterCriteriaOperatorBase = CriteriaOperatorBase.ToString().Replace("[", "").Replace("]", "");
                        sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT * FROM fin_documentfinancemaster WHERE {1}) AS Total LIMIT {0};", POSSettings.PaginationRowsPerPage, filterCriteriaOperatorBase);
                        sqlCountResultTopResults = XPOSettings.Session.ExecuteScalar(sqlCountTopResults).ToString();
                    }
                    else if (DatabaseSettings.DatabaseType.ToString() == "MSSqlServer")
                    {
                        sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT TOP {0} * FROM fin_documentfinancemaster WHERE {1}) AS Total;", POSSettings.PaginationRowsPerPage, CriteriaOperatorBase);
                        sqlCountResultTopResults = XPOSettings.Session.ExecuteScalar(sqlCountTopResults).ToString();
                    }
                    if (Convert.ToInt32(sqlCountResultTopResults) > Convert.ToInt32(POSSettings.PaginationRowsPerPage))
                    {
                        sqlCountResultTopResults = POSSettings.PaginationRowsPerPage.ToString();
                    }
                    windowTitleDefault = GeneralUtils.GetResourceByName("window_title_select_finance_document");
                    string showResults = string.Format(GeneralUtils.GetResourceByName("window_title_show_results"), sqlCountResultTopResults, countResult.ToString());
                    _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);

                    IconButtonWithText buttonNewDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButtonNewDocument_Green", GeneralUtils.GetResourceByName("global_button_label_new_financial_document"), fileActionNewDocument);
                    _actionAreaButtonNewDocument = new ActionAreaButton(buttonNewDocument, _responseTypeNewDocument);
                    IconButtonWithText buttonCloneDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.CloneDocument, "touchButtonCloneDocument_Green");
                    buttonCloneDocument.Sensitive = false;
                    _actionAreaButtonCloneDocument = new ActionAreaButton(buttonCloneDocument, _responseTypeCloneDocument);
                    actionAreaButtons.Add(_actionAreaButtonNewDocument);
                    actionAreaButtons.Add(_actionAreaButtonCloneDocument);
                    buttonNewDocument.Sensitive = (
                        XPOSettings.WorkSessionPeriodTerminal != null
                        && XPOSettings.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open
                    );

                    actionAreaButtons.Add(_actionAreaButtonCancelDocument);
                    _filterChoice = ReportsQueryDialogMode.FILTER_DOCUMENTS_PAGINATION;
                    break;
                case "FT_UNPAYED":
                    string payed = " ";
                    if (DatabaseSettings.DatabaseType.IsSQLite())
                    {
                        payed = "0";
                    }
                    else { payed = "False"; }
                    string criteriaOperatorString = $"{criteriaOperatorShared} (DocumentType = '{InvoiceSettings.InvoiceId}' OR DocumentType = '{CustomDocumentSettings.CreditNoteId}' OR DocumentType = '{DocumentSettings.XpoOidDocumentFinanceTypeInvoiceWayBill}') AND DocumentStatusStatus <> 'A' AND Payed = '{payed}'";
                    criteriaOperator = CriteriaOperator.Parse(criteriaOperatorString);

                    string sql = @"
SELECT
    DocFinMaster.Oid, DocFinMaster.EntityOid, DocFinMaster.DocumentType, DocFinMaster.DocumentParent
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



                    var sqlResult = XPOSettings.Session.ExecuteQuery(string.Format(sql, criteriaOperatorString));
                    var query = string.Format(sql, criteriaOperatorString);
                    List<Guid> resultList = new List<Guid>();
                    string inClause = string.Empty;

                    //Filtrar documentos Notas de crédito
                    foreach (var item in sqlResult.ResultSet[0].Rows)
                    {
                        //Mostrar notas crédito de documentos que já tenham sido liquidados
                        var docMaster = (fin_documentfinancemaster)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancemaster), Guid.Parse(Convert.ToString(item.Values[0])));
                        string docParent = "";
                        bool showCreditNote = false;
                        if (item.Values[3] != null)
                        {
                            docParent = item.Values[3].ToString();
                            if (!string.IsNullOrEmpty(docParent))
                            {
                                var getDocParentXpoObject = (fin_documentfinancemaster)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancemaster), Guid.Parse(docParent));
                                if (getDocParentXpoObject != null && getDocParentXpoObject.Payed == true && docMaster.Payed == false)
                                {
                                    showCreditNote = true;
                                }
                            }
                        }

                        //Mostrar notas de crédito apenas para para clientes com saldo positivo / Mostrar notas crédito de documentos que já tenham sido liquidados
                        var sqlbalanceTotal = string.Format("SELECT Balance FROM view_documentfinancecustomerbalancesummary WHERE (EntityOid = '{0}');", Convert.ToString(item.Values[1]));
                        var getCustomerBalance = XPOSettings.Session.ExecuteScalar(sqlbalanceTotal);
                        if ((getCustomerBalance != null && (Convert.ToDecimal(getCustomerBalance) > 0) && Guid.Parse(Convert.ToString(item.Values[2])) == CustomDocumentSettings.CreditNoteId) ||
                            (Guid.Parse(Convert.ToString(item.Values[2])) != CustomDocumentSettings.CreditNoteId) ||
                            showCreditNote
                            )
                        {
                            resultList.Add(new Guid(Convert.ToString(item.Values[0])));
                            inClause += "'" + (Convert.ToString(item.Values[0])) + "', ";
                        }
                    }
                    string countQuery = string.Empty;


                    countResult = resultList.Count.ToString();

                    if (DatabaseSettings.DatabaseType.ToString() == "MySql" || DatabaseSettings.DatabaseType.ToString() == "SQLite")
                    {
                        string filterCriteriaOperatorBase = criteriaOperatorString.ToString().Replace("[", "").Replace("]", "");
                        sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT * FROM fin_documentfinancemaster WHERE {1} {2}) AS Total LIMIT {0};", POSSettings.PaginationRowsPerPage, filterCriteriaOperatorBase, countQuery);
                        sqlCountResultTopResults = XPOSettings.Session.ExecuteScalar(sqlCountTopResults).ToString();
                    }
                    else if (DatabaseSettings.DatabaseType.ToString() == "MSSqlServer")
                    {
                        sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT TOP {0} * FROM fin_documentfinancemaster WHERE {1} {2}) AS Total;", POSSettings.PaginationRowsPerPage, criteriaOperatorString, countQuery);
                        sqlCountResultTopResults = XPOSettings.Session.ExecuteScalar(sqlCountTopResults).ToString();
                    }


                    criteriaOperator = CriteriaOperator.And(criteriaOperator, new InOperator("Oid", resultList));
                    CriteriaOperatorBase = criteriaOperator;// IN009223 IN009227

                    windowTitleDefault = GeneralUtils.GetResourceByName("window_title_select_finance_document_ft_unpaid");
                    showResults = string.Format(GeneralUtils.GetResourceByName("window_title_show_results"), sqlCountResultTopResults, countResult);
                    if (!string.IsNullOrEmpty(showResults))
                    {
                        _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);
                    }
                    else
                    {
                        _selectRecordWindowTitle = string.Format("{0} ::", windowTitleDefault);
                    }


                    IconButtonWithText buttonPayInvoice = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButtonPayInvoice_Green", GeneralUtils.GetResourceByName("global_button_label_pay_invoice"), fileActionPayInvoice);
                    buttonPayInvoice.Sensitive = false;
                    _actionAreaButtonPayInvoice = new ActionAreaButton(buttonPayInvoice, _responseTypePayInvoice);
                    actionAreaButtons.Add(_actionAreaButtonPayInvoice);
                    //Add Shared Buttons
                    //_actionAreaButtonCancelDocument = new ActionAreaButton(buttonCancelDocument, _responseTypeCancelDocument);
                    actionAreaButtons.Add(_actionAreaButtonCancelDocument);
                    _filterChoice = ReportsQueryDialogMode.FILTER_DOCUMENTS_UNPAYED;// IN009223 IN009227
                    break;
                // CurrentAccount
                case "CC":
                    criteriaOperator = CriteriaOperator.Parse(string.Format("{0} DocumentType = '{1}' AND Payed = 0", criteriaOperatorShared, DocumentSettings.CurrentAccountInputId));
                    CriteriaOperatorBase = criteriaOperator;// IN009223 IN009227

                    countResult = XPOSettings.Session.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperatorBase);

                    if (DatabaseSettings.DatabaseType.ToString() == "MySql" || DatabaseSettings.DatabaseType.ToString() == "SQLite")
                    {
                        string filterCriteriaOperatorBase = CriteriaOperatorBase.ToString().Replace("[", "").Replace("]", "");
                        sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT * FROM fin_documentfinancemaster WHERE {1}) AS Total LIMIT {0};", POSSettings.PaginationRowsPerPage, filterCriteriaOperatorBase);
                        sqlCountResultTopResults = XPOSettings.Session.ExecuteScalar(sqlCountTopResults).ToString();
                    }
                    else if (DatabaseSettings.DatabaseType.ToString() == "MSSqlServer")
                    {
                        sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT TOP {0} * FROM fin_documentfinancemaster WHERE {1}) AS Total;", POSSettings.PaginationRowsPerPage, CriteriaOperatorBase);
                        sqlCountResultTopResults = XPOSettings.Session.ExecuteScalar(sqlCountTopResults).ToString();
                    }



                    windowTitleDefault = GeneralUtils.GetResourceByName("window_title_select_finance_document_cc");
                    showResults = string.Format(GeneralUtils.GetResourceByName("window_title_show_results"), sqlCountResultTopResults, countResult);
                    _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);

                    //Add aditional Buttons to ActionArea
                    IconButtonWithText buttonPayCurrentAcountDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButtonPayCurrentAcountDocument_Green", GeneralUtils.GetResourceByName("global_button_label_pay"), fileActionPayInvoice);
                    buttonPayCurrentAcountDocument.Sensitive = false;
                    _actionAreaButtonPayCurrentAcountsDocument = new ActionAreaButton(buttonPayCurrentAcountDocument, _responseTypePayCurrentAcountsDocument);
                    actionAreaButtons.Add(_actionAreaButtonPayCurrentAcountsDocument);
                    //Add Shared Buttons
                    //_actionAreaButtonCancelDocument = new ActionAreaButton(buttonCancelDocument, _responseTypeCancelDocument);
                    actionAreaButtons.Add(_actionAreaButtonCancelDocument);
                    _filterChoice = ReportsQueryDialogMode.FILTER_PAYMENT_DOCUMENTS;// IN009223 IN009227
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
            TotalDialogFinanceMasterDocuments = 0;
            _dialogDocumentFinanceMaster = new PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinanceMaster>(
                this,
                DialogFlags.DestroyWithParent,
                _selectRecordWindowTitle,
                //TODO:THEME
                GlobalApp.MaxWindowSize,
                null, //XpoDefaultValue
                criteriaOperator,
                GridViewMode.CheckBox,
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

        private void dialogDocumentFinanceMaster_CheckBoxToggled(object sender, EventArgs e)
        {
            bool itemChecked;

            //WorkStation : Store Session Open/Close Status, some operations must be disable when Session is Closed, like Payment Operations
            bool hasOpenWorkStation = (XPOSettings.WorkSessionPeriodTerminal != null && XPOSettings.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open);
            fin_documentfinancemaster documentFinanceMaster;
            //Default Mode 
            if (_dialogDocumentFinanceMaster.GenericTreeViewMode == GridViewMode.Default)
            {
                //XPOMode
                if (_dialogDocumentFinanceMaster.GenericTreeView.Entity != null)
                {
                    documentFinanceMaster = (fin_documentfinancemaster)_dialogDocumentFinanceMaster.GenericTreeView.Entity;
                    TotalDialogFinanceMasterDocuments = documentFinanceMaster.TotalFinal;
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
                    TotalDialogFinanceMasterDocuments = 0.0m;
                    //Enable/Disable Buttons
                    _actionAreaButtonPrintDocument.Button.Sensitive = false;
                    _actionAreaButtonPrintDocumentAs.Button.Sensitive = false;
                    if (_actionAreaButtonPayInvoice != null) _actionAreaButtonPayInvoice.Button.Sensitive = false;
                    if (_actionAreaButtonPayCurrentAcountsDocument != null) _actionAreaButtonPayCurrentAcountsDocument.Button.Sensitive = false;
                    if (_actionAreaButtonCancelDocument != null) _actionAreaButtonCancelDocument.Button.Sensitive = false;
                }
            }
            //CheckBox Mode
            else if (_dialogDocumentFinanceMaster.GenericTreeViewMode == GridViewMode.CheckBox)
            {
                if (_dialogDocumentFinanceMaster.GenericTreeView.MarkedCheckBoxs > 0)
                {
                    //Get value from Model, its Outside XPGuidObject Scope
                    itemChecked = (bool)_dialogDocumentFinanceMaster.GenericTreeView.GetCurrentModelCheckBoxValue();
                    documentFinanceMaster = (fin_documentfinancemaster)_dialogDocumentFinanceMaster.GenericTreeView.Entity;

                    // Add/Remove MarkedFinanceMasterDocuments on click/mark Document
                    if (itemChecked)
                    {
                        _listMarkedFinanceMasterDocuments.Add(documentFinanceMaster);
                        //_logger.Debug(string.Format("_listMarkedFinanceMasterDocuments count: [{0}], Added: [{1}]", _listMarkedFinanceMasterDocuments.Count, documentFinanceMaster.DocumentNumber));
                    }
                    else
                    {
                        _listMarkedFinanceMasterDocuments.Remove(documentFinanceMaster);
                        //_logger.Debug(string.Format("_listMarkedFinanceMasterDocuments count: [{0}], Removed: [{1}]", _listMarkedFinanceMasterDocuments.Count, documentFinanceMaster.DocumentNumber));
                    }

                    // Get Sensitive for Clone : Required for actionAreaButtonCloneDocument.Button.Sensitive
                    bool validMarkedDocumentTypesForCloneSensitive = GetSensitiveForCloneDocuments(_listMarkedFinanceMasterDocuments);

                    // Get Sensitive for SendEmail
                    bool validMarkedDocumentTypesForSendEmailSensitive = GetSensitiveForSendEmailDocuments(_listMarkedFinanceMasterDocuments);

                    //Customer Protection (Payments) to prevent Choosing Diferent Customers in MultiSelect
                    if (_btnCaller.Token == "FT_UNPAYED")
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
                        decimal documentValue;
                        //Get documentValue from Document.TotalFinal to Increment/Decrement base on Credit Bool, ex Credit=True Value=100 else if Credit=False Value=-100
                        if (documentFinanceMaster.DocumentType.Credit)
                        {
                            /* IN009166 - Making Total calculation based on window option selected 
                             * "FT_UNPAYED": Payment Documents
                             */
                            if ("FT_UNPAYED".Equals(_btnCaller.Token))
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
                                decimal documentDebit = Convert.ToDecimal(XPOSettings.Session.ExecuteScalar(sql));
                                /* IN009166 - forced to use "TotalFinalRound" and not "TotalFinal" instead */
                                documentValue = (documentDebit != 0) ? documentDebit : documentFinanceMaster.TotalFinal;
                            }
                            else
                            {
                                /* IN009166 - Total of Documents selected
                                 * When getting TotalFinal, the calculation returns a value different than the one is being shown.
                                 * Then, we selected TotalFinalRound as show on Documents window for each register.
                                 */
                                // documentDebit = Convert.ToDecimal(XPOSettings.Session.ExecuteScalar(sql));
                                // documentValue = (documentDebit != 0) ? documentDebit : documentFinanceMaster.TotalFinal;
                                documentValue = documentFinanceMaster.TotalFinal;
                            }
                        }
                        else
                        {
                            /* IN009166 */
                            documentValue = -documentFinanceMaster.TotalFinal;
                        };
                        TotalDialogFinanceMasterDocuments += (itemChecked) ? documentValue : -documentValue;
                    }

                    //Enable/Disable Buttons for all Modes
                    _actionAreaButtonPrintDocument.Button.Sensitive = true;
                    _actionAreaButtonPrintDocumentAs.Button.Sensitive = true;
                    if (!GeneralSettings.AppUseParkingTicketModule)
                    {
                        _actionAreaButtonOpenDocument.Button.Sensitive = true;
                    }
                    _actionAreaButtonFilter.Button.Sensitive = true;
                    _actionAreaButtonMore.Button.Sensitive = true;
                    if (_actionAreaButtonCloneDocument != null) _actionAreaButtonCloneDocument.Button.Sensitive = validMarkedDocumentTypesForCloneSensitive;
                    _actionAreaButtonSendEmailDocument.Button.Sensitive = validMarkedDocumentTypesForSendEmailSensitive;
                    if (_actionAreaButtonPayCurrentAcountsDocument != null && _btnCaller.Token == "CC") _actionAreaButtonPayCurrentAcountsDocument.Button.Sensitive = hasOpenWorkStation;
                    //Must Greater than zero to Pay Something or Zero to Issue Zero Payment Document and Set Payed to Documents
                    if (_actionAreaButtonPayInvoice != null && _btnCaller.Token == "FT_UNPAYED" && documentFinanceMaster.DocumentStatusStatus != "F") _actionAreaButtonPayInvoice.Button.Sensitive = (hasOpenWorkStation && TotalDialogFinanceMasterDocuments >= 0);
                    //Cancel Documents must me in ALL, CC, or FT_UNPAYED Mode 
                    if (_actionAreaButtonCancelDocument != null && (
                            _btnCaller.Token == "ALL" ||
                            _btnCaller.Token == "FT_UNPAYED" ||
                            _btnCaller.Token == "CC"
                            )
                            && permissionFinanceDocumentCancelDocument
                        )
                        _actionAreaButtonCancelDocument.Button.Sensitive = true;
                }
                else
                {
                    //Reset Helper Vars
                    TotalDialogFinanceMasterDocuments = 0.0m;
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

            var countResult = XPOSettings.Session.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), criteriaCount);
            string nDocs = _dialogDocumentFinanceMaster.GenericTreeView.Entities.Count.ToString();
            string showResults = string.Format(GeneralUtils.GetResourceByName("window_title_show_results"), nDocs, countResult);

            //Finish Updating Title
            _dialogDocumentFinanceMaster.WindowSettings.WindowTitle.Text = (TotalDialogFinanceMasterDocuments != 0) ? string.Format("{0} :: {1} - {2}", windowTitleDefault, DataConversionUtils.DecimalToStringCurrency(TotalDialogFinanceMasterDocuments, XPOSettings.ConfigurationSystemCurrency.Acronym), showResults) : string.Format("{0} :: {1}", windowTitleDefault, showResults);

        }

        private void dialogFinanceMasterDocuments_Response(object o, ResponseArgs args)
        {
            //Get Sender Reference : require for use Transient
            _dialogFinanceDocumentsResponse = (PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinanceMaster>)o;

            fin_documentfinancemaster documentFinanceMaster = (fin_documentfinancemaster)_dialogFinanceDocumentsResponse.GenericTreeView.Entity;

            if (args.ResponseId != ResponseType.Close)
            {
                //Assign Choosed Printer based on user ResponseType
                if (documentFinanceMaster.SourceOrderMain != null)
                {
                    _printerChoosed = (args.ResponseId == _responseTypePrint) ? TerminalSettings.LoggedTerminal.ThermalPrinter : _printerGeneric;
                }
                else
                {
                    _printerChoosed = (args.ResponseId == _responseTypePrint) ? TerminalSettings.LoggedTerminal.Printer : _printerGeneric;
                }

                if (
                        (args.ResponseId == _responseTypePrint && !logicpos.Utils.ShowMessageTouchRequiredValidPrinter(_dialogFinanceDocumentsResponse, _printerChoosed))
                        || args.ResponseId == _responseTypePrintAs
                    )
                {
                    //Assign Choosed Printer based on user ResponseType
                    //TK016249 - Impressoras - Diferenciação entre Tipos (helio)
                    if (documentFinanceMaster.SourceOrderMain == null) { _printerChoosed = (args.ResponseId == _responseTypePrint) ? TerminalSettings.LoggedTerminal.Printer : _printerGeneric; }
                    else { _printerChoosed = (args.ResponseId == _responseTypePrint) ? TerminalSettings.LoggedTerminal.ThermalPrinter : _printerGeneric; }


                    //Single Record Mode - Default - USED HERE ONLY TO TEST Both Dialogs Modes (Default and CheckBox)
                    if (_dialogFinanceDocumentsResponse.GenericTreeViewMode == GridViewMode.Default)
                    {
                        FrameworkCalls.PrintFinanceDocument(this, _printerChoosed, documentFinanceMaster);
                    }
                    //Multi Record Mode - CheckBox - ACTIVE MODE
                    else if (_dialogFinanceDocumentsResponse.GenericTreeViewMode == GridViewMode.CheckBox)
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
                    if (_dialogFinanceDocumentsResponse.GenericTreeViewMode == GridViewMode.Default)
                    {
                        _listSelectFinanceMasterDocuments.Add(documentFinanceMaster);
                    }
                    //Multi Record Mode - CheckBox - ACTIVE MODE
                    else if (_dialogFinanceDocumentsResponse.GenericTreeViewMode == GridViewMode.CheckBox)
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
                        resultDocument = CallPayInvoicesDialog(_dialogFinanceDocumentsResponse, TotalDialogFinanceMasterDocuments);
                        //UnCheck all Marked CheckBoxs
                        if (resultDocument != null) UnCheckAll_FinanceMasterDocuments(_dialogFinanceDocumentsResponse, false);

                        //Update Window title and reset filter if zero results in filter after pay invoices
                        _dialogFinanceDocumentsResponse.GenericTreeView.Entities.Criteria = CriteriaOperatorBase;
                        _dialogFinanceDocumentsResponse.GenericTreeView.Entities.TopReturnedObjects = POSSettings.PaginationRowsPerPage * _dialogFinanceDocumentsResponse.GenericTreeView.CurrentPageNumber;
                        _dialogFinanceDocumentsResponse.GenericTreeView.Refresh();
                        string nDocs = _dialogDocumentFinanceMaster.GenericTreeView.Entities.Count.ToString();

                        if (nDocs == "0")
                        {
                            _dialogFinanceDocumentsResponse.GenericTreeView.Entities.Criteria = CriteriaOperatorBase;
                            _dialogFinanceDocumentsResponse.GenericTreeView.Entities.TopReturnedObjects = POSSettings.PaginationRowsPerPage * _dialogFinanceDocumentsResponse.GenericTreeView.CurrentPageNumber;
                            _dialogFinanceDocumentsResponse.GenericTreeView.Refresh();
                            criteriaOperatorFilter = null;
                            _afterFilterTitle = null;

                            var countResult = XPOSettings.Session.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperatorBase);
                            nDocs = _dialogDocumentFinanceMaster.GenericTreeView.Entities.Count.ToString();
                            var showResults = string.Format(GeneralUtils.GetResourceByName("window_title_show_results"), nDocs, countResult);
                            _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);
                            _dialogDocumentFinanceMaster.WindowSettings.WindowTitle.Text = _selectRecordWindowTitle;
                        }
                        else
                        {
                            var countResult = XPOSettings.Session.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), criteriaOperatorFilter);
                            string showResults = string.Format(GeneralUtils.GetResourceByName("window_title_show_results"), nDocs, countResult);
                            _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);
                            _dialogDocumentFinanceMaster.WindowSettings.WindowTitle.Text = _selectRecordWindowTitle;
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
                        ResponseType dialogResponse = logicpos.Utils.ShowMessageBox(_dialogFinanceDocumentsResponse, DialogFlags.Modal, new Size(700, 440), MessageType.Question, ButtonsType.OkCancel, GeneralUtils.GetResourceByName("global_question"),
                            string.Format(GeneralUtils.GetResourceByName("window_title_dialog_clone_documents_confirmation"), documentList)
                        );

                        if (dialogResponse == ResponseType.Ok)
                        {
                            //Start Open Documents
                            CallCloneFinanceMasterDocuments(_dialogFinanceDocumentsResponse, _listSelectFinanceMasterDocuments);

                            //UnCheck all Marked CheckBoxs, After call CallCancelDocumentsDialog
                            UnCheckAll_FinanceMasterDocuments(_dialogFinanceDocumentsResponse, true);

                            //IN009255 Usabilidade - Opção de Copiar Documentos não apresenta mensagem de sucesso ao usuário. 
                            //Mensagem de sucesso clone
                            logicpos.Utils.ShowMessageTouch(this, DialogFlags.DestroyWithParent | DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_button_label_clone_document"), GeneralUtils.GetResourceByName("dialog_message_operation_successfully"));
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
                        TotalDialogFinanceMasterDocuments = 0.0m;
                        //Reset Customer
                        _selectedDocumentEntityOid = new Guid();
                        //Finish Updating Title
                        string nDocs = _dialogDocumentFinanceMaster.GenericTreeView.Entities.Count.ToString();
                        string nDocsTotal = _dialogDocumentFinanceMaster.GenericTreeView.Entities.Count.ToString();

                        _dialogDocumentFinanceMaster.WindowSettings.WindowTitle.Text = string.Format("{0} - Numero de Documentos: {1}", _selectRecordWindowTitle, nDocs);
                        _dialogFinanceDocumentsResponse.WindowSettings.WindowTitle.Text = _selectRecordWindowTitle;
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
                    throw new NotImplementedException();
                }// IN009223 IN009227 - Begin
                else if (args.ResponseId == (ResponseType)DialogResponseType.LoadMore)
                {

                    _dialogFinanceDocumentsResponse.GenericTreeView.Entities.TopReturnedObjects = (POSSettings.PaginationRowsPerPage * _dialogFinanceDocumentsResponse.GenericTreeView.CurrentPageNumber);
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

                    var countResult = XPOSettings.Session.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), criteriaCount);
                    string nDocs = _dialogDocumentFinanceMaster.GenericTreeView.Entities.Count.ToString();
                    string showResults = string.Format(GeneralUtils.GetResourceByName("window_title_show_results"), nDocs, countResult);

                    _dialogDocumentFinanceMaster.WindowSettings.WindowTitle.Text = string.Format("{0} :: {1}", windowTitleDefault, showResults);
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

                    PosReportsQueryDialog dialog = new PosReportsQueryDialog(_dialogFinanceDocumentsResponse, DialogFlags.Modal, _filterChoice, "fin_documentfinancemaster", GeneralUtils.GetResourceByName("window_title_dialog_report_filter"));
                    DialogResponseType response = (DialogResponseType)dialog.Run();

                    //IF button Clean Filter Clicked
                    if (DialogResponseType.CleanFilter.Equals(response))
                    {
                        _dialogFinanceDocumentsResponse.GenericTreeView.Entities.Criteria = CriteriaOperatorBase;
                        _dialogFinanceDocumentsResponse.GenericTreeView.Entities.TopReturnedObjects = POSSettings.PaginationRowsPerPage * _dialogFinanceDocumentsResponse.GenericTreeView.CurrentPageNumber;
                        _dialogFinanceDocumentsResponse.GenericTreeView.Refresh();
                        criteriaOperatorFilter = null;
                        _afterFilterTitle = null;

                        var countResult = XPOSettings.Session.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperatorBase);
                        string nDocs = _dialogDocumentFinanceMaster.GenericTreeView.Entities.Count.ToString();
                        string showResults = string.Format(GeneralUtils.GetResourceByName("window_title_show_results"), nDocs, countResult);

                        _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);
                        _dialogDocumentFinanceMaster.WindowSettings.WindowTitle.Text = _selectRecordWindowTitle;
                    }
                    else if (DialogResponseType.Ok.Equals(response))
                    {
                        filterField = "DocumentType";
                        statusField = "DocumentStatusStatus";

                        /* IN009066 - FS and NC added to reports */
                        extraFilter = $@" AND ({statusField} <> 'A') AND (
			                    {filterField} = '{InvoiceSettings.InvoiceId}' OR 
			                    {filterField} = '{DocumentSettings.SimplifiedInvoiceId}' OR 
			                    {filterField} = '{DocumentSettings.InvoiceAndPaymentId}' OR 
			                    {filterField} = '{DocumentSettings.ConsignationInvoiceId}' OR 
			                    {filterField} = '{DocumentSettings.DebitNoteId}' OR 
			                    {filterField} = '{CustomDocumentSettings.CreditNoteId}' OR 
			                    {filterField} = '{DocumentSettings.PaymentDocumentTypeId}' 
			                    OR 
			                    {filterField} = '{DocumentSettings.CurrentAccountInputId}'
			                    )".Replace(Environment.NewLine, string.Empty);
                        /* IN009089 - # TO DO: above, we need to check with business this condition:  {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput}' */

                        // Assign Dialog FilterValue to Method Result Value
                        result.Add($"{dialog.FilterValue}{extraFilter}");
                        result.Add(dialog.FilterValueHumanReadble);
                        string addFilter = dialog.FilterValue;

                        criteriaOperatorFilter = CriteriaOperator.And(CriteriaOperatorBase, CriteriaOperator.Parse(addFilter));
                        //criteriaOperatorFilter = CriteriaOperator.Parse(addFilter);
                        _dialogFinanceDocumentsResponse.GenericTreeView.Entities.Criteria = criteriaOperatorFilter;
                        _dialogFinanceDocumentsResponse.GenericTreeView.Entities.TopReturnedObjects = POSSettings.PaginationRowsPerPage * _dialogFinanceDocumentsResponse.GenericTreeView.CurrentPageNumber;
                        _dialogFinanceDocumentsResponse.GenericTreeView.Refresh();

                        var countResult = XPOSettings.Session.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), criteriaOperatorFilter);
                        string nDocs = _dialogDocumentFinanceMaster.GenericTreeView.Entities.Count.ToString();
                        string showResults = string.Format(GeneralUtils.GetResourceByName("window_title_show_results"), nDocs, countResult);

                        _dialogDocumentFinanceMaster.WindowSettings.WindowTitle.Text = string.Format("{0} :: {1}", windowTitleDefault, showResults);
                        _afterFilterTitle = _dialogDocumentFinanceMaster.WindowSettings.WindowTitle.Text;
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
                if (!string.IsNullOrEmpty(_afterFilterTitle)) { _dialogDocumentFinanceMaster.WindowSettings.WindowTitle.Text = _afterFilterTitle; }

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

        private bool TreeModelForEachTask_ActionPrintDocuments(TreeModel model, TreePath path, TreeIter iter)
        {
            int columnIndexCheckBox = 1;
            int columnIndexGuid = 2;

            bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));
            Guid itemGuid = new Guid(model.GetValue(iter, columnIndexGuid).ToString());

            if (itemChecked)
            {
                fin_documentfinancemaster documentFinanceMaster = XPOUtility.GetEntityById<fin_documentfinancemaster>(itemGuid);
                //Required to use _dialogFinanceDocumentsResponse to Fix TransientFor, ALT+TAB
                FrameworkCalls.PrintFinanceDocument(_dialogFinanceDocumentsResponse, _printerChoosed, documentFinanceMaster);
            }

            return false;
        }

        private fin_documentfinancemaster PayCurrentAcountDocuments(
          PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinanceMaster> parentWindow,
          List<fin_documentfinancemaster> pFinanceDocuments
        )
        {
            //Local Vars
            PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinanceMaster> parentDialog = (PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinanceMaster>)parentWindow;
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
                DocumentProcessingParameters ccCustomerOid = new DocumentProcessingParameters(pFinanceDocuments[0].EntityOid, articleBag);
                ccCustomerOid.Customer = pFinanceDocuments[0].EntityOid;
                /* Please see ERR201810#14 */
                // Call to overloaded constructor method, where "pSkipPersistFinanceDocument" parameter was settled to false when calling main constructor method.
                //
                // We are now calling main class constructor method, setting "pSkipPersistFinanceDocument" parameter to "true" instead, 
                // avoiding double invoice creation issue for "Conta Corrente" payments.
                PaymentDialog dialog = new PaymentDialog(parentWindow, DialogFlags.DestroyWithParent, articleBag, false, false, true, ccCustomerOid, null);

                int response = dialog.Run();
                if (response == (int)ResponseType.Ok)
                {
                    //Prepare ProcessFinanceDocumentParameter
                    DocumentProcessingParameters processFinanceDocumentParameter = new DocumentProcessingParameters(
                      DocumentSettings.SimplifiedInvoiceId, dialog.ArticleBagFullPayment)
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
                    parentDialog.WindowSettings.WindowTitle.Text = _selectRecordWindowTitle;
                    //Reset Totals
                    TotalDialogFinanceMasterDocuments = 0.0m;
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
                return resultDocument;
            }
        }

        private fin_documentfinancepayment CallPayInvoicesDialog(
          PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinanceMaster> parentWindow,
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

            PosPayInvoicesDialog dialogPayInvoices = new PosPayInvoicesDialog(parentWindow, DialogFlags.DestroyWithParent, pPaymentAmount, noOfInvoices);
            ResponseType response = (ResponseType)dialogPayInvoices.Run();
            if (response == ResponseType.Ok)
            {
                //Start Processing Documents
                resultDocumentFinancePayment = PayInvoices(
                    parentWindow,
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

        private fin_documentfinancepayment PayInvoices(
            PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinanceMaster> parentWindow,
            List<fin_documentfinancemaster> pFinanceDocuments,
            Guid pCustomer,
            Guid pPaymentMethod,
            Guid pConfigurationCurrency,
            decimal pPaymentAmount,
            string pPaymentNotes = ""
        )
        {
            //Local Vars
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
                return FrameworkCalls.PersistFinanceDocumentPayment(parentWindow, listInvoices, listCreditNotes, pCustomer, pPaymentMethod, pConfigurationCurrency, pPaymentAmount, pPaymentNotes);
            }
            catch (DocumentProcessingValidationException ex)
            {
                return resultDocument;
            }
        }

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
                        logicpos.Utils.ShowMessageTouch(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Ok, string.Format(GeneralUtils.GetResourceByName("global_warning_cant_open_title")), string.Format(GeneralUtils.GetResourceByName("window_dialog_cant_open_document"), document.DocumentNumber));
                    }
                    else if (LicenseSettings.LicenceRegistered == false)
                    {
                        logicpos.Utils.ShowMessageBoxUnlicensedError(this, GeneralUtils.GetResourceByName("global_printing_function_disabled"));
                    }
                    else
                    {
                        documents.Add(LogicPOS.Reporting.Common.FastReport.GenerateDocumentFinanceMasterPDFIfNotExists(document));
                    }
                }

                foreach (var item in documents)
                {
                    if (LicenseSettings.LicenceRegistered == false)
                    {
                        logicpos.Utils.ShowMessageBoxUnlicensedError(this, GeneralUtils.GetResourceByName("global_printing_function_disabled"));
                    }
                    else if (File.Exists(item))
                    {
                        if (logicpos.Utils.UsePosPDFViewer() == true)
                        {
                            string docPath = string.Format(@"{0}\{1}", Environment.CurrentDirectory, item);
                            var ScreenSizePDF = GlobalApp.ScreenSize;
                            int widthPDF = ScreenSizePDF.Width;
                            int heightPDF = ScreenSizePDF.Height;
                            System.Windows.Forms.Application.Run(new LogicPOS.PDFViewer.Winforms.PDFViewer(docPath, widthPDF - 50, heightPDF - 25, false));
                        }

                    }
                }
            }
            catch (DocumentProcessingValidationException ex)
            {

            }
        }

        private ResponseType CallSendEmailFinanceMasterDocuments(
            PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinanceMaster> parentWindow,
            List<fin_documentfinancemaster> pDocuments
        )
        {
            Dictionary<fin_documentfinancemaster, string> documents = new Dictionary<fin_documentfinancemaster, string>();
            List<string> attachmentFileNames = new List<string>();
            // Get Customer from first Document
            erp_customer customer = XPOUtility.GetEntityById<erp_customer>(pDocuments[0].EntityOid);
            string customerEmail = (customer.Email != null) ? customer.Email : string.Empty;
            string documentList = string.Empty;

            try
            {
                // Call GenerateDocument and add it to List
                foreach (fin_documentfinancemaster document in pDocuments)
                {
                    documents.Add(document, LogicPOS.Reporting.Common.FastReport.GenerateDocumentFinanceMasterPDFIfNotExists(document));
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
                string subject = GeneralSettings.PreferenceParameters["SEND_MAIL_FINANCE_DOCUMENTS_SUBJECT"];
                string mailBodyTemplate = GeneralSettings.PreferenceParameters["SEND_MAIL_FINANCE_DOCUMENTS_BODY"];

                Dictionary<string, string> customTokensDictionary = new Dictionary<string, string>
                {
                    { "DOCUMENT_LIST", documentList }
                };
                // Prepare List of Dictionaries to send to replaceTextTokens
                List<Dictionary<string, string>> tokensDictionaryList = new List<Dictionary<string, string>>
                {
                    GeneralSettings.PreferenceParameters,
                    customTokensDictionary
                };
                string mailBody = StringUtils.ReplaceTextTokens(mailBodyTemplate, tokensDictionaryList);

                PosSendEmailDialog dialog = new PosSendEmailDialog(
                    parentWindow,
                    DialogFlags.Modal,
                    new System.Drawing.Size(800, 640),
                    GeneralUtils.GetResourceByName("window_title_send_email"),
                    subject,
                    customerEmail,
                    mailBody,
                    attachmentFileNames
                    );

                ResponseType responseType = (ResponseType)dialog.Run();
                dialog.Destroy();

                return responseType;
            }
            catch (DocumentProcessingValidationException ex)
            {
                return ResponseType.None;
            }
        }

        private void CallSendEmailFinancePaymentDocuments(
            PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinancePayment> parentWindow,
            List<fin_documentfinancepayment> pDocuments
        )
        {
            Dictionary<fin_documentfinancepayment, string> documents = new Dictionary<fin_documentfinancepayment, string>();
            List<string> attachmentFileNames = new List<string>();
            // Get Customer from first Document
            erp_customer customer = XPOUtility.GetEntityById<erp_customer>(pDocuments[0].EntityOid);
            string customerEmail = (customer.Email != null) ? customer.Email : string.Empty;
            string mailBody = string.Empty;

            // Call GenerateDocument and add it to List
            foreach (fin_documentfinancepayment document in pDocuments)
            {
                documents.Add(document, LogicPOS.Reporting.Common.FastReport.GenerateDocumentFinancePaymentPDFIfNotExists(document));
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
                parentWindow,
                DialogFlags.Modal,
                new System.Drawing.Size(800, 640),
                GeneralUtils.GetResourceByName("window_title_send_email"),
                "Subject",
                customerEmail,
                mailBody,
                attachmentFileNames
                );

            ResponseType responseType = (ResponseType)dialog.Run();
            dialog.Destroy();

        }

        private void CallCloneFinanceMasterDocuments(
            PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinanceMaster> parentWindow,
            List<fin_documentfinancemaster> pFinanceDocuments
        )
        {
            //Local Vars
            List<fin_documentfinancemaster> resultDocument = new List<fin_documentfinancemaster>();

            SortingCollection articleDetailSortingCollection = new SortingCollection
            {
                new SortProperty("Ord", SortingDirection.Ascending)
            };

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

                    // SerialNumber
                    if (!string.IsNullOrEmpty(articleDetail.SerialNumber))
                    {
                        articleBagProps.SerialNumber = articleDetail.SerialNumber;
                    }

                    // Warehouse
                    if (!string.IsNullOrEmpty(articleDetail.Warehouse))
                    {
                        articleBagProps.Warehouse = articleDetail.Warehouse;
                    }
                    articleBag.Add(articleBagKey, articleBagProps);
                }

                // Init ProcessFinanceDocumentParameter
                DocumentProcessingParameters processFinanceDocumentParameter = new DocumentProcessingParameters(document.DocumentType.Oid, articleBag)
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
                DocumentProcessingUtils.PersistFinanceDocument(processFinanceDocumentParameter);
            }
        }

        private void CallCancelFinanceMasterDocumentsDialog(Window pDialog, List<fin_documentfinancemaster> pListSelectDocuments)
        {
            logicpos.Utils.ResponseText dialogResponse;
            DateTime currentDateTime;
            List<string> ignoredDocuments = new List<string>();

            foreach (var documentMaster in pListSelectDocuments)
            {
                //Check if Can Cancell Document
                if (CanCancelFinanceMasterDocument(documentMaster))
                {
                    string fileWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_input_text_default.png";

                    //Call Request Motive Dialog
                    dialogResponse = logicpos.Utils.GetInputText(pDialog,
                                                                 DialogFlags.Modal,
                                                                 fileWindowIcon,
                                                                 string.Format(GeneralUtils.GetResourceByName("global_cancel_document_input_text_label"), documentMaster.DocumentNumber),
                                                                 string.Empty,
                                                                 RegexUtils.RegexAlfaNumericExtendedForMotive,
                                                                 true);

                    if (dialogResponse.ResponseType == ResponseType.Ok)
                    {
                        //_logger.Debug(string.Format("DocumentNumber:[{0}], DocumentStatusStatus:[{1}], reason:[{2}]", document.DocumentNumber, document.DocumentStatusStatus, dialogResponse.InputText));
                        currentDateTime = XPOUtility.CurrentDateTimeAtomic();
                        documentMaster.DocumentStatusStatus = "A";
                        documentMaster.DocumentStatusDate = currentDateTime.ToString(CultureSettings.DateTimeFormatCombinedDateTime);
                        documentMaster.DocumentStatusReason = dialogResponse.Text;
                        documentMaster.DocumentStatusUser = XPOSettings.LoggedUser.CodeInternal;
                        /* IN009083 - uncommented */
                        //ATWS: Check if Sent Resend Document to AT WebServices                                                 //WIP: CancellWayBills : 
                        bool sendDocumentToAT = false;                                                                          //WIP: CancellWayBills : 
                                                                                                                                //Financial.service - Correções no envio de documentos AT [IN:014494]
                        if (CultureSettings.CountryIdIsPortugal(XPOSettings.ConfigurationSystemCountry.Oid)     //WIP: CancellWayBills : 
                            && documentMaster.DocumentType.WsAtDocument                                                         //WIP: CancellWayBills : 
                            && documentMaster.DocumentType.WayBill                                                              //WIP: CancellWayBills : 
                            && documentMaster.DocumentType.Oid != DocumentSettings.XpoOidDocumentFinanceTypeInvoiceWayBill && documentMaster.ShipToCountry != null && documentMaster.ShipToCountry == "PT"       //Envio de Documentos transporte AT (Estrangeiro) [IN:016502]               //WIP: CancellWayBills : 
                            )                                                                                                   //WIP: CancellWayBills : 
                        {                                                                                                       //WIP: CancellWayBills : 
                            sendDocumentToAT = true;                                                                            //WIP: CancellWayBills : 
                        }                                                                                                       //WIP: CancellWayBills : 

                        /* IN009083 - case document is not "DocumentType.WayBill", we will send it to AT if able to */
                        //if (
                        //    SettingsApp.ServiceATSendDocuments
                        //    && SettingsApp.XpoOidConfigurationCountryPortugal.Equals(SettingsApp.ConfigurationSystemCountry.Oid)
                        //    && documentMaster.DocumentType.WsAtDocument
                        //    && !documentMaster.DocumentType.WayBill
                        //    )
                        //{
                        //    sendDocumentToAT = true;
                        //}/* IN009083 - end */

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
                        //{
                        //    /* IN009032 and then removed by IN009083 */
                        //    //documentMaster.ATResendDocument = true;
                        //}

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

                            ignoredDocuments.Add(string.Format("{0} [{1}]", documentMaster.DocumentNumber, GeneralUtils.GetResourceByName("dialog_message_error_in_at_ws_call_status")));
                        }

                        /* Restore stock items */
                        if (!ProcessArticleStockMode.None.Equals(documentMaster.DocumentType.StockMode))
                        {
                            //Process Stock : Restore Stock after Cancelling Documents
                            if (LicenseSettings.LicenseModuleStocks && ModulesSettings.StockManagementModule != null)
                            {
                                ModulesSettings.StockManagementModule.Add(documentMaster, true);
                            }
                            else
                            {
                                ProcessArticleStock.Add(documentMaster, true);
                            }
                        }


                        documentMaster.Save();

                        //Audit
                        XPOUtility.Audit("FINANCE_DOCUMENT_CANCELLED", string.Format("{0} {1}: {2}", documentMaster.DocumentType.Designation, GeneralUtils.GetResourceByName("global_document_cancelled"), documentMaster.DocumentNumber));
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

        private bool CanCancelFinanceMasterDocument(fin_documentfinancemaster pDocumentFinanceMaster)
        {
            bool isCancellable = false;
            DateTime currentDateDay = XPOUtility.CurrentDateTimeAtomicMidnight();
            DateTime documentDateDay = XPOUtility.DateTimeToMidnightDate(pDocumentFinanceMaster.Date);

            //Moçambique - Pedidos da reunião 13/10/2020 [IN:014327]
            //Pode cancelar documentos de origem do tipo fatura ou fatura simplificada
            if ((CultureSettings.MozambiqueCountryId.Equals(XPOSettings.ConfigurationSystemCountry.Oid) && pDocumentFinanceMaster.DocumentStatusStatus != "A" && currentDateDay == documentDateDay))
            {

                isCancellable = true;
                if ((pDocumentFinanceMaster.DocumentType.Oid != DocumentSettings.SimplifiedInvoiceId && pDocumentFinanceMaster.DocumentType.Oid != InvoiceSettings.InvoiceId))
                {/* IN009083 - if invoice products are already in transport */
                    return false;
                }
                if (!pDocumentFinanceMaster.DocumentType.WayBill && IsAlreadyReceivedByAT(pDocumentFinanceMaster.Oid))
                {/* IN009083 - if fiscal document was already received by AT */
                    return false;
                }
                else if (pDocumentFinanceMaster.DocumentType.WayBill && pDocumentFinanceMaster.ShipFromDeliveryDate <= DateTime.Now)
                {/* IN009083 - if invoice products are already in transport */
                    return false;
                }
            }

            /* IN009083 - document is not cancelled nor invoiced and Date of document is Today */
            else if (pDocumentFinanceMaster.DocumentStatusStatus != "A" && pDocumentFinanceMaster.DocumentStatusStatus != "F" && currentDateDay == documentDateDay)
            {
                isCancellable = true;

                if (!pDocumentFinanceMaster.DocumentType.WayBill && IsAlreadyReceivedByAT(pDocumentFinanceMaster.Oid))
                {/* IN009083 - if fiscal document was already received by AT */
                    return false;
                }
                else if (pDocumentFinanceMaster.DocumentType.WayBill && pDocumentFinanceMaster.ShipFromDeliveryDate <= DateTime.Now)
                {/* IN009083 - if invoice products are already in transport */
                    return false;
                }

                /* IN009083 */
                {//Check if Document have dependent non Cancelled Child FinanceDocuments
                    string sqlFinanceMaster = string.Format("SELECT DocumentNumber FROM fin_documentfinancemaster WHERE DocumentParent = '{0}' AND DocumentStatusStatus <> 'A' ORDER BY CreatedAt;", pDocumentFinanceMaster.Oid);
                    SQLSelectResultData xPSelectDataFinanceMaster = XPOUtility.GetSelectedDataFromQuery(sqlFinanceMaster);

                    /* IN009083 - if found one simple dependent, returns */
                    if (xPSelectDataFinanceMaster.DataRows.Length > 0)
                    {
                        return false;
                    }
                }
                /* IN009083 */
                {//Check if Document have dependent non Cancelled Payments
                    string sqlFinancePayment = string.Format("SELECT fmaDocumentNumber AS DocumentNumber FROM view_documentfinancepayment WHERE fmaOid = '{0}' AND fpaPaymentStatus <> 'A' ORDER BY fmaCreatedAt;", pDocumentFinanceMaster.Oid);
                    SQLSelectResultData xPSelectDataFinancePayment = XPOUtility.GetSelectedDataFromQuery(sqlFinancePayment);

                    /* IN009083 - has payments */
                    if (xPSelectDataFinancePayment.DataRows.Length > 0)
                    {
                        return false;
                    }
                }
            }
            return isCancellable;
        }

        private bool IsAlreadyReceivedByAT(Guid oid)
        {
            bool result = false;

            /* IN009083 - we are considering here that we do not cancel Sales Documents if already received by AT */
            string sql = string.Format("SELECT COUNT(*) FROM sys_systemauditat WHERE DocumentMaster = '{0}' AND ReturnCode = '0';", oid);
            var sqlResult = XPOSettings.Session.ExecuteScalar(sql);
            int countResult = Convert.ToUInt16(sqlResult);
            /* if document already received successfully by AT */
            if (countResult > 0)
            {
                result = true;
            }


            return result;
        }

        private bool TreeModelForEachTask_ActionGetFinanceDocumentsList(TreeModel model, TreePath path, TreeIter iter)
        {
            int columnIndexCheckBox = 1;
            int columnIndexGuid = 2;

            bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));
            Guid itemGuid = new Guid(model.GetValue(iter, columnIndexGuid).ToString());

            if (itemChecked)
            {
                //Add to FinanceMasterDocuments
                _listSelectFinanceMasterDocuments.Add(XPOUtility.GetEntityById<fin_documentfinancemaster>(itemGuid));
            }

            return false;
        }

        private void _touchButtonPosToolbarWorkSessionPeriods_Clicked(object sender, EventArgs e)
        {
            //Default ActionArea Buttons
            IconButtonWithText buttonPrintDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Print, "touchButtonPrintDocument_Green");
            IconButtonWithText buttonClose = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);
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

            PosSelectRecordDialog<XPCollection, Entity, TreeViewWorkSessionPeriod>
              dialogWorkSessionPeriods = new PosSelectRecordDialog<XPCollection, Entity, TreeViewWorkSessionPeriod>(
                this,
                DialogFlags.DestroyWithParent,
                GeneralUtils.GetResourceByName("window_title_select_worksession_period_day"),
                GlobalApp.MaxWindowSize,
                null, //XpoDefaultValue
                criteriaOperator,
                GridViewMode.CheckBox,
                actionAreaButtons
              );

            //CheckBox Capture CursorChanged/CheckBoxToggled Event, And enable/disable Buttons based on Valid Selection, Must be Here, Where we have a refence to Buttons
            dialogWorkSessionPeriods.CheckBoxToggled += delegate
            {
                //Use inside delegate to have accesss to local references, ex dialogPartialPayment, actionAreaButtonOk
                if (dialogWorkSessionPeriods.GenericTreeViewMode == GridViewMode.Default)
                {
                    //DataTableMode else use XPGuidObject
                    if (dialogWorkSessionPeriods.GenericTreeView.Entity != null) actionAreaButtonPrint.Button.Sensitive = true;
                }
                else if (dialogWorkSessionPeriods.GenericTreeViewMode == GridViewMode.CheckBox)
                {
                    actionAreaButtonPrint.Button.Sensitive = (dialogWorkSessionPeriods.GenericTreeView.MarkedCheckBoxs > 0);
                }
            };

            //Events
            dialogWorkSessionPeriods.Response += dialogWorkSessionPeriods_Response;

            //Call Dialog
            int response = dialogWorkSessionPeriods.Run();
            dialogWorkSessionPeriods.Destroy();
        }

        private void dialogWorkSessionPeriods_Response(object o, ResponseArgs args)
        {
            PosSelectRecordDialog<XPCollection, Entity, TreeViewWorkSessionPeriod>
              dialog = (PosSelectRecordDialog<XPCollection, Entity, TreeViewWorkSessionPeriod>)o;

            if (args.ResponseId != ResponseType.Close)
            {
                if (args.ResponseId == _responseTypePrint)
                {
                    //Single Record Mode - Default - USED HERE ONLY TO TEST Both Dialogs Modes (Default and CheckBox)
                    if (dialog.GenericTreeViewMode == GridViewMode.Default)
                    {
                        //use dialog.GenericTreeView.DataTableRow.ItemArray
                    }
                    //Multi Record Mode - CheckBox - ACTIVE MODE
                    else if (dialog.GenericTreeViewMode == GridViewMode.CheckBox)
                    {
                        //Required to use ListStoreModel and not ListStoreModelFilterSort, we only loop the visible filtered rows, and not The hidden Checked Rows
                        dialog.GenericTreeView.ListStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask_ActionPrintPeriod));
                    }
                }
                dialog.Run();
            }
        }

        private bool TreeModelForEachTask_ActionPrintPeriod(TreeModel model, TreePath path, TreeIter iter)
        {
            int columnIndexCheckBox = 1;
            int columnIndexGuid = 2;

            bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));
            Guid itemGuid = new Guid(model.GetValue(iter, columnIndexGuid).ToString());

            if (itemChecked)
            {
                pos_worksessionperiod workSessionPeriodParent = XPOUtility.GetEntityById<pos_worksessionperiod>(itemGuid);
                pos_worksessionperiod workSessionPeriodChild;
                //Print Parent Session : PrintWorkSessionMovement
                var workSessionParentDto = MappingUtils.GetPrintWorkSessionDto(workSessionPeriodParent);
                FrameworkCalls.PrintWorkSessionMovement(this, TerminalSettings.LoggedTerminal.ThermalPrinter, workSessionParentDto);

                //Get Child Sessions
                string sql = string.Format(@"SELECT Oid FROM pos_worksessionperiod WHERE Parent = '{0}' ORDER BY DateStart;", workSessionPeriodParent.Oid);
                SQLSelectResultData xPSelectData = XPOUtility.GetSelectedDataFromQuery(sql);
                foreach (DevExpress.Xpo.DB.SelectStatementResultRow row in xPSelectData.DataRows)
                {
                    //Print Child Sessions
                    workSessionPeriodChild = XPOUtility.GetEntityById<pos_worksessionperiod>(new Guid(row.Values[xPSelectData.GetFieldIndexFromName("Oid")].ToString()));
                    //PrintWorkSessionMovement
                    var workSessionChildDto = MappingUtils.GetPrintWorkSessionDto(workSessionPeriodChild);
                    FrameworkCalls.PrintWorkSessionMovement(this, TerminalSettings.LoggedTerminal.ThermalPrinter, workSessionChildDto);
                }
            }

            return false;
        }

        private void UnCheckAll_FinanceMasterDocuments(PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinanceMaster> pDialog, bool pRefreshTree)
        {
            //UnCheck all Marked CheckBoxs
            pDialog.GenericTreeView.UnCheckAll();
            //Refresh Tree
            if (pRefreshTree) pDialog.GenericTreeView.Refresh();
            //Restore Title, without Totals
            pDialog.WindowSettings.WindowTitle.Text = _selectRecordWindowTitle;
            //Reset Totals
            TotalDialogFinanceMasterDocuments = 0.0m;

            //Dont Disable First and Last button (New and Close)
            for (int i = 2; i < pDialog.ActionAreaButtons.Count - 1; i++)
            {
                //_logger.Debug(string.Format("index[{0}], Label[{1}]", i, pDialog.ActionAreaButtons[i].Button.Name));
                pDialog.ActionAreaButtons[i].Button.Sensitive = false;
            }
        }

        private void _toolbarFinanceDocumentsPayments_Clicked(object sender, EventArgs e)
        {
            bool validMarkedDocumentTypesForSendEmailSensitive = false;
            bool itemChecked = false;
            fin_documentfinancepayment documentFinancePayment;
            // IN009223 IN009227
            //string fileActionMore = SharedUtils.OSSlash(GeneralSettings.Path["images"] + @"Icons\icon_pos_more.png");
            //string fileActionFilter = SharedUtils.OSSlash(GeneralSettings.Path["images"] + @"Icons\icon_pos_filter.png");

            //Default ActionArea Buttons
            // IN009223 IN009227
            //TouchButtonIconWithText buttonMore = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.More, "touchButtonMore_Grey", string.Format(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_button_label_more, SettingsApp.PaginationRowsPerPage), fileActionMore);
            //TouchButtonIconWithText buttonFilter = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Filter, "touchButtonFilter_Green", "Filter", fileActionFilter);
            IconButtonWithText buttonClose = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);
            IconButtonWithText buttonPrintDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Print, "touchButtonPrintDocument_Green");
            IconButtonWithText buttonPrintDocumentAs = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.PrintAs, "touchButtonPrintDocumentAs_Green");
            IconButtonWithText buttonCancelDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButtonCancelDocument_Green", GeneralUtils.GetResourceByName("global_button_label_cancel_document"), IconSettings.ActionCancel);
            IconButtonWithText buttonOpenDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument, "touchButtonOpenDocument_Green");
            IconButtonWithText buttonSendEmailDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.SendEmailDocument, "touchButtonSendEmailDocument_Green");
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
            _filterChoice = ReportsQueryDialogMode.FILTER_PAYMENT_DOCUMENTS;

            //Define Criteria
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            CriteriaOperatorBase = criteriaOperator;// IN009223 IN009227

            var countResult = XPOSettings.Session.Evaluate(typeof(fin_documentfinancepayment), CriteriaOperator.Parse("Count()"), CriteriaOperatorBase);
            string sqlCountTopResults = "0";
            string sqlCountResultTopResults = "0";

            if (DatabaseSettings.DatabaseType.ToString() == "MySql" || DatabaseSettings.DatabaseType.ToString() == "SQLite")
            {
                string filterCriteriaOperatorBase = CriteriaOperatorBase.ToString().Replace("[", "").Replace("]", "");
                sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT * FROM fin_documentfinancepayment WHERE {1}) AS Total LIMIT {0};", POSSettings.PaginationRowsPerPage, filterCriteriaOperatorBase);
                sqlCountResultTopResults = XPOSettings.Session.ExecuteScalar(sqlCountTopResults).ToString();
            }
            else if (DatabaseSettings.DatabaseType.ToString() == "MSSqlServer")
            {
                sqlCountTopResults = string.Format("SELECT COUNT(*) FROM (SELECT TOP {0} * FROM fin_documentfinancepayment WHERE {1}) AS Total;", POSSettings.PaginationRowsPerPage, CriteriaOperatorBase);
                sqlCountResultTopResults = XPOSettings.Session.ExecuteScalar(sqlCountTopResults).ToString();
            }


            windowTitleDefault = GeneralUtils.GetResourceByName("window_title_dialog_document_finance_payment");
            //NOT IN USE string nDocs = _dialogDocumentFinanceMaster.GenericTreeView.DataSource.Count.ToString();
            string showResults = string.Format(GeneralUtils.GetResourceByName("window_title_show_results"), sqlCountResultTopResults, countResult);
            _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);

            PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinancePayment>
              dialogPayments = new PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinancePayment>(
                this,
                DialogFlags.DestroyWithParent,
                _selectRecordWindowTitle,
                GlobalApp.MaxWindowSize,
                null, //XpoDefaultValue
                criteriaOperator,
                GridViewMode.CheckBox,
                actionAreaButtons
              );


            //CheckBox Capture CursorChanged/CheckBoxToggled Event, And enable/disable Buttons based on Valid Selection, Must be Here, Where we have a refence to Buttons
            dialogPayments.CheckBoxToggled += delegate
            {
                //Use inside delegate to have accesss to local references, ex dialogPartialPayment, actionAreaButtonOk
                if (dialogPayments.GenericTreeViewMode == GridViewMode.Default)
                {
                    //DataTableMode else use XPGuidObject
                    if (dialogPayments.GenericTreeView.Entity != null)
                    {
                        _actionAreaButtonPrintPayment.Button.Sensitive = true;
                        _actionAreaButtonPrintPaymentAs.Button.Sensitive = true;
                        _actionAreaButtonCancelPayment.Button.Sensitive = true;
                        _actionAreaButtonOpenDocument.Button.Sensitive = true;
                        _actionAreaButtonFilter.Button.Sensitive = true;
                    }
                }
                else if (dialogPayments.GenericTreeViewMode == GridViewMode.CheckBox)
                {

                    //Get value from Model, its Outside XPGuidObject Scope
                    itemChecked = (bool)dialogPayments.GenericTreeView.GetCurrentModelCheckBoxValue();
                    documentFinancePayment = (fin_documentfinancepayment)dialogPayments.GenericTreeView.Entity;
                    // Add/Remove MarkedFinanceMasterDocuments on click/mark Document
                    if (itemChecked)
                    {
                        _listMarkedFinancePaymentDocuments.Add(documentFinancePayment);
                        //_logger.Debug(string.Format("_listMarkedFinancePaymentDocuments count: [{0}], Added: [{1}]", _listMarkedFinancePaymentDocuments.Count, documentFinancePayment.PaymentRefNo));
                    }
                    else
                    {
                        _listMarkedFinancePaymentDocuments.Remove(documentFinancePayment);
                        //_logger.Debug(string.Format("_listMarkedFinancePaymentDocuments count: [{0}], Removed: [{1}]", _listMarkedFinancePaymentDocuments.Count, documentFinancePayment.PaymentRefNo));
                    }
                    // Get Sensitive for SendEmail
                    validMarkedDocumentTypesForSendEmailSensitive = GetSensitiveForSendEmailDocuments(_listMarkedFinancePaymentDocuments);

                    if (dialogPayments.GenericTreeView.MarkedCheckBoxs > 0)
                    {
                        _actionAreaButtonPrintPayment.Button.Sensitive = true;
                        _actionAreaButtonPrintPaymentAs.Button.Sensitive = true;
                        _actionAreaButtonCancelPayment.Button.Sensitive = (permissionFinanceDocumentCancelDocument);
                        _actionAreaButtonOpenDocument.Button.Sensitive = (permissionFinanceDocumentCancelDocument);
                        _actionAreaButtonSendEmailDocument.Button.Sensitive = (validMarkedDocumentTypesForSendEmailSensitive);
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
            PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinancePayment>
              dialog = (PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinancePayment>)o;

            fin_documentfinancepayment documentFinancePayment = (fin_documentfinancepayment)dialog.GenericTreeView.Entity;

            if (args.ResponseId != ResponseType.Close)
            {
                if (
                        (args.ResponseId == _responseTypePrint && !logicpos.Utils.ShowMessageTouchRequiredValidPrinter(_dialogFinanceDocumentsResponse, _printerChoosed))
                        || args.ResponseId == _responseTypePrintAs
                    )
                {
                    //Assign Choosed Printer based on user ResponseType
                    _printerChoosed = (args.ResponseId == _responseTypePrint && TerminalSettings.LoggedTerminal.Printer != null) ? TerminalSettings.LoggedTerminal.Printer : TerminalSettings.LoggedTerminal.ThermalPrinter;

                    //Single Record Mode - Default - USED HERE ONLY TO TEST Both Dialogs Modes (Default and CheckBox)
                    if (dialog.GenericTreeViewMode == GridViewMode.Default)
                    {
                        var printerDto = MappingUtils.GetPrinterDto(_printerChoosed);
                        FrameworkCalls.PrintFinanceDocumentPayment(this, printerDto, documentFinancePayment);
                    }
                    //Multi Record Mode - CheckBox - ACTIVE MODE
                    else if (dialog.GenericTreeViewMode == GridViewMode.CheckBox)
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
                    if (dialog.GenericTreeViewMode == GridViewMode.Default)
                    {
                        _listSelectFinancePaymentDocuments.Add(documentFinancePayment);
                    }
                    //Multi Record Mode - CheckBox - ACTIVE MODE
                    else if (dialog.GenericTreeViewMode == GridViewMode.CheckBox)
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
                        dialog.GenericTreeView.Entities.TopReturnedObjects = (POSSettings.PaginationRowsPerPage * dialog.GenericTreeView.CurrentPageNumber);
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

                        var countResult = XPOSettings.Session.Evaluate(typeof(fin_documentfinancepayment), CriteriaOperator.Parse("Count()"), criteriaCount);
                        string nDocs = dialog.GenericTreeView.Entities.Count.ToString();
                        string showResults = string.Format(GeneralUtils.GetResourceByName("window_title_show_results"), nDocs, countResult);

                        _selectRecordWindowTitle = string.Format("{0} :: {1}", windowTitleDefault, showResults);
                        dialog.WindowSettings.WindowTitle.Text = _selectRecordWindowTitle;
                    }
                    else if (args.ResponseId == (ResponseType)DialogResponseType.Filter)
                    {
                        //Reset current page to 1 ( Pagination go to defined initialy )
                        dialog.GenericTreeView.CurrentPageNumber = 1;

                        PosReportsQueryDialog dialogPayedDocuments = new PosReportsQueryDialog(dialog, DialogFlags.Modal, ReportsQueryDialogMode.FILTER_PAYMENT_DOCUMENTS, typeof(fin_documentfinancepayment).Name, GeneralUtils.GetResourceByName("window_title_dialog_report_filter"));
                        DialogResponseType response = (DialogResponseType)dialogPayedDocuments.Run();
                        List<string> result = new List<string>();

                        // Filter SellDocuments
                        string filterField = string.Empty;
                        string statusField = string.Empty;
                        string extraFilter = string.Empty;

                        if (DialogResponseType.CleanFilter.Equals(response))
                        {
                            dialog.GenericTreeView.Entities.Criteria = CriteriaOperatorBase;
                            dialog.GenericTreeView.Entities.TopReturnedObjects = POSSettings.PaginationRowsPerPage * dialog.GenericTreeView.CurrentPageNumber;

                            dialog.GenericTreeView.Refresh();
                            criteriaOperatorFilter = null;

                            var countResult = XPOSettings.Session.Evaluate(typeof(fin_documentfinancepayment), CriteriaOperator.Parse("Count()"), CriteriaOperatorBase);
                            string nDocs = dialog.GenericTreeView.Entities.Count.ToString();
                            string showResults = string.Format(GeneralUtils.GetResourceByName("window_title_show_results"), nDocs, countResult);
                            //Finish Updating Title
                            dialog.WindowSettings.WindowTitle.Text = string.Format("{0} :: {1} ", windowTitleDefault, showResults);
                        }
                        else if (DialogResponseType.Ok.Equals(response))
                        {
                            filterField = "DocumentType";
                            statusField = "DocumentStatusStatus";

                            /* IN009066 - FS and NC added to reports */
                            extraFilter = $@" AND ({statusField} <> 'A') AND (
			                    {filterField} = '{InvoiceSettings.InvoiceId}' OR 
			                    {filterField} = '{DocumentSettings.SimplifiedInvoiceId}' OR 
			                    {filterField} = '{DocumentSettings.InvoiceAndPaymentId}' OR 
			                    {filterField} = '{DocumentSettings.ConsignationInvoiceId}' OR 
			                    {filterField} = '{DocumentSettings.DebitNoteId}' OR 
			                    {filterField} = '{CustomDocumentSettings.CreditNoteId}' OR 
			                    {filterField} = '{DocumentSettings.PaymentDocumentTypeId}' 
			                    OR 
			                    {filterField} = '{DocumentSettings.CurrentAccountInputId}'
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
                            dialog.GenericTreeView.Entities.Criteria = criteriaOperator;
                            dialog.GenericTreeView.Entities.TopReturnedObjects = POSSettings.PaginationRowsPerPage * dialog.GenericTreeView.CurrentPageNumber;

                            criteriaOperatorFilter = criteriaOperator;
                            dialog.GenericTreeView.Refresh();

                            var countResult = XPOSettings.Session.Evaluate(typeof(fin_documentfinancepayment), CriteriaOperator.Parse("Count()"), criteriaOperatorFilter);
                            string nDocs = dialog.GenericTreeView.Entities.Count.ToString();
                            string showResults = string.Format(GeneralUtils.GetResourceByName("window_title_show_results"), nDocs, countResult);
                            //Finish Updating Title
                            dialog.WindowSettings.WindowTitle.Text = $"{windowTitleDefault} :: {showResults} ";
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

        private bool TreeModelForEachTask_ActionGetPaymentDocumentsList(TreeModel model, TreePath path, TreeIter iter)
        {
            int columnIndexCheckBox = 1;
            int columnIndexGuid = 2;

            bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));
            Guid itemGuid = new Guid(model.GetValue(iter, columnIndexGuid).ToString());

            if (itemChecked)
            {
                //Add to FinancePaymentDocuments
                _listSelectFinancePaymentDocuments.Add(XPOUtility.GetEntityById<fin_documentfinancepayment>(itemGuid));
            }

            return false;
        }

        private void UnCheckAll_FinancePaymentDocuments(PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentFinancePayment> pDialog, bool pRefreshTree)
        {
            //UnCheck all Marked CheckBoxs
            pDialog.GenericTreeView.UnCheckAll();
            //Refresh Tree
            if (pRefreshTree) pDialog.GenericTreeView.Refresh();

            //Dont Disable First and Last button (New and Close)
            for (int i = 1; i < pDialog.ActionAreaButtons.Count - 1; i++)
            {
                //_logger.Debug(string.Format("index[{0}], Label[{1}]", i, pDialog.ActionAreaButtons[i].Button.Name));
                pDialog.ActionAreaButtons[i].Button.Sensitive = false;
            }
        }

        private bool TreeModelForEachTask_ActionPrintPayments(TreeModel model, TreePath path, TreeIter iter)
        {
            int columnIndexCheckBox = 1;
            int columnIndexGuid = 2;

            bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));
            Guid itemGuid = new Guid(model.GetValue(iter, columnIndexGuid).ToString());

            if (itemChecked)
            {
                fin_documentfinancepayment documentFinancePayment = XPOUtility.GetEntityById<fin_documentfinancepayment>(itemGuid);
                var printerDto = MappingUtils.GetPrinterDto(_printerChoosed);
                FrameworkCalls.PrintFinanceDocumentPayment(this, printerDto, documentFinancePayment);
            }

            return false;
        }

        private void CallCancelFinancePaymentDocumentsDialog(Window pDialog, List<fin_documentfinancepayment> pListSelectDocuments)
        {
            //SAF-T Notes
            //4.4: Documentos de recibos emitidos
            //  4.4.4.8.1. * Estado atual do recibo (PaymentStatus) : “A” — Recibo anulado

            logicpos.Utils.ResponseText dialogResponse;
            DateTime currentDateTime;
            List<string> ignoredDocuments = new List<string>();

            foreach (var document in pListSelectDocuments)
            {
                if (CanCancelFinancePaymentDocument(document))
                {
                    string fileWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_input_text_default.png";

                    dialogResponse = logicpos.Utils.GetInputText(pDialog, DialogFlags.Modal, fileWindowIcon, string.Format(GeneralUtils.GetResourceByName("global_cancel_document_input_text_label"), document.PaymentRefNo), string.Empty, RegexUtils.RegexAlfaNumericExtendedForMotive, true);
                    if (dialogResponse.ResponseType == ResponseType.Ok)
                    {
                        //_logger.Debug(string.Format("PaymentRefNo:[{0}], DocumentStatusStatus:[{1}], reason:[{2}]", document.PaymentRefNo, document.PaymentStatus, dialogResponse.InputText));
                        currentDateTime = XPOUtility.CurrentDateTimeAtomic();
                        document.PaymentStatus = "A";
                        document.PaymentStatusDate = currentDateTime.ToString(CultureSettings.DateTimeFormatCombinedDateTime);
                        document.Reason = dialogResponse.Text;
                        document.SourceID = XPOSettings.LoggedUser.CodeInternal;
                        document.Save();

                        //Audit
                        XPOUtility.Audit("FINANCE_DOCUMENT_CANCELLED", string.Format("{0} {1}: {2}", document.DocumentType.Designation, GeneralUtils.GetResourceByName("global_document_cancelled"), document.PaymentRefNo));

                        //Removed Payed BIT to all DocumentPayment Documents (FT and NC)
                        foreach (fin_documentfinancemasterpayment documentPayment in document.DocumentPayment)
                        {
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

        private bool CanCancelFinancePaymentDocument(fin_documentfinancepayment pDocumentFinancePayment)
        {
            bool result = false;

            if (pDocumentFinancePayment.PaymentStatus != "A")
            {
                result = true;
            }

            return result;
        }

        private void OpenFinancePaymentDocuments(Window pDialog, List<fin_documentfinancepayment> pListSelectDocuments)
        {
            List<string> documents = new List<string>();

            foreach (fin_documentfinancepayment document in pListSelectDocuments)
            {
                if (LicenseSettings.LicenceRegistered == false)
                {
                    logicpos.Utils.ShowMessageBoxUnlicensedError(this, GeneralUtils.GetResourceByName("global_printing_function_disabled"));
                }
                else documents.Add(LogicPOS.Reporting.Common.FastReport.GenerateDocumentFinancePaymentPDFIfNotExists(document));
            }

            foreach (var item in documents)
            {
                if (LicenseSettings.LicenceRegistered == false)
                {
                    logicpos.Utils.ShowMessageBoxUnlicensedError(this, GeneralUtils.GetResourceByName("global_printing_function_disabled"));
                }
                else if (File.Exists(item))
                {
                    if (logicpos.Utils.UsePosPDFViewer() == true)
                    {
                        string docPath = string.Format(@"{0}\{1}", Environment.CurrentDirectory, item);
                        var ScreenSizePDF = GlobalApp.ScreenSize;
                        int widthPDF = ScreenSizePDF.Width;
                        int heightPDF = ScreenSizePDF.Height;
                        System.Windows.Forms.Application.Run(new LogicPOS.PDFViewer.Winforms.PDFViewer(docPath, widthPDF - 50, heightPDF - 25, false));
                    }
                    else
                    {
                        System.Diagnostics.Process.Start(string.Format(@"{0}\{1}", Environment.CurrentDirectory, item));
                    }
                }
            }

        }

        private void ShowIgnoredDocuments(Window parentWindow, List<string> pIgnoredDocuments)
        {
            //Show Ignored Documents
            string ignoredDocumentsMessage = string.Empty;
            if (pIgnoredDocuments.Count > 0)
            {
                //Sort Documents List
                pIgnoredDocuments.Sort();

                for (int i = 0; i < pIgnoredDocuments.Count; i++)
                {
                    ignoredDocumentsMessage += $"{Environment.NewLine}{pIgnoredDocuments[i]}";
                }

                string infoMessage = string.Format(GeneralUtils.GetResourceByName("app_info_show_ignored_cancelled_documents"), ignoredDocumentsMessage);
                logicpos.Utils.ShowMessageBox(parentWindow, DialogFlags.Modal, new Size(600, 400), MessageType.Info, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_information"), infoMessage);
            }
        }

        private void _touchButtonPosToolbarMerchandiseEntry_Clicked(object sender, EventArgs e)
        {
            ProcessArticleStockParameter response = PosArticleStockDialog.GetProcessArticleStockParameter(this);

            if (response != null)
            {
                if (response.ArticleCollectionSimple.Count > 0)
                {
                    foreach (var item in response.ArticleCollectionSimple)
                    {
                        response.Quantity = item.Value;
                        response.Article = item.Key;
                        ProcessArticleStock.Add(ProcessArticleStockMode.In, response);
                    }
                }
            }
        }

        private bool GetSensitiveForCloneDocuments(List<fin_documentfinancemaster> documents)
        {
            bool result = true;
            // Valid DocumentTypes
            Guid[] validDocumentTypes = new Guid[] {
                InvoiceSettings.InvoiceId,
                DocumentSettings.SimplifiedInvoiceId,
                DocumentSettings.InvoiceAndPaymentId,
                DocumentSettings.XpoOidDocumentFinanceTypeBudget,
                DocumentSettings.XpoOidDocumentFinanceTypeProformaInvoice
            };

            foreach (fin_documentfinancemaster item in documents)
            {
                // Check if is an invalid DocumentType (Outside of validDocumentTypes Array)
                if (Array.IndexOf(validDocumentTypes, item.DocumentType.Oid) < 0)
                {
                    // Detected invalid DocumentType, return false
                    return false;
                }
            }


            return result;
        }

        private bool GetSensitiveForSendEmailDocuments(List<fin_documentfinancemaster> documents)
        {
            bool result = true;

            ArrayList customerList = new ArrayList();

            foreach (fin_documentfinancemaster item in documents)
            {
                if (!customerList.Contains(item.EntityOid))
                {
                    customerList.Add(item.EntityOid);
                }
            }

            result = customerList.Count <= 1;

            return result;
        }

        private bool GetSensitiveForSendEmailDocuments(List<fin_documentfinancepayment> documents)
        {
            bool result = true;

            ArrayList customerList = new ArrayList();

            foreach (fin_documentfinancepayment item in documents)
            {
                if (!customerList.Contains(item.EntityOid))
                {
                    customerList.Add(item.EntityOid);
                }
            }

            result = customerList.Count <= 1;

            return result;
        }
    }
}
