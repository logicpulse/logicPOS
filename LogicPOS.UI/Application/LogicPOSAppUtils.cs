using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos;
using logicpos.Classes.DataLayer;
using logicpos.Classes.Enums.App;
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
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace LogicPOS.UI.Application
{
    internal class LogicPOSAppUtils
    {
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _quitAfterBootStrap = false;
        private bool _autoBackupFlowIsEnabled = false;
        private TimeSpan _backupDatabaseTimeSpan = new TimeSpan();
        private TimeSpan _databaseBackupTimeSpanRangeStart = new TimeSpan();
        private TimeSpan _databaseBackupTimeSpanRangeEnd = new TimeSpan();
        private static bool _needToUpdate = false;

        public void StartApp(AppMode mode)
        {
            try
            {
                Initialize();
                LogicPOSAppContext.DialogThreadNotify?.WakeupMain();
                InitAppMode(mode);

                InitBackupTimerProcess();

                if (!_quitAfterBootStrap) Gtk.Application.Run();
            }
            catch (Exception ex)
            {
                //CustomAlerts.ShowContactSupportErrorAlert(LoginWindow.Instance); ::: DÚVIDA LUCIANO

                var messageDialog = new CustomAlert(LoginWindow.Instance)
                                            .WithMessage(ex.Message)
                                            .ShowAlert();
            }
            finally
            {

                if (LogicPOSAppContext.UsbDisplay != null)
                {
                    LogicPOSAppContext.UsbDisplay.Close();
                }

                if (LogicPOSAppContext.WeighingBalance != null && LogicPOSAppContext.WeighingBalance.IsPortOpen())
                {
                    LogicPOSAppContext.WeighingBalance.ClosePort();
                }
            }
        }

        private void Initialize()
        {
            bool createDatabase = LogicPOSSettings.XPOCreateDatabaseAndSchema;
            bool createDatabaseObjectsWithFixtures = createDatabase;

            AutoCreateOption xpoAutoCreateOption = createDatabase ? AutoCreateOption.DatabaseAndSchema : AutoCreateOption.None;

            if (File.Exists(LogicPOSSettings.LicenceFileName))
            {
                Utils.AssignLicence(LogicPOSSettings.LicenceFileName, true);
            }

            LogicPOSAppContext.MultiUserEnvironment = AppSettings.Instance.appMultiUserEnvironment;
            LogicPOSAppContext.UseVirtualKeyBoard = AppSettings.Instance.useVirtualKeyBoard;

            LogicPOSAppContext.Notifications = new Dictionary<string, bool>
            {
                ["SHOW_PRINTER_UNDEFINED"] = true
            };

            LogicPOSAppContext.OpenFileDialogStartPath = Directory.GetCurrentDirectory();

            DatabaseSettings.DatabaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), AppSettings.Instance.databaseType);

            DatabaseSettings.DatabaseName = AppSettings.Instance.databaseName;

            string connectionString = string.Format(AppSettings.Instance.xpoConnectionString, DatabaseSettings.DatabaseName.ToLower());
            DatabaseSettings.AssignConnectionStringToSettings(connectionString);

            if (LogicPOSSettings.UseProtectedFiles)
            {
                LogicPOSAppContext.ProtectedFiles = InitProtectedFiles();
            }

            bool databaseCreated = false;

            if (createDatabase == false)
            {
                try
                {

                    LogicPOSSettings.FirstBoot = true;
                    databaseCreated = DataLayer.CreateDatabaseSchema(
                        connectionString,
                        DatabaseSettings.DatabaseType,
                        DatabaseSettings.DatabaseName,
                        out _needToUpdate);

                }
                catch (Exception ex)
                {
                    LogicPOSAppContext.DialogThreadNotify.WakeupMain();

                    var messageDialog = new CustomAlert(LoginWindow.Instance)
                    .WithMessage(ex.Message)
                    .WithSize(new Size(900,700))
                    .WithMessageType(MessageType.Error)
                    .WithButtonsType(ButtonsType.Ok)
                    .WithTitleResource("global_error")
                    .ShowAlert();

                    Environment.Exit(0);
                }
            }
            LogicPOSSettings.FirstBoot = false;
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
                LogicPOSAppContext.DialogThreadNotify.WakeupMain();

                var messageDialog = new CustomAlert(LoginWindow.Instance)
                                    .WithMessage(ex.Message)
                                    .WithSize(new Size(900, 700))
                                    .WithMessageType(MessageType.Error)
                                    .WithButtonsType(ButtonsType.Ok)
                                    .WithTitleResource("global_error")
                                    .ShowAlert();

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
                    LogicPOSAppContext.DialogThreadNotify.WakeupMain();

                    string endMessage = "Invalid database Schema! Fix database Schema and Try Again!";

                    var messageDialog = new CustomAlert(LoginWindow.Instance)
                                            .WithMessage(string.Format(endMessage, Environment.NewLine))
                                            .WithSize(new Size(500, 300))
                                            .WithMessageType(MessageType.Error)
                                            .WithButtonsType(ButtonsType.Ok)
                                            .WithTitleResource("global_error")
                                            .ShowAlert();

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
                LogicPOSAppContext.DialogThreadNotify.WakeupMain();

                string endMessage = "Xpo Create Schema and Fixtures Done!{0}Please assign false to 'xpoCreateDatabaseAndSchema' and 'xpoCreateDatabaseObjectsWithFixtures' and run App again";
                _logger.Debug(string.Format("void Init() :: xpoCreateDatabaseAndSchema: {0}", endMessage));
               
                var messageDialog = new CustomAlert(LoginWindow.Instance)
                    .WithMessage(string.Format(endMessage, Environment.NewLine))
                    .WithSize(new Size(500, 300))
                    .WithMessageType(MessageType.Info)
                    .WithButtonsType(ButtonsType.Ok)
                    .WithTitleResource("global_information")
                    .ShowAlert();

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


            CultureSettings.CurrentCulture = CultureSettings.CurrentCulture = new CultureInfo(ConfigurationManager.AppSettings["customCultureResourceDefinition"]);

            _logger.Debug(string.Format("CUSTOM CULTURE :: CurrentUICulture '{0}' in use.", CultureInfo.CurrentUICulture));

            CultureSettings.CurrentCultureNumberFormat = CultureInfo.GetCultureInfo(LogicPOSSettings.CultureNumberFormat);

            //Init AppSession
            string appSessionFile = Utils.GetSessionFileName();
            if (databaseCreated && File.Exists(appSessionFile)) File.Delete(appSessionFile);
            POSSession.CurrentSession = POSSession.GetSessionFromFile(appSessionFile);

            //Try to Get open Session Day/Terminal for this Terminal
            XPOSettings.WorkSessionPeriodDay = WorkSessionProcessor.GetSessionPeriod(WorkSessionPeriodType.Day);
            XPOSettings.WorkSessionPeriodTerminal = WorkSessionProcessor.GetSessionPeriod(WorkSessionPeriodType.Terminal);

            //Use Detected ScreenSize
            var appScreenSize = AppSettings.Instance.appScreenSize;

            if (appScreenSize == new Size(0, 0))
            {

                LogicPOSAppContext.ScreenSize = Utils.GetThemeScreenSize();
            }
            else
            {

                LogicPOSAppContext.ScreenSize = Utils.GetThemeScreenSize(appScreenSize);
            }

            LogicPOSAppContext.ExpressionEvaluator.EvaluateFunction += ExpressionEvaluatorExtended.ExpressionEvaluator_EvaluateFunction;

            ExpressionEvaluatorExtended.InitVariablesStartupWindow();
            ExpressionEvaluatorExtended.InitVariablesPosMainWindow();

            LogicPOSAppContext.MaxWindowSize = new Size(LogicPOSAppContext.ScreenSize.Width - 40, LogicPOSAppContext.ScreenSize.Height - 40);
            LogicPOSAppContext.ExpressionEvaluator.Variables.Add("globalScreenSize", LogicPOSAppContext.ScreenSize);
            GeneralSettings.ScreenSize = LogicPOSAppContext.ScreenSize;

            try
            {
                LogicPOSAppContext.Theme = XmlToObjectParser.ParseFromFile(LogicPOSSettings.FileTheme);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(LogicPOSAppContext.Theme);

            }
            catch (Exception ex)
            {
                LogicPOSAppContext.DialogThreadNotify.WakeupMain();

                var messageDialog = new CustomAlert(LoginWindow.Instance)
                        .WithMessage(ex.Message)
                        .ShowAlert();

               // CustomAlerts.ShowThemeRenderingErrorAlert(ex.Message,LoginWindow.Instance);
            }

            FastReportUtils.InitializeFastReports(LogicPOSSettings.AppName);

            if (TerminalSettings.LoggedTerminal.PoleDisplay != null)
            {
                LogicPOSAppContext.UsbDisplay = UsbDisplayDevice.InitDisplay();
                LogicPOSAppContext.UsbDisplay.WriteCentered(string.Format("{0} {1}", LogicPOSSettings.AppName, GeneralSettings.ProductVersion), 1);
                LogicPOSAppContext.UsbDisplay.WriteCentered(LogicPOSSettings.AppUrl, 2);
                LogicPOSAppContext.UsbDisplay.EnableStandBy();
            }

            if (TerminalSettings.LoggedTerminal.BarcodeReader != null)
            {
                LogicPOSAppContext.BarCodeReader = new InputReader();
            }

            if (TerminalSettings.LoggedTerminal.WeighingMachine != null)
            {

                if (TerminalSettings.LoggedTerminal.WeighingMachine.PortName == TerminalSettings.LoggedTerminal.PoleDisplay.COM)
                {
                    _logger.Debug(string.Format("Port " + TerminalSettings.LoggedTerminal.WeighingMachine.PortName + "Already taken by pole display"));
                }
                else
                {
                    if (Utils.IsPortOpen(TerminalSettings.LoggedTerminal.WeighingMachine.PortName))
                    {
                        LogicPOSAppContext.WeighingBalance = new WeighingBalance(TerminalSettings.LoggedTerminal.WeighingMachine);
                    }

                }

            }

            XPOUtility.Audit("APP_START", string.Format("{0} {1} clr {2}", LogicPOSSettings.AppName, GeneralSettings.ProductVersion, GeneralSettings.ProductAssembly.ImageRuntimeVersion));
            if (databaseCreated) XPOUtility.Audit("DATABASE_CREATE");

            // Plugin Errors Messages
            if (PluginSettings.HasSoftwareVendorPlugin == false ||
                PluginSettings.SoftwareVendor.IsValidSecretKey(PluginSettings.SecretKey) == false)
            {
                /* IN009034 */
                LogicPOSAppContext.DialogThreadNotify?.WakeupMain();

                _logger.Debug(string.Format("void Init() :: Wrong key detected [{0}]. Use a valid LogicposFinantialLibrary with same key as SoftwareVendorPlugin", PluginSettings.SecretKey));

                var messageDialog = new CustomAlert(LoginWindow.Instance)
                        .WithMessageResource("dialog_message_error_plugin_softwarevendor_not_registered")
                        .WithSize(new Size(650, 380))
                        .WithMessageType(MessageType.Error)
                        .WithButtonsType(ButtonsType.Ok)
                        .WithTitleResource("global_error")
                        .ShowAlert();
            }

            try
            {
                CustomAppOperationMode customAppOperationMode = AppOperationModeSettings.GetCustomAppOperationMode();
                GeneralSettings.AppUseParkingTicketModule = CustomAppOperationMode.PARKING.Equals(customAppOperationMode);

                GeneralSettings.AppUseBackOfficeMode = CustomAppOperationMode.BACKOFFICE.Equals(customAppOperationMode);

                if (GeneralSettings.AppUseParkingTicketModule)
                {
                    LogicPOSAppContext.ParkingTicket = new ParkingTicket();
                }
            }
            catch (Exception)
            {
                _logger.Error(string.Format("void Init() :: Missing AppUseParkingTicketModule Token in Settings, using default value: [{0}]", GeneralSettings.AppUseParkingTicketModule));
            }

            XPOUtility.SystemNotification();
#if DEBUG
            LicenseSettings.LicenseModuleStocks = true;
            PluginSettings.AppCompanyName = LicenseSettings.LicenseCompany = LicenseSettings.LicenseReseller = "Logicpulse";
#endif

            if (databaseCreated && Directory.Exists(PathsSettings.Paths["documents"].ToString()))
            {
                string documentsFolder = PathsSettings.Paths["documents"].ToString();
                DirectoryInfo di = new DirectoryInfo(documentsFolder);
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

        private void InitBackupTimerProcess()
        {
            bool xpoCreateDatabaseAndSchema = LogicPOSSettings.XPOCreateDatabaseAndSchema;
            Directory.CreateDirectory(PathsSettings.BackupsFolderLocation);
            bool backupsFolderExists = Directory.Exists(PathsSettings.BackupsFolderLocation);

            if (backupsFolderExists == false)
            {
                ResponseType response = new CustomAlert(LoginWindow.Instance)
                                        .WithMessageResource(string.Format(GeneralUtils.GetResourceByName("dialog_message_error_create_directory_backups"), PathsSettings.BackupsFolderLocation))
                                        .WithMessageType(MessageType.Question)
                                        .WithButtonsType(ButtonsType.YesNo)
                                        .WithTitleResource("global_error")
                                        .ShowAlert();

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

        private ProtectedFiles InitProtectedFiles()
        {
            bool debug = true;
            string filePath = LogicPOSSettings.ProtectedFilesFileName;
            List<string> fileList = LogicPOSSettings.ProtectedFilesList;

            ProtectedFiles protectedFiles;
            //ReCreate File MODE
            if (LogicPOSSettings.ProtectedFilesRecreateCSV)
            {
                protectedFiles = new ProtectedFiles(fileList, filePath);
                string md5FromFile = CryptographyUtils.MD5HashFile(filePath);

                string message = string.Format(@"ProtectedFiles '{1}' re-created with {2} files found!{0}{0}Assign false to 'SettingsApp.ProtectedFilesRecreateCsv' and run app again.", Environment.NewLine, filePath, fileList.Count);

                ExportProtectedFiles(fileList);
     
                var messageDialog = new CustomAlert(LoginWindow.Instance)
                                    .WithMessage(message)
                                    .WithSize(new Size(600, 350))
                                    .WithMessageType(MessageType.Info)
                                    .WithButtonsType(ButtonsType.Ok)
                                    .WithTitleResource("global_information")
                                    .ShowAlert();
                
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
                    if (!LogicPOSSettings.ProtectedFilesIgnoreProtection)
                    {
                        /*Utils.ShowMessageBox(   DUVÍDA ::: LUCIANO
                            LoginWindow.Instance,
                            DialogFlags.Modal,
                            new Size(800, 400),
                            MessageType.Error,
                            ButtonsType.Close,
                            CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName,
                            "global_error"), string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName,
                            "dialog_message_error_protected_files_invalid_files_detected"),
                            filesMessage));*/

                        var messageDialog = new CustomAlert(LoginWindow.Instance)
                                            .WithMessage(filesMessage)
                                            .WithMessageResource(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName,
                                                                                "dialog_message_error_protected_files_invalid_files_detected"))
                                            .WithSize(new Size(800, 400))
                                            .WithMessageType(MessageType.Error)
                                            .WithButtonsType(ButtonsType.Close)
                                            .WithTitleResource("global_error")
                                            .ShowAlert();


                        Environment.Exit(0);
                    }
                }
            }

            return protectedFiles;
        }

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
                files[pFileList.Count] = LogicPOSSettings.ProtectedFilesFileName;

                //Empty password, to zip without password
                result = CompressionUtils.ZipPack(files, filename, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        private void InitAppMode(AppMode pAppMode)
        {
            if (pAppMode == AppMode.Backoffice)
            {
                LogicPOSAppContext.BackOffice = new BackOfficeWindow();
            }
            else
            {
                _logger.Debug("Init Theme Object ");
                var predicate = (Predicate<dynamic>)((x) => x.ID == "StartupWindow");
                var themeWindow = LogicPOSAppContext.Theme.Theme.Frontoffice.Window.Find(predicate);

                _logger.Debug("Init windowImageFileName ");
                string windowImageFileName = string.Format(themeWindow.Globals.ImageFileName, LogicPOSAppContext.ScreenSize.Width, LogicPOSAppContext.ScreenSize.Height);
                _logger.Debug("StartupWindow " + windowImageFileName);
                LoginWindow.Instance = new LoginWindow(windowImageFileName, _needToUpdate);

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

            Gtk.Application.Quit();
        }

        public static void Quit(Window parentWindow)
        {
            ResponseType responseType = new CustomAlert(parentWindow)
                                            .WithMessageResource(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quit_message"))
                                            .WithSize(new Size(400, 300))
                                            .WithMessageType(MessageType.Question)
                                            .WithButtonsType(ButtonsType.YesNo)
                                            .WithTitleResource("global_quit_title")
                                            .ShowAlert();


            if (responseType == ResponseType.Yes)
            {
                QuitWithoutConfirmation();
            }
        }

        private void StartBackupTimer()
        {
            try
            {
                GLib.Timeout.Add(LogicPOSSettings.BackupTimerInterval, new GLib.TimeoutHandler(UpdateBackupTimer));
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
