using Gtk;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Pages;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Windows
{
    public partial class BackOfficeWindow
    {
        #region Documents
        public XAccordionParentButton BtnDocumentsSection = new XAccordionParentButton(LocalizedString.Instance["dialog_button_label_select_record_finance_documents"],
                                                                                      "Assets/Images/Icons/Accordion/pos_backoffice_documentos.png");
        public VBox PanelDocuments = new VBox(false, 2);
        public XAccordionChildButton BtnNewDocument { get; } = new XAccordionChildButton(LocalizedString.Instance["dialog_button_label_new_finance_documents"]);
        public XAccordionChildButton BtnDocuments { get; } = new XAccordionChildButton(LocalizedString.Instance["dialog_button_label_select_record_finance_documents"]);
        public XAccordionChildButton BtnReceiptsEmission { get; } = new XAccordionChildButton(LocalizedString.Instance["dialog_button_label_select_finance_documents_ft_unpaid"]);
        public XAccordionChildButton BtnReceipts { get; } = new XAccordionChildButton(LocalizedString.Instance["dialog_button_label_select_payments"]);
        public XAccordionChildButton BtnCurrentAccount { get; } = new XAccordionChildButton(LocalizedString.Instance["dialog_button_label_select_finance_documents_cc"]);
        #endregion

        #region Reports
        public XAccordionParentButton BtnReportsSection = new XAccordionParentButton(LocalizedString.Instance["global_reports"],
                                                                                     "Assets/Images/Icons/Accordion/pos_backoffice_relatorios.png");

        public VBox PanelReports = new VBox(false, 2);
        public XAccordionChildButton BtnReports { get; } = new XAccordionChildButton(LocalizedString.Instance["global_reports"]);
        #endregion

        #region Articles
        public XAccordionParentButton BtnArticlesSection = new XAccordionParentButton(LocalizedString.Instance["global_articles"],
                                                                                      "Assets/Images/Icons/Accordion/pos_backoffice_artigos.png");
        public VBox PanelArticles = new VBox(false, 2);
        public XAccordionChildButton BtnArticleFamilies { get; } = new XAccordionChildButton(LocalizedString.Instance["global_families"]);
        public XAccordionChildButton BtnArticleSubfamilies { get; } = new XAccordionChildButton(LocalizedString.Instance["global_subfamilies"]);
        public XAccordionChildButton BtnArticles { get; } = new XAccordionChildButton(LocalizedString.Instance["global_articles"]);
        public XAccordionChildButton BtnArticleTypes { get; } = new XAccordionChildButton(LocalizedString.Instance["global_article_types"]);
        public XAccordionChildButton BtnArticleClasses { get; } = new XAccordionChildButton(LocalizedString.Instance["global_article_class"]);
        public XAccordionChildButton BtnPriceTypes { get; } = new XAccordionChildButton(LocalizedString.Instance["global_price_type"]);
        public XAccordionChildButton BtnSotck { get; } = new XAccordionChildButton(LocalizedString.Instance["global_stock_movements"]);
        #endregion

        #region Fiscal
        public XAccordionParentButton BtnFiscalSection = new XAccordionParentButton(LocalizedString.Instance["global_documents"],
                                                                                    "Assets/Images/Icons/Accordion/pos_backoffice_informacao_fiscal.png");
        public VBox PanelFiscal = new VBox(false, 2);
        public XAccordionChildButton BtnFiscalYears { get; } = new XAccordionChildButton(LocalizedString.Instance["global_documentfinance_years"]);
        public XAccordionChildButton BtnDocumentSeries { get; } = new XAccordionChildButton(LocalizedString.Instance["global_documentfinance_series"]);
        public XAccordionChildButton BtnDocumentTypes { get; } = new XAccordionChildButton(LocalizedString.Instance["global_documentfinance_type"]);
        public XAccordionChildButton BtnVatRates { get; } = new XAccordionChildButton(LocalizedString.Instance["global_vat_rates"]);
        public XAccordionChildButton BtnVatExemptionReasons { get; } = new XAccordionChildButton(LocalizedString.Instance["global_vat_exemption_reason"]);
        public XAccordionChildButton BtnPaymentConditions { get; } = new XAccordionChildButton(LocalizedString.Instance["global_payment_conditions"]);
        public XAccordionChildButton BtnPaymentMethods { get; } = new XAccordionChildButton(LocalizedString.Instance["global_payment_methods"]);
        #endregion

        #region Customers
        public XAccordionParentButton BtnCustomersSection = new XAccordionParentButton(LocalizedString.Instance["global_customers"],
                                                                                       "Assets/Images/Icons/Accordion/pos_backoffice_clientes.png");
        public VBox PanelCustomers = new VBox(false, 2);
        public XAccordionChildButton BtnCustomers { get; } = new XAccordionChildButton(LocalizedString.Instance["global_customers"]);
        public XAccordionChildButton BtnCustomerTypes { get; } = new XAccordionChildButton(LocalizedString.Instance["global_customer_types"]);
        public XAccordionChildButton BtnCustomerDiscountGroups { get; } = new XAccordionChildButton(LocalizedString.Instance["global_customer_discount_groups"]);
        #endregion

        #region Users
        public XAccordionParentButton UsersSection = new XAccordionParentButton(LocalizedString.Instance["global_users"],
                                                                               "Assets/Images/Icons/Accordion/pos_backoffice_utilizadores.png");
        public VBox PanelUsers = new VBox(false, 2);
        public XAccordionChildButton BtnUsers { get; } = new XAccordionChildButton(LocalizedString.Instance["global_users"]);
        public XAccordionChildButton BtnPermissions { get; } = new XAccordionChildButton(LocalizedString.Instance["global_user_permissions"]);
        public XAccordionChildButton BtnCommissionGroups { get; } = new XAccordionChildButton(LocalizedString.Instance["global_user_commission_groups"]);
        #endregion

        #region Devices
        public XAccordionParentButton BtnDevicesSection = new XAccordionParentButton(LocalizedString.Instance["global_devices"],
                                                                                     "Assets/Images/Icons/Accordion/pos_backoffice_impressoras.png");
        public VBox PanelDevices = new VBox(false, 2);
        public XAccordionChildButton BtnPrinterTypes { get; } = new XAccordionChildButton(LocalizedString.Instance["global_ConfigurationPrintersType"]);
        public XAccordionChildButton BtnPrinters { get; } = new XAccordionChildButton(LocalizedString.Instance["global_ConfigurationPrinters"]);
        public XAccordionChildButton BtnInputReaders { get; } = new XAccordionChildButton(LocalizedString.Instance["global_ConfigurationInputReader"]);
        public XAccordionChildButton BtnPoleDisplays { get; } = new XAccordionChildButton(LocalizedString.Instance["global_ConfigurationPoleDisplay"]);
        public XAccordionChildButton BtnWeighingMachine { get; } = new XAccordionChildButton(LocalizedString.Instance["global_ConfigurationWeighingMachine"]);
        #endregion

        #region Others
        public XAccordionParentButton BtnOthersSection = new XAccordionParentButton(LocalizedString.Instance["global_other_tables"],
                                                                                    "Assets/Images/Icons/Accordion/pos_backoffice_outras_tabelas.png");
        public VBox PanelOthers = new VBox(false, 2);   
        public XAccordionChildButton BtnCountries { get; } = new XAccordionChildButton(LocalizedString.Instance["global_country"]);
        public XAccordionChildButton BtnCurrencies { get; } = new XAccordionChildButton(LocalizedString.Instance["global_ConfigurationCurrency"]);
        public XAccordionChildButton BtnPlaces { get; } = new XAccordionChildButton(LocalizedString.Instance["global_places"]);
        public XAccordionChildButton BtnTables { get; } = new XAccordionChildButton(LocalizedString.Instance["global_place_tables"]);
        public XAccordionChildButton BtnMovementTypes { get; } = new XAccordionChildButton(LocalizedString.Instance["global_places_movement_type"]);
        public XAccordionChildButton BtnMeasurementUnits { get; } = new XAccordionChildButton(LocalizedString.Instance["global_units_measure"]);
        public XAccordionChildButton BtnSizeUnits { get; } = new XAccordionChildButton(LocalizedString.Instance["global_units_size"]);
        public XAccordionChildButton BtnHolidays { get; } = new XAccordionChildButton(LocalizedString.Instance["global_holidays"]);
        public XAccordionChildButton BtnWarehouses { get; } = new XAccordionChildButton(LocalizedString.Instance["global_warehouse"]);
        #endregion

        #region Configuration
        public XAccordionParentButton BtnConfigurationSection = new XAccordionParentButton(LocalizedString.Instance["global_configuration"],
                                                                                           "Assets/Images/Icons/Accordion/pos_backoffice_configuracao.png");
        public VBox PanelConfiguration = new VBox(false, 2);
        public XAccordionChildButton BtnCompanyPreferenceParameters { get; } = new XAccordionChildButton(LocalizedString.Instance["global_preferenceparameter_company"]);
        public XAccordionChildButton BtnSystemPreferenceParameters { get; } = new XAccordionChildButton(LocalizedString.Instance["global_preferenceparameter_system"]);
        public XAccordionChildButton BtnTerminals { get; } = new XAccordionChildButton(LocalizedString.Instance["global_places_terminals"]);
        #endregion

        #region Import
        public XAccordionParentButton BtnImportSection = new XAccordionParentButton(LocalizedString.Instance["global_import"],
                                                                                     "Assets/Images/Icons/Accordion/pos_backoffice_import.png");
        public VBox PanelImport = new VBox(false, 2);
        public XAccordionChildButton BtnImportArticles { get; } = new XAccordionChildButton(LocalizedString.Instance["global_import_articles"]);
        public XAccordionChildButton BtnImportCustomers { get; } = new XAccordionChildButton(LocalizedString.Instance["global_import_costumers"]);
        #endregion

        #region Export
        public XAccordionParentButton BtnExportSection = new XAccordionParentButton(LocalizedString.Instance["global_export"],
                                                                                    "Assets/Images/Icons/Accordion/pos_backoffice_export.png");
        public VBox PanelExport = new VBox(false, 2);
        public XAccordionChildButton BtnExportYearlySaft { get; } = new XAccordionChildButton(LocalizedString.Instance["global_export_saftpt_whole_year"]);
        public XAccordionChildButton BtnExportLastMonthSaft { get; } = new XAccordionChildButton(LocalizedString.Instance["global_export_saftpt_last_month"]);
        public XAccordionChildButton BtnExportCustomSaft { get; } = new XAccordionChildButton(LocalizedString.Instance["global_export_saftpt_custom"]);
        public XAccordionChildButton BtnExportArticles { get; } = new XAccordionChildButton(LocalizedString.Instance["global_export_articles"]);
        public XAccordionChildButton BtnExportCustomers { get; } = new XAccordionChildButton(LocalizedString.Instance["global_export_costumers"]);
        #endregion

        #region System
        public XAccordionParentButton BtnSystemSection = new XAccordionParentButton(LocalizedString.Instance["global_system"],
                                                                                    "Assets/Images/Icons/Accordion/pos_backoffice_sistema.png");
        public VBox PanelSystem = new VBox(false, 2);
        public XAccordionChildButton BtnNotifications { get; } = new XAccordionChildButton(LocalizedString.Instance["window_title_dialog_notification"]);
        public XAccordionChildButton BtnChangeLogs { get; } = new XAccordionChildButton(LocalizedString.Instance["change_log"]);
        public XAccordionChildButton BtnBackup { get; } = new XAccordionChildButton(LocalizedString.Instance["global_database_backup"]);
        public XAccordionChildButton BtnRestore { get; } = new XAccordionChildButton(LocalizedString.Instance["global_database_restore"]);
        public XAccordionChildButton BtnLogout { get; } = new XAccordionChildButton(LocalizedString.Instance["global_application_logout_user"]);

        #endregion

        public List<VBox> Panels = new List<VBox>();

        private void AddDocumentsSection()
        {
            PanelButtons.PackStart(BtnDocumentsSection.Button, false, false, 0);
            PanelButtons.PackStart(PanelDocuments, false, false, 0);
            {
                PanelDocuments.PackStart(BtnNewDocument.Button, false, false, 0);
                PanelDocuments.PackStart(BtnDocuments.Button, false, false, 0);
                PanelDocuments.PackStart(BtnReceiptsEmission.Button, false, false, 0);
                PanelDocuments.PackStart(BtnReceipts.Button, false, false, 0);
                PanelDocuments.PackStart(BtnCurrentAccount.Button, false, false, 0);
            }

            BtnDocumentsSection.Button.Clicked += delegate { ShowPanel(PanelDocuments); };
            BtnNewDocument.Button.Clicked += BtnNewDocument_Clicked;
            BtnDocuments.Button.Clicked += BtnDocuments_Clicked;
            BtnReceiptsEmission.Button.Clicked += BtnDocuments_Clicked;
            BtnReceipts.Button.Clicked += BtnReceipts_Clicked;
            BtnCurrentAccount.Button.Clicked += BtnCurrentAccount_Clicked;
        }

        private void AddReportsSection()
        {
            PanelButtons.PackStart(BtnReportsSection.Button, false, false, 0);
            PanelButtons.PackStart(PanelReports, false, false, 0);
            {
                PanelReports.PackStart(BtnReports.Button, false, false, 0);
            }

            BtnReportsSection.Button.Clicked += delegate { ShowPanel(PanelReports); };
            BtnReports.Button.Clicked += BtnReports_Clicked;
        }

        private void AddArticlesSection()
        {
            PanelButtons.PackStart(BtnArticlesSection.Button, false, false, 0);
            PanelButtons.PackStart(PanelArticles, false, false, 0);
            {
                PanelArticles.PackStart(BtnArticleFamilies.Button, false, false, 0);
                PanelArticles.PackStart(BtnArticleSubfamilies.Button, false, false, 0);
                PanelArticles.PackStart(BtnArticles.Button, false, false, 0);
                PanelArticles.PackStart(BtnArticleTypes.Button, false, false, 0);
                PanelArticles.PackStart(BtnArticleClasses.Button, false, false, 0);
                PanelArticles.PackStart(BtnPriceTypes.Button, false, false, 0);
                PanelArticles.PackStart(BtnSotck.Button, false, false, 0);
            }

            BtnArticlesSection.Button.Clicked += delegate { ShowPanel(PanelArticles); };
            BtnArticleFamilies.Button.Clicked += delegate { ShowPage(ArticleFamiliesPage.Instance, LocalizedString.Instance["global_families"]); };
            BtnArticleSubfamilies.Button.Clicked += delegate { ShowPage(ArticleSubfamiliesPage.Instance, LocalizedString.Instance["global_subfamilies"]); };
            BtnArticles.Button.Clicked += delegate { ShowPage(ArticlesPage.Instance, LocalizedString.Instance["global_articles"]); };
            BtnArticleTypes.Button.Clicked += delegate { ShowPage(ArticleTypesPage.Instance, LocalizedString.Instance["global_article_types"]); };
            BtnArticleClasses.Button.Clicked += delegate { ShowPage(ArticleClassesPage.Instance, LocalizedString.Instance["global_article_class"]); };
            BtnPriceTypes.Button.Clicked += delegate { ShowPage(PriceTypesPage.Instance, LocalizedString.Instance["global_price_type"]); };
            BtnSotck.Button.Clicked += BtnStock_Clicked;
        }

        private void AddFiscalSection()
        {
            PanelButtons.PackStart(BtnFiscalSection.Button, false, false, 0);
            PanelButtons.PackStart(PanelFiscal, false, false, 0);
            {
                PanelFiscal.PackStart(BtnFiscalYears.Button, false, false, 0);
                PanelFiscal.PackStart(BtnDocumentSeries.Button, false, false, 0);
                PanelFiscal.PackStart(BtnDocumentTypes.Button, false, false, 0);
                PanelFiscal.PackStart(BtnVatRates.Button, false, false, 0);
                PanelFiscal.PackStart(BtnVatExemptionReasons.Button, false, false, 0);
                PanelFiscal.PackStart(BtnPaymentConditions.Button, false, false, 0);
                PanelFiscal.PackStart(BtnPaymentMethods.Button, false, false, 0);
            }

            BtnFiscalSection.Button.Clicked += delegate { ShowPanel(PanelFiscal); };
            BtnFiscalYears.Button.Clicked += delegate { ShowPage(FiscalYearsPage.Instance, LocalizedString.Instance["global_documentfinance_years"]); };
            BtnDocumentSeries.Button.Clicked += delegate { ShowPage(DocumentSeriesPage.Instance, LocalizedString.Instance["global_documentfinance_series"]); };
            BtnDocumentTypes.Button.Clicked += delegate { ShowPage(DocumentTypesPage.Instance, LocalizedString.Instance["global_documentfinance_type"]); };
            BtnVatRates.Button.Clicked += delegate { ShowPage(VatRatesPage.Instance, LocalizedString.Instance["global_vat_rates"]); };
            BtnVatExemptionReasons.Button.Clicked += delegate { ShowPage(VatExemptionReasonsPage.Instance, LocalizedString.Instance["global_vat_exemption_reason"]); };
            BtnPaymentConditions.Button.Clicked += delegate { ShowPage(PaymentConditionsPage.Instance, LocalizedString.Instance["global_payment_conditions"]); };
            BtnPaymentMethods.Button.Clicked += delegate { ShowPage(PaymentMethodsPage.Instance, LocalizedString.Instance["global_payment_methods"]); };
        }

        private void AddCustomersSection()
        {
            PanelButtons.PackStart(BtnCustomersSection.Button, false, false, 0);
            PanelButtons.PackStart(PanelCustomers, false, false, 0);
            {
                PanelCustomers.PackStart(BtnCustomers.Button, false, false, 0);
                PanelCustomers.PackStart(BtnCustomerTypes.Button, false, false, 0);
                PanelCustomers.PackStart(BtnCustomerDiscountGroups.Button, false, false, 0);
            }

            BtnCustomersSection.Button.Clicked += delegate { ShowPanel(PanelCustomers); };
            BtnCustomers.Button.Clicked += delegate { ShowPage(CustomersPage.Instance, LocalizedString.Instance["global_customers"]); };
            BtnCustomerTypes.Button.Clicked += delegate { ShowPage(CustomerTypePage.Instance, LocalizedString.Instance["global_customer_types"]); };
            BtnCustomerDiscountGroups.Button.Clicked += delegate { ShowPage(DiscountGroupsPage.Instance, LocalizedString.Instance["global_customer_discount_groups"]); };
        }

        private void AddUsersSection()
        {
            PanelButtons.PackStart(UsersSection.Button, false, false, 0);
            PanelButtons.PackStart(PanelUsers, false, false, 0);
            {
                PanelUsers.PackStart(BtnUsers.Button, false, false, 0);
                PanelUsers.PackStart(BtnPermissions.Button, false, false, 0);
                PanelUsers.PackStart(BtnCommissionGroups.Button, false, false, 0);
            }

            UsersSection.Button.Clicked += delegate { ShowPanel(PanelUsers); };
            BtnUsers.Button.Clicked += delegate { ShowPage(UsersPage.Instance, LocalizedString.Instance["global_users"]); };
            BtnPermissions.Button.Clicked += delegate { ShowPage(PermissionsPage.Instance, LocalizedString.Instance["global_user_permissions"]); };
            BtnCommissionGroups.Button.Clicked += delegate { ShowPage(CommissionGroupsPage.Instance, LocalizedString.Instance["global_user_commission_groups"]); };
        }

        private void AddDevicesSection()
        {
            PanelButtons.PackStart(BtnDevicesSection.Button, false, false, 0);
            PanelButtons.PackStart(PanelDevices, false, false, 0);
            {
                PanelDevices.PackStart(BtnPrinterTypes.Button, false, false, 0);
                PanelDevices.PackStart(BtnPrinters.Button, false, false, 0);
                PanelDevices.PackStart(BtnInputReaders.Button, false, false, 0);
                PanelDevices.PackStart(BtnPoleDisplays.Button, false, false, 0);
                PanelDevices.PackStart(BtnWeighingMachine.Button, false, false, 0);
            }

            BtnDevicesSection.Button.Clicked += delegate { ShowPanel(PanelDevices); };
            BtnPrinterTypes.Button.Clicked += delegate { ShowPage(PrinterTypesPage.Instance, LocalizedString.Instance["global_ConfigurationPrintersType"]); };
            BtnPrinters.Button.Clicked += delegate { ShowPage(PrintersPage.Instance, LocalizedString.Instance["global_ConfigurationPrinters"]); };
            BtnInputReaders.Button.Clicked += delegate { ShowPage(InputReadersPage.Instance, LocalizedString.Instance["global_ConfigurationInputReader"]); };
            BtnPoleDisplays.Button.Clicked += delegate { ShowPage(PoleDisplaysPage.Instance, LocalizedString.Instance["global_ConfigurationPoleDisplay"]); };
            BtnWeighingMachine.Button.Clicked += delegate { ShowPage(WeighingMachinesPage.Instance, LocalizedString.Instance["global_ConfigurationWeighingMachine"]); };
        }

        private void AddOthersSection()
        {
            PanelButtons.PackStart(BtnOthersSection.Button, false, false, 0);
            PanelButtons.PackStart(PanelOthers, false, false, 0);
            {
                PanelOthers.PackStart(BtnCountries.Button, false, false, 0);
                PanelOthers.PackStart(BtnCurrencies.Button, false, false, 0);
                PanelOthers.PackStart(BtnPlaces.Button, false, false, 0);
                PanelOthers.PackStart(BtnTables.Button, false, false, 0);
                PanelOthers.PackStart(BtnMovementTypes.Button, false, false, 0);
                PanelOthers.PackStart(BtnMeasurementUnits.Button, false, false, 0);
                PanelOthers.PackStart(BtnSizeUnits.Button, false, false, 0);
                PanelOthers.PackStart(BtnHolidays.Button, false, false, 0);
                PanelOthers.PackStart(BtnWarehouses.Button, false, false, 0);
            }

            BtnOthersSection.Button.Clicked += delegate { ShowPanel(PanelOthers); };
            BtnCountries.Button.Clicked += delegate { ShowPage(CountriesPage.Instance, LocalizedString.Instance["global_country"]); };
            BtnCurrencies.Button.Clicked += delegate { ShowPage(CurrenciesPage.Instance, LocalizedString.Instance["global_ConfigurationCurrency"]); };
            BtnPlaces.Button.Clicked += delegate { ShowPage(PlacesPage.Instance, LocalizedString.Instance["global_places"]); };
            BtnTables.Button.Clicked += delegate { ShowPage(TablesPage.Instance, LocalizedString.Instance["global_place_tables"]); };
            BtnMovementTypes.Button.Clicked += delegate { ShowPage(MovementTypesPage.Instance, LocalizedString.Instance["global_places_movement_type"]); };
            BtnMeasurementUnits.Button.Clicked += delegate { ShowPage(MeasurementUnitsPage.Instance, LocalizedString.Instance["global_units_measure"]); };
            BtnSizeUnits.Button.Clicked += delegate { ShowPage(SizeUnitsPage.Instance, LocalizedString.Instance["global_units_size"]); };
            BtnHolidays.Button.Clicked += delegate { ShowPage(HolidaysPage.Instance, LocalizedString.Instance["global_holidays"]); };
            BtnWarehouses.Button.Clicked += delegate { ShowPage(WarehousesPage.Instance, LocalizedString.Instance["global_warehouse"]); };
        }

        private void AddConfigurationSection()
        {
            PanelButtons.PackStart(BtnConfigurationSection.Button, false, false, 0);
            PanelButtons.PackStart(PanelConfiguration, false, false, 0);
            {
                PanelConfiguration.PackStart(BtnCompanyPreferenceParameters.Button, false, false, 0);
                PanelConfiguration.PackStart(BtnSystemPreferenceParameters.Button, false, false, 0);
                PanelConfiguration.PackStart(BtnTerminals.Button, false, false, 0);
            }

            BtnConfigurationSection.Button.Clicked += delegate { ShowPanel(PanelConfiguration); };
            BtnCompanyPreferenceParameters.Button.Clicked += delegate { ShowPage(PreferenceParametersPage.CompanyPageInstance, LocalizedString.Instance["global_preferenceparameter_company"]); };
            BtnSystemPreferenceParameters.Button.Clicked += delegate { ShowPage(PreferenceParametersPage.SystemPageInstance, LocalizedString.Instance["global_preferenceparameter_system"]); };
            BtnTerminals.Button.Clicked += delegate { ShowPage(TerminalsPage.Instance, LocalizedString.Instance["global_places_terminals"]); };
        }

        private void AddImportSection()
        {
            PanelButtons.PackStart(BtnImportSection.Button, false, false, 0);
            PanelButtons.PackStart(PanelImport, false, false, 0);
            {
                PanelImport.PackStart(BtnImportArticles.Button, false, false, 0);
                PanelImport.PackStart(BtnImportCustomers.Button, false, false, 0);
            }

            BtnImportSection.Button.Clicked += delegate { ShowPanel(PanelImport); };
        }

        private void AddExportSection()
        {
            PanelButtons.PackStart(BtnExportSection.Button, false, false, 0);
            PanelButtons.PackStart(PanelExport, false, false, 0);
            {
                PanelExport.PackStart(BtnExportYearlySaft.Button, false, false, 0);
                PanelExport.PackStart(BtnExportLastMonthSaft.Button, false, false, 0);
                PanelExport.PackStart(BtnExportCustomSaft.Button, false, false, 0);
                PanelExport.PackStart(BtnExportArticles.Button, false, false, 0);
                PanelExport.PackStart(BtnExportCustomers.Button, false, false, 0);
            }

            BtnExportSection.Button.Clicked += delegate { ShowPanel(PanelExport); };
            BtnExportYearlySaft.Button.Clicked += BtnExportYearlySaft_Clicked;
            BtnExportLastMonthSaft.Button.Clicked += BtnExportLastMonthSaft_Clicked;
            BtnExportCustomSaft.Button.Clicked += BtnExportCustomSaft_Clicked;
        }

        private void AddSystemSection()
        {
            PanelButtons.PackStart(BtnSystemSection.Button, false, false, 0);
            PanelButtons.PackStart(PanelSystem, false, false, 0);
            {
                PanelSystem.PackStart(BtnNotifications.Button, false, false, 0);
                PanelSystem.PackStart(BtnChangeLogs.Button, false, false, 0);
                PanelSystem.PackStart(BtnBackup.Button, false, false, 0);
                PanelSystem.PackStart(BtnRestore.Button, false, false, 0);
                PanelSystem.PackStart(BtnLogout.Button, false, false, 0);
            }

            BtnSystemSection.Button.Clicked += delegate { ShowPanel(PanelSystem); };
            BtnChangeLogs.Button.Clicked += BtnChangeLog_Clicked;
            BtnBackup.Button.Clicked += BtnBackupDb_Clicked;
            BtnRestore.Button.Clicked += BtnRestoreDb_Clicked;
            BtnLogout.Button.Clicked += BtnLogout_Clicked;
        }

        private void AddSections()
        {
            AddDocumentsSection();
            AddReportsSection();
            AddArticlesSection();
            AddFiscalSection();
            AddCustomersSection();
            AddUsersSection();
            AddDevicesSection();
            AddOthersSection();
            AddConfigurationSection();
            AddImportSection();
            AddExportSection();
            AddSystemSection();
        }

        private void ShowPanel(VBox panel)
        {
            foreach (var p in Panels)
            {
                p.Visible = p == panel;
            }
        }

        private void RegisterPanels()
        {
            Panels.Add(PanelDocuments);
            Panels.Add(PanelReports);
            Panels.Add(PanelArticles);
            Panels.Add(PanelFiscal);
            Panels.Add(PanelCustomers);
            Panels.Add(PanelUsers);
            Panels.Add(PanelDevices);
            Panels.Add(PanelOthers);
            Panels.Add(PanelConfiguration);
            Panels.Add(PanelImport);
            Panels.Add(PanelExport);
            Panels.Add(PanelSystem);
        }
    }
}
