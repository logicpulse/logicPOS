using Gtk;
using logicpos;
using logicpos.Classes.Enums.Finance;
using logicpos.Classes.Enums.Tickets;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Api.Entities;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Settings.Terminal;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.DTOs.Common;
using LogicPOS.DTOs.Printing;
using LogicPOS.Finance.DocumentProcessing;
using LogicPOS.Finance.Saft;
using LogicPOS.Finance.Services;
using LogicPOS.Globalization;
using LogicPOS.Printing.Documents;
using LogicPOS.Settings;
using LogicPOS.Shared.Orders;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI
{
    internal class FrameworkCalls
    {
        //Log4Net
        protected static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Size _sizeDefaultWindowSize = new Size(600, 400);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //ProcessFinanceDocument
        //Use: DocumentFinanceMaster resultDocument = FrameworkCalls.ProcessFinanceDocument(SourceWindow, processFinanceDocumentParameter);

        public static fin_documentfinancemaster PersistFinanceDocument(Window parentWindow, DocumentProcessingParameters pProcessFinanceDocumentParameter)
        {
            bool printDocument = true;
            fin_documentfinancemaster result = null;

            try
            {
                //Protection to Check if SystemDate is < Last DocumentDate
              //  ResponseType messageDialog = Utils.ShowMessageTouchCheckIfFinanceDocumentHasValidDocumentDate(parentWindow, pProcessFinanceDocumentParameter);
                var responseType = new CustomAlert(parentWindow)
                                              .WithMessage("IfFinanceDocumentHasValidDocumentDate")
                                              .WithMessageType(MessageType.Question)
                                              .WithButtonsType(ButtonsType.YesNo)
                                              .WithTitleResource("global_information")
                                              .ShowAlert();

                if (responseType != ResponseType.Yes) return result;

                fin_documentfinancemaster documentFinanceMaster = DocumentProcessingUtils.PersistFinanceDocument(pProcessFinanceDocumentParameter, true);
                fin_documentfinancedetailorderreference fin_documentfinancedetailorderreference = new fin_documentfinancedetailorderreference();
                if (documentFinanceMaster != null)
                {
                    //ATWS : SendDocumentToATWSDialog                    
                    if (SendDocumentToATWSEnabled(documentFinanceMaster))
                    {
                        //Financial.service - Envio de Documentos transporte AT (Estrangeiro) [IN:016502]
                        //It is not necessary to communicate to AT if the destination country is different from Portugal
                        //https://suportesage.zendesk.com/hc/pt/articles/203628246-Se-efectuar-um-documento-de-transporte-para-o-estrangeiro-tenho-que-comunicar-
                        if (documentFinanceMaster.DocumentType.SaftDocumentType == SaftDocumentType.MovementOfGoods && documentFinanceMaster.ShipToCountry == "PT")
                        {
                            try
                            {
                                _logger.Debug(string.Format("Send Document {0} to AT", documentFinanceMaster.DocumentNumber));
                                SendDocumentToATWSDialog(parentWindow, documentFinanceMaster);
                            }
                            catch (Exception Ex)
                            {
                                _logger.Error(Ex.Message);
                            }

                            //SendDocumentToATWSDialog(parentWindow, documentFinanceMaster);
                        }
                    }
                    /* TK013134 - Parking Ticket Module */
                    foreach (var item in GeneralSettings.PendentPaidParkingTickets)
                    {
                        _logger.Debug("[PARKING TICKET] Informing Access.Track that the parking ticket has been payed...");
                        logicpos.AccessTrackParkingTicketService.TimeService accessTrackParkingTicketService = new logicpos.AccessTrackParkingTicketService.TimeService();

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
                    foreach (var item in GeneralSettings.PendentPaidParkingCards)
                    {
                        _logger.Debug("[PARKING TICKET] Informing Access.Track that the parking card has been payed...");
                        logicpos.AccessTrackParkingTicketService.TimeService accessTrackParkingTicketService = new logicpos.AccessTrackParkingTicketService.TimeService();

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
                                 responseType = new CustomAlert(parentWindow)
                                              .WithMessageResource("dialog_message_request_print_document_confirmation")
                                              .WithMessageType(MessageType.Question)
                                              .WithButtonsType(ButtonsType.YesNo)
                                              .WithTitleResource("window_title_dialog_document_finance")
                                              .ShowAlert();

                        if (responseType == ResponseType.No) printDocument = false;
                    }

                    //Print Document
                    fin_documentfinancemaster documentMaster = (fin_documentfinancemaster)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancemaster), documentFinanceMaster.Oid);
                    //documentFinanceMaster.Reload();
                    if (printDocument) PrintFinanceDocument(parentWindow, documentMaster);
                }
            }
            catch (Exception ex)
            {
                string errorMessage;
                switch (ex.Message)
                {
                    case "ERROR_MISSING_SERIE":
                        errorMessage = string.Format(GeneralUtils.GetResourceByName("dialog_message_error_creating_financial_document"), GeneralUtils.GetResourceByName("dialog_message_error_creating_financial_document_missing_series"));
                        break;
                    case "ERROR_INVALID_DOCUMENT_NUMBER":
                        errorMessage = string.Format(GeneralUtils.GetResourceByName("dialog_message_error_creating_financial_document"), GeneralUtils.GetResourceByName("dialog_message_error_creating_financial_document_invalid_documentnumber"));
                        break;
                    case "ERROR_COMMIT_FINANCE_DOCUMENT":
                        errorMessage = string.Format(GeneralUtils.GetResourceByName("dialog_message_error_creating_financial_document"), GeneralUtils.GetResourceByName("dialog_message_error_creating_financial_document_commit_session"));
                        break;
                    //TODO: NEW CLASS FinanceDocumentValidate : IMPLEMENT HERE THE RESULT EXCEPTION FOR VALIDATE_SIMPLIFIED_INVOICE
                    default:
                        errorMessage = string.Format(GeneralUtils.GetResourceByName("dialog_message_error_creating_financial_document"), ex.Message);
                        break;
                }
                
                var messageDialog= new CustomAlert(parentWindow)
                                 .WithMessage(errorMessage)
                                 .WithSize(_sizeDefaultWindowSize)
                                 .WithMessageType(MessageType.Error)
                                 .WithButtonsType(ButtonsType.Close)
                                 .WithTitleResource("global_error")
                                 .ShowAlert();
            }

            return result;
        }

        //ATWS: Check if Document is a Valid Document to send to ATWebServices
        //Financial.service - Correções no envio de documentos AT [IN:014494]
        public static bool SendDocumentToATWSEnabled(fin_documentfinancemaster documentFinanceMaster)
        {
            bool result = false;

            //Check if SystemCountry is Portugal and is a valid WsAtDocument
            if (CultureSettings.CountryIdIsPortugal(XPOSettings.ConfigurationSystemCountry.Oid) && documentFinanceMaster.DocumentType.WsAtDocument)
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
                    result = /*SettingsApp.ServiceATSendDocumentsWayBill &&*/ documentFinanceMaster.EntityOid != InvoiceSettings.FinalConsumerId;
                }
            }

            return result;
        }

        //ATWS: Send Document to AT WebWebService : UI Part, With Retry Dialog, Calling above SendDocumentToATWS
        public static bool SendDocumentToATWSDialog(Window parentWindow, fin_documentfinancemaster pDocumentFinanceMaster)
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
                    /*dialogResponse = Utils.ShowMessageBox(parentWindow, DialogFlags.Modal, new Size(700, 440), MessageType.Error, ButtonsType.YesNo, GeneralUtils.GetResourceByName("global_error"),
                        string.Format(GeneralUtils.GetResourceByName("dialog_message_error_in_at_webservice"), sendDocumentResult.ReturnCode, sendDocumentResult.ReturnMessage)
                    );*/

                    dialogResponse = new CustomAlert(parentWindow)
                                   .WithMessageResource("dialog_message_error_in_at_webservice")
                                   .WithSize(new Size(700,440))
                                   .WithMessageType(MessageType.Error)
                                   .WithButtonsType(ButtonsType.YesNo)
                                   .WithTitleResource("global_error")
                                   .ShowAlert();

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
                    ATService servicesAT = new ATService(documentMaster);
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
            Window parentWindow,
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
                fin_documentfinancepayment documentFinancePayment = DocumentProcessingUtils.PersistFinanceDocumentPayment(pInvoices, pCreditNotes, pCustomer, pPaymentMethod, pConfigurationCurrency, pPaymentAmount, pPaymentNotes);
                if (documentFinancePayment != null)
                {
                    //Always send back the Valid Document
                    result = documentFinancePayment;

                    //Print Document
                    PrintFinanceDocumentPayment(parentWindow, documentFinancePayment);
                }
            }
            catch (DocumentProcessingValidationException ex)
            {
                string errorMessage;
                switch (ex.Message)
                {
                    case "ERROR_MISSING_SERIE":
                        errorMessage = string.Format(GeneralUtils.GetResourceByName("dialog_message_error_creating_financial_document"), GeneralUtils.GetResourceByName("dialog_message_error_creating_financial_document_missing_series"));
                        break;
                    case "ERROR_COMMIT_FINANCE_DOCUMENT_PAYMENT":
                    default:
                        errorMessage = string.Format(GeneralUtils.GetResourceByName("dialog_message_error_creating_financial_document"), ex.Exception.Message);
                        break;
                }
                var messageDialog = new CustomAlert(parentWindow)
                    .WithMessage(errorMessage)
                    .WithSize(_sizeDefaultWindowSize)
                    .WithMessageType(MessageType.Error)
                    .WithButtonsType(ButtonsType.Close)
                    .WithTitleResource("Global_error")
                    .ShowAlert();
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //ExportSaftPt

        public static string ExportSaft(Window parentWindow, ExportSaftPtMode pExportSaftPtMode)
        {
            string result = string.Empty;
            DateTime dateCurrent = XPOUtility.CurrentDateTimeAtomic();
            DateTime dateStart, dateEnd;

            switch (pExportSaftPtMode)
            {
                case ExportSaftPtMode.WholeYear:
                    dateStart = new DateTime(dateCurrent.Year, 1, 1);
                    dateEnd = new DateTime(dateCurrent.Year, 12, 31);
                    result = ExportSaft(parentWindow, dateStart, dateEnd);
                    break;
                case ExportSaftPtMode.LastMonth:
                    dateStart = dateCurrent.AddMonths(-1);
                    dateStart = new DateTime(dateStart.Year, dateStart.Month, 1);
                    dateEnd = dateStart.AddMonths(1).AddSeconds(-1);
                    result = ExportSaft(parentWindow, dateStart, dateEnd);
                    break;
                case ExportSaftPtMode.Custom:
                    PosDatePickerStartEndDateDialog dialog = new PosDatePickerStartEndDateDialog(parentWindow, DialogFlags.DestroyWithParent);
                    ResponseType response = (ResponseType)dialog.Run();
                    if (response == ResponseType.Ok)
                    {
                        result = ExportSaft(parentWindow, dialog.DateStart, dialog.DateEnd);
                    }
                    dialog.Destroy();
                    break;
            }

            return result;
        }

        public static string ExportSaft(Window parentWindow, DateTime? pDateTimeStart, DateTime? pDateTimeEnd)
        {
            string result = string.Empty;

            try
            {
                //Overload Management
                if (pDateTimeStart == null || pDateTimeEnd == null)
                {
                    //Angola - Certificação [TK:016268]
                    if (System.Configuration.ConfigurationManager.AppSettings["cultureFinancialRules"] == "pt-PT")
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

                var messageDialog = new CustomAlert(parentWindow)
                    .WithMessage(string.Format(GeneralUtils.GetResourceByName("dialog_message_saftpt_exported_successfully"), result))
                    .WithSize(_sizeDefaultWindowSize)
                    .WithMessageType(MessageType.Info)
                    .WithButtonsType(ButtonsType.Close)
                    .WithTitleResource("global_information")
                    .ShowAlert();
            }
            catch (Exception ex)
            {
                var messageDialog = new CustomAlert(parentWindow)
                                    .WithMessageResource("dialog_message_saftpt_exported_error")
                                    .WithSize(_sizeDefaultWindowSize)
                                    .WithMessageType(MessageType.Error)
                                    .WithButtonsType(ButtonsType.Close)
                                    .WithTitleResource("global_error")
                                    .ShowAlert();

 
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PrintFinanceDocument

        public static bool PrintFinanceDocument(Window parentWindow, fin_documentfinancemaster pDocumentFinanceMaster)
        {
            return PrintFinanceDocument(parentWindow,  pDocumentFinanceMaster);
        }

        public static bool PrintFinanceDocument(
            Window sourceWindow,
            sys_configurationprinters pPrinter,
            fin_documentfinancemaster financeMaster,
            string terminalDesignation, string userName, CompanyPrintingInformationsDto companyInformationsDto)
        {
            bool result = false;
            bool openDrawer = false;
            PosDocumentFinancePrintDialog.PrintDialogResponse printDialogResponse;

            DeleteGeneratedFiles();

            //TK016249 - Impressoras - Diferenciação entre Tipos
            //Deteta janela de origem de forma a escolher qual impressora usar - TicketList -> ThermalPrinter | PosDocumentFinanceDialog -> Printer
            sys_configurationprinters printer;
            sys_configurationprinters financeMasterPrinter;
            if (PrintingSettings.ThermalPrinter.UsingThermalPrinter)
            {
                //Both printer can be the same, if not Defined in DocumentType
                //Printer for Drawer and Document, if not defined in DocumentType
                printer = pPrinter != null ? pPrinter : TerminalSettings.LoggedTerminal.ThermalPrinter;
                financeMasterPrinter = financeMaster.DocumentType.Printer != null ? financeMaster.DocumentType.Printer : printer;
            }
            else
            {
                //Both printer can be the same, if not Defined in DocumentType
                //Printer for Drawer and Document, if not defined in DocumentType
                printer = pPrinter != null ? pPrinter : TerminalSettings.LoggedTerminal.Printer;
                financeMasterPrinter = financeMaster.DocumentType.Printer != null ? financeMaster.DocumentType.Printer : printer;
            }

            try
            {
                //Overload Management
                if (financeMasterPrinter == null)
                {
                    //Notification : Show Message TouchTerminalWithoutAssociatedPrinter and Store user input, to Show Next Time(Yes) or Not (No)
                    if (financeMasterPrinter == null)
                    {
                      var messageDialog = new CustomAlert(sourceWindow)
                                        .WithMessageResource(financeMaster.DocumentType.ResourceString)
                                        .ShowAlert();
                    }
                    else
                    {
                        printDialogResponse = PosDocumentFinancePrintDialog.GetDocumentFinancePrintProperties(sourceWindow, financeMaster);
                        //Print with default DocumentFinanceYearSerieTerminal Template
                        var financeMasterDto = MappingUtils.GetPrintDocumentMasterDto(financeMaster);
                        var printerDto = new PrinterDto { Designation = printer.Designation, CutCommand = printer.ThermalCutCommand };
                        if (printDialogResponse.Response == ResponseType.Ok) result = Printing.Utility.PrintingUtils.PrintFinanceDocument(printerDto, financeMasterDto);
                    }
                }
                else
                {
                    bool validFiles = true;
                    string extraMessage = string.Format(GeneralUtils.GetResourceByName("dialog_message_error_protected_files_invalid_files_detected_print_document_ignored"), financeMaster.DocumentNumber);

                    //Printer Drawer : Set openDrawer
                    switch (Printing.Utility.PrintingUtils.GetPrinterToken(printer.PrinterType.Token))
                    {
                        //ThermalPrinter : Ticket Files
                        case "THERMAL_PRINTER_WINDOWS":
                        case "THERMAL_PRINTER_LINUX":
                        case "THERMAL_PRINTER_SOCKET":
                            openDrawer = true;
                            break;
                    }

                    //Printer Document : Set Valid Files
                    switch (Printing.Utility.PrintingUtils.GetPrinterToken(financeMasterPrinter.PrinterType.Token))
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
                    printDialogResponse = PosDocumentFinancePrintDialog.GetDocumentFinancePrintProperties(sourceWindow, financeMaster);

                    //Print with Parameters Printer and Template
                    if (printDialogResponse.Response == ResponseType.Ok)
                    {
                        var financeMasterPrinterDto = MappingUtils.GetPrinterDto(financeMasterPrinter);
                        var financeMasterDto = MappingUtils.GetPrintDocumentMasterDto(financeMaster);

                        result = Printing.Utility.PrintingUtils.PrintFinanceDocument(
                            financeMasterPrinterDto,
                            financeMasterDto,
                            printDialogResponse.CopyNames,
                            printDialogResponse.SecondCopy,
                            printDialogResponse.Motive);



                        //OpenDoor use Printer Drawer
                        if (openDrawer && financeMaster.DocumentType.PrintOpenDrawer && !printDialogResponse.SecondCopy)
                        {
                            var resultOpenDoor = Printing.Utility.PrintingUtils.OpenDoor();
                            if (!resultOpenDoor)
                            {
                                var messageDialog = new CustomAlert(sourceWindow)
                                                    .WithMessageResource("open_cash_draw_permissions")
                                                    .WithSize(_sizeDefaultWindowSize)
                                                    .WithMessageType(MessageType.Info)
                                                    .WithButtonsType(ButtonsType.Close)
                                                    .WithTitleResource("global_information")
                                                    .ShowAlert();
                            }
                            else
                            {
                                XPOUtility.Audit("CASHDRAWER_OUT", string.Format(
                                 GeneralUtils.GetResourceByName("audit_message_cashdrawer_out"),
                                 TerminalSettings.LoggedTerminal.Designation,
                                 "Button Open Door"));
                            }

                        }

                    }
                }

            }
            catch (Exception ex)
            {
                var printerDto = MappingUtils.GetPrinterDto(financeMasterPrinter);
                //Utils.ShowMessageTouchErrorPrintingTicket(sourceWindow, printerDto, ex); ::: DÚVIDA NA SUBSTITUIÇÃO ::: LUCIANO

                var messageDialog = new CustomAlert(sourceWindow)
                                                .WithMessage(ex.Message)
                                                .WithMessageType(MessageType.Error)
                                                .WithTitleResource("global_error")
                                                .ShowAlert();

            }

            return result;
        }

        private static void DeleteGeneratedFiles()
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

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PrintFinanceDocumentPayment

        public static bool PrintFinanceDocumentPayment(Window parentWindow, fin_documentfinancepayment pDocumentFinancePayment)
        {
            return PrintFinanceDocumentPayment(parentWindow, pDocumentFinancePayment);
        }

        public static bool PrintFinanceDocumentPayment(
            Window parentWindow,
            PrinterDto printer,
            string terminalDesignation,
            string userName,
            CompanyPrintingInformationsDto companyInformationsDto,
            Document pDocumentFinancePayment)
        {
            bool result = false;

            if (LicenseSettings.LicenceRegistered == false)
            {
                var messageDialog = new CustomAlert(parentWindow)
                                                .WithMessageResource(ResourceNames.PRINTING_DISABLED_MESSAGE)
                                                .WithSize(_sizeDefaultWindowSize)
                                                .WithTitleResource("global_error")
                                                .ShowAlert();
                //Utils.ShowMessageBoxUnlicensedError(parentWindow, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, ResourceNames.PRINTING_DISABLED_MESSAGE));
                return false;
            }

            try
            {
                if (printer == null)
                {
                    /*Utils.ShowMessageTouchTerminalWithoutAssociatedPrinter(
                        parentWindow,
                        GeneralUtils.GetResourceByName("global_documentfinance_type_title_rc"));*/

                    var messageDialog = new CustomAlert(parentWindow)
                                               .WithMessageResource("global_documentfinance_type_title_rc")
                                               .ShowAlert();

                    return false;
                }

                //ProtectedFiles Protection
                bool validFiles = true;
                string extraMessage = string.Format(GeneralUtils.GetResourceByName("dialog_message_error_protected_files_invalid_files_detected_print_document_ignored"), pDocumentFinancePayment.Number);
                switch (Printing.Utility.PrintingUtils.GetPrinterToken(printer.Token))
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
               // var DocumentFinancePaymentDto = MappingUtils.GetPrintingFinancePaymentDto(pDocumentFinancePayment);
                //ProtectedFiles Protection
                if (!validFiles) return false;
                //Recibos com impressão em impressora térmica
                if (TerminalSettings.HasLoggedTerminal)
                {
                    var responseType = new CustomAlert(parentWindow)
                                                .WithMessageResource("global_printer_choose_printer")
                                                .WithMessageType(MessageType.Question)
                                                .WithButtonsType(ButtonsType.YesNo)
                                                .WithTitleResource("dialog_edit_DialogConfigurationPrintersType_tab1_label")
                                                .ShowAlert();

                    if (responseType == ResponseType.Yes)
                    {
                        var printerDto = LoggedTerminalSettings.GetPrinterDto();

                        result = Printing.Utility.PrintingUtils.PrintFinanceDocumentPayment(printerDto, terminalDesignation, userName, companyInformationsDto, pDocumentFinancePayment);
                    }
                    else
                    {
                        result = Printing.Utility.PrintingUtils.PrintFinanceDocumentPayment(printer, terminalDesignation, userName, companyInformationsDto, pDocumentFinancePayment);
                    }
                }
                else
                {
                    //Call Print Document A4
                    result = Printing.Utility.PrintingUtils.PrintFinanceDocumentPayment(printer, terminalDesignation, userName, companyInformationsDto, pDocumentFinancePayment);
                }

            }
            catch (Exception ex)
            {
                var messageDialog = new CustomAlert(parentWindow)
                                    .WithMessage(ex.Message)
                                    .ShowAlert();

                //Utils.ShowMessageTouchErrorPrintingTicket(parentWindow, printer, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //Shared Method to call all other PrintTicketMethods to Check Licence and other Protections
        public static bool SharedPrintTicket(Window parentWindow, sys_configurationprinters pPrinter, TicketType pTicketType)
        {
            bool result = false;

            if (LicenseSettings.LicenceRegistered == false)
            {
                var messageDialog = new CustomAlert(parentWindow)
                            .WithMessageResource("global_printing_function_disabled")
                            .ShowAlert();
                //Utils.ShowMessageBoxUnlicensedError(parentWindow, GeneralUtils.GetResourceByName("global_printing_function_disabled"));
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
                                ticketTitle = GeneralUtils.GetResourceByName("global_documentticket_type_title_tt");
                                break;
                            case TicketType.ArticleOrder:
                                ticketTitle = GeneralUtils.GetResourceByName("global_documentticket_type_title_ar");
                                break;
                            case TicketType.WorkSession:
                                ticketTitle = GeneralUtils.GetResourceByName("global_documentticket_type_title_ws");
                                break;
                            case TicketType.CashDrawer:
                                ticketTitle = GeneralUtils.GetResourceByName("global_documentticket_type_title_cs");
                                break;
                            default:
                                break;
                        }
                        var messageDialog = new CustomAlert(parentWindow)
                                                .WithMessage(ticketTitle)
                                                .ShowAlert();
                        //Utils.ShowMessageTouchTerminalWithoutAssociatedPrinter(parentWindow, ticketTitle);
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

        public static bool PrintOrderRequest(Window parentWindow, sys_configurationprinters pPrinter, OrderMain pDocumentOrderMain, fin_documentorderticket orderTicket, 
                                            string terminalDesignation, CompanyPrintingInformationsDto companyInformationsDto)
        {
            bool result = false;

            try
            {
                if (SharedPrintTicket(parentWindow, pPrinter, TicketType.TableOrder))
                {
                    var printer = MappingUtils.GetPrinterDto(pPrinter);
                    var orderTicketDto = MappingUtils.GetPrintOrderTicketDto(orderTicket);
                    OrderRequest thermalPrinterInternalDocumentOrderRequest = new OrderRequest(printer,orderTicketDto,terminalDesignation,"userTest",companyInformationsDto );
                    thermalPrinterInternalDocumentOrderRequest.Print();
                }
            }
            catch (Exception ex)
            {
                var printerDto = MappingUtils.GetPrinterDto(pPrinter);

                var messageDialog = new CustomAlert(parentWindow)
                            .WithMessage(ex.Message)
                            .ShowAlert();

                //Utils.ShowMessageTouchErrorPrintingTicket(parentWindow, printerDto, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PrintArticleRequest

        public static bool PrintArticleRequest(Window parentWindow, fin_documentorderticket orderTicket, 
            string terminalDesignation, string userName, CompanyPrintingInformationsDto companyInformationsDto)
        {
            bool result = false;

            try
            {
                //Removed: Printer is always NULL, is defined in Ticket Article
                //if (SharedPrintTicket(parentWindow, null, TicketType.ArticleOrder))
                //{
                var orderTicketDto = MappingUtils.GetPrintOrderTicketDto(orderTicket);
                result = Printing.Utility.PrintingUtils.PrintArticleRequest(orderTicketDto, terminalDesignation,userName,companyInformationsDto);
                //}
            }
            catch (Exception ex)
            {
                var messageDialog = new CustomAlert(parentWindow)
                            .WithMessage(ex.Message)
                            .ShowAlert();

                //Utils.ShowMessageTouchErrorPrintingTicket(parentWindow, null, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PrintWorkSessionMovement

        public static bool PrintWorkSessionMovement(
            Window parentWindow,
            sys_configurationprinters printerEntity,
            PrintWorkSessionDto workSessionPeriod,
            string terminalDesignation
            )
        {
            bool result = false;
            sys_configurationprinterstemplates template = XPOUtility.GetEntityById<sys_configurationprinterstemplates>(PrintingSettings.WorkSessionMovementPrintingTemplateId);

            try
            {
                if (SharedPrintTicket(parentWindow, printerEntity, TicketType.WorkSession))
                {
                    var printerDto = MappingUtils.GetPrinterDto(printerEntity);
                    string workSessionMovementPrintingFileTemplate = XPOUtility.WorkSession.GetWorkSessionMovementPrintingFileTemplate();
                    var sessionPeriodSummaryDetails = WorkSessionProcessor.GetSessionPeriodSummaryDetails(workSessionPeriod.Id);
                    result = Printing.Utility.PrintingUtils.PrintWorkSessionMovement(
                        printerDto,
                        terminalDesignation,
                        workSessionPeriod,
                        workSessionMovementPrintingFileTemplate,
                        sessionPeriodSummaryDetails);
                }
            }
            catch (Exception ex)
            {
                var printer = MappingUtils.GetPrinterDto(printerEntity);

                var messageDialog = new CustomAlert(parentWindow)
                                        .WithMessage(ex.Message)
                                        .ShowAlert();

                //Utils.ShowMessageTouchErrorPrintingTicket(parentWindow, printer, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PrintCashDrawerOpenAndMoneyInOut

        public static bool PrintCashDrawerOpenAndMoneyInOut(Window parentWindow, sys_configurationprinters pPrinter, string pTicketTitle, decimal pMovementAmount, decimal pTotalAmountInCashDrawer, string pMovementDescription,
            string terminalDesignation)
        {
            var printer = MappingUtils.GetPrinterDto(pPrinter);
            bool result = false;
            sys_configurationprinterstemplates template = XPOUtility.GetEntityById<sys_configurationprinterstemplates>(PrintingSettings.CashDrawerMoneyMovementPrintingTemplateId);

            try
            {
                if (SharedPrintTicket(parentWindow, pPrinter, TicketType.CashDrawer))
                {

                    result = Printing.Utility.PrintingUtils.PrintCashDrawerOpenAndMoneyInOut(printer, terminalDesignation, pTicketTitle, pMovementAmount, pTotalAmountInCashDrawer, pMovementDescription);
                }
            }
            catch (Exception ex)
            {
                var messageDialog = new CustomAlert(parentWindow)
                                        .WithMessage(ex.Message)
                                        .ShowAlert();

                //Utils.ShowMessageTouchErrorPrintingTicket(parentWindow, printer, ex);
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
            if (!LogicPOSSettings.UseProtectedFiles && LogicPOSSettings.ProtectedFilesIgnoreProtection) return true;

            bool result = true;

            //Check if ContainsKey and ! IsValidFile
            if (LogicPOSAppContext.ProtectedFiles.ContainsKey(pFilePath) && !LogicPOSAppContext.ProtectedFiles.IsValidFile(pFilePath))
            {
                result = false;
            }

            if (!result)
            {
                string message = string.Format(GeneralUtils.GetResourceByName("dialog_message_error_protected_files_invalid_files_detected"), pFilePath);
                if (pExtraMessage != string.Empty) message = string.Format("{1}{0}{0}{2}", Environment.NewLine, message, pExtraMessage);
                
                var messageDialog = new CustomAlert(LoginWindow.Instance)
                                        .WithMessage(message)
                                        .WithSize(new Size(800,400))
                                        .WithMessageType(MessageType.Error)
                                        .WithButtonsType(ButtonsType.Close)
                                        .WithTitleResource("global_error")
                                        .ShowAlert();
            }

            return result;
        }
    }
}
