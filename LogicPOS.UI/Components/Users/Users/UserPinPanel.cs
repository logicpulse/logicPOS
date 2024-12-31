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

        public ValidatableTextBox EntryPin { get; set; }
        public TextButton ButtonKeyOK { get; set; }

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

            EntryPin = new ValidatableTextBox(parentWindow,
                                              KeyboardMode.None,
                                              RegularExpressions.LoginPin,
                                              true)

            {
                InvisibleChar = '*',
                Visibility = false
            };

            EntryPin.ModifyFont(Pango.FontDescription.FromString(font));
            EntryPin.Alignment = 0.5F;

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
            EntryPin.Validate();
            //Event
            EntryPin.Changed += TxtPin_Changed;
            EntryPin.KeyReleaseEvent += TxtPin_KeyReleaseEvent;

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

            ButtonKeyOK = new TextButton(new ButtonSettings
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
                _buttonQuitOrOk = ButtonKeyOK;
            }

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
            buttonKeyCE.Clicked += BtnCE_Clicked;

            //Prepare Table

            //row0
            int entryPinTablePos = showSystemButtons ? 2 : 3;
            //Without ResetPassword Button
            if (!showSystemButtons)
            {
                _table.Attach(EntryPin, 0, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            }
            //With ResetPassword Button
            else
            {
                _table.Attach(EntryPin, 0, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
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
                _table.Attach(ButtonKeyOK, 0, 3, 7, 8, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
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
            ButtonKeyOK.Sensitive = entry.Validated;
            ButtonKeyResetPassword.Sensitive = entry.Validated && _mode == NumberPadPinMode.Password;
        }

        private void buttonKey_Clicked(object sender, EventArgs e)
        {
            TextButton button = (TextButton)sender;
            ClearEntryPinStatusMessage();
            EntryPin.InsertText(button.ButtonLabel.Text, ref _tempCursorPosition);
            EntryPin.GrabFocus();
            EntryPin.Position = EntryPin.Text.Length;
        }

        private void BtnCE_Clicked(object sender, EventArgs e)
        {
            ClearEntryPinStatusMessage();
            //_entryPin.DeleteText(0, _entryPin.Text.Length);
            EntryPin.DeleteText(EntryPin.Text.Length - 1, EntryPin.Text.Length);
            //_tempCursorPosition = _entryPin.Position;
            EntryPin.GrabFocus();
            EntryPin.Position = EntryPin.Text.Length;
        }

        private void TxtPin_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key.ToString().Equals("Return"))
            {
                if (EntryPin.Validated)
                {
                    ButtonKeyOK.Click();
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
                        _oldPassword = EntryPin.Text;
                        _mode = NumberPadPinMode.PasswordNew;
                        UpdateStatusLabels();
                    }
                    break;
                case NumberPadPinMode.PasswordNew:
                    if (EntryPin.Text == _oldPassword)
                    {
                        CustomAlerts.Error(parentWindow)
                                    .WithTitleResource("window_title_dialog_change_password")
                                    .WithMessageResource("pos_pinpad_message_password_equal_error")
                                    .ShowAlert();

                        ClearEntryPinStatusMessage(true);
                    }
                    else
                    {
                        _newPassword = EntryPin.Text;
                        _mode = NumberPadPinMode.PasswordNewConfirm;
                        UpdateStatusLabels();
                    }
                    break;
                case NumberPadPinMode.PasswordNewConfirm:
                    if (_newPassword == EntryPin.Text)
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

            EntryPin.GrabFocus();

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
            string password = EntryPin.Text;

            var loginResult = Login(user.Id, password);

            if (loginResult.IsError)
            {
                if (loginResult.FirstError == ApiErrors.CommunicationError)
                {
                    CustomAlerts.ShowApiErrorAlert(SourceWindow, loginResult.FirstError);
                    return false;
                }

                EntryPin.ModifyText(StateType.Normal, Color.Red.ToGdkColor());
                EntryPin.Text = GeneralUtils.GetResourceByName("status_message_pin_error");
                EntryPin.Visibility = true;
                _entryPinShowStatus = true;
                return false;
            }

            EntryPin.ModifyText(StateType.Normal, Color.Black.ToGdkColor());
            EntryPin.Visibility = false;
            _entryPinShowStatus = false;
            return true;
        }

        private void ClearEntryPinStatusMessage(bool pForceClear = false)
        {
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
                    _labelStatus.Text = GeneralUtils.GetResourceByName("pos_pinpad_message_type_password");
                    ButtonKeyOK.ButtonLabel.Text = !_notLoginAuth ? GeneralUtils.GetResourceByName("widget_pospinpad_login") : GeneralUtils.GetResourceByName("widget_pospinpad_ok");
                    break;
                case NumberPadPinMode.PasswordOld:
                    _labelStatus.Text = GeneralUtils.GetResourceByName("pos_pinpad_message_type_old_password");
                    ButtonKeyOK.ButtonLabel.Text = GeneralUtils.GetResourceByName("widget_pospinpad_ok");
                    break;
                case NumberPadPinMode.PasswordNew:
                    _labelStatus.Text = GeneralUtils.GetResourceByName("pos_pinpad_message_type_new_password");
                    ButtonKeyOK.ButtonLabel.Text = GeneralUtils.GetResourceByName("widget_pospinpad_ok");
                    break;
                case NumberPadPinMode.PasswordNewConfirm:
                    _labelStatus.Text = GeneralUtils.GetResourceByName("pos_pinpad_message_type_new_password_confirm");
                    ButtonKeyOK.ButtonLabel.Text = GeneralUtils.GetResourceByName("widget_pospinpad_ok");
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
                BackOfficeWindow.ShowBackOffice(SourceWindow);
                return;
            }

            POSWindow.ShowPOSWindow(SourceWindow);
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
