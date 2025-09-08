using LogicPOS.UI.Components.Users;

namespace LogicPOS.UI.Components.Pages
{
    internal partial class DashBoardPage
    {
        public void UpdatePrivileges()
        {
            BtnTerminals.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPLACETERMINAL_MENU");
            BtnPreferenceParameters.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETER_VIEW");
            BtnFiscalYears.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_CREATE");
            BtnPrinters.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPRINTERS_VIEW");

            BtnArticles.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLE_VIEW");
            BtnCustomers.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CUSTOMER_VIEW");
            BtnUsers.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_USERDETAIL_VIEW");
            BtnTables.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_VIEW");

            BtnDocuments.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_MENU");
            BtnNewDocument.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_CREATE");
            BtnPayments.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_VIEW");
            BtnArticleStock.Sensitive = AuthenticationService.UserHasPermission("STOCK_MERCHANDISE_ENTRY_ACCESS");

            BtnReportsMenu.Sensitive = AuthenticationService.UserHasPermission("REPORT_ACCESS");
            BtnPrintReportRouter.Sensitive = AuthenticationService.UserHasPermission("REPORT_COMPANY_BILLING");
            BtnCustomerBalanceDetails.Sensitive = AuthenticationService.UserHasPermission("REPORT_CUSTOMER_BALANCE_DETAILS");
            BtnSalesPerDate.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_PER_DATE");
        }
    }
}
