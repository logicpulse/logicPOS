
using LogicPOS.Api.Features.Reports.WorkSession.Common;
using LogicPOS.UI.Printing.Enums;
using LogicPOS.UI.Printing.Tickets;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Printing
{
    public partial class WorkSessionPrinter
    {

        void PrintSubfamilyTotal(WorkSessionData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_subfamily"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var ticketTable = new TicketTable(columns);

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.SubfamilyReportItems)
            {
                summaryTotalQuantity += Convert.ToDecimal(item.Quantity);
                summaryTotal += item.Total;

                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Designation;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                ticketTable.Rows.Add(dataRow);
            }
            var tableCustomPrint = ticketTable.GetTable();

            //Dynamic Print All except Last One (Totals), Double Height in Titles
            for (int x = 0; x < tableCustomPrint.Count; x++)
            {
                if (x == 0)
                {
                    _printer.BoldMode(tableCustomPrint[x]);
                }
                else
                {
                    _printer.Append(tableCustomPrint[x]);
                }
            }
            _printer.Separator(' ');
        }

        void PrintArticleTotal(WorkSessionData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_article"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var ticketTable = new TicketTable(columns);

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.ArticleReportItems)
            {
                summaryTotalQuantity += Convert.ToDecimal(item.Quantity);
                summaryTotal += item.Total;

                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Designation;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                ticketTable.Rows.Add(dataRow);
            }
            var tableCustomPrint = ticketTable.GetTable();

            //Dynamic Print All except Last One (Totals), Double Height in Titles
            for (int x = 0; x < tableCustomPrint.Count; x++)
            {
                if (x == 0)
                {
                    _printer.BoldMode(tableCustomPrint[x]);
                }
                else
                {
                    _printer.Append(tableCustomPrint[x]);
                }
            }
            _printer.Separator(' ');
        }

        void PrintTaxTotal(WorkSessionData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_tax"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var ticketTable = new TicketTable(columns);

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.TaxReportItems)
            {
                summaryTotalQuantity += Convert.ToDecimal(item.Quantity);
                summaryTotal += item.Total;

                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Designation;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                ticketTable.Rows.Add(dataRow);
            }
            var tableCustomPrint = ticketTable.GetTable();

            for (int x = 0; x < tableCustomPrint.Count; x++)
            {
                if (x == 0)
                {
                    _printer.BoldMode(tableCustomPrint[x]);
                }
                else
                {
                    _printer.Append(tableCustomPrint[x]);
                }
            }
            _printer.Separator(' ');
        }

        void PrintPaymentMethodsTotal(WorkSessionData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_payment_method"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var ticketTable = new TicketTable(columns);
            decimal summaryTotalQuantity = 0, summaryTotal = 0;

            foreach (var item in workSessionData.PaymentReportItems)
            {
                summaryTotalQuantity += Convert.ToDecimal(item.Quantity);
                summaryTotal += item.Total;

                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Designation;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                ticketTable.Rows.Add(dataRow);
            }
            var tableCustomPrint = ticketTable.GetTable();

            for (int x = 0; x < tableCustomPrint.Count; x++)
            {
                if (x == 0)
                {
                    _printer.BoldMode(tableCustomPrint[x]);
                }
                else
                {
                    _printer.Append(tableCustomPrint[x]);
                }
            }
            _printer.Separator(' ');
        }

        void PrintDocumentTypeTotal(WorkSessionData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_documentfinance_type"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var ticketTable = new TicketTable(columns);
            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.DocumentTypeReportItems)
            {
                summaryTotalQuantity += Convert.ToDecimal(item.Quantity);
                summaryTotal += item.Total;

                var documentType = "global_documentfinance_type_title_fr";
                documentType = documentType.Substring(0, documentType.Length - 2) + item.Designation.ToLower();

                var dataRow = ticketTable.NewRow();
                dataRow[0] = GeneralUtils.GetResourceByName(documentType);
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                ticketTable.Rows.Add(dataRow);
            }
            var tableCustomPrint = ticketTable.GetTable();

            for (int x = 0; x < tableCustomPrint.Count; x++)
            {
                if (x == 0)
                {
                    _printer.BoldMode(tableCustomPrint[x]);
                }
                else
                {
                    _printer.Append(tableCustomPrint[x]);
                }
            }
            _printer.Separator(' ');
        }

        void PrintHoursTotal(WorkSessionData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_hour"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var ticketTable = new TicketTable(columns);

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.HoursReportItems)
            {
                summaryTotalQuantity += Convert.ToDecimal(item.Quantity);
                summaryTotal += item.Total;

                var hour = item.Date.Hour.ToString();

                var dataRow = ticketTable.NewRow();
                dataRow[0] = hour;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                ticketTable.Rows.Add(dataRow);
            }
            var tableCustomPrint = ticketTable.GetTable();

            for (int x = 0; x < tableCustomPrint.Count; x++)
            {
                if (x == 0)
                {
                    _printer.BoldMode(tableCustomPrint[x]);
                }
                else
                {
                    _printer.Append(tableCustomPrint[x]);
                }
            }
            _printer.Separator(' ');
        }

        void PrintUsersTotal(WorkSessionData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_user"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var ticketTable = new TicketTable(columns);

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.UserReportItems)
            {
                summaryTotalQuantity += Convert.ToDecimal(item.Quantity);
                summaryTotal += item.Total;

                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Designation;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                ticketTable.Rows.Add(dataRow);
            }
            var tableCustomPrint = ticketTable.GetTable();

            for (int x = 0; x < tableCustomPrint.Count; x++)
            {
                if (x == 0)
                {
                    _printer.BoldMode(tableCustomPrint[x]);
                }
                else
                {
                    _printer.Append(tableCustomPrint[x]);
                }
            }
            _printer.Separator(' ');
        }
    }

}