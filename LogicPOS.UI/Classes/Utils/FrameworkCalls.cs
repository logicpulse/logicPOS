using Gtk;
using LogicPOS.Api.Features.Reports.WorkSession.Common;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.DTOs.Printing;
using LogicPOS.Printing.Documents;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI
{
    internal class FrameworkCalls
    {
        //Log4Net
        protected static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Size _sizeDefaultWindowSize = new Size(600, 400);


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

        public static bool PrintFinanceDocument(Window parentWindow, fin_documentfinancemaster pDocumentFinanceMaster)
        {
            return PrintFinanceDocument(parentWindow, pDocumentFinanceMaster);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PrintFinanceDocumentPayment

        public static bool PrintFinanceDocumentPayment(Window parentWindow, fin_documentfinancepayment pDocumentFinancePayment)
        {
            return PrintFinanceDocumentPayment(parentWindow, pDocumentFinancePayment);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

   
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
                result = Printing.Utility.PrintingUtils.PrintArticleRequest(orderTicketDto, terminalDesignation, userName, companyInformationsDto);
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
                                        .WithSize(new Size(800, 400))
                                        .WithMessageType(MessageType.Error)
                                        .WithButtonsType(ButtonsType.Close)
                                        .WithTitleResource("global_error")
                                        .ShowAlert();
            }

            return result;
        }
    }
}
