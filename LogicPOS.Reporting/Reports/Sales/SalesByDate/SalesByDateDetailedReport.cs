using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByDateDetailedReport : SalesDetailedReport
    {
        public SalesByDateDetailedReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            ) : base(
                filter,
                readableFilter,
                "[DocumentFinanceDetail.DocumentDate]",
                "[DocumentFinanceDetail.DocumentDate]",
                "REPORT_SALES_DETAIL_PER_DATE",
                viewMode
                )
        {
            Initialize();
        }
    }
}
