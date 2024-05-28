using logicpos.datalayer.DataLayer.Xpo;
using LogicPOS.Globalization;
using LogicPOS.Printing.Templates;
using LogicPOS.Printing.Tickets;
using LogicPOS.Settings;
using System;
using System.Collections.Generic;
using System.Data;

namespace LogicPOS.Printing.Documents
{
    public class ThermalPrinterInternalDocumentOrderRequest : ThermalPrinterBaseInternalTemplate
    {
        //Private Members
        /* IN008024 */
        //private string _appOperationModeToken = LogicPOS.Settings.GeneralSettings.Settings["appOperationModeToken"];
        private readonly fin_documentorderticket _orderTicket;
        private readonly bool _enableArticlePrinter;

        public ThermalPrinterInternalDocumentOrderRequest(sys_configurationprinters pPrinter, fin_documentorderticket pOrderTicket)
            : this(pPrinter, pOrderTicket, false) { }

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
                    , CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_order_request")
                    , _orderTicket.TicketId
                );

                //Table|Order #2|Name/Zone
                _ticketSubTitle = string.Format("{0}: #{1}/{2}"
                    , CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, string.Format("global_table_appmode_{0}", AppOperationModeSettings.CustomAppOperationMode.AppOperationTheme).ToLower()) /* IN008024 */
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
                PrintTitles();

                //Align Center
                _genericThermalPrinter.SetAlignCenter();

                PrintDocumentDetails();

                //Reset to Left
                _genericThermalPrinter.SetAlignLeft();

                //Line Feed
                _genericThermalPrinter.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void PrintDocumentDetails()
        {
            List<TicketColumn> columns = new List<TicketColumn>
            {
                new TicketColumn("Designation", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"), 0, TicketColumnsAlignment.Left),
                new TicketColumn("Quantity", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                new TicketColumn("UnitMeasure", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_unit_measure_acronym"), 3, TicketColumnsAlignment.Right)
            };

            //Prepare Table with Padding
            DataTable dataTable = TicketTable.InitDataTableFromTicketColumns(columns);
            TicketTable ticketTable = new TicketTable(dataTable, columns, _maxCharsPerLineNormal);

            //Print Items
            DataRow dataRow;
            foreach (fin_documentorderdetail item in _orderTicket.OrderDetail)
            {
                //Add All Rows if Normal Mode without explicit ArticlePrinter defined, or print Printer Articles for explicit defined ArticlePrinter 
                if (!_enableArticlePrinter || _genericThermalPrinter.Printer == item.Article.Printer)
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
            ticketTable.Print(_genericThermalPrinter);
        }
    }
}