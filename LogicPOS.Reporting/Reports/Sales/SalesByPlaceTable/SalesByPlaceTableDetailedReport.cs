using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByPlaceTableDetailedReport: SalesDetailedReport
    {
        public SalesByPlaceTableDetailedReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            ) : base(
                filter,
                readableFilter,
                 "([DocumentFinanceDetail.PlaceCode]) [DocumentFinanceDetail.PlaceDesignation] / ([DocumentFinanceDetail.PlaceTableCode]) [DocumentFinanceDetail.PlaceTableDesignation]",
                "[DocumentFinanceDetail.PlaceTableOrd]",
                "REPORT_SALES_DETAIL_PER_PLACE_TABLE",
                viewMode
                )
        {

            Initialize();
        }
    }
}
