using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Reporting.Common;
using LogicPOS.Reporting.Reports.Documents;
using LogicPOS.Settings;
using LogicPOS.Shared.CustomDocument;
using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByUserReport : SalesReport
    {
        public SalesByUserReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            ) : base(
                filter,
                readableFilter,
               "([DocumentFinanceMaster.CreatedBy.Code]) [DocumentFinanceMaster.CreatedBy.Name]",
               "[DocumentFinanceMaster.CreatedBy.Ord]",
                "REPORT_SALES_DETAIL_PER_USER",
                viewMode
                )
        {

            Initialize();
        }
    }
}
