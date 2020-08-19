using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.DataLayer;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Logic.Hardware;
using logicpos.Classes.Logic.Others;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.Classes.Reports;
using logicpos.financial.library.Classes.Utils;
using logicpos.financial.library.Classes.WorkSession;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;

namespace logicpos
{
    class LogicPos
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //BootStrap
        private bool _quitAfterBootStrap = false;
        /* IN009163 and IN009164 - Opt to auto-backup flow */
        private bool _autoBackupFlowIsEnabled = false;
        //Days, hours, minutes, seconds, milliseconds
        private TimeSpan _backupDatabaseTimeSpan = new TimeSpan();
        private TimeSpan _databaseBackupTimeSpanRangeStart = new TimeSpan();
        private TimeSpan _databaseBackupTimeSpanRangeEnd = new TimeSpan();
        
        public void StartApp(AppMode pMode)
        {
            try
            {
                Init();
                /* IN009005: this must stay here if "loading" is implemented */
                GlobalApp.DialogThreadNotify.WakeupMain();
                InitAppMode(pMode);

                // Old Stub used to Init MediaNova Module
                InitModules();

                /* IN009164 */
                InitBackupTimerProcess();

                // Check if user cancel App Run on BootStrap and Launch Quit
                if (!_quitAfterBootStrap) Application.Run();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(500, 240), MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "app_error_contact_support"));
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

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void Init()
        {
            try
            {
                //Used to Force create DatabaseScema and Fixtures with XPO (Non Script Mode): Requirements for Work: Empty or Non Exist Database
                //Notes: OnError "An exception of type 'DevExpress.Xpo.DB.Exceptions.SchemaCorrectionNeededException'", UnCheck [X] Break when this exception is user-unhandled and continue, watch log and wait until sucefull message appear
                bool xpoCreateDatabaseAndSchema = SettingsApp.XPOCreateDatabaseAndSchema;
                bool xpoCreateDatabaseObjectsWithFixtures = xpoCreateDatabaseAndSchema;
                //Prepare AutoCreateOption
                AutoCreateOption xpoAutoCreateOption = (xpoCreateDatabaseAndSchema) ? AutoCreateOption.DatabaseAndSchema : AutoCreateOption.None;

                //Init Settings Main Config Settings
                //GlobalFramework.Settings = ConfigurationManager.AppSettings;

                //Override Licence data with Encrypted File Data
                if (File.Exists(SettingsApp.LicenceFileName))
                {
                    Utils.AssignLicence(SettingsApp.LicenceFileName, true);
                }

                //Other Global App Settings
                GlobalApp.MultiUserEnvironment = Convert.ToBoolean(GlobalFramework.Settings["appMultiUserEnvironment"]);
                GlobalApp.UseVirtualKeyBoard = Convert.ToBoolean(GlobalFramework.Settings["useVirtualKeyBoard"]);

                //Init App Notifications
                GlobalApp.Notifications = new System.Collections.Generic.Dictionary<string, bool>();
                GlobalApp.Notifications["SHOW_PRINTER_UNDEFINED"] = true;

                //System
                GlobalApp.FilePickerStartPath = System.IO.Directory.GetCurrentDirectory();

                //Get DataBase Details
                GlobalFramework.DatabaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), GlobalFramework.Settings["databaseType"]);
                //Override default Database name with parameter from config
                string configDatabaseName = GlobalFramework.Settings["databaseName"];
                GlobalFramework.DatabaseName = (string.IsNullOrEmpty(configDatabaseName)) ? SettingsApp.DatabaseName : configDatabaseName;
                //Xpo Connection String
                string xpoConnectionString = string.Format(GlobalFramework.Settings["xpoConnectionString"], GlobalFramework.DatabaseName.ToLower());
                Utils.AssignConnectionStringToSettings(xpoConnectionString);

                //Removed Protected Files
                //ProtectedFiles, Before Create Database from Scripts, usefull if Scripts are modified by User
                if (SettingsApp.ProtectedFilesUse) GlobalApp.ProtectedFiles = InitProtectedFiles();

                //Check if Database Exists if Not Create it from Scripts
                bool databaseCreated = false;
                if (!xpoCreateDatabaseAndSchema)
                {
                    //Get result to check if DB is created (true)
                    try
                    {
                        // Launch Scripts
                        SettingsApp.firstBoot = true;
                        databaseCreated = DataLayer.CreateDatabaseSchema(xpoConnectionString, GlobalFramework.DatabaseType, GlobalFramework.DatabaseName);                        
                    }
                    catch (Exception ex)
                    {
                        //Extra protection to prevent goes to login without a valid connection
                        _log.Error("void Init() :: DataLayer.CreateDatabaseSchema :: " + ex.Message, ex);

                        /* IN009034 */
                        GlobalApp.DialogThreadNotify.WakeupMain();

                        Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(900, 700), MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), ex.Message);
                        Environment.Exit(0);
                    }
                }
                SettingsApp.firstBoot = false;
                //Init XPO Connector DataLayer
                try
                {
                    /* IN007011 */
                    var connectionStringBuilder = new System.Data.Common.DbConnectionStringBuilder()
                    { ConnectionString = xpoConnectionString };
                    if (connectionStringBuilder.ContainsKey("password")) { connectionStringBuilder["password"] = "*****"; };
                    _log.Debug(string.Format("void Init() :: Init XpoDefault.DataLayer: [{0}]", connectionStringBuilder.ToString()));

                    XpoDefault.DataLayer = XpoDefault.GetDataLayer(xpoConnectionString, xpoAutoCreateOption);
                    GlobalFramework.SessionXpo = new Session(XpoDefault.DataLayer) { LockingOption = LockingOption.None };
                }
                catch (Exception ex)
                {
                    _log.Error("void Init() :: Init XpoDefault.DataLayer: " + ex.Message, ex);

                    /* IN009034 */
                    GlobalApp.DialogThreadNotify.WakeupMain();

                    Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(900, 700), MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), ex.Message);
                    throw; // TO DO
                }

                //Check Valid Database Scheme
                if (!xpoCreateDatabaseAndSchema && !FrameworkUtils.IsRunningOnMono())
                {
                    bool isSchemaValid = DataLayer.IsSchemaValid(xpoConnectionString);
                    _log.Debug(string.Format("void Init() :: Check if Database Scheme: isSchemaValid : [{0}]", isSchemaValid));
                    if (!isSchemaValid)
                    {
                        /* IN009034 */
                        GlobalApp.DialogThreadNotify.WakeupMain();

                        string endMessage = "Invalid database Schema! Fix database Schema and Try Again!";
                        Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(500, 300), MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), string.Format(endMessage, Environment.NewLine));
                        Environment.Exit(0);
                    }
                }

                // Assign PluginSoftwareVendor Reference to DataLayer SettingsApp to use In Date Protection, we Required to assign it Statically to Prevent Circular References
                // Required to be here, before it is used in above lines, ex Utils.GetTerminal()
                if (GlobalFramework.PluginSoftwareVendor != null) logicpos.datalayer.App.SettingsApp.PluginSoftwareVendor = GlobalFramework.PluginSoftwareVendor;           

                //If not in Xpo create database Scheme Mode, Get Terminal from Db
                if (!xpoCreateDatabaseAndSchema)
                {
                    GlobalFramework.LoggedTerminal = Utils.GetTerminal();
                }

                //After Assigned LoggedUser
                if (xpoCreateDatabaseObjectsWithFixtures)
                {
                    InitFixtures.InitUserAndTerminal(GlobalFramework.SessionXpo);
                    InitFixtures.InitOther(GlobalFramework.SessionXpo);
                    InitFixtures.InitDocumentFinance(GlobalFramework.SessionXpo);
                    InitFixtures.InitWorkSession(GlobalFramework.SessionXpo);
                }

                //End Xpo Create Scheme and Fixtures, Terminate App and Request assign False to Developer Vars
                if (xpoCreateDatabaseAndSchema)
                {
                    /* IN009034 */
                    GlobalApp.DialogThreadNotify.WakeupMain();

                    string endMessage = "Xpo Create Schema and Fixtures Done!{0}Please assign false to 'xpoCreateDatabaseAndSchema' and 'xpoCreateDatabaseObjectsWithFixtures' and run App again";
                    _log.Debug(string.Format("void Init() :: xpoCreateDatabaseAndSchema: {0}", endMessage));

                    Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(500, 300), MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), string.Format(endMessage, Environment.NewLine));
                    Environment.Exit(0);
                }

                //Init PreferenceParameters
                GlobalFramework.PreferenceParameters = FrameworkUtils.GetPreferencesParameters();
                //Init Preferences Path
                MainApp.InitPathsPrefs();

                //CultureInfo/Localization
                string culture = GlobalFramework.PreferenceParameters["CULTURE"];

                /* IN008013 */
                if (String.IsNullOrEmpty(culture))
                {
                    culture = GlobalFramework.Settings["customCultureResourceDefinition"];
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
                GlobalFramework.CurrentCulture = CultureInfo.CurrentUICulture;
                
                /* IN006018 and IN007009 */
                _log.Debug(string.Format("CUSTOM CULTURE :: CurrentUICulture '{0}' in use.", CultureInfo.CurrentUICulture));

                //Always use en-US NumberFormat because of mySql Requirements
                GlobalFramework.CurrentCultureNumberFormat = CultureInfo.GetCultureInfo(SettingsApp.CultureNumberFormat);

                //Init AppSession
                string appSessionFile = Utils.GetSessionFileName();
                if (databaseCreated && File.Exists(appSessionFile)) File.Delete(appSessionFile);
                GlobalFramework.SessionApp = GlobalFrameworkSession.InitSession(appSessionFile);

                //Try to Get open Session Day/Terminal for this Terminal
                GlobalFramework.WorkSessionPeriodDay = ProcessWorkSessionPeriod.GetSessionPeriod(WorkSessionPeriodType.Day);
                GlobalFramework.WorkSessionPeriodTerminal = ProcessWorkSessionPeriod.GetSessionPeriod(WorkSessionPeriodType.Terminal);

                //Use Detected ScreenSize
                string appScreenSize = string.IsNullOrEmpty(GlobalFramework.Settings["appScreenSize"])
                    ? GlobalFramework.PreferenceParameters["APP_SCREEN_SIZE"]
                    : GlobalFramework.Settings["appScreenSize"];
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
                GlobalFramework.screenSize = GlobalApp.ScreenSize;
                //Parse and store Theme in Singleton
                try
                {
                    GlobalApp.Theme = XmlToObjectParser.ParseFromFile(SettingsApp.FileTheme);
                    // Use with dynamic Theme properties like: 
                    // GlobalApp.Theme.Theme.Frontoffice.Window[0].Globals.Name = PosBaseWindow
                    // GlobalApp.Theme.Theme.Frontoffice.Window[1].Objects.TablePadUser.Position = 50,50
                    // or use predicate with from object id ex 
                    //var predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "StartupWindow");
                    //var themeWindow = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);
                    //_log.Debug(string.Format("Message: [{0}]", themeWindow.Globals.Title));
                }
                catch (Exception ex)
                {
                    /* IN009034 */
                    GlobalApp.DialogThreadNotify.WakeupMain();

                    _log.Debug("void Init() :: XmlToObjectParser.ParseFromFile(SettingsApp.FileTheme) :: " + ex);
                    Utils.ShowMessageTouchErrorRenderTheme(GlobalApp.WindowStartup, ex.Message);
                }

                //Init FastReports Custom Functions and Custom Vars
                CustomFunctions.Register(SettingsApp.AppName);

                //Hardware : Init Display
                if (GlobalFramework.LoggedTerminal.PoleDisplay != null)
                {
                    GlobalApp.UsbDisplay = (UsbDisplayDevice)UsbDisplayDevice.InitDisplay();
                    GlobalApp.UsbDisplay.WriteCentered(string.Format("{0} {1}", SettingsApp.AppName, FrameworkUtils.ProductVersion), 1);
                    GlobalApp.UsbDisplay.WriteCentered(SettingsApp.AppUrl, 2);
                    GlobalApp.UsbDisplay.EnableStandBy();
                }

                //Hardware : Init BarCodeReader 
                if (GlobalFramework.LoggedTerminal.BarcodeReader != null)
                {
                    GlobalApp.BarCodeReader = new InputReader();
                }

                //Hardware : Init WeighingBalance
                if (GlobalFramework.LoggedTerminal.WeighingMachine != null)
                {
                    GlobalApp.WeighingBalance = new WeighingBalance(GlobalFramework.LoggedTerminal.WeighingMachine);
                    //_log.Debug(string.Format("IsPortOpen: [{0}]", GlobalApp.WeighingBalance.IsPortOpen()));
                }

                //Send To Log
                _log.Debug(string.Format("void Init() :: ProductVersion: [{0}], ImageRuntimeVersion: [{1}], IsLicensed: [{2}]", FrameworkUtils.ProductVersion, FrameworkUtils.ProductAssembly.ImageRuntimeVersion, LicenceManagement.IsLicensed));

                //Audit
                FrameworkUtils.Audit("APP_START", string.Format("{0} {1} clr {2}", SettingsApp.AppName, FrameworkUtils.ProductVersion, FrameworkUtils.ProductAssembly.ImageRuntimeVersion));
                if (databaseCreated) FrameworkUtils.Audit("DATABASE_CREATE");

                // Plugin Errors Messages
                if (GlobalFramework.PluginSoftwareVendor == null || !GlobalFramework.PluginSoftwareVendor.IsValidSecretKey(SettingsApp.SecretKey))
                {
                    /* IN009034 */
                    GlobalApp.DialogThreadNotify.WakeupMain();

                    _log.Debug(String.Format("void Init() :: Wrong key detected [{0}]. Use a valid LogicposFinantialLibrary with same key as SoftwareVendorPlugin", SettingsApp.SecretKey));
                    Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(650, 380), MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_error_plugin_softwarevendor_not_registered"));
                }

                // TK013134: HardCoded Modules : PakingTicket
                try
                {
                    // Override default AppUseParkingTicketModule
                    /* IN009239 */
                    //GlobalFramework.AppUseParkingTicketModule = Convert.ToBoolean(GlobalFramework.Settings["appMultiUserEnvironment"]);
                    CustomAppOperationMode customAppOperationMode = FrameworkUtils.GetCustomAppOperationMode();
                    GlobalFramework.AppUseParkingTicketModule = CustomAppOperationMode.PARKING.Equals(customAppOperationMode);

                    //TK016235 BackOffice - Mode
                    GlobalFramework.AppUseBackOfficeMode = CustomAppOperationMode.BACKOFFICE.Equals(customAppOperationMode);

                    // Init Global Object GlobalApp.ParkingTicket
                    if (GlobalFramework.AppUseParkingTicketModule)
                    {
                        GlobalApp.ParkingTicket = new ParkingTicket();
                    }
                }
                catch (Exception)
                {
                    _log.Error(string.Format("void Init() :: Missing AppUseParkingTicketModule Token in Settings, using default value: [{0}]", GlobalFramework.AppUseParkingTicketModule));
                }

                //Create SystemNotification
                FrameworkUtils.SystemNotification();

                //Clean Documents Folder on New Database, else we have Document files that dont correspond to Database
                if (databaseCreated && Directory.Exists(GlobalFramework.Path["documents"].ToString()))
                {
                    string documentsFolder = GlobalFramework.Path["documents"].ToString();
                    System.IO.DirectoryInfo di = new DirectoryInfo(documentsFolder);
                    if (di.GetFiles().Length > 0)
                    {
                        _log.Debug(string.Format("void Init() :: New database created. Start Delete [{0}] document(s) from [{1}] folder!", di.GetFiles().Length, documentsFolder));
                        foreach (FileInfo file in di.GetFiles())
                        {
                            try
                            {
                                file.Delete();
                            }
                            catch (Exception)
                            {
                                _log.Error(string.Format("void Init() :: Error! Cant delete Document file: [{0}]", file.Name));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("void Init() :: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// It creates automatic backup process and its call to TimerHandler.
        /// 
        /// Please see IN009163 and IN009164
        /// </summary>
        private void InitBackupTimerProcess()
        {
            bool xpoCreateDatabaseAndSchema = SettingsApp.XPOCreateDatabaseAndSchema;
            bool validDirectoryBackup = FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(Convert.ToString(GlobalFramework.Path["backups"])));
            _log.Debug("void InitBackupTimerProcess() :: xpoCreateDatabaseAndSchema [ " + xpoCreateDatabaseAndSchema + " ] :: validDirectoryBackup [ " + validDirectoryBackup + " ]");

            //Show Dialog if Cant Create Backups Directory (Extra Protection for Shared Network Folders)
            if (!validDirectoryBackup)
            {
                ResponseType response = Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_error_create_directory_backups"), Convert.ToString(GlobalFramework.Path["backups"])));
                //Enable Quit After BootStrap, Preventing Application.Run()
                if (response == ResponseType.No) _quitAfterBootStrap = true;
            }

            //Start Database Backup Timer if not create XPO Schema and SoftwareVendor is Active
            if (GlobalFramework.PluginSoftwareVendor != null && validDirectoryBackup && !xpoCreateDatabaseAndSchema)
            {
                /* IN009163 and IN009164 - Opt to auto-backup flow */
                _autoBackupFlowIsEnabled = Boolean.Parse(GlobalFramework.PreferenceParameters["DATABASE_BACKUP_AUTOMATIC_ENABLED"]);

                /* IN009164 */
                if (_autoBackupFlowIsEnabled)
                {
                    /* IN009164 - considering these variables are only used for automatic backup purposes, will be settled only when Auto-Backup Flow is enabled */
                    _backupDatabaseTimeSpan = TimeSpan.Parse(GlobalFramework.PreferenceParameters["DATABASE_BACKUP_TIMESPAN"]);
                    _databaseBackupTimeSpanRangeStart = TimeSpan.Parse(GlobalFramework.PreferenceParameters["DATABASE_BACKUP_TIME_SPAN_RANGE_START"]);
                    _databaseBackupTimeSpanRangeEnd = TimeSpan.Parse(GlobalFramework.PreferenceParameters["DATABASE_BACKUP_TIME_SPAN_RANGE_END"]);
                    /* IN009164 - TimeoutHandler() for UpdateBackupTimer() will not be created if Auto-Backup Flow is enabled */
                    StartBackupTimer();
                }
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private ProtectedFiles InitProtectedFiles()
        {
            bool debug = true;
            ProtectedFiles protectedFiles = null;
            string filePath = SettingsApp.ProtectedFilesFileName;
            List<string> fileList = SettingsApp.ProtectedFilesList;

            //ReCreate File MODE
            if (SettingsApp.ProtectedFilesRecreateCSV)
            {
                protectedFiles = new ProtectedFiles(fileList, filePath);
                string md5FromFile = FrameworkUtils.MD5HashFile(filePath);
                //End Xpo Create Scheme and Fixtures, Terminate App and Request assign False to Developer Vars
                //string message = string.Format(@"ProtectedFiles '{1}' re-created with {2} files found!
                //    {0}- Assign false to 'SettingsApp.ProtectedFilesRecreateCsv'.
                //    {0}- Update logicpos.financial.library SettingsApp.ProtectedFilesFileHash with Hash: '{3}.'"
                //    , Environment.NewLine, filePath, fileList.Count, md5FromFile
                //);
                //_log.Debug(String.Format("Protected files: [{0}]", message));
                string message = string.Format(@"ProtectedFiles '{1}' re-created with {2} files found!{0}{0}Assign false to 'SettingsApp.ProtectedFilesRecreateCsv' and run app again.", Environment.NewLine, filePath, fileList.Count);

                ExportProtectedFiles(fileList);
                Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new System.Drawing.Size(600, 350), MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), message);
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
                    if (debug) _log.Debug(string.Format("Message: [{0}], Valid: [{1}], IsValidFile: [{2}]", item.Key, item.Value.Valid, protectedFiles.IsValidFile(item.Key)));
                }

                List<string> getInvalidAndMissingFiles = protectedFiles.GetInvalidAndMissingFiles(fileList);

                if (getInvalidAndMissingFiles.Count > 0)
                {
                    string filesMessage = string.Empty;
                    for (int i = 0; i < getInvalidAndMissingFiles.Count; i++)
                    {
                        if (debug) _log.Debug(string.Format("InvalidFile: [{0}]", getInvalidAndMissingFiles[i]));
                        filesMessage += string.Format("{0}{1}", getInvalidAndMissingFiles[i], Environment.NewLine);
                    }

                    //If Not IgnoreProtection, show alert and exit
                    if (!SettingsApp.ProtectedFilesIgnoreProtection)
                    {
                        Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(800, 400), MessageType.Error, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_error_protected_files_invalid_files_detected"), filesMessage));
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
            string filename = string.Format("{0}{1}", GlobalFramework.Path["temp"], "protected.zip");

            try
            {
                for (int i = 0; i < pFileList.Count; i++)
                {
                    files[i] = pFileList[i];
                }
                files[pFileList.Count] = SettingsApp.ProtectedFilesFileName;

                //Empty password, to zip without password
                result = Utils.ZipPack(files, filename, string.Empty);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void InitAppMode(AppMode pAppMode)
        {
            //Run in BackOffice Mode
            if (pAppMode == AppMode.Backoffice)
            {
                GlobalApp.WindowBackOffice = new BackOfficeMainWindow();
            }
            //Run in POS Mode
            else
            {
                //Init Theme Object
                _log.Debug("Init Theme Object ");
                var predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "StartupWindow");
                var themeWindow = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);

                //// Inject themeWindow into 
                //GlobalApp.ExpressionEvaluator.Variables.Add("themeWindow", themeWindow);

                try
                {
                    _log.Debug("Init windowImageFileName ");
                    string windowImageFileName = string.Format(themeWindow.Globals.ImageFileName, GlobalApp.ScreenSize.Width, GlobalApp.ScreenSize.Height);
                    _log.Debug("StartupWindow " + windowImageFileName);
                    GlobalApp.WindowStartup = new StartupWindow(windowImageFileName);
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                }
            };
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        // Old Stub used to Init MediaNova Module : Leave it here to House future Modules or Plugins Initialization
        public void InitModules()
        {
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static

        public static void QuitWithoutConfirmation(bool pAudit = true)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            try
            {
                //Audit
                if (pAudit) FrameworkUtils.Audit("APP_CLOSE");
                //Before use DeleteSession()
                /* IN005943 */
                GlobalFramework.SessionApp.CleanSession();
                GlobalFramework.SessionApp.Write();
                //GlobalFramework.SessionApp.DeleteSession();
                //Disconnect SessionXpo
                GlobalFramework.SessionXpo.Disconnect();
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
            ResponseType responseType = Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, new Size(400, 300), MessageType.Question, ButtonsType.YesNo, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_quit_title"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_quit_message"));

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
                GLib.Timeout.Add(SettingsApp.BackupTimerInterval, new GLib.TimeoutHandler(UpdateBackupTimer));
            }
            catch (Exception ex)
            {
                _log.Error("void StartBackupTimer() :: _autoBackupFlowIsActive: [" + _autoBackupFlowIsEnabled + "] :: "  + ex.Message, ex);
            }
        }

        private bool UpdateBackupTimer()
        {
            _log.Debug("bool UpdateBackupTimer()");
            bool debug = false;

            DateTime currentDateTime = FrameworkUtils.CurrentDateTimeAtomic();
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
                    if (debug) _log.Debug(string.Format("Inside of TimeRange: currentDateTime:[{0}], backupLastDateTime:[{1}], timeSpanDiference:[{2}], backupDatabaseTimeSpan:[{3}] ", currentDateTime, currentDateTimeLastBackup, timeSpanDiference, _backupDatabaseTimeSpan));
                }
            }
            else
            {
                if (debug) _log.Debug(string.Format("Outside of TimeRange: [{0}] > [{1}] && [{2}] < [{3}]", currentDateTime.TimeOfDay, _databaseBackupTimeSpanRangeStart, currentDateTime.TimeOfDay, _databaseBackupTimeSpanRangeEnd));
            }

            // Returning true means that the timeout routine should be invoked
            // again after the timeout period expires. Returning false would
            // terminate the timeout.
            return true;
        }
    }
}
