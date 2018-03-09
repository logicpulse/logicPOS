using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets;
using logicpos.financial.library.Classes.Reports;
using logicpos.printer.generic;
using logicpos.resources.Resources.Localization;
using logicpos.shared.Enums;
using logicpos.shared.Enums.ThermalPrinter;
using System;
using System.Collections.Generic;

namespace logicpos.financial.library.Classes.Hardware.Printers
{
    public class PrintRouter
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Get Printer Token 

        public static string GetPrinterToken(string pPrinterToken)
        {
            string result = pPrinterToken;

            //Check if Developer Enabled PDF Printer
            if (SettingsApp.PrintPDFEnabled)
            {
                result = "REPORT_EXPORT_PDF";
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //System Print Helper Method

        //OverLoads DocumentFinanceMaster
        private static bool SystemPrintInsert(FIN_DocumentFinanceMaster pDocumentFinanceMaster, string PrinterDesignation)
        {
            List<int> copyNames = CustomReport.CopyNames(pDocumentFinanceMaster.DocumentType.PrintCopies);
            return SystemPrintInsert(pDocumentFinanceMaster, null, PrinterDesignation, 1, copyNames, false, String.Empty, GlobalFramework.LoggedUser, GlobalFramework.LoggedTerminal);
        }

        private static bool SystemPrintInsert(FIN_DocumentFinanceMaster pDocumentFinanceMaster, string pPrinterDesignation, int pPrintCopies, List<int> pCopyNames, bool pSecondPrint, string pPrintMotive)
        {
            return SystemPrintInsert(pDocumentFinanceMaster, null, pPrinterDesignation, pPrintCopies, pCopyNames, pSecondPrint, pPrintMotive, GlobalFramework.LoggedUser, GlobalFramework.LoggedTerminal);
        }

        //OverLoads DocumentFinancePayment
        private static bool SystemPrintInsert(FIN_DocumentFinancePayment pDocumentFinancePayment, string PrinterDesignation)
        {
            List<int> copyNames = CustomReport.CopyNames(pDocumentFinancePayment.DocumentType.PrintCopies);
            return SystemPrintInsert(null, pDocumentFinancePayment, PrinterDesignation, 1, copyNames, false, String.Empty, GlobalFramework.LoggedUser, GlobalFramework.LoggedTerminal);
        }

        private static bool SystemPrintInsert(FIN_DocumentFinancePayment pDocumentFinancePayment, string pPrinterDesignation, int pPrintCopies, List<int> pCopyNames)
        {
            return SystemPrintInsert(null, pDocumentFinancePayment, pPrinterDesignation, pPrintCopies, pCopyNames, false, String.Empty, GlobalFramework.LoggedUser, GlobalFramework.LoggedTerminal);
        }

        private static bool SystemPrintInsert(FIN_DocumentFinanceMaster pDocumentFinanceMaster, FIN_DocumentFinancePayment pDocumentFinancePayment, string pPrinterDesignation, int pPrintCopies, List<int> pCopyNames, bool pSecondPrint, string pPrintMotive, SYS_UserDetail pUserDetail, POS_ConfigurationPlaceTerminal pConfigurationPlaceTerminal)
        {
            bool result = false;

            try
            {
                //Start UnitOfWork
                using (UnitOfWork uowSession = new UnitOfWork())
                {
                    string designation = String.Empty;
                    //Get Objects into Current UOW Session
                    SYS_UserDetail userDetail = (SYS_UserDetail)FrameworkUtils.GetXPGuidObject(uowSession, typeof(SYS_UserDetail), pUserDetail.Oid);
                    POS_ConfigurationPlaceTerminal configurationPlaceTerminal = (POS_ConfigurationPlaceTerminal)FrameworkUtils.GetXPGuidObject(uowSession, typeof(POS_ConfigurationPlaceTerminal), pConfigurationPlaceTerminal.Oid);

                    //Convert CopyNames List to Comma Delimited String
                    string copyNamesCommaDelimited = CustomReport.CopyNamesCommaDelimited(pCopyNames);

                    //SystemPrint
                    SYS_SystemPrint systemPrint = new SYS_SystemPrint(uowSession)
                    {
                        Date = FrameworkUtils.CurrentDateTimeAtomic(),
                        Designation = designation,
                        PrintCopies = pPrintCopies,
                        CopyNames = copyNamesCommaDelimited,
                        SecondPrint = pSecondPrint,
                        UserDetail = userDetail,
                        Terminal = configurationPlaceTerminal
                    };
                    if (pPrintMotive != String.Empty) systemPrint.PrintMotive = pPrintMotive;

                    //Mode: DocumentFinanceMaster
                    if (pDocumentFinanceMaster != null)
                    {
                        FIN_DocumentFinanceMaster documentFinanceMaster = (FIN_DocumentFinanceMaster)FrameworkUtils.GetXPGuidObject(uowSession, typeof(FIN_DocumentFinanceMaster), pDocumentFinanceMaster.Oid);
                        systemPrint.DocumentMaster = documentFinanceMaster;
                        designation = string.Format("{0} {1} : {2}", Resx.global_printed, documentFinanceMaster.DocumentType.Designation, documentFinanceMaster.DocumentNumber);
                        //Update DocumentFinanceMaster
                        if (!documentFinanceMaster.Printed) documentFinanceMaster.Printed = true;
                    }
                    //Mode: DocumentFinancePayment
                    if (pDocumentFinancePayment != null)
                    {
                        FIN_DocumentFinancePayment documentFinancePayment = (FIN_DocumentFinancePayment)FrameworkUtils.GetXPGuidObject(uowSession, typeof(FIN_DocumentFinancePayment), pDocumentFinancePayment.Oid);
                        systemPrint.DocumentPayment = documentFinancePayment;
                        designation = string.Format("{0} {1} : {2}", Resx.global_printed, documentFinancePayment.DocumentType.Designation, documentFinancePayment.PaymentRefNo);
                    }
                    systemPrint.Designation = designation;

                    try
                    {
                        //Commit UOW Changes : Before get current OrderMain
                        uowSession.CommitChanges();
                        //Audit
                        FrameworkUtils.Audit("SYSTEM_PRINT_FINANCE_DOCUMENT", designation);
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message, ex);
                        uowSession.RollbackTransaction();
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

        public static bool PrintFinanceDocument(SYS_ConfigurationPrinters pPrinter, FIN_DocumentFinanceMaster pDocumentFinanceMaster, List<int> pCopyNames, bool pSecondCopy, string pMotive)
        {
            bool result = false;

            if (pPrinter != null)
            {
                //Init Helper Vars
                bool resultSystemPrint = false;
                int printCopies = pCopyNames.Count;
                //Get Hash4Chars from Hash
                string hash4Chars = ProcessFinanceDocument.GenDocumentHash4Chars(pDocumentFinanceMaster.Hash);

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
                        //Impressora SINOCAN em ambiente WindowsLinux/USB
                        case "THERMAL_PRINTER_USB":
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
                    _log.Warn(ex.Message, ex);
                    throw new Exception(ex.Message);
                }
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Quick function to get Printer and Template and Send to Base PrintFinanceDocument, and Open Drawer if Has a Valid Payment Method

        public static bool PrintFinanceDocument(FIN_DocumentFinanceMaster pDocumentFinanceMaster)
        {
            return PrintFinanceDocument(GlobalFramework.SessionXpo, pDocumentFinanceMaster);
        }

        public static bool PrintFinanceDocument(Session pSession, FIN_DocumentFinanceMaster pDocumentFinanceMaster)
        {
            List<int> printCopies = new List<int>();
            for (int i = 0; i < pDocumentFinanceMaster.DocumentType.PrintCopies; i++)
            {
                printCopies.Add(i);
            }

            return PrintFinanceDocument(pSession, pDocumentFinanceMaster, printCopies, false, String.Empty);
        }

        public static bool PrintFinanceDocument(Session pSession, FIN_DocumentFinanceMaster pDocumentFinanceMaster, List<int> pCopyNames, bool pSecondCopy, string pMotive)
        {
            bool result = false;

            try
            {
                //Finish Payment with Print Job + Open Drawer (If Not TableConsult)
                FIN_DocumentFinanceYearSerieTerminal xDocumentFinanceYearSerieTerminal = ProcessFinanceDocumentSeries.GetDocumentFinanceYearSerieTerminal(pSession, pDocumentFinanceMaster.DocumentType.Oid);
                PrintFinanceDocument(GlobalFramework.LoggedTerminal.Printer, pDocumentFinanceMaster, pCopyNames, pSecondCopy, pMotive);

                //Open Door if Has Valid Payment
                if (pDocumentFinanceMaster.PaymentMethod != null)
                {
                    OpenDoor(GlobalFramework.LoggedTerminal.Printer);
                }
                result = true;
            }
            catch (Exception ex)
            {
                _log.Warn(ex.Message, ex);
                throw new Exception(ex.Message);
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public static bool PrintFinanceDocumentPayment(SYS_ConfigurationPrinters pPrinter, FIN_DocumentFinancePayment pDocumentFinancePayment)
        {
            bool result = false;

            if (pPrinter != null)
            {
                //Init Helper Vars
                bool resultSystemPrint = false;
                //Initialize CopyNames List from PrintCopies
                List<int> copyNames = CustomReport.CopyNames(pDocumentFinancePayment.DocumentType.PrintCopies);
                int printCopies = copyNames.Count;

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
                        //Impressora SINOCAN em ambiente WindowsLinux/USB
                        case "THERMAL_PRINTER_USB":
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
                    _log.Warn(ex.Message, ex);
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public static bool PrintWorkSessionMovement(SYS_ConfigurationPrinters pPrinter, POS_WorkSessionPeriod pWorkSessionPeriod)
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
                        //Impressora SINOCAN em ambiente WindowsLinux/USB
                        case "THERMAL_PRINTER_USB":
                            //NonCurrentAcount
                            ThermalPrinterInternalDocumentWorkSession thermalPrinterInternalDocumentWorkSession = new ThermalPrinterInternalDocumentWorkSession(pPrinter, pWorkSessionPeriod, SplitCurrentAccountMode.NonCurrentAcount);
                            thermalPrinterInternalDocumentWorkSession.Print();
                            //CurrentAcount
                            thermalPrinterInternalDocumentWorkSession = new ThermalPrinterInternalDocumentWorkSession(pPrinter, pWorkSessionPeriod, SplitCurrentAccountMode.CurrentAcount);
                            thermalPrinterInternalDocumentWorkSession.Print();
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
                    _log.Warn(ex.Message, ex);
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public static bool PrintArticleRequest(FIN_DocumentOrderTicket pOrderTicket)
        {
            bool result = false;

            try
            {
                //Initialize printerArticleQueue to Store Articles > Printer Queue
                List<SYS_ConfigurationPrinters> printerArticles = new List<SYS_ConfigurationPrinters>();
                foreach (FIN_DocumentOrderDetail item in pOrderTicket.OrderDetail)
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
                    foreach (SYS_ConfigurationPrinters item in printerArticles)
                    {
                        ThermalPrinterInternalDocumentOrderRequest thermalPrinterInternalDocumentOrderRequest = new ThermalPrinterInternalDocumentOrderRequest(item, pOrderTicket, true);
                        thermalPrinterInternalDocumentOrderRequest.Print();
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                _log.Warn(ex.Message, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Used for Money Movements and Open/Close Terminal/Day Sessions

        public static bool PrintCashDrawerOpenAndMoneyInOut(SYS_ConfigurationPrinters pPrinter, String pTicketTitle, decimal pMovementAmount, decimal pTotalAmountInCashDrawer, string pMovementDescription)
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
                        //Impressora SINOCAN em ambiente WindowsLinux/USB
                        case "THERMAL_PRINTER_USB":
                            ThermalPrinterInternalDocumentCashDrawer thermalPrinterInternalDocumentCashDrawer = new ThermalPrinterInternalDocumentCashDrawer(pPrinter, pTicketTitle, pTotalAmountInCashDrawer, pMovementAmount, pMovementDescription);
                            thermalPrinterInternalDocumentCashDrawer.Print();
                            break;
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    _log.Warn(ex.Message, ex);
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public static bool OpenDoor(SYS_ConfigurationPrinters pPrinter)
        {
            bool result = false;
            bool hasPermission = FrameworkUtils.HasPermissionTo("HARDWARE_DRAWER_OPEN");

            if (pPrinter != null && hasPermission)
            {
                try
                {
                    switch (pPrinter.PrinterType.Token)
                    {
                        //Impressora SINOCAN em ambiente Windows
                        case "THERMAL_PRINTER_WINDOWS":
                        //Impressora SINOCAN em ambiente Linux
                        case "THERMAL_PRINTER_LINUX":
                        //Impressora SINOCAN em ambiente WindowsLinux/Socket
                        case "THERMAL_PRINTER_SOCKET":
                        //Impressora SINOCAN em ambiente WindowsLinux/USB
                        case "THERMAL_PRINTER_USB":
                            PrintObject printObjectSINOCAN = new PrintObject(0);

                            int m = Convert.ToInt32(GlobalFramework.Settings["DoorValueM"]);
                            int t1 = Convert.ToInt32(GlobalFramework.Settings["DoorValueT1"]);
                            int t2 = Convert.ToInt32(GlobalFramework.Settings["DoorValueT2"]);

                            printObjectSINOCAN.OpenDoor(pPrinter.PrinterType.Token, pPrinter.NetworkName, m, t1, t2);

                            //Audit
                            FrameworkUtils.Audit("CASHDRAWER_OPEN", Resx.audit_message_cashdrawer_open);

                            break;
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    _log.Warn(ex.Message, ex);
                }
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    }
}
