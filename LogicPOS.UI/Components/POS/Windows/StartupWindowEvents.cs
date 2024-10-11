using Gtk;
using logicpos.Classes.Enums.Widgets;
using System;
using LogicPOS.Globalization;
using LogicPOS.Shared;
using LogicPOS.Settings;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Buttons;
using logicpos;

namespace LogicPOS.UI.Components.Windows
{
    public partial class StartupWindow
    {
        private void TablePadUser_Clicked(object sender, EventArgs e)
        {
            CustomButton button = (CustomButton)sender;

            UsersPanel.SelectedButtonOid = button.CurrentButtonId;

            //Assign User Detail to Member Reference
            AssignUserDetail();
        }

        private void ButtonKeyOK_Clicked(object sender, EventArgs e)
        {
            _userPinPanel.ProcessPassword(this, _selectedLoginUser);
        }

        private void ButtonKeyResetPassword_Clicked(object sender, EventArgs e)
        {
            //Require to store current Current Pin, else when we change mode it resets pin to messages
            string currentPin = _userPinPanel.EntryPin.Text;
            _userPinPanel.Mode = NumberPadPinMode.PasswordReset;
            //Restore Pin after UpdateLabelStatus triggered in mode
            _userPinPanel.EntryPin.Text = currentPin;
            _userPinPanel.ProcessPassword(this, _selectedLoginUser);
        }

        private void StartupWindow_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            //if (args.Event.Key.ToString().Equals("Return"))
            //{
            //    _numberPadPin.ProcessPassword(this, _selectedUserDetail);
            //}
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

        private void AssignUserDetail()
        {
            try
            {
                if (UsersPanel.SelectedButtonOid != null)
                {
                    _selectedLoginUser = XPOUtility.GetEntityById<sys_userdetail>(UsersPanel.SelectedButtonOid);
                    if (_selectedLoginUser != null)
                    {
                        //Change NumberPadPinMode Mode
                        _userPinPanel.Mode = (_selectedLoginUser.PasswordReset) ? NumberPadPinMode.PasswordOld : NumberPadPinMode.Password;

                        if (_selectedLoginUser.PasswordReset)
                        {
                            //_logger.Debug(string.Format("Name: [{0}], PasswordReset: [{1}]", _selectedUserDetail.Name, _selectedUserDetail.PasswordReset));
                            Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_information"),
                                string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_user_request_change_password"), _selectedLoginUser.Name, XPOSettings.DefaultValueUserDetailAccessPin)
                            );
                        }
                    }
                }

                //Grab Focus
                _userPinPanel.EntryPin.GrabFocus();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
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
                //Only Reset LoggedUser if equal to pUser
                if (XPOSettings.LoggedUser.Equals(pUserDetail))
                {
                    XPOSettings.LoggedUser = null;
                    GeneralSettings.LoggedUserPermissions = null;
                }
            }
            //Update Table, In case user change Users in BackOffice
            GlobalApp.StartupWindow.UsersPanel.UpdateSql();
            //Required to Assign Details to Update Select User
            AssignUserDetail();
            //Show Startup Windows, or Not (Silent Mode)
            if (pGotoStartupWindow) GlobalApp.StartupWindow.ShowAll();
        }
    }
}
