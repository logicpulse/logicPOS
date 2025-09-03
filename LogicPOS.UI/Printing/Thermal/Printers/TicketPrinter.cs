using ESC_POS_USB_NET.Enums;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Printing.Enums;
using LogicPOS.UI.Printing.Thermal.Printers;
using LogicPOS.UI.Printing.Tickets;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using Printer = ESC_POS_USB_NET.Printer.Printer;

namespace LogicPOS.UI.Printing
{
    public class PosTicketPrinter : ThermalPrinter
    {
        private readonly TicketPrintingData _ticket;
        private string _ticketTitle = string.Empty;
        private string _ticketSubTitle = string.Empty;
        public PosTicketPrinter(Printer printer, TicketPrintingData ticket) : base(printer)
        {
            _ticket = ticket;
        }

        private void PrintTitle()
        {

            _ticketTitle = string.Format("{0}: #{1}"
                , GeneralUtils.GetResourceByName("global_order_request")
                , _ticket.Number.ToString()
            );
            var mode = AppSettings.Instance.UseBackOfficeMode ? "retail" : "default";
            //Table|Order #2|Name/Zone
            _ticketSubTitle = string.Format("{0}: #{1}/{2}"
                , GeneralUtils.GetResourceByName(string.Format($"global_table_appmode_{mode}").ToLower()) /* IN008024 */
                , _ticket.Table
                , _ticket.Place
            );
            _printer.AlignCenter();
            _printer.SetLineHeight(50);
            _printer.ExpandedMode(PrinterModeState.On);
            _printer.CondensedMode(PrinterModeState.On);
            _printer.DoubleWidth3();
            _printer.BoldMode(_ticketTitle);
            _printer.Append(_ticketSubTitle);
            _printer.NormalWidth();
            _printer.CondensedMode(PrinterModeState.Off);
            _printer.ExpandedMode(PrinterModeState.Off);
        }

        private void PrintDocumentDetails()
        {
            _printer.NormalLineHeight();
            _printer.NewLine();
            List<TicketColumn> columns = new List<TicketColumn>
            {
                new TicketColumn("Designation", GeneralUtils.GetResourceByName("global_designation"), 0, TicketColumnsAlignment.Left),
                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                new TicketColumn("UnitMeasure", GeneralUtils.GetResourceByName("global_unit_measure_acronym"), 3, TicketColumnsAlignment.Right)
            };

            //Prepare Table with Padding
            DataTable dataTable = TicketTable.InitDataTableFromTicketColumns(columns);
            TicketTable ticketTable = new TicketTable(columns);

            DataRow dataRow;
            foreach (var item in _ticket.Items)
            {
                dataRow = ticketTable.NewRow();
                dataRow[0] = item.Article;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Unit;
                ticketTable.Rows.Add(dataRow);
            }

            ticketTable.Print(_printer);
        }

        private void PrintFooter()
        {
            _printer.Separator(' ');
            _printer.Append(GeneralUtils.GetResourceByName("global_internal_document_footer1"));
            _printer.Append(GeneralUtils.GetResourceByName("global_internal_document_footer2"));
            _printer.Separator(' ');
            _printer.NewLine();
            _printer.Append(GeneralUtils.GetResourceByName("global_internal_document_footer3"));
            _printer.Separator(' ');
            _printer.NewLine();
            _printer.Append(string.Format("{0} - {1}", AuthenticationService.User.Name, TerminalService.Terminal.Designation));
            _printer.NewLine();
            //Printed On | Company|App|Version
            _printer.Append(string.Format("{1}: {2}{0}{3}: {4} {5}"
                , Environment.NewLine
                , GeneralUtils.GetResourceByName("global_printed_on_date")
                , DateTime.Now.ToLocalTime()
                , "LogicPulse"//_customVars["APP_COMPANY"]
                , "LogicPOS"//_customVars["APP_NAME"]
                , "vs1.010.1"//_customVars["APP_VERSION"]
                ));
        }

        public override void Print()
        {
            PrintHeader();
            PrintTitle();
            PrintDocumentDetails();
            PrintFooter();
            _printer.FullPaperCut();
            _printer.PrintDocument();
            _printer.Clear();
        }
    }
}
