using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByPaymentConditionReport : SalesReport
    {
        public SalesByPaymentConditionReport(
         CustomReportDisplayMode viewMode,
         string filter,
         string readableFilter
         ) : base(
             filter,
             readableFilter,
             "([DocumentFinanceMaster.PaymentCondition.Code]) [DocumentFinanceMaster.PaymentCondition.Designation]",
             "[DocumentFinanceMaster.PaymentCondition.Ord]",
             "REPORT_SALES_PER_PAYMENT_CONDITION",
             viewMode
             )
        {
            Initialize();
        }
    }
}
