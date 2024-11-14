using Gtk;
using logicpos;
using logicpos.Classes.DataLayer;
using logicpos.Classes.Enums;
using logicpos.Classes.Enums.Finance;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Settings;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Accordions;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.PoleDisplays;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;


namespace LogicPOS.UI.Components.BackOffice.Windows
{
    public partial class BackOfficeWindow : BackOfficeBaseWindow
    {
        public static BackOfficeWindow Instance { get; set; }

        private readonly string _privilegesBackOfficeMenuOperation = $"{LogicPOSSettings.PrivilegesBackOfficeCRUDOperationPrefix}_{"MENU"}";

        public BackOfficeWindow()
        {
            Initialize();
            ShowAll();
        }

        private void BackOfficeMainWindow_Show(object sender, EventArgs e)
        {
            LabelTerminalInfo.Text = $"{TerminalService.Terminal.Designation} : {AuthenticationService.User.Name}";
            Menu.UpdateMenuPrivileges();
            string currentNodePrivilegesToken = string.Format(_privilegesBackOfficeMenuOperation, Menu.CurrentPageChildButton.Name.ToUpper());
            CurrentPage.Sensitive = AuthenticationService.UserHasPermission(currentNodePrivilegesToken);
        }


        private void Initialize()
        {
            InitializeMenuSections();

            ResizeMenuButtons();

            PanelButtons.Add(Menu);

            AddEventHandlers();

            ShowStartPage();
        }

        private void ShowStartPage()
        {
            CurrentPage = new DashBoardPage(this);
            PanelContent.PackEnd(CurrentPage);
        }

        private void InitializeMenuSections()
        {
            var sections = CreateMenuSections();
            Menu = new Accordion(sections, LogicPOSSettings.PrivilegesBackOfficeMenuOperationFormat);
            Menu.WidthRequest = ButtonSize.Width;
        }

        private void AddEventHandlers()
        {
            Menu.Clicked += MenuButton_Clicked;
            BtnExit.Clicked += BtnExit_Clicked;
            BtnNewVersion.Clicked += BtnNewVesion_Clicked;
            BtnDashboard.Clicked += BtnDashBoard_Clicked;
            BtnPOS.Clicked += BtnPOS_Clicked;
            Shown += BackOfficeMainWindow_Show;
        }

        private void ResizeMenuButtons()
        {
            if (LogicPOSAppContext.BackOfficeScreenSize.Height <= 800)
            {
                PanelButtons.Put(Menu, 0, 28);
            }
            else
            {
                PanelButtons.Put(Menu, 0, 40);
            }
        }


        private Dictionary<string, AccordionNode> CreateMenuSections()
        {
            Dictionary<string, AccordionNode> sections = new Dictionary<string, AccordionNode>();

            if (GeneralSettings.AppUseBackOfficeMode)
            {
                bool openDay = WorkSessionProcessor.SessionPeriodOpen(WorkSessionPeriodType.Day, "");
                if (openDay)
                {
                    pos_worksessionperiod workSessionPeriodDay = WorkSessionProcessor.GetSessionPeriod(WorkSessionPeriodType.Day);
                    XPOSettings.WorkSessionPeriodTerminal = WorkSessionProcessor.GetSessionPeriod(WorkSessionPeriodType.Day);
                    XPOSettings.WorkSessionPeriodTerminal.SessionStatus = WorkSessionPeriodStatus.Open;
                }
            }

            Dictionary<string, AccordionNode> usersSection = CreateUsersSection();

            Dictionary<string, AccordionNode> fiscalSection = CreateFiscalSection();

            Dictionary<string, AccordionNode> othersSection = CreateOthersSection();

            Dictionary<string, AccordionNode> devicesSection = CreateDevicesSection();

            Dictionary<string, AccordionNode> configurationSection = CreateConfigurationSection();

            Dictionary<string, AccordionNode> importSection = CreateImportSection();

            Dictionary<string, AccordionNode> exportSection = CreateExportSection();

            Dictionary<string, AccordionNode> documentsSection = CreateDocumentsSection();

            Dictionary<string, AccordionNode> reportsSection = CreateReporButtons();

            Dictionary<string, AccordionNode> articleButtons = CreateArticlesSection();

            Dictionary<string, AccordionNode> customersSection = CreateCustomersSection();

            sections.Add("TopMenuFinanceDocuments", new AccordionNode(GeneralUtils.GetResourceByName("dialog_button_label_select_record_finance_documents")) { Children = documentsSection, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_documentos.png") });

            sections.Add("TopMenuReports", new AccordionNode(GeneralUtils.GetResourceByName("global_reports")) { Children = reportsSection, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_relatorios.png") });

            sections.Add("TopMenuArticles", new AccordionNode(GeneralUtils.GetResourceByName("global_articles")) { Children = articleButtons, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_artigos.png") });

            sections.Add("TopMenuDocuments", new AccordionNode(GeneralUtils.GetResourceByName("global_documents")) { Children = fiscalSection, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_informacao_fiscal.png") });

            sections.Add("TopMenuCustomers", new AccordionNode(GeneralUtils.GetResourceByName("global_customers")) { Children = customersSection, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_clientes.png") });

            sections.Add("TopMenuUsers", new AccordionNode(GeneralUtils.GetResourceByName("global_users")) { Children = usersSection, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_utilizadores.png") });

            sections.Add("TopMenuDevices", new AccordionNode(GeneralUtils.GetResourceByName("global_devices")) { Children = devicesSection, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_impressoras.png") });

            sections.Add("TopMenuOtherTables", new AccordionNode(GeneralUtils.GetResourceByName("global_other_tables")) { Children = othersSection, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_outras_tabelas.png") });

            sections.Add("TopMenuConfiguration", new AccordionNode(GeneralUtils.GetResourceByName("global_configuration")) { Children = configurationSection, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_configuracao.png") });

            sections.Add("TopMenuImport", new AccordionNode(GeneralUtils.GetResourceByName("global_import")) { Children = importSection, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_import.png") });

            sections.Add("TopMenuExport", new AccordionNode(GeneralUtils.GetResourceByName("global_export")) { Children = exportSection, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_export.png") });

            sections.Add("System", new AccordionNode(GeneralUtils.GetResourceByName("global_system")) { Children = CreateSystemSection(), GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_sistema.png") });

            return sections;
        }

        private Dictionary<string, AccordionNode> CreateExportSection()
        {
            Dictionary<string, AccordionNode> exportSection = new Dictionary<string, AccordionNode>();
            exportSection.Add("System_ExportSaftPT_SaftPt", new AccordionNode(GeneralUtils.GetResourceByName("global_export_saftpt_whole_year")) { Clicked = BtnExportYearlySaft_Clicked  });
            exportSection.Add("System_ExportSaftPT_E-Fatura", new AccordionNode(GeneralUtils.GetResourceByName("global_export_saftpt_last_month")) { Clicked = BtnExportLastMonthSaft_Clicked });
            exportSection.Add("System_ExportSaftPT_Custom", new AccordionNode(GeneralUtils.GetResourceByName("global_export_saftpt_custom")) { Clicked = BtnExportCustomSaft_Clicked });
            exportSection.Add("System_Export_Articles", new AccordionNode(GeneralUtils.GetResourceByName("global_export_articles")) { Clicked = delegate { ExcelProcessing.OpenFilePicker(this, ImportExportFileOpen.ExportArticles); } });
            exportSection.Add("System_Export_Costumers", new AccordionNode(GeneralUtils.GetResourceByName("global_export_costumers")) { Clicked = delegate { ExcelProcessing.OpenFilePicker(this, ImportExportFileOpen.ExportCustomers); } });
            return exportSection;
        }

        private Dictionary<string, AccordionNode> CreateImportSection()
        {
            return new Dictionary<string, AccordionNode>
                {
                    { "System_Import_Articles", new AccordionNode(GeneralUtils.GetResourceByName("global_import_articles")) { Clicked = delegate { ExcelProcessing.OpenFilePicker(this, ImportExportFileOpen.OpenExcelArticles); } } },
                    { "System_Import_Costumers", new AccordionNode(GeneralUtils.GetResourceByName("global_import_costumers")) { Clicked = delegate { ExcelProcessing.OpenFilePicker(this, ImportExportFileOpen.OpenExcelCostumers); } } }
                };
        }

        private Dictionary<string, AccordionNode> CreateConfigurationSection()
        {
            return new Dictionary<string, AccordionNode>
                {
                    { "ConfigurationPreferenceParameterCompany", new AccordionNode(GeneralUtils.GetResourceByName("global_preferenceparameter_company")) { Content = new PreferenceParametersPage(this, new Dictionary<string, string>{{"parameters","company"}}) } },
                    { "ConfigurationPreferenceParameterSystem", new AccordionNode(GeneralUtils.GetResourceByName("global_preferenceparameter_system")) {Content = new PreferenceParametersPage(this, new Dictionary<string, string>{{"parameters","system"}}) }  },
                    { "ConfigurationPlaceTerminal", new AccordionNode(GeneralUtils.GetResourceByName("global_places_terminals")) { Content = new TerminalsPage(this) } }
                };
        }

        private Dictionary<string, AccordionNode> CreateDevicesSection()
        {
            return new Dictionary<string, AccordionNode>
                {
                    { "ConfigurationPrintersType", new AccordionNode(GeneralUtils.GetResourceByName("global_ConfigurationPrintersType")) { Content = new PrinterTypesPage(this) } },
                    { "ConfigurationPrinters", new AccordionNode(GeneralUtils.GetResourceByName("global_ConfigurationPrinters")) { Content = new PrintersPage(this) } },
                    { "ConfigurationInputReader", new AccordionNode(GeneralUtils.GetResourceByName("global_ConfigurationInputReader")) { Content = new InputReadersPage(this) } },
                    { "ConfigurationPoleDisplay", new AccordionNode(GeneralUtils.GetResourceByName("global_ConfigurationPoleDisplay")) { Content = new PoleDisplaysPage(this) } },
                    { "ConfigurationWeighingMachine", new AccordionNode(GeneralUtils.GetResourceByName("global_ConfigurationWeighingMachine")) { Content = new WeighingMachinesPage(this) } }
                };
        }

        private Dictionary<string, AccordionNode> CreateOthersSection()
        {
            Dictionary<string, AccordionNode> othersSection = new Dictionary<string, AccordionNode>
                {
                    { "ConfigurationCountry", new AccordionNode(GeneralUtils.GetResourceByName("global_country")) { Content = new CountriesPage(this) } },
                    { "ConfigurationCurrency", new AccordionNode(GeneralUtils.GetResourceByName("global_ConfigurationCurrency")) { Content = new CurrenciesPage(this) } },
                    { "ConfigurationPlace", new AccordionNode(GeneralUtils.GetResourceByName("global_places")) { Content = new PlacesPage(this) } }
                };

            string configurationPlaceTableLabel = AppOperationModeSettings.IsDefaultTheme ? GeneralUtils.GetResourceByName("global_place_tables") : GeneralUtils.GetResourceByName("window_title_dialog_orders");
            othersSection.Add("ConfigurationPlaceTable", new AccordionNode(configurationPlaceTableLabel) { Content = new TablesPage(this) });
            othersSection.Add("ConfigurationPlaceMovementType", new AccordionNode(GeneralUtils.GetResourceByName("global_places_movement_type")) { Content = new MovementTypesPage(this) });
            othersSection.Add("ConfigurationUnitMeasure", new AccordionNode(GeneralUtils.GetResourceByName("global_units_measure")) { Content = new MeasurementUnitsPage(this) });
            othersSection.Add("ConfigurationUnitSize", new AccordionNode(GeneralUtils.GetResourceByName("global_units_size")) { Content = new SizeUnitsPage(this) });
            othersSection.Add("ConfigurationHolidays", new AccordionNode(GeneralUtils.GetResourceByName("global_holidays")) { Content = new HolidaysPage(this) });
            othersSection.Add("Warehouse", new AccordionNode(GeneralUtils.GetResourceByName("global_warehouse")) { Content = new WarehousesPage(this) });
            return othersSection;
        }

        private Dictionary<string, AccordionNode> CreateFiscalSection()
        {
            return new Dictionary<string, AccordionNode>
                {
                    { "DocumentFinanceYears", new AccordionNode(GeneralUtils.GetResourceByName("global_documentfinance_years")) { Content = FiscalYearsPage.Instance } },
                    { "DocumentFinanceSeries", new AccordionNode(GeneralUtils.GetResourceByName("global_documentfinance_series")) { Content = new DocumentSeriesPage(this) } },
                    { "DocumentFinanceType", new AccordionNode(GeneralUtils.GetResourceByName("global_documentfinance_type")) { Content = new DocumentTypesPage(this) } },
                    { "ConfigurationVatRate", new AccordionNode(GeneralUtils.GetResourceByName("global_vat_rates")) { Content = new VatRatesPage(this) } },
                    { "ConfigurationVatExemptionReason", new AccordionNode(GeneralUtils.GetResourceByName("global_vat_exemption_reason")) { Content = new VatExemptionReasonsPage(this) } },
                    { "ConfigurationPaymentCondition", new AccordionNode(GeneralUtils.GetResourceByName("global_payment_conditions")) { Content = new PaymentConditionsPage(this) } },
                    { "ConfigurationPaymentMethod", new AccordionNode(GeneralUtils.GetResourceByName("global_payment_methods")) { Content = new PaymentMethodsPage(this) } }
                };
        }

        private Dictionary<string, AccordionNode> CreateUsersSection()
        {
            return new Dictionary<string, AccordionNode>
                {
                    { "UserDetail", new AccordionNode(GeneralUtils.GetResourceByName("global_users")) { Content = new UsersPage(this) } },
                    { "UserPermissionProfile", new AccordionNode(GeneralUtils.GetResourceByName("global_user_permissions")) { Content = new PermissionsPage(this) } },
                    { "UserCommissionGroup", new AccordionNode(GeneralUtils.GetResourceByName("global_user_commission_groups")) { Content = new CommissionGroupsPage(this) } }
                };
        }

        private Dictionary<string, AccordionNode> CreateSystemSection()
        {
            Dictionary<string, AccordionNode> buttons = new Dictionary<string, AccordionNode>
            {
                { "System_Notification", new AccordionNode(GeneralUtils.GetResourceByName("window_title_dialog_notification")) { Clicked = BtnNotificaion_Clicked } },
                { "System_ChangeLog", new AccordionNode(GeneralUtils.GetResourceByName("change_log")) { Clicked = BtnChangeLog_Clicked } },
                { "System_DataBaseBackup", new AccordionNode(GeneralUtils.GetResourceByName("global_database_backup")) { Clicked = BtnBackupDb_Clicked } },
                { "System_DataBaseRestore_FromSystem", new AccordionNode(GeneralUtils.GetResourceByName("global_database_restore")) { Clicked = BtnRestoreDbFromFile_Clicked } },
                { "System_DataBaseRestore_FromFile", new AccordionNode(GeneralUtils.GetResourceByName("global_database_restore_from_file")) { Clicked = BtnRestoreDbFromFile_Clicked } },
                { "System_Menu", new AccordionNode(GeneralUtils.GetResourceByName("global_application_logout_user")) { Clicked = BtnLogout_Clicked } }
            };

            return buttons;
        }

        private Dictionary<string, AccordionNode> CreateCustomersSection()
        {
            return new Dictionary<string, AccordionNode>
                {
                    { "Customer", new AccordionNode(GeneralUtils.GetResourceByName("global_customers")) { Content = new CustomersPage(this) } },
                    { "CustomerType", new AccordionNode(GeneralUtils.GetResourceByName("global_customer_types")) { Content = new CustomerTypePage(this) } },
                    { "CustomerDiscountGroup", new AccordionNode(GeneralUtils.GetResourceByName("global_customer_discount_groups")) { Content = new DiscountGroupsPage(this) } }
                };
        }

        private Dictionary<string, AccordionNode> CreateReporButtons()
        {
            return new Dictionary<string, AccordionNode>
                {
                    { "DocumentsReports", new AccordionNode(GeneralUtils.GetResourceByName("global_reports")) { Clicked = delegate { Utils.StartReportsMenuFromBackOffice(this); } } }
                };
        }

        private Dictionary<string, AccordionNode> CreateDocumentsSection()
        {
            return new Dictionary<string, AccordionNode>
                {
                    { "DocumentsNew", new AccordionNode(GeneralUtils.GetResourceByName("dialog_button_label_new_finance_documents")) { Clicked = delegate { Utils.StartNewDocumentFromBackOffice(this); } } },
                    { "DocumentsShow", new AccordionNode(GeneralUtils.GetResourceByName("dialog_button_label_select_record_finance_documents")) { Clicked = delegate {  } } },
                    { "DocumentsPay", new AccordionNode(GeneralUtils.GetResourceByName("dialog_button_label_select_finance_documents_ft_unpaid")) { Clicked = delegate {  } } },
                    { "DocumentsPayments", new AccordionNode(GeneralUtils.GetResourceByName("dialog_button_label_select_payments")) { Clicked = delegate { } } },
                    { "DocumentsCurrentAccount", new AccordionNode(GeneralUtils.GetResourceByName("dialog_button_label_select_finance_documents_cc")) { Clicked = delegate { } } }
                };
        }

        private Dictionary<string, AccordionNode> CreateArticlesSection()
        {
            Dictionary<string, AccordionNode> articleButtons = new Dictionary<string, AccordionNode>
            {
                { "ArticleFamily", new AccordionNode(GeneralUtils.GetResourceByName("global_families")) { Content = new ArticleFamiliesPage(this) } },
                { "ArticleSubFamily", new AccordionNode(GeneralUtils.GetResourceByName("global_subfamilies")) { Content = new ArticleSubfamiliesPage(this) } },
                { "Article", new AccordionNode(GeneralUtils.GetResourceByName("global_articles")) { Content = new ArticlesPage(this) } },
                { "ArticleType", new AccordionNode(GeneralUtils.GetResourceByName("global_article_types")) { Content = new ArticleTypesPage(this) } },
                { "ArticleClass", new AccordionNode(GeneralUtils.GetResourceByName("global_article_class")) { Content = new ArticleClassesPage(this) } },
                { "ConfigurationPriceType", new AccordionNode(GeneralUtils.GetResourceByName("global_price_type")) { Content = new PriceTypesPage(this) } },
                { "ArticleStock", new AccordionNode(GeneralUtils.GetResourceByName("global_stock_movements")) { Clicked = delegate { Utils.OpenArticleStockDialog(this); } } }
            };

            return articleButtons;
        }

    }
}
