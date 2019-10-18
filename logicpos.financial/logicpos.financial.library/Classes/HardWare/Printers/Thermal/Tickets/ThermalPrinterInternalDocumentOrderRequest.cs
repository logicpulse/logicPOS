using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Enums;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Data;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets
{
    public class ThermalPrinterInternalDocumentOrderRequest : ThermalPrinterBaseInternalTemplate
    {
        //Private Members
        /* IN008024 */
        //private string _appOperationModeToken = GlobalFramework.Settings["appOperationModeToken"];
        private fin_documentorderticket _orderTicket;
        private bool _enableArticlePrinter;

        public ThermalPrinterInternalDocumentOrderRequest(sys_configurationprinters pPrinter, fin_documentorderticket pOrderTicket)
            :this(pPrinter, pOrderTicket, false) { }

        public ThermalPrinterInternalDocumentOrderRequest(sys_configurationprinters pPrinter, fin_documentorderticket pOrderTicket, bool pEnableArticlePrinter)
            : base(pPrinter)
        {
            try
            {
                //Parameters
                _orderTicket = pOrderTicket;
                _enableArticlePrinter = pEnableArticlePrinter;

                //Order Request #1/3
                _ticketTitle = string.Format("{0}: #{1}"
                    , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_order_request")
                    , _orderTicket.TicketId
                );

                //Table|Order #2|Name/Zone
                _ticketSubTitle = string.Format("{0}: #{1}/{2}"
                    , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], string.Format("global_table_appmode_{0}", SettingsApp.CustomAppOperationMode.AppOperationTheme).ToLower()) /* IN008024 */
                    , _orderTicket.OrderMain.PlaceTable.Designation
                    , _orderTicket.OrderMain.PlaceTable.Place.Designation
                );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Override Parent Template
        public override void PrintContent()
        {
            try
            {
                //Call Base Template PrintHeader
                base.PrintTitles();

                //Align Center
                _thermalPrinterGeneric.SetAlignCenter();

                PrintDocumentDetails();

                //Reset to Left
                _thermalPrinterGeneric.SetAlignLeft();

                //Line Feed
                _thermalPrinterGeneric.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void PrintDocumentDetails()
        {
            List<TicketColumn> columns = new List<TicketColumn>();
            columns.Add(new TicketColumn("Designation", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_designation"), 0, TicketColumnsAlign.Left));
            columns.Add(new TicketColumn("Quantity", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_quantity_acronym"), 8, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
            columns.Add(new TicketColumn("UnitMeasure", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_unit_measure_acronym"), 3, TicketColumnsAlign.Right));

            //Prepare Table with Padding
            DataTable dataTable = TicketTable.InitDataTableFromTicketColumns(columns);
            TicketTable ticketTable = new TicketTable(dataTable, columns, _maxCharsPerLineNormal);

            //Print Items
            DataRow dataRow;
            foreach (fin_documentorderdetail item in _orderTicket.OrderDetail)
            {
                //Add All Rows if Normal Mode without explicit ArticlePrinter defined, or print Printer Articles for explicit defined ArticlePrinter 
                if (! _enableArticlePrinter || _thermalPrinterGeneric.Printer == item.Article.Printer)
                {
                    //Add Rows to main Ticket
                    dataRow = ticketTable.NewRow();
                    dataRow[0] = item.Designation;
                    dataRow[1] = item.Quantity;
                    dataRow[2] = item.UnitMeasure;
                    //Add DataRow to Table, Ready for Print
                    ticketTable.Rows.Add(dataRow);
                }
            }

            //Print Table
            ticketTable.Print(_thermalPrinterGeneric);
        }
    }
}