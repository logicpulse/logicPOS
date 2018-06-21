using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.service.App;
using logicpos.financial.service.Objects;
using logicpos.financial.service.Objects.Service;
using logicpos.financial.service.Test.Modules.AT;
using logicpos.financial.servicewcf;
using logicpos.plugin.contracts;
using logicpos.plugin.library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;

namespace logicpos.financial.service
{
    class Program
    {
        //Log4Net basics with a Console Application (c#)
        //http://geekswithblogs.net/MarkPearl/archive/2012/01/30/log4net-basics-with-a-console-application-c.aspx
        //Add the assembly for the log4net.config to Properties/AssemblyInfo.cs
        //[assembly: log4net.Config.XmlConfigurator(Watch = true)]

        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static SortedList<string, Action> _testActions;
        private static string _line = string.Empty;

        //Service Private Members
        public static string SERVICE_NAME = "LogicPulse LogicPos Financial Service";
        private static ServiceHost _serviceHost;
        private static Uri _baseAddress;
        private static int _servicePort = 50391;
        public static int ServicePort
        {
            get { return Program._servicePort; }
        }
        //Timer
        private static System.Timers.Timer _timer;
        private static bool _timerRunningTasks = false;

        static void Main(string[] args)
        {
            //Init Settings Main Config Settings
            GlobalFramework.Settings = ConfigurationManager.AppSettings;

            //Base Bootstrap Init from LogicPos
            Init();

            //Service Initialization
            string uri = string.Format("http://localhost:{0}/Service1.svc", _servicePort);
            _log.Debug(string.Format("Service URI: {0}", uri));
            _baseAddress = new Uri(uri);

            //Service Mode
            if (!Environment.UserInteractive)
            {
                // Running as service
                using (var service = new Service())
                {
                    _log.Debug("Service.Run(service)");
                    Service.Run(service);
                }
            }
            //Console Mode
            else
            {
                Utils.Log("Launch service? [Y or Enter] or any other key to run in interactive develop/debug mode");
                ConsoleKeyInfo cki = Console.ReadKey();

                //Service Mode
                if (cki.Key.ToString().ToUpper() == "y".ToUpper() || cki.Key.ToString() == "Enter")
                {
                    // Running as console app
                    Start(args);

                    Console.Clear();
                    Utils.Log(string.Format("The service is ready at {0}", _baseAddress));
                    Utils.Log("Press any key to stop the service and exit");
                    Console.WriteLine();
                    Console.ReadKey();
                    Stop();
                }
                //Interactive develop mode
                else
                {
                    //Init Test Actions
                    InitTestActions();
                    //Init Main
                    InitMain();
                }
            }
        }

        //LogicPos BootStrap
        private static void Init()
        {
            try
            {
                //After Construct Settings (ex Required path["certificates"])
                //Utils.Log(string.Format("BootStrap {0}....", SettingsApp.AppName));

                // Init Paths
                GlobalFramework.Path = new Hashtable();
                GlobalFramework.Path.Add("temp", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathTemp"]));
                GlobalFramework.Path.Add("certificates", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathCertificates"]));
                GlobalFramework.Path.Add("plugins", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathPlugins"]));
                //Create Directories
                FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(Convert.ToString(GlobalFramework.Path["temp"])));
                FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(Convert.ToString(GlobalFramework.Path["certificates"])));

                // VendorPlugin
                InitPlugins();

                //Prepare AutoCreateOption
                AutoCreateOption xpoAutoCreateOption = AutoCreateOption.None;

                //Get DataBase Details
                GlobalFramework.DatabaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), GlobalFramework.Settings["databaseType"]);
                GlobalFramework.DatabaseName = SettingsApp.DatabaseName;
                //Override default Database name with parameter from config
                string configDatabaseName = GlobalFramework.Settings["databaseName"];
                GlobalFramework.DatabaseName = (string.IsNullOrEmpty(configDatabaseName)) ? SettingsApp.DatabaseName : configDatabaseName;
                //Xpo Connection String
                string xpoConnectionString = string.Format(GlobalFramework.Settings["xpoConnectionString"], GlobalFramework.DatabaseName.ToLower());

                //Init XPO Connector DataLayer
                try
                {
                    _log.Info(string.Format("Init XpoDefault.DataLayer: [{0}]", xpoConnectionString));
                    XpoDefault.DataLayer = XpoDefault.GetDataLayer(xpoConnectionString, xpoAutoCreateOption);
                    GlobalFramework.SessionXpo = new Session(XpoDefault.DataLayer) { LockingOption = LockingOption.None };
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    //Output only if in Console Mode
                    if (Environment.UserInteractive)
                    {
                        Utils.Log(ex.Message);
                        Utils.Log("Press any key...");
                        Console.ReadKey();
                    }
                    Environment.Exit(0);
                }

                //PreferenceParameters
                GlobalFramework.PreferenceParameters = FrameworkUtils.GetPreferencesParameters();

                //CultureInfo/Localization
                string culture = GlobalFramework.PreferenceParameters["CULTURE"];
                if (!string.IsNullOrEmpty(culture))
                {
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
                }
                GlobalFramework.CurrentCulture = CultureInfo.CurrentUICulture;
                //Always use en-US NumberFormat because of MySql Requirements
                GlobalFramework.CurrentCultureNumberFormat = CultureInfo.GetCultureInfo(SettingsApp.CultureNumberFormat);

                //SettingsApp
                string companyCountryOid = GlobalFramework.PreferenceParameters["COMPANY_COUNTRY_OID"];
                string systemCurrencyOid = GlobalFramework.PreferenceParameters["SYSTEM_CURRENCY_OID"];
                SettingsApp.ConfigurationSystemCountry = (CFG_ConfigurationCountry)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(CFG_ConfigurationCountry), new Guid(companyCountryOid));
                SettingsApp.ConfigurationSystemCurrency = (CFG_ConfigurationCurrency)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(CFG_ConfigurationCurrency), new Guid(systemCurrencyOid));

                //After Construct Settings (ex Required path["certificates"])
                Utils.Log(string.Format("BootStrap {0}....", SettingsApp.AppName));

                //Show WS Mode
                Utils.Log(string.Format("ServiceATEnableTestMode: [{0}]", SettingsApp.ServiceATEnableTestMode));

                // Protection to Check if all Required values are met
                if (!HasAllRequiredValues())
                {
                    throw new Exception($"Error! Invalid Parameters Met! Required parameters missing! Check parameters: AccountFiscalNumber: [{SettingsApp.ServicesATAccountFiscalNumber}], ATAccountPassword: [{SettingsApp.ServicesATAccountPassword}], TaxRegistrationNumber: [{SettingsApp.ServicesATTaxRegistrationNumber}]");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private static void InitPlugins()
        {
            // Check Path before launch error found Plugin
            string pathPlugins = GlobalFramework.Path["plugins"].ToString();
            if (string.IsNullOrEmpty(pathPlugins))
            {
                // Error Missing pluginPath
                string errorMessage = "Error! missing plugin path in config! Please fix config and try again!";
                _log.Error(errorMessage);
                Console.WriteLine(errorMessage);
                Console.ReadKey();
                Environment.Exit(0);
            }
            // Init PluginContainer
            GlobalFramework.PluginContainer = new PluginContainer(GlobalFramework.Path["plugins"].ToString());

            // PluginSoftwareVendor
            GlobalFramework.PluginSoftwareVendor = (GlobalFramework.PluginContainer.GetFirstPluginOfType<ISoftwareVendor>());
            if (GlobalFramework.PluginSoftwareVendor != null)
            {
                // Show Loaded Plugin
                _log.Info(string.Format("Registered plugin: [{0}] Name : [{1}]", typeof(ISoftwareVendor), GlobalFramework.PluginSoftwareVendor.Name));
                // Init Plugin
                SettingsApp.InitSoftwareVendorPluginSettings();
            }
            else
            {
                // Error Loading Required Plugin
                string errorMessage = string.Format("Error! missing required plugin: [{0}]. Install required plugin and try again!", typeof(ISoftwareVendor));
                _log.Error(errorMessage);
                Console.WriteLine(errorMessage);
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private static bool HasAllRequiredValues()
        {
            return !(string.IsNullOrEmpty(SettingsApp.ServicesATAccountFiscalNumber)
                || string.IsNullOrEmpty(SettingsApp.ServicesATAccountPassword)
                || string.IsNullOrEmpty(SettingsApp.ServicesATTaxRegistrationNumber)
            );
        }

        private static void InitTestActions()
        {
            // Init Dictionary
            _testActions = new SortedList<string, Action>();

            _testActions.Add("1) TestSendDocument.SendFinanceDocument()", () =>
                TestSendDocument.SendDocumentNonWayBill()
            );
            _testActions.Add("2) TestSendDocument.SendWayBillDocument()", () =>
                TestSendDocument.SendDocumentWayBill()
            );
        }

        private static void InitMain()
        {
            ConsoleKeyInfo cki;
            // Prevent example from ending if CTL+C is pressed.
            Console.TreatControlCAsInput = true;

            //Utils.Log("Press any combination of CTL, ALT, and SHIFT, and a console key.");
            //Utils.Log("Press the Escape (Esc) key to quit: \n");
            ListTestActions();

            do
            {
                Utils.Log("INP> ");
                cki = Console.ReadKey();
                Utils.Log("\n");

                //Pressed Number Key
                if (cki.Key.ToString().Length == 2 && cki.Key.ToString().Substring(0, 1) == "D")
                {
                    int keyNumber = Convert.ToInt16(cki.Key.ToString().Substring(1, 1)) - 1;
                    try
                    {
                        if (keyNumber >= 0 && keyNumber <= _testActions.Count)
                        {
                            Console.Clear();
                            //Utils.Log(string.Format("Pressed keyNumber: {0}", keyNumber, _testActions.Keys[0]));
                            Utils.Log(string.Format("Test: {0}", _testActions.Keys[keyNumber]));
                            Utils.Log(_line);

                            try
                            {
                                _testActions.Values[keyNumber].Invoke();
                            }
                            catch (Exception ex)
                            {
                                Utils.Log(ex.Message);
                            }

                            Utils.Log(_line);
                            Utils.Log("Press any key...");
                            Console.ReadKey();
                            ListTestActions();
                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.Log(ex.Message);
                        Utils.Log("Press any key...");
                        Console.ReadKey();
                    }
                }
                else
                {
                    ListTestActions();
                }
            } while (cki.Key != ConsoleKey.Escape);

            //Quit
            //Utils.Log("Press any key....");
            //Console.ReadKey();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper

        private static void ListTestActions()
        {
            Console.Clear();

            Utils.Log(string.Format("{0} : Database: [{1}]", SettingsApp.AppName, SettingsApp.DatabaseName));
            Utils.Log(_line);

            int i = 0;
            foreach (var testAction in _testActions)
            {
                i++;
                //Utils.Log(string.Format("{0}) {1}", i.ToString("00"), testAction.Key));
                Utils.Log(testAction.Key);
            }
            Utils.Log(_line);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Service Methods

        public static void Start(string[] args)
        {
            _log.Debug("Service Started");

            //Call ModifyHttpSettings
            Utils.ModifyHttpSettings();

            //Init ServiceHost
            _serviceHost = new ServiceHost(typeof(Service1), _baseAddress);

            // Enable metadata publishing.
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            _serviceHost.Description.Behaviors.Add(smb);

            // Open the ServiceHost to start listening for messages. Since
            // no endpoints are explicitly configured, the runtime will create
            // one endpoint per base address for each service contract implemented
            // by the service.
            _serviceHost.Open();

            //StartTimer
            StartTimer(_timer);
        }

        public static void Stop()
        {
            // onstop code here
            _log.Debug("Service Stoped");
            // Close the ServiceHost.
            _serviceHost.Close();

            //StopTimer
            StopTimer(_timer);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Timer

        public static void StartTimer(System.Timers.Timer pTimer)
        {
            if (SettingsApp.ServiceTimerEnabled)
            {
                _log.Debug("Service StartTimer");
                pTimer = new System.Timers.Timer(SettingsApp.ServiceTimerInterval);
                pTimer.AutoReset = true;
                pTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimerElapsedEvent);
                pTimer.Start();
            }
        }

        public static void StopTimer(System.Timers.Timer pTimer)
        {
            if (SettingsApp.ServiceTimerEnabled && pTimer != null)
            {
                //_log.Debug("Service StopTimer");
                pTimer.Stop();
                pTimer = null;
            }
        }

        public static void TimerElapsedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_timerRunningTasks)
            {
                //Started Running Tasks
                _timerRunningTasks = true;

                //_log.Debug(String.Format("Implement TimerElapsedEvent Code Here"));
                if (SettingsApp.ServiceATSendDocuments || SettingsApp.ServiceATSendDocumentsWayBill)
                {
                    Utils.ServiceSendPendentDocuments();
                }

                //Finished Running Tasks
                _timerRunningTasks = false;
            }
        }
    }
}
