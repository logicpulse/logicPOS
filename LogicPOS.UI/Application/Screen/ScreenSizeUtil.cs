using Gtk;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Settings;
using Serilog;
using System;
using System.Drawing;

namespace LogicPOS.UI.Application.Screen
{
    internal static class ScreenSizeUtil
    {
        public static Size GetScreenSize()
        {
            Size result = new Size();
            // Moke Window only to extract its Resolution
            Window window = new Window("");
            Gdk.Screen screen = window.Screen;
            Gdk.Rectangle monitorGeometry = screen.GetMonitorGeometry(AppSettings.Instance.AppScreen);
            result = new Size(monitorGeometry.Width, monitorGeometry.Height);
            // CleanUp
            window.Dispose();
            screen.Dispose();
            return result;
        }

        public static Size GetThemeScreenSize()
        {
            return GetThemeScreenSize(GetScreenSize());
        }

        public static Size GetThemeScreenSize(Size screenSize)
        {
            Size result = screenSize;

            ScreenResolution screenSizeEnum = GetSupportedScreenResolution(screenSize);
        
            switch (screenSizeEnum)
            {
                case ScreenResolution.res800x600:
                case ScreenResolution.res1024x768:
                case ScreenResolution.res1280x768:
                case ScreenResolution.res1280x1024:
                case ScreenResolution.res1366x768:
                case ScreenResolution.res1680x1050:
                case ScreenResolution.res1920x1080:
                    break;
                case ScreenResolution.res1024x600:
                case ScreenResolution.res1280x720:
                    result = new Size(800, 600);
                    break;
                case ScreenResolution.res1152x864:
                    result = new Size(1024, 768);
                    break;
                case ScreenResolution.res1280x800:
                case ScreenResolution.res1360x768:
                    result = new Size(1280, 768);
                    break;
                case ScreenResolution.res1440x900:
                case ScreenResolution.res1536x864:
                case ScreenResolution.res1600x900:
                    result = new Size(1366, 768);
                    break;
                case ScreenResolution.res1920x1200:
                case ScreenResolution.res1920x1280:
                case ScreenResolution.res2160x1440:
                case ScreenResolution.res2560x1080:
                case ScreenResolution.res2560x1440:
                case ScreenResolution.res3440x1440:
                case ScreenResolution.res3840x2160:
                    result = new Size(1920, 1080);
                    break;
                default:
                    result = new Size(1024, 768);
                    break;
            }
            return result;
        }

        public static ScreenResolution GetSupportedScreenResolution(Size screenSize)
        {
            ScreenResolution supportedScreenSizeEnum;
            string screenSizeForEnum = string.Format("res{0}x{1}", screenSize.Width, screenSize.Height);

            try
            {
                supportedScreenSizeEnum = (ScreenResolution)Enum.Parse(typeof(ScreenResolution), screenSizeForEnum, true);
            }
            catch (Exception ex)
            {
                Log.Error("ScreenSize GetSupportedScreenResolution(Size screenSize) :: " + ex.Message, ex);

                LogicPOSApp.DialogThreadNotify.WakeupMain();

                var messageDialog = new CustomAlert(LoginWindow.Instance)
                    .WithMessage($"ShowUnsupportedResolutionErrorAlert{screenSize.Width}, {screenSize.Height}")
                    .WithSize(new Size())
                    .ShowAlert();

                supportedScreenSizeEnum = ScreenResolution.resDefault;
            }
            return supportedScreenSizeEnum;
        }
    }
}
