using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByTerminalReport : SalesReport
    {
        public SalesByTerminalReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            ) : base(
                filter,
                readableFilter,
                 "([DocumentFinanceMaster.CreatedWhere.Code]) [DocumentFinanceMaster.CreatedWhere.Designation]",
                "[DocumentFinanceMaster.CreatedWhere.Ord]",
                "REPORT_SALES_PER_TERMINAL",
                viewMode
                )
        {

            Initialize();
        }
    }
}
