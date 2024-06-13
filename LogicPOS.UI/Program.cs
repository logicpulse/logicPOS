using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Logic.License;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Globalization;
using LogicPOS.Modules;
using LogicPOS.Modules.StockManagement;
using LogicPOS.Persistence.Services;
using LogicPOS.Plugin.Abstractions;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace logicpos
{
    internal class Program
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Thread _loadingThread;
        public static Dialog SplashScreen { get; set; }

        public static void InitializeSettings()
        {
            GeneralSettings.Settings = ConfigurationManager.AppSettings;
        }

        public static void InitializeGtk()
        {
            Application.Init();
            Theme.ParseTheme();
        }

        public static void ShowLoadingScreen()
        {
            SplashScreen = Utils.CreateSplashScreen(
                new Window("POS start loading"),
                true);

            _loadingThread = new Thread(() => SplashScreen.Run());
            _loadingThread.Start();
        }

        private static void KeepUIResponsive()
        {
            while (Application.EventsPending())
            {
                Application.RunIteration();
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            using (var singleProgramInstance = new SingleProgramInstance())
            {
                InitializeSettings();

                PathsSettings.InitializePaths();

                InitializeGtk();

                ShowLoadingScreen();

                SetCulture();

                InitializePlugins();

                KeepUIResponsive();

                if (singleProgramInstance.IsSingleInstance)
                {
                    ShowInstanceAlreadyRunningMessage();
                    return;

                }

                StartApp();
            }
        }

        private static void ShowInstanceAlreadyRunningMessage()
        {
            Utils.ShowMessageNonTouch(
                                    null,
                                    DialogFlags.Modal,
                                    MessageType.Info,
                                    ButtonsType.Ok,
                                    GeneralUtils.GetResourceByName("dialog_message_pos_instance_already_running"),
                                    GeneralUtils.GetResourceByName("global_information"));
        }

        private static void InitializePlugins()
        {

            PluginSettings.InitializeContainer();

            PluginSettings.SoftwareVendor = PluginSettings.PluginContainer.GetFirstPluginOfType<ISoftwareVendor>();

            if (PluginSettings.HasSoftwareVendorPlugin)
            {
                PluginSettings.InitializeSoftwareVendorPluginSettings();
                //PluginSettings.SoftwareVendor.ValidateEmbeddedResources();
            }

            // Init Stock Module
            ModulesSettings.StockManagementModule = (PluginSettings.PluginContainer.GetFirstPluginOfType<IStockManagementModule>());

            // Try to Get LicenceManager IntellilockPlugin if in Release 
            if (!Debugger.IsAttached)
            {
                PluginSettings.LicenceManager = (PluginSettings.PluginContainer.GetFirstPluginOfType<ILicenseManager>());
                // Show Loaded Plugin
                if (PluginSettings.LicenceManager != null)
                {
                    _logger.Debug(string.Format("Registered plugin: [{0}] Name : [{1}]", typeof(ILicenseManager), PluginSettings.LicenceManager.Name));
                }
            }

            _loadingThread.Abort();

            SplashScreen.Destroy();
        }

        private static void StartApp()
        {
            if (PluginSettings.LicenceManager != null && Debugger.IsAttached == false)
            {
                _logger.Debug("void StartApp() :: Boot LogicPos after LicenceManager.IntellilockPlugin");
                // Boot LogicPos after LicenceManager.IntellilockPlugin
                LicenseRouter licenseRouter = new LicenseRouter();
            }
            else
            {
                bool dbExists = DatabaseService.DatabaseExists();
                // Boot LogicPos without pass in LicenseRouter
                _logger.Debug("void StartApp() :: Boot LogicPos without pass in LicenseRouter");
                /* IN009005: creating a new thread for app start up */
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(StartFrontOffice));
                GlobalApp.DialogThreadNotify = new ThreadNotify(new ReadyEvent(Utils.NotifyLoadingIsDone));
                thread.Start();

                /* Show "loading" */
                _logger.Debug("void StartApp() :: Show 'loading'");
                GlobalApp.LoadingDialog = Utils.CreateSplashScreen(new Window("POS start up"), dbExists);
                GlobalApp.LoadingDialog.Run();
                /* IN009005: end */


            }
        }

        private static void StartFrontOffice()
        {
            _logger.Debug("void StartFrontOffice() :: StartApp: AppMode.FrontOffice");

            LogicPOSApp logicPos = new LogicPOSApp();
            logicPos.StartApp(AppMode.FrontOffice);
        }

        private static string GetCultureFromDb()
        {
            string sql = "SELECT value FROM cfg_configurationpreferenceparameter where token = 'CULTURE';";
            XPOSettings.Session = DatabaseService.CreateDatabaseSession();
            return XPOSettings.Session.ExecuteScalar(sql).ToString();
        }


        public static void SetCulture()
        {
            try
            {
                string cultureFromDb = GetCultureFromDb();

                if (CultureSettings.OSHasCulture(cultureFromDb) == false)
                {
                    CultureSettings.CurrentCulture = new CultureInfo("pt-PT");
                    GeneralSettings.Settings["customCultureResourceDefinition"] = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
                }
                else
                {
                    GeneralSettings.Settings["customCultureResourceDefinition"] = cultureFromDb;
                    CultureSettings.CurrentCulture = new CultureInfo(cultureFromDb);
                }

            }
            catch
            {

                if (CultureSettings.OSHasCulture(ConfigurationManager.AppSettings["customCultureResourceDefinition"]) == false)
                {
                    CultureSettings.CurrentCulture = new CultureInfo("pt-PT");
                    GeneralSettings.Settings["customCultureResourceDefinition"] = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
                }
                else
                {
                    CultureSettings.CurrentCulture = new CultureInfo(CultureSettings.CurrentCultureName);
                    GeneralSettings.Settings["customCultureResourceDefinition"] = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
                }

                _logger.Error(string.Format("Missing Culture in DataBase or DB not created yet, using {0} from config.", CultureSettings.CurrentCultureName));
            }
        }
    }
}