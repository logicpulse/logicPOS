using LogicPOS.Api.Entities.Enums;
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

namespace LogicPOS.Printing.Documents
{
    public class WorkSession : BaseInternalTemplate
    {
        private readonly WorkSessionData _workSessionReceiptsData;
        private readonly WorkSessionData _workSessionDocumentsData;
        public WorkSession(PrinterDto printer, string terminalDesignation,string userName, WorkSessionData workSessionDocumentsData, WorkSessionData workSessionReceiptsData, CompanyPrintingInformationsDto companyPrintingInformationsDto) : base(printer, terminalDesignation, userName)
        {
            _printer = new GenericThermalPrinter(printer);
            _workSessionDocumentsData = workSessionDocumentsData;
            _workSessionReceiptsData = workSessionReceiptsData;
            _companyInformationsDto = companyPrintingInformationsDto;
            _terminalDesignation = terminalDesignation;
            _userName = userName;
            DefineTicketTitle();
            DefineTicketSubtitle();
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
            if (_workSessionReceiptsData.WorkSession.Type==WorkSessionPeriodType.Day)
            {
                if (_workSessionReceiptsData.WorkSession.Status==WorkSessionPeriodStatus.Open)
                {
                    _ticketTitle = GeneralUtils.GetResourceByName("ticket_title_worksession_day_resume");
                }
                else
                {
                    _ticketTitle = GeneralUtils.GetResourceByName("ticket_title_worksession_day_close");
                }
                return;
            }

            if (_workSessionReceiptsData.WorkSession.Status==WorkSessionPeriodStatus.Open)
            {
                _ticketTitle = GeneralUtils.GetResourceByName("ticket_title_worksession_terminal_resume");
            }
            else
            {
                _ticketTitle = GeneralUtils.GetResourceByName("ticket_title_worksession_terminal_close");
            }
            _ticketSubTitle = _workSessionReceiptsData.WorkSession.Type==WorkSessionPeriodType.Terminal? _terminalDesignation : string.Empty;
        }

        public override void PrintContent()
        {
            PrintHeader();
            PrintTitles(_ticketTitle);
            _printer.SetAlignCenter();
            PrintWorkSessionMovement(_workSessionDocumentsData, _workSessionReceiptsData);
        }


        public bool PrintWorkSessionMovement(
            WorkSessionData workSessionDocumentsData,
           WorkSessionData workSessionReceiptsData
           )
        {
            string dateCloseDisplay = workSessionDocumentsData.WorkSession.StartDate.ToString(CultureSettings.DateTimeFormat);


            //Print Header Summary
            DataRow dataRow = null;
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Label", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Value", typeof(string)));
            //Open DateTime
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_open_datetime"));
            dataRow[1] = workSessionDocumentsData.WorkSession.StartDate.ToString(CultureSettings.DateTimeFormat);
            dataTable.Rows.Add(dataRow);
            //Close DataTime
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_close_datetime"));
            dataRow[1] = workSessionDocumentsData.WorkSession.EndDate?.ToString(CultureSettings.DateTimeFormat);
            dataTable.Rows.Add(dataRow);
            //Open Total CashDrawer
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_open_total_cashdrawer"));
            dataRow[1] = (workSessionDocumentsData.OpenTotal).ToString(); 
            dataTable.Rows.Add(dataRow);
            //Close Total CashDrawer
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_close_total_cashdrawer"));
            dataRow[1] = (workSessionDocumentsData.Total).ToString();
            dataTable.Rows.Add(dataRow);
            //Total Money In
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_total_money_in"));
            dataRow[1] = (workSessionDocumentsData.TotalIn).ToString();
            dataTable.Rows.Add(dataRow);
            //Total Money Out
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_total_money_out"));
            dataRow[1] = (workSessionDocumentsData.TotalOut ).ToString();
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
            int groupPositionTitlePayments = (workSessionDocumentsData.WorkSession.Type== WorkSessionPeriodType.Day)? 9 : 8;
            //If CurrentAccount Mode decrease 1, it dont have PaymentMethods
            //groupPositionTitlePayments--;


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
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_family"), 0, TicketColumnsAlignment.Left),
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
            if (workSessionDocumentsData.FamilyReportItems.Count > 0)
            {
                foreach (var item in workSessionDocumentsData.FamilyReportItems)
                {
                    designation = item.Designation;
                    quantity = Convert.ToDecimal(item.Quantity);
                    unitMeasure = "Un";
                    total = Convert.ToDecimal(item.Total);
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
            summaryTotal = workSessionDocumentsData.Total;
            summaryTotalQuantity = workSessionDocumentsData.UserReportItems.Sum(x => x.Quantity);
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
            for (int x = 0; x < tableCustomPrint.Count - 1; x++)
            {
                //Prepare TextMode Based on Row
                rowTextMode = (x == 0) ? WriteLineTextMode.DoubleHeight : WriteLineTextMode.Normal;
                //Print Row
                _printer.WriteLine(tableCustomPrint[x], rowTextMode);
               
            }

            _printer.LineFeed();

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
            _printer.LineFeed();

            //When finish all groups, print Last Row, the Summary Totals Row, Ommited in Custom Print Loop
            _printer.WriteLine(tableCustomPrint[tableCustomPrint.Count - 1], WriteLineTextMode.DoubleHeight);


            _printer.LineFeed();

            _printer.WriteLine(GeneralUtils.GetResourceByName("global_worksession_resume_paymens_documents"), WriteLineTextMode.Big);
            _printer.LineFeed();
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
            
            //Line Feed
            _printer.LineFeed();

            //When finish all groups, print Last Row, the Summary Totals Row, Ommited in Custom Print Loop
            _printer.WriteLine(tableCustomPrint[tableCustomPrint.Count - 1], WriteLineTextMode.DoubleHeight);
            _printer.LineFeed();
            return true;
        }
        

        void PrintSubfamilyTotal(WorkSessionData workSessionData )
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_subfamily"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

                var dataTable = new DataTable();
                dataTable.Columns.Add(new DataColumn("GroupDesignation", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Quantity", typeof(decimal)));
                dataTable.Columns.Add(new DataColumn("Total", typeof(decimal)));

                decimal summaryTotalQuantity=0, summaryTotal=0;


                foreach (var item in workSessionData.SubfamilyReportItems)
                {
                     summaryTotalQuantity += Convert.ToDecimal(item.Quantity);
                     summaryTotal += item.Total;

                    var dataRow = dataTable.NewRow();
                    dataRow[0] = item.Designation;
                    dataRow[1] = item.Quantity;
                    dataRow[2] = item.Total;
                    dataTable.Rows.Add(dataRow);
                }
                var ticketTable = new TicketTable(dataTable, columns, _printer.MaxCharsPerLineNormal);
                var tableCustomPrint = ticketTable.GetTable();

                //Dynamic Print All except Last One (Totals), Double Height in Titles
                for (int x = 0; x < tableCustomPrint.Count; x++)
                {
                    //Prepare TextMode Based on Row
                    var rowTextMode = (x == 0) ? WriteLineTextMode.DoubleHeight : WriteLineTextMode.Normal;
                    //Print Row
                    _printer.WriteLine(tableCustomPrint[x], rowTextMode);
                }
            _printer.LineFeed();
        }

        void PrintArticleTotal(WorkSessionData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_article"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("GroupDesignation", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Quantity", typeof(decimal)));
            dataTable.Columns.Add(new DataColumn("Total", typeof(decimal)));

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.ArticleReportItems)
            {
                summaryTotalQuantity += Convert.ToDecimal(item.Quantity);
                summaryTotal += item.Total;

                var dataRow = dataTable.NewRow();
                dataRow[0] = item.Designation;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                dataTable.Rows.Add(dataRow);
            }
            var ticketTable = new TicketTable(dataTable, columns, _printer.MaxCharsPerLineNormal);
            var tableCustomPrint = ticketTable.GetTable();

            //Dynamic Print All except Last One (Totals), Double Height in Titles
            for (int x = 0; x < tableCustomPrint.Count; x++)
            {
                //Prepare TextMode Based on Row
                var rowTextMode = (x == 0) ? WriteLineTextMode.DoubleHeight : WriteLineTextMode.Normal;
                //Print Row
                _printer.WriteLine(tableCustomPrint[x], rowTextMode);
            }
            _printer.LineFeed();
        }

        void PrintTaxTotal(WorkSessionData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_tax"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("GroupDesignation", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Quantity", typeof(decimal)));
            dataTable.Columns.Add(new DataColumn("Total", typeof(decimal)));

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.TaxReportItems)
            {
                summaryTotalQuantity += Convert.ToDecimal(item.Quantity);
                summaryTotal += item.Total;

                var dataRow = dataTable.NewRow();
                dataRow[0] = item.Designation;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                dataTable.Rows.Add(dataRow);
            }
            var ticketTable = new TicketTable(dataTable, columns, _printer.MaxCharsPerLineNormal);
            var tableCustomPrint = ticketTable.GetTable();

            for (int x = 0; x < tableCustomPrint.Count; x++)
            {
                var rowTextMode = (x == 0) ? WriteLineTextMode.DoubleHeight : WriteLineTextMode.Normal;
                _printer.WriteLine(tableCustomPrint[x], rowTextMode);
            }
            _printer.LineFeed();
        }


        void PrintPaymentMethodsTotal(WorkSessionData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_payment_method"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("GroupDesignation", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Quantity", typeof(decimal)));
            dataTable.Columns.Add(new DataColumn("Total", typeof(decimal)));

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.PaymentReportItems)
            {
                summaryTotalQuantity += Convert.ToDecimal(item.Quantity);
                summaryTotal += item.Total;

                var dataRow = dataTable.NewRow();
                dataRow[0] = item.Designation;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                dataTable.Rows.Add(dataRow);
            }
           var ticketTable = new TicketTable(dataTable, columns, _printer.MaxCharsPerLineNormal);
            var tableCustomPrint = ticketTable.GetTable();

            for (int x = 0; x < tableCustomPrint.Count; x++)
            {
                var rowTextMode = (x == 0) ? WriteLineTextMode.DoubleHeight : WriteLineTextMode.Normal;
                _printer.WriteLine(tableCustomPrint[x], rowTextMode);
            }
            _printer.LineFeed();
        }

        void PrintDocumentTypeTotal(WorkSessionData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_documentfinance_type"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("GroupDesignation", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Quantity", typeof(decimal)));
            dataTable.Columns.Add(new DataColumn("Total", typeof(decimal)));

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.DocumentTypeReportItems)
            {
                summaryTotalQuantity += Convert.ToDecimal(item.Quantity);
                summaryTotal += item.Total;

                var documentType = "global_documentfinance_type_title_fr";
                documentType = documentType.Substring(0, documentType.Length - 2) + item.Designation.ToLower();

                var dataRow = dataTable.NewRow();
                dataRow[0] = GeneralUtils.GetResourceByName(documentType);
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                dataTable.Rows.Add(dataRow);
            }
            var ticketTable = new TicketTable(dataTable, columns, _printer.MaxCharsPerLineNormal);
            var tableCustomPrint = ticketTable.GetTable();

            //Dynamic Print All except Last One (Totals), Double Height in Titles
            for (int x = 0; x < tableCustomPrint.Count; x++)
            {
                //Prepare TextMode Based on Row
                var rowTextMode = (x == 0) ? WriteLineTextMode.DoubleHeight : WriteLineTextMode.Normal;
                //Print Row
                _printer.WriteLine(tableCustomPrint[x], rowTextMode);
            }
            _printer.LineFeed();
        }

        void PrintHoursTotal(WorkSessionData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_hour"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("GroupDesignation", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Quantity", typeof(decimal)));
            dataTable.Columns.Add(new DataColumn("Total", typeof(decimal)));

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.HoursReportItems)
            {
                summaryTotalQuantity += Convert.ToDecimal(item.Quantity);
                summaryTotal += item.Total;

                var hour = item.Date.Hour.ToString();

                var dataRow = dataTable.NewRow();
                dataRow[0] = hour;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                dataTable.Rows.Add(dataRow);
            }
            var ticketTable = new TicketTable(dataTable, columns, _printer.MaxCharsPerLineNormal);
            var tableCustomPrint = ticketTable.GetTable();

            //Dynamic Print All except Last One (Totals), Double Height in Titles
            for (int x = 0; x < tableCustomPrint.Count; x++)
            {
                //Prepare TextMode Based on Row
                var rowTextMode = (x == 0) ? WriteLineTextMode.DoubleHeight : WriteLineTextMode.Normal;
                //Print Row
                _printer.WriteLine(tableCustomPrint[x], rowTextMode);
            }
            _printer.LineFeed();
        }


        void PrintUsersTotal(WorkSessionData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_user"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("GroupDesignation", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Quantity", typeof(decimal)));
            dataTable.Columns.Add(new DataColumn("Total", typeof(decimal)));

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.UserReportItems)
            {
                summaryTotalQuantity += Convert.ToDecimal(item.Quantity);
                summaryTotal += item.Total;

                var dataRow = dataTable.NewRow();
                dataRow[0] = item.Designation;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                dataTable.Rows.Add(dataRow);
            }
            var ticketTable = new TicketTable(dataTable, columns, _printer.MaxCharsPerLineNormal);
            var tableCustomPrint = ticketTable.GetTable();

            //Dynamic Print All except Last One (Totals), Double Height in Titles
            for (int x = 0; x < tableCustomPrint.Count; x++)
            {
                //Prepare TextMode Based on Row
                var rowTextMode = (x == 0) ? WriteLineTextMode.DoubleHeight : WriteLineTextMode.Normal;
                //Print Row
                _printer.WriteLine(tableCustomPrint[x], rowTextMode);
            }
            _printer.LineFeed();
        }


    }


}



