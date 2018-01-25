using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.shared;
using System;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    public abstract class BackOfficeBaseWindow : Window
    {
        //Log4Net
        protected log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Members
        protected Fixed _fixAccordion;
        protected HBox _hboxContent;
        protected Widget _nodeContent;
        protected Label _labelActiveContent;
        protected Label _labelTerminalInfo;
        protected Label _labelClock;
        protected HBox _hboxStatusBar;
        protected String _clockFormat;
        protected Image _imageLogo = new Image();
        protected int _widthAccordion = 200;
        //Store Active/Last BackOffice User, to keep state when we Change user in POS
        protected SYS_UserDetail _activeUserDetail;
        protected Accordion _accordion;
        public Accordion Accordion
        {
            get { return _accordion; }
            set { _accordion = value; }
        }

        public BackOfficeBaseWindow()
            : base(WindowType.Toplevel)
        {
            Title = Utils.GetWindowTitle("BackOfficeBaseWindow");
            SetPosition(WindowPosition.Center);
            Fullscreen();
            InitUI();
            ShowAll();

            //Events
            DeleteEvent += BackOfficeMainWindow_DeleteEvent;
        }

        private void InitUI()
        {
            //Init Local Vars
            uint borderWidth = 5;
            //Settings
            _clockFormat = GlobalFramework.Settings["dateTimeFormatStatusBar"];
            string fontBackOfficeStatusBar = GlobalFramework.Settings["fontPosStatusBar"];
            string fileImageBackOfficeLogo = Utils.GetThemeFileLocation(GlobalFramework.Settings["fileImageBackOfficeLogo"]);

            //Colors
            System.Drawing.Color colorBackOfficeContentBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBackOfficeContentBackground"]);
            System.Drawing.Color colorBackOfficeStatusBarBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBackOfficeStatusBarBackground"]);
            System.Drawing.Color colorBackOfficeAccordionFixBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBackOfficeAccordionFixBackground"]);
            System.Drawing.Color colorBackOfficeStatusBarFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBackOfficeStatusBarFont"]);
            System.Drawing.Color colorBackOfficeStatusBarBottomBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBackOfficeStatusBarBottomBackground"]);

            ModifyBg(StateType.Normal, Utils.ColorToGdkColor(colorBackOfficeContentBackground));

            //Icon
            string fileImageAppIcon = FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], SettingsApp.AppIcon));
            if (File.Exists(fileImageAppIcon)) Icon = Utils.ImageToPixbuf(System.Drawing.Image.FromFile(fileImageAppIcon));

            //Start Pack UI

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //StatusBar
            EventBox eventBoxStatusBar = new EventBox() { HeightRequest = 38 };
            eventBoxStatusBar.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(colorBackOfficeStatusBarBackground));

            //Logo
            try
            {
                _imageLogo = new Image(fileImageBackOfficeLogo);
                _imageLogo.WidthRequest = _widthAccordion + Convert.ToInt16(borderWidth) * 3;
                _imageLogo.SetAlignment(0.0F, 0.5F);
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("InitUI(): Image [{0}] not found: {1}", fileImageBackOfficeLogo, ex.Message), ex);
            }

            //Style StatusBarFont
            Pango.FontDescription fontDescriptionStatusBar = Pango.FontDescription.FromString(fontBackOfficeStatusBar);

            //Active Content
            _labelActiveContent = new Label() { WidthRequest = 300 };
            _labelActiveContent.ModifyFont(fontDescriptionStatusBar);
            _labelActiveContent.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(colorBackOfficeStatusBarFont));
            _labelActiveContent.SetAlignment(0.0F, 0.5F);

            //TerminalInfo : Terminal : User
            _labelTerminalInfo = new Label(string.Format("{0} : {1}", GlobalFramework.LoggedTerminal.Designation, GlobalFramework.LoggedUser.Name));
            _labelTerminalInfo.ModifyFont(fontDescriptionStatusBar);
            _labelTerminalInfo.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(colorBackOfficeStatusBarFont));
            _labelTerminalInfo.SetAlignment(0.5F, 0.5F);

            //Clock
            _labelClock = new Label(FrameworkUtils.CurrentDateTime(_clockFormat));
            _labelClock.ModifyFont(fontDescriptionStatusBar);
            _labelClock.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(colorBackOfficeStatusBarFont));
            _labelClock.SetAlignment(1.0F, 0.5F);

            //Pack HBox StatusBar
            _hboxStatusBar = new HBox(false, 0) { BorderWidth = borderWidth };
            _hboxStatusBar.PackStart(_imageLogo, false, false, 0);
            _hboxStatusBar.PackStart(_labelActiveContent, false, false, 0);
            _hboxStatusBar.PackStart(_labelTerminalInfo, true, true, 0);

            //TODO:THEME
            if (GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600)
            {
                _labelTerminalInfo.SetAlignment(1.0F, 0.5F);
            }
            else
            {
                _hboxStatusBar.PackStart(_labelClock, false, false, 0);
            }

            _imageLogo.Dispose();

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //StatusBar
            //Fixed fixStatusBarBottom = new Fixed() { HasWindow = true, BorderWidth = borderWidth, HeightRequest = 38 };
            //fixStatusBarBottom.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(colorBackOfficeStatusBarBottomBackground));

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Hbox Accordion + Content
            _hboxContent = new HBox(false, (int)borderWidth) { BorderWidth = borderWidth };
            //_accordion = new Accordion() { WidthRequest = _widthAccordion };

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Accordion and HouseFix
            _fixAccordion = new Fixed() { HasWindow = true, BorderWidth = borderWidth };
            _fixAccordion.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(colorBackOfficeAccordionFixBackground));
            //fixAccordion.Add(_accordion);

            //Pack hboxContent
            _hboxContent.PackStart(_fixAccordion, false, false, 0);

            VBox vboxContent = new VBox(false, 0);

            eventBoxStatusBar.Add(_hboxStatusBar);

            vboxContent.PackStart(eventBoxStatusBar, false, false, 0);
            vboxContent.PackStart(_hboxContent);
            // vboxContent.PackStart(fixStatusBarBottom, false, false, 0);

            //Final Pack
            Add(vboxContent);

            //Clock
            StartClock();
        }

        void BackOfficeMainWindow_DeleteEvent(object o, DeleteEventArgs args)
        {
            Hide();
            GlobalApp.WindowPos.ShowAll();
            //Prevent Window Destroy, When user Uses Close
            args.RetVal = true;
        }

        //Process here the AccordionChildButton Events, only here we have access to BackOffice Objects. Ex.: _hboxContent
        protected void accordion_Clicked(object sender, EventArgs e)
        {
            try
            {
                dynamic clickedButton;

                if (sender.GetType() == typeof(AccordionParentButton))
                {
                    clickedButton = (AccordionParentButton)sender;
                }
                else
                {
                    clickedButton = (AccordionChildButton)sender;
                }

                //Show Button Content if its a Chield Button
                if (clickedButton.GetType() == typeof(AccordionChildButton))
                {
                    if (clickedButton.Content != null)
                    {
                        if (_nodeContent != null)
                        {
                            _hboxContent.Remove(_nodeContent);
                        }
                        _nodeContent = clickedButton.Content;
                        //Store active content button nodeContentActiveButton to reference to have access to Name, used for previleges etc
                        _accordion.CurrentChildButtonContent = clickedButton;
                        _labelActiveContent.Text = clickedButton.Label;

                        if (!_nodeContent.Visible)
                        {
                            _nodeContent.Visible = true;
                        }
                        _hboxContent.PackStart(_nodeContent);
                    }
                    else
                    {
                        if (clickedButton.ExternalAppFileName != null)
                        {
                            FrameworkUtils.ExecuteExternalProcess(clickedButton.ExternalAppFileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private void StartClock()
        {
            // Every second call `update_status' (1000 milliseconds)
            GLib.Timeout.Add(1000, new GLib.TimeoutHandler(UpdateClock));
        }

        private bool UpdateClock()
        {
            _labelClock.Text = FrameworkUtils.CurrentDateTime(_clockFormat);
            // returning true means that the timeout routine should be invoked
            // again after the timeout period expires. Returning false would
            // terminate the timeout.
            return true;
        }
    }
}
