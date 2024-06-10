using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByCurrencyDetailedReport : SalesDetailedReport
    {
        public SalesByCurrencyDetailedReport(
          CustomReportDisplayMode viewMode,
          string filter,
          string readableFilter
          ) : base(
              filter,
              readableFilter,
              "([DocumentFinanceDetail.CurrencyCode]) [DocumentFinanceDetail.CurrencyDesignation]",
              "[DocumentFinanceDetail.CurrencyOrd]",
              "REPORT_SALES_DETAIL_PER_CURRENCY",
              viewMode
              )
        {
            Initialize();
        }
    }
}
