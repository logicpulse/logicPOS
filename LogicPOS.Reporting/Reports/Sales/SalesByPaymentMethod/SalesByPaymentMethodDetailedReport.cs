using logicpos.shared.Enums;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByPaymentMethodDetailedReport : SalesDetailedReport
    {
        public SalesByPaymentMethodDetailedReport(
          CustomReportDisplayMode viewMode,
          string filter,
          string readableFilter
          ) : base(
              filter,
              readableFilter,
              "([DocumentFinanceDetail.PaymentMethodCode]) [DocumentFinanceDetail.PaymentMethodDesignation]",
              "[DocumentFinanceDetail.PaymentMethodOrd]",
              "REPORT_SALES_DETAIL_PER_PAYMENT_METHOD",
              viewMode
              )
        {
            Initialize();
        }
    }
}
