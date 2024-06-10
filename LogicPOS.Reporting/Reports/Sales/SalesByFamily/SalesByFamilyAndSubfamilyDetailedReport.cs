using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByFamilyAndSubfamilyDetailedReport : SalesDetailedReport
    {
        public SalesByFamilyAndSubfamilyDetailedReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            ) : base(
                filter,
                readableFilter,
                "([DocumentFinanceDetail.ArticleFamilyCode]) [DocumentFinanceDetail.ArticleFamilyDesignation] / ([DocumentFinanceDetail.ArticleSubFamilyCode]) [DocumentFinanceDetail.ArticleSubFamilyDesignation]",
                "[DocumentFinanceDetail.ArticleSubFamilyOrd]",
                "REPORT_SALES_DETAIL_PER_FAMILY_AND_SUBFAMILY",
                viewMode
                )
        {

            Initialize();
        }
    }
}
