using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.DataLayer.Xpo;
using System;
using System.Drawing;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using LogicPOS.Settings;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosPinPadDialog : PosBaseDialog
    {
        private readonly sys_userdetail _selectedUserDetail;
        private readonly NumberPadPin _numberPadPin;
        private readonly bool _notLoginAuth;

        public PosPinPadDialog(Window pSourceWindow, DialogFlags pDialogFlags, sys_userdetail pUserDetail, bool pNotLoginAuth = false)
            : base(pSourceWindow, pDialogFlags)
        {
            _notLoginAuth = pNotLoginAuth;
            //Dialog compile time preferences
            bool showCancel = false;
            int DialogHeight = (showCancel) ? 465 : 440;//465 : 400;
            //Init Local Vars Parameters
            _selectedUserDetail = pUserDetail;
            //Init Local Vars
            string windowTitle = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_request_user_pin");
            Size windowSize = new Size(332, DialogHeight);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_users.png";
            string fontNumberPadPinButtonKeysTextAndLabel = GeneralSettings.Settings["fontNumberPadPinButtonKeysTextAndLabel"];
            ActionAreaButtons actionAreaButtons;

            //Init Content
            Fixed fixedContent = new Fixed();

            //NumberPadPin
            _numberPadPin = new NumberPadPin(pSourceWindow, "numberPadPin", Color.Transparent, fontNumberPadPinButtonKeysTextAndLabel, "12", Color.White, Color.Black, 100, 67, _notLoginAuth);
            _numberPadPin.ButtonKeyOK.Clicked += ButtonKeyOK_Clicked;

            fixedContent.Put(_numberPadPin, 0, 0);

            if (showCancel)
            {
                //ActionArea Buttons
                TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

                //ActionArea
                actionAreaButtons = new ActionAreaButtons
                {
                    new ActionAreaButton(buttonCancel, ResponseType.Cancel)
                };
            }
            else
            {
                actionAreaButtons = new ActionAreaButtons();
            }

            //Init Mode
            _numberPadPin.Mode = (_selectedUserDetail.PasswordReset) ? NumberPadPinMode.PasswordOld :
                NumberPadPinMode.Password;

            //Events
            this.KeyReleaseEvent += PosPinPadDialog_KeyReleaseEvent;

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, fixedContent, actionAreaButtons);
        }

        private void ButtonKeyOK_Clicked(object sender, EventArgs e)
        {
            bool finishedJob = _numberPadPin.ProcessPassword(this, _selectedUserDetail, _notLoginAuth);
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
