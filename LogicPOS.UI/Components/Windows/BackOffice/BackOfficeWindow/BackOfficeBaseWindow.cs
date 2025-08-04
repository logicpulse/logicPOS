using Gtk;
using logicpos;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Application.Screen;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using Pango;
using System;
using System.IO;

namespace LogicPOS.UI.Components.Windows
{
    public abstract class BackOfficeBaseWindow : Window
    {
        protected Fixed PanelLeft { get; set; }
        protected VBox PanelButtons { get; } = new VBox(false, 2) { WidthRequest = 200 };
        protected HBox PageContainer { get; set; }
        protected Widget CurrentPage { get; set; }
        protected Label LabelActivePage { get; set; }
        protected Label LabelTerminalInfo { get; set; }
        protected Label LabelDateTime { get; set; }
        protected Label LabelUpdate { get; set; }
        protected HBox StatusBar { get; set; }
        protected string DateTimeFormat { get; set; }
        protected Image Logo { get; set; } = new Image();
        protected Label Reseller { get; set; }
        public System.Drawing.Size ButtonSize = new System.Drawing.Size(200, 38);
        public IconButtonWithText BtnDashboard { get; set; }
        public IconButtonWithText BtnExit { get; set; }
        public IconButtonWithText BtnPOS { get; set; }
        public IconButtonWithText BtnNewVersion { get; set; }

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
            BackOfficeWindow.ScreenSize = ScreenSizeUtil.GetScreenSize();
            uint borderWidth = 5;
            System.Drawing.Size sizeIconDashboard = new System.Drawing.Size(30, 30);
            System.Drawing.Size sizeIcon = new System.Drawing.Size(20, 20);
            System.Drawing.Size sizeIconQuit = new System.Drawing.Size(20, 20);
            System.Drawing.Size sizeButton = new System.Drawing.Size(20, 20);

            string fontPosBackOfficeParent = AppSettings.Instance.FontPosBackOfficeParent;
            string fontDescriptionParentLowRes = AppSettings.Instance.FontPosBackOfficeParentLowRes;
            string fontDescription = fontPosBackOfficeParent;

            if (BackOfficeWindow.ScreenSize.Height <= 800)
            {
                ButtonSize = new System.Drawing.Size(200, 38);
                sizeIcon = new System.Drawing.Size(20, 20);
                sizeButton = new System.Drawing.Size(15, 15);
                sizeIconQuit = new System.Drawing.Size(20, 20);
                sizeIconDashboard = new System.Drawing.Size(20, 20);
                fontDescription = fontDescriptionParentLowRes;
            }

            DateTimeFormat = GeneralUtils.GetResourceByName("backoffice_datetime_format_status_bar");

            string fontBackOfficeStatusBar = AppSettings.Instance.FontPosStatusBar;
            string fileImageBackOfficeLogoLong = AppSettings.Paths.Themes + @"Default\Images\logo_backoffice_long.png";
            string fileImageBackOfficeLogo = Utils.GetThemeFileLocation(AppSettings.Instance.FileImageBackOfficeLogo);

            //Colors
            System.Drawing.Color colorBackOfficeContentBackground = AppSettings.Instance.ColorBackOfficeContentBackground;
            System.Drawing.Color colorBackOfficeStatusBarBackground = AppSettings.Instance.ColorBackOfficeStatusBarBackground;
            System.Drawing.Color colorBackOfficeAccordionFixBackground = AppSettings.Instance.ColorBackOfficeAccordionFixBackground;
            System.Drawing.Color colorBackOfficeStatusBarFont = AppSettings.Instance.ColorBackOfficeStatusBarFont;
            System.Drawing.Color colorBackOfficeStatusBarBottomBackground = AppSettings.Instance.ColorBackOfficeStatusBarBottomBackground;
            System.Drawing.Color colorLabelReseller = System.Drawing.Color.White;
            ModifyBg(StateType.Normal, colorBackOfficeContentBackground.ToGdkColor());

            SetWindowIcon();

            //Start Pack UI
            EventBox statusBar = new EventBox() { HeightRequest = 38 };
            statusBar.ModifyBg(StateType.Normal, colorBackOfficeStatusBarBackground.ToGdkColor());

            //Reseller
            Reseller = new Label();
            Reseller.Text = string.Format(" Powered by {0} ©", AppSettings.License.LicenseData.Reseller);
            Reseller.ModifyFont(Pango.FontDescription.FromString("Trebuchet MS 8 Bold"));
            Reseller.ModifyFg(StateType.Normal, colorBackOfficeStatusBarFont.ToGdkColor());
            Reseller.Justify = Justification.Left;

            Logo = new Image(fileImageBackOfficeLogoLong);

            if (AppSettings.License.LicenseData.Reseller != null &&
                AppSettings.License.LicenseData.Reseller.ToString() != "Logicpulse" &&
                AppSettings.License.LicenseData.Reseller.ToString().ToLower() != "")
            {
                Logo = new Image(fileImageBackOfficeLogo);
            }


            //Style StatusBarFont
            Pango.FontDescription fontDescriptionStatusBar = Pango.FontDescription.FromString(fontBackOfficeStatusBar);
            string _dashboardIcon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_dashboard.png";
            string _updateIcon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_update.png";
            string _exitIcon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_pos_close_backoffice.png";
            string _backPOSIcon = AppSettings.Paths.Images + @"Icons\BackOffice\icon_pos_front_office.png";
            string _iconDashBoard = AppSettings.Paths.Images + @"Icons\BackOffice\icon_other_tables.png";

            //Active Content
            LabelActivePage = new Label() { WidthRequest = 300 };
            LabelActivePage.ModifyFont(fontDescriptionStatusBar);
            LabelActivePage.ModifyFg(StateType.Normal, colorBackOfficeStatusBarFont.ToGdkColor());
            LabelActivePage.SetAlignment(0.0F, 0.5F);

            //TerminalInfo : Terminal : User
            LabelTerminalInfo = new Label($"{TerminalService.Terminal.Designation} : {AuthenticationService.User.Name}");
            LabelTerminalInfo.ModifyFont(fontDescriptionStatusBar);
            LabelTerminalInfo.ModifyFg(StateType.Normal, colorBackOfficeStatusBarFont.ToGdkColor());
            LabelTerminalInfo.SetAlignment(0.5F, 0.5F);

            //Clock
            LabelDateTime = new Label(DateTime.Now.ToString());
            LabelDateTime.ModifyFont(fontDescriptionStatusBar);
            LabelDateTime.ModifyFg(StateType.Normal, colorBackOfficeStatusBarFont.ToGdkColor());
            LabelDateTime.SetAlignment(1.0F, 0.5F);

            //Pack HBox StatusBar
            StatusBar = new HBox(false, 0) { BorderWidth = borderWidth };
            StatusBar.PackStart(Logo, false, false, 0);
            if (AppSettings.License.LicenseData.Reseller != null && AppSettings.License.LicenseData.Reseller.ToString() != "Logicpulse" && AppSettings.License.LicenseData.Registered) StatusBar.PackStart(Reseller, false, false, 0);
            StatusBar.PackStart(LabelActivePage, false, false, 0);
            StatusBar.PackStart(LabelTerminalInfo, true, true, 0);


            //TODO:THEME
            if (BackOfficeWindow.ScreenSize.Width < 1024 || BackOfficeWindow.ScreenSize.Height < 768)
            {
                LabelTerminalInfo.SetAlignment(1.0F, 0.5F);
            }
            else
            {
                StatusBar.PackStart(LabelDateTime, false, false, 0);

            }

            if (AppSettings.Instance.UseBackOfficeMode)
            {
                EventBox eventBoxMinimize = GtkUtils.CreateMinimizeButton();
                eventBoxMinimize.ButtonReleaseEvent += delegate
                {
                    Iconify();
                };
                StatusBar.PackStart(eventBoxMinimize, false, false, 0);
            }

            Logo.Dispose();

            BtnDashboard = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButton_Green",
                    Text = "Dashboard",
                    Font = fontDescription,
                    FontColor = "61, 61, 61".StringToColor(),
                    Icon = _dashboardIcon,
                    IconSize = sizeIconDashboard,
                    ButtonSize = ButtonSize,
                    LeftImage = true
                });

            BtnExit = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButton_Red",
                    Text = GeneralUtils.GetResourceByName("global_quit"),
                    Font = fontDescription,
                    FontColor = "255, 255, 255".StringToColor(),
                    Icon = _exitIcon,
                    IconSize = sizeButton,
                    ButtonSize = ButtonSize,
                    LeftImage = true
                });

            BtnPOS = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButton_Green",
                    Text = "LogicPOS",
                    Font = fontDescription,
                    FontColor = "61, 61, 61".StringToColor(),
                    Icon = _backPOSIcon,
                    IconSize = sizeButton,
                    ButtonSize = ButtonSize,
                    LeftImage = true
                });

            BtnNewVersion = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "Update_Button",
                    BackgroundColor = "168, 204, 79".StringToColor(),
                    Text = GeneralUtils.GetResourceByName("global_update_pos"),
                    Font = fontDescription,
                    FontColor = "61, 61, 61".StringToColor(),
                    Icon = _updateIcon,
                    IconSize = sizeButton,
                    ButtonSize = ButtonSize,
                    LeftImage = true
                });

            LabelDateTime.ModifyFont(fontDescriptionStatusBar);

            PageContainer = new HBox(false, (int)borderWidth) { BorderWidth = borderWidth };

            PanelLeft = new Fixed() { HasWindow = true, BorderWidth = borderWidth };
            PanelLeft.ModifyBg(StateType.Normal, colorBackOfficeAccordionFixBackground.ToGdkColor());
            PanelLeft.Put(BtnDashboard, 0, 0);

            if (AppSettings.Instance.UseBackOfficeMode == false)
            {
                if (BackOfficeWindow.ScreenSize.Height <= 800)
                {
                    PanelLeft.Put(BtnPOS, 0, BackOfficeWindow.ScreenSize.Height - 140);

                }
                else
                {
                    PanelLeft.Put(BtnPOS, 0, BackOfficeWindow.ScreenSize.Height - 135);
                }
            }

            if (BackOfficeWindow.ScreenSize.Height <= 800)
            {
                PanelLeft.Put(BtnExit, 0, BackOfficeWindow.ScreenSize.Height - 100);
                PanelLeft.Put(PanelButtons, 0, 40);
            }
            else
            {
                PanelLeft.Put(BtnExit, 0, BackOfficeWindow.ScreenSize.Height - 95);
                PanelLeft.Put(PanelButtons, 0, 40);
            }

            CheckForUpdates(fontDescriptionStatusBar);

            PageContainer.PackStart(PanelLeft, false, false, 0);

            VBox verticalContent = new VBox(false, 0);

            statusBar.Add(StatusBar);

            verticalContent.PackStart(statusBar, false, false, 0);

            verticalContent.PackStart(PageContainer);

            HeightRequest = 50;

            Add(verticalContent);

            StartClock();
        }

        private void CheckForUpdates(FontDescription fontDescriptionStatusBar)
        {
            string appVersion = AppSettings.ProductVersion.Replace("v", "");

            bool needToUpdate = false;

            return; //tchial0 -> Disable update check
            /*
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
                        if (BackOfficeWindow.ScreenSize.Height <= 800)
                        {
                            LabelUpdate = new Label(string.Format(string.Format(GeneralUtils.GetResourceByName("global_new_version"), GeneralSettings.ServerVersion.ToString())));
                            LabelUpdate.ModifyFont(fontDescriptionStatusBar);
                            LabelUpdate.ModifyFg(StateType.Normal, "61, 61, 61".StringToColor().ToGdkColor());
                            LabelUpdate.SetAlignment(1.0F, 0.5F);
                            PanelButtons.Put(LabelUpdate, 5, BackOfficeWindow.ScreenSize.Height - 165);
                            PanelButtons.Add(LabelUpdate);
                            PanelButtons.Put(BtnNewVersion, 0, BackOfficeWindow.ScreenSize.Height - 140);
                            PanelButtons.Add(BtnNewVersion);
                        }
                        else
                        {
                            LabelUpdate = new Label(string.Format(string.Format(GeneralUtils.GetResourceByName("global_new_version"), GeneralSettings.ServerVersion.ToString())));
                            LabelUpdate.ModifyFont(fontDescriptionStatusBar);
                            LabelUpdate.ModifyFg(StateType.Normal, "61, 61, 61".StringToColor().ToGdkColor());
                            LabelUpdate.SetAlignment(1.0F, 0.5F);
                            PanelButtons.Put(LabelUpdate, 5, BackOfficeWindow.ScreenSize.Height - 160);
                            PanelButtons.Add(LabelUpdate);
                            PanelButtons.Put(BtnNewVersion, 0, BackOfficeWindow.ScreenSize.Height - 135);
                            PanelButtons.Add(BtnNewVersion);
                        }
                    }
                    else
                    {
                        if (BackOfficeWindow.ScreenSize.Height <= 800)
                        {
                            LabelUpdate = new Label(string.Format(string.Format(GeneralUtils.GetResourceByName("global_new_version"), GeneralSettings.ServerVersion.ToString())));
                            LabelUpdate.ModifyFont(fontDescriptionStatusBar);
                            LabelUpdate.ModifyFg(StateType.Normal, "61, 61, 61".StringToColor().ToGdkColor());
                            LabelUpdate.SetAlignment(1.0F, 0.5F);
                            PanelButtons.Put(LabelUpdate, 5, BackOfficeWindow.ScreenSize.Height - 165);
                            PanelButtons.Add(LabelUpdate);
                            PanelButtons.Put(BtnNewVersion, 0, BackOfficeWindow.ScreenSize.Height - 140);
                            PanelButtons.Add(BtnNewVersion);
                        }
                        else
                        {
                            LabelUpdate = new Label(string.Format(string.Format(GeneralUtils.GetResourceByName("global_new_version"), GeneralSettings.ServerVersion.ToString())));
                            LabelUpdate.ModifyFont(fontDescriptionStatusBar);
                            LabelUpdate.ModifyFg(StateType.Normal, "61, 61, 61".StringToColor().ToGdkColor());
                            LabelUpdate.SetAlignment(1.0F, 0.5F);
                            PanelButtons.Put(LabelUpdate, 5, BackOfficeWindow.ScreenSize.Height - 200);
                            PanelButtons.Add(LabelUpdate);
                            PanelButtons.Put(BtnNewVersion, 0, BackOfficeWindow.ScreenSize.Height - 175);
                            PanelButtons.Add(BtnNewVersion);
                        }

                    }
                }
            
            }*/
        }

        private void SetWindowIcon()
        {
            string fileImageAppIcon = $"{AppSettings.Paths.Images}{AppSettings.AppIcon}";
            if (File.Exists(fileImageAppIcon)) Icon = Utils.ImageToPixbuf(System.Drawing.Image.FromFile(fileImageAppIcon));
        }

        private void OnCloseWindow(object o, DeleteEventArgs args)
        {
            if (CustomAlerts.ShowQuitConfirmationAlert(this))
            {
                Gtk.Application.Quit();
            }

            args.RetVal = true; 
        }

        public void ShowPage(Widget page, string pageTitle)
        {
            if (CurrentPage != null)
            {
                PageContainer.Remove(CurrentPage);
            }

            CurrentPage = page;

            LabelActivePage.Text = pageTitle;

            CurrentPage.Visible = true;

            PageContainer.PackStart(CurrentPage);
        }

        private void StartClock()
        {
            GLib.Timeout.Add(1000, new GLib.TimeoutHandler(UpdateClock));
        }

        private bool UpdateClock()
        {
            LabelDateTime.Text = DateTime.Now.ToString(DateTimeFormat);
            return true;
        }
    }
}
