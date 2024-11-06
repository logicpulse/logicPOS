using Gtk;
using logicpos.Classes.Enums.Widgets;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Widgets;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class UserPinPanel : BaseDialog
    {
        private readonly UserDetail _selectedUser;
        private readonly NumberPadPin _numberPadPin;
        private readonly bool _notLoginAuth;

        public UserPinPanel(Window parentWindow,
                               DialogFlags flags,
                               UserDetail user,
                               bool notLoginAuth = false)
            : base(parentWindow, flags)
        {
            _notLoginAuth = notLoginAuth;
            bool showCancel = false;
            int DialogHeight = (showCancel) ? 465 : 440;
            _selectedUser = user;
            string windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_request_user_pin");
            Size windowSize = new Size(332, DialogHeight);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_users.png";
            string fontNumberPadPinButtonKeysTextAndLabel = AppSettings.Instance.fontNumberPadPinButtonKeysTextAndLabel;
            ActionAreaButtons actionAreaButtons;

            Fixed fixedContent = new Fixed();

            _numberPadPin = new NumberPadPin(parentWindow,
                                             "numberPadPin",
                                             Color.Transparent,
                                             fontNumberPadPinButtonKeysTextAndLabel,
                                             "12",
                                             Color.White,
                                             Color.Black,
                                             100,
                                             67,
                                             _notLoginAuth);
            _numberPadPin.ButtonKeyOK.Clicked += ButtonKeyOK_Clicked;

            fixedContent.Put(_numberPadPin, 0, 0);

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

            _numberPadPin.Mode = (_selectedUser.PasswordReset) ? NumberPadPinMode.PasswordOld :
                NumberPadPinMode.Password;

            this.KeyReleaseEvent += PosPinPadDialog_KeyReleaseEvent;

            this.Initialize(this,
                            flags,
                            fileDefaultWindowIcon,
                            windowTitle,
                            windowSize,
                            fixedContent,
                            actionAreaButtons);
        }

        private void ButtonKeyOK_Clicked(object sender, EventArgs e)
        {
            bool finishedJob = _numberPadPin.ProcessPassword(this, _selectedUser, _notLoginAuth);
            if (finishedJob) Respond(ResponseType.Ok);
        }

        //Removed : Conflited with Change Password, When we Implement Default Enter Key in All Dilogs, It Trigger Twice
        private void PosPinPadDialog_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            //if (args.Event.Key.ToString().Equals("Return"))
            //{
            //    bool finishedJob = _numberPadPin.ProcessPassword(this, _selectedUserDetail);
            //    if (finishedJob) Respond(ResponseType.Ok);
            //}
        }
    }
}
