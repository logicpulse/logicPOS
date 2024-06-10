using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByCustomerReport : SalesReport
    {
        public SalesByCustomerReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            ) : base(
                filter,
                readableFilter,
               "[DocumentFinanceMaster.EntityFiscalNumber] / [DocumentFinanceMaster.EntityName]",
                "[DocumentFinanceMaster.EntityFiscalNumber]",
                "REPORT_SALES_PER_CUSTOMER",
                viewMode
                )
        {

            Initialize();
        }
    }
}
