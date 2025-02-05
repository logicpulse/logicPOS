using Gtk;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReportsModal
    {
        #region Components

        #region Summarized Financial Reports
        private XAccordionParentButton BtnSummarizedFinancialReports = new XAccordionParentButton(LocalizedString.Instance["reporttype_label_type1"],
                                                                                                  PathsSettings.ImagesFolderLocation + @"Icons\Reports\report_financial.png");
        private VBox PanelSummarizedFinancialReports = new VBox(false, 2);
        private XAccordionChildButton BtnCompanyBillingReport = new XAccordionChildButton(LocalizedString.Instance["report_company_billing"]);
        private XAccordionChildButton BtnCustomerBalanceSummaryReport = new XAccordionChildButton(LocalizedString.Instance["report_customer_balance_summary"]);
        private XAccordionChildButton BtnSalesByDocumentReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_per_finance_document"]);
        private XAccordionChildButton BtnSalesByDateReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_per_date"]);
        private XAccordionChildButton BtnSalesByUserReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_per_user"]);
        private XAccordionChildButton BtnSalesByTerminalReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_per_terminal"]);
        private XAccordionChildButton BtnSalesByCustomerReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_per_customer"]);
        private XAccordionChildButton BtnSalesByPaymentMethodReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_per_payment_method"]);
        private XAccordionChildButton BtnSalesByPaymentConditionReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_per_payment_condition"]);
        private XAccordionChildButton BtnSalesByCurrencyReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_per_currency"]);
        private XAccordionChildButton BtnSalesByCountryReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_per_country"]);
        private XAccordionChildButton BtnSalesByVatReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_per_vat"]);
        private XAccordionChildButton BtnSalesByVatAndArticleClassReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_per_vat_by_article_class"]);
        #endregion

        #region Detailed Financial Reports
        private XAccordionParentButton BtnDetailedFinancialReports = new XAccordionParentButton(LocalizedString.Instance["reporttype_label_type2"],
                                                                                               PathsSettings.ImagesFolderLocation + @"Icons\Reports\report_financial_detailed.png");
        private VBox PanelDetailedFinancialReports = new VBox(false, 2);
        private XAccordionChildButton BtnCustomerBalanceDetailsReport = new XAccordionChildButton(LocalizedString.Instance["report_customer_balance_details"]);
        private XAccordionChildButton BtnCurrentAccountReport = new XAccordionChildButton(LocalizedString.Instance["report_list_current_account"]);
        private XAccordionChildButton BtnDetailedSalesByDocumentReport = new XAccordionChildButton(LocalizedString.Instance["Vendas por tipo de documento fiscal (Detalhado)"]);
        private XAccordionChildButton BtnDetailedSalesByDateReport = new XAccordionChildButton(LocalizedString.Instance["Vendas por dia (Detalhado)"]);
        private XAccordionChildButton BtnDetailedSalesByUserReport = new XAccordionChildButton(LocalizedString.Instance["Vendas por funcionário (Detalhado)"]);
        private XAccordionChildButton BtnDetailedSalesByTerminalReport = new XAccordionChildButton(LocalizedString.Instance["Vendas por terminal (Detalhado)"]);
        private XAccordionChildButton BtnDetailedSalesByCustomerReport = new XAccordionChildButton(LocalizedString.Instance["Vendas por cliente (Detalhado)"]);
        private XAccordionChildButton BtnDetailedSalesByPaymentMethodReport = new XAccordionChildButton(LocalizedString.Instance["Vendas por método de pagamento (Detalhado)"]);
        private XAccordionChildButton BtnDetailedSalesByPaymentConditionReport = new XAccordionChildButton(LocalizedString.Instance["Vendas por condição de pagamento (Detalhado)"]);
        private XAccordionChildButton BtnDetailedSalesByCurrencyReport = new XAccordionChildButton(LocalizedString.Instance["Vendas por moeda (Detalhado)"]);
        private XAccordionChildButton BtnDetailedSalesByCountryReport = new XAccordionChildButton(LocalizedString.Instance["Vendas por país (Detalhado)"]);
        private XAccordionChildButton BtnDetailedSalesByFamilyReport = new XAccordionChildButton(LocalizedString.Instance["Vendas por família (Detalhado)"]);
        private XAccordionChildButton BtnDetailedSalesBySubfamilyReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_per_family_and_subfamily"]);
        private XAccordionChildButton BtnDetailedSalesByPlaceReport = new XAccordionChildButton(LocalizedString.Instance["Vendas por zona (Detalhado)"]);
        private XAccordionChildButton BtnDetailedSalesByTableReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_per_place_table"]);
        private XAccordionChildButton BtnDetailedSalesByVatReport = new XAccordionChildButton(LocalizedString.Instance["Vendas por taxa IVA (Detalhado/Agrupado)"]);
        #endregion

        #region Grouped Detailed Financial Reports
        private XAccordionParentButton BtnGroupedDetailedFinancialReports = new XAccordionParentButton(LocalizedString.Instance["reporttype_label_type3"],
                                                                                       PathsSettings.ImagesFolderLocation + @"Icons\Reports\report_financial_detailed_grouped.png");

        #endregion

        #region Auxiliary Tables Reports
        private XAccordionParentButton BtnAuxiliaryTablesReports = new XAccordionParentButton(LocalizedString.Instance["reporttype_label_type4"],
                                                                                              PathsSettings.ImagesFolderLocation + @"Icons\Reports\report_auxiliary_tables.png");
        private VBox PanelAuxiliaryTablesReports = new VBox(false, 2);
        private XAccordionChildButton BtnArticlesReports = new XAccordionChildButton(LocalizedString.Instance["report_list_family_subfamily_articles"]);
        private XAccordionChildButton BtnCustomersReports = new XAccordionChildButton(LocalizedString.Instance["report_list_customers"]);
        #endregion

        #region Other Tables Reports
        private XAccordionParentButton BtnOtherTablesReports = new XAccordionParentButton(LocalizedString.Instance["reporttype_label_type5"],
                                                                                                    PathsSettings.ImagesFolderLocation + @"Icons\Reports\report_other_reports.png");
        private VBox PanelOtherReports = new VBox(false, 2);
        private XAccordionChildButton BtnAuditReport = new XAccordionChildButton(LocalizedString.Instance["report_list_audit_table"]);
        private XAccordionChildButton BtnComissionsReport = new XAccordionChildButton(LocalizedString.Instance["report_list_user_commission"]);
        #endregion

        #region Stock Reports
        private XAccordionParentButton BtnStockReports = new XAccordionParentButton(LocalizedString.Instance["reporttype_label_type6"],
                                                                                             PathsSettings.ImagesFolderLocation + @"Icons\Reports\report_other_reports.png");
        private VBox PanelStockReports = new VBox(false, 2);
        private XAccordionChildButton BtnStockMovementsReport = new XAccordionChildButton(LocalizedString.Instance["report_list_stock_movements"]);
        private XAccordionChildButton BtnStockByWarehouseReport = new XAccordionChildButton(LocalizedString.Instance["report_list_stock_warehouse"]);
        private XAccordionChildButton BtnStockByArticleReport = new XAccordionChildButton(LocalizedString.Instance["report_list_stock_article"]);
        private XAccordionChildButton BtnStockBySupplierReport = new XAccordionChildButton(LocalizedString.Instance["report_list_stock_supplier"]);
        private XAccordionChildButton BtnStockByArticleGainReport = new XAccordionChildButton(LocalizedString.Instance["report_list_stock_gain"]);
        #endregion

        private List<VBox> Panels = new List<VBox>();
        #endregion
    }
}
