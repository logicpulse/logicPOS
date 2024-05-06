using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.DataLayer;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Logic.Hardware;
using logicpos.Classes.Logic.Others;
using logicpos.Classes.Utils;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Reports;
using logicpos.financial.library.Classes.Utils;
using logicpos.financial.library.Classes.WorkSession;
using logicpos.plugin.contracts;
using logicpos.shared;
using logicpos.shared.App;
using LogicPOS.Settings;
using LogicPOS.Settings.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using static logicpos.datalayer.App.DataLayerUtils;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;

namespace logicpos
{
    internal class LogicPOSApp
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //BootStrap
        private bool _quitAfterBootStrap = false;
        /* IN009163 and IN009164 - Opt to auto-backup flow */
        private bool _autoBackupFlowIsEnabled = false;
        //Days, hours, minutes, seconds, milliseconds
        private TimeSpan _backupDatabaseTimeSpan = new TimeSpan();
        private TimeSpan _databaseBackupTimeSpanRangeStart = new TimeSpan();
        private TimeSpan _databaseBackupTimeSpanRangeEnd = new TimeSpan();
        private static bool needToUpdate = false;

        public void StartApp(AppMode pMode)
        {
            try
            {
                Init();
                GlobalApp.DialogThreadNotify.WakeupMain();
                InitAppMode(pMode);

                InitBackupTimerProcess();

                if (!_quitAfterBootStrap) Application.Run();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                Utils.ShowMessageTouch(
                    GlobalApp.StartupWindow,
                    DialogFlags.Modal,
                    new Size(500, 240),
                    MessageType.Error,
                    ButtonsType.Ok,
                    CultureResources.GetLanguageResource(GeneralSettings.Settings.GetCultureName(),
                    "global_error"),
                    CultureResources.GetLanguageResource(GeneralSettings.Settings.GetCultureName(),
                    "app_error_contact_support"));
            }
            finally
            {
                // Dispose Devices

                // Always Close Display Device
                if (GlobalApp.UsbDisplay != null)
                {
                    GlobalApp.UsbDisplay.Close();
                }
                // Always Close Com Ports
                if (GlobalApp.WeighingBalance != null && GlobalApp.WeighingBalance.IsPortOpen())
                {
                    GlobalApp.WeighingBalance.ClosePort();
                }
            }
        }


        private void Init()
        {
            try
            {
                //Used to Force create DatabaseScema and Fixtures with XPO (Non Script Mode): Requirements for Work: Empty or Non Exist Database
                //Notes: OnError "An exception of type 'DevExpress.Xpo.DB.Exceptions.SchemaCorrectionNeededException'", UnCheck [X] Break when this exception is user-unhandled and continue, watch log and wait until sucefull message appear
                bool xpoCreateDatabaseAndSchema = POSSettings.XPOCreateDatabaseAndSchema;
                bool xpoCreateDatabaseObjectsWithFixtures = xpoCreateDatabaseAndSchema;
                //Prepare AutoCreateOption
                AutoCreateOption xpoAutoCreateOption = (xpoCreateDatabaseAndSchema) ? AutoCreateOption.DatabaseAndSchema : AutoCreateOption.None;

                //Init Settings Main Config Settings
                //LogicPOS.Settings.GeneralSettings.Settings = ConfigurationManager.AppSettings;

                //Override Licence data with Encrypted File Data
                if (File.Exists(POSSettings.LicenceFileName))
                {
                    Utils.AssignLicence(POSSettings.LicenceFileName, true);
                }

                //Other Global App Settings
                GlobalApp.MultiUserEnvironment = Convert.ToBoolean(GeneralSettings.Settings["appMultiUserEnvironment"]);
                GlobalApp.UseVirtualKeyBoard = Convert.ToBoolean(GeneralSettings.Settings["useVirtualKeyBoard"]);

                //Init App Notifications
                GlobalApp.Notifications = new System.Collections.Generic.Dictionary<string, bool>
                {
                    ["SHOW_PRINTER_UNDEFINED"] = true
                };

                //System
                GlobalApp.FilePickerStartPath = Directory.GetCurrentDirectory();

                //Get DataBase Details
                DatabaseSettings.DatabaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), GeneralSettings.Settings["databaseType"]);
                //Override default Database name with parameter from config
                string configDatabaseName = GeneralSettings.Settings["databaseName"];
                DatabaseSettings.DatabaseName = (string.IsNullOrEmpty(configDatabaseName)) ? POSSettings.DatabaseName : configDatabaseName;
                //Xpo Connection String
                string xpoConnectionString = string.Format(GeneralSettings.Settings["xpoConnectionString"], DatabaseSettings.DatabaseName.ToLower());
                Utils.AssignConnectionStringToSettings(xpoConnectionString);

                //Removed Protected Files
                //ProtectedFiles, Before Create Database from Scripts, usefull if Scripts are modified by User
                if (POSSettings.ProtectedFilesUse) GlobalApp.ProtectedFiles = InitProtectedFiles();

                //Check if Database Exists if Not Create it from Scripts
                bool databaseCreated = false;

                if (!xpoCreateDatabaseAndSchema)
                {
                    //Get result to check if DB is created (true)
                    try
                    {
                        // Launch Scripts
                        POSSettings.firstBoot = true;
                        databaseCreated = DataLayer.CreateDatabaseSchema(
                            xpoConnectionString,
                            DatabaseSettings.DatabaseType,
                            DatabaseSettings.DatabaseName,
                            out needToUpdate);
                    }
                    catch (Exception ex)
                    {
                        //Extra protection to prevent goes to login without a valid connection
                        _logger.Error("void Init() :: DataLayer.CreateDatabaseSchema :: " + ex.Message, ex);

                        /* IN009034 */
                        GlobalApp.DialogThreadNotify.WakeupMain();

                        Utils.ShowMessageTouch(GlobalApp.StartupWindow, DialogFlags.Modal, new Size(900, 700), MessageType.Error, ButtonsType.Ok, CultureResources.GetLanguageResource(GeneralSettings.Settings.GetCultureName(), "global_error"), ex.Message);
                        Environment.Exit(0);
                    }
                }
                POSSettings.firstBoot = false;
                //Init XPO Connector DataLayer
                try
                {
                    /* IN007011 */
                    var connectionStringBuilder = new System.Data.Common.DbConnectionStringBuilder()
                    { ConnectionString = xpoConnectionString };
                    if (connectionStringBuilder.ContainsKey("password")) { connectionStringBuilder["password"] = "*****"; };
                    _logger.Debug(string.Format("void Init() :: Init XpoDefault.DataLayer: [{0}]", connectionStringBuilder.ToString()));

                    XpoDefault.DataLayer = XpoDefault.GetDataLayer(xpoConnectionString, xpoAutoCreateOption);
                    XPOSettings.Session = new Session(XpoDefault.DataLayer) { LockingOption = LockingOption.None };
                }
                catch (Exception ex)
                {
                    _logger.Error("void Init() :: Init XpoDefault.DataLayer: " + ex.Message, ex);

                    /* IN009034 */
                    GlobalApp.DialogThreadNotify.WakeupMain();

                    Utils.ShowMessageTouch(GlobalApp.StartupWindow, DialogFlags.Modal, new Size(900, 700), MessageType.Error, ButtonsType.Ok, CultureResources.GetLanguageResource(GeneralSettings.Settings.GetCultureName(), "global_error"), ex.Message);
                    throw; // TO DO
                }

                //Check Valid Database Scheme
                if (!xpoCreateDatabaseAndSchema && !SharedUtils.IsRunningOnMono())
                {
                    bool isSchemaValid = DataLayer.IsSchemaValid(xpoConnectionString);
                    _logger.Debug(string.Format("void Init() :: Check if Database Scheme: isSchemaValid : [{0}]", isSchemaValid));
                    if (!isSchemaValid)
                    {
                        /* IN009034 */
                        GlobalApp.DialogThreadNotify.WakeupMain();

                        string endMessage = "Invalid database Schema! Fix database Schema and Try Again!";
                        Utils.ShowMessageTouch(GlobalApp.StartupWindow, DialogFlags.Modal, new Size(500, 300), MessageType.Error, ButtonsType.Ok, CultureResources.GetLanguageResource(GeneralSettings.Settings.GetCultureName(), "global_error"), string.Format(endMessage, Environment.NewLine));
                        Environment.Exit(0);
                    }
                }

                //Compare DataBase version with software version
                //Desempenho - Comparar versão da base de dados com o versão software [IN:017526]
#if !DEBUG
                if (string.IsNullOrEmpty(GlobalFramework.DatabaseVersion))
                {
                    try
                    {
                        string sql = string.Format(@"SELECT Version FROM sys_databaseversion;", SharedFramework.DatabaseName);
                        GlobalFramework.DatabaseVersion = XPOSettings.Session.ExecuteScalar(sql).ToString();

                        string[] tmpDatabaseVersion = GlobalFramework.DatabaseVersion.Split('.');
                        long tmpDatabaseVersionNumber = int.Parse(tmpDatabaseVersion[0]) * 10000000 + int.Parse(tmpDatabaseVersion[1]) * 10000 + int.Parse(tmpDatabaseVersion[2]);

                        string[] tmpSoftwareVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
                        long tmpSoftwareVersionNumber = int.Parse(tmpSoftwareVersion[0]) * 10000000 + int.Parse(tmpSoftwareVersion[1]) * 10000 + int.Parse(tmpSoftwareVersion[2]);

                        if (tmpDatabaseVersionNumber > tmpSoftwareVersionNumber)
                        {
                            GlobalApp.DialogThreadNotify.WakeupMain();
                            //throw new InvalidOperationException(string.Format(CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_warning_message_database_version")) + " : " + GlobalFramework.DatabaseVersion);
                            string fileName = "\\LPUpdater\\LPUpdater.exe";
                            string lPathToUpdater = string.Format(@"{0}\{1}", Environment.CurrentDirectory, fileName);
                            Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(500, 300), MessageType.Error, ButtonsType.Ok, CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error"), string.Format(CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_warning_message_database_version")) + " : " + GlobalFramework.DatabaseVersion);
                            System.Diagnostics.Process.Start(lPathToUpdater);
                            Environment.Exit(0);
                        }
                    }
                    catch(Exception Ex)
                    {
                        GlobalApp.DialogThreadNotify.Close();
                        Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(500, 300), MessageType.Error, ButtonsType.Ok, CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error"),  Ex.Message);
                        Environment.Exit(0);
                    }
                }
#endif
                // Assign PluginSoftwareVendor Reference to DataLayer SettingsApp to use In Date Protection, we Required to assign it Statically to Prevent Circular References
                // Required to be here, before it is used in above lines, ex Utils.GetTerminal()
                if (PluginSettings.PluginSoftwareVendor != null) PluginContractsSettings.PluginSoftwareVendor = PluginSettings.PluginSoftwareVendor;

                //If not in Xpo create database Scheme Mode, Get Terminal from Db
                if (!xpoCreateDatabaseAndSchema)
                {
                    datalayer.App.DataLayerFramework.LoggedTerminal = Utils.GetTerminal();
                }

                //After Assigned LoggedUser
                if (xpoCreateDatabaseObjectsWithFixtures)
                {
                    InitFixtures.InitUserAndTerminal(XPOSettings.Session);
                    InitFixtures.InitOther(XPOSettings.Session);
                    InitFixtures.InitDocumentFinance(XPOSettings.Session);
                    InitFixtures.InitWorkSession(XPOSettings.Session);
                }

                //End Xpo Create Scheme and Fixtures, Terminate App and Request assign False to Developer Vars
                if (xpoCreateDatabaseAndSchema)
                {
                    /* IN009034 */
                    GlobalApp.DialogThreadNotify.WakeupMain();

                    string endMessage = "Xpo Create Schema and Fixtures Done!{0}Please assign false to 'xpoCreateDatabaseAndSchema' and 'xpoCreateDatabaseObjectsWithFixtures' and run App again";
                    _logger.Debug(string.Format("void Init() :: xpoCreateDatabaseAndSchema: {0}", endMessage));

                    Utils.ShowMessageTouch(GlobalApp.StartupWindow, DialogFlags.Modal, new Size(500, 300), MessageType.Info, ButtonsType.Ok, CultureResources.GetLanguageResource(GeneralSettings.Settings.GetCultureName(), "global_information"), string.Format(endMessage, Environment.NewLine));
                    Environment.Exit(0);
                }

                //Init PreferenceParameters
                GeneralSettings.PreferenceParameters = SharedUtils.GetPreferencesParameters();
                //Init Preferences Path
                Paths.InitializePathsPrefs();

                //CultureInfo/Localization
                string culture = GeneralSettings.PreferenceParameters["CULTURE"];

                /* IN008013 */
                if (string.IsNullOrEmpty(culture))
                {
                    culture = GeneralSettings.Settings.GetCultureName();
                }



                //if (!string.IsNullOrEmpty(culture))
                //{
                /* IN006018 and IN007009 */
                //logicpos.shared.App.CustomRegion.RegisterCustomRegion();
                //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
                //}
                //if (!Utils.IsLinux)
                //{
                //    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
                //}                
                CultureSettings.CurrentCulture = CultureSettings.CurrentCulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["customCultureResourceDefinition"]);

                /* IN006018 and IN007009 */
                _logger.Debug(string.Format("CUSTOM CULTURE :: CurrentUICulture '{0}' in use.", CultureInfo.CurrentUICulture));

                //Always use en-US NumberFormat because of mySql Requirements
                CultureSettings.CurrentCultureNumberFormat = CultureInfo.GetCultureInfo(POSSettings.CultureNumberFormat);

                //Init AppSession
                string appSessionFile = Utils.GetSessionFileName();
                if (databaseCreated && File.Exists(appSessionFile)) File.Delete(appSessionFile);
                SharedFramework.SessionApp = GlobalFrameworkSession.InitSession(appSessionFile);

                //Try to Get open Session Day/Terminal for this Terminal
                SharedFramework.WorkSessionPeriodDay = ProcessWorkSessionPeriod.GetSessionPeriod(WorkSessionPeriodType.Day);
                SharedFramework.WorkSessionPeriodTerminal = ProcessWorkSessionPeriod.GetSessionPeriod(WorkSessionPeriodType.Terminal);

                //Use Detected ScreenSize
                string appScreenSize = string.IsNullOrEmpty(GeneralSettings.Settings["appScreenSize"])
                    ? GeneralSettings.PreferenceParameters["APP_SCREEN_SIZE"]
                    : GeneralSettings.Settings["appScreenSize"];
                if (appScreenSize.Replace(" ", string.Empty).Equals("0,0") || string.IsNullOrEmpty(appScreenSize))
                {
                    // Force Unknown Screen Size
                    //GlobalApp.ScreenSize = new Size(2000, 1800);
                    GlobalApp.ScreenSize = Utils.GetThemeScreenSize();
                }
                //Use config ScreenSize
                else
                {
                    Size configAppScreenSize = Utils.StringToSize(appScreenSize);
                    GlobalApp.ScreenSize = Utils.GetThemeScreenSize(configAppScreenSize);
                }

                // Init ExpressionEvaluator
                GlobalApp.ExpressionEvaluator.EvaluateFunction += ExpressionEvaluatorExtended.ExpressionEvaluator_EvaluateFunction;
                // Init Variables
                ExpressionEvaluatorExtended.InitVariablesStartupWindow();
                ExpressionEvaluatorExtended.InitVariablesPosMainWindow();

                // Define Max Dialog Window Size
                GlobalApp.MaxWindowSize = new Size(GlobalApp.ScreenSize.Width - 40, GlobalApp.ScreenSize.Height - 40);
                // Add Variables to ExpressionEvaluator.Variables Singleton
                GlobalApp.ExpressionEvaluator.Variables.Add("globalScreenSize", GlobalApp.ScreenSize);
                //to used in shared projects
                SharedFramework.ScreenSize = GlobalApp.ScreenSize;
                //Parse and store Theme in Singleton
                try
                {
                    GlobalApp.Theme = XmlToObjectParser.ParseFromFile(POSSettings.FileTheme);
                    // Use with dynamic Theme properties like: 
                    // GlobalApp.Theme.Theme.Frontoffice.Window[0].Globals.Name = PosBaseWindow
                    // GlobalApp.Theme.Theme.Frontoffice.Window[1].Objects.TablePadUser.Position = 50,50
                    // or use predicate with from object id ex 
                    //var predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "StartupWindow");
                    //var themeWindow = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);
                    //_logger.Debug(string.Format("Message: [{0}]", themeWindow.Globals.Title));
                }
                catch (Exception ex)
                {
                    /* IN009034 */
                    GlobalApp.DialogThreadNotify.WakeupMain();

                    _logger.Debug("void Init() :: XmlToObjectParser.ParseFromFile(SettingsApp.FileTheme) :: " + ex);
                    Utils.ShowMessageTouchErrorRenderTheme(GlobalApp.StartupWindow, ex.Message);
                }

                //Init FastReports Custom Functions and Custom Vars
                CustomFunctions.Register(POSSettings.AppName);

                //Hardware : Init Display
                if (datalayer.App.DataLayerFramework.LoggedTerminal.PoleDisplay != null)
                {
                    GlobalApp.UsbDisplay = (UsbDisplayDevice)UsbDisplayDevice.InitDisplay();
                    GlobalApp.UsbDisplay.WriteCentered(string.Format("{0} {1}", POSSettings.AppName, SharedUtils.ProductVersion), 1);
                    GlobalApp.UsbDisplay.WriteCentered(POSSettings.AppUrl, 2);
                    GlobalApp.UsbDisplay.EnableStandBy();
                }

                //Hardware : Init BarCodeReader 
                if (datalayer.App.DataLayerFramework.LoggedTerminal.BarcodeReader != null)
                {
                    GlobalApp.BarCodeReader = new InputReader();
                }

                //Hardware : Init WeighingBalance
                if (datalayer.App.DataLayerFramework.LoggedTerminal.WeighingMachine != null)
                {
                    //Protecções de integridade das BD's [IN:013327]
                    //Check if port is used by pole display
                    if (datalayer.App.DataLayerFramework.LoggedTerminal.WeighingMachine.PortName == datalayer.App.DataLayerFramework.LoggedTerminal.PoleDisplay.COM)
                    {
                        _logger.Debug(string.Format("Port " + datalayer.App.DataLayerFramework.LoggedTerminal.WeighingMachine.PortName + "Already taken by pole display"));
                    }
                    else
                    {
                        if (Utils.IsPortOpen(datalayer.App.DataLayerFramework.LoggedTerminal.WeighingMachine.PortName))
                        {
                            GlobalApp.WeighingBalance = new WeighingBalance(datalayer.App.DataLayerFramework.LoggedTerminal.WeighingMachine);
                            //_logger.Debug(string.Format("IsPortOpen: [{0}]", GlobalApp.WeighingBalance.IsPortOpen())); }
                        }

                    }

                }

                //Send To Log
                _logger.Debug(string.Format("void Init() :: ProductVersion: [{0}], ImageRuntimeVersion: [{1}], IsLicensed: [{2}]", SharedUtils.ProductVersion, SharedUtils.ProductAssembly.ImageRuntimeVersion, LicenceManagement.IsLicensed));

                //Audit
                SharedUtils.Audit("APP_START", string.Format("{0} {1} clr {2}", POSSettings.AppName, SharedUtils.ProductVersion, SharedUtils.ProductAssembly.ImageRuntimeVersion));
                if (databaseCreated) SharedUtils.Audit("DATABASE_CREATE");

                // Plugin Errors Messages
                if (PluginSettings.PluginSoftwareVendor == null || !PluginSettings.PluginSoftwareVendor.IsValidSecretKey(FinancialLibrarySettings.SecretKey))
                {
                    /* IN009034 */
                    GlobalApp.DialogThreadNotify.WakeupMain();

                    _logger.Debug(string.Format("void Init() :: Wrong key detected [{0}]. Use a valid LogicposFinantialLibrary with same key as SoftwareVendorPlugin", FinancialLibrarySettings.SecretKey));
                    Utils.ShowMessageTouch(
                        GlobalApp.StartupWindow,
                        DialogFlags.Modal,
                        new Size(650, 380),
                        MessageType.Error,
                        ButtonsType.Ok,
                        CultureResources.GetLanguageResource(GeneralSettings.Settings.GetCultureName(),
                        "global_error"),
                        CultureResources.GetLanguageResource(GeneralSettings.Settings.GetCultureName(),
                        "dialog_message_error_plugin_softwarevendor_not_registered"));
                }

                // TK013134: HardCoded Modules : PakingTicket
                try
                {
                    // Override default AppUseParkingTicketModule
                    /* IN009239 */
                    //GlobalFramework.AppUseParkingTicketModule = Convert.ToBoolean(LogicPOS.Settings.GeneralSettings.Settings["appMultiUserEnvironment"]);
                    CustomAppOperationMode customAppOperationMode = GetCustomAppOperationMode();
                    SharedFramework.AppUseParkingTicketModule = CustomAppOperationMode.PARKING.Equals(customAppOperationMode);

                    //TK016235 BackOffice - Mode
                    SharedFramework.AppUseBackOfficeMode = CustomAppOperationMode.BACKOFFICE.Equals(customAppOperationMode);

                    // Init Global Object GlobalApp.ParkingTicket
                    if (SharedFramework.AppUseParkingTicketModule)
                    {
                        GlobalApp.ParkingTicket = new ParkingTicket();
                    }
                }
                catch (Exception)
                {
                    _logger.Error(string.Format("void Init() :: Missing AppUseParkingTicketModule Token in Settings, using default value: [{0}]", SharedFramework.AppUseParkingTicketModule));
                }


                //Create SystemNotification
                SharedUtils.SystemNotification();

                //Activate stock module for debug
#if DEBUG 
                LicenseSettings.LicenseModuleStocks = true;
                PluginSettings.AppCompanyName = LicenseSettings.LicenseCompany = LicenseSettings.LicenseReseller = "Logicpulse";
#endif

                //Clean Documents Folder on New Database, else we have Document files that dont correspond to Database
                if (databaseCreated && Directory.Exists(datalayer.App.DataLayerFramework.Path["documents"].ToString()))
                {
                    string documentsFolder = datalayer.App.DataLayerFramework.Path["documents"].ToString();
                    System.IO.DirectoryInfo di = new DirectoryInfo(documentsFolder);
                    if (di.GetFiles().Length > 0)
                    {
                        _logger.Debug(string.Format("void Init() :: New database created. Start Delete [{0}] document(s) from [{1}] folder!", di.GetFiles().Length, documentsFolder));
                        foreach (FileInfo file in di.GetFiles())
                        {
                            try
                            {
                                file.Delete();
                            }
                            catch (Exception)
                            {
                                _logger.Error(string.Format("void Init() :: Error! Cant delete Document file: [{0}]", file.Name));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("void Init() :: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// It creates automatic backup process and its call to TimerHandler.
        /// 
        /// Please see IN009163 and IN009164
        /// </summary>
        private void InitBackupTimerProcess()
        {
            bool xpoCreateDatabaseAndSchema = POSSettings.XPOCreateDatabaseAndSchema;
            bool validDirectoryBackup = SharedUtils.CreateDirectory(Convert.ToString(datalayer.App.DataLayerFramework.Path["backups"]));
            _logger.Debug("void InitBackupTimerProcess() :: xpoCreateDatabaseAndSchema [ " + xpoCreateDatabaseAndSchema + " ] :: validDirectoryBackup [ " + validDirectoryBackup + " ]");

            //Show Dialog if Cant Create Backups Directory (Extra Protection for Shared Network Folders)
            if (!validDirectoryBackup)
            {
                ResponseType response = Utils.ShowMessageTouch(GlobalApp.StartupWindow, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, CultureResources.GetLanguageResource(GeneralSettings.Settings.GetCultureName(), "global_error"), string.Format(CultureResources.GetLanguageResource(GeneralSettings.Settings.GetCultureName(), "dialog_message_error_create_directory_backups"), Convert.ToString(datalayer.App.DataLayerFramework.Path["backups"])));
                //Enable Quit After BootStrap, Preventing Application.Run()
                if (response == ResponseType.No) _quitAfterBootStrap = true;
            }

            //Start Database Backup Timer if not create XPO Schema and SoftwareVendor is Active
            if (PluginSettings.PluginSoftwareVendor != null && validDirectoryBackup && !xpoCreateDatabaseAndSchema)
            {
                /* IN009163 and IN009164 - Opt to auto-backup flow */
                _autoBackupFlowIsEnabled = bool.Parse(GeneralSettings.PreferenceParameters["DATABASE_BACKUP_AUTOMATIC_ENABLED"]);

                /* IN009164 */
                if (_autoBackupFlowIsEnabled)
                {
                    /* IN009164 - considering these variables are only used for automatic backup purposes, will be settled only when Auto-Backup Flow is enabled */
                    _backupDatabaseTimeSpan = TimeSpan.Parse(GeneralSettings.PreferenceParameters["DATABASE_BACKUP_TIMESPAN"]);
                    _databaseBackupTimeSpanRangeStart = TimeSpan.Parse(GeneralSettings.PreferenceParameters["DATABASE_BACKUP_TIME_SPAN_RANGE_START"]);
                    _databaseBackupTimeSpanRangeEnd = TimeSpan.Parse(GeneralSettings.PreferenceParameters["DATABASE_BACKUP_TIME_SPAN_RANGE_END"]);
                    /* IN009164 - TimeoutHandler() for UpdateBackupTimer() will not be created if Auto-Backup Flow is enabled */
                    StartBackupTimer();
                }
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private ProtectedFiles InitProtectedFiles()
        {
            bool debug = true;
            string filePath = POSSettings.ProtectedFilesFileName;
            List<string> fileList = POSSettings.ProtectedFilesList;

            ProtectedFiles protectedFiles;
            //ReCreate File MODE
            if (POSSettings.ProtectedFilesRecreateCSV)
            {
                protectedFiles = new ProtectedFiles(fileList, filePath);
                string md5FromFile = LogicPOS.Utility.StringUtils.MD5HashFile(filePath);
                //End Xpo Create Scheme and Fixtures, Terminate App and Request assign False to Developer Vars
                //string message = string.Format(@"ProtectedFiles '{1}' re-created with {2} files found!
                //    {0}- Assign false to 'SettingsApp.ProtectedFilesRecreateCsv'.
                //    {0}- Update logicpos.financial.library SettingsApp.ProtectedFilesFileHash with Hash: '{3}.'"
                //    , Environment.NewLine, filePath, fileList.Count, md5FromFile
                //);
                //_logger.Debug(String.Format("Protected files: [{0}]", message));
                string message = string.Format(@"ProtectedFiles '{1}' re-created with {2} files found!{0}{0}Assign false to 'SettingsApp.ProtectedFilesRecreateCsv' and run app again.", Environment.NewLine, filePath, fileList.Count);

                ExportProtectedFiles(fileList);
                Utils.ShowMessageTouch(GlobalApp.StartupWindow, DialogFlags.Modal, new System.Drawing.Size(600, 350), MessageType.Info, ButtonsType.Ok, CultureResources.GetLanguageResource(GeneralSettings.Settings.GetCultureName(), "global_information"), message);
                Environment.Exit(0);
            }
            //Dont check changed files if Developer, Uncomment to Enable
            //else if (SettingsApp.DeveloperMode)
            //{
            //    protectedFiles = new ProtectedFiles(filePath);
            //}
            //Use File
            else
            {
                protectedFiles = new ProtectedFiles(filePath);
                foreach (var item in protectedFiles)
                {
                    if (debug) _logger.Debug(string.Format("Message: [{0}], Valid: [{1}], IsValidFile: [{2}]", item.Key, item.Value.Valid, protectedFiles.IsValidFile(item.Key)));
                }

                List<string> getInvalidAndMissingFiles = protectedFiles.GetInvalidAndMissingFiles(fileList);

                if (getInvalidAndMissingFiles.Count > 0)
                {
                    string filesMessage = string.Empty;
                    for (int i = 0; i < getInvalidAndMissingFiles.Count; i++)
                    {
                        if (debug) _logger.Debug(string.Format("InvalidFile: [{0}]", getInvalidAndMissingFiles[i]));
                        filesMessage += string.Format("{0}{1}", getInvalidAndMissingFiles[i], Environment.NewLine);
                    }

                    //If Not IgnoreProtection, show alert and exit
                    if (!POSSettings.ProtectedFilesIgnoreProtection)
                    {
                        Utils.ShowMessageTouch(
                            GlobalApp.StartupWindow,
                            DialogFlags.Modal,
                            new Size(800, 400),
                            MessageType.Error,
                            ButtonsType.Close,
                            CultureResources.GetLanguageResource(GeneralSettings.Settings.GetCultureName(),
                            "global_error"), string.Format(CultureResources.GetLanguageResource(GeneralSettings.Settings.GetCultureName(),
                            "dialog_message_error_protected_files_invalid_files_detected"),
                            filesMessage));

                        Environment.Exit(0);
                    }
                }
            }

            return protectedFiles;
        }

        //Export Files
        public bool ExportProtectedFiles(List<string> pFileList)
        {
            bool result = false;
            string[] files = new string[pFileList.Count + 1];
            string filename = string.Format("{0}{1}", datalayer.App.DataLayerFramework.Path["temp"], "protected.zip");

            try
            {
                for (int i = 0; i < pFileList.Count; i++)
                {
                    files[i] = pFileList[i];
                }
                files[pFileList.Count] = POSSettings.ProtectedFilesFileName;

                //Empty password, to zip without password
                result = LogicPOS.Utility.Compression.ZipPack(files, filename, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void InitAppMode(AppMode pAppMode)
        {
            //Run in BackOffice Mode
            if (pAppMode == AppMode.Backoffice)
            {
                GlobalApp.BackOfficeMainWindow = new BackOfficeMainWindow();
            }
            //Run in POS Mode
            else
            {
                //Init Theme Object
                _logger.Debug("Init Theme Object ");
                var predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "StartupWindow");
                var themeWindow = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);

                //// Inject themeWindow into 
                //GlobalApp.ExpressionEvaluator.Variables.Add("themeWindow", themeWindow);

                try
                {
                    _logger.Debug("Init windowImageFileName ");
                    string windowImageFileName = string.Format(themeWindow.Globals.ImageFileName, GlobalApp.ScreenSize.Width, GlobalApp.ScreenSize.Height);
                    _logger.Debug("StartupWindow " + windowImageFileName);
                    GlobalApp.StartupWindow = new StartupWindow(windowImageFileName, needToUpdate);


                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            };
        }


        public static void QuitWithoutConfirmation(bool pAudit = true)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            try
            {
                //Audit
                if (pAudit) SharedUtils.Audit("APP_CLOSE");
                //Before use DeleteSession()
                /* IN005943 */
                SharedFramework.SessionApp.CleanSession();
                SharedFramework.SessionApp.Write();
                //GlobalFramework.SessionApp.DeleteSession();
                //Disconnect SessionXpo
                XPOSettings.Session.Disconnect();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            Application.Quit();
            //Environment.Exit(0);
        }

        public static void Quit(Window pSourceWindow)
        {
            ResponseType responseType = Utils.ShowMessageTouch(
                pSourceWindow,
                DialogFlags.Modal,
                new Size(400, 300),
                MessageType.Question,
                ButtonsType.YesNo,
                CultureResources.GetLanguageResource(GeneralSettings.Settings.GetCultureName(),
                "global_quit_title"),
                CultureResources.GetLanguageResource(GeneralSettings.Settings.GetCultureName(),
                "global_quit_message"));

            if (responseType == ResponseType.Yes)
            {
                QuitWithoutConfirmation();
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //BackupTimer

        private void StartBackupTimer()
        {
            try
            {
                // Every second call update_status' (1000 milliseconds)
                GLib.Timeout.Add(POSSettings.BackupTimerInterval, new GLib.TimeoutHandler(UpdateBackupTimer));
            }
            catch (Exception ex)
            {
                _logger.Error("void StartBackupTimer() :: _autoBackupFlowIsActive: [" + _autoBackupFlowIsEnabled + "] :: " + ex.Message, ex);
            }
        }

        private bool UpdateBackupTimer()
        {
            _logger.Debug("bool UpdateBackupTimer()");
            bool debug = false;

            DateTime currentDateTime = CurrentDateTimeAtomic();
            DateTime currentDateTimeLastBackup = DataBaseBackup.GetLastBackupDate();
            TimeSpan timeSpanDiference = currentDateTime - currentDateTimeLastBackup;

            //Check if is in Start end Range
            if (currentDateTime.TimeOfDay > _databaseBackupTimeSpanRangeStart && currentDateTime.TimeOfDay < _databaseBackupTimeSpanRangeEnd)
            {
                if (timeSpanDiference >= _backupDatabaseTimeSpan)
                {
                    /* ERR201810#15 - Database backup issues */
                    DataBaseBackup.Backup(null);
                    //DataBaseBackup.Backup();
                }
                else
                {
                    if (debug) _logger.Debug(string.Format("Inside of TimeRange: currentDateTime:[{0}], backupLastDateTime:[{1}], timeSpanDiference:[{2}], backupDatabaseTimeSpan:[{3}] ", currentDateTime, currentDateTimeLastBackup, timeSpanDiference, _backupDatabaseTimeSpan));
                }
            }
            else
            {
                if (debug) _logger.Debug(string.Format("Outside of TimeRange: [{0}] > [{1}] && [{2}] < [{3}]", currentDateTime.TimeOfDay, _databaseBackupTimeSpanRangeStart, currentDateTime.TimeOfDay, _databaseBackupTimeSpanRangeEnd));
            }

            // Returning true means that the timeout routine should be invoked
            // again after the timeout period expires. Returning false would
            // terminate the timeout.
            return true;
        }
    }
}
