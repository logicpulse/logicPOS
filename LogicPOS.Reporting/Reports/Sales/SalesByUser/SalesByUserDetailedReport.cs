using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByUserDetailedReport : SalesDetailedReport
    {
        public SalesByUserDetailedReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            ) : base(
                filter,
                readableFilter,
               "([DocumentFinanceDetail.UserDetailCode]) [DocumentFinanceDetail.UserDetailName]",
               "[DocumentFinanceDetail.UserDetailOrd]",
                "REPORT_SALES_DETAIL_PER_USER",
                viewMode
                )
        {

            Initialize();
        }
    }
}
