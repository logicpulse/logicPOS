using Gtk;
using logicpos.Classes.Enums.Widgets;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Widgets;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class UserPinModal : BaseDialog
    {
        private readonly User _user;
        private readonly UserPinPanel _pinPanel;
        private readonly bool _notLoginAuth;
        public string JwtToken { get; private set; }

        public UserPinModal(Window parentWindow,
                            User user,
                            bool notLoginAuth = false)
            : base(parentWindow, DialogFlags.DestroyWithParent)
        {
            _notLoginAuth = notLoginAuth;
            bool showCancel = false;
            int DialogHeight = (showCancel) ? 465 : 440;
            _user = user;
            string windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_request_user_pin");
            Size windowSize = new Size(332, DialogHeight);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_users.png";
            string fontNumberPadPinButtonKeysTextAndLabel = AppSettings.Instance.fontNumberPadPinButtonKeysTextAndLabel;
            ActionAreaButtons actionAreaButtons;

            Fixed fixedContent = new Fixed();

            _pinPanel = new UserPinPanel(parentWindow,
                                             "numberPadPin",
                                             Color.Transparent,
                                             fontNumberPadPinButtonKeysTextAndLabel,
                                             "12",
                                             Color.White,
                                             Color.Black,
                                             100,
                                             67,
                                             _notLoginAuth);
            _pinPanel.BtnOk.Clicked += ButtonKeyOK_Clicked;

            fixedContent.Put(_pinPanel, 0, 0);

            if (showCancel)
            {
                IconButtonWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);

                actionAreaButtons = new ActionAreaButtons
                {
                    new ActionAreaButton(buttonCancel, ResponseType.Cancel)
                };
            }
            else
            {
                actionAreaButtons = new ActionAreaButtons();
            }

            if(_user == null)
            {
                _user = AuthenticationService.User;
            }

            _pinPanel.Mode = (_user.PasswordReset) ? NumberPadPinMode.PasswordOld : NumberPadPinMode.Password;


            this.Initialize(this,
                            DialogFlags.DestroyWithParent,
                            fileDefaultWindowIcon,
                            windowTitle,
                            windowSize,
                            fixedContent,
                            actionAreaButtons);
        }

        private void ButtonKeyOK_Clicked(object sender, EventArgs e)
        {
            bool result = _pinPanel.ProcessPassword(this, _user, _notLoginAuth);
            if (result)
            {
                JwtToken = _pinPanel.JwtToken;
                Respond(ResponseType.Ok);
            }
        }
    }
}
