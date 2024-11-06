using Gtk;
using logicpos;
using logicpos.Classes.Enums.Widgets;
using LogicPOS.Api.Entities;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Shared;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Users;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Windows
{
    public partial class LoginWindow
    {
        private void BtnOK_Clicked(object sender, EventArgs e)
        {
            PinPanel.ProcessPassword(this, UsersMenu.SelectedUser);
        }

        private void ButtonKeyResetPassword_Clicked(object sender, EventArgs e)
        {
            string currentPin = PinPanel.EntryPin.Text;
            PinPanel.Mode = NumberPadPinMode.PasswordReset;
            PinPanel.EntryPin.Text = currentPin;
            PinPanel.ProcessPassword(this, UsersMenu.SelectedUser);
        }

        private void StartupWindow_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key.ToString().Equals("Return"))
            {
                PinPanel.ProcessPassword(this, UsersMenu.SelectedUser);
            }
        }

        private void ButtonKeyQuit_Clicked(object sender, EventArgs e)
        {
            LogicPOSAppUtils.Quit(this);
        }

        private void ButtonKeyFrontOffice_Clicked(object sender, EventArgs e)
        {
            Utils.ShowFrontOffice(this);
        }

        private void ButtonKeyBackOffice_Clicked(object sender, EventArgs e)
        {
            Utils.ShowBackOffice(this);
        }

        private void OnUserSelected(UserDetail user)
        {
            PinPanel.Mode = (user.PasswordReset) ? NumberPadPinMode.PasswordOld : NumberPadPinMode.Password;

            if (user.PasswordReset)
            {
                Utils.ShowMessageTouch(this,
                                       DialogFlags.Modal,
                                       MessageType.Info,
                                       ButtonsType.Ok,
                                       CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_information"),
                                       string.Format(GeneralUtils.GetResourceByName("dialog_message_user_request_change_password"), user.Name, XPOSettings.DefaultValueUserDetailAccessPin));
            }

            PinPanel.EntryPin.GrabFocus();
        }

        public void LogOutUser(bool showLoginWindow = true)
        {
            AuthenticationService.LogoutUser();

            if (showLoginWindow) LoginWindow.Instance.ShowAll();
        }

        private void ButtonDeveloper_Clicked(object sender, EventArgs e)
        {

        }
    }
}
