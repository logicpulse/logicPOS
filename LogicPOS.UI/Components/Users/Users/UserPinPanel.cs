using ErrorOr;
using Gtk;
using logicpos;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Enums.Widgets;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Authentication.Login;
using LogicPOS.Api.Features.Users.ResetPassword;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;

namespace LogicPOS.UI.Widgets
{
    public class UserPinPanel : Box
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        protected Gtk.Table _table;
        private Window SourceWindow { get; }
        private int _tempCursorPosition = 0;
        private bool _entryPinShowStatus = false;
        private readonly Label _labelStatus;
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
                UpdateStatusLabels();
            }
        }

        public EventBox Eventbox { get; set; }

        public ValidatableTextBox TxtPin { get; set; }
        public TextButton BtnOk { get; set; }

        public IconButton ButtonKeyResetPassword { get; set; }
        public TextButton ButtonKeyFrontOffice { get; set; }

        public TextButton ButtonKeyBackOffice { get; set; }
        public TextButton ButtonKeyQuit { get; set; }

        public UserPinPanel(Window parentWindow,
                            string name,
                            Color btnColor,
                            string font,
                            string labelStatusFont,
                            Color fontColor,
                            Color labelStatusFontColor,
                            byte btnWidth,
                            byte btnHeight,
                            bool notLoginAuth,
                            bool showSystemButtons = false,
                            bool showWindow = false,
                            uint labelStatusRowSpacing = 20,
                            uint systemButtonsRowSpacing = 40,
                            byte padding = 3)
        {
            SourceWindow = parentWindow;
            Name = name;
            _notLoginAuth = notLoginAuth;
            uint tableRows = showSystemButtons ? 5 : (uint)3;

            Eventbox = new EventBox() { VisibleWindow = showWindow };

            _table = new Gtk.Table(tableRows, 3, true);
            _table.Homogeneous = false;

            TxtPin = new ValidatableTextBox(parentWindow,
                                              KeyboardMode.None,
                                              RegularExpressions.LoginPin,
                                              true)

            {
                InvisibleChar = '*',
                Visibility = false
            };

            TxtPin.ModifyFont(Pango.FontDescription.FromString(font));
            TxtPin.Alignment = 0.5F;

            string resetPasswordImage = PathsSettings.ImagesFolderLocation + @"Icons\Other\pinpad_password_reset.png";

            ButtonKeyResetPassword = new IconButton(
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
            TxtPin.Changed += TxtPin_Changed;
            TxtPin.KeyReleaseEvent += TxtPin_KeyReleaseEvent;

            //Label Status
            _labelStatus = new Label(GeneralUtils.GetResourceByName("pos_pinpad_message_type_password"));
            _labelStatus.ModifyFont(Pango.FontDescription.FromString(labelStatusFont));
            _labelStatus.ModifyFg(StateType.Normal, labelStatusFontColor.ToGdkColor());
            _labelStatus.SetAlignment(0.5F, 0.5f);

            var buttonSize = new Size(btnWidth, btnHeight);

            TextButton buttonKey1 = CreateNumberedButton("1");
            var color = buttonKey1.Style.Background(StateType.Normal);

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
                   BackgroundColor = btnColor,
                   Text = "CE",
                   Font = font,
                   FontColor = fontColor,
                   ButtonSize = buttonSize
               });

            TextButton _buttonQuitOrOk;

            BtnOk = new TextButton(new ButtonSettings
            {
                Name = "touchButtonKeyOK_Green",
                BackgroundColor = btnColor,
                Text = GeneralUtils.GetResourceByName("widget_pospinpad_ok"),
                Font = font,
                FontColor = fontColor,
                ButtonSize = new Size(btnWidth, btnHeight)
            })
            { Sensitive = false };


            if (showSystemButtons)
            {
                ButtonKeyFrontOffice = new TextButton(new ButtonSettings
                {
                    Name = "touchButtonKeyFrontOffice_DarkGrey",
                    BackgroundColor = btnColor,
                    Text = "FO",
                    Font = font,
                    FontColor = fontColor,
                    ButtonSize = new Size(btnWidth, btnHeight)
                })
                { Sensitive = false, Visible = false };

                ButtonKeyQuit = new TextButton(
                   new ButtonSettings
                   {
                       Name = "touchButtonKeyQuit_DarkGrey",
                       BackgroundColor = btnColor,
                       Text = GeneralUtils.GetResourceByName("global_quit_title"),
                       Font = font,
                       FontColor = fontColor,
                       ButtonSize = new Size(btnWidth, btnHeight)
                   });

                ButtonKeyBackOffice = new TextButton(
                    new ButtonSettings
                    {
                        Name = "touchButtonKeyBackOffice_DarkGrey",
                        BackgroundColor = btnColor,
                        Text = "BO",
                        Font = font,
                        FontColor = fontColor,
                        ButtonSize = new Size(btnWidth, btnHeight)
                    })
                { Sensitive = false, Visible = false };

                _buttonQuitOrOk = ButtonKeyQuit;
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
                _table.Attach(ButtonKeyResetPassword, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
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

            TextButton CreateNumberedButton(string number)
            {
                return new TextButton(
                    new ButtonSettings
                    {
                        Name = "buttonUserId",
                        BackgroundColor = btnColor,
                        Text = number,
                        Font = font,
                        FontColor = fontColor,
                        ButtonSize = buttonSize
                    });
            }
        }

        private void TxtPin_Changed(object sender, EventArgs e)
        {
            ValidatableTextBox entry = (ValidatableTextBox)sender;
            ClearEntryPinStatusMessage();
            BtnOk.Sensitive = entry.Validated;
            ButtonKeyResetPassword.Sensitive = entry.Validated && _mode == NumberPadPinMode.Password;
        }

        private void BtnKey_Clicked(object sender, EventArgs e)
        {
            TextButton button = (TextButton)sender;
            ClearEntryPinStatusMessage();
            TxtPin.InsertText(button.ButtonLabel.Text, ref _tempCursorPosition);
            TxtPin.GrabFocus();
            TxtPin.Position = TxtPin.Text.Length;
        }

        private void BtnCE_Clicked(object sender, EventArgs e)
        {
            ClearEntryPinStatusMessage();
            TxtPin.DeleteText(TxtPin.Text.Length - 1, TxtPin.Text.Length);
            TxtPin.GrabFocus();
            TxtPin.Position = TxtPin.Text.Length;
        }

        private void TxtPin_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key.ToString().Equals("Return"))
            {
                if (TxtPin.Validated)
                {
                    BtnOk.Click();
                }
            }
        }

        public bool ProcessPassword(Window parentWindow,
                                    UserDetail user,
                                    bool notLoginAuth = false)
        {
            bool result = false;
            _notLoginAuth = notLoginAuth;

            switch (_mode)
            {
                case NumberPadPinMode.Password:
                    if (ValidatePassword(user))
                    {
                        if (!_notLoginAuth)
                        {
                            ProcessLogin(user);
                        }
                        result = true;
                    }
                    break;
                case NumberPadPinMode.PasswordOld:
                    if (ValidatePassword(user))
                    {
                        _oldPassword = TxtPin.Text;
                        _mode = NumberPadPinMode.PasswordNew;
                        UpdateStatusLabels();
                    }
                    break;
                case NumberPadPinMode.PasswordNew:
                    if (TxtPin.Text == _oldPassword)
                    {
                        CustomAlerts.Error(parentWindow)
                                    .WithTitleResource("window_title_dialog_change_password")
                                    .WithMessageResource("pos_pinpad_message_password_equal_error")
                                    .ShowAlert();

                        ClearEntryPinStatusMessage(true);
                    }
                    else
                    {
                        _newPassword = TxtPin.Text;
                        _mode = NumberPadPinMode.PasswordNewConfirm;
                        UpdateStatusLabels();
                    }
                    break;
                case NumberPadPinMode.PasswordNewConfirm:
                    if (_newPassword == TxtPin.Text)
                    {
                        ChangePassword(user.Id, _oldPassword, _newPassword);

                        CustomAlerts.Information(parentWindow)
                                    .WithTitleResource("window_title_dialog_change_password")
                                    .WithMessageResource("pos_pinpad_message_password_changed")
                                    .ShowAlert();

                        ProcessLogin(user);
                        result = true;
                    }
                    else
                    {

                        CustomAlerts.Error(parentWindow)
                                    .WithTitleResource("window_title_dialog_change_password")
                                    .WithMessageResource("pos_pinpad_message_password_confirmation_error")
                                    .ShowAlert();

                        ClearEntryPinStatusMessage(true);
                        _mode = NumberPadPinMode.PasswordNew;
                        UpdateStatusLabels();
                        _newPassword = string.Empty;
                    }
                    break;
                case NumberPadPinMode.PasswordReset:
                    if (ValidatePassword(user))
                    {
                        _mode = NumberPadPinMode.PasswordNew;
                        UpdateStatusLabels();
                    }
                    else
                    {
                        _mode = NumberPadPinMode.Password;
                    }
                    break;
                default:
                    break;
            }

            TxtPin.GrabFocus();

            return result;
        }

        private ErrorOr<string> Login(Guid userId, string password)
        {
            var loginResult = _mediator.Send(new LoginQuery(TerminalService.Terminal.Id, userId, password)).Result;
            JwtToken = loginResult.IsError ? null : loginResult.Value;
            return loginResult;
        }

        public bool ValidatePassword(UserDetail user)
        {
            string password = TxtPin.Text;

            var loginResult = Login(user.Id, password);

            if (loginResult.IsError)
            {
                if (loginResult.FirstError == ApiErrors.CommunicationError)
                {
                    CustomAlerts.ShowApiErrorAlert(SourceWindow, loginResult.FirstError);
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

        private void UpdateStatusLabels()
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

        private void ProcessLogin(UserDetail user)
        {
            _mode = NumberPadPinMode.Password;
            AuthenticationService.LoginUser(user, JwtToken);

            UpdateStatusLabels();

            if (GeneralSettings.AppUseBackOfficeMode)
            {
                SourceWindow.Hide();
                BackOfficeWindow.ShowBackOffice();
                return;
            }
            SourceWindow.Hide();
            POSWindow.ShowPOS();
        }

        private bool ChangePassword(Guid userId,
                                    string oldPassword,
                                    string newPassword)
        {
            var changePasswordResult = _mediator.Send(new ResetPasswordCommand(userId, oldPassword, newPassword)).Result;

            if (changePasswordResult.IsError)
            {
                CustomAlerts.ShowApiErrorAlert(SourceWindow, changePasswordResult.FirstError);
                return false;
            }

            return true;
        }
    }
}
