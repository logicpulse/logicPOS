using Gtk;
using logicpos;
using logicpos.App;
using LogicPOS.Settings;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Drawing;
using System.IO;

namespace LogicPOS.UI.Components.Windows
{
    public abstract class POSWindow : Window
    {
        protected EventBox _eventBoxMinimize;
        protected bool _showMinimize;
        public StartupWindow SourceWindow { get; set; }
        public EventBox ScreenArea { get; set; }

        private dynamic GetTheme()
        {
            var predicate = (Predicate<dynamic>)((x) => x.ID == "PosBaseWindow");
            var theme = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);
            return theme;
        }

        private int GetScreenWidthFromTheme(dynamic theme)
            => Convert.ToInt16(theme.Globals.ScreenWidth);

        private int GetScreenHeightFromTheme(dynamic theme)
            => Convert.ToInt16(theme.Globals.ScreenHeight);

        private Color GetScreenBackgroundColorOuterFromTheme(dynamic theme)
            => (theme.Globals.ScreenBackgroundColorOuter as string)
            .StringToColor();

        private Color GetScreenBackgroundColorFromTheme(dynamic theme)
            => (theme.Globals.ScreenBackgroundColor as string).StringToColor();

        private string GetAppIconFileLocation()
        {
            return string.Format("{0}{1}", PathsSettings.ImagesFolderLocation, @"Icos\application.ico");
        }

        private void SetAppIcon()
        {
            string appIconFileLocation = GetAppIconFileLocation();

            if (File.Exists(appIconFileLocation)) Icon = Utils.ImageToPixbuf(System.Drawing.Image.FromFile(appIconFileLocation));
        }

        private void ConfigureScreen(
            int screenWidth,
            int screenHeight,
            Color screenBackgroundColor,
            string backgroundImageFileLocation)
        {
            ScreenArea = new EventBox();
            ScreenArea.ModifyBg(StateType.Normal, screenBackgroundColor.ToGdkColor());
            ScreenArea.WidthRequest = screenWidth;
            ScreenArea.HeightRequest = screenHeight;

            if (backgroundImageFileLocation != string.Empty)
            {
                ScreenArea.Style = Utils.GetThemeStyleBackground(backgroundImageFileLocation);
            }
        }

        private void InitializeUI(
            int screenWidth,
            int screenHeight,
            Color screenBackgroundColorOuter)
        {
            SetDefaultSize(screenWidth, screenHeight);
            WindowPosition = WindowPosition.Center;
            Modal = true;
            Fullscreen();
            ModifyBg(StateType.Normal, screenBackgroundColorOuter.ToGdkColor());
        }

        private void ConfigureAlignment()
        {
            Alignment alignment = new Alignment(0.5f, 0.5f, 0.0f, 0.0f);
            alignment.Add(ScreenArea);

            Add(alignment);
        }

        public POSWindow(string backgroundImageFileLocation)
            : base(WindowType.Toplevel)
        {
            var theme = GetTheme();

            string errorMessage = "Node: <Window ID=\"PosBaseWindow\">";

            if (theme == null)
            {
                Utils.ShowMessageTouchErrorRenderTheme(this, errorMessage);
            }

            try
            {
                Name = Convert.ToString(theme.Globals.Name);
                int screenWidth = GetScreenWidthFromTheme(theme);
                int screenHeight = GetScreenHeightFromTheme(theme);
                Color screenBackgroundColorOuter = GetScreenBackgroundColorOuterFromTheme(theme);
                Color screenBackgroundColor = GetScreenBackgroundColorFromTheme(theme);

                CheckMonitorGeometry(screenWidth, screenHeight);

                InitializeUI(screenWidth, screenHeight, screenBackgroundColorOuter);

                SetAppIcon();

                ConfigureScreen(screenWidth, screenHeight, screenBackgroundColor, backgroundImageFileLocation);

                ConfigureAlignment();

                DeleteEvent += PosBaseWindow_DeleteEvent;

            }
            catch (Exception ex)
            {
                Utils.ShowMessageTouchErrorRenderTheme(this, $"{errorMessage}{Environment.NewLine}{Environment.NewLine}{ex.Message}");
            }

        }

        private void PosBaseWindow_DeleteEvent(object o, DeleteEventArgs args)
        {
            Hide();

            if (SourceWindow == null)
            {
                LogicPOSApp.Quit(this);
            }

            SourceWindow.ShowAll();

            args.RetVal = true;
        }


        protected void CheckMonitorGeometry(int width, int height)
        {
            Gdk.Screen screen = Screen;
            Gdk.Rectangle monitorGeometry = screen.GetMonitorGeometry(AppSettings.Instance.appScreen);

            if (monitorGeometry.Width < width || monitorGeometry.Height < height)
            {
                Utils.ShowMessageTouch(
                    this,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Ok,
                    GeneralUtils.GetResourceByName("global_error"),
                    string.Format(GeneralUtils.GetResourceByName("dialog_message_low_resolution_detected"),
                    width,
                    height));
                Environment.Exit(0);
            }

        }
    }
}