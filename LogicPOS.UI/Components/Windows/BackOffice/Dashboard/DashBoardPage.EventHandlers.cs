using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Services;
using MediatR;
using System;

namespace LogicPOS.UI.Components.Pages
{
    internal partial class DashBoardPage
    {
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

            ComboSalesYear.Changed += ComboSalesYear_Changed;

        }

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
            var modal = new ReportsFilterModal(BackOfficeWindow.Instance);
            modal.TxtArticle.Component.Sensitive = false;
            modal.TxtCustomer.Component.Sensitive = false;
            modal.TxtDocumentNumber.Component.Sensitive = false;
            modal.TxtDocumentType.Component.Sensitive = false;
            modal.TxtSerialNumber.Component.Sensitive = false;
            modal.TxtVatRate.Component.Sensitive = false;
            modal.TxtWarehouse.Component.Sensitive = false;

            var response = (ResponseType)modal.Run();

            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByDateReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
        }

        private void BtnCustomerBalanceDetails_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(BackOfficeWindow.Instance);
            modal.TxtArticle.Component.Sensitive = false;
            modal.TxtCustomer.Component.Sensitive = true;
            modal.TxtDocumentNumber.Component.Sensitive = false;
            modal.TxtDocumentType.Component.Sensitive = false;
            modal.TxtSerialNumber.Component.Sensitive = false;
            modal.TxtVatRate.Component.Sensitive = false;
            modal.TxtWarehouse.Component.Sensitive = false;
            modal.TxtCustomer.IsRequired = true;

            var response = (ResponseType)modal.Run();

            if (response == ResponseType.Ok)
            {
                if (modal.TxtCustomer.SelectedEntity == null)
                {
                    new CustomAlert(BackOfficeWindow.Instance)
                                    .WithMessageType(MessageType.Info)
                                    .WithTitle("Cliente não selecionado")
                                    .WithMessage("É necessário selecionar o Cliente para o Relátorio pretendido!")
                                    .ShowAlert();

                    modal.Run();
                    return;
                }
                ReportsService.ShowCustomerBalanceDetailsReport(modal.StartDate, modal.EndDate, (modal.TxtCustomer.SelectedEntity as Customer).Id);
                modal.Destroy();
            }

        }

        private void BtnPrintReportRouter_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(BackOfficeWindow.Instance);
            modal.TxtArticle.Component.Sensitive = false;
            modal.TxtDocumentNumber.Component.Sensitive = false;
            modal.TxtDocumentType.Component.Sensitive = false;
            modal.TxtSerialNumber.Component.Sensitive = false;
            modal.TxtVatRate.Component.Sensitive = false;
            modal.TxtWarehouse.Component.Sensitive = false;
            modal.TxtCustomer.Component.Sensitive= false;

            var response = (ResponseType)modal.Run();

            if (response == ResponseType.Ok)
            {
                
                ReportsService.ShowCompanyBillingReport(modal.StartDate, modal.EndDate);
                modal.Destroy();
            }
        }

        private void BtnReports_Clicked(object sender, EventArgs e)
        {
            ReportsModal.ShowModal(BackOfficeWindow.Instance);
        }

        private void BtnArticleStock_Clicked(object sender, EventArgs e)
        {
            StockManagementModal.ShowModal(BackOfficeWindow.Instance);
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

        private void ComboSalesYear_Changed(object sender, EventArgs e)
        {
            DrawGraph(int.Parse(ComboSalesYear.ActiveText));
        }

    }
}
