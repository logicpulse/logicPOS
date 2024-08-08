using Gdk;
using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Logic.Others;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Settings;
using LogicPOS.UI;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Widgets;
using Newtonsoft.Json;
using System;
using Image = Gtk.Image;

namespace logicpos
{
    public partial class StartupWindow : PosBaseWindow
    {
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private NumberPadPin _userPinPanel;
        public TablePad UsersPanel { get; set; }
        private sys_userdetail _selectedLoginUser;

        public StartupWindow(
            string backgroundImage,
            bool needToUpdate)
            : base(backgroundImage)
        {
            InitializeUI();

            //InitPlataform
            InitPlataformParameters();

            //show changelog
            if (needToUpdate)
            {
                Utils.ShowChangeLog(this);
            }

            //Show Notifications to all users after Show UI, here we dont have a logged user Yet
            Utils.ShowNotifications(this);

            //Assign to member UserDetail reference, after InitUi, this way ChangePassword Message appears after StartupWindow
            AssignUserDetail();

            //Events
            this.KeyReleaseEvent += StartupWindow_KeyReleaseEvent;
        }

        private void InitPlataformParameters()
        {
            try
            {
                //Get ConfigurationPreferenceParameter Values to Check if Plataform is Inited
                cfg_configurationpreferenceparameter configurationPreferenceParameterCompanyCountryOid = (XPOUtility.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "COMPANY_COUNTRY_OID")) as cfg_configurationpreferenceparameter);
                cfg_configurationpreferenceparameter configurationPreferenceParameterSystemCurrencyOid = (XPOUtility.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "SYSTEM_CURRENCY_OID")) as cfg_configurationpreferenceparameter);
                cfg_configurationpreferenceparameter configurationPreferenceParameterCompanyCountryCode2 = (XPOUtility.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "COMPANY_COUNTRY_CODE2")) as cfg_configurationpreferenceparameter);
                cfg_configurationpreferenceparameter configurationPreferenceParameterCompanyFiscalNumber = (XPOUtility.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "COMPANY_FISCALNUMBER")) as cfg_configurationpreferenceparameter);

                if (
                    string.IsNullOrEmpty(configurationPreferenceParameterCompanyCountryOid.Value) ||
                    string.IsNullOrEmpty(configurationPreferenceParameterCompanyCountryCode2.Value) ||
                    string.IsNullOrEmpty(configurationPreferenceParameterCompanyFiscalNumber.Value) ||
                    string.IsNullOrEmpty(configurationPreferenceParameterSystemCurrencyOid.Value)
                )
                {
                    PosEditCompanyDetails dialog = new PosEditCompanyDetails(this, DialogFlags.DestroyWithParent | DialogFlags.Modal, false);
                    ResponseType response = (ResponseType)dialog.Run();
                    dialog.Destroy();
                }

                //Always Get Objects from Prefs to Singleton : with and without PosEditCompanyDetails
                configurationPreferenceParameterCompanyCountryOid = (XPOUtility.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "COMPANY_COUNTRY_OID")) as cfg_configurationpreferenceparameter);
                configurationPreferenceParameterSystemCurrencyOid = (XPOUtility.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "SYSTEM_CURRENCY_OID")) as cfg_configurationpreferenceparameter);
                XPOSettings.ConfigurationSystemCountry = (cfg_configurationcountry)XPOSettings.Session.GetObjectByKey(typeof(cfg_configurationcountry), new Guid(configurationPreferenceParameterCompanyCountryOid.Value));
                XPOSettings.ConfigurationSystemCurrency = (cfg_configurationcurrency)XPOSettings.Session.GetObjectByKey(typeof(cfg_configurationcurrency), new Guid(configurationPreferenceParameterSystemCurrencyOid.Value));

                _logger.Debug(string.Format("Using System Country: [{0}], Currency: [{1}]", XPOSettings.ConfigurationSystemCountry.Designation, XPOSettings.ConfigurationSystemCurrency.Designation));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private dynamic GetTheme()
        {
            var predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "StartupWindow");
            var theme = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);
            return theme;
        }

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
                string numberPadPinFont = theme.Objects.NumberPadPin.Font;
                System.Drawing.Color numberPadPinFontColor = (theme.Objects.NumberPadPin.FontColor as string).StringToColor();
                uint numberPadPinRowSpacingSystemButtons = Convert.ToUInt16(theme.Objects.NumberPadPin.RowSpacingSystemButtons);
                uint numberPadPinRowSpacingLabelStatus = Convert.ToUInt16(theme.Objects.NumberPadPin.RowSpacingLabelStatus);
                //Objects:NumberPadPin:LabelStatus
                string numberPadPinLabelStatusFont = theme.Objects.NumberPadPin.LabelStatus.Font;
                System.Drawing.Color numberPadPinLabelStatusFontColor = (theme.Objects.NumberPadPin.LabelStatus.FontColor as string).StringToColor();
                //Objects:NumberPadPin:Size (EventBox)
                bool NumberPadPinVisibleWindow = Convert.ToBoolean(theme.Objects.NumberPadPin.VisibleWindow);
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
                bool tablePadUserVisible = Convert.ToBoolean(theme.Objects.TablePadUser.Visible);

                //Init UI
                Fixed fix = new Fixed();


                if (AppSettings.Instance.appShowMinimize)
                {
                    EventBox eventBoxMinimize = GtkUtils.CreateMinimizeButton();
                    eventBoxMinimize.ButtonReleaseEvent += delegate { Iconify(); };
                    fix.Put(eventBoxMinimize, GlobalApp.ScreenSize.Width - 27 - 10, 10);
                }

                //NumberPadPin
                _userPinPanel = new NumberPadPin(this,
                                                 "numberPadPin",
                                                 System.Drawing.Color.Transparent,
                                                 numberPadPinFont,
                                                 numberPadPinLabelStatusFont,
                                                 numberPadPinFontColor,
                                                 numberPadPinLabelStatusFontColor,
                                                 Convert.ToByte(numberPadPinButtonSize.Width),
                                                 Convert.ToByte(numberPadPinButtonSize.Height),
                                                 false,
                                                 true,
                                                 NumberPadPinVisibleWindow,
                                                 numberPadPinRowSpacingLabelStatus,
                                                 numberPadPinRowSpacingSystemButtons);
              
                if (numberPadPinSize.Width > 0 || numberPadPinSize.Height > 0)
                {
                    _userPinPanel.Eventbox.WidthRequest = numberPadPinSize.Width;
                    _userPinPanel.Eventbox.HeightRequest = numberPadPinSize.Height;
                }

                //Put in Fix
                fix.Put(_userPinPanel, numberPadPinPosition.X, numberPadPinPosition.Y);
                //Over NumberPadPin
                //fix.Put(touchButtonKeyPasswordReset, numberPadPinButtonPasswordResetPosition.X, numberPadPinButtonPasswordResetPosition.Y);
                //Events
                _userPinPanel.ButtonKeyOK.Clicked += ButtonKeyOK_Clicked;
                _userPinPanel.ButtonKeyResetPassword.Clicked += ButtonKeyResetPassword_Clicked;
                _userPinPanel.ButtonKeyFrontOffice.Clicked += ButtonKeyFrontOffice_Clicked;
                _userPinPanel.ButtonKeyBackOffice.Clicked += ButtonKeyBackOffice_Clicked;
                _userPinPanel.ButtonKeyQuit.Clicked += ButtonKeyQuit_Clicked;

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
                //Objects:TablePadUser
                string sqlTablePadUser = @"
                        SELECT 
                            Oid as id, Name as name, Name as label, ButtonImage as image
                        FROM 
                            sys_userdetail
                        WHERE 
                            (Disabled IS NULL or Disabled <> 1)
                    ";

                UsersPanel = new TablePad(
                    sqlTablePadUser,
                    "ORDER BY Ord",
                    "",
                    Guid.Empty,
                    true,
                    tablePadUserTableConfig.Rows,
                    tablePadUserTableConfig.Columns,
                    "buttonUserId",
                    System.Drawing.Color.Transparent,
                    tablePadUserButtonSize.Width,
                    tablePadUserButtonSize.Height,
                    tablePadUserButtonPrev,
                    tablePadUserButtonNext
                );
                UsersPanel.SourceWindow = this;
                UsersPanel.Clicked += TablePadUser_Clicked;

                //Put in Fix
                if (tablePadUserVisible)
                {
                    fix.Put(tablePadUserButtonPrev, tablePadUserButtonPrevPosition.X, tablePadUserButtonPrevPosition.Y);
                    fix.Put(tablePadUserButtonNext, tablePadUserButtonNextPosition.X, tablePadUserButtonNextPosition.Y);
                    fix.Put(UsersPanel, tablePadUserPosition.X, tablePadUserPosition.Y);
                }

                //Label Version
                string appVersion = "";
                if (LicenseSettings.LicenseReseller != null &&
                    LicenseSettings.LicenseReseller.ToString().ToLower() != "Logicpulse" &&
                    LicenseSettings.LicenseReseller.ToString().ToLower() != "")
                {
                    //appVersion = string.Format("Brough by {1}\n{0}",appVersion, GlobalFramework.LicenceReseller);
                    appVersion = string.Format("Powered by {0}© Vers. {1}", LicenseSettings.LicenseReseller, GeneralSettings.ProductVersion);
                }
                else
                {
                    if (PluginSettings.AppSoftwareVersionFormat != null)
                    {
                        appVersion = string.Format(PluginSettings.AppSoftwareVersionFormat, GeneralSettings.ProductVersion);
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



                if (Program.DebugMode)
                {
                    Button buttonDeveloper = new Button("Developer");
                    fix.Put(buttonDeveloper, 10, 100);
                    buttonDeveloper.Clicked += ButtonDeveloper_Clicked;
                }

                //LOGO
                if (PluginSettings.LicenceManager != null)
                {
                    string fileImageBackOfficeLogo = string.Format(PathsSettings.Paths["themes"] + @"Default\Images\logicPOS_loggericpulse_loggerin.png");

                    if (!string.IsNullOrEmpty(LicenseSettings.LicenseReseller) && LicenseSettings.LicenseReseller == "NewTech")
                    {
                        fileImageBackOfficeLogo = string.Format(PathsSettings.Paths["themes"] + @"Default\Images\Branding\{0}\logicPOS_loggericpulse_loggerin.png", "NT");
                    }
                }
                else
                {
                    Image imageLogo = new Image(Utils.GetThemeFileLocation(AppSettings.Instance.fileImageBackOfficeLogo));
                    fix.Put(imageLogo, GlobalApp.ScreenSize.Width - 430, 80);
                }
                //string fileImageBackOfficeLogo = Utils.GetThemeFileLocation(LogicPOS.Settings.AppSettings.Instance.fileImageBackOfficeLogo"]);
                ScreenArea.Add(fix);

                //Force EntryPin to be the Entry with Focus
                _userPinPanel.EntryPin.GrabFocus();

                ShowAll();

                //Events
                _userPinPanel.ExposeEvent += delegate
                {
                    _userPinPanel.ButtonKeyFrontOffice.Hide();
                    _userPinPanel.ButtonKeyBackOffice.Hide();
                };

            }
            else
            {
                Utils.ShowMessageTouchErrorRenderTheme(this, errorMessage);
            }
        }

        private void ButtonDeveloper_Clicked(object sender, EventArgs e)
        {
            PosDeveloperTestDialog dialog = new PosDeveloperTestDialog(this);

            ResponseType response = (ResponseType)dialog.Run();

            dialog.Destroy();
        }
    }
}
