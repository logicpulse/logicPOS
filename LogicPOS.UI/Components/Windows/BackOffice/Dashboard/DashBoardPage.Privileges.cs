using LogicPOS.UI.Components.Users;

using LogicPOS.UI.Components.System.Users.Permissions;
namespace LogicPOS.UI.Components.Pages
{
    internal partial class DashBoardPage
    {
        public void UpdatePrivileges()
        {
            BtnTerminals.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.Terminals.BACKOFFICE_MAN_CONFIGURATIONPLACETERMINAL_MENU);
            BtnPreferenceParameters.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.PreferenceParameters.BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETER_VIEW);
            BtnFiscalYears.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.Finance.FiscalYears.BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_CREATE);
            BtnPrinters.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.Printers.BACKOFFICE_MAN_CONFIGURATIONPRINTERS_VIEW);

            BtnArticles.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.Articles.BACKOFFICE_MAN_ARTICLE_VIEW);
            BtnCustomers.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.Customers.BACKOFFICE_MAN_CUSTOMER_VIEW);
            BtnUsers.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.System.Users.BACKOFFICE_MAN_USERDETAIL_VIEW);
            BtnTables.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.Tables.BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_VIEW);

            BtnDocuments.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.Finance.Documents.Types.BACKOFFICE_MAN_DOCUMENTFINANCETYPE_MENU);
            BtnNewDocument.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.Finance.Documents.Types.BACKOFFICE_MAN_DOCUMENTFINANCETYPE_CREATE);
            BtnPayments.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.Finance.FiscalYears.BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_VIEW);
            BtnArticleStock.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.Stocks.STOCK_MERCHANDISE_ENTRY_ACCESS);

            BtnReportsMenu.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.Reports.REPORT_ACCESS);
            BtnPrintReportRouter.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.Reports.REPORT_COMPANY_BILLING);
            BtnCustomerBalanceDetails.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.Reports.REPORT_CUSTOMER_BALANCE_DETAILS);
            BtnSalesPerDate.Sensitive = AuthenticationService.UserHasPermission(UserProfilePermissions.Reports.REPORT_SALES_DETAIL_PER_DATE);
        }
    }
}
