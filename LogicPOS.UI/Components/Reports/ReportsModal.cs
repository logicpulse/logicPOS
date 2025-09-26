using Gtk;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Settings;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReportsModal : Modal
    {
        private ReportsModal(Window parent) : base(parent,
                                                  LocalizedString.Instance["global_reports"],
                                                  new Size(500, 509),
                                                  AppSettings.Paths.Images + @"Icons\Windows\icon_window_reports.png")
        {
            RegisterPanels();
            ShowPanel(PanelSummarizedFinancialReports);
            AddEventHandlers();
            UpdatePrivileges();
        }

        private void ShowPanel(VBox panel)
        {
            foreach (var p in Panels)
            {
                p.Visible = p == panel;
            }
        }

        private void AddEventHandlers()
        {
            BtnCompanyBillingReport.Button.Clicked += BtnCompanyBillingReport_Clicked;
            BtnCustomerBalanceSummaryReport.Button.Clicked += BtnCustomerBalanceSummaryReport_Clicked;
            BtnSalesByDocumentReport.Button.Clicked += BtnSalesByDocumentReport_Clicked;
            BtnSalesByDateReport.Button.Clicked += BtnSalesByDateReport_Clicked;
            BtnSalesByUserReport.Button.Clicked += BtnSalesByUserReport_Clicked;
            BtnSalesByTerminalReport.Button.Clicked += BtnSalesByTerminalReport_Clicked;
            BtnSalesByCustomerReport.Button.Clicked += BtnSalesByCustomerReport_Clicked;
            BtnSalesByPaymentMethodReport.Button.Clicked += BtnSalesByPaymentMethodReport_Clicked;
            BtnSalesByPaymentConditionReport.Button.Clicked += BtnSalesByPaymentConditionReport_Clicked;
            BtnSalesByCurrencyReport.Button.Clicked += BtnSalesByCurrencyReport_Clicked;
            BtnSalesByCountryReport.Button.Clicked += BtnSalesByCountryReport_Clicked;
            BtnSalesByVatReport.Button.Clicked += BtnSalesByVatAndArticleTypeReport_Clicked;
            BtnSalesByVatAndArticleClassReport.Button.Clicked += BtnSalesByVatAndArticleClassReport_Clicked;
            BtnDetailedSalesByCustomerReport.Button.Clicked += BtnDetailedSalesByCustomerReport_Clicked;
            BtnCustomerBalanceDetailsReport.Button.Clicked += BtnCustomerBalanceDetailsReport_Clicked;
            BtnDetailedSalesByDocumentReport.Button.Clicked += BtnDetailedSalesByDocumentReport_Clicked;
            BtnDetailedSalesByDateReport.Button.Clicked += BtnDetailedSalesByDateReport_Clicked;
            BtnDetailedSalesByUserReport.Button.Clicked += BtnDetailedSalesByUserReport_Clicked;
            BtnDetailedSalesByTerminalReport.Button.Clicked += BtnDetailedSalesByTerminalReport_Clicked;
            BtnDetailedSalesByPaymentConditionReport.Button.Clicked += BtnDetailedSalesByPaymentConditionReport_Clicked;
            BtnDetailedSalesByPaymentMethodReport.Button.Clicked += BtnDetailedSalesByPaymentMethodReport_Clicked;
            BtnDetailedSalesByCurrencyReport.Button.Clicked += BtnDetailedSalesByCurrencyReport_Clicked;
            BtnDetailedSalesByCountryReport.Button.Clicked += BtnDetailedSalesByCountryReport_Clicked;
            BtnDetailedSalesByFamilyReport.Button.Clicked += BtnDetailedSalesByFamilyReport_Clicked;
            BtnDetailedSalesBySubfamilyReport.Button.Clicked += BtnDetailedSalesBySubfamilyReport_Clicked;
            BtnDetailedSalesByPlaceReport.Button.Clicked += BtnDetailedSalesByPlaceReport_Clicked;
            BtnDetailedSalesByTableReport.Button.Clicked += BtnDetailedSalesByTableReport_Clicked;
            BtnDetailedSalesByVatReport.Button.Clicked += BtnDetailedSalesByVatGroupReport_Clicked;
            BtnArticlesReports.Button.Clicked += BtnArticlesReport_Clicked;
            BtnCustomersReports.Button.Clicked += BtnCustomersReport_Clicked;
            BtnComissionsReport.Button.Clicked += BtnCommissionsReport_Clicked;
            BtnStockMovementsReport.Button.Clicked += BtnStockMovementsReport_Clicked;
            BtnStockByWarehouseReport.Button.Clicked += BtnStockByWarehouseReport_Clicked;
            BtnStockByArticleReport.Button.Clicked += BtnStockByArticleReport_Clicked;
            BtnStockBySupplierReport.Button.Clicked += BtnStockBySupplierReport_Clicked;
            BtnStockByArticleGainReport.Button.Clicked += BtnStockByArticleGainReport_Clicked;
            BtnArticleTotalSoldReport.Button.Clicked += BtnArticleTotalSoldReport_Clicked;
            BtnDeletedOrdersReport.Button.Clicked += BtnDeletedOrdersReport_Clicked;
            BtnAuditReport.Button.Clicked += BtnAuditReport_Clicked;
            
        }



        public static void ShowModal(Window parent)
        {
            var modal = new ReportsModal(parent);
            modal.Run();
            modal.Destroy();
        }

        public void UpdatePrivileges()
        {
            BtnCustomerBalanceSummaryReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_CUSTOMER_BALANCE_SUMMARY");
            BtnAuditReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_LIST_AUDIT_TABLE");
            BtnCustomersReports.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_LIST_CUSTOMERS");
            BtnArticlesReports.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_LIST_FAMILY_SUBFAMILY_ARTICLES");
            BtnStockMovementsReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_LIST_STOCK_MOVEMENTS");
            BtnComissionsReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_LIST_USER_COMMISSION");
            BtnSalesByCountryReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_PER_COUNTRY");
            BtnSalesByCurrencyReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_PER_CURRENCY");
            BtnSalesByCustomerReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_PER_CUSTOMER");
            BtnSalesByDateReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_PER_DATE");
            BtnSalesByDocumentReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_PER_FINANCE_DOCUMENT");
            BtnSalesByPaymentConditionReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_PER_PAYMENT_CONDITION");
            BtnSalesByPaymentMethodReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_PER_PAYMENT_METHOD");
            BtnSalesByTerminalReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_PER_TERMINAL");
            BtnSalesByUserReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_PER_USER");
            BtnStockByWarehouseReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_LIST_STOCK_WAREHOUSE");
            BtnStockByArticleReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_LIST_STOCK_ARTICLE");
            BtnStockBySupplierReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_LIST_STOCK_SUPPLIER");
            BtnStockByArticleGainReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_LIST_STOCK_GAIN");
            BtnSalesByVatReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_PER_VAT");
            BtnSalesByVatAndArticleClassReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_PER_VAT_BY_ARTICLE_CLASS");
            BtnDetailedSalesByCountryReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_PER_COUNTRY");
            BtnDetailedSalesByCurrencyReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_PER_CURRENCY");
            BtnDetailedSalesByCustomerReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_PER_CUSTOMER");
            BtnDetailedSalesByDateReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_PER_DATE");
            BtnDetailedSalesByDocumentReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_PER_FINANCE_DOCUMENT");
            BtnDetailedSalesByFamilyReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_PER_FAMILY");
            BtnDetailedSalesByPaymentConditionReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_PER_PAYMENT_CONDITION");
            BtnDetailedSalesByPaymentMethodReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_PER_PAYMENT_METHOD");
            BtnDetailedSalesByPlaceReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_PER_PLACE");
            BtnDetailedSalesByTableReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_PER_PLACE_TABLE");
            BtnDetailedSalesByTerminalReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_PER_TERMINAL");
            BtnDetailedSalesByUserReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_PER_USER");
            BtnDetailedSalesByVatReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_GROUP_PER_VAT");
            BtnDetailedSalesBySubfamilyReport.Button.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_PER_FAMILY_AND_SUBFAMILY");
        }
    }
}
