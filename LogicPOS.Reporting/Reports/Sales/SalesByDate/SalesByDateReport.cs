using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByDateReport : SalesReport
    {
        public SalesByDateReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            ) : base(
                filter,
                readableFilter,
                "[DocumentFinanceMaster.DocumentDate]",
                "[DocumentFinanceMaster.DocumentDate]",
                "REPORT_SALES_PER_DATE",
                viewMode
                )
        {
            Initialize();
        }
    }
}
