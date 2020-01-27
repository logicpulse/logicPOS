using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Logic.License;
using logicpos.plugin.contracts;
using logicpos.plugin.library;
using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using logicpos.resources.Resources.Localization;
using System.Globalization;
using System.Threading;
using logicpos.datalayer.Enums;
using System.IO;

//Log4Net
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace logicpos
{
    class MainApp
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // Use this to force Plugin with Debug Attach
        private static bool forceShowPluginLicenceWithDebugger = false;

        /* IN009203 - Mutex */
        private static string appGuid = "bfb677c2-a44a-46f8-93ab-d2d6a54e0b53";
        private static SingleProgramInstance _SingleProgramInstance = new SingleProgramInstance(appGuid);

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                // Show current Configuration File
                 _log.Debug(String.Format("Use configuration file: [{0}]", System.AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));

                /* IN009203 - Mutex block */
                using (_SingleProgramInstance)
                {
                    // Init Settings Main Config Settings
                    GlobalFramework.Settings = ConfigurationManager.AppSettings;
                    
                    //IN009296 BackOffice - Mudar o idioma da aplicação
                    SetCulture();

                    // BootStrap Paths
                    InitPaths();

                    // Init PluginContainer
                    GlobalFramework.PluginContainer = new PluginContainer(GlobalFramework.Path["plugins"].ToString());

                    // PluginSoftwareVendor
                    GlobalFramework.PluginSoftwareVendor = (GlobalFramework.PluginContainer.GetFirstPluginOfType<ISoftwareVendor>());
                    if (GlobalFramework.PluginSoftwareVendor != null)
                    {
                        // Show Loaded Plugin
                        _log.Debug(String.Format("Registered plugin: [{0}] Name : [{1}]", typeof(ISoftwareVendor), GlobalFramework.PluginSoftwareVendor.Name));
                        // Init Plugin
                        SettingsApp.InitSoftwareVendorPluginSettings();
                        // Check if all Resources are Embedded
                        GlobalFramework.PluginSoftwareVendor.ValidateEmbeddedResources();
                    }
                    else
                    {
                        // Show Loaded Plugin
                        _log.Error(String.Format("Error missing required plugin type Installed: [{0}]", typeof(ISoftwareVendor)));
                    }

                    // Try to Get LicenceManager IntellilockPlugin if in Release 
                    if (!Debugger.IsAttached || forceShowPluginLicenceWithDebugger)
                    {
                        GlobalFramework.PluginLicenceManager = (GlobalFramework.PluginContainer.GetFirstPluginOfType<ILicenceManager>());
                        // Show Loaded Plugin
                        if (GlobalFramework.PluginLicenceManager != null)
                        {
                            _log.Debug(String.Format("Registered plugin: [{0}] Name : [{1}]", typeof(ILicenceManager), GlobalFramework.PluginLicenceManager.Name));
                        }
                    }

                    // Required before LicenseRouter
                    Application.Init();

                    //Render GTK Theme : In Start to Style UI in BootStrap Dialogs Error
                    Theme.ParseTheme(true, false);

                    /* IN009203 - prevent lauching multiple instances of application */
                    if (_SingleProgramInstance.IsSingleInstance)
                    {/* No app instance is running, start it */
                        /* IN009034 */
                        StartApp();
                    }
                    else
                    {/* App already running, show info to user and ends the main flow. */

                        {/* Forcing to load Custom Culture when showing proper message to user for Mutex implementation  */

                            /* IN006018 and IN007009 */
                            //logicpos.shared.App.CustomRegion.RegisterCustomRegion();
                            //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(GlobalFramework.Settings["customCultureResourceDefinition"]);
                        }

                        Utils.ShowMessageNonTouch(null, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_pos_instance_already_running"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
        
        /// <summary>
        /// Start application.
		/// Please see IN009005 and IN009034 for details.
        /// </summary>
		private static void StartApp()
        {


            if (GlobalFramework.PluginLicenceManager != null && (!Debugger.IsAttached || forceShowPluginLicenceWithDebugger))
            {
                _log.Debug("void StartApp() :: Boot LogicPos after LicenceManager.IntellilockPlugin");
                // Boot LogicPos after LicenceManager.IntellilockPlugin
                LicenseRouter licenseRouter = new LicenseRouter();
            }
            else
            {
                    bool dbExists = Utils.checkIfDbExists();
                    // Boot LogicPos without pass in LicenseRouter
                    _log.Debug("void StartApp() :: Boot LogicPos without pass in LicenseRouter");
                    /* IN009005: creating a new thread for app start up */
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(StartFrontOffice));
                    GlobalApp.DialogThreadNotify = new ThreadNotify(new ReadyEvent(Utils.ThreadDialogReadyEvent));
                    thread.Start();

                    /* Show "loading" */
                    _log.Debug("void StartApp() :: Show 'loading'");
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
            _log.Debug("void StartFrontOffice() :: StartApp: AppMode.FrontOffice");

            LogicPos logicPos = new LogicPos();
            logicPos.StartApp(AppMode.FrontOffice);
        }

        private static void InitPaths()
        {
            // Init Paths
            GlobalFramework.Path = new Hashtable();
            GlobalFramework.Path.Add("assets", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathAssets"]));
            GlobalFramework.Path.Add("images", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathImages"]));
            GlobalFramework.Path.Add("keyboards", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathKeyboards"]));
            GlobalFramework.Path.Add("themes", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathThemes"]));
            GlobalFramework.Path.Add("sounds", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathSounds"]));
            GlobalFramework.Path.Add("resources", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathResources"]));
            GlobalFramework.Path.Add("reports", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathReports"]));
            GlobalFramework.Path.Add("temp", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathTemp"]));
            GlobalFramework.Path.Add("cache", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathCache"]));
            GlobalFramework.Path.Add("plugins", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathPlugins"]));
            GlobalFramework.Path.Add("documents", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathDocuments"]));
            //Create Directories
            FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(Convert.ToString(GlobalFramework.Path["temp"])));
            FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(Convert.ToString(GlobalFramework.Path["cache"])));
            FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(Convert.ToString(GlobalFramework.Path["documents"])));
            FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(string.Format(@"{0}Database\Other", Convert.ToString(GlobalFramework.Path["resources"]))));
            FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(string.Format(@"{0}Database\{1}\Other", Convert.ToString(GlobalFramework.Path["resources"]), GlobalFramework.Settings["databaseType"], @"Database\MSSqlServer")));
            FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(string.Format(@"{0}Database\{1}\Other", Convert.ToString(GlobalFramework.Path["resources"]), GlobalFramework.Settings["databaseType"], @"Database\SQLite")));
            FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(string.Format(@"{0}Database\{1}\Other", Convert.ToString(GlobalFramework.Path["resources"]), GlobalFramework.Settings["databaseType"], @"Database\MySql")));
        }

        public static void InitPathsPrefs()
        {
            try
            {
                // PreferencesValues
                // Require to add end Slash, Prefs DirChooser dont add extra Slash in the End
                GlobalFramework.Path.Add("backups", FrameworkUtils.OSSlash(GlobalFramework.PreferenceParameters["PATH_BACKUPS"] + '/'));
                GlobalFramework.Path.Add("saftpt", FrameworkUtils.OSSlash(GlobalFramework.PreferenceParameters["PATH_SAFTPT"] + '/'));
                //Create Directories
                FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(Convert.ToString(GlobalFramework.Path["backups"])));
                FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(Convert.ToString(GlobalFramework.Path["saftpt"])));
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
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
                    GlobalFramework.SessionXpo = Utils.SessionXPO();
                    string getCultureFromDB = GlobalFramework.SessionXpo.ExecuteScalar(sql).ToString();                    
                    if (!Utils.getCultureFromOS(getCultureFromDB))
                    {
                        GlobalFramework.CurrentCulture = new CultureInfo("pt-PT");
                        GlobalFramework.Settings["customCultureResourceDefinition"] = ConfigurationManager.AppSettings["customCultureResourceDefinition"]; 
                    }
                    else
                    {
                        GlobalFramework.Settings["customCultureResourceDefinition"] = getCultureFromDB;
                        GlobalFramework.CurrentCulture = new System.Globalization.CultureInfo(getCultureFromDB);
                    }
                        
                }
            }
            catch
            {

                if (!Utils.getCultureFromOS(ConfigurationManager.AppSettings["customCultureResourceDefinition"]))
                {
                    GlobalFramework.CurrentCulture = new CultureInfo("pt-PT");
                    GlobalFramework.Settings["customCultureResourceDefinition"] = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
                }
                else
                {
                    GlobalFramework.CurrentCulture = new CultureInfo(GlobalFramework.Settings["customCultureResourceDefinition"]);
                    GlobalFramework.Settings["customCultureResourceDefinition"] = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
                }

                _log.Error(String.Format("Missing Culture in DataBase or DB not created yet, using {0} from config.", GlobalFramework.Settings["customCultureResourceDefinition"]));
            }            
        }
    }
}