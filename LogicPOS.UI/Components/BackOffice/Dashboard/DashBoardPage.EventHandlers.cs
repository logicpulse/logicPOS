using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Windows;
using System;

namespace LogicPOS.UI.Components.Pages
{
    internal partial class DashBoardPage
    {

        #region Documents
        private void BtnNewDocument_Clicked(object sender, EventArgs e)
        {
            CreateDocumentModal.ShowModal(BackOfficeWindow.Instance);
        }

        private void BtnDocuments_Clicked(object sender, EventArgs e)
        {
            var modal = new DocumentsModal(BackOfficeWindow.Instance);
            modal.Run();
            modal.Destroy();
        }

        private void BtnPayments_Clicked(object sender, EventArgs e)
        {
            var modal = new ReceiptsModal(BackOfficeWindow.Instance);
            modal.Run();
            modal.Destroy();
        }

        private void BtnCurrentAccount_Clicked(object sender, EventArgs e)
        {
            CustomerCurrentAccountFilterModal.ShowModal(BackOfficeWindow.Instance);
        }

        #endregion

        private void BtnSalesPerDate_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnCustomerBalanceDetails_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnPrintReportRouter_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnReports_Clicked(object sender, EventArgs e)
        {
            ReportsModal.ShowModal(BackOfficeWindow.Instance);
        }

        private void BtnArticleStock_Clicked(object sender, EventArgs e)
        {
            logicpos.Utils.OpenArticleStockDialog(_parentWindow);
        }

        private void BtnTables_Clicked(object sender, EventArgs e)
        {
            BtnTables.Page = TablesPage.Instance;
            BackOfficeWindow.Instance.MenuBtn_Clicked(BtnTables, null);
        }

        private void BtnUsers_Clicked(object sender, EventArgs e)
        {
            BtnUsers.Page = UsersPage.Instance;
            BackOfficeWindow.Instance.MenuBtn_Clicked(BtnUsers, null);
        }

        private void BtnCustomers_Clicked(object sender, EventArgs e)
        {
            BtnCustomers.Page = CustomersPage.Instance;
            BackOfficeWindow.Instance.MenuBtn_Clicked(BtnCustomers, null);
        }

        private void BtnArticles_Clicked(object sender, EventArgs e)
        {
            BtnArticles.Page = ArticlesPage.Instance;
            BackOfficeWindow.Instance.MenuBtn_Clicked(BtnArticles, null);
        }

        private void BtnPrinters_Clicked(object sender, EventArgs e)
        {
            BtnPrinters.Page = PrintersPage.Instance;
            BackOfficeWindow.Instance.MenuBtn_Clicked(BtnPrinters, null);
        }

        private void BtnFiscalYears_Clicked(object sender, EventArgs e)
        {
            BtnFiscalYears.Page = FiscalYearsPage.Instance;
            BackOfficeWindow.Instance.MenuBtn_Clicked(BtnFiscalYears, null);
        }

        private void BtnPreferenceParameters_Clicked(object sender, EventArgs e)
        {
            BtnPreferenceParameters.Page = PreferenceParametersPage.CompanyPageInstance;
            BackOfficeWindow.Instance.MenuBtn_Clicked(BtnPreferenceParameters, null);
        }

        private void BtnTerminals_Clicked(object sender, EventArgs e)
        {
            BtnTerminals.Page = TerminalsPage.Instance;
            BackOfficeWindow.Instance.MenuBtn_Clicked(BtnTerminals, null);
        }
    }
}
