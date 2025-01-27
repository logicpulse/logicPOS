using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Application;
using System;

namespace LogicPOS.UI.Components.Pages
{
    internal partial class DashBoardPage : Box
    {
        private readonly Window _parentWindow;



        public DashBoardPage(Window parentWindow)
        {
            _parentWindow = parentWindow;
            int fontGenericTreeViewColumn = Convert.ToInt16(AppSettings.Instance.fontGenericTreeViewColumn);
            var predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "PosBaseWindow");
            var themeWindow = LogicPOSAppContext.Theme.Theme.Frontoffice.Window.Find(predicate);

            InitializeButtons();
            AddEventHandlers();

            Design(parentWindow, themeWindow);

            ShowAll();
        }

        private void AddEventHandlers()
        {
            BtnTerminals.Clicked += BtnTerminals_Clicked;
            BtnPreferenceParameters.Clicked += BtnPreferenceParameters_Clicked;
            BtnFiscalYears.Clicked += BtnFiscalYears_Clicked;
            BtnPrinters.Clicked += BtnPrinters_Clicked;

            BtnArticles.Clicked += BtnArticles_Clicked;
            BtnCustomers.Clicked += BtnCustomers_Clicked;
            BtnUsers.Clicked += BtnUsers_Clicked;
            BtnTables.Clicked += BtnTables_Clicked;

            BtnDocuments.Clicked += BtnDocuments_Clicked;
            BtnNewDocument.Clicked += BtnNewDocument_Clicked;
            BtnPayments.Clicked += BtnPayments_Clicked;
            BtnArticleStock.Clicked += BtnArticleStock_Clicked;

            BtnReportsMenu.Clicked += BtnReports_Clicked;
            BtnPrintReportRouter.Clicked += BtnPrintReportRouter_Clicked;

            BtnCustomerBalanceDetails.Clicked += BtnCustomerBalanceDetails_Clicked;
            BtnSalesPerDate.Clicked += BtnSalesPerDate_Clicked;

        }

    }
}
