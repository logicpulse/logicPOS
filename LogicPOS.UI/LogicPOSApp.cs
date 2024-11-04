using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.DataLayer;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Logic.Hardware;
using logicpos.Classes.Logic.Others;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Reporting.Utility;
using LogicPOS.Settings;
using LogicPOS.Settings.Enums;
using LogicPOS.Shared;
using LogicPOS.UI;
using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;

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
        private static bool _needToUpdate = false;

        public void StartApp(AppMode pMode)
        {
            try
            {
                Init();
                GlobalApp.DialogThreadNotify?.WakeupMain();
                InitAppMode(pMode);

                InitBackupTimerProcess();

                if (!_quitAfterBootStrap) Application.Run();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                Utils.ShowMessageBox(
                    GlobalApp.StartupWindow,
                    DialogFlags.Modal,
                    new Size(500, 240),
                    MessageType.Error,
                    ButtonsType.Ok,
                    CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName,
                    "global_error"),
                    CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName,
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

            bool createDatabase = POSSettings.XPOCreateDatabaseAndSchema;
            bool createDatabaseObjectsWithFixtures = createDatabase;
       
            AutoCreateOption xpoAutoCreateOption = (createDatabase) ? AutoCreateOption.DatabaseAndSchema : AutoCreateOption.None;

            if (File.Exists(POSSettings.LicenceFileName))
            {
                Utils.AssignLicence(POSSettings.LicenceFileName, true);
            }

            GlobalApp.MultiUserEnvironment = AppSettings.Instance.appMultiUserEnvironment;
            GlobalApp.UseVirtualKeyBoard = AppSettings.Instance.useVirtualKeyBoard;

            GlobalApp.Notifications = new Dictionary<string, bool>
            {
                ["SHOW_PRINTER_UNDEFINED"] = true
            };

            GlobalApp.OpenFileDialogStartPath = Directory.GetCurrentDirectory();
    
            DatabaseSettings.DatabaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), AppSettings.Instance.databaseType);
           
            DatabaseSettings.DatabaseName = AppSettings.Instance.databaseName;
       
            string connectionString = string.Format(AppSettings.Instance.xpoConnectionString, DatabaseSettings.DatabaseName.ToLower());
            DatabaseSettings.AssignConnectionStringToSettings(connectionString);

            if (POSSettings.UseProtectedFiles)
            {
                GlobalApp.ProtectedFiles = InitProtectedFiles();
            }

            bool databaseCreated = false;

            if (createDatabase == false)
            {
                try
                {
                 
                    POSSettings.FirstBoot = true;
                    databaseCreated = DataLayer.CreateDatabaseSchema(
                        connectionString,
                        DatabaseSettings.DatabaseType,
                        DatabaseSettings.DatabaseName,
                        out _needToUpdate);

                }
                catch (Exception ex)
                {
                    GlobalApp.DialogThreadNotify.WakeupMain();

                    Utils.ShowMessageBox(null,
                                         DialogFlags.Modal,
                                         new Size(900, 700),
                                         MessageType.Error,
                                         ButtonsType.Ok,
                                         GeneralUtils.GetResourceByName("global_error"),
                                         ex.Message);
                    Environment.Exit(0);
                }
            }
            POSSettings.FirstBoot = false;
            //Init XPO Connector DataLayer
            try
            {
                /* IN007011 */
                var connectionStringBuilder = new System.Data.Common.DbConnectionStringBuilder()
                { ConnectionString = connectionString };
                if (connectionStringBuilder.ContainsKey("password")) { connectionStringBuilder["password"] = "*****"; };
                _logger.Debug(string.Format("void Init() :: Init XpoDefault.DataLayer: [{0}]", connectionStringBuilder.ToString()));

                XpoDefault.DataLayer = XpoDefault.GetDataLayer(connectionString, xpoAutoCreateOption);
                XPOSettings.Session = new Session(XpoDefault.DataLayer) { LockingOption = LockingOption.None };
            }
            catch (Exception ex)
            {
                _logger.Error("void Init() :: Init XpoDefault.DataLayer: " + ex.Message, ex);

                /* IN009034 */
                GlobalApp.DialogThreadNotify.WakeupMain();

                Utils.ShowMessageBox(GlobalApp.StartupWindow, DialogFlags.Modal, new Size(900, 700), MessageType.Error, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_error"), ex.Message);
                throw; // TO DO
            }

            //Check Valid Database Scheme
            if (!createDatabase && !GeneralUtils.IsRunningOnMono)
            {
                bool isSchemaValid = DataLayer.IsSchemaValid(connectionString);
                _logger.Debug(string.Format("void Init() :: Check if Database Scheme: isSchemaValid : [{0}]", isSchemaValid));
                if (!isSchemaValid)
                {
                    /* IN009034 */
                    GlobalApp.DialogThreadNotify.WakeupMain();

                    string endMessage = "Invalid database Schema! Fix database Schema and Try Again!";
                    Utils.ShowMessageBox(GlobalApp.StartupWindow, DialogFlags.Modal, new Size(500, 300), MessageType.Error, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_error"), string.Format(endMessage, Environment.NewLine));
                    Environment.Exit(0);
                }
            }

            //If not in Xpo create database Scheme Mode, Get Terminal from Db
            if (createDatabase == false)
            {
                TerminalSettings.LoggedTerminal = Utils.GetOrCreateTerminal();
            }

            //After Assigned LoggedUser
            if (createDatabaseObjectsWithFixtures)
            {
                InitFixtures.InitUserAndTerminal(XPOSettings.Session);
                InitFixtures.InitOther(XPOSettings.Session);
                InitFixtures.InitDocumentFinance(XPOSettings.Session);
                InitFixtures.InitWorkSession(XPOSettings.Session);
            }

            //End Xpo Create Scheme and Fixtures, Terminate App and Request assign False to Developer Vars
            if (createDatabase)
            {
                /* IN009034 */
                GlobalApp.DialogThreadNotify.WakeupMain();

                string endMessage = "Xpo Create Schema and Fixtures Done!{0}Please assign false to 'xpoCreateDatabaseAndSchema' and 'xpoCreateDatabaseObjectsWithFixtures' and run App again";
                _logger.Debug(string.Format("void Init() :: xpoCreateDatabaseAndSchema: {0}", endMessage));

                Utils.ShowMessageBox(GlobalApp.StartupWindow, DialogFlags.Modal, new Size(500, 300), MessageType.Info, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_information"), string.Format(endMessage, Environment.NewLine));
                Environment.Exit(0);
            }

            //Init PreferenceParameters
            GeneralSettings.PreferenceParameters = XPOUtility.GetPreferencesParameters();
            //Init Preferences Path
            PathsSettings.InitializePreferencesPaths();

            //CultureInfo/Localization
            string culture = GeneralSettings.PreferenceParameters["CULTURE"];

            /* IN008013 */
            if (string.IsNullOrEmpty(culture))
            {
                culture = CultureSettings.CurrentCultureName;
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
            POSSession.CurrentSession = POSSession.GetSessionFromFile(appSessionFile);

            //Try to Get open Session Day/Terminal for this Terminal
            XPOSettings.WorkSessionPeriodDay = WorkSessionProcessor.GetSessionPeriod(WorkSessionPeriodType.Day);
            XPOSettings.WorkSessionPeriodTerminal = WorkSessionProcessor.GetSessionPeriod(WorkSessionPeriodType.Terminal);

            //Use Detected ScreenSize
            var appScreenSize = AppSettings.Instance.appScreenSize;

            if (appScreenSize == new Size(0,0))
            {

                GlobalApp.ScreenSize = Utils.GetThemeScreenSize();
            }
            else
            {

                GlobalApp.ScreenSize = Utils.GetThemeScreenSize(appScreenSize);
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
            GeneralSettings.ScreenSize = GlobalApp.ScreenSize;
            //Parse and store Theme in Singleton
            try
            {
                GlobalApp.Theme = XmlToObjectParser.ParseFromFile(POSSettings.FileTheme);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(GlobalApp.Theme);

            }
            catch (Exception ex)
            {
                /* IN009034 */
                GlobalApp.DialogThreadNotify.WakeupMain();

                _logger.Debug("void Init() :: XmlToObjectParser.ParseFromFile(SettingsApp.FileTheme) :: " + ex);
                Utils.ShowMessageTouchErrorRenderTheme(GlobalApp.StartupWindow, ex.Message);
            }

            //Init FastReports Custom Functions and Custom Vars
            FastReportUtils.InitializeFastReports(POSSettings.AppName);

            //Hardware : Init Display
            if (TerminalSettings.LoggedTerminal.PoleDisplay != null)
            {
                GlobalApp.UsbDisplay = (UsbDisplayDevice)UsbDisplayDevice.InitDisplay();
                GlobalApp.UsbDisplay.WriteCentered(string.Format("{0} {1}", POSSettings.AppName, GeneralSettings.ProductVersion), 1);
                GlobalApp.UsbDisplay.WriteCentered(POSSettings.AppUrl, 2);
                GlobalApp.UsbDisplay.EnableStandBy();
            }

            //Hardware : Init BarCodeReader 
            if (TerminalSettings.LoggedTerminal.BarcodeReader != null)
            {
                GlobalApp.BarCodeReader = new InputReader();
            }

            //Hardware : Init WeighingBalance
            if (TerminalSettings.LoggedTerminal.WeighingMachine != null)
            {
                //Protecções de integridade das BD's [IN:013327]
                //Check if port is used by pole display
                if (TerminalSettings.LoggedTerminal.WeighingMachine.PortName == TerminalSettings.LoggedTerminal.PoleDisplay.COM)
                {
                    _logger.Debug(string.Format("Port " + TerminalSettings.LoggedTerminal.WeighingMachine.PortName + "Already taken by pole display"));
                }
                else
                {
                    if (Utils.IsPortOpen(TerminalSettings.LoggedTerminal.WeighingMachine.PortName))
                    {
                        GlobalApp.WeighingBalance = new WeighingBalance(TerminalSettings.LoggedTerminal.WeighingMachine);
                        //_logger.Debug(string.Format("IsPortOpen: [{0}]", GlobalApp.WeighingBalance.IsPortOpen())); }
                    }

                }

            }

            //Send To Log
            _logger.Debug(string.Format("void Init() :: ProductVersion: [{0}], ImageRuntimeVersion: [{1}], IsLicensed: [{2}]", GeneralSettings.ProductVersion, GeneralSettings.ProductAssembly.ImageRuntimeVersion, LicenseSettings.LicenceRegistered));

            //Audit
            XPOUtility.Audit("APP_START", string.Format("{0} {1} clr {2}", POSSettings.AppName, GeneralSettings.ProductVersion, GeneralSettings.ProductAssembly.ImageRuntimeVersion));
            if (databaseCreated) XPOUtility.Audit("DATABASE_CREATE");

            // Plugin Errors Messages
            if (PluginSettings.HasSoftwareVendorPlugin == false ||
                PluginSettings.SoftwareVendor.IsValidSecretKey(PluginSettings.SecretKey) == false)
            {
                /* IN009034 */
                GlobalApp.DialogThreadNotify.WakeupMain();

                _logger.Debug(string.Format("void Init() :: Wrong key detected [{0}]. Use a valid LogicposFinantialLibrary with same key as SoftwareVendorPlugin", PluginSettings.SecretKey));
                Utils.ShowMessageBox(
                    GlobalApp.StartupWindow,
                    DialogFlags.Modal,
                    new Size(650, 380),
                    MessageType.Error,
                    ButtonsType.Ok,
                    CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName,
                    "global_error"),
                    CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName,
                    "dialog_message_error_plugin_softwarevendor_not_registered"));
            }

            // TK013134: HardCoded Modules : PakingTicket
            try
            {
                CustomAppOperationMode customAppOperationMode = AppOperationModeSettings.GetCustomAppOperationMode();
                GeneralSettings.AppUseParkingTicketModule = CustomAppOperationMode.PARKING.Equals(customAppOperationMode);

                //TK016235 BackOffice - Mode
                GeneralSettings.AppUseBackOfficeMode = CustomAppOperationMode.BACKOFFICE.Equals(customAppOperationMode);

                // Init Global Object GlobalApp.ParkingTicket
                if (GeneralSettings.AppUseParkingTicketModule)
                {
                    GlobalApp.ParkingTicket = new ParkingTicket();
                }
            }
            catch (Exception)
            {
                _logger.Error(string.Format("void Init() :: Missing AppUseParkingTicketModule Token in Settings, using default value: [{0}]", GeneralSettings.AppUseParkingTicketModule));
            }


            //Create SystemNotification
            XPOUtility.SystemNotification();

            //Activate stock module for debug
#if DEBUG
            LicenseSettings.LicenseModuleStocks = true;
            PluginSettings.AppCompanyName = LicenseSettings.LicenseCompany = LicenseSettings.LicenseReseller = "Logicpulse";
#endif

            //Clean Documents Folder on New Database, else we have Document files that dont correspond to Database
            if (databaseCreated && Directory.Exists(PathsSettings.Paths["documents"].ToString()))
            {
                string documentsFolder = PathsSettings.Paths["documents"].ToString();
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

        /// <summary>
        /// It creates automatic backup process and its call to TimerHandler.
        /// 
        /// Please see IN009163 and IN009164
        /// </summary>
        private void InitBackupTimerProcess()
        {
            bool xpoCreateDatabaseAndSchema = POSSettings.XPOCreateDatabaseAndSchema;
            Directory.CreateDirectory(PathsSettings.BackupsFolderLocation);
            bool backupsFolderExists = Directory.Exists(PathsSettings.BackupsFolderLocation);

            if (backupsFolderExists == false)
            {
                ResponseType response = Utils.ShowMessageTouch(GlobalApp.StartupWindow, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, GeneralUtils.GetResourceByName("global_error"), string.Format(GeneralUtils.GetResourceByName("dialog_message_error_create_directory_backups"), PathsSettings.BackupsFolderLocation));
                //Enable Quit After BootStrap, Preventing Application.Run()
                if (response == ResponseType.No) _quitAfterBootStrap = true;
            }

            //Start Database Backup Timer if not create XPO Schema and SoftwareVendor is Active
            if (PluginSettings.HasSoftwareVendorPlugin && backupsFolderExists && xpoCreateDatabaseAndSchema == false)
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
                string md5FromFile = CryptographyUtils.MD5HashFile(filePath);
               
                string message = string.Format(@"ProtectedFiles '{1}' re-created with {2} files found!{0}{0}Assign false to 'SettingsApp.ProtectedFilesRecreateCsv' and run app again.", Environment.NewLine, filePath, fileList.Count);

                ExportProtectedFiles(fileList);
                Utils.ShowMessageBox(GlobalApp.StartupWindow, DialogFlags.Modal, new System.Drawing.Size(600, 350), MessageType.Info, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_information"), message);
                Environment.Exit(0);
            }
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
                        Utils.ShowMessageBox(
                            GlobalApp.StartupWindow,
                            DialogFlags.Modal,
                            new Size(800, 400),
                            MessageType.Error,
                            ButtonsType.Close,
                            CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName,
                            "global_error"), string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName,
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
            string filename = string.Format("{0}{1}", PathsSettings.TempFolderLocation, "protected.zip");

            try
            {
                for (int i = 0; i < pFileList.Count; i++)
                {
                    files[i] = pFileList[i];
                }
                files[pFileList.Count] = POSSettings.ProtectedFilesFileName;

                //Empty password, to zip without password
                result = CompressionUtils.ZipPack(files, filename, string.Empty);
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
                GlobalApp.BackOffice = new BackOfficeWindow();
            }
            //Run in POS Mode
            else
            {
                //Init Theme Object
                _logger.Debug("Init Theme Object ");
                var predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "StartupWindow");
                var themeWindow = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);

                _logger.Debug("Init windowImageFileName ");
                string windowImageFileName = string.Format(themeWindow.Globals.ImageFileName, GlobalApp.ScreenSize.Width, GlobalApp.ScreenSize.Height);
                _logger.Debug("StartupWindow " + windowImageFileName);
                GlobalApp.StartupWindow = new StartupWindow(windowImageFileName, _needToUpdate);

            };
        }


        public static void QuitWithoutConfirmation(bool pAudit = true)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            try
            {
                if (pAudit) XPOUtility.Audit("APP_CLOSE");

                POSSession.CurrentSession.CleanSession();
                POSSession.CurrentSession.Save();

                XPOSettings.Session.Disconnect();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            Application.Quit();
            //Environment.Exit(0);
        }

        public static void Quit(Window parentWindow)
        {
            ResponseType responseType = Utils.ShowMessageBox(
                parentWindow,
                DialogFlags.Modal,
                new Size(400, 300),
                MessageType.Question,
                ButtonsType.YesNo,
                CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName,
                "global_quit_title"),
                CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName,
                "global_quit_message"));

            if (responseType == ResponseType.Yes)
            {
                QuitWithoutConfirmation();
            }
        }

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

            DateTime currentDateTime = XPOUtility.CurrentDateTimeAtomic();
            DateTime currentDateTimeLastBackup = DataBaseBackup.GetLastBackupDate();
            TimeSpan timeSpanDiference = currentDateTime - currentDateTimeLastBackup;

            if (currentDateTime.TimeOfDay > _databaseBackupTimeSpanRangeStart && currentDateTime.TimeOfDay < _databaseBackupTimeSpanRangeEnd)
            {
                if (timeSpanDiference >= _backupDatabaseTimeSpan)
                {
                    DataBaseBackup.Backup(null);
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

            return true;
        }
    }
}
