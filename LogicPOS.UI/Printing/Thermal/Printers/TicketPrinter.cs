using ESC_POS_USB_NET.Enums;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Printing.Enums;
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
        private readonly PosTicket _ticket;
        private readonly Table _table;
        private string _ticketTitle = string.Empty;
        private string _ticketSubTitle = string.Empty;
        public PosTicketPrinter(Printer printer, PosTicket ticket, Table table) : base(printer)
        {
            _ticket = ticket;
            _table = table;
        }

        private void PrintTitle()
        {
            //Order Request #1/3
            if (_ticket==null)
            {
                new SimpleAlert().WithMessageType(Gtk.MessageType.Info)
                                 .WithMessage("Sem pedido recente")
                                 .ShowAlert();
                return;
            }
            _ticketTitle = string.Format("{0}: #{1}"
                , GeneralUtils.GetResourceByName("global_order_request")
                , _ticket.Number.ToString()
            );
            var mode = AppSettings.Instance.UseBackOfficeMode?"retail":"default";
            //Table|Order #2|Name/Zone
            _ticketSubTitle = string.Format("{0}: #{1}/{2}"
                , GeneralUtils.GetResourceByName(string.Format($"global_table_appmode_{mode}").ToLower()) /* IN008024 */
                , _table.Designation
                , _table.Place.Designation
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

            //Print Items
            DataRow dataRow;
            foreach (var item in _ticket.Items)
            {
                //Add All Rows if Normal Mode without explicit ArticlePrinter defined, or print Printer Articles for explicit defined ArticlePrinter 

                //Add Rows to main Ticket
                dataRow = ticketTable.NewRow();
                dataRow[0] = item.Article.Designation;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Article.Unit;
                //Add DataRow to Table, Ready for Print
                ticketTable.Rows.Add(dataRow);

            }

            //Print Table
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
