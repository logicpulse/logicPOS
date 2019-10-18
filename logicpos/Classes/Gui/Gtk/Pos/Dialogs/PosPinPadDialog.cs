using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using System;
using System.Drawing;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Widgets;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    class PosPinPadDialog : PosBaseDialog
    {
        private sys_userdetail _selectedUserDetail;
        private NumberPadPin _numberPadPin;

        public PosPinPadDialog(Window pSourceWindow, DialogFlags pDialogFlags, sys_userdetail pUserDetail)
            : base(pSourceWindow, pDialogFlags)
        {
            //Dialog compile time preferences
            Boolean showCancel = false;
            int DialogHeight = (showCancel) ? 465 : 440;//465 : 400;
            //Init Local Vars Parameters
            _selectedUserDetail = pUserDetail;
            //Init Local Vars
            String windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_request_user_pin");
            Size windowSize = new Size(332, DialogHeight);
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_users.png");
            String fontNumberPadPinButtonKeysTextAndLabel = GlobalFramework.Settings["fontNumberPadPinButtonKeysTextAndLabel"];
            ActionAreaButtons actionAreaButtons;

            //Init Content
            Fixed fixedContent = new Fixed();

            //NumberPadPin
            _numberPadPin = new NumberPadPin(pSourceWindow, "numberPadPin", System.Drawing.Color.Transparent, fontNumberPadPinButtonKeysTextAndLabel, "12" ,Color.White, Color.Black, 100, 67);
            _numberPadPin.ButtonKeyOK.Clicked += ButtonKeyOK_Clicked;

            fixedContent.Put(_numberPadPin, 0, 0);

            if (showCancel)
            {
                //ActionArea Buttons
                TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

                //ActionArea
                actionAreaButtons = new ActionAreaButtons();
                actionAreaButtons.Add(new ActionAreaButton(buttonCancel, ResponseType.Cancel));
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

        void ButtonKeyOK_Clicked(object sender, EventArgs e)
        {
            bool finishedJob = _numberPadPin.ProcessPassword(this, _selectedUserDetail);
            if (finishedJob) Respond(ResponseType.Ok);
        }

        //Removed : Conflited with Change Password, When we Implement Default Enter Key in All Dilogs, It Trigger Twice
        void PosPinPadDialog_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            //if (args.Event.Key.ToString().Equals("Return"))
            //{
            //    bool finishedJob = _numberPadPin.ProcessPassword(this, _selectedUserDetail);
            //    if (finishedJob) Respond(ResponseType.Ok);
            //}
        }
    }
}
