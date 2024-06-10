using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByCurrencyReport : SalesReport
    {
        public SalesByCurrencyReport(
          CustomReportDisplayMode viewMode,
          string filter,
          string readableFilter
          ) : base(
              filter,
              readableFilter,
              "([DocumentFinanceMaster.Currency.Code]) [DocumentFinanceMaster.Currency.Designation]",
              "[DocumentFinanceMaster.Currency.Ord]",
              "REPORT_SALES_PER_CURRENCY",
              viewMode
              )
        {
            Initialize();
        }
    }
}
