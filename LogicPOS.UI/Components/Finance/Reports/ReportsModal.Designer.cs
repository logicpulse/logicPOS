
using Gtk;
using LogicPOS.UI.Buttons;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReportsModal
    {
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

    }
}
