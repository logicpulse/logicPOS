using Gtk;
using logicpos;
using logicpos.App;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Accordions;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using Pango;
using System;
using System.Collections.Generic;
using System.IO;

namespace LogicPOS.UI.Components.BackOffice.Windows
{
    public abstract class BackOfficeBaseWindow : Window
    {
        protected Fixed _panelButtons;
        protected HBox _panelContent;
        protected Widget _currentPage;
        protected Label _labelActivePage;
        protected Label _labelTerminalInfo;
        protected Label _labelDateTime;
        protected Label _labelUpdate;
        protected HBox _statusBar;
        protected string _dataTimeFormat;
        protected Image _imageLogo = new Image();
        protected Label _reseller;
        public System.Drawing.Size _buttonsSize = new System.Drawing.Size(200,38);
        public Accordion Accordion { get; set; }

        public IconButtonWithText _btnDashboard;
        public IconButtonWithText _btnExit;
        public IconButtonWithText _btnPOS;
        public IconButtonWithText _btnNewVersion;

        public BackOfficeBaseWindow()
            : base(WindowType.Toplevel)
        {
            Title = Utils.GetWindowTitle("BackOfficeBaseWindow");
            SetPosition(WindowPosition.Center);
            Fullscreen();
            DesignUI();
            ShowAll();
            AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            DeleteEvent += OnCloseWindow;
        }

        private void DesignUI()
        {

            GlobalApp.BackOfficeScreenSize = Utils.GetScreenSize();
            uint borderWidth = 5;
            System.Drawing.Size sizeIconDashboard = new System.Drawing.Size(30, 30);
            System.Drawing.Size sizeIcon = new System.Drawing.Size(20, 20);
            System.Drawing.Size sizeIconQuit = new System.Drawing.Size(20, 20);
            System.Drawing.Size sizeButton = new System.Drawing.Size(20, 20);

            string fontPosBackOfficeParent = AppSettings.Instance.fontPosBackOfficeParent;
            string fontDescriptionParentLowRes = AppSettings.Instance.fontPosBackOfficeParentLowRes;
            string fontDescription = fontPosBackOfficeParent;

            if (GlobalApp.BackOfficeScreenSize.Height <= 800)
            {
                _buttonsSize = new System.Drawing.Size(200, 38);
                sizeIcon = new System.Drawing.Size(20, 20);
                sizeButton = new System.Drawing.Size(15, 15);
                sizeIconQuit = new System.Drawing.Size(20, 20);
                sizeIconDashboard = new System.Drawing.Size(20, 20);
                fontDescription = fontDescriptionParentLowRes;
            }

            _dataTimeFormat = GeneralUtils.GetResourceByName("backoffice_datetime_format_status_bar");

            string fontBackOfficeStatusBar = AppSettings.Instance.fontPosStatusBar;
            string fileImageBackOfficeLogoLong = PathsSettings.Paths["themes"] + @"Default\Images\logo_backoffice_long.png";
            string fileImageBackOfficeLogo = Utils.GetThemeFileLocation(AppSettings.Instance.fileImageBackOfficeLogo);

            //Colors
            System.Drawing.Color colorBackOfficeContentBackground = AppSettings.Instance.colorBackOfficeContentBackground;
            System.Drawing.Color colorBackOfficeStatusBarBackground = AppSettings.Instance.colorBackOfficeStatusBarBackground;
            System.Drawing.Color colorBackOfficeAccordionFixBackground = AppSettings.Instance.colorBackOfficeAccordionFixBackground;
            System.Drawing.Color colorBackOfficeStatusBarFont = AppSettings.Instance.colorBackOfficeStatusBarFont;
            System.Drawing.Color colorBackOfficeStatusBarBottomBackground = AppSettings.Instance.colorBackOfficeStatusBarBottomBackground;
            System.Drawing.Color colorLabelReseller = System.Drawing.Color.White;
            ModifyBg(StateType.Normal, colorBackOfficeContentBackground.ToGdkColor());

            SetWindowIcon();

            //Start Pack UI
            EventBox statusBar = new EventBox() { HeightRequest = 38 };
            statusBar.ModifyBg(StateType.Normal, colorBackOfficeStatusBarBackground.ToGdkColor());

            //Reseller
            _reseller = new Label();
            _reseller.Text = string.Format(" Powered by {0} ©", LicenseSettings.LicenseReseller);
            _reseller.ModifyFont(Pango.FontDescription.FromString("Trebuchet MS 8 Bold"));
            _reseller.ModifyFg(StateType.Normal, colorBackOfficeStatusBarFont.ToGdkColor());
            _reseller.Justify = Justification.Left;

            _imageLogo = new Image(fileImageBackOfficeLogoLong);

            if (LicenseSettings.LicenseReseller != null &&
                LicenseSettings.LicenseReseller.ToString() != "Logicpulse" &&
                LicenseSettings.LicenseReseller.ToString().ToLower() != "")
            {
                _imageLogo = new Image(fileImageBackOfficeLogo);
            }


            //Style StatusBarFont
            Pango.FontDescription fontDescriptionStatusBar = Pango.FontDescription.FromString(fontBackOfficeStatusBar);
            string _dashboardIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_dashboard.png";
            string _updateIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_update.png";
            string _exitIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_pos_close_backoffice.png";
            string _backPOSIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_pos_front_office.png";
            string _iconDashBoard = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_other_tables.png";

            //Active Content
            _labelActivePage = new Label() { WidthRequest = 300 };
            _labelActivePage.ModifyFont(fontDescriptionStatusBar);
            _labelActivePage.ModifyFg(StateType.Normal, colorBackOfficeStatusBarFont.ToGdkColor());
            _labelActivePage.SetAlignment(0.0F, 0.5F);

            //TerminalInfo : Terminal : User
            _labelTerminalInfo = new Label(string.Format("{0} : {1}", TerminalSettings.LoggedTerminal.Designation, XPOSettings.LoggedUser.Name));
            _labelTerminalInfo.ModifyFont(fontDescriptionStatusBar);
            _labelTerminalInfo.ModifyFg(StateType.Normal, colorBackOfficeStatusBarFont.ToGdkColor());
            _labelTerminalInfo.SetAlignment(0.5F, 0.5F);

            //Clock
            _labelDateTime = new Label(XPOUtility.CurrentDateTime(_dataTimeFormat));
            _labelDateTime.ModifyFont(fontDescriptionStatusBar);
            _labelDateTime.ModifyFg(StateType.Normal, colorBackOfficeStatusBarFont.ToGdkColor());
            _labelDateTime.SetAlignment(1.0F, 0.5F);

            //Pack HBox StatusBar
            _statusBar = new HBox(false, 0) { BorderWidth = borderWidth };
            _statusBar.PackStart(_imageLogo, false, false, 0);
            if (LicenseSettings.LicenseReseller != null && LicenseSettings.LicenseReseller.ToString() != "Logicpulse" && LicenseSettings.LicenceRegistered) _statusBar.PackStart(_reseller, false, false, 0);
            _statusBar.PackStart(_labelActivePage, false, false, 0);
            _statusBar.PackStart(_labelTerminalInfo, true, true, 0);


            //TODO:THEME
            if (GlobalApp.BackOfficeScreenSize.Width < 1024 || GlobalApp.BackOfficeScreenSize.Height < 768)
            {
                _labelTerminalInfo.SetAlignment(1.0F, 0.5F);
            }
            else
            {
                _statusBar.PackStart(_labelDateTime, false, false, 0);

            }

            if (GeneralSettings.AppUseBackOfficeMode)
            {
                EventBox eventBoxMinimize = GtkUtils.CreateMinimizeButton();
                eventBoxMinimize.ButtonReleaseEvent += delegate
                {
                    Iconify();
                };
                _statusBar.PackStart(eventBoxMinimize, false, false, 0);
            }

            _imageLogo.Dispose();

            _btnDashboard = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "DASHBOARD_ICON",
                    BackgroundColor = "168, 204, 79".StringToColor(),
                    Text = "Dashboard ",
                    Font = fontDescription,
                    FontColor = "61, 61, 61".StringToColor(),
                    Icon = _dashboardIcon,
                    IconSize = sizeIconDashboard,
                    ButtonSize = _buttonsSize,
                    LeftImage = true
                });

            _btnExit = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "EXIT_BUTTON",
                    BackgroundColor = "201, 102, 88".StringToColor(),
                    Text = GeneralUtils.GetResourceByName("global_quit"),
                    Font = fontDescription,
                    FontColor = "255, 255, 255".StringToColor(),
                    Icon = _exitIcon,
                    IconSize = sizeButton,
                    ButtonSize = _buttonsSize,
                    LeftImage = true
                });

            _btnPOS = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "POS",
                    BackgroundColor = "168, 204, 79".StringToColor(),
                    Text = "LogicPOS",
                    Font = fontDescription,
                    FontColor = "61, 61, 61".StringToColor(),
                    Icon = _backPOSIcon,
                    IconSize = sizeButton,
                    ButtonSize = _buttonsSize,
                    LeftImage = true
                });

            _btnNewVersion = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "Update_Button",
                    BackgroundColor = "168, 204, 79".StringToColor(),
                    Text = GeneralUtils.GetResourceByName("global_update_pos"),
                    Font = fontDescription,
                    FontColor = "61, 61, 61".StringToColor(),
                    Icon = _updateIcon,
                    IconSize = sizeButton,
                    ButtonSize = _buttonsSize,
                    LeftImage = true
                });

            _labelDateTime.ModifyFont(fontDescriptionStatusBar);

            _panelContent = new HBox(false, (int)borderWidth) { BorderWidth = borderWidth };

            _panelButtons = new Fixed() { HasWindow = true, BorderWidth = borderWidth };
            _panelButtons.ModifyBg(StateType.Normal, colorBackOfficeAccordionFixBackground.ToGdkColor());
            _panelButtons.Put(_btnDashboard, 0, 0);

            if (GeneralSettings.AppUseBackOfficeMode == false)
            {
                if (GlobalApp.BackOfficeScreenSize.Height <= 800)
                {
                    _panelButtons.Put(_btnPOS, 0, GlobalApp.BackOfficeScreenSize.Height - 112);

                }
                else
                {
                    _panelButtons.Put(_btnPOS, 0, GlobalApp.BackOfficeScreenSize.Height - 135);
                }
            }

            if (GlobalApp.BackOfficeScreenSize.Height <= 800)
            {
                _panelButtons.Put(_btnExit, 0, GlobalApp.BackOfficeScreenSize.Height - 85);
            }
            else
            {
                _panelButtons.Put(_btnExit, 0, GlobalApp.BackOfficeScreenSize.Height - 95);
            }

            CheckForUpdates(fontDescriptionStatusBar);

            _panelContent.PackStart(_panelButtons, false, false, 0);

            VBox verticalContent = new VBox(false, 0);

            statusBar.Add(_statusBar);

            verticalContent.PackStart(statusBar, false, false, 0);

            verticalContent.PackStart(_panelContent);

            HeightRequest = 50;

            Add(verticalContent);

            StartClock();
        }

        private void CheckForUpdates(FontDescription fontDescriptionStatusBar)
        {
            string appVersion = GeneralSettings.ProductVersion.Replace("v", "");

            bool needToUpdate = false;

            return; //tchial0 -> Disable update check

            if (GeneralSettings.ServerVersion != null)
            {

                string[] tmpNew = appVersion.Split('.');
                long tmpNewVer = int.Parse(tmpNew[0]) * 10000000 + int.Parse(tmpNew[1]) * 10000 + int.Parse(tmpNew[2]);

                string[] tmpOld = GeneralSettings.ServerVersion.ToString().Split('.');
                long tmpOldVer = int.Parse(tmpOld[0]) * 10000000 + int.Parse(tmpOld[1]) * 10000 + int.Parse(tmpOld[2]);

                if (tmpNewVer < tmpOldVer)
                {
                    needToUpdate = true;
                }

                if (needToUpdate)
                {
                    if (GeneralSettings.AppUseBackOfficeMode)
                    {
                        if (GlobalApp.BackOfficeScreenSize.Height <= 800)
                        {
                            _labelUpdate = new Label(string.Format(string.Format(GeneralUtils.GetResourceByName("global_new_version"), GeneralSettings.ServerVersion.ToString())));
                            _labelUpdate.ModifyFont(fontDescriptionStatusBar);
                            _labelUpdate.ModifyFg(StateType.Normal, "61, 61, 61".StringToColor().ToGdkColor());
                            _labelUpdate.SetAlignment(1.0F, 0.5F);
                            _panelButtons.Put(_labelUpdate, 5, GlobalApp.BackOfficeScreenSize.Height - 165);
                            _panelButtons.Add(_labelUpdate);
                            _panelButtons.Put(_btnNewVersion, 0, GlobalApp.BackOfficeScreenSize.Height - 140);
                            _panelButtons.Add(_btnNewVersion);
                        }
                        else
                        {
                            _labelUpdate = new Label(string.Format(string.Format(GeneralUtils.GetResourceByName("global_new_version"), GeneralSettings.ServerVersion.ToString())));
                            _labelUpdate.ModifyFont(fontDescriptionStatusBar);
                            _labelUpdate.ModifyFg(StateType.Normal, "61, 61, 61".StringToColor().ToGdkColor());
                            _labelUpdate.SetAlignment(1.0F, 0.5F);
                            _panelButtons.Put(_labelUpdate, 5, GlobalApp.BackOfficeScreenSize.Height - 160);
                            _panelButtons.Add(_labelUpdate);
                            _panelButtons.Put(_btnNewVersion, 0, GlobalApp.BackOfficeScreenSize.Height - 135);
                            _panelButtons.Add(_btnNewVersion);
                        }
                    }
                    else
                    {
                        if (GlobalApp.BackOfficeScreenSize.Height <= 800)
                        {
                            _labelUpdate = new Label(string.Format(string.Format(GeneralUtils.GetResourceByName("global_new_version"), GeneralSettings.ServerVersion.ToString())));
                            _labelUpdate.ModifyFont(fontDescriptionStatusBar);
                            _labelUpdate.ModifyFg(StateType.Normal, "61, 61, 61".StringToColor().ToGdkColor());
                            _labelUpdate.SetAlignment(1.0F, 0.5F);
                            _panelButtons.Put(_labelUpdate, 5, GlobalApp.BackOfficeScreenSize.Height - 165);
                            _panelButtons.Add(_labelUpdate);
                            _panelButtons.Put(_btnNewVersion, 0, GlobalApp.BackOfficeScreenSize.Height - 140);
                            _panelButtons.Add(_btnNewVersion);
                        }
                        else
                        {
                            _labelUpdate = new Label(string.Format(string.Format(GeneralUtils.GetResourceByName("global_new_version"), GeneralSettings.ServerVersion.ToString())));
                            _labelUpdate.ModifyFont(fontDescriptionStatusBar);
                            _labelUpdate.ModifyFg(StateType.Normal, "61, 61, 61".StringToColor().ToGdkColor());
                            _labelUpdate.SetAlignment(1.0F, 0.5F);
                            _panelButtons.Put(_labelUpdate, 5, GlobalApp.BackOfficeScreenSize.Height - 200);
                            _panelButtons.Add(_labelUpdate);
                            _panelButtons.Put(_btnNewVersion, 0, GlobalApp.BackOfficeScreenSize.Height - 175);
                            _panelButtons.Add(_btnNewVersion);
                        }

                    }
                }
            }
        }

        private void SetWindowIcon()
        {
            string fileImageAppIcon = $"{PathsSettings.ImagesFolderLocation}{POSSettings.AppIcon}";
            if (File.Exists(fileImageAppIcon)) Icon = Utils.ImageToPixbuf(System.Drawing.Image.FromFile(fileImageAppIcon));
        }

        private void OnCloseWindow(object o, DeleteEventArgs args)
        {
            Hide();
            GlobalApp.PosMainWindow.ShowAll();
            args.RetVal = true;
        }

        protected void AccordionButton_Click(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(AccordionParentButton))
            {
                return;
            }

            var button = (AccordionChildButton)sender;

            if (button.Page != null)
            {
                if (_currentPage != null)
                {
                    _panelContent.Remove(_currentPage);
                }

                _currentPage = button.Page;
                Accordion.CurrentPageChildButton = button;
                _labelActivePage.Text = button.Label;

                 _currentPage.Visible = true;
                
                _panelContent.PackStart(_currentPage);

                return;
            }

            if (button.ExternalApplication != null)
            {
                GeneralUtils.ExecuteExternalProcess(button.ExternalApplication);
            }
        }

        private void StartClock()
        {
            GLib.Timeout.Add(1000, new GLib.TimeoutHandler(UpdateClock));
        }

        private bool UpdateClock()
        {
            _labelDateTime.Text = XPOUtility.CurrentDateTime(_dataTimeFormat);
            return true;
        }
    }
}
