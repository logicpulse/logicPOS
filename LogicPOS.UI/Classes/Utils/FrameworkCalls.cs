using Gtk;
using logicpos.Classes.Enums.Tickets;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.DTOs.Printing;
using LogicPOS.Printing.Documents;
using LogicPOS.Settings;
using LogicPOS.Shared.Orders;
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

        //Shared Method to call all other PrintTicketMethods to Check Licence and other Protections
        public static bool SharedPrintTicket(Window parentWindow, Api.Entities.Printer printer, TicketType pTicketType)
        {
            bool result = false;

            if (LicenseSettings.LicenceRegistered == false)
            {
                var messageDialog = new CustomAlert(parentWindow)
                            .WithMessageResource("global_printing_function_disabled")
                            .ShowAlert();

            }
            else
            {
                try
                {
                    //Notification : Show Message TouchTerminalWithoutAssociatedPrinter and Store user input, to Show Next Time(Yes) or Not (No)
                    if (printer == null)
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

        public static bool PrintOrderRequest(Window parentWindow,
                                             Api.Entities.Printer printerEntity,
                                             OrderMain pDocumentOrderMain,
                                             fin_documentorderticket orderTicket,
                                             string terminalDesignation,
                                             CompanyPrintingInformationsDto companyInformationsDto)
        {
            bool result = false;

            var printerDto = new PrinterDto
            {
                Id = printerEntity.Id,
                Designation = printerEntity.Designation,
                NetworkName = printerEntity.NetworkName,
                Token = printerEntity.Type.Token,
                IsThermal = printerEntity.Type.ThermalPrinter
            };

            try
            {
                if (SharedPrintTicket(parentWindow, printerEntity, TicketType.TableOrder))
                {


                    var orderTicketDto = MappingUtils.GetPrintOrderTicketDto(orderTicket);

                    OrderRequest thermalPrinterInternalDocumentOrderRequest = new OrderRequest(printerDto,
                                                                                               orderTicketDto,
                                                                                               terminalDesignation,
                                                                                               "userTest",
                                                                                               companyInformationsDto);
                    thermalPrinterInternalDocumentOrderRequest.Print();
                }
            }
            catch (Exception ex)
            {
                CustomAlerts.ShowErrorPrintingTicketAlert(parentWindow,
                                                          printerEntity.Designation,
                                                          printerEntity.NetworkName,
                                                          ex.Message);
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
        //PrintWorkSessionMovement

        public static bool PrintWorkSessionMovement(Window parentWindow,
                                                    Api.Entities.Printer printerEntity,
                                                    PrintWorkSessionDto workSessionPeriod,
                                                    string terminalDesignation)
        {
            bool result = false;
            sys_configurationprinterstemplates template = XPOUtility.GetEntityById<sys_configurationprinterstemplates>(PrintingSettings.WorkSessionMovementPrintingTemplateId);

            try
            {
                if (SharedPrintTicket(parentWindow, printerEntity, TicketType.WorkSession))
                {
                    var printerDto = new PrinterDto
                    {
                        Id = printerEntity.Id,
                        Designation = printerEntity.Designation,
                        NetworkName = printerEntity.NetworkName,
                        Token = printerEntity.Type.Token,
                        IsThermal = printerEntity.Type.ThermalPrinter
                    };

                    string workSessionMovementPrintingFileTemplate = XPOUtility.WorkSession.GetWorkSessionMovementPrintingFileTemplate();
                    var sessionPeriodSummaryDetails = WorkSessionProcessor.GetSessionPeriodSummaryDetails(workSessionPeriod.Id);

                    result = Printing.Utility.PrintingUtils.PrintWorkSessionMovement(printerDto,
                                                                                     terminalDesignation,
                                                                                     workSessionPeriod,
                                                                                     workSessionMovementPrintingFileTemplate,
                                                                                     sessionPeriodSummaryDetails);
                }
            }
            catch (Exception ex)
            {
                CustomAlerts.ShowErrorPrintingTicketAlert(parentWindow,
                                                          printerEntity.Designation,
                                                          printerEntity.NetworkName,
                                                          ex.Message);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PrintCashDrawerOpenAndMoneyInOut

        public static bool PrintCashDrawerOpenAndMoneyInOut(Window parentWindow,
                                                            Api.Entities.Printer printerEntity,
                                                            string pTicketTitle,
                                                            decimal pMovementAmount,
                                                            decimal pTotalAmountInCashDrawer,
                                                            string pMovementDescription,
                                                            string terminalDesignation)
        {
            bool result = false;

            try
            {
                if (SharedPrintTicket(parentWindow, printerEntity, TicketType.CashDrawer))
                {
                    var printerDto = new PrinterDto
                    {
                        Id = printerEntity.Id,
                        Designation = printerEntity.Designation,
                        NetworkName = printerEntity.NetworkName,
                        Token = printerEntity.Type.Token,
                        IsThermal = printerEntity.Type.ThermalPrinter
                    };

                    result = Printing.Utility.PrintingUtils.PrintCashDrawerOpenAndMoneyInOut(printerDto,
                                                                                             terminalDesignation,
                                                                                             pTicketTitle,
                                                                                             pMovementAmount,
                                                                                             pTotalAmountInCashDrawer,
                                                                                             pMovementDescription);
                }
            }
            catch (Exception ex)
            {
                CustomAlerts.ShowErrorPrintingTicketAlert(parentWindow,
                                                           printerEntity.Designation,
                                                           printerEntity.NetworkName,
                                                           ex.Message);
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
