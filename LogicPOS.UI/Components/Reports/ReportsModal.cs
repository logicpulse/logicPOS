using Gtk;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals.Common;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReportsModal : Modal
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
        private XAccordionChildButton BtnDetailedSalesByDocumentReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_per_finance_document"]);
        private XAccordionChildButton BtnDetailedSalesByDateReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_per_date"]);
        private XAccordionChildButton BtnDetailedSalesByUserReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_per_user"]);
        private XAccordionChildButton BtnDetailedSalesByTerminalReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_per_terminal"]);
        private XAccordionChildButton BtnDetailedSalesByCustomerReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_per_customer"]);
        private XAccordionChildButton BtnDetailedSalesByPaymentMethodReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_per_payment_method"]);
        private XAccordionChildButton BtnDetailedSalesByPaymentConditionReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_per_payment_condition"]);
        private XAccordionChildButton BtnDetailedSalesByCurrencyReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_per_currency"]);
        private XAccordionChildButton BtnDetailedSalesByCountryReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_per_country"]);
        private XAccordionChildButton BtnDetailedSalesByFamilyReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_per_family"]);
        private XAccordionChildButton BtnDetailedSalesBySubfamilyReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_per_family_and_subfamily"]);
        private XAccordionChildButton BtnDetailedSalesByPlaceReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_per_place"]);
        private XAccordionChildButton BtnDetailedSalesByTableReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_per_place_table"]);
        private XAccordionChildButton BtnDetailedSalesByVatReport = new XAccordionChildButton(LocalizedString.Instance["report_sales_detail_group_per_vat"]);
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
        public ReportsModal(Window parent) : base(parent,
                                                  LocalizedString.Instance["global_reports"],
                                                  new Size(500, 509),
                                                  PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_reports.png")
        {
            RegisterPanels();
            ShowPanel(PanelSummarizedFinancialReports);
            AddEventHandlers();
        }

        protected override ActionAreaButtons CreateActionAreaButtons() => null;

        protected override Widget CreateBody()
        {
            Viewport viewport = new Viewport() { ShadowType = ShadowType.None };
            viewport.Add(CreateButtonsPanel());
            viewport.ResizeMode = ResizeMode.Parent;

            var scrolledWindow = new ScrolledWindow();
            scrolledWindow.ShadowType = ShadowType.EtchedIn;
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrolledWindow.Add(viewport);
            scrolledWindow.ResizeMode = ResizeMode.Parent;
            return scrolledWindow;
        }

        private void RegisterPanels()
        {
            Panels.Add(PanelSummarizedFinancialReports);
            Panels.Add(PanelDetailedFinancialReports);
            Panels.Add(PanelAuxiliaryTablesReports);
            Panels.Add(PanelOtherReports);
            Panels.Add(PanelStockReports);
        }
        
        private void ShowPanel(VBox panel)
        {
            foreach (var p in Panels)
            {
                p.Visible = p == panel;
            }
        }

        private void AddSummarizedFinancialReportsPanel(VBox container)
        {
            container.PackStart(BtnSummarizedFinancialReports.Button, false, false, 0);
            {
                PanelSummarizedFinancialReports.PackStart(BtnCompanyBillingReport.Button, false, false, 0);
                PanelSummarizedFinancialReports.PackStart(BtnCustomerBalanceSummaryReport.Button, false, false, 0);
                PanelSummarizedFinancialReports.PackStart(BtnSalesByDocumentReport.Button, false, false, 0);
                PanelSummarizedFinancialReports.PackStart(BtnSalesByDateReport.Button, false, false, 0);
                PanelSummarizedFinancialReports.PackStart(BtnSalesByUserReport.Button, false, false, 0);
                PanelSummarizedFinancialReports.PackStart(BtnSalesByTerminalReport.Button, false, false, 0);
                PanelSummarizedFinancialReports.PackStart(BtnSalesByCustomerReport.Button, false, false, 0);
                PanelSummarizedFinancialReports.PackStart(BtnSalesByPaymentMethodReport.Button, false, false, 0);
                PanelSummarizedFinancialReports.PackStart(BtnSalesByPaymentConditionReport.Button, false, false, 0);
                PanelSummarizedFinancialReports.PackStart(BtnSalesByCurrencyReport.Button, false, false, 0);
                PanelSummarizedFinancialReports.PackStart(BtnSalesByCountryReport.Button, false, false, 0);
                PanelSummarizedFinancialReports.PackStart(BtnSalesByVatReport.Button, false, false, 0);
                PanelSummarizedFinancialReports.PackStart(BtnSalesByVatAndArticleClassReport.Button, false, false, 0);
            }
            container.PackStart(PanelSummarizedFinancialReports, false, false, 0);
            BtnSummarizedFinancialReports.Button.Clicked += (sender, e) => ShowPanel(PanelSummarizedFinancialReports);
        }

        private void AddDetailedFinancialReportsPanel(VBox container)
        {
            container.PackStart(BtnDetailedFinancialReports.Button, false, false, 0);
            {
                PanelDetailedFinancialReports.PackStart(BtnCustomerBalanceDetailsReport.Button, false, false, 0);
                PanelDetailedFinancialReports.PackStart(BtnCurrentAccountReport.Button, false, false, 0);
                PanelDetailedFinancialReports.PackStart(BtnDetailedSalesByDocumentReport.Button, false, false, 0);
                PanelDetailedFinancialReports.PackStart(BtnDetailedSalesByDateReport.Button, false, false, 0);
                PanelDetailedFinancialReports.PackStart(BtnDetailedSalesByUserReport.Button, false, false, 0);
                PanelDetailedFinancialReports.PackStart(BtnDetailedSalesByTerminalReport.Button, false, false, 0);
                PanelDetailedFinancialReports.PackStart(BtnDetailedSalesByCustomerReport.Button, false, false, 0);
                PanelDetailedFinancialReports.PackStart(BtnDetailedSalesByPaymentMethodReport.Button, false, false, 0);
                PanelDetailedFinancialReports.PackStart(BtnDetailedSalesByPaymentConditionReport.Button, false, false, 0);
                PanelDetailedFinancialReports.PackStart(BtnDetailedSalesByCurrencyReport.Button, false, false, 0);
                PanelDetailedFinancialReports.PackStart(BtnDetailedSalesByCountryReport.Button, false, false, 0);
                PanelDetailedFinancialReports.PackStart(BtnDetailedSalesByFamilyReport.Button, false, false, 0);
                PanelDetailedFinancialReports.PackStart(BtnDetailedSalesBySubfamilyReport.Button, false, false, 0);
                PanelDetailedFinancialReports.PackStart(BtnDetailedSalesByPlaceReport.Button, false, false, 0);
                PanelDetailedFinancialReports.PackStart(BtnDetailedSalesByTableReport.Button, false, false, 0);
                PanelDetailedFinancialReports.PackStart(BtnDetailedSalesByVatReport.Button, false, false, 0);

            }
            container.PackStart(PanelDetailedFinancialReports, false, false, 0);
            BtnDetailedFinancialReports.Button.Clicked += (sender, e) => ShowPanel(PanelDetailedFinancialReports);
        }
       
        private void AddAuxiliaryTablesReportsPanel(VBox container)
        {
            container.PackStart(BtnAuxiliaryTablesReports.Button, false, false, 0);
            {
                PanelAuxiliaryTablesReports.PackStart(BtnArticlesReports.Button, false, false, 0);
                PanelAuxiliaryTablesReports.PackStart(BtnCustomersReports.Button, false, false, 0);
            }
            container.PackStart(PanelAuxiliaryTablesReports, false, false, 0);
            BtnAuxiliaryTablesReports.Button.Clicked += (sender, e) => ShowPanel(PanelAuxiliaryTablesReports);
        }

        private void AddOtherReportsPanel(VBox container)
        {
            container.PackStart(BtnOtherTablesReports.Button, false, false, 0);
            {
                PanelOtherReports.PackStart(BtnAuditReport.Button, false, false, 0);
                PanelOtherReports.PackStart(BtnComissionsReport.Button, false, false, 0);
            }
            container.PackStart(PanelOtherReports, false, false, 0);
            BtnOtherTablesReports.Button.Clicked += (sender, e) => ShowPanel(PanelOtherReports);
        }

        private void AddStockReportsPanel(VBox container)
        {
            container.PackStart(BtnStockReports.Button, false, false, 0);
            {
                PanelStockReports.PackStart(BtnStockMovementsReport.Button, false, false, 0);
                PanelStockReports.PackStart(BtnStockByWarehouseReport.Button, false, false, 0);
                PanelStockReports.PackStart(BtnStockByArticleReport.Button, false, false, 0);
                PanelStockReports.PackStart(BtnStockBySupplierReport.Button, false, false, 0);
                PanelStockReports.PackStart(BtnStockByArticleGainReport.Button, false, false, 0);
            }
            container.PackStart(PanelStockReports, false, false, 0);
            BtnStockReports.Button.Clicked += (sender, e) => ShowPanel(PanelStockReports);
        }

        private VBox CreateButtonsPanel()
        {
            var panel = new VBox(false, 2);

            AddSummarizedFinancialReportsPanel(panel);
            AddDetailedFinancialReportsPanel(panel);
            AddAuxiliaryTablesReportsPanel(panel);
            AddOtherReportsPanel(panel);
            AddStockReportsPanel(panel);

            panel.PackStart(BtnOtherTablesReports.Button, false, false, 0);
            panel.PackStart(BtnStockReports.Button, false, false, 0);
            return panel;
        }

    }
}
