using Gdk;
using Gtk;
using logicpos;
using logicpos.Classes.Logic.Others;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Menus;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using System;
using Image = Gtk.Image;


namespace LogicPOS.UI.Components.Windows
{
    public partial class LoginWindow 
    {
        private UserPinPanel PinPanel { get; set; }
        public UsersMenu MenuUsers { get; set; }

        private void InitializeUI()
        {
            dynamic theme = GetTheme();

            string errorMessage = "Node: <Window ID=\"StartupWindow\">";

            //Assign Theme Vars + UI
            if (theme != null)
            {
                //Globals
                Title = Convert.ToString(theme.Globals.Name);
                //Objects:LabelVersion
                System.Drawing.Point labelVersionPosition = Utils.StringToPosition(theme.Objects.LabelVersion.Position);
                string labelVersionFont = theme.Objects.LabelVersion.Font;
                Color labelVersionFontColor = (theme.Objects.LabelVersion.FontColor as string).StringToGdkColor();
                //Objects:NumberPadPin
                System.Drawing.Point numberPadPinPosition = Utils.StringToPosition(theme.Objects.NumberPadPin.Position);
                System.Drawing.Size numberPadPinButtonSize = (theme.Objects.NumberPadPin.ButtonSize as string).ToSize();
                System.Drawing.Color numberPadPinFontColor = (theme.Objects.NumberPadPin.FontColor as string).StringToColor();
                uint numberPadPinRowSpacingSystemButtons = Convert.ToUInt16(theme.Objects.NumberPadPin.RowSpacingSystemButtons);
                uint numberPadPinRowSpacingLabelStatus = Convert.ToUInt16(theme.Objects.NumberPadPin.RowSpacingLabelStatus);
                //Objects:NumberPadPin:LabelStatus
                System.Drawing.Color numberPadPinLabelStatusFontColor = (theme.Objects.NumberPadPin.LabelStatus.FontColor as string).StringToColor();
                //Objects:NumberPadPin:Size (EventBox)
                System.Drawing.Size numberPadPinSize = (theme.Objects.NumberPadPin.Size as string).ToSize();


                //Objects:TablePadUserButtonPrev
                System.Drawing.Point tablePadUserButtonPrevPosition = Utils.StringToPosition(theme.Objects.TablePadUser.TablePadUserButtonPrev.Position);
                System.Drawing.Size tablePadUserButtonPrevSize = (theme.Objects.TablePadUser.TablePadUserButtonPrev.Size as string).ToSize();
                string tablePadUserButtonPrevImageFileName = theme.Objects.TablePadUser.TablePadUserButtonPrev.ImageFileName;
                //Objects:TablePadUserButtonNext
                System.Drawing.Point tablePadUserButtonNextPosition = Utils.StringToPosition(theme.Objects.TablePadUser.TablePadUserButtonNext.Position);
                System.Drawing.Size tablePadUserButtonNextSize = (theme.Objects.TablePadUser.TablePadUserButtonNext.Size as string).ToSize();
                string tablePadUserButtonNextImageFileName = theme.Objects.TablePadUser.TablePadUserButtonNext.ImageFileName;
                //Objects:TablePadUser
                System.Drawing.Point tablePadUserPosition = Utils.StringToPosition(theme.Objects.TablePadUser.Position);
                System.Drawing.Size tablePadUserButtonSize = (theme.Objects.TablePadUser.ButtonSize as string).ToSize();
                TableConfig tablePadUserTableConfig = Utils.StringToTableConfig(theme.Objects.TablePadUser.TableConfig);
                bool showUsersMenu = Convert.ToBoolean(theme.Objects.TablePadUser.Visible);

                //Init UI
                Fixed fix = new Fixed();


                if (AppSettings.Instance.AppShowMinimize)
                {
                    EventBox eventBoxMinimize = GtkUtils.CreateMinimizeButton();
                    eventBoxMinimize.ButtonReleaseEvent += delegate { Iconify(); };
                    fix.Put(eventBoxMinimize, AppSettings.Instance.AppScreenSize.Width - 27 - 10, 10);
                }

                //NumberPadPin
                PinPanel = new UserPinPanel(parentWindow: this,
                                            numberPadPinLabelStatusFontColor,
                                            numberPadPinButtonSize,
                                            false,
                                            true,
                                            numberPadPinRowSpacingLabelStatus,
                                            numberPadPinRowSpacingSystemButtons);

                if (numberPadPinSize.Width > 0 || numberPadPinSize.Height > 0)
                {
                    PinPanel.Eventbox.WidthRequest = numberPadPinSize.Width;
                    PinPanel.Eventbox.HeightRequest = numberPadPinSize.Height;
                }

                //Put in Fix
                fix.Put(PinPanel, numberPadPinPosition.X, numberPadPinPosition.Y);
                //Over NumberPadPin
                //fix.Put(touchButtonKeyPasswordReset, numberPadPinButtonPasswordResetPosition.X, numberPadPinButtonPasswordResetPosition.Y);
                //Events
                PinPanel.BtnOk.Clicked += PinPanel_BtnOK_Clicked;
                PinPanel.BtnResetPassword.Clicked += BtnResetPassword_Clicked;
                //PinPanel.ButtonKeyFrontOffice.Clicked += ButtonKeyFrontOffice_Clicked;
                PinPanel.BtnQuit.Clicked += BtnQuit_Clicked;

                //Objects:TablePadUserButtonPrev
                IconButton tablePadUserButtonPrev = new IconButton(
                    new ButtonSettings
                    {
                        Name = "TablePadUserButtonPrev",
                        Icon = tablePadUserButtonPrevImageFileName,
                        IconSize = new System.Drawing.Size(tablePadUserButtonPrevSize.Width - 2, tablePadUserButtonPrevSize.Height - 2),
                        ButtonSize = new System.Drawing.Size(tablePadUserButtonPrevSize.Width, tablePadUserButtonPrevSize.Height)
                    });

                tablePadUserButtonPrev.Relief = ReliefStyle.None;
                tablePadUserButtonPrev.BorderWidth = 0;
                tablePadUserButtonPrev.CanFocus = false;

                //Objects:TablePadUserButtonNext
                IconButton tablePadUserButtonNext = new IconButton(
                    new ButtonSettings
                    {
                        Name = "TablePadUserButtonNext",
                        Icon = tablePadUserButtonNextImageFileName,
                        IconSize = new System.Drawing.Size(tablePadUserButtonNextSize.Width - 2,
                                                           tablePadUserButtonNextSize.Height - 2),
                        ButtonSize = new System.Drawing.Size(tablePadUserButtonNextSize.Width,
                                                             tablePadUserButtonNextSize.Height)
                    });

                tablePadUserButtonNext.Relief = ReliefStyle.None;
                tablePadUserButtonNext.BorderWidth = 0;
                tablePadUserButtonNext.CanFocus = false;

                MenuUsers = new UsersMenu(tablePadUserTableConfig.Rows,
                                          tablePadUserTableConfig.Columns,
                                          tablePadUserButtonPrev,
                                          tablePadUserButtonNext,
                                          this);

                MenuUsers.OnEntitySelected += OnUserSelected;

                if (showUsersMenu)
                {
                    fix.Put(tablePadUserButtonPrev, tablePadUserButtonPrevPosition.X, tablePadUserButtonPrevPosition.Y);
                    fix.Put(tablePadUserButtonNext, tablePadUserButtonNextPosition.X, tablePadUserButtonNextPosition.Y);
                    fix.Put(MenuUsers, tablePadUserPosition.X, tablePadUserPosition.Y);
                }

                //Label Version
                string appVersion = "";
                if (AppSettings.License.LicenseData.Reseller != null &&
                    AppSettings.License.LicenseData.Reseller.ToString().ToLower() != "logicpulse" &&
                    AppSettings.License.LicenseData.Reseller.ToString().ToLower() != "")
                {
                    //appVersion = string.Format("Brough by {1}\n{0}",appVersion, GlobalFramework.LicenceReseller);
                    appVersion = string.Format("Powered by {0}© Vers. {1}", AppSettings.License.LicenseData.Reseller, AppSettings.ProductVersion);
                }
                else
                {
                    if (AppSettings.Plugins.AppSoftwareVersionFormat != null)
                    {
                        appVersion = string.Format(AppSettings.Plugins.AppSoftwareVersionFormat, AppSettings.ProductVersion);
                    }
                }

                Label labelVersion = new Label(appVersion);
                Pango.FontDescription fontDescLabelVersion = Pango.FontDescription.FromString(labelVersionFont);
                labelVersion.ModifyFg(StateType.Normal, labelVersionFontColor);
                labelVersion.ModifyFont(fontDescLabelVersion);
                labelVersion.Justify = Justification.Center;
                labelVersion.WidthRequest = 307;
                labelVersion.HeightRequest = 50;
                labelVersion.SetAlignment(0.5F, 0.5F);

                //Put in Fix
                fix.Put(labelVersion, labelVersionPosition.X, labelVersionPosition.Y);

                //LOGO
                if (AppSettings.Plugins.LicenceManager != null)
                {
                    string fileImageBackOfficeLogo = string.Format(AppSettings.Paths.Themes + @"Default\Images\logicPOS_loggericpulse_login.png");

                    if (!string.IsNullOrEmpty(AppSettings.License.LicenseData.Reseller) && AppSettings.License.LicenseData.Reseller == "NewTech")
                    {
                        fileImageBackOfficeLogo = string.Format(AppSettings.Paths.Themes + @"Default\Images\Branding\{0}\logicPOS_loggericpulse_login.png", "NT");
                    }
                }
                else
                {
                    Image imageLogo = new Image(Utils.GetThemeFileLocation(AppSettings.Instance.FileImageBackOfficeLogo));
                    fix.Put(imageLogo, AppSettings.Instance.AppScreenSize.Width - 430, 80);
                }

                ScreenArea.Add(fix);

                PinPanel.TxtPin.GrabFocus();

                ShowAll();

            }
            else
            {
                CustomAlerts.ShowThemeRenderingErrorAlert(errorMessage, this);
            }
        }

    }
}
