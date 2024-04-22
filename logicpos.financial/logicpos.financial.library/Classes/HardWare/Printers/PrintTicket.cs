using DansCSharpLibrary.JsonSerialization;
using logicpos.datalayer.App;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal;
using logicpos.shared.App;
using System;
using System.Data;

/*
 * Notes to add fields to Ticket DataLoop, Must add the Field to all of this files
 * logicpos.framework\ThermalDotNet\Util.cs
 * logicpos.framework\ThermalDotNet\ThermalPrinter.cs
 * logicpos.framework\logicpos.virtualprinter\PreviewForm.cs
 * logicpos.framework\logicpos.virtualprinter\TextoFormDlg.cs
 */

namespace logicpos.financial.library.Classes.Hardware.Printer
{
    public class PrintTicket
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Functions

        private static DataTable getStaticData()
        {
            //Select without licence fields COMPANY_* added above
            string sql = @"
                SELECT 
                    Token as token, Value as value 
                FROM 
                    cfg_configurationpreferenceparameter 
                WHERE 
                    Token = 'COMPANY_POSTALCODE' OR
                    Token = 'COMPANY_CITY' OR
                    Token = 'COMPANY_FAX'
                ORDER BY
                    Ord
            ";
            DataTable resultDataTable = SharedUtils.GetDataTableFromQuery(sql);
            resultDataTable.Columns[0].ColumnName = "token";
            resultDataTable.Columns[0].Unique = true;
            resultDataTable.PrimaryKey = new DataColumn[] { resultDataTable.Columns["token"] };

            //Add extra data to dataCompany DataTable
            //license
            resultDataTable.Rows.Add("COMPANY_NAME", SharedFramework.LicenseCompany);
            resultDataTable.Rows.Add("COMPANY_FISCALNUMBER", SharedFramework.LicenseNif);
            resultDataTable.Rows.Add("COMPANY_ADDRESS", SharedFramework.LicenseAddress);
            resultDataTable.Rows.Add("COMPANY_TELEPHONE", SharedFramework.LicenseTelephone);
            resultDataTable.Rows.Add("COMPANY_EMAIL", SharedFramework.LicenseEmail);
            //User/Terminal      
            resultDataTable.Rows.Add("TERMINAL_NAME", DataLayerFramework.LoggedTerminal.Designation);
            resultDataTable.Rows.Add("TERMINAL_USERNAME", DataLayerFramework.LoggedUser.Name);
            //Footer Lines : Deprecated
            //resultDataTable.Rows.Add("FOOTER_LINE1", SettingsApp.TicketFooterLine1);
            //resultDataTable.Rows.Add("FOOTER_LINE2", SettingsApp.TicketFooterLine2);
            //resultDataTable.Rows.Add("FOOTER_LINE3", SettingsApp.TicketFooterLine3);
            //resultDataTable.Rows.Add("FOOTER_LINE4", SettingsApp.TicketFooterLine4);
            //resultDataTable.Rows.Add("FOOTER_LINE5", SettingsApp.TicketFooterLine5);
            //resultDataTable.Rows.Add("FOOTER_LINE6", SettingsApp.TicketFooterLine6);
            //resultDataTable.Rows.Add("FOOTER_LINE7", SettingsApp.TicketFooterLine7);
            //resultDataTable.Rows.Add("FOOTER_LINE8", SettingsApp.TicketFooterLine8);

            return resultDataTable;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //Not Used: Implemented to send Data to Reports.exe
        //Use with:
        //PrintTransportObject transportObject = new PrintTransportObject(printCopies, dataTableLoop, dataTableStatic);
        //bool result = TransportDataToGenericPrinter(transportObject);
        private static bool TransportDataToGenericPrinter(PrintTransportObject pTransportObject)
        {
            bool result = false;

            try
            {
                //External App
                string externalApp = "ConsoleApplication.exe";//SettingsApp.ExecutableReports
                //Filename
                Guid guidParameter = Guid.NewGuid();
                string fileName = string.Format("{0}{1}.tob", DataLayerFramework.Path["temp"].ToString(), guidParameter.ToString());
                JsonSerialization.WriteToJsonFile<PrintTransportObject>(fileName, pTransportObject);
                //Call External Application
                SharedUtils.ExecuteExternalProcess(externalApp, string.Format("printpreview {0}", fileName));

                result = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }
    }
}
