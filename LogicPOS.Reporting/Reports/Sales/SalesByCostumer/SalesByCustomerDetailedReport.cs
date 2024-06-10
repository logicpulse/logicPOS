using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByCustomerDetailedReport : SalesDetailedReport
    {
        public SalesByCustomerDetailedReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            ) : base(
                filter,
                readableFilter,
                "[DocumentFinanceDetail.EntityFiscalNumber] / [DocumentFinanceDetail.EntityName]",
                "[DocumentFinanceDetail.EntityFiscalNumber]",
                "REPORT_SALES_DETAIL_PER_CUSTOMER",
                viewMode
                )
        {

            Initialize();
        }
    }
}
