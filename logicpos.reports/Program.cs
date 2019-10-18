using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.Enums;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Reports;
using logicpos.reports.Resources.Localization;
using System;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

//Log4Net
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace logicpos.reports
{
    static class Program
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            _log.Debug(String.Format("logicpos.reports open"));

            //DEVELOPER ONLY : Used !!!ONLY!!! to Force create DatabaseScema and Fixtures with XPO (Non Script Mode): Requirements for Work: Empty or Non Exist Database
            bool xpoCreateDatabaseAndSchema = false;
            bool xpoCreateDatabaseObjectsWithFixtures = xpoCreateDatabaseAndSchema;
            AutoCreateOption xpoAutoCreateOption = (xpoCreateDatabaseAndSchema) ? AutoCreateOption.DatabaseAndSchema : AutoCreateOption.None;


            GlobalFramework.Settings = ConfigurationManager.AppSettings;
            // Init Paths
            GlobalFramework.Path = new Hashtable();
            GlobalFramework.Path.Add("assets", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathAssets"]));
            GlobalFramework.Path.Add("images", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathImages"]));
            GlobalFramework.Path.Add("themes", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathThemes"]));
            GlobalFramework.Path.Add("sounds", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathSounds"]));
            GlobalFramework.Path.Add("resources", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathResources"]));
            GlobalFramework.Path.Add("keyboards", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathKeyboards"]));
            GlobalFramework.Path.Add("reports", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathReports"]));
            GlobalFramework.Path.Add("temp", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathTemp"]));
            GlobalFramework.Path.Add("cache", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathCache"]));
            GlobalFramework.Path.Add("backups", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathBackups"]));

            //CultureInfo/Localization
            string culture = GlobalFramework.Settings["culture"];
            if (!string.IsNullOrEmpty(culture))
            {
                /* IN006018 and IN007009 */
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
            }
            GlobalFramework.CurrentCulture = CultureInfo.CurrentUICulture;
            //Always use en-US NumberFormat because of mySql Requirements
            GlobalFramework.CurrentCultureNumberFormat = CultureInfo.GetCultureInfo(GlobalFramework.Settings["cultureNumberFormat"]);

            //Assign DatabaseType to GlobalApp Singleton
            GlobalFramework.DatabaseType = (DatabaseType) Enum.Parse(typeof(DatabaseType), GlobalFramework.Settings["databaseType"]);

            //Database Credentials
            string databaseName = GlobalFramework.Settings["databaseName"].ToLower();
            //Xpo Connection String
            string xpoConnectionString = String.Format(GlobalFramework.Settings["xpoConnectionString"], databaseName);

            //Init XPO Connector DataLayer
            try
            {
                _log.Debug(String.Format("Init(): Init XpoDefault.DataLayer: [{0}]", xpoConnectionString));
                XpoDefault.DataLayer = XpoDefault.GetDataLayer(xpoConnectionString, xpoAutoCreateOption);
                GlobalFramework.SessionXpo = new Session(XpoDefault.DataLayer) { LockingOption = LockingOption.None };
                //GlobalFramework.SessionXpoBO = new Session(XpoDefault.DataLayer) { LockingOption = LockingOption.None };
            }
            catch (Exception ex)
            {
                _log.Error(String.Format("Init(): {0}", ex.Message), ex);
                throw;
            }

            //PreferenceParameters
            GlobalFramework.PreferenceParameters = FrameworkUtils.GetPreferencesParameters();

            //Init FastReports Custom Functions and Custom Vars WITH Report Resources
            CustomFunctions.Register(GlobalFramework.Settings["appName"], Resx.ResourceManager, false);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            if (args == null)
            {
                _log.Debug(String.Format("Args == null", args));
                Console.WriteLine("args is null"); // Check for null array
            }
            else
            {
                if (args.Length > 0) _log.Debug(String.Format("args[{0}]==[{1}]", args[0].ToUpper(), "logicpos".ToUpper()));
                //Run in BackOffice Mode
                if (args.Length > 0 && args[0].ToUpper() == "logicpos".ToUpper())
                {
                    _log.Debug("Before Application.Run(new FormReporting())");
                    try
                    {
                        _log.Debug("Before new FormReporting() 2");
                        FormReporting reporting = new FormReporting();
                        _log.Debug("After new FormReporting() 2");
                        Application.Run(reporting);
                        _log.Debug("After new reporting.ShowDialog");
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message, ex);
                    }
                    _log.Debug("After Application.Run(new FormReporting())");
                }
                //Run in logicpos.reports Mode
                else
                {
                    // Application.Run(new StartupWindow());
                    Application.Run(new FormReporting());
                }

                _log.Debug(String.Format("Args != null", args));
            }

        }
    }
}
