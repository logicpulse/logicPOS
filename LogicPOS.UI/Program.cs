using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Logic.License;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Modules;
using LogicPOS.Modules.StockManagement;
using LogicPOS.Persistence.Services;
using LogicPOS.Plugin.Abstractions;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Configuration;
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

                CloseLoadingScreen();

                KeepUIResponsive();

                if (singleProgramInstance.IsSingleInstance == false)
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
            InitializeSoftwareVendorPlugin();
            InitializeStockModule();
            InitializeLicenseManagerPlugin();
        }

        private static void CloseLoadingScreen()
        {
            _loadingThread.Abort();
            SplashScreen.Destroy();
        }

        private static void InitializeLicenseManagerPlugin()
        {
            PluginSettings.LicenceManager = (PluginSettings.PluginContainer.GetFirstPluginOfType<ILicenseManager>());
        }

        private static void InitializeStockModule()
        {
            ModulesSettings.StockManagementModule = (PluginSettings.PluginContainer.GetFirstPluginOfType<IStockManagementModule>());
        }

        private static void InitializeSoftwareVendorPlugin()
        {
            PluginSettings.SoftwareVendor = PluginSettings.PluginContainer.GetFirstPluginOfType<ISoftwareVendor>();

            if (PluginSettings.HasSoftwareVendorPlugin)
            {
                PluginSettings.InitializeSoftwareVendorPluginSettings();
            }
        }

        private static void StartApp()
        {
            if (PluginSettings.LicenceManager != null && DebugMode == false)
            {
                LicenseRouter licenseRouter = new LicenseRouter();
            }
            else
            {
                bool dbExists = DatabaseService.DatabaseExists();
                Thread thread = new Thread(new ThreadStart(StartFrontOffice));
                GlobalApp.DialogThreadNotify = new ThreadNotify(new ReadyEvent(Utils.NotifyLoadingIsDone));
                thread.Start();

                _logger.Debug("void StartApp() :: Show 'loading'");
                GlobalApp.LoadingDialog = Utils.CreateSplashScreen(new Window("POS start up"), dbExists);
                GlobalApp.LoadingDialog.Run();
            }
        }

        public static void StartFrontOffice()
        {
            LogicPOSApp logicPos = new LogicPOSApp();
            logicPos.StartApp(AppMode.FrontOffice);
        }

        public static void StartBackOffice()
        {
            LogicPOSApp logicPos = new LogicPOSApp();
            logicPos.StartApp(AppMode.Backoffice);
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


#if (DEBUG)
        public static readonly bool DebugMode = true;
#else
        public static readonly bool DebugMode = false;
#endif

    }
}