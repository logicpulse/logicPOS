using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByFinanceDocumentDetailedReport : SalesDetailedReport
    {
        public SalesByFinanceDocumentDetailedReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            ) : base(
                filter,
                readableFilter,
                "([DocumentFinanceDetail.DocumentTypeCode]) [DocumentFinanceDetail.DocumentTypeDesignation]",
                "[DocumentFinanceDetail.DocumentTypeOrd]",
                "REPORT_SALES_DETAIL_PER_FINANCE_DOCUMENT",
                viewMode
                )
        {

            Initialize();
        }
    }
}
