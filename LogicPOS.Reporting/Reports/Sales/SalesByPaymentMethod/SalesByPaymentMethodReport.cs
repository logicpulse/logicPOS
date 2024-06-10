using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByPaymentMethodReport : SalesReport
    {
        public SalesByPaymentMethodReport(
          CustomReportDisplayMode viewMode,
          string filter,
          string readableFilter
          ) : base(
              filter,
              readableFilter,
              "([DocumentFinanceMaster.PaymentMethod.Code]) [DocumentFinanceMaster.PaymentMethod.Designation]",
              "[DocumentFinanceMaster.PaymentMethod.Ord]",
              "REPORT_SALES_PER_PAYMENT_METHOD",
              viewMode
              )
        {
            Initialize();
        }
    }
}
