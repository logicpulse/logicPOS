using ErrorOr;
using Gtk;
using logicpos;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Enums.Widgets;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Authentication.Login;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Settings;
using LogicPOS.Shared;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;

namespace LogicPOS.UI.Widgets
{
    public class NumberPadPin : Box
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        protected Gtk.Table _table;
        private readonly Window _sourceWindow;
        private int _tempCursorPosition = 0;
        private bool _entryPinShowStatus = false;
        private readonly Label _labelStatus;
        private string _newPassword;
        private bool _notLoginAuth;

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

        public NumberPadPin(Window parentWindow,
                            string pName,
                            Color pButtonColor,
                            string pFont,
                            string pFontLabelStatus,
                            Color pFontColor,
                            Color pFontColorLabelStatus,
                            byte pButtonWidth,
                            byte pButtonHeight,
                            bool pNotLoginAuth,
                            bool pShowSystemButtons = false,
                            bool pVisibleWindow = false,
                            uint pRowSpacingLabelStatus = 20,
                            uint pRowSpacingSystemButtons = 40,
                            byte pPadding = 3)
        {
            _sourceWindow = parentWindow;
            Name = pName;
            _notLoginAuth = pNotLoginAuth;
            //Show or Hide System Buttons (Startup Visible, Pos Change User Invisible)
            uint tableRows = pShowSystemButtons ? 5 : (uint)3;

            Eventbox = new EventBox() { VisibleWindow = pVisibleWindow };

            _table = new Gtk.Table(tableRows, 3, true);
            _table.Homogeneous = false;

            //Pin Entry
            EntryPin = new ValidatableTextBox(parentWindow,
                                              KeyboardMode.None,
                                              RegexUtils.RegexLoginPin,
                                              true)

            {
                InvisibleChar = '*',
                Visibility = false
            };

            EntryPin.ModifyFont(Pango.FontDescription.FromString(pFont));
            EntryPin.Alignment = 0.5F;

            //ResetPassword
            string numberPadPinButtonPasswordResetImageFileName = PathsSettings.ImagesFolderLocation + @"Icons\Other\pinpad_password_reset.png";

            ButtonKeyResetPassword = new IconButton(
                new ButtonSettings
                {
                    Name = "touchButtonKeyPasswordReset",
                    //BackgroundColor = Color.Transparent,
                    BackgroundColor = Color.FromArgb(70, 167, 167, 167),
                    Icon = numberPadPinButtonPasswordResetImageFileName,
                    IconSize = new Size(20, 20),
                    ButtonSize = new Size(25, 25)
                })
            { Sensitive = false };


            //Start Validated
            EntryPin.Validate();
            //Event
            EntryPin.Changed += _entryPin_Changed;
            EntryPin.KeyReleaseEvent += _entryPin_KeyReleaseEvent;

            //Label Status
            _labelStatus = new Label(GeneralUtils.GetResourceByName("pos_pinpad_message_type_password"));
            _labelStatus.ModifyFont(Pango.FontDescription.FromString(pFontLabelStatus));
            _labelStatus.ModifyFg(StateType.Normal, pFontColorLabelStatus.ToGdkColor());
            _labelStatus.SetAlignment(0.5F, 0.5f);

            var buttonSize = new Size(pButtonWidth, pButtonHeight);

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
                   BackgroundColor = pButtonColor,
                   Text = "CE",
                   Font = pFont,
                   FontColor = pFontColor,
                   ButtonSize = buttonSize
               });

            TextButton _buttonQuitOrOk;

            ButtonKeyOK = new TextButton(new ButtonSettings
            {
                Name = "touchButtonKeyOK_Green",
                BackgroundColor = pButtonColor,
                Text = GeneralUtils.GetResourceByName("widget_pospinpad_ok"),
                Font = pFont,
                FontColor = pFontColor,
                ButtonSize = new Size(pButtonWidth, pButtonHeight)
            })
            { Sensitive = false };


            if (pShowSystemButtons)
            {
                ButtonKeyFrontOffice = new TextButton(new ButtonSettings
                {
                    Name = "touchButtonKeyFrontOffice_DarkGrey",
                    BackgroundColor = pButtonColor,
                    Text = "FO",
                    Font = pFont,
                    FontColor = pFontColor,
                    ButtonSize = new Size(pButtonWidth, pButtonHeight)
                })
                { Sensitive = false, Visible = false };

                ButtonKeyQuit = new TextButton(
                   new ButtonSettings
                   {
                       Name = "touchButtonKeyQuit_DarkGrey",
                       BackgroundColor = pButtonColor,
                       Text = GeneralUtils.GetResourceByName("global_quit_title"),
                       Font = pFont,
                       FontColor = pFontColor,
                       ButtonSize = new Size(pButtonWidth, pButtonHeight)
                   });

                ButtonKeyBackOffice = new TextButton(
                    new ButtonSettings
                    {
                        Name = "touchButtonKeyBackOffice_DarkGrey",
                        BackgroundColor = pButtonColor,
                        Text = "BO",
                        Font = pFont,
                        FontColor = pFontColor,
                        ButtonSize = new Size(pButtonWidth, pButtonHeight)
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
            buttonKeyCE.Clicked += buttonKeyCE_Clicked;

            //Prepare Table

            //row0
            int entryPinTablePos = pShowSystemButtons ? 2 : 3;
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
                _table.Attach(ButtonKeyOK, 0, 3, 7, 8, AttachOptions.Fill, AttachOptions.Fill, pPadding, pPadding);
                _table.SetRowSpacing(5, pRowSpacingSystemButtons);
            }

            Eventbox.Add(_table);
            Add(Eventbox);

            TextButton CreateNumberedButton(string number)
            {
                return new TextButton(
                    new ButtonSettings
                    {
                        Name = $"touchButtonKey{number}",
                        //BackgroundColor = pButtonColor,
                        BackgroundColor = Color.FromArgb(70, 167, 167, 167),
                        Text = number,
                        Font = pFont,
                        FontColor = pFontColor,
                        ButtonSize = buttonSize
                    });
            }
        }

        private void _entryPin_Changed(object sender, EventArgs e)
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
                        _mode = NumberPadPinMode.PasswordNew;
                        UpdateStatusLabels();
                    }
                    break;
                case NumberPadPinMode.PasswordNew:
                    if (Login(user.Id, EntryPin.Text).IsError == false)
                    {
                        ResponseType response = Utils.ShowMessageTouch(parentWindow,
                                                                       DialogFlags.Modal,
                                                                       MessageType.Error,
                                                                       ButtonsType.Ok,
                                                                       GeneralUtils.GetResourceByName("window_title_dialog_change_password"),
                                                                       GeneralUtils.GetResourceByName("pos_pinpad_message_password_equal_error"));
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
                        user.AccessPin = CryptographyUtils.GenerateSaltedString(_newPassword);
                        user.PasswordReset = false;
                        user.PasswordResetDate = XPOUtility.CurrentDateTimeAtomic();

                        ResponseType response = Utils.ShowMessageTouch(parentWindow,
                                                                           DialogFlags.Modal,
                                                                           MessageType.Info,
                                                                           ButtonsType.Ok,
                                                                           GeneralUtils.GetResourceByName("window_title_dialog_change_password"),
                                                                           GeneralUtils.GetResourceByName("pos_pinpad_message_password_changed"));
                        ProcessLogin(user);
                        result = true;
                    }
                    else
                    {
                        ResponseType responseType = Utils.ShowMessageTouch(parentWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, GeneralUtils.GetResourceByName("window_title_dialog_change_password"), GeneralUtils.GetResourceByName("pos_pinpad_message_password_confirmation_error"));
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
            var loginResult = _mediator.Send(new LoginQuery(userId, password)).Result;
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
                    SimpleAlerts.ShowApiErrorAlert(_sourceWindow, loginResult.FirstError);
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
            AuthenticationService.LoginUser(user);
            UpdateStatusLabels();

            if (GeneralSettings.AppUseBackOfficeMode)
            {
                Utils.ShowBackOffice(_sourceWindow);
            }
            else
            {
                Utils.ShowFrontOffice(_sourceWindow);
            }
        }
    }
}
