using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByPaymentConditionDetailedReport : SalesDetailedReport
    {
        public SalesByPaymentConditionDetailedReport(
         CustomReportDisplayMode viewMode,
         string filter,
         string readableFilter
         ) : base(
             filter,
             readableFilter,
             "([DocumentFinanceDetail.PaymentConditionCode]) [DocumentFinanceDetail.PaymentConditionDesignation]",
             "[DocumentFinanceDetail.PaymentConditionOrd]",
             "REPORT_SALES_DETAIL_PER_PAYMENT_CONDITION",
             viewMode
             )
        {
            Initialize();
        }
    }
}
