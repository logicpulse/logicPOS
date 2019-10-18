using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.console.App;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets;
using System;
using System.Collections.Generic;

namespace logicpos.financial.console.Test.Classes.HardWare.Printer
{
    class TestThermalPrinterInternalDocumentOrderRequest
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Print(sys_configurationprinters pPrinter)
        {
            try
            {
                fin_documentorderticket orderTicket = (fin_documentorderticket)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentorderticket), SettingsApp.XpoPrintDocumentOrderTicket);

                //Print Document
                if (orderTicket != null)
                {
                    //Test Print Document
                    ThermalPrinterInternalDocumentOrderRequest thermalPrinterInternalDocumentOrderRequest = new ThermalPrinterInternalDocumentOrderRequest(pPrinter, orderTicket);
                    thermalPrinterInternalDocumentOrderRequest.Print();
                    Console.WriteLine(string.Format("ThermalPrinterInternalDocumentOrderRequest Printed: {0}", orderTicket.Oid));

                    //Call PrintArticlePrinters
                    PrintArticlePrinters(orderTicket);
                }
                else
                {
                    Console.WriteLine(string.Format("ERROR: ThermalPrinterInternalDocumentOrderRequest NULL: {0}", orderTicket.Oid));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void PrintArticlePrinters(fin_documentorderticket pOrderTicket)
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
                    Console.WriteLine(string.Format("ThermalPrinterInternalDocumentOrderRequest Printed for {1}: {0}", pOrderTicket.Oid, item.Designation));
                }
            }
        }
    }
}
