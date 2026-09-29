using LogicPOS.Api.Features.POS.WorkSessions.Movements.GetDayReportData;
using LogicPOS.UI.Printing.Tickets;
using LogicPOS.UI.Services;
using System.Collections.Generic;
using System.Linq;
using LogicPOS.Globalization;

namespace LogicPOS.UI.Printing
{
    public partial class WorkSessionPrinter
    {
        void PrintSubfamilyTotal(DayReportData workSessionData)
        {
            var rows = workSessionData.GetTotalPerSubfamily();
            var columns = CreateGroupTotalColumns(
                LocalizedString.Instance["global_subfamily"],
                rows.Select(x => x.Quantity),
                rows.Select(x => x.Total));

            var ticketTable = new TicketTable(columns, Layout.Columns);

            foreach (var item in rows)
            {
                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Subfamily;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                ticketTable.Rows.Add(dataRow);
            }

            PrintGroupTable(ticketTable);
        }

        void PrintArticleTotal(DayReportData workSessionData)
        {
            var rows = workSessionData.GetTotalPerArticle();
            var columns = CreateGroupTotalColumns(
                LocalizedString.Instance["global_article"],
                rows.Select(x => x.Quantity),
                rows.Select(x => x.Total));

            var ticketTable = new TicketTable(columns, Layout.Columns);

            foreach (var item in rows)
            {
                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Article;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                ticketTable.Rows.Add(dataRow);
            }

            PrintGroupTable(ticketTable);
        }

        void PrintTaxTotal(DayReportData workSessionData)
        {
            var rows = workSessionData.GetTotalPerTax();
            var columns = CreateGroupTotalColumns(
                LocalizedString.Instance["global_tax"],
                rows.Select(x => x.Quantity),
                rows.Select(x => x.Total));

            var ticketTable = new TicketTable(columns, Layout.Columns);

            foreach (var item in rows)
            {
                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Tax;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                ticketTable.Rows.Add(dataRow);
            }

            PrintGroupTable(ticketTable);
        }

        void PrintPaymentMethodsTotal(DayReportData workSessionData)
        {
            var rows = workSessionData.GetTotalPerPaymentMethod();
            var columns = CreateGroupTotalColumns(
                LocalizedString.Instance["global_payment_method"],
                rows.Select(x => x.Quantity),
                rows.Select(x => x.Total));

            var ticketTable = new TicketTable(columns, Layout.Columns);

            foreach (var item in rows)
            {
                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Method;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                ticketTable.Rows.Add(dataRow);
            }

            PrintGroupTable(ticketTable);
        }

        void PrintDocumentTypeTotal(DayReportData workSessionData)
        {
            var rows = workSessionData.GetTotalPerDocumentType();
            var columns = CreateGroupTotalColumns(
                LocalizedString.Instance["global_documentfinance_type"],
                rows.Select(x => x.Quantity),
                rows.Select(x => x.Total));

            var ticketTable = new TicketTable(columns, Layout.Columns);

            foreach (var item in rows)
            {
                var documentType = "global_documentfinance_type_title_fr";
                var documentTypeSuffix = (SystemInformationService.SystemInformation.IsAngola && item.DocumentType.ToLower() == "cm") ? "dc" : item.DocumentType.ToLower();
                documentTypeSuffix = (SystemInformationService.SystemInformation.IsAngola && item.DocumentType.ToLower() == "pp") ? "fp" : documentTypeSuffix;

                documentType = documentType.Substring(0, documentType.Length - 2) + documentTypeSuffix;

                var dataRow = ticketTable.NewRow();
                dataRow[0] = LocalizedString.Instance[documentType];
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                ticketTable.Rows.Add(dataRow);
            }

            PrintGroupTable(ticketTable);
        }

        void PrintHoursTotal(DayReportData workSessionData)
        {
            var rows = workSessionData.GetTotalPerHour();
            var columns = CreateGroupTotalColumns(
                LocalizedString.Instance["global_hour"],
                rows.Select(x => x.Quantity),
                rows.Select(x => x.Total));

            var ticketTable = new TicketTable(columns, Layout.Columns);

            foreach (var item in rows)
            {
                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Hour;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                ticketTable.Rows.Add(dataRow);
            }

            PrintGroupTable(ticketTable);
        }

        void PrintUsersTotal(DayReportData workSessionData)
        {
            var rows = workSessionData.GetTotalPerUser();
            var columns = CreateGroupTotalColumns(
                LocalizedString.Instance["global_user"],
                rows.Select(x => x.Quantity),
                rows.Select(x => x.Total));

            var ticketTable = new TicketTable(columns, Layout.Columns);

            foreach (var item in rows)
            {
                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.User;
                dataRow[1] = item.Quantity;
                dataRow[2] = item.Total;
                ticketTable.Rows.Add(dataRow);
            }

            PrintGroupTable(ticketTable);
        }

        private void PrintGroupTable(TicketTable ticketTable)
        {
            var tableCustomPrint = ticketTable.GetTable();
            for (var x = 0; x < tableCustomPrint.Count; x++)
            {
                if (x == 0)
                {
                    AppendBoldLine(_printer, tableCustomPrint[x]);
                }
                else
                {
                    _printer.Append(ToThermalText(tableCustomPrint[x]));
                }
            }

            BlankSeparator();
        }
    }
}
