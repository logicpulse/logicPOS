using Gtk;
using logicpos;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Enums.Widgets;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Users.ResetPassword;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components
{
    public partial class UserPinPanel : Box
    {
        private readonly ISender _mediator = DependencyInjection.Mediator;
        private Window SourceWindow { get; }
        private int _tempCursorPosition = 0;
        private bool _entryPinShowStatus = false;
        private string _oldPassword;
        private string _newPassword;
        private bool _notLoginAuth;
        public string JwtToken { get; private set; }
        private NumberPadPinMode _mode;

        public NumberPadPinMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                UpdateLables();
            }
        }


        public UserPinPanel(Window parentWindow,
                            Color labelStatusFontColor,
                            Size buttonSize,
                            bool notLoginAuth,
                            bool showSystemButtons = false,
                            uint labelStatusRowSpacing = 20,
                            uint systemButtonsRowSpacing = 40,
                            byte padding = 3)
        {
            SourceWindow = parentWindow;
            Name = "numberPadPin";
            _notLoginAuth = notLoginAuth;
            uint tableRows = showSystemButtons ? 5 : (uint)3;
            _btnSize = buttonSize;

            _table = new Gtk.Table(tableRows, 3, true);
            _table.Homogeneous = false;

            TxtPin = new ValidatableTextBox(parentWindow,
                                              KeyboardMode.None,
                                              RegularExpressions.LoginPassword,
                                              true)

            {
                InvisibleChar = '*',
                Visibility = false
            };

            TxtPin.ModifyFont(Pango.FontDescription.FromString(Font));
            TxtPin.Alignment = 0.5F;

            string resetPasswordImage = AppSettings.Paths.Images + @"Icons\Other\pinpad_password_reset.png";

            BtnResetPassword = new IconButton(
                new ButtonSettings
                {
                    Name = "buttonUserId",
                    Icon = resetPasswordImage,
                    IconSize = new Size(20, 20),
                    ButtonSize = new Size(25, 25)
                })
            { Sensitive = false };


            //Start Validated
            TxtPin.Validate();
            //Event

            //Label Status
            _labelStatus = new Label(GeneralUtils.GetResourceByName("pos_pinpad_message_type_password"));
            _labelStatus.ModifyFont(Pango.FontDescription.FromString(StatusLabelFont));
            _labelStatus.ModifyFg(StateType.Normal, labelStatusFontColor.ToGdkColor());
            _labelStatus.SetAlignment(0.5F, 0.5f);

            TextButton buttonKey1 = CreateNumberedButton("1");
            TextButton buttonKey2 = CreateNumberedButton("2");
            TextButton buttonKey3 = CreateNumberedButton("3");
            TextButton buttonKey4 = CreateNumberedButton("4");
            TextButton buttonKey5 = CreateNumberedButton("5");
            TextButton buttonKey6 = CreateNumberedButton("6");
            TextButton buttonKey7 = CreateNumberedButton("7");
            TextButton buttonKey8 = CreateNumberedButton("8");
            TextButton buttonKey9 = CreateNumberedButton("9");
            TextButton buttonKey0 = CreateNumberedButton("0");

            TextButton buttonKeyCE = new TextButton(
               new ButtonSettings
               {
                   Name = "touchButtonKeyCE_Red",
                   Text = "CE",
                   Font = Font,
                   FontColor = ButtonFontColor,
                   ButtonSize = buttonSize
               });

            TextButton _buttonQuitOrOk;

            BtnOk = new TextButton(new ButtonSettings
            {
                Name = "touchButtonKeyOK_Green",
                Text = GeneralUtils.GetResourceByName("widget_pospinpad_ok"),
                Font = Font,
                FontColor = ButtonFontColor,
                ButtonSize = _btnSize
            })
            { Sensitive = false };


            if (showSystemButtons)
            {

                BtnQuit = new TextButton(
                   new ButtonSettings
                   {
                       Name = "touchButtonKeyQuit_DarkGrey",
                       Text = GeneralUtils.GetResourceByName("global_quit_title"),
                       Font = Font,
                       FontColor = ButtonFontColor,
                       ButtonSize = _btnSize
                   });

                _buttonQuitOrOk = BtnQuit;
            }
            else
            {
                _buttonQuitOrOk = BtnOk;
            }

            buttonKey1.Clicked += BtnKey_Clicked;
            buttonKey2.Clicked += BtnKey_Clicked;
            buttonKey3.Clicked += BtnKey_Clicked;
            buttonKey4.Clicked += BtnKey_Clicked;
            buttonKey5.Clicked += BtnKey_Clicked;
            buttonKey6.Clicked += BtnKey_Clicked;
            buttonKey7.Clicked += BtnKey_Clicked;
            buttonKey8.Clicked += BtnKey_Clicked;
            buttonKey9.Clicked += BtnKey_Clicked;
            buttonKey0.Clicked += BtnKey_Clicked;
            buttonKeyCE.Clicked += BtnCE_Clicked;

            //Prepare Table

            //row0
            int entryPinTablePos = showSystemButtons ? 2 : 3;
            //Without ResetPassword Button
            if (!showSystemButtons)
            {
                _table.Attach(TxtPin, 0, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            }
            //With ResetPassword Button
            else
            {
                _table.Attach(TxtPin, 0, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                _table.Attach(BtnResetPassword, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            }
            //row1
            _table.Attach(buttonKey7, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(buttonKey8, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(buttonKey9, 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //row2
            _table.Attach(buttonKey4, 0, 1, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(buttonKey5, 1, 2, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(buttonKey6, 2, 3, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //row3
            _table.Attach(buttonKey1, 0, 1, 3, 4, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(buttonKey2, 1, 2, 3, 4, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(buttonKey3, 2, 3, 3, 4, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //row4
            _table.Attach(buttonKeyCE, 0, 1, 4, 5, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(buttonKey0, 1, 2, 4, 5, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(_buttonQuitOrOk, 2, 3, 4, 5, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //Row5
            _table.Attach(_labelStatus, 0, 3, 5, 6, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.SetRowSpacing(4, labelStatusRowSpacing);
            //row6
            if (showSystemButtons)
            {
                _table.Attach(BtnOk, 0, 3, 7, 8, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                _table.SetRowSpacing(5, systemButtonsRowSpacing);
            }

            Eventbox.Add(_table);
            Add(Eventbox);



            AddEventHandlers();

        }

        public bool ProcessPassword(Guid userId, string password)
        {
            TxtPin.GrabFocus();

            switch (_mode)
            {
                case NumberPadPinMode.Password:
                    return PasswordIsValid(userId, password);
                case NumberPadPinMode.PasswordOld:
                    ProcessOldPassword(userId, password);
                    break;
                case NumberPadPinMode.PasswordNew:
                    ProcessNewPassword(password);
                    break;
                case NumberPadPinMode.PasswordNewConfirm:
                    ProcessNewPasswordConfirmation(userId, password);
                    break;
                case NumberPadPinMode.PasswordReset:
                    ProcessPasswordReset(userId, password);
                    break;
            }

            return false;
        }

        private void ProcessPasswordReset(Guid userId, string password)
        {
            if (PasswordIsValid(userId, password) == false)
            {
                _mode = NumberPadPinMode.Password;
                return;
            }

            _oldPassword = password;
            Mode = NumberPadPinMode.PasswordNew;
        }

        private void ProcessNewPasswordConfirmation(Guid userId, string password)
        {
            if (_newPassword == password && ChangePassword(userId, _oldPassword, _newPassword))
            {
                CustomAlerts.Information(SourceWindow)
                      .WithTitleResource("window_title_dialog_change_password")
                      .WithMessageResource("pos_pinpad_message_password_changed")
                      .ShowAlert();


                Mode = NumberPadPinMode.Password;
                return;
            }

            CustomAlerts.Error(SourceWindow)
                        .WithTitleResource("window_title_dialog_change_password")
                        .WithMessageResource("pos_pinpad_message_password_confirmation_error")
                        .ShowAlert();

            ClearEntryPinStatusMessage(true);
            Mode = NumberPadPinMode.PasswordNew;
            _newPassword = string.Empty;
        }

        private void ProcessNewPassword(string password)
        {
            if (password == _oldPassword)
            {
                CustomAlerts.Error(SourceWindow)
                            .WithTitleResource("window_title_dialog_change_password")
                            .WithMessageResource("pos_pinpad_message_password_equal_error")
                            .ShowAlert();

                ClearEntryPinStatusMessage(true);
            }
            else
            {
                _newPassword = password;
                Mode = NumberPadPinMode.PasswordNewConfirm;
            }
        }

        private void ProcessOldPassword(Guid userId, string password)
        {
            if (PasswordIsValid(userId, password))
            {
                _oldPassword = password;
                Mode = NumberPadPinMode.PasswordNew;
            }
        }

        public bool PasswordIsValid(Guid userId, string password)
        {
            var loginResult = AuthenticationService.Authenticate(userId, password);

            if (loginResult.IsError)
            {
                if (loginResult.FirstError == ApiErrors.APICommunication)
                {
                    ErrorHandlingService.HandleApiError(loginResult, source: SourceWindow);
                    return false;
                }

                TxtPin.ModifyText(StateType.Normal, Color.Red.ToGdkColor());
                TxtPin.Text = GeneralUtils.GetResourceByName("status_message_pin_error");
                TxtPin.Visibility = true;
                _entryPinShowStatus = true;
                return false;
            }

            TxtPin.ModifyText(StateType.Normal, Color.Black.ToGdkColor());
            TxtPin.Visibility = false;
            _entryPinShowStatus = false;
            JwtToken = loginResult.Value;
            Mode = NumberPadPinMode.Password;
            return true;
        }

        private void ClearEntryPinStatusMessage(bool pForceClear = false)
        {
            if (_entryPinShowStatus || pForceClear)
            {
                TxtPin.ModifyText(StateType.Normal, Color.Black.ToGdkColor());
                TxtPin.Text = string.Empty;
                TxtPin.Visibility = false;
                _entryPinShowStatus = false;
            }
        }

        private void UpdateLables()
        {
            switch (_mode)
            {
                case NumberPadPinMode.Password:
                    _labelStatus.Text = GeneralUtils.GetResourceByName("pos_pinpad_message_type_password");
                    BtnOk.ButtonLabel.Text = !_notLoginAuth ? GeneralUtils.GetResourceByName("widget_pospinpad_login") : GeneralUtils.GetResourceByName("widget_pospinpad_ok");
                    break;
                case NumberPadPinMode.PasswordOld:
                    _labelStatus.Text = GeneralUtils.GetResourceByName("pos_pinpad_message_type_old_password");
                    BtnOk.ButtonLabel.Text = GeneralUtils.GetResourceByName("widget_pospinpad_ok");
                    break;
                case NumberPadPinMode.PasswordNew:
                    _labelStatus.Text = GeneralUtils.GetResourceByName("pos_pinpad_message_type_new_password");
                    BtnOk.ButtonLabel.Text = GeneralUtils.GetResourceByName("widget_pospinpad_ok");
                    break;
                case NumberPadPinMode.PasswordNewConfirm:
                    _labelStatus.Text = GeneralUtils.GetResourceByName("pos_pinpad_message_type_new_password_confirm");
                    BtnOk.ButtonLabel.Text = GeneralUtils.GetResourceByName("widget_pospinpad_ok");
                    break;
                default:
                    break;
            }

            ClearEntryPinStatusMessage(true);
        }

        private bool ChangePassword(Guid userId,
                                    string oldPassword,
                                    string newPassword)
        {
            var changePasswordResult = _mediator.Send(new ResetPasswordCommand(userId, oldPassword, newPassword)).Result;

            if (changePasswordResult.IsError)
            {
                ErrorHandlingService.HandleApiError(changePasswordResult, source: SourceWindow);
                return false;
            }

            return true;
        }
    }
}
