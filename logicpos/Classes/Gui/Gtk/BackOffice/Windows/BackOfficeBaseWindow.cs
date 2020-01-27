using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.datalayer.DataLayer.Xpo;
using System;
using System.IO;
using logicpos.resources.Resources.Localization;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using System.Collections.Generic;

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
        protected Label _labelUpdate;
        protected HBox _hboxStatusBar;
        protected String _clockFormat;
        protected Image _imageLogo = new Image();
        protected Label _reseller;
        public int _widthAccordion = 200;
        public int _heightAccordion = 38;
        
        //Store Active/Last BackOffice User, to keep state when we Change user in POS
        protected sys_userdetail _activeUserDetail;
        protected Accordion _accordion;
        public Accordion Accordion
        {
            get { return _accordion; }
            set { _accordion = value; }
        }
        public TouchButtonIconWithText _dashboardButton;
        public TouchButtonIconWithText _exitButton;
        public TouchButtonIconWithText _backPOS;
        public TouchButtonIconWithText _NewVersion;

        public Dictionary<string, AccordionNode> _accordionChildDocumentsTemp = new Dictionary<string, AccordionNode>();
        public Dictionary<string, AccordionNode> _accordionChildArticlesTemp = new Dictionary<string, AccordionNode>();
        public Dictionary<string, AccordionNode> _accordionChildCostumersTemp = new Dictionary<string, AccordionNode>();
        public Dictionary<string, AccordionNode> _accordionChildUsersTemp = new Dictionary<string, AccordionNode>();
        public Dictionary<string, AccordionNode> _accordionChildOtherTablesTemp = new Dictionary<string, AccordionNode>();

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
            GlobalApp.boScreenSize = Utils.GetScreenSize();
            uint borderWidth = 5;
            System.Drawing.Size sizeIconDashboard = new System.Drawing.Size(30, 30);
            System.Drawing.Size sizeIcon = new System.Drawing.Size(20, 20);
            System.Drawing.Size sizeIconQuit = new System.Drawing.Size(20, 20);
            System.Drawing.Size sizeButton = new System.Drawing.Size(20, 20);
            String fontPosBackOfficeParent = GlobalFramework.Settings["fontPosBackOfficeParent"];
            String fontDescriptionParentLowRes = GlobalFramework.Settings["fontDescriptionParentLowRes"];
            String fontDescription = GlobalFramework.Settings["fontDescriptionParentLowRes"];
            //Settings
            //Redimensionar Botões do accordion para 1024
            fontDescription = fontPosBackOfficeParent;
            if (GlobalApp.boScreenSize.Height <= 800)
            {
                _widthAccordion = 208;
                sizeIcon = new System.Drawing.Size(20, 20);
                sizeButton = new System.Drawing.Size(15, 15);
                sizeIconQuit = new System.Drawing.Size(20, 20);
                sizeIconDashboard = new System.Drawing.Size(20, 20);
                _heightAccordion = 25;
                fontDescription = fontDescriptionParentLowRes;
            }            
            //IN009296 BackOffice - Mudar a língua da aplicação 
            try
            {
                string sql = string.Format("UPDATE cfg_configurationpreferenceparameter SET value = '{0}' WHERE token = 'CULTURE'", GlobalFramework.Settings["customCultureResourceDefinition"]);
                GlobalFramework.SessionXpo.ExecuteScalar(sql);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            /* IN006045 */
            //_clockFormat = GlobalFramework.Settings["dateTimeFormatStatusBar"];
            _clockFormat = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "backoffice_datetime_format_status_bar");

            string fontBackOfficeStatusBar = GlobalFramework.Settings["fontPosStatusBar"];
            string fileImageBackOfficeLogoLong = FrameworkUtils.OSSlash(GlobalFramework.Path["themes"] + @"Default\Images\logo_backoffice_long.png");
            string fileImageBackOfficeLogo = Utils.GetThemeFileLocation(GlobalFramework.Settings["fileImageBackOfficeLogo"]);

            //Colors
            System.Drawing.Color colorBackOfficeContentBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBackOfficeContentBackground"]);
            System.Drawing.Color colorBackOfficeStatusBarBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBackOfficeStatusBarBackground"]);
            System.Drawing.Color colorBackOfficeAccordionFixBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBackOfficeAccordionFixBackground"]);
            System.Drawing.Color colorBackOfficeStatusBarFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBackOfficeStatusBarFont"]);
            System.Drawing.Color colorBackOfficeStatusBarBottomBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBackOfficeStatusBarBottomBackground"]);
            System.Drawing.Color colorLabelReseller = System.Drawing.Color.White;
            ModifyBg(StateType.Normal, Utils.ColorToGdkColor(colorBackOfficeContentBackground));

            //Icon
            string fileImageAppIcon = FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], SettingsApp.AppIcon));
            if (File.Exists(fileImageAppIcon)) Icon = Utils.ImageToPixbuf(System.Drawing.Image.FromFile(fileImageAppIcon));

            //Start Pack UI

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //StatusBar
            EventBox eventBoxStatusBar = new EventBox() { HeightRequest = 38 };
            eventBoxStatusBar.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(colorBackOfficeStatusBarBackground));

            //Reseller
            _reseller = new Label();
            _reseller.Text = string.Format(" Brough by {0}", GlobalFramework.LicenceReseller);
            _reseller.ModifyFont(Pango.FontDescription.FromString("Trebuchet MS 8 Bold"));
            _reseller.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(colorBackOfficeStatusBarFont));
            _reseller.Justify = Justification.Left;
            
            //Logo
            try
            {
                if (GlobalFramework.LicenceReseller != null && GlobalFramework.LicenceReseller.ToString().ToLower() != "logicpulse" && GlobalFramework.LicenceReseller.ToString().ToLower() != "") _imageLogo = new Image(fileImageBackOfficeLogo);
                else _imageLogo = new Image(fileImageBackOfficeLogoLong);
                //_imageLogo.WidthRequest = _widthAccordion + Convert.ToInt16(borderWidth) * 3;
                //_imageLogo.SetAlignment(0.0F, 0.5F);

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("InitUI(): Image [{0}] not found: {1}", fileImageBackOfficeLogo, ex.Message), ex);
            }

            //Style StatusBarFont
            Pango.FontDescription fontDescriptionStatusBar = Pango.FontDescription.FromString(fontBackOfficeStatusBar);
            String _dashboardIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_dashboard.png");
            String _updateIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_update.png");
            String _exitIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_pos_close_backoffice.png");
            String _backPOSIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_pos_front_office.png");
            String _iconDashBoard = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_other_tables.png");

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
            if (GlobalFramework.LicenceReseller != null && GlobalFramework.LicenceReseller.ToString().ToLower() != "logicpulse" && LicenceManagement.IsLicensed) _hboxStatusBar.PackStart(_reseller, false, false, 0);
            _hboxStatusBar.PackStart(_labelActiveContent, false, false, 0);
            _hboxStatusBar.PackStart(_labelTerminalInfo, true, true, 0);


            //TODO:THEME
            if (GlobalApp.boScreenSize.Width < 1024 || GlobalApp.boScreenSize.Height < 768)
            {
                _labelTerminalInfo.SetAlignment(1.0F, 0.5F);
            }
            else
            {
                _hboxStatusBar.PackStart(_labelClock, false, false, 0);
                
            }

            if (GlobalFramework.AppUseBackOfficeMode)
            {
                EventBox eventBoxMinimize = Utils.GetMinimizeEventBox();
                eventBoxMinimize.ButtonReleaseEvent += delegate {
                    Iconify();
                };
                _hboxStatusBar.PackStart(eventBoxMinimize, false, false, 0);
                //fix.Put(eventBoxMinimize, GlobalApp.ScreenSize.Width - 27 - 10, 10);
            }

            _imageLogo.Dispose();
            _dashboardButton = new TouchButtonIconWithText("DASHBOARD_ICON", FrameworkUtils.StringToColor("168, 204, 79"), "Dashboard", fontDescription, FrameworkUtils.StringToColor("61, 61, 61"), _dashboardIcon, sizeIconDashboard, _widthAccordion, _heightAccordion, true);
            _exitButton = new TouchButtonIconWithText("EXIT_BUTTON", FrameworkUtils.StringToColor("201, 102, 88"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_quit"), fontDescription, FrameworkUtils.StringToColor("255, 255, 255"), _exitIcon, sizeButton, _widthAccordion, _heightAccordion, true);
            _backPOS = new TouchButtonIconWithText("POS", FrameworkUtils.StringToColor("168, 204, 79"), "LogicPOS", fontDescription, FrameworkUtils.StringToColor("61, 61, 61"), _backPOSIcon, sizeButton, _widthAccordion, _heightAccordion, true);
            _NewVersion = new TouchButtonIconWithText("Update_Button", FrameworkUtils.StringToColor("168, 204, 79"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_update_pos"), fontDescription, FrameworkUtils.StringToColor("61, 61, 61"), _updateIcon, sizeButton, _widthAccordion, _heightAccordion, true);
            _labelClock.ModifyFont(fontDescriptionStatusBar);
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
            _fixAccordion.Put(_dashboardButton, 0, 0);
            _fixAccordion.Add(_dashboardButton);
                        
            //Redimensionar Botões do Backoffice para 1024x768
            if (!GlobalFramework.AppUseBackOfficeMode)
            {
                if (GlobalApp.boScreenSize.Height <= 800)
                {
                    _fixAccordion.Put(_backPOS, 0, GlobalApp.boScreenSize.Height - 112);
                    _fixAccordion.Add(_backPOS);

                }
                else
                {
                    _fixAccordion.Put(_backPOS, 0, GlobalApp.boScreenSize.Height - 135);
                    _fixAccordion.Add(_backPOS);
                }                
            }
            //Redimensionar Botões do Backoffice para 1024x768
            if (GlobalApp.boScreenSize.Height <= 800)
            {
                _fixAccordion.Put(_exitButton, 0, GlobalApp.boScreenSize.Height - 85);
                _fixAccordion.Add(_exitButton);
            }
            else
            {
                _fixAccordion.Put(_exitButton, 0, GlobalApp.boScreenSize.Height - 95);
                _fixAccordion.Add(_exitButton);
            }
			
			//TK016248 - BackOffice - Check New Version 
            string appVersion = FrameworkUtils.ProductVersion.Replace("v", ""); 

            bool needToUpdate = false;
            //GlobalFramework.ServerVersion = "1.3.0000";
            if (GlobalFramework.ServerVersion != null)
            {
                try
                {
                    string[] tmpNew = appVersion.Split('.');
                    long tmpNewVer = int.Parse(tmpNew[0]) * 10000000 + int.Parse(tmpNew[1]) * 10000 + int.Parse(tmpNew[2]);

                    string[] tmpOld = GlobalFramework.ServerVersion.ToString().Split('.');
                    long tmpOldVer = int.Parse(tmpOld[0]) * 10000000 + int.Parse(tmpOld[1]) * 10000 + int.Parse(tmpOld[2]);

                    if (tmpNewVer < tmpOldVer)
                    {
                        needToUpdate = true;
                    }
                }
                catch /*(Exception ex)*/
                {
                    //log.Error(ex.Message, ex);
                }

                if (needToUpdate)
                {
                    if (GlobalFramework.AppUseBackOfficeMode)
                    {
                        if (GlobalApp.boScreenSize.Height <= 800)
                        {
                            _labelUpdate = new Label(string.Format(string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_new_version"), GlobalFramework.ServerVersion.ToString())));
                            _labelUpdate.ModifyFont(fontDescriptionStatusBar);
                            _labelUpdate.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(FrameworkUtils.StringToColor("61, 61, 61")));
                            _labelUpdate.SetAlignment(1.0F, 0.5F);
                            _fixAccordion.Put(_labelUpdate, 5, GlobalApp.boScreenSize.Height - 165);
                            _fixAccordion.Add(_labelUpdate);
                            _fixAccordion.Put(_NewVersion, 0, GlobalApp.boScreenSize.Height - 140);
                            _fixAccordion.Add(_NewVersion);
                        }
                        else
                        {
                            _labelUpdate = new Label(string.Format(string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_new_version"), GlobalFramework.ServerVersion.ToString())));
                            _labelUpdate.ModifyFont(fontDescriptionStatusBar);
                            _labelUpdate.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(FrameworkUtils.StringToColor("61, 61, 61")));
                            _labelUpdate.SetAlignment(1.0F, 0.5F);
                            _fixAccordion.Put(_labelUpdate, 5, GlobalApp.boScreenSize.Height - 160);
                            _fixAccordion.Add(_labelUpdate);
                            _fixAccordion.Put(_NewVersion, 0, GlobalApp.boScreenSize.Height - 135);
                            _fixAccordion.Add(_NewVersion);
                        }
                    }
                    else
                    {
                        if (GlobalApp.boScreenSize.Height <= 800)
                        {
                            _labelUpdate = new Label(string.Format(string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_new_version"), GlobalFramework.ServerVersion.ToString())));
                            _labelUpdate.ModifyFont(fontDescriptionStatusBar);
                            _labelUpdate.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(FrameworkUtils.StringToColor("61, 61, 61")));
                            _labelUpdate.SetAlignment(1.0F, 0.5F);
                            _fixAccordion.Put(_labelUpdate, 5, GlobalApp.boScreenSize.Height - 165);
                            _fixAccordion.Add(_labelUpdate);
                            _fixAccordion.Put(_NewVersion, 0, GlobalApp.boScreenSize.Height - 140);
                            _fixAccordion.Add(_NewVersion);
                        }
                        else
                        {
                            _labelUpdate = new Label(string.Format(string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_new_version"), GlobalFramework.ServerVersion.ToString())));
                            _labelUpdate.ModifyFont(fontDescriptionStatusBar);
                            _labelUpdate.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(FrameworkUtils.StringToColor("61, 61, 61")));
                            _labelUpdate.SetAlignment(1.0F, 0.5F);
                            _fixAccordion.Put(_labelUpdate, 5, GlobalApp.boScreenSize.Height - 200);
                            _fixAccordion.Add(_labelUpdate);
                            _fixAccordion.Put(_NewVersion, 0, GlobalApp.boScreenSize.Height - 175);
                            _fixAccordion.Add(_NewVersion);
                        }
                        
                    }
                }
            }
			//TK016248 End 

            //Pack hboxContent
            _hboxContent.PackStart(_fixAccordion, false, false, 0);
            VBox vboxContent = new VBox(false, 0);

            eventBoxStatusBar.Add(_hboxStatusBar);

            vboxContent.PackStart(eventBoxStatusBar, false, false, 0);

            vboxContent.PackStart(_hboxContent);
            //vboxContent.PackStart(fixStatusBarBottom, false, false, 0);

            this.HeightRequest = 50;
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
            // Every second call update_status' (1000 milliseconds)
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
