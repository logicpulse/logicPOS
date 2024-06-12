using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByVatGroupDetailedReport : SalesDetailedReport
    {
        public SalesByVatGroupDetailedReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            ) : base(
                filter,
                readableFilter,
               "[DocumentFinanceDetail.ArticleVat]",
               "[DocumentFinanceDetail.ArticleVat]",
                "REPORT_SALES_DETAIL_GROUP_PER_VAT",
                viewMode
                )
        {

            Initialize();
        }
    }
}
