using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Users;
using LogicPOS.Utility;
using Medsphere.Widgets;
using System.Drawing;

namespace LogicPOS.UI.Components.Pages
{
    internal partial class DashBoardPage
    {
        private Label _label;
        private IconButtonWithText BtnTerminals { get; set; }
        private IconButtonWithText BtnPreferenceParameters { get; set; }
        private IconButtonWithText BtnFiscalYears { get; set; }
        private IconButtonWithText BtnPrinters { get; set; }
        private IconButtonWithText BtnArticles { get; set; }
        private IconButtonWithText BtnCustomers { get; set; }
        private IconButtonWithText BtnUsers { get; set; }
        private IconButtonWithText BtnTables { get; set; }
        private IconButtonWithText BtnDocuments { get; set; }
        private IconButtonWithText BtnNewDocument { get; set; }
        private IconButtonWithText BtnPayments { get; set; }
        private IconButtonWithText BtnArticleStock { get; set; }
        private IconButtonWithText BtnReportsMenu { get; set; }
        private IconButtonWithText BtnPrintReportRouter { get; set; }
        private IconButtonWithText BtnCustomerBalanceDetails { get; set; }
        private IconButtonWithText BtnSalesPerDate { get; set; }

        public ComboBox selAno;
        private readonly Graph newGraph = new Graph2D();




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
