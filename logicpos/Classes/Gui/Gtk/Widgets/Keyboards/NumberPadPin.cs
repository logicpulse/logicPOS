using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Enums.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using logicpos.Extensions;
using logicpos.shared.App;
using System;
using System.Drawing;
using LogicPOS.Settings.Extensions;
using LogicPOS.Utility;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using logicpos.shared;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    public class NumberPadPin : Box
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Protected
        protected Table _table;
        //Private Members
        private readonly Window _sourceWindow;
        private int _tempCursorPosition = 0;
        private bool _entryPinShowStatus = false;
        private readonly Label _labelStatus;
        //Used to store New Password in Memory, to Compare with Confirmation
        private string _passwordNew;
        private bool _notLoginAuth;
        //Public Properties
        private NumberPadPinMode _mode;
        public NumberPadPinMode Mode
        {
            get { return _mode; }
            set { 
                _mode = value; 
                UpdateStatusLabels(); 
            }
        }

        public EventBox Eventbox { get; set; }

        public EntryValidation EntryPin { get; set; }
        public TouchButtonText ButtonKeyOK { get; set; }

        public TouchButtonIcon ButtonKeyResetPassword { get; set; }
        public TouchButtonText ButtonKeyFrontOffice { get; set; }

        public TouchButtonText ButtonKeyBackOffice { get; set; }
        public TouchButtonText ButtonKeyQuit { get; set; }

        public NumberPadPin(Window pSourceWindow, string pName, Color pButtonColor, string pFont, string pFontLabelStatus, Color pFontColor, Color pFontColorLabelStatus, byte pButtonWidth, byte pButtonHeight, bool pNotLoginAuth, bool pShowSystemButtons = false, bool pVisibleWindow = false, uint pRowSpacingLabelStatus = 20, uint pRowSpacingSystemButtons = 40, byte pPadding = 3)
        {
            _sourceWindow = pSourceWindow;
            this.Name = pName;
            _notLoginAuth = pNotLoginAuth;
            //Show or Hide System Buttons (Startup Visible, Pos Change User Invisible)
            uint tableRows = (pShowSystemButtons) ? (uint)5 : (uint)3;

            Eventbox = new EventBox() { VisibleWindow = pVisibleWindow };

            _table = new Table(tableRows, 3, true);
            _table.Homogeneous = false;

            //Pin Entry
            EntryPin = new EntryValidation(pSourceWindow, KeyboardMode.None, RegexUtils.RegexLoginPin, true) { InvisibleChar = '*', Visibility = false };
            EntryPin.ModifyFont(Pango.FontDescription.FromString(pFont));
            EntryPin.Alignment = 0.5F;

            //ResetPassword
            string numberPadPinButtonPasswordResetImageFileName = GeneralSettings.Paths["images"] + @"Icons\Other\pinpad_password_reset.png";
            ButtonKeyResetPassword = new TouchButtonIcon("touchButtonKeyPasswordReset", Color.Transparent, numberPadPinButtonPasswordResetImageFileName, new Size(20, 20), 25, 25) { Sensitive = false };

            //Start Validated
            EntryPin.Validate();
            //Event
            EntryPin.Changed += _entryPin_Changed;
            EntryPin.KeyReleaseEvent += _entryPin_KeyReleaseEvent;

            //Label Status
            _labelStatus = new Label(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "pos_pinpad_message_type_password"));
            _labelStatus.ModifyFont(Pango.FontDescription.FromString(pFontLabelStatus));
            _labelStatus.ModifyFg(StateType.Normal, pFontColorLabelStatus.ToGdkColor());
            _labelStatus.SetAlignment(0.5F, 0.5f);

            //Initialize Buttons
            TouchButtonText buttonKey1 = new TouchButtonText("touchButtonKey1", pButtonColor, "1", pFont, pFontColor, pButtonWidth, pButtonHeight);
            TouchButtonText buttonKey2 = new TouchButtonText("touchButtonKey2", pButtonColor, "2", pFont, pFontColor, pButtonWidth, pButtonHeight);
            TouchButtonText buttonKey3 = new TouchButtonText("touchButtonKey3", pButtonColor, "3", pFont, pFontColor, pButtonWidth, pButtonHeight);
            TouchButtonText buttonKey4 = new TouchButtonText("touchButtonKey4", pButtonColor, "4", pFont, pFontColor, pButtonWidth, pButtonHeight);
            TouchButtonText buttonKey5 = new TouchButtonText("touchButtonKey5", pButtonColor, "5", pFont, pFontColor, pButtonWidth, pButtonHeight);
            TouchButtonText buttonKey6 = new TouchButtonText("touchButtonKey6", pButtonColor, "6", pFont, pFontColor, pButtonWidth, pButtonHeight);
            TouchButtonText buttonKey7 = new TouchButtonText("touchButtonKey7", pButtonColor, "7", pFont, pFontColor, pButtonWidth, pButtonHeight);
            TouchButtonText buttonKey8 = new TouchButtonText("touchButtonKey8", pButtonColor, "8", pFont, pFontColor, pButtonWidth, pButtonHeight);
            TouchButtonText buttonKey9 = new TouchButtonText("touchButtonKey9", pButtonColor, "9", pFont, pFontColor, pButtonWidth, pButtonHeight);
            TouchButtonText buttonKey0 = new TouchButtonText("touchButtonKey0", pButtonColor, "0", pFont, pFontColor, pButtonWidth, pButtonHeight);
            TouchButtonText buttonKeyCE = new TouchButtonText("touchButtonKeyCE_Red", pButtonColor, "CE", pFont, pFontColor, pButtonWidth, pButtonHeight);

            //Shared Button,can be OK or Quit based on ShowSystemButtons
            TouchButtonText _buttonQuitOrOk;

            //Outside Reference Buttons (Public)
            ButtonKeyOK = new TouchButtonText("touchButtonKeyOK_Green", pButtonColor, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "widget_pospinpad_ok"), pFont, pFontColor, pButtonWidth, pButtonHeight) { Sensitive = false };
            //_buttonKeyResetPassword = new TouchButtonText("touchButtonKeyReset_Red", pButtonColor, CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "widget_pospinpad_change_password, pFont, pFontColor, pButtonWidth, pButtonHeight) { Sensitive = false };

            if (pShowSystemButtons)
            {
                ButtonKeyFrontOffice = new TouchButtonText("touchButtonKeyFrontOffice_DarkGrey", pButtonColor, "FO", pFont, pFontColor, pButtonWidth, pButtonHeight) { Sensitive = false, Visible = false };
                ButtonKeyQuit = new TouchButtonText("touchButtonKeyQuit_DarkGrey", pButtonColor, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_quit_title"), pFont, pFontColor, pButtonWidth, pButtonHeight);
                ButtonKeyBackOffice = new TouchButtonText("touchButtonKeyBackOffice_DarkGrey", pButtonColor, "BO", pFont, pFontColor, pButtonWidth, pButtonHeight) { Sensitive = false, Visible = false };
                _buttonQuitOrOk = ButtonKeyQuit;
            }
            else
            {
                _buttonQuitOrOk = ButtonKeyOK;
            }

            //_buttonKeyOK.Style.SetBgPixmap(StateType.Active, Utils.FileToPixmap("Assets/Themes/Default/Backgrounds/Windows/test.png"));
            //_buttonKeyOK.Style = Utils.GetThemeStyleBackground(@"Backgrounds/Windows/test.png");

            //Events
            buttonKey1.Clicked += buttonKey_Clicked;
            buttonKey2.Clicked += buttonKey_Clicked;
            buttonKey3.Clicked += buttonKey_Clicked;
            buttonKey4.Clicked += buttonKey_Clicked;
            buttonKey5.Clicked += buttonKey_Clicked;
            buttonKey6.Clicked += buttonKey_Clicked;
            buttonKey7.Clicked += buttonKey_Clicked;
            buttonKey8.Clicked += buttonKey_Clicked;
            buttonKey9.Clicked += buttonKey_Clicked;
            buttonKey0.Clicked += buttonKey_Clicked;
            buttonKeyCE.Clicked += buttonKeyCE_Clicked;

            //Prepare Table

            //row0
            int entryPinTablePos = (pShowSystemButtons) ? 2 : 3;
            //Without ResetPassword Button
            if (!pShowSystemButtons)
            {
                _table.Attach(EntryPin, 0, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            }
            //With ResetPassword Button
            else
            {
                _table.Attach(EntryPin, 0, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
                _table.Attach(ButtonKeyResetPassword, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            }
            //row1
            _table.Attach(buttonKey7, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            _table.Attach(buttonKey8, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            _table.Attach(buttonKey9, 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            //row2
            _table.Attach(buttonKey4, 0, 1, 2, 3, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            _table.Attach(buttonKey5, 1, 2, 2, 3, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            _table.Attach(buttonKey6, 2, 3, 2, 3, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            //row3
            _table.Attach(buttonKey1, 0, 1, 3, 4, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            _table.Attach(buttonKey2, 1, 2, 3, 4, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            _table.Attach(buttonKey3, 2, 3, 3, 4, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            //row4
            _table.Attach(buttonKeyCE, 0, 1, 4, 5, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            _table.Attach(buttonKey0, 1, 2, 4, 5, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            _table.Attach(_buttonQuitOrOk, 2, 3, 4, 5, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            //Row5
            _table.Attach(_labelStatus, 0, 3, 5, 6, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            _table.SetRowSpacing(4, pRowSpacingLabelStatus);
            //row6
            if (pShowSystemButtons)
            {
                //_table.Attach(_buttonKeyFrontOffice, 0, 1, 5, 6, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
                //_table.Attach(_buttonKeyOK, 0, 3, 7, 8, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
                //_table.Attach(_buttonKeyBackOffice, 2, 3, 5, 6, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
                _table.Attach(ButtonKeyOK, 0, 3, 7, 8, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
                //space between Status Message and POS Keys
                _table.SetRowSpacing(5, pRowSpacingSystemButtons);
            }

            Eventbox.Add(_table);
            this.Add(Eventbox);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        private void _entryPin_Changed(object sender, EventArgs e)
        {
            EntryValidation entry = (EntryValidation)sender;
            ClearEntryPinStatusMessage();
            ButtonKeyOK.Sensitive = entry.Validated;
            ButtonKeyResetPassword.Sensitive = (entry.Validated && _mode == NumberPadPinMode.Password);
        }

        //Shared Button Key for Numbers
        private void buttonKey_Clicked(object sender, EventArgs e)
        {
            TouchButtonText button = (TouchButtonText)sender;
            ClearEntryPinStatusMessage();
            EntryPin.InsertText(button.LabelText, ref _tempCursorPosition);
            //_entryPin.Position = _tempCursorPosition;
            EntryPin.GrabFocus();
            EntryPin.Position = EntryPin.Text.Length;
        }

        private void buttonKeyCE_Clicked(object sender, EventArgs e)
        {
            ClearEntryPinStatusMessage();
            //_entryPin.DeleteText(0, _entryPin.Text.Length);
            EntryPin.DeleteText(EntryPin.Text.Length - 1, EntryPin.Text.Length);
            //_tempCursorPosition = _entryPin.Position;
            EntryPin.GrabFocus();
            EntryPin.Position = EntryPin.Text.Length;
        }

        private void _entryPin_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key.ToString().Equals("Return"))
            {
                if (EntryPin.Validated)
                {
                    ButtonKeyOK.Click();
                }
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Logic

        public bool ProcessPassword(Window pSourceWindow, sys_userdetail pUserDetail, bool pNotLoginAuth = false)
        {
            bool result = false;
            _notLoginAuth = pNotLoginAuth;
            try
            {
                switch (_mode)
                {
                    case NumberPadPinMode.Password:
                        //Valid User
                        if (ValidatePassword(pUserDetail))
                        {
                            //Start Application if login Authentication
                            if (!_notLoginAuth)
                            {
                                ProcessLogin(pUserDetail);
                            }
                            //Finish Job usefull to PosPinDialog send Respond(ResponseType.Ok) when Done
                            result = true;
                        }
                        break;
                    case NumberPadPinMode.PasswordOld:
                        //Valid User
                        if (ValidatePassword(pUserDetail))
                        {
                            _mode = NumberPadPinMode.PasswordNew;
                            UpdateStatusLabels();
                        }
                        break;
                    case NumberPadPinMode.PasswordNew:
                        //Check If New Password is Equal to Old Password using ValidatePassword
                        if (CryptographyUtils.ValidateSaltedString(pUserDetail.AccessPin, EntryPin.Text))
                        {
                            //Show Error Message
                            ResponseType responseType = logicpos.Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "window_title_dialog_change_password"), CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "pos_pinpad_message_password_equal_error"));
                            ClearEntryPinStatusMessage(true);
                        }
                        else
                        {
                            _passwordNew = EntryPin.Text;
                            _mode = NumberPadPinMode.PasswordNewConfirm;
                            UpdateStatusLabels();
                        }
                        break;
                    case NumberPadPinMode.PasswordNewConfirm:
                        if (_passwordNew == EntryPin.Text)
                        {
                            //Commit Changes
                            pUserDetail.AccessPin =  CryptographyUtils.GenerateSaltedString(_passwordNew);
                            pUserDetail.PasswordReset = false;
                            pUserDetail.PasswordResetDate = XPOHelper.CurrentDateTimeAtomic();
                            pUserDetail.Save();
                            SharedUtils.Audit("USER_CHANGE_PASSWORD", string.Format(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "audit_message_user_change_password"), pUserDetail.Name));
                            ResponseType responseType = logicpos.Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "window_title_dialog_change_password"), CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "pos_pinpad_message_password_changed"));
                            //Start Application
                            ProcessLogin(pUserDetail);
                            //Finish Job usefull to PosPinDialog send Respond(ResponseType.Ok) when Done
                            result = true;
                        }
                        else { 
                            //Show Error Message
                            ResponseType responseType = logicpos.Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "window_title_dialog_change_password"), CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "pos_pinpad_message_password_confirmation_error"));
                            ClearEntryPinStatusMessage(true);
                            //Return to 
                            _mode = NumberPadPinMode.PasswordNew;
                            UpdateStatusLabels();
                            //Reset passwordNew
                            _passwordNew = string.Empty;
                        }
                        break;
                    case NumberPadPinMode.PasswordReset:
                        //Valid User
                        if (ValidatePassword(pUserDetail))
                        {
                            _mode = NumberPadPinMode.PasswordNew;
                            UpdateStatusLabels();
                        }
                        //Return to default request password mode
                        else
                        {
                            _mode = NumberPadPinMode.Password;
                        }
                        break;
                    default:
                        break;
                }

                //Always focus Entry
                EntryPin.GrabFocus();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        public bool ValidatePassword(sys_userdetail pUserDetail)
        {
            bool result = false;
            string password = EntryPin.Text;

            string sql = string.Format(@"
                SELECT 
                    AccessPin 
                FROM 
                    sys_userdetail 
                WHERE 
                    (Disabled <> 1 OR Disabled IS NULL)
                    AND Oid = '{0}'
                ;", pUserDetail.Oid
            );

            try
            {
                var resultObject = XPOSettings.Session.ExecuteScalar(sql);

                if (resultObject != null && resultObject.GetType() == typeof(string) && CryptographyUtils.ValidateSaltedString(resultObject.ToString(), password))
                {
                    EntryPin.ModifyText(StateType.Normal, Color.Black.ToGdkColor());
                    EntryPin.Visibility = false;
                    _entryPinShowStatus = false;
                    result = true;
                }
                else
                {
                    SharedUtils.Audit("USER_loggerIN_ERROR", string.Format(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "audit_message_user_loggerin_error"), pUserDetail.Name));                    
                    EntryPin.ModifyText(StateType.Normal, Color.Red.ToGdkColor());
                    EntryPin.Text = CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "status_message_pin_error");
                    EntryPin.Visibility = true;
                    _entryPinShowStatus = true;
                    result = false;
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return result;
            }
        }

        private void ClearEntryPinStatusMessage(bool pForceClear = false)
        {
            //Clean Error Message
            if (_entryPinShowStatus || pForceClear)
            {
                EntryPin.ModifyText(StateType.Normal, Color.Black.ToGdkColor());
                EntryPin.Text = string.Empty;
                EntryPin.Visibility = false;
                _entryPinShowStatus = false;
            }
        }

        private void UpdateStatusLabels()
        {
            switch (_mode)
            {
                case NumberPadPinMode.Password:
                    _labelStatus.Text = CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "pos_pinpad_message_type_password");
                    //_buttonKeyOK.LabelText = CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "widget_pospinpad_loggerin;
                    ButtonKeyOK.LabelText = (!_notLoginAuth) ? CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "widget_pospinpad_loggerin") : CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "widget_pospinpad_ok");
                    break;
                case NumberPadPinMode.PasswordOld:
                    //Show message to user, to change old password
                    //ResponseType responseType = Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_dialog_change_password, CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "pos_pinpad_message_password_request_change_password);
                    _labelStatus.Text = CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "pos_pinpad_message_type_old_password");
                    ButtonKeyOK.LabelText = CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "widget_pospinpad_ok");
                    break;
                case NumberPadPinMode.PasswordNew:
                    _labelStatus.Text = CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "pos_pinpad_message_type_new_password");
                    ButtonKeyOK.LabelText = CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "widget_pospinpad_ok");
                    break;
                case NumberPadPinMode.PasswordNewConfirm:
                    _labelStatus.Text = CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "pos_pinpad_message_type_new_password_confirm");
                    ButtonKeyOK.LabelText = CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "widget_pospinpad_ok");
                    break;
                default:
                    break;
            }
            //Force Clear Status
            ClearEntryPinStatusMessage(true);
        }

        //Start Application or Change User
        private void ProcessLogin(sys_userdetail pUserDetail)
        {
            XPOSettings.LoggedUser = pUserDetail;
            SharedFramework.LoggedUserPermissions = SharedUtils.GetUserPermissions();
            SharedUtils.Audit("USER_loggerIN", string.Format(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "audit_message_user_loggerin"), pUserDetail.Name));

            //SessionApp Add LoggedUser
            if (!POSSession.CurrentSession.LoggedUsers.ContainsKey(XPOSettings.LoggedUser.Oid))
            {
                POSSession.CurrentSession.LoggedUsers.Add(pUserDetail.Oid, XPOHelper.CurrentDateTimeAtomic());
            }
            else
            {
                POSSession.CurrentSession.LoggedUsers[XPOSettings.LoggedUser.Oid] = XPOHelper.CurrentDateTimeAtomic();
            }
            POSSession.CurrentSession.Save();

            //Returns to default mode
            _mode = NumberPadPinMode.Password;
            UpdateStatusLabels();

            //Process Notifications After Login/Create First Time PosWindow
            //Disabled SystemNotification and ShowNotifications. Moved to Startup Window
            //FrameworkUtils.SystemNotification();
            //Utils.ShowNotifications(_sourceWindow, pUserDetail.Oid);

            //TK016235 BackOffice - Mode
            if (SharedFramework.AppUseBackOfficeMode)
                logicpos.Utils.ShowBackOffice(_sourceWindow);
            else
                logicpos.Utils.ShowFrontOffice(_sourceWindow);
        }
    }
}
