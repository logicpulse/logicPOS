using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.console.App;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets;
using System;
using System.Collections.Generic;

namespace logicpos.financial.console.Test.Classes.HardWare.Printer
{
    class TestThermalPrinterFinanceDocumentMaster
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Print(sys_configurationprinters pPrinter)
        {
            try
            {
                //Parameters
                fin_documentfinancemaster documentFinanceMaster = (fin_documentfinancemaster)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancemaster), SettingsApp.XpoPrintFinanceDocument);
                //Print Document
                if (documentFinanceMaster != null)
                {
                    //Test Print Document
                    List<int> copyNames = new List<int> { 0, /*1, 2, 3*/ };
                    ThermalPrinterFinanceDocumentMaster thermalPrinterFinanceDocument = new ThermalPrinterFinanceDocumentMaster(pPrinter, documentFinanceMaster, copyNames, true, "Cancelado");
                    thermalPrinterFinanceDocument.Print();
                    Console.WriteLine(string.Format("DocumentFinanceMaster Printed: {0}", documentFinanceMaster.DocumentNumber));
                }
                else
                {
                    Console.WriteLine(string.Format("ERROR: DocumentFinanceMaster NULL: {0}", SettingsApp.XpoPrintFinanceDocument));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
