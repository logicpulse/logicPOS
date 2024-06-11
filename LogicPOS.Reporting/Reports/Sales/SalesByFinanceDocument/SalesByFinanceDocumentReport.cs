using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByFinanceDocumentReport : SalesReport
    {
        public SalesByFinanceDocumentReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            ) : base(
                filter,
                readableFilter,
                "([DocumentFinanceMaster.DocumentType.Code]) [DocumentFinanceMaster.DocumentType.Designation]",
                "[DocumentFinanceMaster.DocumentType.Ord]",
                "REPORT_SALES_PER_FINANCE_DOCUMENT",
                viewMode
                )
        {

            Initialize();
        }
    }
}
