using DevExpress.Data.Filtering;
using Gtk;
using logicpos.App;
using logicpos.Classes.DataLayer;
using logicpos.Classes.Enums;
using logicpos.Classes.Enums.Finance;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Finance.WorkSession;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using System;
using System.Collections.Generic;
using System.IO;

//Notes

//Get Reference for TreeView SubMenus ex Refresh() Tree
//Use "(_sourceWindow as BackOfficeMainWindow).Accordion..." or "(GlobalApp.WindowBackOffice.Accordion..." Reference
//((_sourceWindow as BackOfficeMainWindow).Accordion.Nodes["TopMenuDocuments"].Childs["DocumentFinanceSeries"].Content as TreeViewDocumentFinanceSeries).Refresh();
//((_sourceWindow as BackOfficeMainWindow).Accordion.Nodes["TopMenuDocuments"].Childs["DocumentFinanceYearSerieTerminal"].Content as TreeViewDocumentFinanceYearSerieTerminal).Refresh();

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    public class BackOfficeMainWindow : BackOfficeBaseWindow
    {
        //Config
        private readonly string _privilegesBackOfficeMenuOperation = string.Format("{0}_{1}", POSSettings.PrivilegesBackOfficeCRUDOperationPrefix, "MENU");
        //protected log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //ForBackOfficeMode

        public BackOfficeMainWindow(bool empty)
        {
            InitUI();
            ShowAll();
        }

        public BackOfficeMainWindow()
        {
            //Info
            _logger.Debug("BackOfficeMainWindow(): Create object BackOfficeMainWindow");

            if (!GeneralSettings.AppUseBackOfficeMode)
            {
                this.WindowStateEvent += PosMainWindow_WindowStateEvent;
            }


            //Disable not we dont have a title bar
            // Title = Utils.GetWindowTitle(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_backoffice);
            InitUI();
            ShowAll();

            //First Time Assigmnent
            _activeUserDetail = XPOSettings.LoggedUser;

            //Events
            this.Shown += BackOfficeMainWindow_Show;
        }

        private void BackOfficeMainWindow_Show(object sender, EventArgs e)
        {
            if (_activeUserDetail != XPOSettings.LoggedUser)
            {
                _activeUserDetail = XPOSettings.LoggedUser;
                _labelTerminalInfo.Text = string.Format("{0} : {1}", TerminalSettings.LoggedTerminal.Designation, XPOSettings.LoggedUser.Name);

                //Apply Menu Updates
                Accordion.UpdateMenuPrivileges();

                //Hide/Show Current Active Content based on user privileges
                string currentNodePrivilegesToken = string.Format(_privilegesBackOfficeMenuOperation, Accordion.CurrentChildButtonContent.Name.ToUpper());
                _nodeContent.Sensitive = GeneralSettings.LoggedUserHasPermissionTo(currentNodePrivilegesToken);
            }
        }

        private void PosMainWindow_WindowStateEvent(object o, WindowStateEventArgs args)
        {
        }

        [Obsolete]
        private void InitUI()
        {
            Accordion = new Accordion(GetAccordionDefinition(), POSSettings.PrivilegesBackOfficeMenuOperationFormat) { WidthRequest = _widthAccordion };
            //Reajustar posição dos Botões do Accordion para 1024x768
            if (GlobalApp.BoScreenSize.Height <= 800)
            {
                _fixAccordion.Put(Accordion, 0, 28);
            }
            else
            {
                _fixAccordion.Put(Accordion, 0, 40);
            }
            _fixAccordion.Add(Accordion);
            Accordion.Clicked += accordion_Clicked;
            //_dashboardButton.Clicked += ((AccordionNode)_accordionChildArticlesTemp["Article"]).Clicked;
            //_dashboardButton.Content = ((Widget)_accordionChildDocumentsTemp["DocumentsListall"].Content);
            //_dashboardButton.Content = Utils.GetGenericTreeViewXPO<DashBoard>(this);
            _exitButton.Clicked += delegate { LogicPOSApp.Quit(this); };

            //TK016248 BackOffice - Check New Version
            _NewVersion.Clicked += delegate
            {
                DateTime actualDate = DateTime.Now;
                //if (actualDate <= GlobalFramework.LicenceUpdateDate)
                //{
                string fileName = "\\LPUpdater\\LPUpdater.exe";
                string lPathToUpdater = string.Format(@"{0}\{1}", Environment.CurrentDirectory, fileName);
                //string lPathToUpdater = "" + Utils.GetCurrentDirectory() + "\\LPUpdater\\LPUpdater.exe";

                if (File.Exists(lPathToUpdater))
                {
                    ResponseType responseType = logicpos.Utils.ShowMessageBox(this, DialogFlags.Modal, new System.Drawing.Size(600, 400), MessageType.Question, ButtonsType.YesNo, string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_update_POS"), GeneralSettings.ServerVersion), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_pos_update"));

                    if (responseType == ResponseType.Yes)
                    {
                        System.Diagnostics.Process.Start(lPathToUpdater);
                        //Process.Start(lPathToUpdater);
                        LogicPOSApp.QuitWithoutConfirmation();
                    }
                }
                //}
                //else
                //{
                //   Utils.ShowMessageTouch(this, DialogFlags.Modal, new System.Drawing.Size(600, 400), MessageType.Error, ButtonsType.Ok, string.Format(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_error"), GlobalFramework.ServerVersion), CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_license_blocked"));
                //}

            };

            //Imagem do dashboard carregada novamente. evento chamado
            _dashboardButton.Clicked += delegate
            {
                _dashboardButton.Content = logicpos.Utils.GetGenericTreeViewXPO<DashBoard>(this);
                _dashboardButton_Clicked(_dashboardButton, null);
            };

            _backPOS.Clicked += ClickedSystemPos;
        }

        public void _dashboardButton_Clicked(object sender, EventArgs e)
        {
            TouchButtonIconWithText btClicked = (TouchButtonIconWithText)sender;
            try
            {
                //Show Button Content if its a Chield Button
                if (btClicked.GetType() == typeof(TouchButtonIconWithText))
                {
                    if (btClicked.Content != null)
                    {
                        if (_nodeContent != null)
                        {

                            _hboxContent.Remove(_nodeContent);
                        }
                        _nodeContent = btClicked.Content;
                        //Store active content button nodeContentActiveButton to reference to have access to Name, used for previleges etc
                        //_accordion.CurrentChildButtonContent = btClicked;
                        _labelActiveContent.Text = btClicked.Label;

                        if (!_nodeContent.Visible)
                        {
                            _nodeContent.Visible = true;
                        }
                        _hboxContent.PackStart(_nodeContent);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        [Obsolete]
        private Dictionary<string, AccordionNode> GetAccordionDefinition()
        {
            _logger.Debug("GetAccordionDefinition Begin");

            //Init accordionDefinition
            Dictionary<string, AccordionNode> accordionDefinition = null;
            try
            {
                accordionDefinition = new Dictionary<string, AccordionNode>();
                Widget startContent;

                //Define Start Content for backoffice mode
                _dashboardButton.Content = logicpos.Utils.GetGenericTreeViewXPO<DashBoard>(this);
                startContent = logicpos.Utils.GetGenericTreeViewXPO<DashBoard>(this);
                //_labelActiveContent.Text = "DASHBOARD";

                ////Define Start Content with Articles TreeView
                //else
                //{
                //    startContent = Utils.GetGenericTreeViewXPO<TreeViewArticle>(this);
                //    //Hide/Show Current Active Content based on user privileges
                //    string currentNodePrivilegesToken = string.Format(_privilegesBackOfficeMenuOperation, "Article".ToUpper());
                //    startContent.Sensitive = FrameworkUtils.HasPermissionTo(currentNodePrivilegesToken);
                //    _labelActiveContent.Text = CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_articles");
                //}


                _nodeContent = startContent;

                _hboxContent.PackEnd(_nodeContent);

                //Define used CriteriaOperators/Override Defaults from TreeViews
                CriteriaOperator criteriaOperatorCustomer = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1) AND (Hidden IS NULL OR Hidden = 0)");
                CriteriaOperator criteriaConfigurationPreferenceParameterCompany = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1) AND (Token <> 'COMPANY_COUNTRY_OID' AND Token <> 'SYSTEM_CURRENCY_OID' AND FormType = 1)");
                CriteriaOperator criteriaConfigurationPreferenceParameterSystem = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1) AND (FormType = 2)");

                //START WORK SESSION AND DAY FOR BACKOFFICE MODE
                if (GeneralSettings.AppUseBackOfficeMode)
                {
                    bool openDay = ProcessWorkSessionPeriod.SessionPeriodOpen(WorkSessionPeriodType.Day, "");
                    if (openDay)
                    {
                        pos_worksessionperiod workSessionPeriodDay = ProcessWorkSessionPeriod.GetSessionPeriod(WorkSessionPeriodType.Day);
                        XPOSettings.WorkSessionPeriodTerminal = ProcessWorkSessionPeriod.GetSessionPeriod(WorkSessionPeriodType.Day);
                        XPOSettings.WorkSessionPeriodTerminal.SessionStatus = WorkSessionPeriodStatus.Open;
                    }
                }

                ////TK016235 BackOffice - Mode - Finance Documents for backoffice mode
                Dictionary<string, AccordionNode> _accordionChildDocuments = new Dictionary<string, AccordionNode>
                {
                    { "DocumentsNew", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_button_label_new_finance_documents")) { Clicked = delegate { logicpos.Utils.StartNewDocumentFromBackOffice(this); } } },
                    { "DocumentsShow", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_button_label_select_record_finance_documents")) { Clicked = delegate { logicpos.Utils.StartDocumentsMenuFromBackOffice(this, 1); } } },
                    { "DocumentsPay", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_button_label_select_finance_documents_ft_unpaid")) { Clicked = delegate { logicpos.Utils.StartDocumentsMenuFromBackOffice(this, 2); } } },
                    { "DocumentsPayments", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_button_label_select_payments")) { Clicked = delegate { logicpos.Utils.StartDocumentsMenuFromBackOffice(this, 3); } } },
                    { "DocumentsCurrentAccount", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_button_label_select_finance_documents_cc")) { Clicked = delegate { logicpos.Utils.StartDocumentsMenuFromBackOffice(this, 4); } } }
                };
                //_accordionChildDocuments.Add("DocumentsListall", new AccordionNode(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_worksession_resume_finance_documents")) {  Content = Utils.GetGenericTreeViewXPO<DashBoard>(this) }); ;
                logicpos.Utils util = new logicpos.Utils();
                util.AccordionChildDocumentsTemp = _accordionChildDocuments;
                Dictionary<string, AccordionNode> _accordionChildReports = new Dictionary<string, AccordionNode>
                {
                    { "DocumentsReports", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_reports")) { Clicked = delegate { logicpos.Utils.StartReportsMenuFromBackOffice(this); } } }
                };

                //Articles
                Dictionary<string, AccordionNode> _accordionChildArticles = new Dictionary<string, AccordionNode>
                {
                    //, Clicked = testClickedEventHandlerFromOutside }
                    { "ArticleFamily", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_families")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewArticleFamily>(this) } }
                };
                ;
                _accordionChildArticles.Add("ArticleSubFamily", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_subfamilies")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewArticleSubFamily>(this) });
                _accordionChildArticles.Add("Article", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_articles")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewArticle>(this) });
                _accordionChildArticles.Add("ArticleType", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_types")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewArticleType>(this) });
                _accordionChildArticles.Add("ArticleClass", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_class")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewArticleClass>(this) });
                _accordionChildArticles.Add("ConfigurationPriceType", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_price_type")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPriceType>(this) });
                // Disable to Speed uo Opening BO, noew we have Stock Reports
                _accordionChildArticles.Add("ArticleStock", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_stock_movements")) { /*Content = Utils.GetGenericTreeViewXPO<TreeViewArticleStock>(this),*/ Clicked = delegate { logicpos.Utils.OpenArticleStockDialog(this); } });

                //Customers
                Dictionary<string, AccordionNode> _accordionChildCustomers = new Dictionary<string, AccordionNode>
                {
                    { "Customer", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_customers")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewCustomer>(this, criteriaOperatorCustomer) } },
                    { "CustomerType", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_customer_types")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewCustomerType>(this) } },
                    { "CustomerDiscountGroup", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_customer_discount_groups")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewCustomerDiscountGroup>(this) } }
                };

                //Users
                Dictionary<string, AccordionNode> _accordionChildUsers = new Dictionary<string, AccordionNode>
                {
                    { "UserDetail", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_users")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewUser>(this) } },
                    //Commented by Mario: Not Usefull, UserPermissionProfile has same funtionality
                    //_accordionChildUsers.Add("UserProfile", new AccordionNode(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_profile) { Content = Utils.GetGenericTreeViewXPO<TreeViewUserProfile>(this) });
                    //WARNING: Works with diferent constructs, its still need to be improved : new TreeViewUserProfilePermissions(this)
                    { "UserPermissionProfile", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_user_permissions")) { Content = new TreeViewUserProfilePermissions(this) } },
                    { "UserCommissionGroup", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_user_commission_groups")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewUserCommissionGroup>(this) } }
                };
                //Moved to Custom Toolbar
                //_accordionChildUsers.Add("System_ApplyPrivileges", new AccordionNode(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_user_apply_privileges) { Clicked = delegate { Accordion.UpdateMenuPrivileges(); } });

                //Documents
                Dictionary<string, AccordionNode> _accordionDocuments = new Dictionary<string, AccordionNode>
                {
                    { "DocumentFinanceYears", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_years")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewDocumentFinanceYears>(this) } },
                    { "DocumentFinanceSeries", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_series")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewDocumentFinanceSeries>(this) } },
                    { "DocumentFinanceType", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_type")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewDocumentFinanceType>(this) } },
                    //_accordionDocuments.Add("DocumentFinanceYearSerieTerminal", new AccordionNode(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_documentfinance_yearsseriesterminal) { Content = Utils.GetGenericTreeViewXPO<TreeViewDocumentFinanceYearSerieTerminal>(this) });
                    { "ConfigurationVatRate", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_vat_rates")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationVatRate>(this) } },
                    { "ConfigurationVatExemptionReason", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_vat_exemption_reason")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationVatExceptionReason>(this) } },
                    { "ConfigurationPaymentCondition", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_payment_conditions")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPaymentCondition>(this) } },
                    { "ConfigurationPaymentMethod", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_payment_methods")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPaymentMethod>(this) } }
                };

                //AuxiliarTables
                Dictionary<string, AccordionNode> _accordionChildAuxiliarTables = new Dictionary<string, AccordionNode>
                {
                    //_accordionChildAuxiliarTables.Add("ConfigurationCashRegister", new AccordionNode(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_cash_registers) { Content = Utils.GetGenericTreeView<TreeViewConfigurationCashRegister>(this) });
                    { "ConfigurationCountry", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_country")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationCountry>(this) } },
                    { "ConfigurationCurrency", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ConfigurationCurrency")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationCurrency>(this) } },
                    //_accordionChildAuxiliarTables.Add("ConfigurationDevice", new AccordionNode(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_devices) { Content = Utils.GetGenericTreeView<TreeViewConfigurationDevice>(this) });
                    //_accordionChildAuxiliarTables.Add("ConfigurationKeyboard", new AccordionNode(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_keyboards) { Content = Utils.GetGenericTreeView<TreeViewConfigurationKeyboard>(this) });
                    //_accordionChildAuxiliarTables.Add("ConfigurationMaintenance", new AccordionNode(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_maintenance) { Content = Utils.GetGenericTreeView<TreeViewConfigurationMaintenance>(this) });
                    { "ConfigurationPlace", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_places")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPlace>(this) } }
                };
                /* IN009035 */
                string configurationPlaceTableLabel = AppOperationModeSettings.IsDefaultTheme ? CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_place_tables") : CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_orders");
                _accordionChildAuxiliarTables.Add("ConfigurationPlaceTable", new AccordionNode(configurationPlaceTableLabel) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPlaceTable>(this) });
                _accordionChildAuxiliarTables.Add("ConfigurationPlaceMovementType", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_places_movement_type")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPlaceMovementType>(this) });
                _accordionChildAuxiliarTables.Add("ConfigurationUnitMeasure", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_units_measure")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationUnitMeasure>(this) });
                _accordionChildAuxiliarTables.Add("ConfigurationUnitSize", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_units_size")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationUnitSize>(this) });
                _accordionChildAuxiliarTables.Add("ConfigurationHolidays", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_holidays")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationHolidays>(this) });
                _accordionChildAuxiliarTables.Add("Warehouse", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_warehouse")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewWarehouse>(this) });


                //Devices
                Dictionary<string, AccordionNode> _accordionDevices = new Dictionary<string, AccordionNode>
                {
                    { "ConfigurationPrintersType", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ConfigurationPrintersType")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPrintersType>(this) } },
                    { "ConfigurationPrinters", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ConfigurationPrinters")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPrinters>(this) } },
                    { "ConfigurationInputReader", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ConfigurationInputReader")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationInputReader>(this) } },
                    { "ConfigurationPoleDisplay", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ConfigurationPoleDisplay")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPoleDisplay>(this) } },
                    { "ConfigurationWeighingMachine", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ConfigurationWeighingMachine")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationWeighingMachine>(this) } }
                };
                // Deprecated
                //_accordionPrinters.Add("ConfigurationPrintersTemplates", new AccordionNode(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationPrintersTemplates) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPrintersTemplates>(this) });
                //_accordionPrinters.Add("ExternalApp_Composer", new AccordionNode(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_callposcomposer) { Content = null, ExternalAppFileName = SettingsApp.ExecutableComposer });

                //Configuration
                Dictionary<string, AccordionNode> _accordionChildConfiguration = new Dictionary<string, AccordionNode>
                {
                    { "ConfigurationPreferenceParameterCompany", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_preferenceparameter_company")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPreferenceParameter>(this, criteriaConfigurationPreferenceParameterCompany) } },
                    { "ConfigurationPreferenceParameterSystem", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_preferenceparameter_system")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPreferenceParameter>(this, criteriaConfigurationPreferenceParameterSystem) } },
                    { "ConfigurationPlaceTerminal", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_places_terminals")) { Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPlaceTerminal>(this) } }
                };


                //import                
                Dictionary<string, AccordionNode> _accordionChildImport = new Dictionary<string, AccordionNode>
                {
                    { "System_Import_Articles", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_import_articles")) { Clicked = delegate { ExcelProcessing.OpenFilePicker(this, ImportExportFileOpen.OpenExcelArticles); } } },
                    { "System_Import_Costumers", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_import_costumers")) { Clicked = delegate { ExcelProcessing.OpenFilePicker(this, ImportExportFileOpen.OpenExcelCostumers); } } }
                };


                // Add Menu Items Based On Plugins PluginSoftwareVendor
                Dictionary<string, AccordionNode> _accordionChildExport = new Dictionary<string, AccordionNode>();
                //Export
                if (PluginSettings.HasSoftwareVendorPlugin && (CultureSettings.CountryIdIsPortugal(XPOSettings.ConfigurationSystemCountry.Oid) || CultureSettings.CountryIdIsAngola(XPOSettings.ConfigurationSystemCountry.Oid)))
                {
                    if ((System.Configuration.ConfigurationManager.AppSettings["cultureFinancialRules"] == "pt-AO") || (System.Configuration.ConfigurationManager.AppSettings["cultureFinancialRules"] == "pt-PT"))
                    {
                        _accordionChildExport.Add("System_ExportSaftPT_SaftPt", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_export_saftpt_whole_year")) { Clicked = delegate { FrameworkCalls.ExportSaft(this, ExportSaftPtMode.WholeYear); } });
                        _accordionChildExport.Add("System_ExportSaftPT_E-Fatura", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_export_saftpt_last_month")) { Clicked = delegate { FrameworkCalls.ExportSaft(this, ExportSaftPtMode.LastMonth); } });
                        _accordionChildExport.Add("System_ExportSaftPT_Custom", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_export_saftpt_custom")) { Clicked = delegate { FrameworkCalls.ExportSaft(this, ExportSaftPtMode.Custom); } });
                    }
                }
                _accordionChildExport.Add("System_Export_Articles", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_export_articles")) { Clicked = delegate { ExcelProcessing.OpenFilePicker(this, ImportExportFileOpen.ExportArticles); } });
                _accordionChildExport.Add("System_Export_Costumers", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_export_costumers")) { Clicked = delegate { ExcelProcessing.OpenFilePicker(this, ImportExportFileOpen.ExportCustomers); } });
                //System
                Dictionary<string, AccordionNode> _accordionChildSystem = new Dictionary<string, AccordionNode>
                {
                    /* IN006001 - "System" > "Notification" menu option */
                    { "System_Notification", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_notification")) { Clicked = delegate { logicpos.Utils.ShowNotifications(this, true); } } },
                    { "System_ChangeLog", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "change_logger")) { Clicked = delegate { logicpos.Utils.ShowChangeLog(this); } } }
                };
                // Add Menu Items Based On Plugins PluginSoftwareVendor
                if (PluginSettings.HasSoftwareVendorPlugin)
                {
                    _accordionChildSystem.Add("System_DataBaseBackup", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_database_backup")) { Clicked = delegate { DataBaseBackup.Backup(this); } });
                    _accordionChildSystem.Add("System_DataBaseRestore_FromSystem", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_database_restore")) { Clicked = delegate { DataBaseBackup.Restore(this, DataBaseRestoreFrom.SystemBackup); } });
                    _accordionChildSystem.Add("System_DataBaseRestore_FromFile", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_database_restore_from_file")) { Clicked = delegate { DataBaseBackup.Restore(this, DataBaseRestoreFrom.ChooseFromFilePickerDialog); } });
                }
                _accordionChildSystem.Add("System_Menu", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_application_loggerout_user")) { Clicked = ClickedSystemLogout });
                //                _accordionChildSystem.Add("System_Pos", new AccordionNode(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_pos) { Clicked = ClickedSystemPos });
                //_accordionChildSystem.Add("System_Quit", new AccordionNode(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_quit")) { Clicked = delegate { LogicPos.Quit(this); } });

                //Compose Main Accordion Parent Buttons
                //TK016235 BackOffice - Mode
                if (GeneralSettings.AppUseBackOfficeMode)
                {
                    accordionDefinition.Add("TopMenuFinanceDocuments", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_button_label_select_record_finance_documents")) { Childs = _accordionChildDocuments, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_documentos.png") });
                }
                accordionDefinition.Add("TopMenuArticles", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_articles")) { Childs = _accordionChildArticles, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_artigos.png") });
                accordionDefinition.Add("TopMenuDocuments", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documents")) { Childs = _accordionDocuments, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_informacao_fiscal.png") });
                accordionDefinition.Add("TopMenuCustomers", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_customers")) { Childs = _accordionChildCustomers, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_clientes.png") });
                accordionDefinition.Add("TopMenuUsers", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_users")) { Childs = _accordionChildUsers, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_utilizadores.png") });
                accordionDefinition.Add("TopMenuDevices", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_devices")) { Childs = _accordionDevices, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_impressoras.png") });
                accordionDefinition.Add("TopMenuOtherTables", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_other_tables")) { Childs = _accordionChildAuxiliarTables, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_outras_tabelas.png") });
                accordionDefinition.Add("TopMenuConfiguration", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_configuration")) { Childs = _accordionChildConfiguration, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_configuracao.png") });
                //TK016235 BackOffice - Mode
                if (GeneralSettings.AppUseBackOfficeMode)
                {
                    accordionDefinition.Add("TopMenuReports", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_reports")) { Childs = _accordionChildReports, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_relatorios.png") });
                }

                accordionDefinition.Add("TopMenuImport", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_import")) { Childs = _accordionChildImport, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_import.png") });

                if (_accordionChildExport.Count > 0)
                {
                    accordionDefinition.Add("TopMenuExport", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_export")) { Childs = _accordionChildExport, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_export.png") });
                }
                accordionDefinition.Add("TopMenuSystem", new AccordionNode(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_system")) { Childs = _accordionChildSystem, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_sistema.png") });



                //Assets/Images/Icons/Accordion/pos_backoffice_sistema.png
                //Assets/Images/Icons/icon_pos_toolbar_back_office.png
                //TK016235 BackOffice - Mode
                //if (!GlobalFramework.AppUseBackOfficeMode)
                //{
                //    Dictionary<string, AccordionNode> _accordionChildSystemPOSMainWindow = new Dictionary<string, AccordionNode>();
                //    _accordionChildSystemPOSMainWindow.Add("System_Pos", new AccordionNode(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_pos")) { Clicked = ClickedSystemPos });
                //    accordionDefinition.Add("TopMenuPOSMainWindow", new AccordionNode(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_pos")) { Childs = _accordionChildSystemPOSMainWindow, GroupIcon = new Image("Assets/Images/Icons/Accordion/pos_backoffice_sistema.png") });
                //}
                _logger.Debug("GetAccordionDefinition End");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return accordionDefinition;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Accordion Local Clicked EventHandlers

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

        private void ClickedSystemNotification(object sender, EventArgs e)
        {
            Hide();
            logicpos.Utils.ShowNotifications(this);
        }

        private void testClickedEventHandlerFromOutside(object sender, EventArgs e)
        {
            _logger.Debug(string.Format("testClickedEventHandlerFromOutside(): [{0}]", "id1.2"));
        }
    }
}
