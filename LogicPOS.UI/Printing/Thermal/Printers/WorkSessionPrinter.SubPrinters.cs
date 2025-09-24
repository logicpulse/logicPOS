
using LogicPOS.Api.Features.POS.WorkSessions.Movements.GetDayReportData;
using LogicPOS.UI.Printing.Enums;
using LogicPOS.UI.Printing.Tickets;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System.Collections.Generic;

namespace LogicPOS.UI.Printing
{
    public partial class WorkSessionPrinter
    {

        void PrintSubfamilyTotal(DayReportData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_subfamily"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var ticketTable = new TicketTable(columns);

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.GetTotalPerSubfamily())
            {
                summaryTotalQuantity = item.Quantity;
                summaryTotal += item.Total;

                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Subfamily;
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

        void PrintArticleTotal(DayReportData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_article"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var ticketTable = new TicketTable(columns);

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.GetTotalPerArticle())
            {
                summaryTotalQuantity = item.Quantity;
                summaryTotal = item.Total;

                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Article;
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

        void PrintTaxTotal(DayReportData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_tax"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var ticketTable = new TicketTable(columns);

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.GetTotalPerTax())
            {
                summaryTotalQuantity = item.Quantity;
                summaryTotal = item.Total;

                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Tax;
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

        void PrintPaymentMethodsTotal(DayReportData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_payment_method"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var ticketTable = new TicketTable(columns);
            decimal summaryTotalQuantity = 0, summaryTotal = 0;

            foreach (var item in workSessionData.GetTotalPerPaymentMethod())
            {
                summaryTotalQuantity = item.Quantity;
                summaryTotal = item.Total;

                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Method;
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

        void PrintDocumentTypeTotal(DayReportData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_documentfinance_type"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var ticketTable = new TicketTable(columns);
            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.GetTotalPerDocumentType())
            {
                summaryTotalQuantity = item.Quantity;
                summaryTotal = item.Total;

                var documentType = "global_documentfinance_type_title_fr";
                var documentTypeSuffix = (SystemInformationService.SystemInformation.IsAngola && item.DocumentType.ToLower() == "cm") ? "dc" : item.DocumentType.ToLower();
                documentTypeSuffix = (SystemInformationService.SystemInformation.IsAngola && item.DocumentType.ToLower() == "pp") ? "fp" : documentTypeSuffix;

                documentType = documentType.Substring(0, documentType.Length - 2) + documentTypeSuffix;

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

        void PrintHoursTotal(DayReportData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_hour"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var ticketTable = new TicketTable(columns);

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.GetTotalPerHour())
            {
                summaryTotalQuantity = item.Quantity;
                summaryTotal = item.Total;

                var hour = item.Hour;

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

        void PrintUsersTotal(DayReportData workSessionData)
        {
            var columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", GeneralUtils.GetResourceByName("global_user"), 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

            var ticketTable = new TicketTable(columns);

            decimal summaryTotalQuantity = 0, summaryTotal = 0;


            foreach (var item in workSessionData.GetTotalPerUser())
            {
                summaryTotalQuantity = item.Quantity;
                summaryTotal = item.Total;

                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.User;
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