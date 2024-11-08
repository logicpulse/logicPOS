using DevExpress.Xpo.DB;
using LogicPOS.Api.Features.Reports.WorkSession.Common;
using LogicPOS.Data.XPO;
using LogicPOS.DTOs.Printing;
using LogicPOS.Printing.Common;
using LogicPOS.Printing.Enums;
using LogicPOS.Printing.Templates;
using LogicPOS.Printing.Tickets;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.Printing.Documents
{
    public class WorkSession : BaseInternalTemplate
    {
        private readonly PrintWorkSessionDto _workSessionPeriod;
        private readonly WorkSessionData _workSession;
        public WorkSession(PrinterDto printer, string terminalDesignation, WorkSessionData workSessionData, PrintWorkSessionDto printWorkSessionDto) : base(printer, terminalDesignation)
        {
            _printer = new GenericThermalPrinter(printer);
            _workSession=workSessionData;
            _workSessionPeriod=printWorkSessionDto;
        }
        private void DefineTicketSubtitle()
        {
            var ticketSubTitleExtra = GeneralUtils.GetResourceByName("global_current_account");

            if (_ticketSubTitle != string.Empty && ticketSubTitleExtra != string.Empty)
            {
                _ticketSubTitle = string.Format("{0} : ({1})", _ticketSubTitle, ticketSubTitleExtra);
            }
            else if (_ticketSubTitle == string.Empty && ticketSubTitleExtra != string.Empty)
            {
                _ticketSubTitle = string.Format("({0})", ticketSubTitleExtra);
            }
        }

        private void DefineTicketTitle()
        {
            if (_workSessionPeriod.PeriodTypeIsDay)
            {
                if (_workSessionPeriod.SessionStatusIsOpen)
                {
                    _ticketTitle = GeneralUtils.GetResourceByName("ticket_title_worksession_day_resume");
                }
                else
                {
                    _ticketTitle = GeneralUtils.GetResourceByName("ticket_title_worksession_day_close");
                }
                return;
            }

            if (_workSessionPeriod.SessionStatusIsOpen)
            {
                _ticketTitle = GeneralUtils.GetResourceByName("ticket_title_worksession_terminal_resume");
            }
            else
            {
                _ticketTitle = GeneralUtils.GetResourceByName("ticket_title_worksession_terminal_close");
            }
            _ticketSubTitle = _workSessionPeriod.PeriodTypeIsTerminal ? _workSessionPeriod.TerminalDesignation : string.Empty;
        }

        public override void PrintContent()
        {
            PrintTitles(_ticketTitle);
            _printer.SetAlignCenter();
            PrintWorkSessionMovement(_workSessionPeriod, _workSession);
        }


        public bool PrintWorkSessionMovement(
            PrintWorkSessionDto workSessionDto,
           WorkSessionData workSessionData
           )
        {
            

            string dateCloseDisplay = workSessionData.StartDate.ToString(CultureSettings.DateTimeFormat);


            //Print Header Summary
            DataRow dataRow = null;
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Label", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Value", typeof(string)));
            //Open DateTime
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_open_datetime"));
            dataRow[1] = workSessionData.StartDate.ToString(CultureSettings.DateTimeFormat);
            dataTable.Rows.Add(dataRow);
            //Close DataTime
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_close_datetime"));
            dataRow[1] = dateCloseDisplay;
            dataTable.Rows.Add(dataRow);
            //Open Total CashDrawer
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_open_total_cashdrawer"));
            dataRow[1] = workSessionData.OpenTotal.ToString();/* LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(
                (decimal)_sessionPeriodSummaryDetails["totalMoneyInCashDrawerOnOpen"],
                XPOSettings.ConfigurationSystemCurrency.Acronym); Luciano*/
            dataTable.Rows.Add(dataRow);
            //Close Total CashDrawer
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_close_total_cashdrawer"));
            dataRow[1] = workSessionData.Total.ToString();/*LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(
                (decimal)_sessionPeriodSummaryDetails["totalMoneyInCashDrawer"],
                XPOSettings.ConfigurationSystemCurrency.Acronym);*/
            dataTable.Rows.Add(dataRow);
            //Total Money In
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_total_money_in"));
            dataRow[1] = workSessionData.TotalIn.ToString();/*LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(
                (decimal)_sessionPeriodSummaryDetails["totalMoneyIn"],
                XPOSettings.ConfigurationSystemCurrency.Acronym);*/
            dataTable.Rows.Add(dataRow);
            //Total Money Out
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_total_money_out"));
            dataRow[1] = workSessionData.TotalOut.ToString();/*LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(
                (decimal)_sessionPeriodSummaryDetails["totalMoneyOut"],
                XPOSettings.ConfigurationSystemCurrency.Acronym);*/
            dataTable.Rows.Add(dataRow);
            //Configure Ticket Column Properties
            List<TicketColumn> columns = new List<TicketColumn>
                    {
                        new TicketColumn("Label", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlignment.Right),
                        new TicketColumn("Value", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlignment.Left)
                    };
            TicketTable ticketTable = new TicketTable(dataTable, columns, _printer.MaxCharsPerLineNormalBold);
            //Print Ticket Table
            ticketTable.Print(_printer);
            //Line Feed
            _printer.LineFeed();

          
            //Prepare Local vars for Group Loop
            SQLSelectResultData xPSelectData = null;
            string designation = string.Empty;
            decimal quantity = 0.0m;
            decimal total = 0.0m;
            string unitMeasure = string.Empty;
            //Store Final Totals
            decimal summaryTotalQuantity = 0.0m;
            decimal summaryTotal = 0.0m;
            //Used to Custom Print Table Ticket Rows
            List<string> tableCustomPrint = new List<string>();

            //Start to process Group
            int groupPosition = -1;
            //Assign Position to Print Payment Group Split Title
            int groupPositionTitlePayments = (workSessionDto.PeriodTypeIsDay) ? 9 : 8;
            //If CurrentAccount Mode decrease 1, it dont have PaymentMethods
             groupPositionTitlePayments--;

            for(int i = 0; i < 2; i++) { 
                    //Increment Group Position
                    groupPosition++;

                    //Print Group Titles (FinanceDocuments|Payments)
                    if (groupPosition == 0)
                    {
                        _printer.WriteLine(GeneralUtils.GetResourceByName("global_worksession_resume_finance_documents"), WriteLineTextMode.Big);
                        _printer.LineFeed();
                    }
                    else if (groupPosition == groupPositionTitlePayments)
                    {
                        //When finish FinanceDocuemnts groups, print Last Row, the Summary Totals Row
                        _printer.WriteLine(tableCustomPrint[tableCustomPrint.Count - 1], WriteLineTextMode.DoubleHeight);
                        _printer.LineFeed();

                        _printer.WriteLine(GeneralUtils.GetResourceByName("global_worksession_resume_paymens_documents"), WriteLineTextMode.Big);
                        _printer.LineFeed();
                    }

                    //Reset Totals
                    summaryTotalQuantity = 0.0m;
                    summaryTotal = 0.0m;

                    //Get Group Data from group Query
                    //xPSelectData = XPOUtility.GetSelectedDataFromQuery(item.Value.Sql);

                    //Generate Columns
                    columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", workSessionData.FamilyReportItems.ToString(), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                //columns.Add(new TicketColumn("UnitMeasure", string.Empty, 3));
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

                    //Init DataTable
                    dataTable = new DataTable();
                    dataTable.Columns.Add(new DataColumn("GroupDesignation", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Quantity", typeof(decimal)));
                    //dataTable.Columns.Add(new DataColumn("UnitMeasure", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Total", typeof(decimal)));

                    //If Has data
                    if (workSessionData.FamilyReportItems.Count> 0)
                    {
                        foreach (var item in workSessionData.FamilyReportItems)
                        {
                            designation = item.Designation;
                            quantity = Convert.ToDecimal(item.Quantity);
                            unitMeasure = "Un";
                            total = Convert.ToDecimal(item.Total);

                            //Sum Summary Totals
                            summaryTotalQuantity = quantity;
                            summaryTotal = total;
                            //_logger.Debug(string.Format("Designation: [{0}], quantity: [{1}], unitMeasure: [{2}], total: [{3}]", designation, quantity, unitMeasure, total));
                            //Create Row
                            dataRow = dataTable.NewRow();
                            dataRow[0] = designation;
                            dataRow[1] = quantity;
                            //dataRow[2] = unitMeasure;
                            dataRow[2] = total;
                            dataTable.Rows.Add(dataRow);
                        }
                    }
                    else
                    {
                        //Create Row
                        dataRow = dataTable.NewRow();
                        dataRow[0] = GeneralUtils.GetResourceByName("global_cashdrawer_without_movements");
                        dataRow[1] = 0.0m;
                        //dataRow[2] = string.Empty;//UnitMeasure
                        dataRow[2] = 0.0m;
                        dataTable.Rows.Add(dataRow);
                    }

                    //Add Final Summary Row
                    dataRow = dataTable.NewRow();
                    dataRow[0] = GeneralUtils.GetResourceByName("global_total");
                    dataRow[1] = summaryTotalQuantity;
                    //dataRow[2] = string.Empty;
                    dataRow[2] = summaryTotal;
                    dataTable.Rows.Add(dataRow);

                    //Prepare TicketTable
                    ticketTable = new TicketTable(dataTable, columns, _printer.MaxCharsPerLineNormal);

                    //Custom Print Loop, to Print all Table Rows, and Detect Rows to Print in DoubleHeight (Title and Total)
                    tableCustomPrint = ticketTable.GetTable();
                    WriteLineTextMode rowTextMode;

                    //Dynamic Print All except Last One (Totals), Double Height in Titles
                    for (int x = 0; x < tableCustomPrint.Count - 1; i++)
                    {
                        //Prepare TextMode Based on Row
                        rowTextMode = (i == 0) ? WriteLineTextMode.DoubleHeight : WriteLineTextMode.Normal;
                        //Print Row
                        _printer.WriteLine(tableCustomPrint[i], rowTextMode);
                    }

                    //Line Feed
                    _printer.LineFeed();
                }
            

            //When finish all groups, print Last Row, the Summary Totals Row, Ommited in Custom Print Loop
            _printer.WriteLine(tableCustomPrint[tableCustomPrint.Count - 1], WriteLineTextMode.DoubleHeight);

            return true;
        }


    }


}



