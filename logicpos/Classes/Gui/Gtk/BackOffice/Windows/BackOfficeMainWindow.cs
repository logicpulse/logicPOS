using DevExpress.Data.Filtering;
using Gtk;
using logicpos.App;
using logicpos.Classes.DataLayer;
using logicpos.Classes.Enums;
using logicpos.Classes.Enums.Finance;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;

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
        string _privilegesBackOfficeMenuOperation = string.Format("{0}_{1}", SettingsApp.PrivilegesBackOfficeCRUDOperationPrefix, "MENU");

        public BackOfficeMainWindow()
        {
            //Info
            _log.Info("BackOfficeMainWindow(): Create object BackOfficeMainWindow");

            this.WindowStateEvent += PosMainWindow_WindowStateEvent;

            //Disable not we dont have a title bar
            // Title = Utils.GetWindowTitle(Resx.window_title_backoffice);
            InitUI();
            ShowAll();

            //First Time Assigmnent
            _activeUserDetail = GlobalFramework.LoggedUser;

            //Events
            this.Shown += BackOfficeMainWindow_Show;
        }

        void BackOfficeMainWindow_Show(object sender, EventArgs e)
        {
            if (_activeUserDetail != GlobalFramework.LoggedUser)
            {
                _activeUserDetail = GlobalFramework.LoggedUser;
                _labelTerminalInfo.Text = string.Format("{0} : {1}", GlobalFramework.LoggedTerminal.Designation, GlobalFramework.LoggedUser.Name);

                //Apply Menu Updates
                Accordion.UpdateMenuPrivileges();

                //Hide/Show Current Active Content based on user privileges
                string currentNodePrivilegesToken = string.Format(_privilegesBackOfficeMenuOperation, Accordion.CurrentChildButtonContent.Name.ToUpper());
                _nodeContent.Sensitive = FrameworkUtils.HasPermissionTo(currentNodePrivilegesToken);
            }
        }

        private void PosMainWindow_WindowStateEvent(object o, WindowStateEventArgs args)
        {
        }

        private void InitUI()
        {
            Accordion = new Accordion(GetAccordionDefinition(), SettingsApp.PrivilegesBackOfficeMenuOperationFormat) { WidthRequest = _widthAccordion };
            _fixAccordion.Add(Accordion);
            Accordion.Clicked += accordion_Clicked;
        }

        private Dictionary<string, AccordionNode> GetAccordionDefinition()
        {
            _log.Debug("GetAccordionDefinition Begin");

            //Init accordionDefinition
            Dictionary<string, AccordionNode> accordionDefinition = null;
            try
            {
                accordionDefinition = new Dictionary<string, AccordionNode>();

                //Define Start Content with Articles TreeView
                Widget startContent = Utils.GetGenericTreeViewXPO<TreeViewArticle>(this);

                //Hide/Show Current Active Content based on user privileges
                string currentNodePrivilegesToken = string.Format(_privilegesBackOfficeMenuOperation, "Article".ToUpper());
                startContent.Sensitive = FrameworkUtils.HasPermissionTo(currentNodePrivilegesToken);

                _labelActiveContent.Text = Resx.global_articles;
                _nodeContent = startContent;

                _hboxContent.PackEnd(_nodeContent);

                //Define used CriteriaOperators/Override Defaults from TreeViews
                CriteriaOperator criteriaOperatorCustomer = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1) AND (Hidden IS NULL OR Hidden = 0)");
                //CriteriaOperator criteriaConfigurationPreferenceParameter = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1) AND (Token <> 'COMPANY_FISCALNUMBER')");

                //Articles
                Dictionary<string, AccordionNode> _accordionChildArticles = new Dictionary<string, AccordionNode>();
                _accordionChildArticles.Add("ArticleFamily", new AccordionNode(Resx.global_families) { Content = Utils.GetGenericTreeViewXPO<TreeViewArticleFamily>(this), Clicked = testClickedEventHandlerFromOutside });
                _accordionChildArticles.Add("ArticleSubFamily", new AccordionNode(Resx.global_subfamilies) { Content = Utils.GetGenericTreeViewXPO<TreeViewArticleSubFamily>(this) });
                _accordionChildArticles.Add("Article", new AccordionNode(Resx.global_articles) { Content = startContent });
                _accordionChildArticles.Add("ArticleType", new AccordionNode(Resx.global_article_types) { Content = Utils.GetGenericTreeViewXPO<TreeViewArticleType>(this) });
                _accordionChildArticles.Add("ArticleClass", new AccordionNode(Resx.global_article_class) { Content = Utils.GetGenericTreeViewXPO<TreeViewArticleClass>(this) });
                _accordionChildArticles.Add("ConfigurationPriceType", new AccordionNode(Resx.global_price_type) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPriceType>(this) });
                // Disable to Speed uo Opening BO, noew we have Stock Reports
                //_accordionChildArticles.Add("ArticleStock", new AccordionNode(Resx.global_stock_movements) { Content = Utils.GetGenericTreeViewXPO<TreeViewArticleStock>(this) });

                //Customers
                Dictionary<string, AccordionNode> _accordionChildCustomers = new Dictionary<string, AccordionNode>();
                _accordionChildCustomers.Add("Customer", new AccordionNode(Resx.global_customers) { Content = Utils.GetGenericTreeViewXPO<TreeViewCustomer>(this, criteriaOperatorCustomer) });
                _accordionChildCustomers.Add("CustomerType", new AccordionNode(Resx.global_customer_types) { Content = Utils.GetGenericTreeViewXPO<TreeViewCustomerType>(this) });
                _accordionChildCustomers.Add("CustomerDiscountGroup", new AccordionNode(Resx.global_customer_discount_groups) { Content = Utils.GetGenericTreeViewXPO<TreeViewCustomerDiscountGroup>(this) });

                //Users
                Dictionary<string, AccordionNode> _accordionChildUsers = new Dictionary<string, AccordionNode>();
                _accordionChildUsers.Add("UserDetail", new AccordionNode(Resx.global_users) { Content = Utils.GetGenericTreeViewXPO<TreeViewUser>(this) });
                //Commented by Mario: Not Usefull, UserPermissionProfile has same funtionality
                //_accordionChildUsers.Add("UserProfile", new AccordionNode(Resx.global_profile) { Content = Utils.GetGenericTreeViewXPO<TreeViewUserProfile>(this) });
                //WARNING: Works with diferent constructs, its still need to be improved : new TreeViewUserProfilePermissions(this)
                _accordionChildUsers.Add("UserPermissionProfile", new AccordionNode(Resx.global_user_permissions) { Content = new TreeViewUserProfilePermissions(this) });
                _accordionChildUsers.Add("UserCommissionGroup", new AccordionNode(Resx.global_user_commission_groups) { Content = Utils.GetGenericTreeViewXPO<TreeViewUserCommissionGroup>(this) });
                //Moved to Custom Toolbar
                //_accordionChildUsers.Add("System_ApplyPrivileges", new AccordionNode(Resx.global_user_apply_privileges) { Clicked = delegate { Accordion.UpdateMenuPrivileges(); } });

                //Tables
                Dictionary<string, AccordionNode> _accordionDocuments = new Dictionary<string, AccordionNode>();
                _accordionDocuments.Add("DocumentFinanceYears", new AccordionNode(Resx.global_documentfinance_years) { Content = Utils.GetGenericTreeViewXPO<TreeViewDocumentFinanceYears>(this) });
                _accordionDocuments.Add("DocumentFinanceSeries", new AccordionNode(Resx.global_documentfinance_series) { Content = Utils.GetGenericTreeViewXPO<TreeViewDocumentFinanceSeries>(this) });
                _accordionDocuments.Add("DocumentFinanceType", new AccordionNode(Resx.global_documentfinance_type) { Content = Utils.GetGenericTreeViewXPO<TreeViewDocumentFinanceType>(this) });
                //_accordionDocuments.Add("DocumentFinanceYearSerieTerminal", new AccordionNode(Resx.global_documentfinance_yearsseriesterminal) { Content = Utils.GetGenericTreeViewXPO<TreeViewDocumentFinanceYearSerieTerminal>(this) });
                _accordionDocuments.Add("ConfigurationVatRate", new AccordionNode(Resx.global_vat_rates) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationVatRate>(this) });
                _accordionDocuments.Add("ConfigurationVatExemptionReason", new AccordionNode(Resx.global_vat_exemption_reason) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationVatExceptionReason>(this) });
                _accordionDocuments.Add("ConfigurationPaymentCondition", new AccordionNode(Resx.global_payment_conditions) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPaymentCondition>(this) });
                _accordionDocuments.Add("ConfigurationPaymentMethod", new AccordionNode(Resx.global_payment_methods) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPaymentMethod>(this) });

                //Printers
                Dictionary<string, AccordionNode> _accordionPrinters = new Dictionary<string, AccordionNode>();
                _accordionPrinters.Add("ConfigurationPrinters", new AccordionNode(Resx.global_ConfigurationPrinters) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPrinters>(this) });
                _accordionPrinters.Add("ConfigurationPrintersType", new AccordionNode(Resx.global_ConfigurationPrintersType) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPrintersType>(this) });
                //_accordionPrinters.Add("ConfigurationPrintersTemplates", new AccordionNode(Resx.global_ConfigurationPrintersTemplates) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPrintersTemplates>(this) });
                //_accordionPrinters.Add("ExternalApp_Composer", new AccordionNode(Resx.global_callposcomposer) { Content = null, ExternalAppFileName = SettingsApp.ExecutableComposer });

                //Configuration
                Dictionary<string, AccordionNode> _accordionChildConfiguration = new Dictionary<string, AccordionNode>();
                //_accordionChildConfiguration.Add("ConfigurationCashRegister", new AccordionNode(Resx.global_cash_registers) { Content = Utils.GetGenericTreeView<TreeViewConfigurationCashRegister>(this) });
                _accordionChildConfiguration.Add("ConfigurationCountry", new AccordionNode(Resx.global_country) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationCountry>(this) });
                _accordionChildConfiguration.Add("ConfigurationCurrency", new AccordionNode(Resx.global_ConfigurationCurrency) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationCurrency>(this) });
                //_accordionChildConfiguration.Add("ConfigurationDevice", new AccordionNode(Resx.global_devices) { Content = Utils.GetGenericTreeView<TreeViewConfigurationDevice>(this) });
                //_accordionChildConfiguration.Add("ConfigurationKeyboard", new AccordionNode(Resx.global_keyboards) { Content = Utils.GetGenericTreeView<TreeViewConfigurationKeyboard>(this) });
                //_accordionChildConfiguration.Add("ConfigurationMaintenance", new AccordionNode(Resx.global_maintenance) { Content = Utils.GetGenericTreeView<TreeViewConfigurationMaintenance>(this) });
                _accordionChildConfiguration.Add("ConfigurationPlace", new AccordionNode(Resx.global_places) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPlace>(this) });
                _accordionChildConfiguration.Add("ConfigurationPlaceTable", new AccordionNode(Resx.global_place_tables) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPlaceTable>(this) });
                _accordionChildConfiguration.Add("ConfigurationPlaceMovementType", new AccordionNode(Resx.global_places_movement_type) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPlaceMovementType>(this) });
                _accordionChildConfiguration.Add("ConfigurationPlaceTerminal", new AccordionNode(Resx.global_places_terminals) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPlaceTerminal>(this) });
                _accordionChildConfiguration.Add("ConfigurationUnitMeasure", new AccordionNode(Resx.global_units_measure) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationUnitMeasure>(this) });
                _accordionChildConfiguration.Add("ConfigurationUnitSize", new AccordionNode(Resx.global_units_size) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationUnitSize>(this) });
                _accordionChildConfiguration.Add("ConfigurationHolidays", new AccordionNode(Resx.global_holidays) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationHolidays>(this) });
                _accordionChildConfiguration.Add("ConfigurationPreferenceParameter", new AccordionNode(Resx.global_preferenceparameter) { Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPreferenceParameter>(this/*, criteriaConfigurationPreferenceParameter*/) });

                // Add Menu Items Based On Plugins PluginSoftwareVendor
                Dictionary<string, AccordionNode> _accordionChildExport = new Dictionary<string, AccordionNode>();
                //Export
                if (GlobalFramework.PluginSoftwareVendor != null && SettingsApp.ConfigurationSystemCountry.Oid == SettingsApp.XpoOidConfigurationCountryPortugal)
                {
                    _accordionChildExport.Add("System_ExportSaftPT_SaftPt", new AccordionNode(Resx.global_export_saftpt_whole_year) { Clicked = delegate { FrameworkCalls.ExportSaftPt(this, ExportSaftPtMode.WholeYear); } });
                    _accordionChildExport.Add("System_ExportSaftPT_E-Fatura", new AccordionNode(Resx.global_export_saftpt_last_month) { Clicked = delegate { FrameworkCalls.ExportSaftPt(this, ExportSaftPtMode.LastMonth); } });
                    _accordionChildExport.Add("System_ExportSaftPT_Custom", new AccordionNode(Resx.global_export_saftpt_custom) { Clicked = delegate { FrameworkCalls.ExportSaftPt(this, ExportSaftPtMode.Custom); } });
                }

                //System
                Dictionary<string, AccordionNode> _accordionChildSystem = new Dictionary<string, AccordionNode>();
                // Add Menu Items Based On Plugins PluginSoftwareVendor
                if (GlobalFramework.PluginSoftwareVendor != null)
                {
                    _accordionChildSystem.Add("System_DataBaseBackup", new AccordionNode(Resx.global_database_backup) { Clicked = delegate { DataBaseBackup.Backup(this); } });
                    _accordionChildSystem.Add("System_DataBaseRestore_FromSystem", new AccordionNode(Resx.global_database_restore) { Clicked = delegate { DataBaseBackup.Restore(this, DataBaseRestoreFrom.SystemBackup); } });
                    _accordionChildSystem.Add("System_DataBaseRestore_FromFile", new AccordionNode(Resx.global_database_restore_from_file) { Clicked = delegate { DataBaseBackup.Restore(this, DataBaseRestoreFrom.ChooseFromFilePickerDialog); } });
                }
                _accordionChildSystem.Add("System_Menu", new AccordionNode(Resx.global_application_logout_user) { Clicked = ClickedSystemLogout });
                _accordionChildSystem.Add("System_Pos", new AccordionNode(Resx.global_pos) { Clicked = ClickedSystemPos });
                _accordionChildSystem.Add("System_Quit", new AccordionNode(Resx.global_quit) { Clicked = delegate { LogicPos.Quit(this); } });

                //Compose Main Accordion Parent Buttons
                accordionDefinition.Add("TopMenuArticles", new AccordionNode(Resx.global_articles) { Childs = _accordionChildArticles, GroupIcon = new Image("Assets/Images/Icons/Accordion/poson_backoffice_artigos.png") });
                accordionDefinition.Add("TopMenuCustomers", new AccordionNode(Resx.global_customers) { Childs = _accordionChildCustomers, GroupIcon = new Image("Assets/Images/Icons/Accordion/poson_backoffice_clientes.png") });
                accordionDefinition.Add("TopMenuDocuments", new AccordionNode(Resx.global_documents) { Childs = _accordionDocuments, GroupIcon = new Image("Assets/Images/Icons/Accordion/poson_backoffice_informacao_fiscal.png") });
                accordionDefinition.Add("TopMenuPrinters", new AccordionNode(Resx.global_printers) { Childs = _accordionPrinters, GroupIcon = new Image("Assets/Images/Icons/Accordion/poson_backoffice_impressoras.png") });
                accordionDefinition.Add("TopMenuUsers", new AccordionNode(Resx.global_users) { Childs = _accordionChildUsers, GroupIcon = new Image("Assets/Images/Icons/Accordion/poson_backoffice_utilizadores.png") });
                accordionDefinition.Add("TopMenuConfiguration", new AccordionNode(Resx.global_configuration) { Childs = _accordionChildConfiguration, GroupIcon = new Image("Assets/Images/Icons/Accordion/poson_backoffice_configuracao.png") });
                if (_accordionChildExport.Count > 0)
                {
                    accordionDefinition.Add("TopMenuExport", new AccordionNode(Resx.global_export) { Childs = _accordionChildExport, GroupIcon = new Image("Assets/Images/Icons/Accordion/poson_backoffice_export.png") });
                }
                accordionDefinition.Add("TopMenuSystem", new AccordionNode(Resx.global_system) { Childs = _accordionChildSystem, GroupIcon = new Image("Assets/Images/Icons/Accordion/poson_backoffice_sistema.png") });

                _log.Debug("GetAccordionDefinition End");
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return accordionDefinition;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Accordion Local Clicked EventHandlers

        private void ClickedSystemLogout(object sender, EventArgs e)
        {
            Hide();
            //Call Shared WindowStartup LogOutUser, and Show WindowStartup
            GlobalApp.WindowStartup.LogOutUser(true);
        }

        private void ClickedSystemPos(object sender, EventArgs e)
        {
            Hide();
            //Show WindowStartup
            GlobalApp.WindowPos.ShowAll();
        }

        private void testClickedEventHandlerFromOutside(object sender, EventArgs e)
        {
            _log.Debug(string.Format("testClickedEventHandlerFromOutside(): [{0}]", "id1.2"));
        }
    }
}
