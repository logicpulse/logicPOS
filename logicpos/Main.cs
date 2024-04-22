using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Logic.License;
using logicpos.datalayer.App;
using logicpos.plugin.contracts;
using logicpos.plugin.library;
using logicpos.shared.App;
using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace logicpos
{
    internal class MainApp
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       
        private static readonly bool _forceShowPluginLicenceWithDebugger = false;

        /* IN009203 - Mutex */
        private static readonly string _appGuid = "bfb677c2-a44a-46f8-93ab-d2d6a54e0b53";

        private static readonly SingleProgramInstance _singleProgramInstance = new SingleProgramInstance(_appGuid);

        private static Thread loadingThread;

        public static Dialog DialogLoading;

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                // Show current Configuration File
                _logger.Debug(string.Format("Use configuration file: [{0}]", AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));

                /* IN009203 - Mutex block */
                using (_singleProgramInstance)
                {
                    // Init Settings Main Config Settings
                    DataLayerFramework.Settings = ConfigurationManager.AppSettings;

                    // BootStrap Paths
                    InitPaths();

                    // Required before LicenseRouter
                    Application.Init();

                    //Render GTK Theme : In Start to Style UI in BootStrap Dialogs Error
                    Theme.ParseTheme(true, false);

                    /* Show "loading" */
                    _logger.Debug("void StartApp() :: Show 'loading'");
                    DialogLoading = Utils.GetThreadDialog(new Window("POS start loading"), true);
                    loadingThread = new Thread(() => DialogLoading.Run());
                    loadingThread.Start();

                    //Start Loading plugins / resources
                    FirstSteps();

                    // Flush pending events to keep the GUI reponsive
                    while (Application.EventsPending())
                        Application.RunIteration();

                    /* IN009203 - prevent lauching multiple instances of application */
                    if (_singleProgramInstance.IsSingleInstance)
                    {/* No app instance is running, start it */
                        /* IN009034 */
                        StartApp();
                    }
                    else
                    {/* App already running, show info to user and ends the main flow. */

                        {/* Forcing to load Custom Culture when showing proper message to user for Mutex implementation  */

                            /* IN006018 and IN007009 */
                            //logicpos.shared.App.CustomRegion.RegisterCustomRegion();
                            //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(DataLayerFramework.Settings["customCultureResourceDefinition"]);
                        }
                        Utils.ShowMessageNonTouch(null, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "dialog_message_pos_instance_already_running"), resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_information"));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private static void FirstSteps()
        {
            try
            {
                //IN009296 BackOffice - Mudar o idioma da aplicação
                SetCulture();

                // Init PluginContainer
                SharedFramework.PluginContainer = new PluginContainer(DataLayerFramework.Path["plugins"].ToString());

                // PluginSoftwareVendor
                SharedFramework.PluginSoftwareVendor = (SharedFramework.PluginContainer.GetFirstPluginOfType<ISoftwareVendor>());
                if (SharedFramework.PluginSoftwareVendor != null)
                {
                    // Show Loaded Plugin
                    _logger.Debug(string.Format("Registered plugin: [{0}] Name : [{1}]", typeof(ISoftwareVendor), SharedFramework.PluginSoftwareVendor.Name));
                    // Init Plugin
                    SharedSettings.InitSoftwareVendorPluginSettings();
                    // Check if all Resources are Embedded
                    SharedFramework.PluginSoftwareVendor.ValidateEmbeddedResources();
                }
                else
                {
                    // Show Loaded Plugin
                    _logger.Error(string.Format("Error missing required plugin type Installed: [{0}]", typeof(ISoftwareVendor)));
                }




                // Init Stock Module
                POSFramework.StockManagementModule = (SharedFramework.PluginContainer.GetFirstPluginOfType<IStockManagementModule>());

                // Try to Get LicenceManager IntellilockPlugin if in Release 
                if (!Debugger.IsAttached || _forceShowPluginLicenceWithDebugger)
                {
                   SharedFramework.PluginLicenceManager = (SharedFramework.PluginContainer.GetFirstPluginOfType<ILicenceManager>());
                    // Show Loaded Plugin
                    if (SharedFramework.PluginLicenceManager != null)
                    {
                        _logger.Debug(string.Format("Registered plugin: [{0}] Name : [{1}]", typeof(ILicenceManager),SharedFramework.PluginLicenceManager.Name));
                    }
                }

                loadingThread.Abort();

                DialogLoading.Destroy();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                loadingThread.Abort();
                DialogLoading.Destroy();
            }

        }

        /// <summary>
        /// Start application.
        /// Please see IN009005 and IN009034 for details.
        /// </summary>
        private static void StartApp()
        {


            if (SharedFramework.PluginLicenceManager != null && (!Debugger.IsAttached || _forceShowPluginLicenceWithDebugger))
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

        /// <summary>
        /// Start application in FrontOffice mode.
		/// See IN009034 for further details.
        /// </summary>
        private static void StartFrontOffice()
        {
            _logger.Debug("void StartFrontOffice() :: StartApp: AppMode.FrontOffice");

            LogicPos logicPos = new LogicPos();
            logicPos.StartApp(AppMode.FrontOffice);
        }

        private static void InitPaths()
        {
            // Init Paths
            DataLayerFramework.Path = new Hashtable
            {
                { "assets", SharedUtils.OSSlash(DataLayerFramework.Settings["pathAssets"]) },
                { "images", SharedUtils.OSSlash(DataLayerFramework.Settings["pathImages"]) },
                { "keyboards", SharedUtils.OSSlash(DataLayerFramework.Settings["pathKeyboards"]) },
                { "themes", SharedUtils.OSSlash(DataLayerFramework.Settings["pathThemes"]) },
                { "sounds", SharedUtils.OSSlash(DataLayerFramework.Settings["pathSounds"]) },
                { "resources", SharedUtils.OSSlash(DataLayerFramework.Settings["pathResources"]) },
                { "reports", SharedUtils.OSSlash(DataLayerFramework.Settings["pathReports"]) },
                { "temp", SharedUtils.OSSlash(DataLayerFramework.Settings["pathTemp"]) },
                { "cache", SharedUtils.OSSlash(DataLayerFramework.Settings["pathCache"]) },
                { "plugins", SharedUtils.OSSlash(DataLayerFramework.Settings["pathPlugins"]) },
                { "documents", SharedUtils.OSSlash(DataLayerFramework.Settings["pathDocuments"]) },
                { "certificates", SharedUtils.OSSlash(DataLayerFramework.Settings["pathCertificates"]) }
            };
            //Create Directories
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(Convert.ToString(DataLayerFramework.Path["temp"])));
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(Convert.ToString(DataLayerFramework.Path["cache"])));
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(Convert.ToString(DataLayerFramework.Path["documents"])));
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(string.Format(@"{0}Database\Other", Convert.ToString(DataLayerFramework.Path["resources"]))));
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(string.Format(@"{0}Database\{1}\Other", Convert.ToString(DataLayerFramework.Path["resources"]), DataLayerFramework.Settings["databaseType"], @"Database\MSSqlServer")));
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(string.Format(@"{0}Database\{1}\Other", Convert.ToString(DataLayerFramework.Path["resources"]), DataLayerFramework.Settings["databaseType"], @"Database\SQLite")));
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(string.Format(@"{0}Database\{1}\Other", Convert.ToString(DataLayerFramework.Path["resources"]), DataLayerFramework.Settings["databaseType"], @"Database\MySql")));
        }

        public static void InitPathsPrefs()
        {
            try
            {
                // PreferencesValues
                // Require to add end Slash, Prefs DirChooser dont add extra Slash in the End
                DataLayerFramework.Path.Add("backups", SharedUtils.OSSlash(SharedFramework.PreferenceParameters["PATH_BACKUPS"] + '/'));
                DataLayerFramework.Path.Add("saftpt", SharedUtils.OSSlash(SharedFramework.PreferenceParameters["PATH_SAFTPT"] + '/'));
                //Create Directories
                SharedUtils.CreateDirectory(SharedUtils.OSSlash(Convert.ToString(DataLayerFramework.Path["backups"])));
                SharedUtils.CreateDirectory(SharedUtils.OSSlash(Convert.ToString(DataLayerFramework.Path["saftpt"])));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //IN009296 BackOffice - Mudar o idioma da aplicação
        public static void SetCulture()
        {
            try
            {
                if (!Utils.IsLinux)
                {
                    string sql = "SELECT value FROM cfg_configurationpreferenceparameter where token = 'CULTURE';";
                    DataLayerFramework.SessionXpo = Utils.SessionXPO();
                    string getCultureFromDB = DataLayerFramework.SessionXpo.ExecuteScalar(sql).ToString();
                    if (!Utils.getCultureFromOS(getCultureFromDB))
                    {
                        SharedFramework.CurrentCulture = new CultureInfo("pt-PT");
                        DataLayerFramework.Settings["customCultureResourceDefinition"] = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
                    }
                    else
                    {
                        DataLayerFramework.Settings["customCultureResourceDefinition"] = getCultureFromDB;
                        SharedFramework.CurrentCulture = new System.Globalization.CultureInfo(getCultureFromDB);
                    }
                }
            }
            catch
            {

                if (!Utils.getCultureFromOS(ConfigurationManager.AppSettings["customCultureResourceDefinition"]))
                {
                    SharedFramework.CurrentCulture = new CultureInfo("pt-PT");
                    DataLayerFramework.Settings["customCultureResourceDefinition"] = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
                }
                else
                {
                    SharedFramework.CurrentCulture = new CultureInfo(DataLayerFramework.Settings["customCultureResourceDefinition"]);
                    DataLayerFramework.Settings["customCultureResourceDefinition"] = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
                }

                _logger.Error(string.Format("Missing Culture in DataBase or DB not created yet, using {0} from config.", DataLayerFramework.Settings["customCultureResourceDefinition"]));
            }
        }
    }
}