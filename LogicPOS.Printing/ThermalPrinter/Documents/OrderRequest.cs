using LogicPOS.DTOs.Printing;
using LogicPOS.Printing.Enums;
using LogicPOS.Printing.Templates;
using LogicPOS.Printing.Tickets;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Data;

namespace LogicPOS.Printing.Documents
{
    public class OrderRequest : BaseInternalTemplate
    {
        private readonly PrintOrderTicketDto _orderTicket;
        private readonly bool _articlePrinterEnabled;

        public OrderRequest(
            PrinterDto printer,
            PrintOrderTicketDto orderTicket,
            string terminalDesignation,
            string userName,
            CompanyPrintingInformationsDto companyInformationsDto)
            : this(
                  printer,
                  orderTicket, 
                  terminalDesignation, 
                  userName,companyInformationsDto, false)
        { }

        public OrderRequest(
    PrinterDto printer,
    PrintOrderTicketDto orderTicket,
    string terminalDesignation)
    : base(
          printer,null,
          terminalDesignation, "teste")
        { }

        public OrderRequest(
            PrinterDto printer,
            PrintOrderTicketDto orderTicket,
            string terminalDesignation,
            string userName,
            CompanyPrintingInformationsDto companyInformationsDto,
            bool articlePrinterEnabled)
            : base(printer, companyInformationsDto, terminalDesignation, userName)
        {
            try
            {
                //Parameters
                _orderTicket = orderTicket;
                _articlePrinterEnabled = articlePrinterEnabled;
                _companyInformationsDto = companyInformationsDto;
                //Order Request #1/3
                _ticketTitle = string.Format("{0}: #{1}"
                    , GeneralUtils.GetResourceByName("global_order_request")
                    , _orderTicket.TicketId
                );

                //Table|Order #2|Name/Zone
                _ticketSubTitle = string.Format("{0}: #{1}/{2}"
                    , GeneralUtils.GetResourceByName(string.Format("global_table_appmode_{0}", "tchial0_app_operation_mode").ToLower()) /* IN008024 */
                    , _orderTicket.TableDesignation
                    , _orderTicket.PlaceDesignation
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
                PrintHeader();
                //Call Base Template PrintHeader
                PrintTitles();

                //Align Center
                _printer.SetAlignCenter();

                PrintDocumentDetails();

                //Reset to Left
                _printer.SetAlignLeft();

                //Line Feed
                _printer.LineFeed();
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
                new TicketColumn("Designation", GeneralUtils.GetResourceByName("global_designation"), 0, TicketColumnsAlignment.Left),
                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                new TicketColumn("UnitMeasure", GeneralUtils.GetResourceByName("global_unit_measure_acronym"), 3, TicketColumnsAlignment.Right)
            };

            //Prepare Table with Padding
            DataTable dataTable = TicketTable.InitDataTableFromTicketColumns(columns);
            TicketTable ticketTable = new TicketTable(dataTable, columns, _maxCharsPerLineNormal);

            //Print Items
            DataRow dataRow;
            foreach (var item in _orderTicket.OrderDetails)
            {
                //Add All Rows if Normal Mode without explicit ArticlePrinter defined, or print Printer Articles for explicit defined ArticlePrinter 
                if (_articlePrinterEnabled == false || _printer.Printer.Id == item.ArticlePrinter.Id)
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
            ticketTable.Print(_printer);
        }
    }
}