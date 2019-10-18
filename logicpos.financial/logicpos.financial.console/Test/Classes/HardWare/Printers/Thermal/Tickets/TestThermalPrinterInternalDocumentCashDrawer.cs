using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets;
using System;

namespace logicpos.financial.console.Test.Classes.HardWare.Printer
{
    class TestThermalPrinterInternalDocumentCashDrawer
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Print(sys_configurationprinters pPrinter)
        {
            try
            {
                //Test Print Document
                string ticketTitle = "Saida de Caixa";
                decimal totalAmountInCashDrawer = 1280.28m;
                decimal movementAmount = 28.82m;
                string movementDescription = "Pagar Fornecedor";
                ThermalPrinterInternalDocumentCashDrawer thermalPrinterInternalDocumentCashDrawer = new ThermalPrinterInternalDocumentCashDrawer(pPrinter, ticketTitle, totalAmountInCashDrawer, movementAmount, movementDescription);
                thermalPrinterInternalDocumentCashDrawer.Print();
                Console.WriteLine(string.Format("ThermalPrinterInternalDocumentCashDrawer Printed: {0}", ticketTitle));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
