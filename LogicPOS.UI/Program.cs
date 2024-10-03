using Gtk;
using logicpos;
using logicpos.App;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Logic.License;
using LogicPOS.Api.Features.Countries.AddCountry;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Modules;
using LogicPOS.Modules.StockManagement;
using LogicPOS.Persistence.Services;
using LogicPOS.Plugin.Abstractions;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using System;
using System.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Threading;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace LogicPOS.UI
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
                PathsSettings.InitializePaths();

                InitializeGtk();

                if (true)
                {

                    ShowLoadingScreen();

                    SetCulture();

                    InitializePlugins();

                    CloseLoadingScreen();

                    KeepUIResponsive();

                    if (singleProgramInstance.IsSingleInstance == false)
                    {
                        SimpleAlerts.ShowInstanceAlreadyRunningAlert();
                        return;
                    }

                    StartApp();
                }
            }
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
            PluginSettings.LicenceManager = PluginSettings.PluginContainer.GetFirstPluginOfType<ILicenseManager>();
        }

        private static void InitializeStockModule()
        {
            ModulesSettings.StockManagementModule = PluginSettings.PluginContainer.GetFirstPluginOfType<IStockManagementModule>();
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
            if (PluginSettings.LicenceManager != null)
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
            try
            {
                string sql = "SELECT value FROM cfg_configurationpreferenceparameter where token = 'CULTURE';";
                XPOSettings.Session = DatabaseService.CreateDatabaseSession();
                var result = XPOSettings.Session.ExecuteScalar(sql);

                if (result != null)
                {
                    return result.ToString();
                }

                return null;

            } catch (Exception ex)
            {
                _logger.Error("GetCultureFromDb() :: " + ex.Message);
                return null;
            }     
        }

        public static void SetCulture()
        {
            string cultureFromDb = GetCultureFromDb();

            if (cultureFromDb == null || CultureSettings.OSHasCulture(cultureFromDb) == false)
            {
                CultureSettings.CurrentCulture = new CultureInfo("pt-PT");
                AppSettings.Instance.customCultureResourceDefinition = "pt-PT";
            }
            else
            {
                AppSettings.Instance.customCultureResourceDefinition = cultureFromDb;
                CultureSettings.CurrentCulture = new CultureInfo(cultureFromDb);
            }
        }


#if (DEBUG)
        public static readonly bool DebugMode = true;
#else
        public static readonly bool DebugMode = false;
#endif

    }
}