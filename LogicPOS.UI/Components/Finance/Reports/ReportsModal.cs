using Gtk;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Components.Modals.Common;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReportsModal : Modal
    {
        private ReportsModal(Window parent) : base(parent,
                                                  LocalizedString.Instance["global_reports"],
                                                  new Size(500, 509),
                                                  PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_reports.png")
        {
            RegisterPanels();
            ShowPanel(PanelSummarizedFinancialReports);
            AddEventHandlers();
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

        }

        public static void ShowModal(Window parent)
        {
            var modal = new ReportsModal(parent);
            modal.Run();
            modal.Destroy();
        }
    }
}
