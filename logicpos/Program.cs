using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Logic.License;
using logicpos.Classes.Utils;
using logicpos.datalayer.Xpo;
using logicpos.plugin.contracts;
using logicpos.plugin.library;
using logicpos.shared.App;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using LogicPOS.Settings;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace logicpos
{
    internal class Program
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly bool _forceShowPluginLicenceWithDebugger = false;

        private static readonly SingleProgramInstance _singleProgramInstance = new SingleProgramInstance();

        private static Thread _loadingThread;

        public static Dialog DialogLoading { get; set; }

        public static string ConfigurationFile => AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

        public static void InitializeSettings()
        {
            GeneralSettings.Settings = ConfigurationManager.AppSettings;
        }

        public static void InitializeGtk()
        {
            Application.Init();
            Theme.ParseTheme(true, false);
        }

        public static void ShowLoadingScreen()
        {
            _logger.Debug("void StartApp() :: Show 'loading'");
            DialogLoading = Utils.GetThreadDialog(new Window("POS start loading"), true);
            _loadingThread = new Thread(() => DialogLoading.Run());
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
            try
            {
                _logger.Debug($"Use configuration file: [{ConfigurationFile}]");

                using (_singleProgramInstance)
                {
                    InitializeSettings();

                    Paths.InitializePaths();

                    InitializeGtk();

                    ShowLoadingScreen();

                    InitializePluguins();

                    KeepUIResponsive();

    
                    if (_singleProgramInstance.IsSingleInstance)
                    {
                        StartApp();
                    }
                    else
                    {
                        Utils.ShowMessageNonTouch(null, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "dialog_message_pos_instance_already_running"), CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_information"));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private static void InitializePluguins()
        {
            try
            {
                //IN009296 BackOffice - Mudar o idioma da aplica��o
                SetCulture();

                // Init PluginContainer
                PluginSettings.PluginContainer = new PluginContainer(GeneralSettings.Paths["plugins"].ToString());

                // PluginSoftwareVendor
                PluginSettings.PluginSoftwareVendor = PluginSettings.PluginContainer.GetFirstPluginOfType<ISoftwareVendor>();
                if (PluginSettings.HasPlugin)
                {
                    // Show Loaded Plugin
                    _logger.Debug(string.Format("Registered plugin: [{0}] Name : [{1}]", typeof(ISoftwareVendor), PluginSettings.PluginSoftwareVendor.Name));
                    // Init Plugin
                    PluginSettings.InitSoftwareVendorPluginSettings();
                    // Check if all Resources are Embedded
                    PluginSettings.PluginSoftwareVendor.ValidateEmbeddedResources();
                }
                else
                {
                    // Show Loaded Plugin
                    _logger.Error($"Error missing required plugin type Installed: [{typeof(ISoftwareVendor)}]");
                }

                // Init Stock Module
                POSFramework.StockManagementModule = (PluginSettings.PluginContainer.GetFirstPluginOfType<IStockManagementModule>());

                // Try to Get LicenceManager IntellilockPlugin if in Release 
                if (!Debugger.IsAttached || _forceShowPluginLicenceWithDebugger)
                {
                    PluginSettings.PluginLicenceManager = (PluginSettings.PluginContainer.GetFirstPluginOfType<ILicenceManager>());
                    // Show Loaded Plugin
                    if (PluginSettings.PluginLicenceManager != null)
                    {
                        _logger.Debug(string.Format("Registered plugin: [{0}] Name : [{1}]", typeof(ILicenceManager), PluginSettings.PluginLicenceManager.Name));
                    }
                }

                _loadingThread.Abort();

                DialogLoading.Destroy();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                _loadingThread.Abort();
                DialogLoading.Destroy();
            }

        }

        private static void StartApp()
        {
            if (PluginSettings.PluginLicenceManager != null && (!Debugger.IsAttached || _forceShowPluginLicenceWithDebugger))
            {
                _logger.Debug("void StartApp() :: Boot LogicPos after LicenceManager.IntellilockPlugin");
                // Boot LogicPos after LicenceManager.IntellilockPlugin
                LicenseRouter licenseRouter = new LicenseRouter();
            }
            else
            {
                bool dbExists = Utils.checkIfDbExists();
                // Boot LogicPos without pass in LicenseRouter
                _logger.Debug("void StartApp() :: Boot LogicPos without pass in LicenseRouter");
                /* IN009005: creating a new thread for app start up */
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(StartFrontOffice));
                GlobalApp.DialogThreadNotify = new ThreadNotify(new ReadyEvent(Utils.ThreadDialogReadyEvent));
                thread.Start();

                /* Show "loading" */
                _logger.Debug("void StartApp() :: Show 'loading'");
                GlobalApp.DialogThreadWork = Utils.GetThreadDialog(new Window("POS start up"), dbExists);
                GlobalApp.DialogThreadWork.Run();
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
            XPOSettings.Session = Utils.SessionXPO();
            return XPOSettings.Session.ExecuteScalar(sql).ToString();
        }

        //IN009296 BackOffice - Mudar o idioma da aplica��o
        public static void SetCulture()
        {
            try
            {
                string cultureFromDb = GetCultureFromDb();

                if (Utils.OSHasCulture(cultureFromDb) == false)
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

                if (!Utils.OSHasCulture(ConfigurationManager.AppSettings["customCultureResourceDefinition"]))
                {
                    CultureSettings.CurrentCulture = new CultureInfo("pt-PT");
                    GeneralSettings.Settings["customCultureResourceDefinition"] = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
                }
                else
                {
                    CultureSettings.CurrentCulture = new CultureInfo(GeneralSettings.Settings.GetCultureName());
                    GeneralSettings.Settings["customCultureResourceDefinition"] = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
                }

                _logger.Error(string.Format("Missing Culture in DataBase or DB not created yet, using {0} from config.", GeneralSettings.Settings.GetCultureName()));
            }
        }
    }
}