using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByTerminalDetailedReport : SalesDetailedReport
    {
        public SalesByTerminalDetailedReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            ) : base(
                filter,
                readableFilter,
                  "([DocumentFinanceDetail.TerminalCode]) [DocumentFinanceDetail.TerminalDesignation]",
                "[DocumentFinanceDetail.TerminalOrd]",
                "REPORT_SALES_DETAIL_PER_TERMINAL",
                viewMode
                )
        {

            Initialize();
        }
    }
}
