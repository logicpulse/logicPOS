using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets;
using logicpos.financial.library.Classes.Reports;
using logicpos.printer.generic;
using logicpos.shared.Enums;
using logicpos.shared.Enums.ThermalPrinter;
using System;
using System.Collections.Generic;
using logicpos.datalayer.Xpo;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using LogicPOS.Settings;

namespace logicpos.financial.library.Classes.Hardware.Printers
{
    public class PrintRouter
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Get Printer Token 

        public static string GetPrinterToken(string pPrinterToken)
        {
            string result = pPrinterToken;

            //Check if Developer Enabled PDF Printer
            if (PrintingSettings.PrintPDFEnabled)
            {
                result = "REPORT_EXPORT_PDF";
            }

            return result;
        }

        private static bool SystemPrintInsert(fin_documentfinancemaster pDocumentFinanceMaster, string pPrinterDesignation, int pPrintCopies, List<int> pCopyNames, bool pSecondPrint, string pPrintMotive)
        {
            return SystemPrintInsert(pDocumentFinanceMaster, null, pPrinterDesignation, pPrintCopies, pCopyNames, pSecondPrint, pPrintMotive, XPOSettings.LoggedUser, XPOSettings.LoggedTerminal);
        }

        private static bool SystemPrintInsert(fin_documentfinancepayment pDocumentFinancePayment, string pPrinterDesignation, int pPrintCopies, List<int> pCopyNames)
        {
            return SystemPrintInsert(null, pDocumentFinancePayment, pPrinterDesignation, pPrintCopies, pCopyNames, false, string.Empty, XPOSettings.LoggedUser, XPOSettings.LoggedTerminal);
        }

        private static bool SystemPrintInsert(fin_documentfinancemaster pDocumentFinanceMaster, fin_documentfinancepayment pDocumentFinancePayment, string pPrinterDesignation, int pPrintCopies, List<int> pCopyNames, bool pSecondPrint, string pPrintMotive, sys_userdetail pUserDetail, pos_configurationplaceterminal pConfigurationPlaceTerminal)
        {
            bool result = false;

            try
            {
                //Start UnitOfWork
                using (UnitOfWork uowSession = new UnitOfWork())
                {
                    string designation = string.Empty;
                    //Get Objects into Current UOW Session
                    sys_userdetail userDetail = (sys_userdetail)XPOHelper.GetXPGuidObject(uowSession, typeof(sys_userdetail), pUserDetail.Oid);
                    pos_configurationplaceterminal configurationPlaceTerminal = (pos_configurationplaceterminal)XPOHelper.GetXPGuidObject(uowSession, typeof(pos_configurationplaceterminal), pConfigurationPlaceTerminal.Oid);

                    //Convert CopyNames List to Comma Delimited String
                    string copyNamesCommaDelimited = CustomReport.CopyNamesCommaDelimited(pCopyNames);

                    //SystemPrint
                    sys_systemprint systemPrint = new sys_systemprint(uowSession)
                    {
                        Date = XPOHelper.CurrentDateTimeAtomic(),
                        Designation = designation,
                        PrintCopies = pPrintCopies,
                        CopyNames = copyNamesCommaDelimited,
                        SecondPrint = pSecondPrint,
                        UserDetail = userDetail,
                        Terminal = configurationPlaceTerminal
                    };
                    if (pPrintMotive != string.Empty) systemPrint.PrintMotive = pPrintMotive;

                    //Mode: DocumentFinanceMaster
                    if (pDocumentFinanceMaster != null)
                    {
                        fin_documentfinancemaster documentFinanceMaster = (fin_documentfinancemaster)XPOHelper.GetXPGuidObject(uowSession, typeof(fin_documentfinancemaster), pDocumentFinanceMaster.Oid);
                        systemPrint.DocumentMaster = documentFinanceMaster;
                        designation = string.Format("{0} {1} : {2}", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_printed"), documentFinanceMaster.DocumentType.Designation, documentFinanceMaster.DocumentNumber);
                        //Update DocumentFinanceMaster
                        if (!documentFinanceMaster.Printed) documentFinanceMaster.Printed = true;
                    }
                    //Mode: DocumentFinancePayment
                    if (pDocumentFinancePayment != null)
                    {
                        fin_documentfinancepayment documentFinancePayment = (fin_documentfinancepayment)XPOHelper.GetXPGuidObject(uowSession, typeof(fin_documentfinancepayment), pDocumentFinancePayment.Oid);
                        systemPrint.DocumentPayment = documentFinancePayment;
                        designation = string.Format("{0} {1} : {2}", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_printed"), documentFinancePayment.DocumentType.Designation, documentFinancePayment.PaymentRefNo);
                    }
                    systemPrint.Designation = designation;

                    try
                    {
                        //Commit UOW Changes : Before get current OrderMain
                        uowSession.CommitChanges();
                        //Audit
                       XPOHelper.Audit("SYSTEM_PRINT_FINANCE_DOCUMENT", designation);
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message, ex);
                        uowSession.RollbackTransaction();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public static bool PrintFinanceDocument(sys_configurationprinters pPrinter, fin_documentfinancemaster pDocumentFinanceMaster, List<int> pCopyNames, bool pSecondCopy, string pMotive)
        {
            bool result = false;

            if (pPrinter != null)
            {
                int printCopies = pCopyNames.Count;
                //Get Hash4Chars from Hash
                string hash4Chars = FinanceDocumentProcessingUtils.GenDocumentHash4Chars(pDocumentFinanceMaster.Hash);

                try
                {
                    //Init Helper Vars
                    bool resultSystemPrint;
                    switch (GetPrinterToken(pPrinter.PrinterType.Token))
                    {
                        //Impressora SINOCAN em ambiente Windows
                        case "THERMAL_PRINTER_WINDOWS":
                        //Impressora SINOCAN em ambiente Linux
                        //Impressora SINOCAN em ambiente WindowsLinux/Socket
                        case "THERMAL_PRINTER_SOCKET":
                            ThermalPrinterFinanceDocumentMaster thermalPrinterFinanceDocument = new ThermalPrinterFinanceDocumentMaster(pPrinter, pDocumentFinanceMaster, pCopyNames, pSecondCopy, pMotive);
                            thermalPrinterFinanceDocument.Print();
                            //Add to SystemPrint Audit
                            resultSystemPrint = SystemPrintInsert(pDocumentFinanceMaster, pPrinter.Designation, printCopies, pCopyNames, pSecondCopy, pMotive);
                            break;
                        case "GENERIC_PRINTER_WINDOWS":
                            CustomReport.ProcessReportFinanceDocument(CustomReportDisplayMode.Print, pDocumentFinanceMaster.Oid, hash4Chars, pCopyNames, pSecondCopy, pMotive);
                            //Add to SystemPrint Audit
                            resultSystemPrint = SystemPrintInsert(pDocumentFinanceMaster, pPrinter.Designation, printCopies, pCopyNames, pSecondCopy, pMotive);
                            break;
                        case "REPORT_EXPORT_PDF":
                            CustomReport.ProcessReportFinanceDocument(CustomReportDisplayMode.ExportPDF, pDocumentFinanceMaster.Oid, hash4Chars, pCopyNames, pSecondCopy, pMotive);
                            //Add to SystemPrint Audit : Developer : Use here Only to Test SystemPrintInsert
                            resultSystemPrint = SystemPrintInsert(pDocumentFinanceMaster, pPrinter.Designation, printCopies, pCopyNames, pSecondCopy, pMotive);
                            break;
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex.Message, ex);
                    throw new Exception(ex.Message);
                }
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Quick function to get Printer and Template and Send to Base PrintFinanceDocument, and Open Drawer if Has a Valid Payment Method

        public static bool PrintFinanceDocument(fin_documentfinancemaster pDocumentFinanceMaster)
        {
            return PrintFinanceDocument(XPOSettings.Session, pDocumentFinanceMaster);
        }

        public static bool PrintFinanceDocument(Session pSession, fin_documentfinancemaster pDocumentFinanceMaster)
        {
            List<int> printCopies = new List<int>();
            for (int i = 0; i < pDocumentFinanceMaster.DocumentType.PrintCopies; i++)
            {
                printCopies.Add(i);
            }

            return PrintFinanceDocument(pSession, pDocumentFinanceMaster, printCopies, false, string.Empty);
        }

        public static bool PrintFinanceDocument(Session pSession, fin_documentfinancemaster pDocumentFinanceMaster, List<int> pCopyNames, bool pSecondCopy, string pMotive)
        {
            bool result;
            try
            {
                //Finish Payment with Print Job + Open Drawer (If Not TableConsult)
                fin_documentfinanceyearserieterminal xDocumentFinanceYearSerieTerminal = ProcessFinanceDocumentSeries.GetDocumentFinanceYearSerieTerminal(pSession, pDocumentFinanceMaster.DocumentType.Oid);
                PrintFinanceDocument(XPOSettings.LoggedTerminal.Printer, pDocumentFinanceMaster, pCopyNames, pSecondCopy, pMotive);

                //Open Door if Has Valid Payment
                if (pDocumentFinanceMaster.PaymentMethod != null)
                {
                    OpenDoor(XPOSettings.LoggedTerminal.ThermalPrinter);
                }
                result = true;
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message, ex);
                throw new Exception(ex.Message);
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public static bool PrintFinanceDocumentPayment(sys_configurationprinters pPrinter, fin_documentfinancepayment pDocumentFinancePayment)
        {
            bool result = false;

            if (pPrinter != null)
            {
                //Initialize CopyNames List from PrintCopies
                List<int> copyNames = CustomReport.CopyNames(pDocumentFinancePayment.DocumentType.PrintCopies);
                int printCopies = copyNames.Count;

                try
                {
                    //Init Helper Vars
                    bool resultSystemPrint;
                    switch (GetPrinterToken(pPrinter.PrinterType.Token))
                    {
                        //Impressora SINOCAN em ambiente Windows
                        case "THERMAL_PRINTER_WINDOWS":
                        //Impressora SINOCAN em ambiente Linux
                        case "THERMAL_PRINTER_LINUX":
                        //Impressora SINOCAN em ambiente WindowsLinux/Socket
                        case "THERMAL_PRINTER_SOCKET":
                            ThermalPrinterFinanceDocumentPayment thermalPrinterFinanceDocumentPayment = new ThermalPrinterFinanceDocumentPayment(pPrinter, pDocumentFinancePayment, copyNames, false);
                            thermalPrinterFinanceDocumentPayment.Print();
                            //Add to SystemPrint Audit
                            resultSystemPrint = SystemPrintInsert(pDocumentFinancePayment, pPrinter.Designation, printCopies, copyNames);
                            break;
                        case "GENERIC_PRINTER_WINDOWS":
                            CustomReport.ProcessReportFinanceDocumentPayment(CustomReportDisplayMode.Print, pDocumentFinancePayment.Oid, copyNames);
                            //Add to SystemPrint Audit
                            resultSystemPrint = SystemPrintInsert(pDocumentFinancePayment, pPrinter.Designation, printCopies, copyNames);
                            break;
                        case "REPORT_EXPORT_PDF":
                            CustomReport.ProcessReportFinanceDocumentPayment(CustomReportDisplayMode.ExportPDF, pDocumentFinancePayment.Oid, copyNames);
                            //Add to SystemPrint Audit : Developer : Use here Only to Test SystemPrintInsert
                            resultSystemPrint = SystemPrintInsert(pDocumentFinancePayment, pPrinter.Designation, printCopies, copyNames);
                            break;
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex.Message, ex);
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public static bool PrintWorkSessionMovement(sys_configurationprinters pPrinter, pos_worksessionperiod pWorkSessionPeriod)
        {
            bool result = false;

            if (pPrinter != null)
            {
                try
                {
                    switch (GetPrinterToken(pPrinter.PrinterType.Token))
                    {
                        //Impressora SINOCAN em ambiente Windows
                        case "THERMAL_PRINTER_WINDOWS":
                        //Impressora SINOCAN em ambiente Linux
                        case "THERMAL_PRINTER_LINUX":
                        //Impressora SINOCAN em ambiente WindowsLinux/Socket
                        case "THERMAL_PRINTER_SOCKET":
                            //NonCurrentAcount
                            ThermalPrinterInternalDocumentWorkSession thermalPrinterInternalDocumentWorkSession = new ThermalPrinterInternalDocumentWorkSession(pPrinter, pWorkSessionPeriod, SplitCurrentAccountMode.NonCurrentAcount);
                            thermalPrinterInternalDocumentWorkSession.Print();
                            //CurrentAcount
                            //Use Config to print this
                            if(Convert.ToBoolean(LogicPOS.Settings.GeneralSettings.PreferenceParameters["USE_CC_DAILY_TICKET"]))
                            {
                                thermalPrinterInternalDocumentWorkSession = new ThermalPrinterInternalDocumentWorkSession(pPrinter, pWorkSessionPeriod, SplitCurrentAccountMode.CurrentAcount);
                                thermalPrinterInternalDocumentWorkSession.Print();
                            }                            
                            break;
                        case "GENERIC_PRINTER_WINDOWS":
                            break;
                        case "REPORT_EXPORT_PDF":
                            break;
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex.Message, ex);
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public static bool PrintArticleRequest(fin_documentorderticket pOrderTicket)
        {
            bool result;
            try
            {
                //Initialize printerArticleQueue to Store Articles > Printer Queue
                List<sys_configurationprinters> printerArticles = new List<sys_configurationprinters>();
                foreach (fin_documentorderdetail item in pOrderTicket.OrderDetail)
                {
                    if (item.Article.Printer != null && item.Article.Printer.PrinterType.ThermalPrinter)
                    {
                        //Add Printer
                        if (!printerArticles.Contains(item.Article.Printer)) printerArticles.Add(item.Article.Printer);
                    }
                }

                //Print Tickets for Article Printers
                if (printerArticles.Count > 0)
                {
                    foreach (sys_configurationprinters item in printerArticles)
                    {
                        ThermalPrinterInternalDocumentOrderRequest thermalPrinterInternalDocumentOrderRequest = new ThermalPrinterInternalDocumentOrderRequest(item, pOrderTicket, true);
                        thermalPrinterInternalDocumentOrderRequest.Print();
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Used for Money Movements and Open/Close Terminal/Day Sessions

        public static bool PrintCashDrawerOpenAndMoneyInOut(sys_configurationprinters pPrinter, string pTicketTitle, decimal pMovementAmount, decimal pTotalAmountInCashDrawer, string pMovementDescription)
        {
            bool result = false;

            if (pPrinter != null)
            {
                try
                {
                    switch (GetPrinterToken(pPrinter.PrinterType.Token))
                    {
                        //Impressora SINOCAN em ambiente Windows
                        case "THERMAL_PRINTER_WINDOWS":
                        //Impressora SINOCAN em ambiente Linux
                        case "THERMAL_PRINTER_LINUX":
                        //Impressora SINOCAN em ambiente WindowsLinux/Socket
                        case "THERMAL_PRINTER_SOCKET":
                            ThermalPrinterInternalDocumentCashDrawer thermalPrinterInternalDocumentCashDrawer = new ThermalPrinterInternalDocumentCashDrawer(pPrinter, pTicketTitle, pTotalAmountInCashDrawer, pMovementAmount, pMovementDescription);
                            thermalPrinterInternalDocumentCashDrawer.Print();
                            break;
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex.Message, ex);
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public static bool OpenDoor(sys_configurationprinters pPrinter)
        {
            try
            {
                bool result = false;
                if (XPOSettings.LoggedTerminal.ThermalPrinter != null)
                {                    
                    bool hasPermission = GeneralSettings.HasPermissionTo("HARDWARE_DRAWER_OPEN");
                    int m = XPOSettings.LoggedTerminal.ThermalPrinter.ThermalOpenDrawerValueM;
                    int t1 = XPOSettings.LoggedTerminal.ThermalPrinter.ThermalOpenDrawerValueT1;
                    int t2 = XPOSettings.LoggedTerminal.ThermalPrinter.ThermalOpenDrawerValueT2;
                    PrintObject printObjectSINOCAN = new PrintObject(0);
                    if (XPOSettings.LoggedTerminal.ThermalPrinter != null && hasPermission)
                    {
                        try
                        {
                            switch (XPOSettings.LoggedTerminal.ThermalPrinter.PrinterType.Token)
                            {
                                //Impressora SINOCAN em ambiente Windows
                                case "THERMAL_PRINTER_WINDOWS":
                                    printObjectSINOCAN.OpenDoor(XPOSettings.LoggedTerminal.ThermalPrinter.PrinterType.Token, XPOSettings.LoggedTerminal.ThermalPrinter.Designation, m, t1, t2);
                                    break;
                                //Impressora SINOCAN em ambiente Linux
                                case "THERMAL_PRINTER_SOCKET":
                                    // Deprecated
                                    //int m = Convert.ToInt32(LogicPOS.Settings.GeneralSettings.Settings["DoorValueM"]);
                                    //int t1 = Convert.ToInt32(LogicPOS.Settings.GeneralSettings.Settings["DoorValueT1"]);
                                    //int t2 = Convert.ToInt32(LogicPOS.Settings.GeneralSettings.Settings["DoorValueT2"]);
                                    // Open Drawer
                                    //TK016249 - Impressoras - Diferenciação entre Tipos
                                    printObjectSINOCAN.OpenDoor(XPOSettings.LoggedTerminal.ThermalPrinter.PrinterType.Token, XPOSettings.LoggedTerminal.ThermalPrinter.NetworkName, m, t1, t2);
                                    //Audit
                                   XPOHelper.Audit("CASHDRAWER_OPEN", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "audit_message_cashdrawer_open"));

                                    break;
                            }

                            result = true;
                        }
                        catch (Exception ex)
                        {
                            _logger.Warn(ex.Message, ex);
                        }
                    }

                    return result;
                }

                return result;

            }
            catch(Exception ex)
            {
                _logger.Error("Error open cash drawer :" + ex.Message);
                return false;
            }
            
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    }
}
