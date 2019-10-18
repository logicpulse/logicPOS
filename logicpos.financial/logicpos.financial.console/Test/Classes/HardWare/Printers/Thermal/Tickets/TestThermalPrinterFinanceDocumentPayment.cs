using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.console.App;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets;
using System;
using System.Collections.Generic;

namespace logicpos.financial.console.Test.Classes.HardWare.Printer
{
    class TestThermalPrinterFinanceDocumentPayment
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Print(sys_configurationprinters pPrinter)
        {
            try
            {
                //Parameters
                fin_documentfinancepayment documentFinancePayment = (fin_documentfinancepayment)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancepayment), SettingsApp.XpoPrintFinanceDocumentPayment);
                //Print Document
                if (documentFinancePayment != null)
                {
                    List<int> copyNames = new List<int> { 0/*, 1*/ };
                    //Test Print Document
                    ThermalPrinterFinanceDocumentPayment thermalPrinterFinanceDocumentPayment = new ThermalPrinterFinanceDocumentPayment(pPrinter, documentFinancePayment, copyNames, true);
                    thermalPrinterFinanceDocumentPayment.Print();
                    Console.WriteLine(string.Format("DocumentFinancePayment Printed: {0}", documentFinancePayment.PaymentRefNo));
                }
                else
                {
                    Console.WriteLine(string.Format("ERROR: DocumentFinancePayment NULL: {0}", SettingsApp.XpoPrintFinanceDocumentPayment));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
