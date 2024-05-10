using Gtk;
using logicpos.App;
using logicpos.Extensions;
using System;
using System.Drawing;
using System.IO;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using LogicPOS.Settings;

namespace logicpos
{
    public abstract class PosBaseWindow : Gtk.Window
    {
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected EventBox _eventBoxMinimize;
        protected bool _showMinimize;
        public StartupWindow SourceWindow { get; set; }
        public EventBox ScreenArea { get; set; }

        private dynamic GetTheme()
        {
            var predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "PosBaseWindow");
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
            return string.Format("{0}{1}", GeneralSettings.Paths["images"], @"Icos\application.ico");
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

        public PosBaseWindow(string backgroundImageFileLocation)
            : base(WindowType.Toplevel)
        {
            _logger.Debug("PosBaseWindow: " + backgroundImageFileLocation);

            var theme = GetTheme();

            string errorMessage = "Node: <Window ID=\"PosBaseWindow\">";

            if (theme == null)
            {
                Utils.ShowMessageTouchErrorRenderTheme(this, errorMessage);
                _logger.Debug("PosBaseWindow end");
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
                _logger.Error(ex.Message, ex);
                Utils.ShowMessageTouchErrorRenderTheme(this, string.Format("{1}{0}{0}{2}", Environment.NewLine, errorMessage, ex.Message));
            }

            _logger.Debug("PosBaseWindow end");
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

        private int GetMonitorNumber()
        {
            return string.IsNullOrEmpty(GeneralSettings.Settings["appScreen"])
                    ? 0
                    : Convert.ToInt32(GeneralSettings.Settings["appScreen"]);
        }

        protected void CheckMonitorGeometry(int width, int height)
        {
            try
            {
                Gdk.Screen screen = Screen;
                Gdk.Rectangle monitorGeometry = screen.GetMonitorGeometry(GetMonitorNumber());

                if (monitorGeometry.Width < width || monitorGeometry.Height < height)
                {
                    Utils.ShowMessageTouch(
                        this, 
                        DialogFlags.Modal, 
                        MessageType.Error, 
                        ButtonsType.Ok, 
                        CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_error"), 
                        string.Format(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "dialog_message_low_resolution_detected"), 
                        width, 
                        height));
                    Environment.Exit(0);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}