using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.GenericTreeView;
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

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosDocumentFinanceSelectRecordDialog
    {
        //Reference to selecte Printer (Print | PrintAs Response)
        private SYS_ConfigurationPrinters _printerChoosed;
        //Store Reference to Generic Printer
        private SYS_ConfigurationPrinters _printerGeneric;
        //ResponseType (Above 10)
        //DialogFinanceMaster ResponseTypes
        private ResponseType _responseTypePrint = (ResponseType)11;
        private ResponseType _responseTypePrintAs = (ResponseType)12;
        private ResponseType _responseTypePayCurrentAcountsDocument = (ResponseType)13;
        private ResponseType _responseTypeNewDocument = (ResponseType)14;
        private ResponseType _responseTypePayInvoice = (ResponseType)15;
        private ResponseType _responseTypeCancelDocument = (ResponseType)16;
        private ResponseType _responseTypeOpenDocument = (ResponseType)17;
        private ResponseType _responseTypeCloneDocument = (ResponseType)18;
        private ResponseType _responseTypeSendEmailDocument = (ResponseType)19;
        //Store list of Master and PaymentDocuments, created in TreeModelForEachTask_ActionGetFinanceDocumentsList/ActionGetPaymentDocumentsList
        private List<FIN_DocumentFinanceMaster> _listSelectFinanceMasterDocuments;
        private List<FIN_DocumentFinancePayment> _listSelectFinancePaymentDocuments;
        private List<FIN_DocumentFinanceMaster> _listMarkedFinanceMasterDocuments = new List<FIN_DocumentFinanceMaster>();
        private List<FIN_DocumentFinancePayment> _listMarkedFinancePaymentDocuments = new List<FIN_DocumentFinancePayment>();
        private string _selectRecordWindowTitle;
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
            string fileActionClose = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_close.png");
            string fileActionPrint = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_print.png");
            string fileActionNewDocument = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_toolbar_finance_new_document.png");
            string fileActionPayInvoice = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_payment_full.png");
            string fileActionCancel = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_cancel_document.png");
            bool generatePdfDocuments = Convert.ToBoolean(GlobalFramework.Settings["generatePdfDocuments"]);

            //Default/Shared ActionArea Buttons
            TouchButtonIconWithText buttonClose = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Close);
            TouchButtonIconWithText buttonPrintDocument = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Print, "touchButtonPrintDocument_Green");
            TouchButtonIconWithText buttonPrintDocumentAs = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.PrintAs, "touchButtonPrintDocumentAs_Green");
            TouchButtonIconWithText buttonCancelDocument = ActionAreaButton.FactoryGetDialogButtonType("touchButtonCancelDocument_Green", Resx.global_button_label_cancel_document, _fileActionCancel);
            TouchButtonIconWithText buttonOpenDocument = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.OpenDocument, "touchButtonOpenDocument_Green");
            TouchButtonIconWithText buttonSendEmailDocument = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.SendEmailDocument, "touchButtonSendEmailDocument_Green");
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

            //SHARED for All Modes : Add to Criteria Operator
            string criteriaOperatorShared = "(Disabled IS NULL OR Disabled  <> 1) AND ";

            switch (_dialogFinanceMasterCallerButton.Token)
            {
                //All
                case "ALL":
                    _selectRecordWindowTitle = Resx.window_title_select_finance_document;
                    criteriaOperator = CriteriaOperator.Parse(string.Format("{0} DocumentType <> '{1}'", criteriaOperatorShared, SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput));
                    //Add aditional Buttons to ActionArea
                    TouchButtonIconWithText buttonNewDocument = ActionAreaButton.FactoryGetDialogButtonType("touchButtonNewDocument_Green", Resx.global_button_label_new_financial_document, fileActionNewDocument);
                    _actionAreaButtonNewDocument = new ActionAreaButton(buttonNewDocument, _responseTypeNewDocument);
                    TouchButtonIconWithText buttonCloneDocument = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.CloneDocument, "touchButtonCloneDocument_Green");
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
                    break;
                // Payments
                case "FT_UNPAYED":
                    _selectRecordWindowTitle = Resx.window_title_select_finance_document_ft_unpaid;
                    criteriaOperator = CriteriaOperator.Parse(string.Format("{0} (DocumentType = '{1}' OR DocumentType = '{2}' OR DocumentType = '{3}') AND Payed = 0 AND DocumentStatusStatus <> 'A'", criteriaOperatorShared, SettingsApp.XpoOidDocumentFinanceTypeInvoice, SettingsApp.XpoOidDocumentFinanceTypeCreditNote, SettingsApp.XpoOidDocumentFinanceTypeDebitNote));
                    //Add aditional Buttons to ActionArea
                    TouchButtonIconWithText buttonPayInvoice = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPayInvoice_Green", Resx.global_button_label_pay_invoice, fileActionPayInvoice);
                    buttonPayInvoice.Sensitive = false;
                    _actionAreaButtonPayInvoice = new ActionAreaButton(buttonPayInvoice, _responseTypePayInvoice);
                    actionAreaButtons.Add(_actionAreaButtonPayInvoice);
                    //Add Shared Buttons
                    //_actionAreaButtonCancelDocument = new ActionAreaButton(buttonCancelDocument, _responseTypeCancelDocument);
                    actionAreaButtons.Add(_actionAreaButtonCancelDocument);
                    break;
                // CurrentAccount
                case "CC":
                    _selectRecordWindowTitle = Resx.window_title_select_finance_document_cc;
                    criteriaOperator = CriteriaOperator.Parse(string.Format("{0} DocumentType = '{1}' AND Payed = 0", criteriaOperatorShared, SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput));
                    //Add aditional Buttons to ActionArea
                    TouchButtonIconWithText buttonPayCurrentAcountDocument = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPayCurrentAcountDocument_Green", Resx.global_button_label_pay, fileActionPayInvoice);
                    buttonPayCurrentAcountDocument.Sensitive = false;
                    _actionAreaButtonPayCurrentAcountsDocument = new ActionAreaButton(buttonPayCurrentAcountDocument, _responseTypePayCurrentAcountsDocument);
                    actionAreaButtons.Add(_actionAreaButtonPayCurrentAcountsDocument);
                    //Add Shared Buttons
                    //_actionAreaButtonCancelDocument = new ActionAreaButton(buttonCancelDocument, _responseTypeCancelDocument);
                    actionAreaButtons.Add(_actionAreaButtonCancelDocument);
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
            FIN_DocumentFinanceMaster documentFinanceMaster = null;
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
                        documentFinanceMaster = (FIN_DocumentFinanceMaster)_dialogDocumentFinanceMaster.GenericTreeView.DataSourceRow;
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
                        documentFinanceMaster = (FIN_DocumentFinanceMaster)_dialogDocumentFinanceMaster.GenericTreeView.DataSourceRow;

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

                        //Get documentValue from Document.TotalFinal to Increment/Decrement base on Credit Bool, ex Credit=True Value=100 else if Credit=False Value=-100
                        if (documentFinanceMaster.DocumentType.Credit)
                        {
                            //This Query Exists 3 Locations, Find it and change in all Locations - Required "GROUP BY fmaOid,fmaTotalFinal" to work with SQLServer
                            string sql = string.Format("SELECT fmaTotalFinal - SUM(fmpCreditAmount) as Result FROM view_documentfinancepayment WHERE fmaOid = '{0}' AND fpaPaymentStatus <> 'A' GROUP BY fmaOid,fmaTotalFinal;", documentFinanceMaster.Oid);
                            documentDebit = Convert.ToDecimal(GlobalFramework.SessionXpo.ExecuteScalar(sql));
                            documentValue = (documentDebit != 0) ? documentDebit : documentFinanceMaster.TotalFinal;
                        }
                        else
                        {
                            documentValue = -documentFinanceMaster.TotalFinal;
                        };

                        _totalDialogFinanceMasterDocuments += (itemChecked) ? documentValue : -documentValue;

                        //Enable/Disable Buttons for all Modes
                        _actionAreaButtonPrintDocument.Button.Sensitive = true;
                        _actionAreaButtonPrintDocumentAs.Button.Sensitive = true;
                        _actionAreaButtonOpenDocument.Button.Sensitive = true;
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
                        _actionAreaButtonCloneDocument.Button.Sensitive = false;
                        if (_actionAreaButtonPayCurrentAcountsDocument != null) _actionAreaButtonPayCurrentAcountsDocument.Button.Sensitive = false;
                        if (_actionAreaButtonPayInvoice != null) _actionAreaButtonPayInvoice.Button.Sensitive = false;
                        if (_actionAreaButtonCancelDocument != null) _actionAreaButtonCancelDocument.Button.Sensitive = false;
                        // Require to Empty listMarkedFinanceMasterDocuments (MarkedCheckBoxs > 0)
                        _listMarkedFinanceMasterDocuments.Clear();
                    }
                }
                //Finish Updating Title
                _dialogDocumentFinanceMaster.WindowTitle = (_totalDialogFinanceMasterDocuments != 0) ? string.Format("{0} : {1}", _selectRecordWindowTitle, FrameworkUtils.DecimalToStringCurrency(_totalDialogFinanceMasterDocuments)) : _selectRecordWindowTitle;
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

            FIN_DocumentFinanceMaster documentFinanceMaster = (FIN_DocumentFinanceMaster)_dialogFinanceDocumentsResponse.GenericTreeView.DataSourceRow;

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
                    _listSelectFinanceMasterDocuments = new List<FIN_DocumentFinanceMaster>();

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
                        foreach (FIN_DocumentFinanceMaster item in _listSelectFinanceMasterDocuments)
                        {
                            i++;
                            documentList += string.Format("- {0}{1}", item.DocumentNumber, (i < _listSelectFinanceMasterDocuments.Count) ? Environment.NewLine : string.Empty);
                        }

                        // Call Dialog
                        ResponseType dialogResponse = Utils.ShowMessageTouch(_dialogFinanceDocumentsResponse, DialogFlags.Modal, new Size(700, 440), MessageType.Question, ButtonsType.OkCancel, Resx.global_question,
                            string.Format(Resx.window_title_dialog_clone_documents_confirmation, documentList)
                        );

                        if (dialogResponse == ResponseType.Ok)
                        {
                            //Start Open Documents
                            CallCloneFinanceMasterDocuments(_dialogFinanceDocumentsResponse, _listSelectFinanceMasterDocuments);

                            //UnCheck all Marked CheckBoxs, After call CallCancelDocumentsDialog
                            UnCheckAll_FinanceMasterDocuments(_dialogFinanceDocumentsResponse, true);
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
                }
                _dialogFinanceDocumentsResponse.Run();
            }
            else
            {
                //Reset listMarkedFinanceMasterDocuments
                _listMarkedFinanceMasterDocuments.Clear();
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
                    FIN_DocumentFinanceMaster documentFinanceMaster = (FIN_DocumentFinanceMaster)FrameworkUtils.GetXPGuidObject(typeof(FIN_DocumentFinanceMaster), itemGuid);
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

        private FIN_DocumentFinanceMaster PayCurrentAcountDocuments(
          PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> pSourceWindow,
          List<FIN_DocumentFinanceMaster> pFinanceDocuments
        )
        {
            //Local Vars
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> parentDialog = (PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster>)pSourceWindow;
            FIN_DocumentFinanceMaster resultDocument = null;

            try
            {
                //Init Global ArticleBag
                ArticleBag articleBag = new ArticleBag();
                ArticleBagKey articleBagKey;
                ArticleBagProperties articleBagProps;

                foreach (FIN_DocumentFinanceMaster document in pFinanceDocuments)
                {
                    foreach (FIN_DocumentFinanceDetail detail in document.DocumentDetail)
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

                PosPaymentsDialog dialog = new PosPaymentsDialog(pSourceWindow, DialogFlags.DestroyWithParent, articleBag, false, false);

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
                    parentDialog.WindowTitle = _selectRecordWindowTitle;// Resx.window_title_select_finance_document_cc;
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

        private FIN_DocumentFinancePayment CallPayInvoicesDialog(
          PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> pSourceWindow,
          decimal pPaymentAmount
        )
        {
            //Initialize local Vars
            FIN_DocumentFinancePayment resultDocumentFinancePayment = null;
            int noOfInvoices = 0;

            //Count NoOfInvoices
            foreach (FIN_DocumentFinanceMaster document in _listSelectFinanceMasterDocuments)
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
        private FIN_DocumentFinancePayment PayInvoices(
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> pSourceWindow,
            List<FIN_DocumentFinanceMaster> pFinanceDocuments,
            Guid pCustomer,
            Guid pPaymentMethod,
            Guid pConfigurationCurrency,
            decimal pPaymentAmount,
            string pPaymentNotes = ""
        )
        {
            //Local Vars
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> parentDialog = (PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster>)pSourceWindow;
            FIN_DocumentFinancePayment resultDocument = null;
            List<FIN_DocumentFinanceMaster> listInvoices = new List<FIN_DocumentFinanceMaster>();
            List<FIN_DocumentFinanceMaster> listCreditNotes = new List<FIN_DocumentFinanceMaster>();

            try
            {
                foreach (FIN_DocumentFinanceMaster document in pFinanceDocuments)
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
            List<FIN_DocumentFinanceMaster> pFinanceDocuments
        )
        {
            List<string> documents = new List<string>();

            try
            {
                // Call GenerateDocument and add it to List
                foreach (FIN_DocumentFinanceMaster document in pFinanceDocuments)
                {
                    documents.Add(ProcessFinanceDocument.GenerateDocumentFinanceMasterPDFIfNotExists(document));
                }

                foreach (var item in documents)
                {
                    if (File.Exists(item))
                    {
                        // Open file with System.Diagnostics.Process.Start
                        System.Diagnostics.Process.Start(FrameworkUtils.OSSlash(string.Format(@"{0}\{1}", Environment.CurrentDirectory, item)));
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
            List<FIN_DocumentFinanceMaster> pDocuments
        )
        {
            Dictionary<FIN_DocumentFinanceMaster, string> documents = new Dictionary<FIN_DocumentFinanceMaster, string>();
            List<string> attachmentFileNames = new List<string>();
            // Get Customer from first Document
            ERP_Customer customer = (ERP_Customer)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(ERP_Customer), pDocuments[0].EntityOid);
            string customerEmail = (customer.Email != null) ? customer.Email : string.Empty;
            string subject = string.Empty;
            string mailBody = string.Empty;
            string documentList = string.Empty;

            try
            {
                // Call GenerateDocument and add it to List
                foreach (FIN_DocumentFinanceMaster document in pDocuments)
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
                    Resx.window_title_send_email,
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
        /// SAME has Above Version only changed type FIN_DocumentFinanceMaster with FIN_DocumentFinancePayment
        /// </summary>
        /// <param name="Documents"></param>
        private void CallSendEmailFinancePaymentDocuments(
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinancePayment> pSourceWindow,
            List<FIN_DocumentFinancePayment> pDocuments
        )
        {
            Dictionary<FIN_DocumentFinancePayment, string> documents = new Dictionary<FIN_DocumentFinancePayment, string>();
            List<string> attachmentFileNames = new List<string>();
            // Get Customer from first Document
            ERP_Customer customer = (ERP_Customer)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(ERP_Customer), pDocuments[0].EntityOid);
            string customerEmail = (customer.Email != null) ? customer.Email : string.Empty;
            string mailBody = string.Empty;

            try
            {
                // Call GenerateDocument and add it to List
                foreach (FIN_DocumentFinancePayment document in pDocuments)
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
                    Resx.window_title_send_email,
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
            List<FIN_DocumentFinanceMaster> pFinanceDocuments
        )
        {
            //Local Vars
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> parentDialog = (PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster>)pSourceWindow;
            List<FIN_DocumentFinanceMaster> resultDocument = new List<FIN_DocumentFinanceMaster>();

            SortingCollection articleDetailSortingCollection = new SortingCollection();
            articleDetailSortingCollection.Add(new SortProperty("Ord", DevExpress.Xpo.DB.SortingDirection.Ascending));

            try
            {
                foreach (FIN_DocumentFinanceMaster document in pFinanceDocuments)
                {
                    // Apply Sorting, this way we respect same Order
                    document.DocumentDetail.Sorting = articleDetailSortingCollection;
                    // Init ArticleBag with Discount
                    ArticleBag articleBag = new ArticleBag(document.Discount);

                    foreach (FIN_DocumentFinanceDetail articleDetail in document.DocumentDetail)
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

        private void CallCancelFinanceMasterDocumentsDialog(Window pDialog, List<FIN_DocumentFinanceMaster> pListSelectDocuments)
        {
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
                        dialogResponse = Utils.GetInputText(pDialog, DialogFlags.Modal, fileWindowIcon, string.Format(Resx.global_cancel_document_input_text_label, documentMaster.DocumentNumber), string.Empty, SettingsApp.RegexAlfaNumericExtended, true);

                        if (dialogResponse.ResponseType == ResponseType.Ok)
                        {
                            //_log.Debug(string.Format("DocumentNumber:[{0}], DocumentStatusStatus:[{1}], reason:[{2}]", document.DocumentNumber, document.DocumentStatusStatus, dialogResponse.InputText));
                            currentDateTime = FrameworkUtils.CurrentDateTimeAtomic();
                            documentMaster.DocumentStatusStatus = "A";
                            documentMaster.DocumentStatusDate = currentDateTime.ToString(SettingsApp.DateTimeFormatCombinedDateTime);
                            documentMaster.DocumentStatusReason = dialogResponse.Text;
                            documentMaster.DocumentStatusUser = GlobalFramework.LoggedUser.CodeInternal;
                            //WIP: CancellWayBills : //ATWS: Check if Sent Resend Document to AT WebServices
                            //WIP: CancellWayBills : bool resendDocumentToAT = false;
                            //WIP: CancellWayBills : if (
                            //WIP: CancellWayBills :     SettingsApp.ServiceATSendDocumentsWayBill 
                            //WIP: CancellWayBills :     && SettingsApp.ConfigurationSystemCountry.Oid == SettingsApp.XpoOidConfigurationCountryPortugal
                            //WIP: CancellWayBills :     && documentMaster.DocumentType.WsAtDocument
                            //WIP: CancellWayBills :     && documentMaster.DocumentType.WayBill
                            //WIP: CancellWayBills :     && documentMaster.DocumentType.Oid != SettingsApp.XpoOidDocumentFinanceTypeInvoiceWayBill
                            //WIP: CancellWayBills :     )
                            //WIP: CancellWayBills : {
                            //WIP: CancellWayBills :     resendDocumentToAT = true;
                            //WIP: CancellWayBills : }

                            //WIP: CancellWayBills : //Assign ResendDocument to Document
                            //WIP: CancellWayBills : if (resendDocumentToAT) documentMaster.ATResendDocument = true;
                            documentMaster.Save();
                            //ATWS: Call ResendDocument to At WebService
                            //WIP: CancellWayBills : if (resendDocumentToAT) 
                            //WIP: CancellWayBills : {
                            //WIP: CancellWayBills : 	FrameworkCalls.SendDocumentToATWSDialog(pDialog, documentMaster);
                            //WIP: CancellWayBills : }

                            //Process Stock : Restore Stock after Cancell Documents
                            ProcessArticleStock.Add(documentMaster, true);

                            //Audit
                            FrameworkUtils.Audit("FINANCE_DOCUMENT_CANCELLED", string.Format("{0} {1}: {2}", documentMaster.DocumentType.Designation, Resx.global_document_cancelled, documentMaster.DocumentNumber));
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
                _log.Error(ex.Message, ex);
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
        private FIN_DocumentFinancePayment OpenDocument(
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> pSourceWindow,
            List<FIN_DocumentFinanceMaster> pFinanceDocuments,
            Guid pCustomer,
            Guid pPaymentMethod,
            Guid pConfigurationCurrency,
            decimal pPaymentAmount,
            string pPaymentNotes = ""
        )
        {
            //Local Vars
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster> parentDialog = (PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinanceMaster>)pSourceWindow;
            FIN_DocumentFinancePayment resultDocument = null;
            List<FIN_DocumentFinanceMaster> listInvoices = new List<FIN_DocumentFinanceMaster>();
            List<FIN_DocumentFinanceMaster> listCreditNotes = new List<FIN_DocumentFinanceMaster>();

            try
            {
                foreach (FIN_DocumentFinanceMaster document in pFinanceDocuments)
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

        private bool CanCancelFinanceMasterDocument(FIN_DocumentFinanceMaster pDocumentFinanceMaster)
        {
            bool result = false;
            List<string> dependentDocuments = new List<string>();
            string documentNumber = string.Empty;
            DateTime currentDateDay = FrameworkUtils.CurrentDateTimeAtomicMidnight();
            DateTime currentDocumentDateDay = FrameworkUtils.DateTimeToMidnightDate(pDocumentFinanceMaster.Date);

            //Check if Document have dependent non Cancelled Child FinanceDocuments
            string sqlFinanceMaster = string.Format("SELECT DocumentNumber FROM fin_documentfinancemaster WHERE DocumentParent = '{0}' AND DocumentStatusStatus <> 'A' ORDER BY CreatedAt;", pDocumentFinanceMaster.Oid);
            XPSelectData xPSelectDataFinanceMaster = FrameworkUtils.GetSelectedDataFromQuery(sqlFinanceMaster);
            foreach (SelectStatementResultRow row in xPSelectDataFinanceMaster.Data)
            {
                documentNumber = row.Values[xPSelectDataFinanceMaster.GetFieldIndex("DocumentNumber")].ToString();
                if (!dependentDocuments.Contains(documentNumber)) dependentDocuments.Add(documentNumber);
                //_log.Debug(string.Format("DocumentNumber: [{0}]", documentNumber));
            }

            //Check if Document have dependent non Cancelled Payments
            string sqlFinancePayment = string.Format("SELECT fmaDocumentNumber AS DocumentNumber FROM view_documentfinancepayment WHERE fmaOid = '{0}' AND fpaPaymentStatus <> 'A' ORDER BY fmaCreatedAt;", pDocumentFinanceMaster.Oid);
            XPSelectData xPSelectDataFinancePayment = FrameworkUtils.GetSelectedDataFromQuery(sqlFinancePayment);
            foreach (SelectStatementResultRow row in xPSelectDataFinancePayment.Data)
            {
                documentNumber = row.Values[xPSelectDataFinancePayment.GetFieldIndex("DocumentNumber")].ToString();
                if (!dependentDocuments.Contains(documentNumber)) dependentDocuments.Add(documentNumber);
                //_log.Debug(string.Format("DocumentNumber: [{0}]", documentNumber));
            }

            //Check if Current Document is not Already Canceled or Invoiced and Dont Have DependentDocuments, and Date of Document is Today
            if (
                    //Is not Invoiced or Cancelled
                    pDocumentFinanceMaster.DocumentStatusStatus != "A" &&
                    pDocumentFinanceMaster.DocumentStatusStatus != "F" &&
                    //Dont have Dependent Documents
                    dependentDocuments.Count == 0 &&
                    //Document date is Today
                    currentDateDay == currentDocumentDateDay
                )
            {
                result = true;
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
                    _listSelectFinanceMasterDocuments.Add((FIN_DocumentFinanceMaster)FrameworkUtils.GetXPGuidObject(typeof(FIN_DocumentFinanceMaster), itemGuid));
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
            TouchButtonIconWithText buttonPrintDocument = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Print, "touchButtonPrintDocument_Green");
            TouchButtonIconWithText buttonClose = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Close);
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
                Resx.window_title_select_worksession_period_day,
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
                    POS_WorkSessionPeriod workSessionPeriodParent = (POS_WorkSessionPeriod)FrameworkUtils.GetXPGuidObject(typeof(POS_WorkSessionPeriod), itemGuid);
                    POS_WorkSessionPeriod workSessionPeriodChild;
                    //Print Parent Session : PrintWorkSessionMovement
                    FrameworkCalls.PrintWorkSessionMovement(this, GlobalFramework.LoggedTerminal.Printer, workSessionPeriodParent);

                    //Get Child Sessions
                    string sql = string.Format(@"SELECT Oid FROM pos_worksessionperiod WHERE Parent = '{0}' ORDER BY DateStart;", workSessionPeriodParent.Oid);
                    XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sql);
                    foreach (DevExpress.Xpo.DB.SelectStatementResultRow row in xPSelectData.Data)
                    {
                        //Print Child Sessions
                        workSessionPeriodChild = (POS_WorkSessionPeriod)FrameworkUtils.GetXPGuidObject(typeof(POS_WorkSessionPeriod), new Guid(row.Values[xPSelectData.GetFieldIndex("Oid")].ToString()));
                        //PrintWorkSessionMovement
                        FrameworkCalls.PrintWorkSessionMovement(this, GlobalFramework.LoggedTerminal.Printer, workSessionPeriodChild);
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
            for (int i = 1; i < pDialog.ActionAreaButtons.Count - 1; i++)
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
            FIN_DocumentFinancePayment documentFinancePayment;

            //Default ActionArea Buttons
            TouchButtonIconWithText buttonClose = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Close);
            TouchButtonIconWithText buttonPrintDocument = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Print, "touchButtonPrintDocument_Green");
            TouchButtonIconWithText buttonPrintDocumentAs = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.PrintAs, "touchButtonPrintDocumentAs_Green");
            TouchButtonIconWithText buttonCancelDocument = ActionAreaButton.FactoryGetDialogButtonType("touchButtonCancelDocument_Green", Resx.global_button_label_cancel_document, _fileActionCancel);
            TouchButtonIconWithText buttonOpenDocument = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.OpenDocument, "touchButtonOpenDocument_Green");
            TouchButtonIconWithText buttonSendEmailDocument = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.SendEmailDocument, "touchButtonSendEmailDocument_Green");
            buttonPrintDocument.Sensitive = false;
            buttonPrintDocumentAs.Sensitive = false;
            buttonCancelDocument.Sensitive = false;
            buttonOpenDocument.Sensitive = false;
            buttonSendEmailDocument.Sensitive = false;
            //ActionArea Buttons
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            //Add references to Send to Event CursorChanged
            ActionAreaButton actionAreaButtonClose = new ActionAreaButton(buttonClose, ResponseType.Close);
            _actionAreaButtonPrintPayment = new ActionAreaButton(buttonPrintDocument, _responseTypePrint);
            _actionAreaButtonPrintPaymentAs = new ActionAreaButton(buttonPrintDocumentAs, _responseTypePrintAs);
            _actionAreaButtonCancelPayment = new ActionAreaButton(buttonCancelDocument, _responseTypeCancelDocument);
            _actionAreaButtonOpenDocument = new ActionAreaButton(buttonOpenDocument, _responseTypeOpenDocument);
            _actionAreaButtonSendEmailDocument = new ActionAreaButton(buttonSendEmailDocument, _responseTypeSendEmailDocument);
            actionAreaButtons.Add(_actionAreaButtonCancelPayment);
            actionAreaButtons.Add(_actionAreaButtonPrintPayment);
            actionAreaButtons.Add(_actionAreaButtonPrintPaymentAs);
            actionAreaButtons.Add(_actionAreaButtonOpenDocument);
            actionAreaButtons.Add(_actionAreaButtonSendEmailDocument);
            actionAreaButtons.Add(actionAreaButtonClose);

            //Define Criteria
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");

            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinancePayment>
              dialogPayments = new PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentFinancePayment>(
                this,
                DialogFlags.DestroyWithParent,
                Resx.window_title_select_payments,
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
                    }
                }
                else if (dialogPayments.GenericTreeViewMode == GenericTreeViewMode.CheckBox)
                {

                    //Get value from Model, its Outside XPGuidObject Scope
                    itemChecked = (bool)dialogPayments.GenericTreeView.GetCurrentModelCheckBoxValue();
                    documentFinancePayment = (FIN_DocumentFinancePayment)dialogPayments.GenericTreeView.DataSourceRow;
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

            FIN_DocumentFinancePayment documentFinancePayment = (FIN_DocumentFinancePayment)dialog.GenericTreeView.DataSourceRow;

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
                else if (
                    args.ResponseId == _responseTypeCancelDocument ||
                    args.ResponseId == _responseTypeOpenDocument ||
                    args.ResponseId == _responseTypeSendEmailDocument
                )
                {
                    _listSelectFinancePaymentDocuments = new List<FIN_DocumentFinancePayment>();

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
                    }

                    //COMMON : Always refresh TreeView, IF Dialog Returns a Valid Document/OK
                    if (resultDocument != null)
                    {
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
                    _listSelectFinancePaymentDocuments.Add((FIN_DocumentFinancePayment)FrameworkUtils.GetXPGuidObject(typeof(FIN_DocumentFinancePayment), itemGuid));
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
                    FIN_DocumentFinancePayment documentFinancePayment = (FIN_DocumentFinancePayment)FrameworkUtils.GetXPGuidObject(typeof(FIN_DocumentFinancePayment), itemGuid);

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

        private void CallCancelFinancePaymentDocumentsDialog(Window pDialog, List<FIN_DocumentFinancePayment> pListSelectDocuments)
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

                        dialogResponse = Utils.GetInputText(pDialog, DialogFlags.Modal, fileWindowIcon, string.Format(Resx.global_cancel_document_input_text_label, document.PaymentRefNo), string.Empty, SettingsApp.RegexAlfaNumericExtended, true);
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
                            FrameworkUtils.Audit("FINANCE_DOCUMENT_CANCELLED", string.Format("{0} {1}: {2}", document.DocumentType.Designation, Resx.global_document_cancelled, document.PaymentRefNo));

                            //Removed Payed BIT to all DocumentPayment Documents (FT and NC)
                            foreach (FIN_DocumentFinanceMasterPayment documentPayment in document.DocumentPayment)
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
        private bool CanCancelFinancePaymentDocument(FIN_DocumentFinancePayment pDocumentFinancePayment)
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

        private void OpenFinancePaymentDocuments(Window pDialog, List<FIN_DocumentFinancePayment> pListSelectDocuments)
        {
            List<string> documents = new List<string>();

            try
            {
                foreach (FIN_DocumentFinancePayment document in pListSelectDocuments)
                {
                    documents.Add(ProcessFinanceDocument.GenerateDocumentFinancePaymentPDFIfNotExists(document));
                }

                foreach (var item in documents)
                {
                    if (File.Exists(item))
                    {
                        System.Diagnostics.Process.Start(FrameworkUtils.OSSlash(string.Format(@"{0}\{1}", Environment.CurrentDirectory, item)));
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

                string infoMessage = string.Format(Resx.app_info_show_ignored_cancelled_documents, ignoredDocumentsMessage);
                Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, new Size(600, 400), MessageType.Info, ButtonsType.Ok, Resx.global_information, infoMessage);
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
        private bool GetSensitiveForCloneDocuments(List<FIN_DocumentFinanceMaster> documents)
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
                foreach (FIN_DocumentFinanceMaster item in documents)
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

        // FIN_DocumentFinanceMaster Overload
        private bool GetSensitiveForSendEmailDocuments(List<FIN_DocumentFinanceMaster> documents)
        {
            bool result = true;

            ArrayList customerList = new ArrayList();

            try
            {
                foreach (FIN_DocumentFinanceMaster item in documents)
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

        // FIN_DocumentFinancePayment Overload
        private bool GetSensitiveForSendEmailDocuments(List<FIN_DocumentFinancePayment> documents)
        {
            bool result = true;

            ArrayList customerList = new ArrayList();

            try
            {
                foreach (FIN_DocumentFinancePayment item in documents)
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
