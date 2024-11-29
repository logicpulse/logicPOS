using Gtk;
using logicpos;
using logicpos.Classes.DataLayer;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Logic.Hardware;
using logicpos.Classes.Logic.Others;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
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
                CustomAlerts.ShowContactSupportErrorAlert(LoginWindow.Instance, ex.StackTrace);
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
            LogicPOSAppContext.MultiUserEnvironment = AppSettings.Instance.appMultiUserEnvironment;
            LogicPOSAppContext.UseVirtualKeyBoard = AppSettings.Instance.useVirtualKeyBoard;

            LogicPOSAppContext.Notifications = new Dictionary<string, bool>
            {
                ["SHOW_PRINTER_UNDEFINED"] = true
            };

            LogicPOSAppContext.OpenFileDialogStartPath = Directory.GetCurrentDirectory();

            LogicPOSSettings.FirstBoot = false;

   
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
                CustomAlerts.ShowThemeRenderingErrorAlert(ex.Message, LoginWindow.Instance);
            }

            if (TerminalService.Terminal.PoleDisplay != null)
            {
                LogicPOSAppContext.UsbDisplay = UsbDisplayDevice.InitDisplay();
                LogicPOSAppContext.UsbDisplay.WriteCentered(string.Format("{0} {1}", LogicPOSSettings.AppName, GeneralSettings.ProductVersion), 1);
                LogicPOSAppContext.UsbDisplay.WriteCentered(LogicPOSSettings.AppUrl, 2);
                LogicPOSAppContext.UsbDisplay.EnableStandBy();
            }

            if (TerminalService.Terminal.BarcodeReader != null)
            {
                LogicPOSAppContext.BarCodeReader = new InputReader();
            }

            if (TerminalService.Terminal.WeighingMachine != null)
            {

                if (TerminalService.Terminal.WeighingMachine.PortName == TerminalService.Terminal.PoleDisplay.COMPort)
                {
                    _logger.Debug(string.Format("Port " + TerminalService.Terminal.WeighingMachine.PortName + "Already taken by pole display"));
                }
                else
                {
                    if (Utils.IsPortOpen(TerminalService.Terminal.WeighingMachine.PortName))
                    {
                        LogicPOSAppContext.WeighingBalance = new WeighingBalance(TerminalService.Terminal.WeighingMachine);
                    }

                }

            }

            if (PluginSettings.HasSoftwareVendorPlugin == false ||
                PluginSettings.SoftwareVendor.IsValidSecretKey(PluginSettings.SecretKey) == false)
            {
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

                if (response == ResponseType.No) _quitAfterBootStrap = true;
            }

            if (PluginSettings.HasSoftwareVendorPlugin && backupsFolderExists && xpoCreateDatabaseAndSchema == false)
            {
                _autoBackupFlowIsEnabled = PreferenceParametersService.DatabaseBackupAutomaticEnabled;

                if (_autoBackupFlowIsEnabled)
                {
                    _backupDatabaseTimeSpan = PreferenceParametersService.DatabaseBackupTimeSpan;
                    _databaseBackupTimeSpanRangeStart = PreferenceParametersService.DatabaseBackupTimeSpanRangeStart;
                    _databaseBackupTimeSpanRangeEnd = PreferenceParametersService.DatabaseBackupTimeSpanRangeEnd;
                    StartBackupTimer();
                }
            }
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
                BackOfficeWindow.Instance = new BackOfficeWindow();
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
            Gtk.Application.Quit();
        }

        public static void Quit(Window parentWindow)
        {
            ResponseType responseType = new CustomAlert(parentWindow)
                                            .WithMessageResource("global_quit_message")
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
