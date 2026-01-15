using Gtk;
using logicpos;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Logic.Hardware;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application.Screen;
using LogicPOS.UI.Components.POS.Devices.Hardware;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace LogicPOS.UI.Application
{
    internal class LogicPOSApp
    {
        public static Dialog LoadingDialog { get; set; }
        public static ThreadNotify DialogThreadNotify { get; set; }
        public static PosKeyboardDialog DialogPosKeyboard { get; set; }
        public static dynamic Theme { get; set; }
        public static ExpressionEvaluationService ExpressionEvaluator { get; set; } = new ExpressionEvaluationService();
        public static UsbDisplayDevice UsbDisplay { get; set; }
        public static InputReader BarCodeReader { get; set; }
        public static WeighingBalance WeighingBalance { get; set; }

        public void Start()
        {
            try
            {
                Log.Information("Configuring LogicPOS UI...");
                InitializeScreenSize();
                InitializeExpressionEvaluator();
                InitializeTheme();
                InitializeTerminalDevices();
                DialogThreadNotify?.WakeupMain();
                LoginWindow.Instance.ShowAll();
                Gtk.Application.Run();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred during application startup: {Message}", ex.Message);
                CustomAlerts.ShowContactSupportErrorAlert(LoginWindow.Instance, ex.StackTrace);
            }
            finally
            {
                CloseDevices();
            }
        }

        private static void CloseDevices()
        {
            if (UsbDisplay != null)
            {
                UsbDisplay.Close();
            }

            if (WeighingBalance != null && WeighingBalance.IsPortOpen())
            {
                WeighingBalance.ClosePort();
            }
        }

        private static void InitializeTerminalDevices()
        {
            if (TerminalService.Terminal.PoleDisplay != null)
            {
                UsbDisplay = UsbDisplayDevice.InitDisplay();
                UsbDisplay.WriteCentered(string.Format("{0} {1}", AppSettings.AppName, AppSettings.ProductVersion), 1);
                UsbDisplay.WriteCentered("www.logicpulse.com", 2);
                UsbDisplay.EnableStandBy();
            }

            if (TerminalService.Terminal.BarcodeReader != null)
            {
                BarCodeReader = new InputReader();
            }

            if (TerminalService.Terminal.WeighingMachine != null)
            {

                if (TerminalService.Terminal.WeighingMachine.PortName == TerminalService.Terminal.PoleDisplay.COMPort)
                {
                    Log.Debug(string.Format("Port " + TerminalService.Terminal.WeighingMachine.PortName + "Already taken by pole display"));
                }
                else
                {
                    if (logicpos.Utils.IsPortOpen(TerminalService.Terminal.WeighingMachine.PortName))
                    {
                        WeighingBalance = new WeighingBalance(TerminalService.Terminal.WeighingMachine);
                    }

                }

            }
        }

        private static void InitializeTheme()
        {
            try
            {
                Log.Information("Parsing theme from {File}", AppSettings.Instance.ThemeFilePath);
                Theme = XmlToObjectParser.ParseFromFile(AppSettings.Instance.ThemeFilePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error parsing theme {File}", AppSettings.Instance.ThemeFilePath);
                DialogThreadNotify.WakeupMain();
                CustomAlerts.ShowThemeRenderingErrorAlert(ex.Message, LoginWindow.Instance);
            }
        }

        private static void InitializeExpressionEvaluator()
        {
            ExpressionEvaluator.EvaluateFunction += ExpressionEvaluatorExtended.ExpressionEvaluator_EvaluateFunction;
            ExpressionEvaluatorExtended.InitVariablesStartupWindow();
            ExpressionEvaluatorExtended.InitVariablesPosMainWindow();
            ExpressionEvaluator.Variables.Add("globalScreenSize", AppSettings.Instance.AppScreenSize);
        }

        private static void InitializeScreenSize()
        {
            var appScreenSize = AppSettings.Instance.AppScreenSize;
            AppSettings.Instance.AppScreenSize = appScreenSize == new Size(0, 0) ? ScreenSizeUtil.GetThemeScreenSize() : ScreenSizeUtil.GetThemeScreenSize(appScreenSize);
            AppSettings.MaxWindowSize = new Size(AppSettings.Instance.AppScreenSize.Width - 40, AppSettings.Instance.AppScreenSize.Height - 40);
        }

    }
}
