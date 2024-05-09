using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Classes.Hardware.Printers;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.shared.Classes.Orders;
using System;
using System.Collections.Generic;
using System.Drawing;
using logicpos.Classes.Enums.Finance;
using logicpos.Classes.Enums.Tickets;
using logicpos.datalayer.Enums;
using logicpos.financial.service.Objects.Modules.AT;
using logicpos.shared.App;
using logicpos.datalayer.Xpo;
using LogicPOS.DTOs.Common;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using LogicPOS.Settings;

//Class to Link Project LogicPos to FrameWork API, used to Show Common Messages for LogicPos

namespace logicpos
{
    internal class FrameworkCalls
    {
        //Log4Net
        protected static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Size _sizeDefaultWindowSize = new Size(600, 400);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //ProcessFinanceDocument
        //Use: DocumentFinanceMaster resultDocument = FrameworkCalls.ProcessFinanceDocument(SourceWindow, processFinanceDocumentParameter);

        public static fin_documentfinancemaster PersistFinanceDocument(Window pSourceWindow, ProcessFinanceDocumentParameter pProcessFinanceDocumentParameter)
        {
            bool printDocument = true;
            fin_documentfinancemaster result = null;

            try
            {
                //Protection to Check if SystemDate is < Last DocumentDate
                ResponseType responseType = Utils.ShowMessageTouchCheckIfFinanceDocumentHasValidDocumentDate(pSourceWindow, pProcessFinanceDocumentParameter);
                if (responseType != ResponseType.Yes) return result;

                fin_documentfinancemaster documentFinanceMaster = ProcessFinanceDocument.PersistFinanceDocument(pProcessFinanceDocumentParameter, true);
                fin_documentfinancedetailorderreference fin_documentfinancedetailorderreference = new fin_documentfinancedetailorderreference();
                if (documentFinanceMaster != null)
                {
                    //ATWS : SendDocumentToATWSDialog                    
                    if (SendDocumentToATWSEnabled(documentFinanceMaster))
                    {
                        //Financial.service - Envio de Documentos transporte AT (Estrangeiro) [IN:016502]
                        //It is not necessary to communicate to AT if the destination country is different from Portugal
                        //https://suportesage.zendesk.com/hc/pt/articles/203628246-Se-efectuar-um-documento-de-transporte-para-o-estrangeiro-tenho-que-comunicar-
                        if (documentFinanceMaster.DocumentType.SaftDocumentType == SaftDocumentType.MovementOfGoods && documentFinanceMaster.ShipToCountry == "PT" )
                        {
                            try
                            {
                                _logger.Debug(string.Format("Send Document {0} to AT", documentFinanceMaster.DocumentNumber));
                                SendDocumentToATWSDialog(pSourceWindow, documentFinanceMaster);
                            }
                            catch(Exception Ex)
                            {
                                _logger.Error(Ex.Message);
                            }
                            
                            //SendDocumentToATWSDialog(pSourceWindow, documentFinanceMaster);
                        }                 
                    }
                    /* TK013134 - Parking Ticket Module */
                    foreach (var item in SharedFramework.PendentPayedParkingTickets)
                    {
                        _logger.Debug("[PARKING TICKET] Informing Access.Track that the parking ticket has been payed...");
                        AccessTrackParkingTicketService.TimeService accessTrackParkingTicketService = new AccessTrackParkingTicketService.TimeService();

                        bool isTicketPayedInformed = accessTrackParkingTicketService.payTicket(item.Key);


                        _logger.Debug($"[PARKING TICKET] Barcode '{item.Key}' sent to Access.Track: Guid '{item.Value}'");

                        if (!isTicketPayedInformed)
                        {
                            _logger.Debug($"[PARKING TICKET] Barcode '{item.Key}' not identified by Access.Track: Guid '{item.Value}'");
                        }
                        else if (accessTrackParkingTicketService.isTicketValid(item.Key))
                        {
                            _logger.Debug($"[PARKING TICKET] Barcode '{item.Key}' payed successfully");
                        }
                        else
                        {
                            _logger.Error($"[PARKING TICKET] Barcode '{item.Key}' payment has not been recognized by Access.Track!");
                        }

                    }
                    //IN009279 Parking ticket Service - implementar Cartão cliente 
                    int i = 0;
                    foreach (var item in SharedFramework.PendentPayedParkingCards)
                    {
                        _logger.Debug("[PARKING TICKET] Informing Access.Track that the parking card has been payed...");
                        AccessTrackParkingTicketService.TimeService accessTrackParkingTicketService = new AccessTrackParkingTicketService.TimeService();

                        //Number of months paid is passed by document notes
                        string sql = string.Format("SELECT Notes FROM fin_documentfinancemaster where SourceOrderMain = '{0}'", item.Value);
                        var sqlResult = XPOSettings.Session.ExecuteScalar(sql);
                        string sqlResultquantity = sqlResult.ToString();

                        string[] quantity = sqlResultquantity.Trim().Split(' ');
                        int splitCount = quantity.Length;

                        int[] number = new int[splitCount];
                        if (quantity[i].Contains(","))
                            number[i] = int.Parse(quantity[i].Substring(0, quantity[i].IndexOf(','))); //Contains decimal separator
                        else
                            number[i] = int.Parse(quantity[i]); //Contains only numbers, no decimal separator.

                        DateTime localDate = DateTime.Now;
                        DateTime endDate = localDate.AddMonths(Convert.ToInt32(number[i]));
                        string dateNow = localDate.ToString();
                        string dateEnd = endDate.ToString();
                        accessTrackParkingTicketService.payCard(item.Key, dateNow, dateEnd);
                        i++;
                    }//IN009279 ENDS

                    //Always Send back the Valid Document
                    result = documentFinanceMaster;


                    if (documentFinanceMaster.DocumentType.PrintRequestConfirmation)
                    {
                        responseType = Utils.ShowMessageTouch(
                            pSourceWindow,
                            DialogFlags.Modal,
                            MessageType.Question,
                            ButtonsType.YesNo,
                            CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_dialog_document_finance"),
                            CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_request_print_document_confirmation")
                        );

                        if (responseType == ResponseType.No) printDocument = false;
                    }

                    //Print Document
                    fin_documentfinancemaster documentMaster = (fin_documentfinancemaster)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancemaster), documentFinanceMaster.Oid);
                    //documentFinanceMaster.Reload();
                    if (printDocument) PrintFinanceDocument(pSourceWindow, documentMaster);
                }
            }
            catch (Exception ex)
            {
                string errorMessage;
                switch (ex.Message)
                {
                    case "ERROR_MISSING_SERIE":
                        errorMessage = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_creating_financial_document"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_creating_financial_document_missing_series"));
                        break;
                    case "ERROR_INVALID_DOCUMENT_NUMBER":
                        errorMessage = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_creating_financial_document"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_creating_financial_document_invalid_documentnumber"));
                        break;
                    case "ERROR_COMMIT_FINANCE_DOCUMENT":
                        errorMessage = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_creating_financial_document"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_creating_financial_document_commit_session"));
                        break;
                    //TODO: NEW CLASS FinanceDocumentValidate : IMPLEMENT HERE THE RESULT EXCEPTION FOR VALIDATE_SIMPLIFIED_INVOICE
                    default:
                        errorMessage = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_creating_financial_document"), ex.Message);
                        break;
                }
                Utils.ShowMessageTouch(
                  pSourceWindow,
                  DialogFlags.Modal,
                  _sizeDefaultWindowSize,
                  MessageType.Error,
                  ButtonsType.Close,
                  CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error"),
                  errorMessage
                );
            }

            return result;
        }

        //ATWS: Check if Document is a Valid Document to send to ATWebServices
		//Financial.service - Correções no envio de documentos AT [IN:014494]
        public static bool SendDocumentToATWSEnabled(fin_documentfinancemaster documentFinanceMaster)
        {
            bool result = false;

            //Check if SystemCountry is Portugal and is a valid WsAtDocument
            if (XPOSettings.ConfigurationSystemCountry.Oid == CultureSettings.XpoOidConfigurationCountryPortugal && documentFinanceMaster.DocumentType.WsAtDocument)
            {
                //Documents
                if (!documentFinanceMaster.DocumentType.WayBill || documentFinanceMaster.DocumentType.Oid == DocumentSettings.XpoOidDocumentFinanceTypeInvoiceWayBill)
                {
                    //If Enabled in Config
                    result = false; //(SettingsApp.ServiceATSendDocuments);
                }
                //DocumentsWayBill
                else
                {
                    //If Enabled in Config and is not a FinalConsumer
                    //É obrigatório comunicar um documento de transporte à AT cujo destinatário seja um consumidor final?
                    //Não. Estão excluídos das obrigações de comunicação os documentos de transporte em que o destinatário ou adquirente seja consumidor final.
                    result = (/*SettingsApp.ServiceATSendDocumentsWayBill &&*/ documentFinanceMaster.EntityOid != InvoiceSettings.FinalConsumerId);
                }
            }

            return result;
        }

        //ATWS: Send Document to AT WebWebService : UI Part, With Retry Dialog, Calling above SendDocumentToATWS
        public static bool SendDocumentToATWSDialog(Window pSourceWindow, fin_documentfinancemaster pDocumentFinanceMaster)
        {
            //Send Document to AT WebServices - With Retry to notify user and force user to skip
            ServicesATSoapResult sendDocumentResult = new ServicesATSoapResult();
            ResponseType dialogResponse = ResponseType.Yes;
            /* IN009083 - returns true when WS call fails and user opts to do not retry */
            bool isCanceledByUser = false;
            while (sendDocumentResult.ReturnCode != "0" && dialogResponse == ResponseType.Yes)
            {
                //Call SendDocumentToATWS and Receive Result
                sendDocumentResult = SendDocumentToATWS(pDocumentFinanceMaster);

                if (sendDocumentResult == null || sendDocumentResult.ReturnCode != "0")
                {
                    dialogResponse = Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, new Size(700, 440), MessageType.Error, ButtonsType.YesNo, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error"),
                        string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_in_at_webservice"), sendDocumentResult.ReturnCode, sendDocumentResult.ReturnMessage)
                    );
                    /* IN009083 - returns true when WS call fails and user opts to do not retry */
                    if (ResponseType.No.Equals(dialogResponse))
                    {
                        isCanceledByUser = true;
                    }
                }
            }
            return isCanceledByUser;
        }
		//Financial.service - Correções no envio de documentos AT [IN:014494]
        //NEW SEND METHOD
        public static ServicesATSoapResult SendDocumentToATWS(fin_documentfinancemaster pDocumentFinanceMaster)
        {
            ServicesATSoapResult result;
            try
            {
                fin_documentfinancemaster documentMaster = pDocumentFinanceMaster;

                if (documentMaster != null)
                {
                    //Send Document
                    ServicesAT servicesAT = new ServicesAT(documentMaster);
                    //Get Result from SendDocument Object
                    string resultSend = servicesAT.Send();
                    //Get SoapResult
                    result = servicesAT.SoapResult;

                    if (
                        //Error: Não foi possível resolver o nome remoto: 'servicos.portaldasfinancas.gov.pt'
                        result == null
                        //Error: <faultcode>33</faultcode>
                        ||
                        result != null && string.IsNullOrEmpty(result.ReturnCode)
                        )
                    {
                        result = new ServicesATSoapResult("200", resultSend);
                        servicesAT.PersistResult(result);
                        _logger.Error(string.Format("Error {0}: [{1}]", result.ReturnCode, result.ReturnMessage));
                    }
                    else
                    {
                        //Output in ServiceAT With Log, here is optional
                        //Utils.Log(string.Format("SendDocument Result: [{0}]:[{1}]:[{2}]", result.ReturnCode, result.ReturnMessage, result.ReturnRaw));
                    }
                }
                else
                {
                    //All messages are in PT, from ATWS, dont required translation here
                    string errorMsg = string.Format("Documento Inválido: {0}", pDocumentFinanceMaster.DocumentNumber);
                    result = new ServicesATSoapResult("202", errorMsg);
                    _logger.Error(string.Format("Error {0}: [{1}]", result.ReturnCode, result.ReturnMessage));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                if (Environment.UserInteractive) { _logger.Error(ex.Message); }
                //Send Error Message : 210 is All Exceptions Errors
                result = new ServicesATSoapResult("210", ex.Message);
            }

            //Dont Send null ServicesATSoapResult here, else triggers erros outside
            return result;
        }


       
       
        public static fin_documentfinancepayment PersistFinanceDocumentPayment(
            Window pSourceWindow, 
            List<fin_documentfinancemaster> pInvoices, 
            List<fin_documentfinancemaster> pCreditNotes, 
            Guid pCustomer, 
            Guid pPaymentMethod, 
            Guid pConfigurationCurrency, 
            decimal pPaymentAmount, 
            string pPaymentNotes = "")
        {
            fin_documentfinancepayment result = null;

            try
            {
                fin_documentfinancepayment documentFinancePayment = ProcessFinanceDocument.PersistFinanceDocumentPayment(pInvoices, pCreditNotes, pCustomer, pPaymentMethod, pConfigurationCurrency, pPaymentAmount, pPaymentNotes);
                if (documentFinancePayment != null)
                {
                    //Always send back the Valid Document
                    result = documentFinancePayment;

                    //Print Document
                    PrintFinanceDocumentPayment(pSourceWindow, documentFinancePayment);
                }
            }
            catch (ProcessFinanceDocumentValidationException ex)
            {
                string errorMessage;
                switch (ex.Message)
                {
                    case "ERROR_MISSING_SERIE":
                        errorMessage = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_creating_financial_document"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_creating_financial_document_missing_series"));
                        break;
                    case "ERROR_COMMIT_FINANCE_DOCUMENT_PAYMENT":
                    default:
                        errorMessage = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_creating_financial_document"), ex.Exception.Message);
                        break;
                }
                Utils.ShowMessageTouch(
                  pSourceWindow,
                  DialogFlags.Modal,
                  _sizeDefaultWindowSize,
                  MessageType.Error,
                  ButtonsType.Close,
                  CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error"),
                  errorMessage
                );
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //ExportSaftPt

        public static string ExportSaft(Window pSourceWindow, ExportSaftPtMode pExportSaftPtMode)
        {
            string result = string.Empty;
            DateTime dateCurrent = XPOHelper.CurrentDateTimeAtomic();
            DateTime dateStart, dateEnd;

            switch (pExportSaftPtMode)
            {
                case ExportSaftPtMode.WholeYear:
                    dateStart = new DateTime(dateCurrent.Year, 1, 1);
                    dateEnd = new DateTime(dateCurrent.Year, 12, 31);
                    result = ExportSaft(pSourceWindow, dateStart, dateEnd);
                    break;
                case ExportSaftPtMode.LastMonth:
                    dateStart = dateCurrent.AddMonths(-1);
                    dateStart = new DateTime(dateStart.Year, dateStart.Month, 1);
                    dateEnd = dateStart.AddMonths(1).AddSeconds(-1);
                    result = ExportSaft(pSourceWindow, dateStart, dateEnd);
                    break;
                case ExportSaftPtMode.Custom:
                    PosDatePickerStartEndDateDialog dialog = new PosDatePickerStartEndDateDialog(pSourceWindow, DialogFlags.DestroyWithParent);
                    ResponseType response = (ResponseType)dialog.Run();
                    if (response == ResponseType.Ok)
                    {
                        result = ExportSaft(pSourceWindow, dialog.DateStart, dialog.DateEnd);
                    }
                    dialog.Destroy();
                    break;
            }

            return result;
        }

        public static string ExportSaft(Window pSourceWindow, DateTime? pDateTimeStart, DateTime? pDateTimeEnd)
        {
            string result = string.Empty;

            try
            {
                //Overload Management
                if (pDateTimeStart == null || pDateTimeEnd == null)
                {
					//Angola - Certificação [TK:016268]
                    if(System.Configuration.ConfigurationManager.AppSettings["cultureFinancialRules"] == "pt-PT")
                    {
                        result = SaftPt.ExportSaftPt();
                    }
                    else if (System.Configuration.ConfigurationManager.AppSettings["cultureFinancialRules"] == "pt-AO")
                    {
                        result = SaftAo.ExportSaftAO();
                    }
                }
                else
                {
                    DateTime dateTimeStart = Convert.ToDateTime(pDateTimeStart);
                    DateTime dateTimeEnd = Convert.ToDateTime(pDateTimeEnd);
					//Angola - Certificação [TK:016268]
                    if (System.Configuration.ConfigurationManager.AppSettings["cultureFinancialRules"] == "pt-PT")
                    {
                        result = SaftPt.ExportSaftPt(dateTimeStart, dateTimeEnd);
                    }
                    else if (System.Configuration.ConfigurationManager.AppSettings["cultureFinancialRules"] == "pt-AO")
                    {
                        result = SaftAo.ExportSaftAO(dateTimeStart, dateTimeEnd);
                    }
                    
                }

                Utils.ShowMessageTouch(
                  pSourceWindow,
                  DialogFlags.Modal,
                  _sizeDefaultWindowSize,
                  MessageType.Info,
                  ButtonsType.Close,
                  CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_information"),
                  string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_saftpt_exported_successfully"), result)
                );
            }
            catch (Exception ex)
            {
                Utils.ShowMessageTouch(
                  pSourceWindow,
                  DialogFlags.Modal,
                  _sizeDefaultWindowSize,
                  MessageType.Error,
                  ButtonsType.Close,
                  CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error"),
                  CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_saftpt_exported_error")
                );
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PrintFinanceDocument

        public static bool PrintFinanceDocument(Window pSourceWindow, fin_documentfinancemaster pDocumentFinanceMaster)
        {
            return PrintFinanceDocument(pSourceWindow, null, pDocumentFinanceMaster);
        }

        public static bool PrintFinanceDocument(Window pSourceWindow, sys_configurationprinters pPrinter, fin_documentfinancemaster pDocumentFinanceMaster)
        {
            bool result = false;
            bool openDrawer = false;
            PosDocumentFinancePrintDialog.PrintDialogResponse response;

            //Delete generated files
            try
            {

                string rootFolderPath = @"temp";
                string filesToDelete = @"*qrcode*";
                string[] fileList = System.IO.Directory.GetFiles(rootFolderPath, filesToDelete);
                foreach (string file in fileList)
                {
                    System.Diagnostics.Debug.WriteLine(file + "will be deleted");
                    System.IO.File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            if (!LicenceManagement.IsLicensed || !LicenceManagement.CanPrint)
            {
                Utils.ShowMessageTouchErrorUnlicencedFunctionDisabled(pSourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_printing_function_disabled"));
                return false;
            }
			//TK016249 - Impressoras - Diferenciação entre Tipos
            //Deteta janela de origem de forma a escolher qual impressora usar - TicketList -> ThermalPrinter | PosDocumentFinanceDialog -> Printer
            sys_configurationprinters printer;
            sys_configurationprinters printerDoc;            
            if (PrintingSettings.UsingThermalPrinter)
            {
                //Both printer can be the same, if not Defined in DocumentType
                //Printer for Drawer and Document, if not defined in DocumentType
                printer = (pPrinter != null) ? pPrinter : XPOSettings.LoggedTerminal.ThermalPrinter;
                printerDoc = (pDocumentFinanceMaster.DocumentType.Printer != null) ? pDocumentFinanceMaster.DocumentType.Printer : printer;
            }
            else
            {
                //Both printer can be the same, if not Defined in DocumentType
                //Printer for Drawer and Document, if not defined in DocumentType
                printer = (pPrinter != null) ? pPrinter : XPOSettings.LoggedTerminal.Printer;
                printerDoc = (pDocumentFinanceMaster.DocumentType.Printer != null) ? pDocumentFinanceMaster.DocumentType.Printer : printer;
            }

            try
            {
                //Overload Management
                if (printerDoc == null)
                {
                    //Notification : Show Message TouchTerminalWithoutAssociatedPrinter and Store user input, to Show Next Time(Yes) or Not (No)
                    if (printerDoc == null)
                    {
                        Utils.ShowMessageTouchTerminalWithoutAssociatedPrinter(pSourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), pDocumentFinanceMaster.DocumentType.ResourceString));
                    }
                    else
                    {
                        response = PosDocumentFinancePrintDialog.GetDocumentFinancePrintProperties(pSourceWindow, pDocumentFinanceMaster);
                        //Print with default DocumentFinanceYearSerieTerminal Template
                        if (response.Response == ResponseType.Ok) result = PrintRouter.PrintFinanceDocument(pDocumentFinanceMaster);
                    }
                }
                else
                {
                    bool validFiles = true;
                    string extraMessage = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_protected_files_invalid_files_detected_print_document_ignored"), pDocumentFinanceMaster.DocumentNumber);

                    //Printer Drawer : Set openDrawer
                    switch (PrintRouter.GetPrinterToken(printer.PrinterType.Token))
                    {
                        //ThermalPrinter : Ticket Files
                        case "THERMAL_PRINTER_WINDOWS":
                        case "THERMAL_PRINTER_LINUX":
                        case "THERMAL_PRINTER_SOCKET":
                            openDrawer = true;
                            break;
                    }

                    //Printer Document : Set Valid Files
                    switch (PrintRouter.GetPrinterToken(printerDoc.PrinterType.Token))
                    {
                        //ThermalPrinter : Ticket Files
                        case "THERMAL_PRINTER_WINDOWS":
                        case "THERMAL_PRINTER_SOCKET":
                            //validFiles = IsValidProtectedFile(SharedUtils.OSSlash(template.FileTemplate), extraMessage);
                            break;
                        //FastReport : Report Files
                        case "GENERIC_PRINTER_WINDOWS":
                        case "REPORT_EXPORT_PDF":
                            //Required both Template Files ReportDocumentFinance and ReportDocumentFinanceWayBill
                            //validFiles = (
                            //    IsValidProtectedFile(SharedUtils.OSSlash(@"Resources/Reports/UserReports/ReportDocumentFinance.frx"), extraMessage) &&
                            //    IsValidProtectedFile(SharedUtils.OSSlash(@"Resources/Reports/UserReports/ReportDocumentFinanceWayBill.frx"), extraMessage)
                            //);
                            break;
                        case "VIRTUAL_SCREEN":
                            break;
                    }

                    //ProtectedFiles Protection
                    if (!validFiles) return false;

                    //Call Print Document : Receives ResponseType.Ok without user Confirmation, if Document was never Printer
                    response = PosDocumentFinancePrintDialog.GetDocumentFinancePrintProperties(pSourceWindow, pDocumentFinanceMaster);

                    //Print with Parameters Printer and Template
                    if (response.Response == ResponseType.Ok)
                    {
                        //Print Document use Printer Document
                        result = PrintRouter.PrintFinanceDocument(printerDoc, pDocumentFinanceMaster, response.CopyNames, response.SecondCopy, response.Motive);


                        
                        //OpenDoor use Printer Drawer
                        if (openDrawer && pDocumentFinanceMaster.DocumentType.PrintOpenDrawer && !response.SecondCopy) 
                        {
                            var resultOpenDoor = PrintRouter.OpenDoor(XPOSettings.LoggedTerminal.Printer);
                            if (!resultOpenDoor)
                            {
                                Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_information"), string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "open_cash_draw_permissions")));
                            }
                            else
                            {
                                SharedUtils.Audit("CASHDRAWER_OUT", string.Format(
                                CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "audit_message_cashdrawer_out"),
                                XPOSettings.LoggedTerminal.Designation,
                                "Button Open Door"));
                            }
                          
                        }
                    
                    }
                }

            }
            catch (Exception ex)
            {
                Utils.ShowMessageTouchErrorPrintingTicket(pSourceWindow, printer, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PrintFinanceDocumentPayment

        public static bool PrintFinanceDocumentPayment(Window pSourceWindow, fin_documentfinancepayment pDocumentFinancePayment)
        {
            return PrintFinanceDocumentPayment(pSourceWindow, null, pDocumentFinancePayment);
        }

        public static bool PrintFinanceDocumentPayment(Window pSourceWindow, sys_configurationprinters pPrinter, fin_documentfinancepayment pDocumentFinancePayment)
        {
            bool result = false;

            if (!LicenceManagement.IsLicensed || !LicenceManagement.CanPrint)
            {
                Utils.ShowMessageTouchErrorUnlicencedFunctionDisabled(pSourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_printing_function_disabled"));
                return false;
            }

            sys_configurationprinters printer = (pPrinter != null)
              ? pPrinter :
              XPOSettings.LoggedTerminal.Printer;

            try
            {
                //Notification : Show Message TouchTerminalWithoutAssociatedPrinter and Store user input, to Show Next Time(Yes) or Not (No)
                if (printer == null)
                {
                    Utils.ShowMessageTouchTerminalWithoutAssociatedPrinter(pSourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_documentfinance_type_title_rc"));
                }
                else
                {
                    //ProtectedFiles Protection
                    bool validFiles = true;
                    string extraMessage = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_protected_files_invalid_files_detected_print_document_ignored"), pDocumentFinancePayment.PaymentRefNo);
                    switch (PrintRouter.GetPrinterToken(printer.PrinterType.Token))
                    {
                        //ThermalPrinter : Ticket Files
                        case "THERMAL_PRINTER_WINDOWS":
                        case "THERMAL_PRINTER_SOCKET":
                            break;
                        //FastReport : Report Files
                        case "GENERIC_PRINTER_WINDOWS":
                        case "REPORT_EXPORT_PDF":
                            //validFiles = (IsValidProtectedFile(SharedUtils.OSSlash(@"Resources/Reports/UserReports/ReportDocumentFinancePayment.frx"), extraMessage));
                            break;
                        case "VIRTUAL_SCREEN":
                            break;
                    }
                    //ProtectedFiles Protection
                    if (!validFiles) return false;
                    //Recibos com impressão em impressora térmica
                    if (XPOSettings.LoggedTerminal.ThermalPrinter != null)
                    {
                        ResponseType responseType = Utils.ShowMessageTouch(pSourceWindow, DialogFlags.DestroyWithParent, MessageType.Question, ButtonsType.YesNo, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_edit_DialogConfigurationPrintersType_tab1_label"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_printer_choose_printer")); 

                        if (responseType == ResponseType.Yes)
                        {
                            //Call Print Document thermal
                            result = PrintRouter.PrintFinanceDocumentPayment(XPOSettings.LoggedTerminal.ThermalPrinter, pDocumentFinancePayment);
                        }
                        else
                        {
                            result = PrintRouter.PrintFinanceDocumentPayment(printer, pDocumentFinancePayment);
                        }
                    }
                    else
                    {
                        //Call Print Document A4
                        result = PrintRouter.PrintFinanceDocumentPayment(printer, pDocumentFinancePayment);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessageTouchErrorPrintingTicket(pSourceWindow, printer, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //Shared Method to call all other PrintTicketMethods to Check Licence and other Protections
        public static bool SharedPrintTicket(Window pSourceWindow, sys_configurationprinters pPrinter, TicketType pTicketType)
        {
            bool result = false;

            if (!LicenceManagement.IsLicensed || !LicenceManagement.CanPrint)
            {
                Utils.ShowMessageTouchErrorUnlicencedFunctionDisabled(pSourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_printing_function_disabled"));
            }
            else
            {
                try
                {
                    //Notification : Show Message TouchTerminalWithoutAssociatedPrinter and Store user input, to Show Next Time(Yes) or Not (No)
                    if (pPrinter == null)
                    {
                        string ticketTitle = string.Empty;
                        switch (pTicketType)
                        {
                            case TicketType.TableOrder:
                                ticketTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_documentticket_type_title_tt");
                                break;
                            case TicketType.ArticleOrder:
                                ticketTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_documentticket_type_title_ar");
                                break;
                            case TicketType.WorkSession:
                                ticketTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_documentticket_type_title_ws");
                                break;
                            case TicketType.CashDrawer:
                                ticketTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_documentticket_type_title_cs");
                                break;
                            default:
                                break;
                        }
                        Utils.ShowMessageTouchTerminalWithoutAssociatedPrinter(pSourceWindow, ticketTitle);
                    }
                    else
                    {
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PrintTableTicket

        public static bool PrintOrderRequest(Window pSourceWindow, sys_configurationprinters pPrinter, OrderMain pDocumentOrderMain, fin_documentorderticket pOrderTicket)
        {
            bool result = false;

            try
            {
                if (SharedPrintTicket(pSourceWindow, pPrinter, TicketType.TableOrder))
                {
                    ThermalPrinterInternalDocumentOrderRequest thermalPrinterInternalDocumentOrderRequest = new ThermalPrinterInternalDocumentOrderRequest(pPrinter, pOrderTicket);
                    thermalPrinterInternalDocumentOrderRequest.Print();
                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessageTouchErrorPrintingTicket(pSourceWindow, pPrinter, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PrintArticleRequest

        public static bool PrintArticleRequest(Window pSourceWindow, fin_documentorderticket pOrderTicket)
        {
            bool result = false;

            try
            {
                //Removed: Printer is always NULL, is defined in Ticket Article
                //if (SharedPrintTicket(pSourceWindow, null, TicketType.ArticleOrder))
                //{
                result = PrintRouter.PrintArticleRequest(pOrderTicket);
                //}
            }
            catch (Exception ex)
            {
                Utils.ShowMessageTouchErrorPrintingTicket(pSourceWindow, null, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PrintWorkSessionMovement

        public static bool PrintWorkSessionMovement(Window pSourceWindow, sys_configurationprinters pPrinter, pos_worksessionperiod pWorkSessionPeriod)
        {
            bool result = false;
            sys_configurationprinterstemplates template = (sys_configurationprinterstemplates)XPOHelper.GetXPGuidObject(typeof(sys_configurationprinterstemplates), PrintingSettings.XpoOidConfigurationPrintersTemplateWorkSessionMovement);

            try
            {
                if (SharedPrintTicket(pSourceWindow, pPrinter, TicketType.WorkSession))
                {
                    //PrintWorkSessionMovement
                    result = PrintRouter.PrintWorkSessionMovement(pPrinter, pWorkSessionPeriod);
                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessageTouchErrorPrintingTicket(pSourceWindow, pPrinter, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PrintCashDrawerOpenAndMoneyInOut

        public static bool PrintCashDrawerOpenAndMoneyInOut(Window pSourceWindow, sys_configurationprinters pPrinter, string pTicketTitle, decimal pMovementAmount, decimal pTotalAmountInCashDrawer, string pMovementDescription)
        {
            bool result = false;
            sys_configurationprinterstemplates template = (sys_configurationprinterstemplates)XPOHelper.GetXPGuidObject(typeof(sys_configurationprinterstemplates), PrintingSettings.XpoOidConfigurationPrintersTemplateCashDrawerOpenAndMoneyInOut);

            try
            {
                if (SharedPrintTicket(pSourceWindow, pPrinter, TicketType.CashDrawer))
                {
                    result = PrintRouter.PrintCashDrawerOpenAndMoneyInOut(pPrinter, pTicketTitle, pMovementAmount, pTotalAmountInCashDrawer, pMovementDescription);
                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessageTouchErrorPrintingTicket(pSourceWindow, pPrinter, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //ProtectedFiles Protection

        //Proxy Method to Call GlobalApp.ProtectedFiles.IsValidFile with UI Dialog
        public static bool IsValidProtectedFile(string pFilePath)
        {
            return IsValidProtectedFile(pFilePath, string.Empty);
        }

        public static bool IsValidProtectedFile(string pFilePath, string pExtraMessage)
        {
            //Always valid if ProtectedFilesUse not Enabled and ProtectedFilesIgnoreProtection equal to true, this skip Validation
            if (!POSSettings.ProtectedFilesUse && POSSettings.ProtectedFilesIgnoreProtection) return true;

            bool result = true;

            //Check if ContainsKey and ! IsValidFile
            if (GlobalApp.ProtectedFiles.ContainsKey(pFilePath) && !GlobalApp.ProtectedFiles.IsValidFile(pFilePath))
            {
                result = false;
            }

            if (!result)
            {
                string message = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_protected_files_invalid_files_detected"), pFilePath);
                if (pExtraMessage != string.Empty) message = string.Format("{1}{0}{0}{2}", Environment.NewLine, message, pExtraMessage);
                Utils.ShowMessageTouch(GlobalApp.StartupWindow, DialogFlags.Modal, new Size(800, 400), MessageType.Error, ButtonsType.Close, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error"), message);
            }

            return result;
        }
    }
}
