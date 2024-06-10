using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByCountryDetailedReport : SalesDetailedReport
    {
        public SalesByCountryDetailedReport(
          CustomReportDisplayMode viewMode,
          string filter,
          string readableFilter
          ) : base(
              filter,
              readableFilter,
              "([DocumentFinanceDetail.EntityCountryCode2]) [DocumentFinanceDetail.CountryDesignation]",
              "[DocumentFinanceDetail.CountryOrd]",
              "REPORT_SALES_DETAIL_PER_COUNTRY",
              viewMode
              )
        {
            Initialize();
        }
    }

}
