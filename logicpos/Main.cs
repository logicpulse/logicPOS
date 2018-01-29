using Gtk;
using logicpos.App;
using logicpos.Classes.Logic.License;
using logicpos.plugin.contracts;
using logicpos.plugin.library;
using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;

//Log4Net
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace logicpos
{
    class MainApp
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                // Show current Configuration File
                _log.Info(String.Format("Use configuration file: [{0}]", System.AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));   

                // Init Settings Main Config Settings
                GlobalFramework.Settings = ConfigurationManager.AppSettings;

                // BootStrap Paths
                InitPaths();

                // Init PluginContainer
                GlobalFramework.PluginContainer = new PluginContainer(GlobalFramework.Path["plugins"].ToString());
                
                // PluginSoftwareVendor
                GlobalFramework.PluginSoftwareVendor = (GlobalFramework.PluginContainer.GetFirstPluginOfType<ISoftwareVendor>());
                if (GlobalFramework.PluginSoftwareVendor != null)
                {
                    // Show Loaded Plugin
                    _log.Info(String.Format("Registered plugin: [{0}] Name : [{1}]", typeof(ISoftwareVendor), GlobalFramework.PluginSoftwareVendor.Name));   
                    // Init Plugin
                    SettingsApp.InitSoftwareVendorPluginSettings();
                }
                // Try to Get LicenceManager IntellilockPlugin if in Release 
                if (! Debugger.IsAttached)
                {
                    GlobalFramework.PluginLicenceManager = (GlobalFramework.PluginContainer.GetFirstPluginOfType<ILicenceManager>());
                    // Show Loaded Plugin
                    if (GlobalFramework.PluginLicenceManager != null) {
                        _log.Info(String.Format("Registered plugin: [{0}] Name : [{1}]", typeof(ILicenceManager), GlobalFramework.PluginLicenceManager.Name));
                    }
                }

                // Required before LicenseRouter
                Application.Init();

                //Render GTK Theme : In Start to Style UI in BootStrap Dialogs Error
                Theme.ParseTheme(true, false);

                // Initialize LicenseRouter if IntellilockPlugin plugin is Registered in PluginContainer
                if (GlobalFramework.PluginLicenceManager != null && ! Debugger.IsAttached)
                {
                    // Boot LogicPos after LicenceManager.IntellilockPlugin
                    LicenseRouter licenseRouter = new LicenseRouter();
                }
                else
                {
                    // Boot LogicPos without pass in LicenseRouter
                    LogicPos logicPos = new LogicPos();
                    logicPos.StartApp(AppMode.FrontOffice);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
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
            GlobalFramework.Path.Add("backups", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathBackups"]));
            GlobalFramework.Path.Add("plugins", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathPlugins"]));
            GlobalFramework.Path.Add("saftpt", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathSaftPt"]));
            //Create Directories
            FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(Convert.ToString(GlobalFramework.Path["temp"])));
            FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(Convert.ToString(GlobalFramework.Path["cache"])));
            FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(Convert.ToString(GlobalFramework.Path["saftpt"])));
            FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(string.Format(@"{0}Database\Other", Convert.ToString(GlobalFramework.Path["resources"]))));
            FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(string.Format(@"{0}Database\{1}\Other", Convert.ToString(GlobalFramework.Path["resources"]), GlobalFramework.Settings["databaseType"], @"Database\MSSqlServer")));
            FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(string.Format(@"{0}Database\{1}\Other", Convert.ToString(GlobalFramework.Path["resources"]), GlobalFramework.Settings["databaseType"], @"Database\SQLite")));
            FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(string.Format(@"{0}Database\{1}\Other", Convert.ToString(GlobalFramework.Path["resources"]), GlobalFramework.Settings["databaseType"], @"Database\MySql")));
        }
    }
}