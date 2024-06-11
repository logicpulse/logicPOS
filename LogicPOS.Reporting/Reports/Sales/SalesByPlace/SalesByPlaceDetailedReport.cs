using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByPlaceDetailedReport : SalesDetailedReport
    {
        public SalesByPlaceDetailedReport(
           CustomReportDisplayMode viewMode,
           string filter,
           string readableFilter
           ) : base(
               filter,
               readableFilter,
                 "([DocumentFinanceDetail.PlaceCode]) [DocumentFinanceDetail.PlaceDesignation]",
               "[DocumentFinanceDetail.PlaceOrd]",
               "REPORT_SALES_DETAIL_PER_PLACE",
               viewMode
               )
        {

            Initialize();
        }
    }
}
