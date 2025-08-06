using ESC_POS_USB_NET.Printer;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.Api.Features.Reports.WorkSession.Common;
using LogicPOS.Api.Features.Reports.WorkSession.GetWorkSessionData;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Printing.Enums;
using LogicPOS.UI.Printing.Tickets;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LogicPOS.UI.Printing
{
    public partial class WorkSessionPrinter : ThermalPrinter
    {
        private readonly WorkSessionData _workSessionReceiptsData;
        private readonly WorkSessionData _workSessionDocumentsData;
        protected string _SubTitle;
        protected string _Title;

        public WorkSessionPrinter(Printer printer, Guid workSessionId) : base(printer)
        {
            _workSessionDocumentsData = GetWorkSessionDocumentsData(workSessionId);
            _workSessionReceiptsData = GetWorkSessionReceiptsData(workSessionId);
            DefineTicketTitle();
            DefineTicketSubtitle();
        }

        WorkSessionData GetWorkSessionDocumentsData(Guid workSessionId)
        {
            var result = _mediator.Send(new GetWorkSessionDocumentsDataQuery(workSessionId)).Result;
            if (result.IsError)
            {
                CustomAlerts.Error()
                            .WithMessage(result.FirstError.Description)
                            .ShowAlert();
            }

            return result.Value;
        }

        WorkSessionData GetWorkSessionReceiptsData(Guid workSessionId)
        {
            var result = _mediator.Send(new GetWorkSessionReceiptsDataQuery(workSessionId)).Result;
            if (result.IsError)
            {
                CustomAlerts.Error()
                            .WithMessage(result.FirstError.Description)
                            .ShowAlert();
            }

            return result.Value;
        }
        private void DefineTicketSubtitle()
        {
            var ticketSubTitleExtra = GeneralUtils.GetResourceByName("global_current_account");

            if (!string.IsNullOrEmpty(_SubTitle) && !string.IsNullOrEmpty(ticketSubTitleExtra))
            {
                _SubTitle = string.Format("{0} : ({1})", _SubTitle, ticketSubTitleExtra);
            }
            else if (string.IsNullOrEmpty(_SubTitle) && !string.IsNullOrEmpty(ticketSubTitleExtra))
            {
                _SubTitle = string.Format("({0})", ticketSubTitleExtra);
            }
        }

        private void DefineTicketTitle()
        {
            if (_workSessionReceiptsData.WorkSession.Type == WorkSessionPeriodType.Day)
            {
                if (_workSessionReceiptsData.WorkSession.Status == WorkSessionPeriodStatus.Open)
                {
                    _Title = GeneralUtils.GetResourceByName("ticket_title_worksession_day_resume");
                }
                else
                {
                    _Title = GeneralUtils.GetResourceByName("ticket_title_worksession_day_close");
                }
                return;
            }

            if (_workSessionReceiptsData.WorkSession.Status == WorkSessionPeriodStatus.Open)
            {
                _Title = GeneralUtils.GetResourceByName("ticket_title_worksession_terminal_resume");
            }
            else
            {
                _Title = GeneralUtils.GetResourceByName("ticket_title_worksession_terminal_close");
            }
            _SubTitle = _workSessionReceiptsData.WorkSession.Type == WorkSessionPeriodType.Terminal ? TerminalService.Terminal.Designation : string.Empty;
        }

        public void PrintFooter()
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
            _printer.AlignCenter();
            _printer.DoubleWidth2();
            _printer.BoldMode(_Title);
            _printer.BoldMode(_SubTitle);
            _printer.NewLine();
            _printer.NormalWidth();

            PrintWorkSessionMovement(_workSessionDocumentsData, _workSessionReceiptsData);
            PrintFooter();
            _printer.FullPaperCut();
            _printer.PrintDocument();
            _printer.Clear();

        }

        public bool PrintWorkSessionMovement(WorkSessionData workSessionDocumentsData,
                                             WorkSessionData workSessionReceiptsData)
        {
            string dateCloseDisplay = workSessionDocumentsData.WorkSession.StartDate.ToString(AppSettings.Culture.DateTimeFormat);


            //Print Header Summary

            List<TicketColumn> columns = new List<TicketColumn>
                    {
                        new TicketColumn("Label", "", Convert.ToInt16(48 / 2) - 2, TicketColumnsAlignment.Right),
                        new TicketColumn("Value", "", Convert.ToInt16(48/ 2) - 2, TicketColumnsAlignment.Left)
                    };
            DataRow dataRow = null;
            TicketTable ticketTable = new TicketTable(columns);
            //Open DateTime
            dataRow = ticketTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_open_datetime"));
            dataRow[1] = workSessionDocumentsData.WorkSession.StartDate.ToString(AppSettings.Culture.DateTimeFormat);
            ticketTable.Rows.Add(dataRow);
            //Close DataTime
            dataRow = ticketTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_close_datetime"));
            dataRow[1] = workSessionDocumentsData.WorkSession.EndDate?.ToString(AppSettings.Culture.DateTimeFormat);
            ticketTable.Rows.Add(dataRow);
            //Open Total CashDrawer
            dataRow = ticketTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_open_total_cashdrawer"));
            dataRow[1] = (WorkSessionService.OpenTotal).ToString("F2") + $" {workSessionDocumentsData.Currency}";
            ticketTable.Rows.Add(dataRow);
            //Close Total CashDrawer
            dataRow = ticketTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_close_total_cashdrawer"));
            dataRow[1] = (workSessionDocumentsData.FamilyReportItems.Sum(t=>t.Total)).ToString("F2") + $" {workSessionDocumentsData.Currency}";
            ticketTable.Rows.Add(dataRow);
            //Total Money In
            dataRow = ticketTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_total_money_in"));
            dataRow[1] = (WorkSessionService.CashDrawerInTotal).ToString("F2") + $" {workSessionDocumentsData.Currency}";
            ticketTable.Rows.Add(dataRow);
            //Total Money Out
            dataRow = ticketTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_total_money_out"));
            dataRow[1] = (WorkSessionService.CashDrawerOutTotal).ToString("F2") + $" {workSessionDocumentsData.Currency}";
            ticketTable.Rows.Add(dataRow);



            ticketTable.Print(_printer);
            _printer.NewLine();

            string unitMeasure = string.Empty;
            decimal summaryTotalQuantity = 0.0m;
            decimal summaryTotal = 0.0m;
            List<string> tableCustomPrint = new List<string>();

            int groupPosition = -1;
            int groupPositionTitlePayments = (workSessionDocumentsData.WorkSession.Type == WorkSessionPeriodType.Day) ? 9 : 8;

            groupPosition++;

            if (groupPosition == 0)
            {
                _printer.DoubleWidth2();
                _printer.BoldMode(GeneralUtils.GetResourceByName("global_worksession_resume_finance_documents"));
                _printer.NormalWidth();
                _printer.Separator(' ');
            }

            summaryTotalQuantity = 0.0m;
            summaryTotal = 0.0m;

            columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_family"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                //columns.Add(new TicketColumn("UnitMeasure", string.Empty, 3));
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            //Init DataTable
            ticketTable = new TicketTable(columns);

            //If Has data
            if (workSessionDocumentsData.FamilyReportItems.Count > 0)
            {
                foreach (var item in workSessionDocumentsData.FamilyReportItems)
                {

                    unitMeasure = "Un";
                    //Create Row
                    dataRow = ticketTable.NewRow();
                    dataRow[0] = item.Designation;
                    dataRow[1] = Convert.ToDecimal(item.Quantity);
                    //dataRow[2] = unitMeasure;
                    dataRow[2] = Convert.ToDecimal(item.Total);
                    ticketTable.Rows.Add(dataRow);
                }
            }

            else
            {
                //Create Row
                dataRow = ticketTable.NewRow();
                dataRow[0] = GeneralUtils.GetResourceByName("global_cashdrawer_without_movements");
                dataRow[1] = 0.0m;
                //dataRow[2] = string.Empty;//UnitMeasure
                dataRow[2] = 0.0m;
                ticketTable.Rows.Add(dataRow);
            }

            //Add Final Summary Row
            summaryTotal = workSessionDocumentsData.Total;
            summaryTotalQuantity = workSessionDocumentsData.FamilyReportItems.Sum(x => x.Quantity);
            dataRow = ticketTable.NewRow();
            dataRow[0] = GeneralUtils.GetResourceByName("global_total");
            dataRow[1] = summaryTotalQuantity;
            //dataRow[2] = string.Empty;
            dataRow[2] = summaryTotal;
            ticketTable.Rows.Add(dataRow);



            //Custom Print Loop, to Print all Table Rows, and Detect Rows to Print in DoubleHeight (Title and Total)
            tableCustomPrint = ticketTable.GetTable();

            //Dynamic Print All except Last One (Totals), Double Height in Titles
            for (int x = 0; x < tableCustomPrint.Count - 1; x++)
            {
                if (x == 0)
                {
                    _printer.BoldMode(tableCustomPrint[x]);
                    _printer.NormalWidth();
                }
                else
                {
                    _printer.Append(tableCustomPrint[x]);
                }
            }

            _printer.NewLine();

            if (workSessionDocumentsData.SubfamilyReportItems.Count > 0)
            {
                PrintSubfamilyTotal(workSessionDocumentsData);
            }
            if (workSessionDocumentsData.SubfamilyReportItems.Count > 0)
            {
                PrintArticleTotal(workSessionDocumentsData);
            }
            if (workSessionDocumentsData.TaxReportItems.Count > 0)
            {
                PrintTaxTotal(workSessionDocumentsData);
            }
            if (workSessionDocumentsData.PaymentReportItems.Count > 0)
            {
                PrintPaymentMethodsTotal(workSessionDocumentsData);
            }
            if (workSessionDocumentsData.DocumentTypeReportItems.Count > 0)
            {
                PrintDocumentTypeTotal(workSessionDocumentsData);
            }
            if (workSessionDocumentsData.HoursReportItems.Count > 0)
            {
                PrintHoursTotal(workSessionDocumentsData);
            }
            if (workSessionDocumentsData.UserReportItems.Count > 0)
            {
                PrintUsersTotal(workSessionDocumentsData);
            }

            //Line Feed
            _printer.NewLine();

            //When finish all groups, print Last Row, the Summary Totals Row, Ommited in Custom Print Loop
            _printer.BoldMode(tableCustomPrint[tableCustomPrint.Count - 1]);


            _printer.NewLine();
            _printer.DoubleWidth2();
            _printer.BoldMode(GeneralUtils.GetResourceByName("global_worksession_resume_paymens_documents"));
            _printer.NormalWidth();
            _printer.Separator(' ');
            summaryTotal = workSessionReceiptsData.Total;
            summaryTotalQuantity = workSessionReceiptsData.UserReportItems.Sum(x => x.Quantity);
            if (workSessionReceiptsData.PaymentReportItems.Count > 0)
            {
                PrintPaymentMethodsTotal(workSessionReceiptsData);
            }

            if (workSessionReceiptsData.HoursReportItems.Count > 0)
            {
                PrintHoursTotal(workSessionReceiptsData);
            }
            if (workSessionReceiptsData.UserReportItems.Count > 0)
            {
                PrintUsersTotal(workSessionReceiptsData);
            }
            ticketTable = new TicketTable(columns);
            dataRow = ticketTable.NewRow();
            dataRow[0] = GeneralUtils.GetResourceByName("global_total");
            dataRow[1] = summaryTotalQuantity;
            //dataRow[2] = string.Empty;
            dataRow[2] = summaryTotal;
            ticketTable.Rows.Add(dataRow);

            //Custom Print Loop, to Print all Table Rows, and Detect Rows to Print in DoubleHeight (Title and Total)
            tableCustomPrint = ticketTable.GetTable();

            //Line Feed
            _printer.Separator(' ');

            //When finish all groups, print Last Row, the Summary Totals Row, Ommited in Custom Print Loop
           
            _printer.BoldMode(tableCustomPrint[tableCustomPrint.Count - 1]);
            _printer.NewLine();
            _printer.NormalWidth();
            return true;
        }
    }
}



