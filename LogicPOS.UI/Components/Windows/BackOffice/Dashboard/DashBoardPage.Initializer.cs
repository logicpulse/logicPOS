using LogicPOS.UI.Buttons;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System.Drawing;

namespace LogicPOS.UI.Components.Pages
{
    internal partial class DashBoardPage
    {
        private void InitializeButtons()
        {
            string font = "8";
            string terminalsIcon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_terminals.png";
            Size iconSize = new Size(35, 35);

            BtnTerminals = new IconButtonWithText(
                   new ButtonSettings
                   {
                       Name = "BACKOFFICE_MAN_CONFIGURATIONPLACETERMINAL_MENU",
                       BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                       Text = GeneralUtils.GetResourceByName("dialog_edit_ConfigurationPlaceTerminal_tab1_label"),
                       Font = font,
                       FontColor = _colorBaseDialogDefaultButtonFont,
                       Icon = terminalsIcon,
                       IconSize = iconSize,
                       ButtonSize = new Size(105, 70)
                   });

            BtnPreferenceParameters = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETER_VIEW",
                    BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_application_setup"),
                    Font = font,
                    FontColor = _colorBaseDialogDefaultButtonFont,
                    Icon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_configurations.png",
                    IconSize = iconSize,
                    ButtonSize = new Size(105, 70)
                });


            BtnFiscalYears = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_CREATE",
                    BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_documentfinance_years_short"),
                    Font = font,
                    FontColor = _colorBaseDialogDefaultButtonFont,
                    Icon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_fiscal_year.png",
                    IconSize = iconSize,
                    ButtonSize = new Size(105, 70)
                });

            BtnPrinters = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "BACKOFFICE_MAN_CONFIGURATIONPRINTERS_VIEW",
                    BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_printers"),
                    Font = font,
                    FontColor = _colorBaseDialogDefaultButtonFont,
                    Icon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_printer.png",
                    IconSize = iconSize,
                    ButtonSize = new Size(105, 70)
                });

            //Buttons Tabelas
            BtnArticles = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "BACKOFFICE_MAN_ARTICLE_VIEW",
                    BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_articles"),
                    Font = font,
                    FontColor = _colorBaseDialogDefaultButtonFont,
                    Icon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_articles.png",
                    IconSize = iconSize,
                    ButtonSize = new Size(105, 70)
                });

            BtnCustomers = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "BACKOFFICE_MAN_CUSTOMER_VIEW",
                    BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_customers"),
                    Font = font,
                    FontColor = _colorBaseDialogDefaultButtonFont,
                    Icon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_costumers.png",
                    IconSize = iconSize,
                    ButtonSize = new Size(105, 70)
                });

            BtnUsers = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "BACKOFFICE_MAN_USERDETAIL_VIEW",
                    BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_users"),
                    Font = font,
                    FontColor = _colorBaseDialogDefaultButtonFont,
                    Icon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_employees.png",
                    IconSize = iconSize,
                    ButtonSize = new Size(105, 70)
                });

            BtnTables = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_VIEW",
                    BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_other_tables"),
                    Font = font,
                    FontColor = _colorBaseDialogDefaultButtonFont,
                    Icon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_other_tables.png",
                    IconSize = iconSize,
                    ButtonSize = new Size(105, 70)
                });

            //Buttons Documentos
            BtnDocuments = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "BACKOFFICE_MAN_DOCUMENTSSHOW_MENU",
                    BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_worksession_resume_finance_documents"),
                    Font = font,
                    FontColor = _colorBaseDialogDefaultButtonFont,
                    Icon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_documents_new.png",
                    IconSize = iconSize,
                    ButtonSize = new Size(105, 70)
                });

            BtnNewDocument = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "BACKOFFICE_MAN_DOCUMENTSNEW_MENU",
                    BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("window_title_dialog_new_document"),
                    Font = font,
                    FontColor = _colorBaseDialogDefaultButtonFont,
                    Icon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_documents.png",
                    IconSize = iconSize,
                    ButtonSize = new Size(105, 70)
                });

            BtnPayments = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "BACKOFFICE_MAN_DOCUMENTSPAYMENTS_MENU",
                    BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("dialog_button_label_select_payments"),
                    Font = font,
                    FontColor = _colorBaseDialogDefaultButtonFont,
                    Icon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_documents_new.png",
                    IconSize = iconSize,
                    ButtonSize = new Size(105, 70)
                });

            BtnArticleStock = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "STOCK_MERCHANDISE_ENTRY_ACCESS",
                    BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_documentticket_type_title_cs_short"),
                    Font = font,
                    FontColor = _colorBaseDialogDefaultButtonFont,
                    Icon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_documents_merch.png",
                    IconSize = iconSize,
                    ButtonSize = new Size(105, 70)
                });

            //Buttons Relatórios
            BtnReportsMenu = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "REPORT_ACCESS",
                    BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_reports"),
                    Font = font,
                    FontColor = _colorBaseDialogDefaultButtonFont,
                    Icon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_reports.png",
                    IconSize = iconSize,
                    ButtonSize = new Size(105, 70)
                });

            BtnPrintReportRouter = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "REPORT_COMPANY_BILLING",
                    BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("report_company_billing_short"),
                    Font = font,
                    FontColor = _colorBaseDialogDefaultButtonFont,
                    Icon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_reports_sales_report.png",
                    IconSize = iconSize,
                    ButtonSize = new Size(105, 70)
                });

            BtnCustomerBalanceDetails = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "REPORT_CUSTOMER_BALANCE_DETAILS",
                    BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("report_customer_balance_details_short"),
                    Font = font,
                    FontColor = _colorBaseDialogDefaultButtonFont,
                    Icon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_reports_sales_client.png",
                    IconSize = iconSize,
                    ButtonSize = new Size(105, 70)
                });

            BtnSalesPerDate = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "REPORT_SALES_DETAIL_PER_DATE",
                    BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("report_sales_per_date"),
                    Font = font,
                    FontColor = _colorBaseDialogDefaultButtonFont,
                    Icon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_reports_sales_day.png",
                    IconSize = iconSize,
                    ButtonSize = new Size(105, 70)
                });

        }

        private void InitializeComboBoxSalesYears()
        {
            var years = GetAvailableSalesYears();
            ComboSalesYear = new Gtk.ComboBox(years);
            ComboSalesYear.Active = years.Length - 1;
        }
    }
}
