using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.console.App;
using logicpos.financial.library.Classes.Hardware.Printers;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Enums;
using logicpos.financial.library.Classes.Reports;
using logicpos.financial.library.Classes.Reports.BOs;
using logicpos.financial.library.Classes.Reports.BOs.Documents;
using System;
using System.Collections.Generic;

namespace logicpos.financial.console.Test.Classes.HardWare.Printers.Thermal
{
    class TestThermalPrinterGeneric
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Print(sys_configurationprinters pPrinter)
        {
            try
            {
                ThermalPrinterGeneric thermalPrinterGeneric = new ThermalPrinterGeneric(pPrinter);

                List<TicketColumn> columns = new List<TicketColumn>();
                columns.Add(new TicketColumn("Code", "Code", 6, TicketColumnsAlign.Right));
                columns.Add(new TicketColumn("Designation", CustomFunctions.Res("global_designation"), 0));
                columns.Add(new TicketColumn("Quantity", "Qnt", 7, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
                columns.Add(new TicketColumn("UnitMeasure", "UM", 3));
                columns.Add(new TicketColumn("Price", "Preço", 10, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
                columns.Add(new TicketColumn("Tax", "Taxa", 7, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
                columns.Add(new TicketColumn("Discount", "Desc", 7, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
                columns.Add(new TicketColumn("Total", "Total", 11, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));

                string sql = string.Format(@"
                    SELECT 
                        Code,Designation,Quantity,UnitMeasure,Price,Vat,Discount,TotalFinal
                    FROM 
                        fin_documentfinancedetail 
                    WHERE 
                        DocumentMaster = '{0}'
                    ORDER 
                        BY Ord
                    ;",
                    SettingsApp.XpoPrintFinanceDocument
                );
                TicketTable ticketTable = new TicketTable(sql, columns, thermalPrinterGeneric.MaxCharsPerLineSmall);
                ticketTable.Print(thermalPrinterGeneric);
                //Cut
                thermalPrinterGeneric.Cut(true);
                //Print Buffer
                thermalPrinterGeneric.PrintBuffer();

                //Get Result Objects
                ResultFRBODocumentFinanceMaster FRBOHelperResponseProcessReportFinanceDocument = FRBOHelper.GetFRBOFinanceDocument(SettingsApp.XpoPrintFinanceDocument);
                List<FRBODocumentFinanceMasterView> gcDocumentFinanceMaster = FRBOHelperResponseProcessReportFinanceDocument.DocumentFinanceMaster.List;
                List<FRBODocumentFinanceDetail> gcDocumentFinanceDetail = FRBOHelperResponseProcessReportFinanceDocument.DocumentFinanceMaster.List[0].DocumentFinanceDetail;
                List<FRBODocumentFinanceMasterTotalView> gcDocumentFinanceMasterTotal = FRBOHelperResponseProcessReportFinanceDocument.DocumentFinanceMaster.List[0].DocumentFinanceMasterTotal; ;

                _log.Debug(string.Format("DocumentNumber: [{0}]", gcDocumentFinanceMaster[0].DocumentNumber));

                foreach (FRBODocumentFinanceDetail item in gcDocumentFinanceDetail)
                {
                    _log.Debug(string.Format("Designation: [{0}], Price: [{1}]", item.Designation, item.Price));
                }

                foreach (FRBODocumentFinanceMasterTotalView item in gcDocumentFinanceMasterTotal)
                {
                    _log.Debug(string.Format("Designation: [{0}], Value :[{1}]", item.Designation, item.Value));
                }

                Dictionary<string, string> CustomVars = GlobalFramework.FastReportCustomVars;
                _log.Debug(string.Format("Company_Name: [{0}]", CustomVars["Company_Name"]));

                //ThermalPrinterFinanceDocument thermalPrinterFinanceDocument = new ThermalPrinterFinanceDocument(pPrinter);
                //thermalPrinterFinanceDocument.Print();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool OpenDoor(sys_configurationprinters pPrinter)
        {
            return PrintRouter.OpenDoor(pPrinter);
        }
    }
}
