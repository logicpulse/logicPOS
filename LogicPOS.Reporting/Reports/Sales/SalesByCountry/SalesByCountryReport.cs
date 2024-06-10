using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByCountryReport : SalesReport
    {
        public SalesByCountryReport(
          CustomReportDisplayMode viewMode,
          string filter,
          string readableFilter
          ) : base(
              filter,
              readableFilter,
              "[DocumentFinanceMaster.EntityCountry]",
              "[DocumentFinanceMaster.EntityCountry]",
              "REPORT_SALES_PER_COUNTRY",
              viewMode
              )
        {
            Initialize();
        }
    }

}
