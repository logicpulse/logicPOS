using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByFamilyDetailedReport : SalesDetailedReport
    {
        public SalesByFamilyDetailedReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
        ) : base(
            filter,
            readableFilter,
            "([DocumentFinanceDetail.ArticleFamilyCode]) [DocumentFinanceDetail.ArticleFamilyDesignation]",
            "[DocumentFinanceDetail.ArticleFamilyOrd]",
            "REPORT_SALES_DETAIL_PER_FAMILY",
            viewMode
         )
        {
            Initialize();
        }

    }
}
