using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;
using System;

namespace logicpos
{
    partial class StartupWindow
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        private void _tablePadUser_Clicked(object sender, EventArgs e)
        {
            TouchButtonBase button = (TouchButtonBase)sender;

            //Assign CurrentId to TablePad.CurrentId, to Know last Clicked Button Id
            _tablePadUser.SelectedButtonOid = button.CurrentButtonOid;

            //Assign User Detail to Member Reference
            AssignUserDetail();
        }

        private void ButtonKeyOK_Clicked(object sender, EventArgs e)
        {
            _numberPadPin.ProcessPassword(this, _selectedUserDetail);
        }

        void ButtonKeyResetPassword_Clicked(object sender, EventArgs e)
        {
            //Require to store current Current Pin, else when we change mode it resets pin to messages
            string currentPin = _numberPadPin.EntryPin.Text;
            _numberPadPin.Mode = NumberPadPinMode.PasswordReset;
            //Restore Pin after UpdateLabelStatus triggered in mode
            _numberPadPin.EntryPin.Text = currentPin;
            _numberPadPin.ProcessPassword(this, _selectedUserDetail);
        }

        //Removed : Conflited with Change Password, When we Implement Default Enter Key in All Dilogs, It Trigger Twice
        void StartupWindow_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            //if (args.Event.Key.ToString().Equals("Return"))
            //{
            //    _numberPadPin.ProcessPassword(this, _selectedUserDetail);
            //}
        }

        void ButtonKeyQuit_Clicked(object sender, EventArgs e)
        {
            LogicPos.Quit(this);
        }

        private void ButtonKeyFrontOffice_Clicked(object sender, EventArgs e)
        {
            Utils.ShowFrontOffice(this);
        }

        private void ButtonKeyBackOffice_Clicked(object sender, EventArgs e)
        {
            Utils.ShowBackOffice(this);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Logic

        //Assign Selected UserDetail to classe Member
        private void AssignUserDetail()
        {
            try 
	        {	        
                if (_tablePadUser.SelectedButtonOid != null) 
                {
                    _selectedUserDetail = (FrameworkUtils.GetXPGuidObject(typeof(sys_userdetail), _tablePadUser.SelectedButtonOid) as sys_userdetail);
                    if (_selectedUserDetail != null)
                    {
                        //Change NumberPadPinMode Mode
                        _numberPadPin.Mode = (_selectedUserDetail.PasswordReset) ? NumberPadPinMode.PasswordOld : NumberPadPinMode.Password;

                        if (_selectedUserDetail.PasswordReset)
                        {
                            //_log.Debug(string.Format("Name: [{0}], PasswordReset: [{1}]", _selectedUserDetail.Name, _selectedUserDetail.PasswordReset));
                            Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"),
                                string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_user_request_change_password"), _selectedUserDetail.Name, SettingsApp.DefaultValueUserDetailAccessPin)
                            );
                        }
                    }
                }

                //Grab Focus
                _numberPadPin.EntryPin.GrabFocus();
	        }
	        catch (Exception ex)
	        {
                _log.Error(ex.Message, ex);
	        }        
        }

        //Main Logout User Method, Shared for FrontOffice and BackOffice
        public void LogOutUser(bool pShowStartup)
        {
            LogOutUser(pShowStartup, GlobalFramework.LoggedUser);
        }

        public void LogOutUser(bool pGotoStartupWindow, sys_userdetail pUserDetail)
        {
            if (
                GlobalFramework.SessionApp.LoggedUsers.ContainsKey(pUserDetail.Oid))
            {
                GlobalFramework.SessionApp.LoggedUsers.Remove(pUserDetail.Oid);
                GlobalFramework.SessionApp.Write();
                FrameworkUtils.Audit("USER_LOGOUT", string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "audit_message_user_logout"), pUserDetail.Name));
                //Only Reset LoggedUser if equal to pUser
                if (GlobalFramework.LoggedUser.Equals(pUserDetail))
                {
                    GlobalFramework.LoggedUser = null;
                    GlobalFramework.LoggedUserPermissions = null;
                }
            }
            //Update Table, In case user change Users in BackOffice
            GlobalApp.WindowStartup.TablePadUser.UpdateSql();
            //Required to Assign Details to Update Select User
            AssignUserDetail();
            //Show Startup Windows, or Not (Silent Mode)
            if (pGotoStartupWindow) GlobalApp.WindowStartup.ShowAll();
        }
    }
}
