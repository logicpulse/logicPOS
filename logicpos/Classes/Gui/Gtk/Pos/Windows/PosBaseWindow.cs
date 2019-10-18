using Gtk;
using logicpos.App;
using logicpos.resources.Resources.Localization;
using System;
using System.Drawing;
using System.IO;

namespace logicpos
{
    public abstract class PosBaseWindow : Gtk.Window
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Protected
        /* IN008024 */
        //protected string _appOperationModeToken = GlobalFramework.Settings["appOperationModeToken"];
        protected EventBox _eventBoxMinimize;
        protected bool _showMinimize;

        //Public Properties
        //SourceWindow
        private StartupWindow _sourceWindow;
        public StartupWindow SourceWindow
        {
            get { return (_sourceWindow); }
            set { _sourceWindow = value; /*TransientFor = _sourceWindow;*/ }
        }
        //ScreenArea
        protected EventBox _eventboxScreenArea;
        public EventBox ScreenArea
        {
            get { return _eventboxScreenArea; }
            set { _eventboxScreenArea = value; }
        }

        //Constructor
        public PosBaseWindow(String pFileImageBackground)
            : base(Gtk.WindowType.Toplevel)
        {
            _log.Debug("PosBaseWindow: " + pFileImageBackground);
            //Init Theme Object
            var predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "PosBaseWindow");
            var themeWindow = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);
            //Shared error Message
            string errorMessage = "Node: <Window ID=\"PosBaseWindow\">";

            //Assign Theme Vars + UI
            if (themeWindow != null)
            {
                try
                {
                    _log.Debug("themeWindow: " + themeWindow);

                    //Globals
                    Name = Convert.ToString(themeWindow.Globals.Name);
                    int screenWidth = Convert.ToInt16(themeWindow.Globals.ScreenWidth);
                    int screenHeight = Convert.ToInt16(themeWindow.Globals.ScreenHeight);
                    Color screenBackgroundColorOuter = FrameworkUtils.StringToColor(themeWindow.Globals.ScreenBackgroundColorOuter);
                    Color screenBackgroundColor = FrameworkUtils.StringToColor(themeWindow.Globals.ScreenBackgroundColor);

                    //Check Requirenments
                    CheckMonitorGeometry(screenWidth, screenHeight);

                    //Init UI
                    SetDefaultSize(screenWidth, screenHeight);
                    WindowPosition = WindowPosition.Center;
                    Modal = true;
                    Fullscreen();
                    ModifyBg(StateType.Normal, Utils.ColorToGdkColor(screenBackgroundColorOuter));

                    //Icon
                    string fileImageAppIcon = FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], @"Icos\application.ico"));
                    if (File.Exists(fileImageAppIcon)) Icon = Utils.ImageToPixbuf(System.Drawing.Image.FromFile(fileImageAppIcon));

                    _eventboxScreenArea = new EventBox();
                    _eventboxScreenArea.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(screenBackgroundColor));
                    _eventboxScreenArea.WidthRequest = screenWidth;
                    _eventboxScreenArea.HeightRequest = screenHeight;

                    Alignment _alignmentWindow = new Alignment(0.5f, 0.5f, 0.0f, 0.0f);
                    _alignmentWindow.Add(_eventboxScreenArea);
                    Add(_alignmentWindow);

                    DeleteEvent += PosBaseWindow_DeleteEvent;
                    _log.Debug("Theme Background: " + pFileImageBackground);

                    //Theme Background
                    if (pFileImageBackground != string.Empty)
                    {
                        _eventboxScreenArea.Style = Utils.GetThemeStyleBackground(pFileImageBackground);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    Utils.ShowMessageTouchErrorRenderTheme(this, string.Format("{1}{0}{0}{2}", Environment.NewLine, errorMessage, ex.Message));
                }
            }
            else
            {
                Utils.ShowMessageTouchErrorRenderTheme(this, errorMessage);
            }
            _log.Debug("PosBaseWindow end");
        }

        void PosBaseWindow_DeleteEvent(object o, DeleteEventArgs args)
        {
            Hide();

            //Pos
            if (SourceWindow != null)
            {
                SourceWindow.ShowAll();
            }
            //Startup Window
            else
            {
                LogicPos.Quit(this);
            };

            //Prevent Window Destroy, When user Uses Close
            args.RetVal = true;
        }

        protected void CheckMonitorGeometry(int pWidth, int pHeight)
        {
            try
            {
                Gdk.Screen screen = this.Screen;
                Gdk.Rectangle monitorGeometry = screen.GetMonitorGeometry(string.IsNullOrEmpty(GlobalFramework.Settings["appScreen"])
                    ? 0
                    : Convert.ToInt32(GlobalFramework.Settings["appScreen"]));
                if (monitorGeometry.Width < pWidth || monitorGeometry.Height < pHeight)
                {
                    Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_low_resolution_detected"), pWidth, pHeight));
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}