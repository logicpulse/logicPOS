using LogicPOS.UI.Printing.Enums;
using LogicPOS.UI.Printing.Tickets;
using LogicPOS.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Printing
{
    public partial class WorkSessionPrinter
    {
        private const string GroupDecimalFormat = "{0:0.00}";

        /// <summary>
        /// Title (dynamic) + Qnt + Total sized from the actual formatted values so large amounts are not clipped.
        /// </summary>
        private List<TicketColumn> CreateGroupTotalColumns(
            string groupTitle,
            IEnumerable<decimal> quantities,
            IEnumerable<decimal> totals)
        {
            var qtyTitle = LocalizedString.Instance["global_quantity_acronym"];
            var totalTitle = LocalizedString.Instance["global_totalfinal_acronym"];

            var qtyList = (quantities ?? Enumerable.Empty<decimal>()).DefaultIfEmpty(0m).ToList();
            var totalList = (totals ?? Enumerable.Empty<decimal>()).DefaultIfEmpty(0m).ToList();

            // Non-last columns reserve 1 char for the divider; last column uses full width.
            var qtyContent = qtyList.Max(q => string.Format(GroupDecimalFormat, q).Length);
            var totalContent = totalList.Max(t => string.Format(GroupDecimalFormat, t).Length);

            var qtyWidth = Math.Max(qtyTitle.Length + 1, qtyContent + 1);
            var totalWidth = Math.Max(totalTitle.Length, totalContent);

            const int minTitleWidth = 6;
            var maxFixed = Math.Max(8, Layout.Columns - minTitleWidth);
            if (qtyWidth + totalWidth > maxFixed)
            {
                // Prefer keeping the total amount intact.
                totalWidth = Math.Min(totalWidth, maxFixed - 4);
                qtyWidth = Math.Max(4, maxFixed - totalWidth);
            }

            return new List<TicketColumn>
            {
                new TicketColumn("GroupTitle", groupTitle, 0, TicketColumnsAlignment.Left),
                new TicketColumn("Quantity", qtyTitle, qtyWidth, TicketColumnsAlignment.Right, typeof(decimal), GroupDecimalFormat),
                new TicketColumn("Total", totalTitle, totalWidth, TicketColumnsAlignment.Right, typeof(decimal), GroupDecimalFormat)
            };
        }

        private List<TicketColumn> CreateHeaderSummaryColumns(IEnumerable<string> values)
        {
            var valueWidth = (values ?? Enumerable.Empty<string>())
                .Select(v => string.IsNullOrEmpty(v) ? 0 : v.Length)
                .DefaultIfEmpty(12)
                .Max();

            valueWidth = Math.Max(valueWidth, 12);
            valueWidth = Math.Min(valueWidth, Math.Max(12, Layout.Columns - 10));
            var labelWidth = Math.Max(8, Layout.Columns - valueWidth);

            return new List<TicketColumn>
            {
                new TicketColumn("Label", "", labelWidth, TicketColumnsAlignment.Right),
                new TicketColumn("Value", "", valueWidth, TicketColumnsAlignment.Left)
            };
        }
    }
}
