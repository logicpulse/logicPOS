﻿using Gtk;
using logicpos;
using logicpos.App;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Accordions;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using Pango;
using System;
using System.IO;

namespace LogicPOS.UI.Components.BackOffice.Windows
{
    public abstract class BackOfficeBaseWindow : Window
    {
        protected Fixed PanelButtons { get; set; }
        protected HBox PanelContent { get; set; }
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
        public Accordion Menu { get; set; }
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
                ButtonSize = new System.Drawing.Size(200, 38);
                sizeIcon = new System.Drawing.Size(20, 20);
                sizeButton = new System.Drawing.Size(15, 15);
                sizeIconQuit = new System.Drawing.Size(20, 20);
                sizeIconDashboard = new System.Drawing.Size(20, 20);
                fontDescription = fontDescriptionParentLowRes;
            }

            DateTimeFormat = GeneralUtils.GetResourceByName("backoffice_datetime_format_status_bar");

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
            Reseller = new Label();
            Reseller.Text = string.Format(" Powered by {0} ©", LicenseSettings.LicenseReseller);
            Reseller.ModifyFont(Pango.FontDescription.FromString("Trebuchet MS 8 Bold"));
            Reseller.ModifyFg(StateType.Normal, colorBackOfficeStatusBarFont.ToGdkColor());
            Reseller.Justify = Justification.Left;

            Logo = new Image(fileImageBackOfficeLogoLong);

            if (LicenseSettings.LicenseReseller != null &&
                LicenseSettings.LicenseReseller.ToString() != "Logicpulse" &&
                LicenseSettings.LicenseReseller.ToString().ToLower() != "")
            {
                Logo = new Image(fileImageBackOfficeLogo);
            }


            //Style StatusBarFont
            Pango.FontDescription fontDescriptionStatusBar = Pango.FontDescription.FromString(fontBackOfficeStatusBar);
            string _dashboardIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_dashboard.png";
            string _updateIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_update.png";
            string _exitIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_pos_close_backoffice.png";
            string _backPOSIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_pos_front_office.png";
            string _iconDashBoard = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_other_tables.png";

            //Active Content
            LabelActivePage = new Label() { WidthRequest = 300 };
            LabelActivePage.ModifyFont(fontDescriptionStatusBar);
            LabelActivePage.ModifyFg(StateType.Normal, colorBackOfficeStatusBarFont.ToGdkColor());
            LabelActivePage.SetAlignment(0.0F, 0.5F);

            //TerminalInfo : Terminal : User
            LabelTerminalInfo = new Label(string.Format("{0} : {1}", TerminalSettings.LoggedTerminal.Designation, XPOSettings.LoggedUser.Name));
            LabelTerminalInfo.ModifyFont(fontDescriptionStatusBar);
            LabelTerminalInfo.ModifyFg(StateType.Normal, colorBackOfficeStatusBarFont.ToGdkColor());
            LabelTerminalInfo.SetAlignment(0.5F, 0.5F);

            //Clock
            LabelDateTime = new Label(XPOUtility.CurrentDateTime(DateTimeFormat));
            LabelDateTime.ModifyFont(fontDescriptionStatusBar);
            LabelDateTime.ModifyFg(StateType.Normal, colorBackOfficeStatusBarFont.ToGdkColor());
            LabelDateTime.SetAlignment(1.0F, 0.5F);

            //Pack HBox StatusBar
            StatusBar = new HBox(false, 0) { BorderWidth = borderWidth };
            StatusBar.PackStart(Logo, false, false, 0);
            if (LicenseSettings.LicenseReseller != null && LicenseSettings.LicenseReseller.ToString() != "Logicpulse" && LicenseSettings.LicenceRegistered) StatusBar.PackStart(Reseller, false, false, 0);
            StatusBar.PackStart(LabelActivePage, false, false, 0);
            StatusBar.PackStart(LabelTerminalInfo, true, true, 0);


            //TODO:THEME
            if (GlobalApp.BackOfficeScreenSize.Width < 1024 || GlobalApp.BackOfficeScreenSize.Height < 768)
            {
                LabelTerminalInfo.SetAlignment(1.0F, 0.5F);
            }
            else
            {
                StatusBar.PackStart(LabelDateTime, false, false, 0);

            }

            if (GeneralSettings.AppUseBackOfficeMode)
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
                    Name = "DASHBOARD_ICON",
                    BackgroundColor = "168, 204, 79".StringToColor(),
                    Text = "Dashboard ",
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
                    Name = "EXIT_BUTTON",
                    BackgroundColor = "201, 102, 88".StringToColor(),
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
                    Name = "POS",
                    BackgroundColor = "168, 204, 79".StringToColor(),
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

            PanelContent = new HBox(false, (int)borderWidth) { BorderWidth = borderWidth };

            PanelButtons = new Fixed() { HasWindow = true, BorderWidth = borderWidth };
            PanelButtons.ModifyBg(StateType.Normal, colorBackOfficeAccordionFixBackground.ToGdkColor());
            PanelButtons.Put(BtnDashboard, 0, 0);

            if (GeneralSettings.AppUseBackOfficeMode == false)
            {
                if (GlobalApp.BackOfficeScreenSize.Height <= 800)
                {
                    PanelButtons.Put(BtnPOS, 0, GlobalApp.BackOfficeScreenSize.Height - 112);

                }
                else
                {
                    PanelButtons.Put(BtnPOS, 0, GlobalApp.BackOfficeScreenSize.Height - 135);
                }
            }

            if (GlobalApp.BackOfficeScreenSize.Height <= 800)
            {
                PanelButtons.Put(BtnExit, 0, GlobalApp.BackOfficeScreenSize.Height - 85);
            }
            else
            {
                PanelButtons.Put(BtnExit, 0, GlobalApp.BackOfficeScreenSize.Height - 95);
            }

            CheckForUpdates(fontDescriptionStatusBar);

            PanelContent.PackStart(PanelButtons, false, false, 0);

            VBox verticalContent = new VBox(false, 0);

            statusBar.Add(StatusBar);

            verticalContent.PackStart(statusBar, false, false, 0);

            verticalContent.PackStart(PanelContent);

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
                            LabelUpdate = new Label(string.Format(string.Format(GeneralUtils.GetResourceByName("global_new_version"), GeneralSettings.ServerVersion.ToString())));
                            LabelUpdate.ModifyFont(fontDescriptionStatusBar);
                            LabelUpdate.ModifyFg(StateType.Normal, "61, 61, 61".StringToColor().ToGdkColor());
                            LabelUpdate.SetAlignment(1.0F, 0.5F);
                            PanelButtons.Put(LabelUpdate, 5, GlobalApp.BackOfficeScreenSize.Height - 165);
                            PanelButtons.Add(LabelUpdate);
                            PanelButtons.Put(BtnNewVersion, 0, GlobalApp.BackOfficeScreenSize.Height - 140);
                            PanelButtons.Add(BtnNewVersion);
                        }
                        else
                        {
                            LabelUpdate = new Label(string.Format(string.Format(GeneralUtils.GetResourceByName("global_new_version"), GeneralSettings.ServerVersion.ToString())));
                            LabelUpdate.ModifyFont(fontDescriptionStatusBar);
                            LabelUpdate.ModifyFg(StateType.Normal, "61, 61, 61".StringToColor().ToGdkColor());
                            LabelUpdate.SetAlignment(1.0F, 0.5F);
                            PanelButtons.Put(LabelUpdate, 5, GlobalApp.BackOfficeScreenSize.Height - 160);
                            PanelButtons.Add(LabelUpdate);
                            PanelButtons.Put(BtnNewVersion, 0, GlobalApp.BackOfficeScreenSize.Height - 135);
                            PanelButtons.Add(BtnNewVersion);
                        }
                    }
                    else
                    {
                        if (GlobalApp.BackOfficeScreenSize.Height <= 800)
                        {
                            LabelUpdate = new Label(string.Format(string.Format(GeneralUtils.GetResourceByName("global_new_version"), GeneralSettings.ServerVersion.ToString())));
                            LabelUpdate.ModifyFont(fontDescriptionStatusBar);
                            LabelUpdate.ModifyFg(StateType.Normal, "61, 61, 61".StringToColor().ToGdkColor());
                            LabelUpdate.SetAlignment(1.0F, 0.5F);
                            PanelButtons.Put(LabelUpdate, 5, GlobalApp.BackOfficeScreenSize.Height - 165);
                            PanelButtons.Add(LabelUpdate);
                            PanelButtons.Put(BtnNewVersion, 0, GlobalApp.BackOfficeScreenSize.Height - 140);
                            PanelButtons.Add(BtnNewVersion);
                        }
                        else
                        {
                            LabelUpdate = new Label(string.Format(string.Format(GeneralUtils.GetResourceByName("global_new_version"), GeneralSettings.ServerVersion.ToString())));
                            LabelUpdate.ModifyFont(fontDescriptionStatusBar);
                            LabelUpdate.ModifyFg(StateType.Normal, "61, 61, 61".StringToColor().ToGdkColor());
                            LabelUpdate.SetAlignment(1.0F, 0.5F);
                            PanelButtons.Put(LabelUpdate, 5, GlobalApp.BackOfficeScreenSize.Height - 200);
                            PanelButtons.Add(LabelUpdate);
                            PanelButtons.Put(BtnNewVersion, 0, GlobalApp.BackOfficeScreenSize.Height - 175);
                            PanelButtons.Add(BtnNewVersion);
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

        protected void MenuButton_Clicked(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(AccordionParentButton))
            {
                return;
            }

            var button = (AccordionChildButton)sender;

            if (button.Page != null)
            {
                if (CurrentPage != null)
                {
                    PanelContent.Remove(CurrentPage);
                }

                CurrentPage = button.Page;
                Menu.CurrentPageChildButton = button;
                LabelActivePage.Text = button.Label;

                CurrentPage.Visible = true;

                PanelContent.PackStart(CurrentPage);

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
            LabelDateTime.Text = XPOUtility.CurrentDateTime(DateTimeFormat);
            return true;
        }
    }
}
