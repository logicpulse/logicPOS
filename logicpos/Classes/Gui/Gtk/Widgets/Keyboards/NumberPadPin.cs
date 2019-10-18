using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Enums.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.resources.Resources.Localization;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    public class NumberPadPin : Box
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Protected
        protected Table _table;
        //Private Members
        private Window _sourceWindow;
        private int _tempCursorPosition = 0;
        private Boolean _entryPinShowStatus = false;
        private Label _labelStatus;
        //Used to store New Password in Memory, to Compare with Confirmation
        private string _passwordNew;
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

        private EventBox _eventbox;
        public EventBox Eventbox
        {
            get { return _eventbox; }
            set { _eventbox = value; }
        }

        private EntryValidation _entryPin;
        public EntryValidation EntryPin
        {
            get { return _entryPin; }
            set { _entryPin = value; }
        }
        private TouchButtonText _buttonKeyOK;
        public TouchButtonText ButtonKeyOK
        {
            get { return _buttonKeyOK; }
            set { _buttonKeyOK = value; }
        }
        private TouchButtonIcon _buttonKeyResetPassword;
        public TouchButtonIcon ButtonKeyResetPassword
        {
            get { return _buttonKeyResetPassword; }
            set { _buttonKeyResetPassword = value; }
        }
        private TouchButtonText _buttonKeyFrontOffice;
        public TouchButtonText ButtonKeyFrontOffice
        {
            get { return _buttonKeyFrontOffice; }
            set { _buttonKeyFrontOffice = value; }
        }
        private TouchButtonText _buttonKeyBackOffice;
        public TouchButtonText ButtonKeyBackOffice
        {
            get { return _buttonKeyBackOffice; }
            set { _buttonKeyBackOffice = value; }
        }
        private TouchButtonText _buttonKeyQuit;
        public TouchButtonText ButtonKeyQuit
        {
            get { return _buttonKeyQuit; }
            set { _buttonKeyQuit = value; }
        }

        public NumberPadPin(Window pSourceWindow, string pName, Color pButtonColor, string pFont, string pFontLabelStatus, Color pFontColor, Color pFontColorLabelStatus, byte pButtonWidth, byte pButtonHeight, bool pShowSystemButtons = false, bool pVisibleWindow = false, uint pRowSpacingLabelStatus = 20, uint pRowSpacingSystemButtons = 40, byte pPadding = 3)
        {
            _sourceWindow = pSourceWindow;
            this.Name = pName;

            //Show or Hide System Buttons (Startup Visible, Pos Change User Invisible)
            uint tableRows = (pShowSystemButtons) ? (uint)5 : (uint)3;

            _eventbox = new EventBox() { VisibleWindow = pVisibleWindow };

            _table = new Table(tableRows, 3, true);
            _table.Homogeneous = false;

            //Pin Entry
            _entryPin = new EntryValidation(pSourceWindow, KeyboardMode.None, SettingsApp.RegexLoginPin, true) { InvisibleChar = '*', Visibility = false };
            _entryPin.ModifyFont(Pango.FontDescription.FromString(pFont));
            _entryPin.Alignment = 0.5F;

            //ResetPassword
            string numberPadPinButtonPasswordResetImageFileName = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Other\pinpad_password_reset.png");
            _buttonKeyResetPassword = new TouchButtonIcon("touchButtonKeyPasswordReset", System.Drawing.Color.Transparent, numberPadPinButtonPasswordResetImageFileName, new Size(20, 20), 25, 25) { Sensitive = false };

            //Start Validated
            _entryPin.Validate();
            //Event
            _entryPin.Changed += _entryPin_Changed;
            _entryPin.KeyReleaseEvent += _entryPin_KeyReleaseEvent;

            //Label Status
            _labelStatus = new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_pinpad_message_type_password"));
            _labelStatus.ModifyFont(Pango.FontDescription.FromString(pFontLabelStatus));
            _labelStatus.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(pFontColorLabelStatus));
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
            _buttonKeyOK = new TouchButtonText("touchButtonKeyOK_Green", pButtonColor, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_pospinpad_ok"), pFont, pFontColor, pButtonWidth, pButtonHeight) { Sensitive = false };
            //_buttonKeyResetPassword = new TouchButtonText("touchButtonKeyReset_Red", pButtonColor, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_pospinpad_change_password, pFont, pFontColor, pButtonWidth, pButtonHeight) { Sensitive = false };

            if (pShowSystemButtons)
            {
                _buttonKeyFrontOffice = new TouchButtonText("touchButtonKeyFrontOffice_DarkGrey", pButtonColor, "FO", pFont, pFontColor, pButtonWidth, pButtonHeight) { Sensitive = false, Visible = false };
                _buttonKeyQuit = new TouchButtonText("touchButtonKeyQuit_DarkGrey", pButtonColor, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_quit_title"), pFont, pFontColor, pButtonWidth, pButtonHeight);
                _buttonKeyBackOffice = new TouchButtonText("touchButtonKeyBackOffice_DarkGrey", pButtonColor, "BO", pFont, pFontColor, pButtonWidth, pButtonHeight) { Sensitive = false, Visible = false };
                _buttonQuitOrOk = _buttonKeyQuit;
            }
            else
            {
                _buttonQuitOrOk = _buttonKeyOK;
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
                _table.Attach(_entryPin, 0, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
            }
            //With ResetPassword Button
            else
            {
                _table.Attach(_entryPin, 0, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
                _table.Attach(_buttonKeyResetPassword, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
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
                _table.Attach(_buttonKeyOK, 0, 3, 7, 8, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
                //space between Status Message and POS Keys
                _table.SetRowSpacing(5, pRowSpacingSystemButtons);
            }

            _eventbox.Add(_table);
            this.Add(_eventbox);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        void _entryPin_Changed(object sender, EventArgs e)
        {
            EntryValidation entry = (EntryValidation)sender;
            ClearEntryPinStatusMessage();
            _buttonKeyOK.Sensitive = entry.Validated;
            _buttonKeyResetPassword.Sensitive = (entry.Validated && _mode == NumberPadPinMode.Password);
        }

        //Shared Button Key for Numbers
        void buttonKey_Clicked(object sender, EventArgs e)
        {
            TouchButtonText button = (TouchButtonText)sender;
            ClearEntryPinStatusMessage();
            _entryPin.InsertText(button.LabelText, ref _tempCursorPosition);
            //_entryPin.Position = _tempCursorPosition;
            _entryPin.GrabFocus();
            _entryPin.Position = _entryPin.Text.Length;
        }

        void buttonKeyCE_Clicked(object sender, EventArgs e)
        {
            ClearEntryPinStatusMessage();
            //_entryPin.DeleteText(0, _entryPin.Text.Length);
            _entryPin.DeleteText(_entryPin.Text.Length - 1, _entryPin.Text.Length);
            //_tempCursorPosition = _entryPin.Position;
            _entryPin.GrabFocus();
            _entryPin.Position = _entryPin.Text.Length;
        }

        void _entryPin_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key.ToString().Equals("Return"))
            {
                if (_entryPin.Validated)
                {
                    _buttonKeyOK.Click();
                }
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Logic

        public bool ProcessPassword(Window pSourceWindow, sys_userdetail pUserDetail)
        {
            bool result = false;

            try
            {
                switch (_mode)
                {
                    case NumberPadPinMode.Password:
                        //Valid User
                        if (ValidatePassword(pUserDetail))
                        {
                            //Start Application
                            ProcessLogin(pUserDetail);
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
                        if (CryptographyUtils.SaltedString.ValidateSaltedString(pUserDetail.AccessPin, _entryPin.Text))
                        {
                            //Show Error Message
                            ResponseType responseType = Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_change_password"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_pinpad_message_password_equal_error"));
                            ClearEntryPinStatusMessage(true);
                        }
                        else
                        {
                            _passwordNew = _entryPin.Text;
                            _mode = NumberPadPinMode.PasswordNewConfirm;
                            UpdateStatusLabels();
                        }
                        break;
                    case NumberPadPinMode.PasswordNewConfirm:
                        if (_passwordNew == _entryPin.Text)
                        {
                            //Commit Changes
                            pUserDetail.AccessPin =  CryptographyUtils.SaltedString.GenerateSaltedString(_passwordNew);
                            pUserDetail.PasswordReset = false;
                            pUserDetail.PasswordResetDate = FrameworkUtils.CurrentDateTimeAtomic();
                            pUserDetail.Save();
                            FrameworkUtils.Audit("USER_CHANGE_PASSWORD", string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "audit_message_user_change_password"), pUserDetail.Name));
                            ResponseType responseType = Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_change_password"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_pinpad_message_password_changed"));
                            //Start Application
                            ProcessLogin(pUserDetail);
                            //Finish Job usefull to PosPinDialog send Respond(ResponseType.Ok) when Done
                            result = true;
                        }
                        else { 
                            //Show Error Message
                            ResponseType responseType = Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_change_password"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_pinpad_message_password_confirmation_error"));
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
                _entryPin.GrabFocus();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        public bool ValidatePassword(sys_userdetail pUserDetail)
        {
            bool result = false;
            string password = _entryPin.Text;

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
                var resultObject = GlobalFramework.SessionXpo.ExecuteScalar(sql);

                if (resultObject != null && resultObject.GetType() == typeof(String) && CryptographyUtils.SaltedString.ValidateSaltedString(resultObject.ToString(), password))
                {
                    _entryPin.ModifyText(StateType.Normal, Utils.ColorToGdkColor(Color.Black));
                    _entryPin.Visibility = false;
                    _entryPinShowStatus = false;
                    result = true;
                }
                else
                {
                    FrameworkUtils.Audit("USER_LOGIN_ERROR", string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "audit_message_user_login_error"), pUserDetail.Name));                    
                    _entryPin.ModifyText(StateType.Normal, Utils.ColorToGdkColor(Color.Red));
                    _entryPin.Text = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "status_message_pin_error");
                    _entryPin.Visibility = true;
                    _entryPinShowStatus = true;
                    result = false;
                }

                return result;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return result;
            }
        }

        private void ClearEntryPinStatusMessage(bool pForceClear = false)
        {
            //Clean Error Message
            if (_entryPinShowStatus || pForceClear)
            {
                _entryPin.ModifyText(StateType.Normal, Utils.ColorToGdkColor(Color.Black));
                _entryPin.Text = string.Empty;
                _entryPin.Visibility = false;
                _entryPinShowStatus = false;
            }
        }

        private void UpdateStatusLabels()
        {
            switch (_mode)
            {
                case NumberPadPinMode.Password:
                    _labelStatus.Text = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_pinpad_message_type_password");
                    //_buttonKeyOK.LabelText = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_pospinpad_login;
                    _buttonKeyOK.LabelText = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_pospinpad_login");
                    break;
                case NumberPadPinMode.PasswordOld:
                    //Show message to user, to change old password
                    //ResponseType responseType = Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_change_password, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_pinpad_message_password_request_change_password);
                    _labelStatus.Text = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_pinpad_message_type_old_password");
                    _buttonKeyOK.LabelText = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_pospinpad_ok");
                    break;
                case NumberPadPinMode.PasswordNew:
                    _labelStatus.Text = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_pinpad_message_type_new_password");
                    _buttonKeyOK.LabelText = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_pospinpad_ok");
                    break;
                case NumberPadPinMode.PasswordNewConfirm:
                    _labelStatus.Text = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_pinpad_message_type_new_password_confirm");
                    _buttonKeyOK.LabelText = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_pospinpad_ok");
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
            GlobalFramework.LoggedUser = pUserDetail;
            GlobalFramework.LoggedUserPermissions = FrameworkUtils.GetUserPermissions();
            FrameworkUtils.Audit("USER_LOGIN", string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "audit_message_user_login"), pUserDetail.Name));

            //SessionApp Add LoggedUser
            if (!GlobalFramework.SessionApp.LoggedUsers.ContainsKey(GlobalFramework.LoggedUser.Oid))
            {
                GlobalFramework.SessionApp.LoggedUsers.Add(pUserDetail.Oid, FrameworkUtils.CurrentDateTimeAtomic());
            }
            else
            {
                GlobalFramework.SessionApp.LoggedUsers[GlobalFramework.LoggedUser.Oid] = FrameworkUtils.CurrentDateTimeAtomic();
            }
            GlobalFramework.SessionApp.Write();

            //Returns to default mode
            _mode = NumberPadPinMode.Password;
            UpdateStatusLabels();

            //Process Notifications After Login/Create First Time PosWindow
            //Disabled SystemNotification and ShowNotifications. Moved to Startup Window
            //FrameworkUtils.SystemNotification();
            //Utils.ShowNotifications(_sourceWindow, pUserDetail.Oid);

            //TK016235 BackOffice - Mode
            if (GlobalFramework.AppUseBackOfficeMode)
                Utils.ShowBackOffice(_sourceWindow);
            else
                Utils.ShowFrontOffice(_sourceWindow);
        }
    }
}
