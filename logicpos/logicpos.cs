using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.Classes.Reports;
using logicpos.financial.library.Classes.Utils;
using logicpos.financial.library.Classes.WorkSession;
using logicpos.Classes.DataLayer;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Logic.Hardware;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using logicpos.Classes.Enums.App;

namespace logicpos
{
    class LogicPos
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //BootStrap
        private bool _quitAfterBootStrap = false;
        //Days, hours, minutes, seconds, milliseconds
        private TimeSpan _backupDatabaseTimeSpan = new TimeSpan();
        private TimeSpan _databaseBackupTimeSpanRangeStart = new TimeSpan();
        private TimeSpan _databaseBackupTimeSpanRangeEnd = new TimeSpan();

        public void StartApp(AppMode pMode)
        {
            try
            {
                Init();
                InitAppMode(pMode);
                // Old Stub used to Init MediaNova Module
                InitModules();
                //Check if user cancel App Run on BootStrap and Launch Quit
                if (!_quitAfterBootStrap) Application.Run();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(500, 240), MessageType.Error, ButtonsType.Ok, Resx.global_error, Resx.app_error_contact_support);
            }
            finally
            {
                //Always Close Display Device
                if (GlobalApp.HWUsbDisplay != null) GlobalApp.HWUsbDisplay.Close();
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

                //CultureInfo/Localization
                if (GlobalFramework.Settings["culture"] != null)
                {
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(GlobalFramework.Settings["culture"]);
                }
                GlobalFramework.CurrentCulture = CultureInfo.CurrentUICulture;

                //Always use en-US NumberFormat because of mySql Requirements
                GlobalFramework.CurrentCultureNumberFormat = CultureInfo.GetCultureInfo(SettingsApp.CultureNumberFormat);

                //Other Global App Settings
                GlobalApp.MultiUserEnvironment = Convert.ToBoolean(GlobalFramework.Settings["appMultiUserEnvironment"]);
                GlobalApp.UseVirtualKeyBoard = Convert.ToBoolean(GlobalFramework.Settings["useVirtualKeyBoard"]);

                //Init App Notifications
                GlobalApp.Notifications = new System.Collections.Generic.Dictionary<string, bool>();
                GlobalApp.Notifications["SHOW_PRINTER_UNDEFINED"] = true;

                //System
                GlobalApp.FilePickerStartPath = System.IO.Directory.GetCurrentDirectory();

                bool validDirectoryBackup = FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(Convert.ToString(GlobalFramework.Path["backups"])));
                //Show Dialog if Cant Create Backups Directory (Extra Protection for Shared Network Folders)
                if (!validDirectoryBackup)
                {
                    ResponseType response = Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, Resx.global_error, string.Format(Resx.dialog_message_error_create_directory_backups, Convert.ToString(GlobalFramework.Path["backups"])));
                    //Enable Quit After BootStrap, Preventing Application.Run()
                    if (response == ResponseType.No) _quitAfterBootStrap = true;
                }

                //Get DataBase Details
                GlobalFramework.DatabaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), GlobalFramework.Settings["databaseType"]);
                GlobalFramework.DatabaseName = SettingsApp.DatabaseName;
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
                        databaseCreated = DataLayer.CreateDatabaseSchema(xpoConnectionString, GlobalFramework.DatabaseType, GlobalFramework.DatabaseName);
                    }
                    catch (Exception ex)
                    {
                        //Extra protection to prevent goes to login without a valid connection
                        _log.Error(ex.Message, ex);
                        Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(900, 700), MessageType.Error, ButtonsType.Ok, Resx.global_error, ex.Message);
                        Environment.Exit(0);
                    }
                }

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
                    Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(900, 700), MessageType.Error, ButtonsType.Ok, Resx.global_error, ex.Message);
                    throw;
                }

                //Check Valid Database Scheme
                if (!xpoCreateDatabaseAndSchema && !FrameworkUtils.IsRunningOnMono())
                {
                    bool isSchemaValid = DataLayer.IsSchemaValid(xpoConnectionString);
                    _log.Info(string.Format("Check if Database Scheme: isSchemaValid : [{0}]", isSchemaValid));
                    if (!isSchemaValid)
                    {
                        string endMessage = "Invalid database Schema! Fix database Schema and Try Again!";
                        Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(500, 300), MessageType.Error, ButtonsType.Ok, Resx.global_error, string.Format(endMessage, Environment.NewLine));
                        Environment.Exit(0);
                    }
                }

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
                    string endMessage = "Xpo Create Schema and Fixtures Done!{0}Please assign false to 'xpoCreateDatabaseAndSchema' and 'xpoCreateDatabaseObjectsWithFixtures' and run App again";
                    Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(500, 300), MessageType.Info, ButtonsType.Ok, Resx.global_information, string.Format(endMessage, Environment.NewLine));
                    Environment.Exit(0);
                }

                //PreferenceParameters
                GlobalFramework.PreferenceParameters = FrameworkUtils.GetPreferencesParameters();

                //Init AppSession
                string appSessionFile = Utils.GetSessionFileName();
                if (databaseCreated && File.Exists(appSessionFile)) File.Delete(appSessionFile);
                GlobalFramework.SessionApp = GlobalFrameworkSession.InitSession(appSessionFile);

                //Try to Get open Session Day/Terminal for this Terminal
                GlobalFramework.WorkSessionPeriodDay = ProcessWorkSessionPeriod.GetSessionPeriod(WorkSessionPeriodType.Day);
                GlobalFramework.WorkSessionPeriodTerminal = ProcessWorkSessionPeriod.GetSessionPeriod(WorkSessionPeriodType.Terminal);

                //Theme
                GlobalApp.ScreenSize = Utils.StringToSize(GlobalFramework.Settings["appScreenSize"]);
                GlobalApp.MaxWindowSize = new Size(GlobalApp.ScreenSize.Width - 20, GlobalApp.ScreenSize.Height - 20);
                try
                {
                    GlobalApp.Theme = XmlToObjectParser.ParseFromFile(SettingsApp.FileTheme);
                }
                catch (Exception ex)
                {
                    Utils.ShowMessageTouchErrorRenderTheme(GlobalApp.WindowStartup, ex.Message);
                }

                //Init FastReports Custom Functions and Custom Vars
                CustomFunctions.Register(SettingsApp.AppName);

                //Init Display
                bool hardwareDisplayEnabled = Convert.ToBoolean(GlobalFramework.Settings["hardwareDisplayEnabled"]);
                if (hardwareDisplayEnabled)
                {
                    GlobalApp.HWUsbDisplay = (UsbDisplayDevice)UsbDisplayDevice.InitDisplay();
                    GlobalApp.HWUsbDisplay.WriteCentered(string.Format("{0} {1}", SettingsApp.AppName, FrameworkUtils.ProductVersion), 1);
                    GlobalApp.HWUsbDisplay.WriteCentered(SettingsApp.AppUrl, 2);
                    GlobalApp.HWUsbDisplay.EnableStandBy();
                }

                //Init BarCodeReader 
                GlobalApp.HWBarCodeReader = new InputReader();

                //Start Database Backup Timer if not create XPO Schema and SoftwareVendor is Active
                if (GlobalFramework.PluginSoftwareVendor != null && validDirectoryBackup && !xpoCreateDatabaseAndSchema)
                {
                    _backupDatabaseTimeSpan = TimeSpan.Parse(GlobalFramework.Settings["databaseBackupTimeSpan"]);
                    _databaseBackupTimeSpanRangeStart = TimeSpan.Parse(GlobalFramework.Settings["databaseBackupTimeSpanRangeStart"]);
                    _databaseBackupTimeSpanRangeEnd = TimeSpan.Parse(GlobalFramework.Settings["databaseBackupTimeSpanRangeEnd"]);
                    StartBackupTimer();
                }

                //Send To Log
                _log.Debug(string.Format("ProductVersion: [{0}], ImageRuntimeVersion: [{1}], IsLicensed: [{2}]", FrameworkUtils.ProductVersion, FrameworkUtils.ProductAssembly.ImageRuntimeVersion, LicenceManagement.IsLicensed));

                //Audit
                FrameworkUtils.Audit("APP_START", string.Format("{0} {1} clr {2}", SettingsApp.AppName, FrameworkUtils.ProductVersion, FrameworkUtils.ProductAssembly.ImageRuntimeVersion));
                if (databaseCreated) FrameworkUtils.Audit("DATABASE_CREATE");

                // Plugin Errors Messages
                if (GlobalFramework.PluginSoftwareVendor == null || ! GlobalFramework.PluginSoftwareVendor.IsValidSecretKey(SettingsApp.SecretKey))
                {
                    Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(650, 380), MessageType.Error, ButtonsType.Ok, Resx.global_error, Resx.dialog_message_error_plugin_softwarevendor_not_registered);
                    _log.Debug(String.Format("Wrong key detected [{0}]. Use a valid LogicposFinantialLibrary with same key as SoftwareVendorPlugin", SettingsApp.SecretKey));
                }

                //Create SystemNotification
                FrameworkUtils.SystemNotification();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
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
                Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new System.Drawing.Size(600, 350), MessageType.Info, ButtonsType.Ok, Resx.global_information, message);
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
                        Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(800, 400), MessageType.Error, ButtonsType.Close, Resx.global_error, string.Format(Resx.dialog_message_error_protected_files_invalid_files_detected, filesMessage));
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
                try
                {
                    _log.Debug("Init windowImageFileName ");
                    string windowImageFileName = string.Format(themeWindow.Globals.ImageFileName, GlobalApp.ScreenSize.Width, GlobalApp.ScreenSize.Height);
                    _log.Debug("new StartupWindow " + windowImageFileName);
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
                //GlobalFramework.SessionApp.CleanSession();
                //GlobalFramework.SessionApp.Write();
                GlobalFramework.SessionApp.DeleteSession();
                //Disconnect SessionXpo
                GlobalFramework.SessionXpo.Disconnect();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            Application.Quit();
        }

        public static void Quit(Window pSourceWindow)
        {
            ResponseType responseType = Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, new Size(400, 300), MessageType.Question, ButtonsType.YesNo, Resx.global_quit_title, Resx.global_quit_message);

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
                // Every second call `update_status' (1000 milliseconds)
                GLib.Timeout.Add(SettingsApp.BackupTimerInterval, new GLib.TimeoutHandler(UpdateBackupTimer));
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private bool UpdateBackupTimer()
        {
            bool debug = false;

            DateTime currentDateTime = FrameworkUtils.CurrentDateTimeAtomic();
            DateTime currentDateTimeLastBackup = DataBaseBackup.GetLastBackupDate();
            TimeSpan timeSpanDiference = currentDateTime - currentDateTimeLastBackup;

            //Check if is in Start end Range
            if (currentDateTime.TimeOfDay > _databaseBackupTimeSpanRangeStart && currentDateTime.TimeOfDay < _databaseBackupTimeSpanRangeEnd)
            {
                if (timeSpanDiference >= _backupDatabaseTimeSpan)
                {
                    DataBaseBackup.Backup();
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

            // returning true means that the timeout routine should be invoked
            // again after the timeout period expires. Returning false would
            // terminate the timeout.
            return true;
        }
    }
}
