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
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Windows
{
    public partial class StartupWindow
    {
        private void ButtonKeyOK_Clicked(object sender, EventArgs e)
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
            LogicPOSApp.Quit(this);
        }

        private void ButtonKeyFrontOffice_Clicked(object sender, EventArgs e)
        {
            Utils.ShowFrontOffice(this);
        }

        private void ButtonKeyBackOffice_Clicked(object sender, EventArgs e)
        {
            Utils.ShowBackOffice(this);
        }

        private void UserSelected(UserDetail user)
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

        public void LogOutUser(bool pShowStartup)
        {
            LogOutUser(pShowStartup, XPOSettings.LoggedUser);
        }

        public void LogOutUser(bool pGotoStartupWindow, sys_userdetail pUserDetail)
        {
            if (
                POSSession.CurrentSession.LoggedUsers.ContainsKey(pUserDetail.Oid))
            {
                POSSession.CurrentSession.LoggedUsers.Remove(pUserDetail.Oid);
                POSSession.CurrentSession.Save();
                XPOUtility.Audit("USER_logout", string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "audit_message_user_logout"), pUserDetail.Name));
 
                if (XPOSettings.LoggedUser.Equals(pUserDetail))
                {
                    XPOSettings.LoggedUser = null;
                    GeneralSettings.LoggedUserPermissions = null;
                }
            }

            if (pGotoStartupWindow) LogicPOSAppContext.StartupWindow.ShowAll();
        }
    }
}
