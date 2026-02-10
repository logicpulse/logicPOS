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
                global::System.Drawing.Point labelVersionPosition = Utils.StringToPosition(theme.Objects.LabelVersion.Position);
                string labelVersionFont = theme.Objects.LabelVersion.Font;
                Color labelVersionFontColor = (theme.Objects.LabelVersion.FontColor as string).StringToGdkColor();
                //Objects:NumberPadPin
                global::System.Drawing.Point numberPadPinPosition = Utils.StringToPosition(theme.Objects.NumberPadPin.Position);
                global::System.Drawing.Size numberPadPinButtonSize = (theme.Objects.NumberPadPin.ButtonSize as string).ToSize();
                global::System.Drawing.Color numberPadPinFontColor = (theme.Objects.NumberPadPin.FontColor as string).StringToColor();
                uint numberPadPinRowSpacingSystemButtons = Convert.ToUInt16(theme.Objects.NumberPadPin.RowSpacingSystemButtons);
                uint numberPadPinRowSpacingLabelStatus = Convert.ToUInt16(theme.Objects.NumberPadPin.RowSpacingLabelStatus);
                //Objects:NumberPadPin:LabelStatus
                global::System.Drawing.Color numberPadPinLabelStatusFontColor = (theme.Objects.NumberPadPin.LabelStatus.FontColor as string).StringToColor();
                //Objects:NumberPadPin:Size (EventBox)
                global::System.Drawing.Size numberPadPinSize = (theme.Objects.NumberPadPin.Size as string).ToSize();


                //Objects:TablePadUserButtonPrev
                global::System.Drawing.Point tablePadUserButtonPrevPosition = Utils.StringToPosition(theme.Objects.TablePadUser.TablePadUserButtonPrev.Position);
                global::System.Drawing.Size tablePadUserButtonPrevSize = (theme.Objects.TablePadUser.TablePadUserButtonPrev.Size as string).ToSize();
                string tablePadUserButtonPrevImageFileName = theme.Objects.TablePadUser.TablePadUserButtonPrev.ImageFileName;
                //Objects:TablePadUserButtonNext
                global::System.Drawing.Point tablePadUserButtonNextPosition = Utils.StringToPosition(theme.Objects.TablePadUser.TablePadUserButtonNext.Position);
                global::System.Drawing.Size tablePadUserButtonNextSize = (theme.Objects.TablePadUser.TablePadUserButtonNext.Size as string).ToSize();
                string tablePadUserButtonNextImageFileName = theme.Objects.TablePadUser.TablePadUserButtonNext.ImageFileName;
                //Objects:TablePadUser
                global::System.Drawing.Point tablePadUserPosition = Utils.StringToPosition(theme.Objects.TablePadUser.Position);
                global::System.Drawing.Size tablePadUserButtonSize = (theme.Objects.TablePadUser.ButtonSize as string).ToSize();
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
                        Name = "buttonPosScrollers",
                        Icon = tablePadUserButtonPrevImageFileName,
                        IconSize = new global::System.Drawing.Size(tablePadUserButtonPrevSize.Width - 2, tablePadUserButtonPrevSize.Height - 2),
                        ButtonSize = new global::System.Drawing.Size(tablePadUserButtonPrevSize.Width, tablePadUserButtonPrevSize.Height)
                    });

                tablePadUserButtonPrev.Relief = ReliefStyle.None;
                tablePadUserButtonPrev.BorderWidth = 0;
                tablePadUserButtonPrev.CanFocus = false;

                //Objects:TablePadUserButtonNext
                IconButton tablePadUserButtonNext = new IconButton(
                    new ButtonSettings
                    {
                        Name = "buttonPosScrollers",
                        Icon = tablePadUserButtonNextImageFileName,
                        IconSize = new global::System.Drawing.Size(tablePadUserButtonNextSize.Width - 2,
                                                           tablePadUserButtonNextSize.Height - 2),
                        ButtonSize = new global::System.Drawing.Size(tablePadUserButtonNextSize.Width,
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


                Label labelVersion = new Label($"Powered by LogicPulse Technologies © Vers. v{SystemVersionProvider.Version}");
                Pango.FontDescription fontDescLabelVersion = Pango.FontDescription.FromString(labelVersionFont);
                labelVersion.ModifyFg(StateType.Normal, labelVersionFontColor);
                labelVersion.ModifyFont(fontDescLabelVersion);
                labelVersion.Justify = Justification.Center;
                labelVersion.WidthRequest = 307;
                labelVersion.HeightRequest = 50;
                labelVersion.SetAlignment(0.5F, 0.5F);

                //Put in Fix
                fix.Put(labelVersion, labelVersionPosition.X, labelVersionPosition.Y);

                Image imageLogo = new Image(AppSettings.Paths.GetThemeFileLocation("Images\\logicPOS_logo.png"));
                fix.Put(imageLogo, AppSettings.Instance.AppScreenSize.Width - 430, 80);


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
