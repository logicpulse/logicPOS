using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.console.App;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets;
using logicpos.shared.Enums.ThermalPrinter;
using System;

namespace logicpos.financial.console.Test.Classes.HardWare.Printer
{
    class TestThermalPrinterInternalDocumentWorkSession
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Print(sys_configurationprinters pPrinter)
        {
            try
            {
                //Parameters
                pos_worksessionperiod workSessionPeriod = (pos_worksessionperiod)GlobalFramework.SessionXpo.GetObjectByKey(typeof(pos_worksessionperiod), SettingsApp.XpoPrintWorkSessionPeriod);

                //Print WorkSession
                if (workSessionPeriod != null)
                {
                    //Get Filter Field : To filter Day or Terminal
                    string periodField = (workSessionPeriod.PeriodType == WorkSessionPeriodType.Day) ? "wspPeriodParent" : "wspPeriod";

                    string sqlShared = @"
                        SELECT 
                            COUNT(*) as Count
                        FROM 
                            view_worksessionmovement 
                        WHERE
                            (dfmDocument IS NOT NULL AND wmtMovementTypeToken = 'FINANCE_DOCUMENT')  
                            AND {0} = '{1}' 
                            AND wsmDocumentFinanceType {2} '{3}';
                    ";

                    //Generate Queries
                    string sqlNonCurrentAccount = string.Format(sqlShared, periodField, workSessionPeriod.Oid, "<>", SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput);
                    string sqlCurrentAccount = string.Format(sqlShared, periodField, workSessionPeriod.Oid, "=", SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput);
                    //Get Queries Results
                    object totRecsNonCurrentAccount = GlobalFramework.SessionXpo.ExecuteScalar(sqlNonCurrentAccount);
                    object totRecsCurrentAccount = GlobalFramework.SessionXpo.ExecuteScalar(sqlCurrentAccount);
                    //Shared
                    ThermalPrinterInternalDocumentWorkSession thermalPrinterInternalDocumentWorkSession;
                    //Tests
                    bool printSplitCurrentAccountModeNonCurrentAcount = true;
                    bool printSplitCurrentAccountModeCurrentAcount = true;
                    bool printSplitCurrentAccountModeAll = true;

                    //Test Print Document
                    //Always print NonCurrent Account Movements, even if dont have any Movements, Show zero values
                    if (printSplitCurrentAccountModeNonCurrentAcount)
                    {
                        thermalPrinterInternalDocumentWorkSession = new ThermalPrinterInternalDocumentWorkSession(pPrinter, workSessionPeriod, SplitCurrentAccountMode.NonCurrentAcount);
                        thermalPrinterInternalDocumentWorkSession.Print();
                    }

                    //Test Combine NonCC and CC (All)
                    if (printSplitCurrentAccountModeAll)
                    {
                        thermalPrinterInternalDocumentWorkSession = new ThermalPrinterInternalDocumentWorkSession(pPrinter, workSessionPeriod, SplitCurrentAccountMode.All);
                        thermalPrinterInternalDocumentWorkSession.Print();
                    }

                    //Only Prints Current Account Movements if if Has Movements
                    if (printSplitCurrentAccountModeCurrentAcount && Convert.ToInt16(totRecsCurrentAccount) > 0)
                    {
                        thermalPrinterInternalDocumentWorkSession = new ThermalPrinterInternalDocumentWorkSession(pPrinter, workSessionPeriod, SplitCurrentAccountMode.CurrentAcount);
                        thermalPrinterInternalDocumentWorkSession.Print();
                    }

                    Console.WriteLine("ThermalPrinterInternalDocumentWorkSession Printed");
                }
                else
                {
                    Console.WriteLine("ERROR: ThermalPrinterInternalDocumentWorkSession");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
