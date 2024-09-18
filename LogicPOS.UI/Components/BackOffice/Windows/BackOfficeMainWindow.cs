using DevExpress.Data.Filtering;
using Gtk;
using logicpos;
using logicpos.App;
using logicpos.Classes.DataLayer;
using logicpos.Classes.Enums;
using logicpos.Classes.Enums.Finance;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Accordions;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.PoleDisplays;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.IO;


namespace LogicPOS.UI.Components.BackOffice.Windows
{
    public class BackOfficeMainWindow : BackOfficeBaseWindow
    {
        private readonly string _privilegesBackOfficeMenuOperation = $"{POSSettings.PrivilegesBackOfficeCRUDOperationPrefix}_{"MENU"}";

        [Obsolete]
        public BackOfficeMainWindow()
        {
            Initialize();
            ShowAll();
        }

        private void BackOfficeMainWindow_Show(object sender, EventArgs e)
        {
            _labelTerminalInfo.Text = string.Format("{0} : {1}", TerminalSettings.LoggedTerminal.Designation, XPOSettings.LoggedUser.Name);
            Accordion.UpdateMenuPrivileges();
            string currentNodePrivilegesToken = string.Format(_privilegesBackOfficeMenuOperation, Accordion.CurrentPageChildButton.Name.ToUpper());
            _currentPage.Sensitive = GeneralSettings.LoggedUserHasPermissionTo(currentNodePrivilegesToken);
        }

        [Obsolete]
        private void Initialize()
        {
            InitializeAccordionButtons();

            ResizeAccordionButtons();

            _panelButtons.Add(Accordion);

            AddEventHandlers();

            ShowStartPage();
        }

        private void ShowStartPage()
        {
            _currentPage = new DashBoardPage(this);
            _panelContent.PackEnd(_currentPage);
        }

        private void InitializeAccordionButtons()
        {
            var nodes = GetAccordionNodes();
            Accordion = new Accordion(nodes, POSSettings.PrivilegesBackOfficeMenuOperationFormat);
            Accordion.WidthRequest = _buttonsSize.Width;
        }

        private void AddEventHandlers()
        {
            Accordion.Clicked += AccordionButton_Click;

            _btnExit.Clicked += ButtonExit_Click;

            _btnNewVersion.Clicked += ButtonNewVesion_Click;

            _btnDashboard.Clicked += ButtonDashBoard_Click;

            _btnPOS.Clicked += ClickedSystemPos;

            Shown += BackOfficeMainWindow_Show;
        }

        private void ButtonExit_Click(object sender, EventArgs args)
        {
            LogicPOSApp.Quit(this);
        }

        private void ButtonDashBoard_Click(object sender, EventArgs args)
        {
            _btnDashboard.Page = new DashBoardPage(this);
            Button_Click(_btnDashboard, null);
        }

        private void ButtonNewVesion_Click(object sender, EventArgs args)
        {
            DateTime actualDate = DateTime.Now;

            string fileName = "\\LPUpdater\\LPUpdater.exe";
            string lPathToUpdater = string.Format(@"{0}\{1}", Environment.CurrentDirectory, fileName);

            if (File.Exists(lPathToUpdater))
            {
                ResponseType responseType = Utils.ShowMessageBox(this, DialogFlags.Modal, new System.Drawing.Size(600, 400), MessageType.Question, ButtonsType.YesNo, string.Format(GeneralUtils.GetResourceByName("window_title_dialog_update_POS"), GeneralSettings.ServerVersion), GeneralUtils.GetResourceByName("global_pos_update"));

                if (responseType == ResponseType.Yes)
                {
                    System.Diagnostics.Process.Start(lPathToUpdater);
                    LogicPOSApp.QuitWithoutConfirmation();
                }
            }
        }

        private void ResizeAccordionButtons()
        {
            if (GlobalApp.BackOfficeScreenSize.Height <= 800)
            {
                _panelButtons.Put(Accordion, 0, 28);
            }
            else
            {
                _panelButtons.Put(Accordion, 0, 40);
            }
        }

        public void Button_Click(object sender, EventArgs e)
        {
            IconButtonWithText button = (IconButtonWithText)sender;

            if (button.Page == null)
            {
                return;
            }

            if (_currentPage != null)
            {
                _panelContent.Remove(_currentPage);
            }

            _currentPage = button.Page;

            _labelActivePage.Text = button.Label;

            _currentPage.Visible = true;

            _panelContent.PackStart(_currentPage);
        }

        [Obsolete]
        private Dictionary<string, AccordionNode> GetAccordionNodes()
        {
            Dictionary<string, AccordionNode> nodes = new Dictionary<string, AccordionNode>();

            //Define used CriteriaOperators/Override Defaults from TreeViews
            CriteriaOperator criteriaOperatorCustomer = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1) AND (Hidden IS NULL OR Hidden = 0)");
            CriteriaOperator criteriaConfigurationPreferenceParameterCompany = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1) AND (Token <> 'COMPANY_COUNTRY_OID' AND Token <> 'SYSTEM_CURRENCY_OID' AND FormType = 1)");
            CriteriaOperator criteriaConfigurationPreferenceParameterSystem = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1) AND (FormType = 2)");

            //START WORK SESSION AND DAY FOR BACKOFFICE MODE
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

            //Users
            Dictionary<string, AccordionNode> _accordionChildUsers = new Dictionary<string, AccordionNode>
                {
                    { "UserDetail", new AccordionNode(GeneralUtils.GetResourceByName("global_users")) { Content = new UsersPage(this) } },
                    { "UserPermissionProfile", new AccordionNode(GeneralUtils.GetResourceByName("global_user_permissions")) { Content = new PermissionsPage(this) } },
                    { "UserCommissionGroup", new AccordionNode(GeneralUtils.GetResourceByName("global_user_commission_groups")) { Content = new CommissionGroupPage(this) } }
                };

            //Documents
            Dictionary<string, AccordionNode> _accordionDocuments = new Dictionary<string, AccordionNode>
                {
                    { "DocumentFinanceYears", new AccordionNode(GeneralUtils.GetResourceByName("global_documentfinance_years")) { Content = new FiscalYearsPage(this) } },
                    { "DocumentFinanceSeries", new AccordionNode(GeneralUtils.GetResourceByName("global_documentfinance_series")) { Content = new DocumentSeriesPage(this) } },
                    { "DocumentFinanceType", new AccordionNode(GeneralUtils.GetResourceByName("global_documentfinance_type")) { Content = new DocumentTypesPage(this) } },
                    { "ConfigurationVatRate", new AccordionNode(GeneralUtils.GetResourceByName("global_vat_rates")) { Content = new VatRatesPage(this) } },
                    { "ConfigurationVatExemptionReason", new AccordionNode(GeneralUtils.GetResourceByName("global_vat_exemption_reason")) { Content = new VatExemptionReasonsPage(this) } },
                    { "ConfigurationPaymentCondition", new AccordionNode(GeneralUtils.GetResourceByName("global_payment_conditions")) { Content = new PaymentConditionsPage(this) } },
                    { "ConfigurationPaymentMethod", new AccordionNode(GeneralUtils.GetResourceByName("global_payment_methods")) { Content = new PaymentMethodsPage(this) } }
                };

            //AuxiliarTables
            Dictionary<string, AccordionNode> _accordionChildAuxiliarTables = new Dictionary<string, AccordionNode>
                {
                    { "ConfigurationCountry", new AccordionNode(GeneralUtils.GetResourceByName("global_country")) { Content = new CountriesPage(this) } },
                    { "ConfigurationCurrency", new AccordionNode(GeneralUtils.GetResourceByName("global_ConfigurationCurrency")) { Content = new CurrenciesPage(this) } },
                    { "ConfigurationPlace", new AccordionNode(GeneralUtils.GetResourceByName("global_places")) { Content = new PlacesPage(this) } }
                };
            /* IN009035 */
            string configurationPlaceTableLabel = AppOperationModeSettings.IsDefaultTheme ? GeneralUtils.GetResourceByName("global_place_tables") : GeneralUtils.GetResourceByName("window_title_dialog_orders");
            _accordionChildAuxiliarTables.Add("ConfigurationPlaceTable", new AccordionNode(configurationPlaceTableLabel) { Content = new TablesPage(this) });
            _accordionChildAuxiliarTables.Add("ConfigurationPlaceMovementType", new AccordionNode(GeneralUtils.GetResourceByName("global_places_movement_type")) { Content = new MovementTypePage(this) });
            _accordionChildAuxiliarTables.Add("ConfigurationUnitMeasure", new AccordionNode(GeneralUtils.GetResourceByName("global_units_measure")) { Content = new MeasurementUnitsPage(this) });
            _accordionChildAuxiliarTables.Add("ConfigurationUnitSize", new AccordionNode(GeneralUtils.GetResourceByName("global_units_size")) { Content = new SizeUnitsPage(this) });
            _accordionChildAuxiliarTables.Add("ConfigurationHolidays", new AccordionNode(GeneralUtils.GetResourceByName("global_holidays")) { Content = new HolidayPage(this) });
            _accordionChildAuxiliarTables.Add("Warehouse", new AccordionNode(GeneralUtils.GetResourceByName("global_warehouse")) { Content = new WarehousesPage(this) });


            //Devices
            Dictionary<string, AccordionNode> _accordionDevices = new Dictionary<string, AccordionNode>
                {
                    { "ConfigurationPrintersType", new AccordionNode(GeneralUtils.GetResourceByName("global_ConfigurationPrintersType")) { Content = new PrinterTypesPage(this) } },
                    { "ConfigurationPrinters", new AccordionNode(GeneralUtils.GetResourceByName("global_ConfigurationPrinters")) { Content = new PrintersPage(this) } },
                    { "ConfigurationInputReader", new AccordionNode(GeneralUtils.GetResourceByName("global_ConfigurationInputReader")) { Content = new InputReadersPage(this) } },
                    { "ConfigurationPoleDisplay", new AccordionNode(GeneralUtils.GetResourceByName("global_ConfigurationPoleDisplay")) { Content = new PoleDisplaysPage(this) } },
                    { "ConfigurationWeighingMachine", new AccordionNode(GeneralUtils.GetResourceByName("global_ConfigurationWeighingMachine")) { Content = new WeighingMachinesPage(this) } }
                };

            //Configuration
            Dictionary<string, AccordionNode> _accordionChildConfiguration = new Dictionary<string, AccordionNode>
                {
                    { "ConfigurationPreferenceParameterCompany", new AccordionNode(GeneralUtils.GetResourceByName("global_preferenceparameter_company")) { Content = new PreferenceParametersPage(this, new Dictionary<string, string>{{"parameters","company"}}) } },
                    { "ConfigurationPreferenceParameterSystem", new AccordionNode(GeneralUtils.GetResourceByName("global_preferenceparameter_system")) {Content = new PreferenceParametersPage(this, new Dictionary<string, string>{{"parameters","system"}}) }  },
                    { "ConfigurationPlaceTerminal", new AccordionNode(GeneralUtils.GetResourceByName("global_places_terminals")) { Content = new TerminalsPage(this) } }
                };


            //import                
            Dictionary<string, AccordionNode> _accordionChildImport = new Dictionary<string, AccordionNode>
                {
                    { "System_Import_Articles", new AccordionNode(GeneralUtils.GetResourceByName("global_import_articles")) { Clicked = delegate { ExcelProcessing.OpenFilePicker(this, ImportExportFileOpen.OpenExcelArticles); } } },
                    { "System_Import_Costumers", new AccordionNode(GeneralUtils.GetResourceByName("global_import_costumers")) { Clicked = delegate { ExcelProcessing.OpenFilePicker(this, ImportExportFileOpen.OpenExcelCostumers); } } }
                };


            // Add Menu Items Based On Plugins PluginSoftwareVendor
            Dictionary<string, AccordionNode> _accordionChildExport = new Dictionary<string, AccordionNode>();
            //Export
            if (PluginSettings.HasSoftwareVendorPlugin && (CultureSettings.CountryIdIsPortugal(XPOSettings.ConfigurationSystemCountry.Oid) || CultureSettings.CountryIdIsAngola(XPOSettings.ConfigurationSystemCountry.Oid)))
            {
                if (System.Configuration.ConfigurationManager.AppSettings["cultureFinancialRules"] == "pt-AO" || System.Configuration.ConfigurationManager.AppSettings["cultureFinancialRules"] == "pt-PT")
                {
                    _accordionChildExport.Add("System_ExportSaftPT_SaftPt", new AccordionNode(GeneralUtils.GetResourceByName("global_export_saftpt_whole_year")) { Clicked = delegate { FrameworkCalls.ExportSaft(this, ExportSaftPtMode.WholeYear); } });
                    _accordionChildExport.Add("System_ExportSaftPT_E-Fatura", new AccordionNode(GeneralUtils.GetResourceByName("global_export_saftpt_last_month")) { Clicked = delegate { FrameworkCalls.ExportSaft(this, ExportSaftPtMode.LastMonth); } });
                    _accordionChildExport.Add("System_ExportSaftPT_Custom", new AccordionNode(GeneralUtils.GetResourceByName("global_export_saftpt_custom")) { Clicked = delegate { FrameworkCalls.ExportSaft(this, ExportSaftPtMode.Custom); } });
                }
            }
            _accordionChildExport.Add("System_Export_Articles", new AccordionNode(GeneralUtils.GetResourceByName("global_export_articles")) { Clicked = delegate { ExcelProcessing.OpenFilePicker(this, ImportExportFileOpen.ExportArticles); } });
            _accordionChildExport.Add("System_Export_Costumers", new AccordionNode(GeneralUtils.GetResourceByName("global_export_costumers")) { Clicked = delegate { ExcelProcessing.OpenFilePicker(this, ImportExportFileOpen.ExportCustomers); } });
            //System
            Dictionary<string, AccordionNode> _accordionChildSystem = new Dictionary<string, AccordionNode>
                {
                    /* IN006001 - "System" > "Notification" menu option */
                    { "System_Notification", new AccordionNode(GeneralUtils.GetResourceByName("window_title_dialog_notification")) { Clicked = delegate { Utils.ShowNotifications(this, true); } } },
                    { "System_ChangeLog", new AccordionNode(GeneralUtils.GetResourceByName("change_log")) { Clicked = delegate { Utils.ShowChangeLog(this); } } }
                };
            // Add Menu Items Based On Plugins PluginSoftwareVendor
            if (PluginSettings.HasSoftwareVendorPlugin)
            {
                _accordionChildSystem.Add("System_DataBaseBackup", new AccordionNode(GeneralUtils.GetResourceByName("global_database_backup")) { Clicked = delegate { DataBaseBackup.Backup(this); } });
                _accordionChildSystem.Add("System_DataBaseRestore_FromSystem", new AccordionNode(GeneralUtils.GetResourceByName("global_database_restore")) { Clicked = delegate { DataBaseBackup.Restore(this, DataBaseRestoreFrom.SystemBackup); } });
                _accordionChildSystem.Add("System_DataBaseRestore_FromFile", new AccordionNode(GeneralUtils.GetResourceByName("global_database_restore_from_file")) { Clicked = delegate { DataBaseBackup.Restore(this, DataBaseRestoreFrom.ChooseFromFilePickerDialog); } });
            }
            _accordionChildSystem.Add("System_Menu", new AccordionNode(GeneralUtils.GetResourceByName("global_application_logout_user")) { Clicked = ClickedSystemLogout });

            if (GeneralSettings.AppUseBackOfficeMode)
            {
                var documentButtons = CreateDocumentButtons();
                nodes.Add("TopMenuFinanceDocuments", new AccordionNode(GeneralUtils.GetResourceByName("dialog_button_label_select_record_finance_documents")) { Children = documentButtons, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_documentos.png") });

                var reportButtons = CreateReporButtons();
                nodes.Add("TopMenuReports", new AccordionNode(GeneralUtils.GetResourceByName("global_reports")) { Children = reportButtons, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_relatorios.png") });
            }

            var articleButtons = CreateArticleButtons();
            nodes.Add("TopMenuArticles", new AccordionNode(GeneralUtils.GetResourceByName("global_articles")) { Children = articleButtons, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_artigos.png") });

            nodes.Add("TopMenuDocuments", new AccordionNode(GeneralUtils.GetResourceByName("global_documents")) { Children = _accordionDocuments, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_informacao_fiscal.png") });

            var customerButtons = CreateCustomerButtons(criteriaOperatorCustomer);
            nodes.Add("TopMenuCustomers", new AccordionNode(GeneralUtils.GetResourceByName("global_customers")) { Children = customerButtons, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_clientes.png") });

            nodes.Add("TopMenuUsers", new AccordionNode(GeneralUtils.GetResourceByName("global_users")) { Children = _accordionChildUsers, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_utilizadores.png") });
            nodes.Add("TopMenuDevices", new AccordionNode(GeneralUtils.GetResourceByName("global_devices")) { Children = _accordionDevices, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_impressoras.png") });
            nodes.Add("TopMenuOtherTables", new AccordionNode(GeneralUtils.GetResourceByName("global_other_tables")) { Children = _accordionChildAuxiliarTables, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_outras_tabelas.png") });
            nodes.Add("TopMenuConfiguration", new AccordionNode(GeneralUtils.GetResourceByName("global_configuration")) { Children = _accordionChildConfiguration, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_configuracao.png") });


            nodes.Add("TopMenuImport", new AccordionNode(GeneralUtils.GetResourceByName("global_import")) { Children = _accordionChildImport, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_import.png") });

            if (_accordionChildExport.Count > 0)
            {
                nodes.Add("TopMenuExport", new AccordionNode(GeneralUtils.GetResourceByName("global_export")) { Children = _accordionChildExport, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_export.png") });
            }


            return nodes;
        }

        private Dictionary<string, AccordionNode> CreateCustomerButtons(CriteriaOperator criteriaOperatorCustomer)
        {
            return new Dictionary<string, AccordionNode>
                {
                    { "Customer", new AccordionNode(GeneralUtils.GetResourceByName("global_customers")) { Content = new CustomersPage(this) } },
                    { "CustomerType", new AccordionNode(GeneralUtils.GetResourceByName("global_customer_types")) { Content = new CustomerTypePage(this) } },
                    { "CustomerDiscountGroup", new AccordionNode(GeneralUtils.GetResourceByName("global_customer_discount_groups")) { Content = new DiscountGroupPage(this) } }
                };
        }

        private Dictionary<string, AccordionNode> CreateReporButtons()
        {
            return new Dictionary<string, AccordionNode>
                {
                    { "DocumentsReports", new AccordionNode(GeneralUtils.GetResourceByName("global_reports")) { Clicked = delegate { Utils.StartReportsMenuFromBackOffice(this); } } }
                };
        }

        private Dictionary<string, AccordionNode> CreateDocumentButtons()
        {
            return new Dictionary<string, AccordionNode>
                {
                    { "DocumentsNew", new AccordionNode(GeneralUtils.GetResourceByName("dialog_button_label_new_finance_documents")) { Clicked = delegate { Utils.StartNewDocumentFromBackOffice(this); } } },
                    { "DocumentsShow", new AccordionNode(GeneralUtils.GetResourceByName("dialog_button_label_select_record_finance_documents")) { Clicked = delegate { Utils.StartDocumentsMenuFromBackOffice(this, 1); } } },
                    { "DocumentsPay", new AccordionNode(GeneralUtils.GetResourceByName("dialog_button_label_select_finance_documents_ft_unpaid")) { Clicked = delegate { Utils.StartDocumentsMenuFromBackOffice(this, 2); } } },
                    { "DocumentsPayments", new AccordionNode(GeneralUtils.GetResourceByName("dialog_button_label_select_payments")) { Clicked = delegate { Utils.StartDocumentsMenuFromBackOffice(this, 3); } } },
                    { "DocumentsCurrentAccount", new AccordionNode(GeneralUtils.GetResourceByName("dialog_button_label_select_finance_documents_cc")) { Clicked = delegate { Utils.StartDocumentsMenuFromBackOffice(this, 4); } } }
                };
        }

        [Obsolete]
        private Dictionary<string, AccordionNode> CreateArticleButtons()
        {
            Dictionary<string, AccordionNode> articleButtons = new Dictionary<string, AccordionNode>
            {
                { "ArticleFamily", new AccordionNode(GeneralUtils.GetResourceByName("global_families")) { Content = new ArticleFamiliesPage(this) } },
                { "ArticleSubFamily", new AccordionNode(GeneralUtils.GetResourceByName("global_subfamilies")) { Content = new ArticleSubfamiliesPage(this) } },
                { "Article", new AccordionNode(GeneralUtils.GetResourceByName("global_articles")) { Content = new ArticlesPage(this) } },
                { "ArticleType", new AccordionNode(GeneralUtils.GetResourceByName("global_article_types")) { Content = new ArticleTypePage(this) } },
                { "ArticleClass", new AccordionNode(GeneralUtils.GetResourceByName("global_article_class")) { Content = new ArticleClassPage(this) } },
                { "ConfigurationPriceType", new AccordionNode(GeneralUtils.GetResourceByName("global_price_type")) { Content = new PriceTypesPage(this) } },
                { "ArticleStock", new AccordionNode(GeneralUtils.GetResourceByName("global_stock_movements")) { Clicked = delegate { Utils.OpenArticleStockDialog(this); } } }
            };

            return articleButtons;
        }

        private void ClickedSystemLogout(object sender, EventArgs e)
        {
            Hide();
            //Call Shared WindowStartup LogOutUser, and Show WindowStartup
            GlobalApp.StartupWindow.LogOutUser(true);
        }

        private void ClickedSystemPos(object sender, EventArgs e)
        {
            Hide();
            //Show WindowStartup
            GlobalApp.PosMainWindow.ShowAll();
        }
    }
}
