using Gtk;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Components.Modals.Common;
using System;
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
        }

        public static void ShowModal(Window parent)
        {
            var modal = new ReportsModal(parent);
            modal.Run();
            modal.Destroy();
        }
    }
}
