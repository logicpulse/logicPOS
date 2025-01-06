using Gtk;
using logicpos.Classes.Enums.Widgets;
using LogicPOS.Api.Entities;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Windows
{
    public partial class LoginWindow
    {
        private void BtnOK_Clicked(object sender, EventArgs e)
        {
            if (MenuUsers.SelectedEntity == null)
            {
                CustomAlerts.Warning(this)
                            .WithSize(new Size(500, 340))
                            .WithTitleResource("global_warning")
                            .WithMessage("Usuário não selecionado!")
                            .ShowAlert();
                return;
            }

            PinPanel.ProcessPassword(MenuUsers.SelectedEntity);
        }

        private void BtnResetPassword_Clicked(object sender, EventArgs e)
        {
            string currentPin = PinPanel.TxtPin.Text;
            PinPanel.Mode = NumberPadPinMode.PasswordReset;
            PinPanel.TxtPin.Text = currentPin;
            PinPanel.ProcessPassword(MenuUsers.SelectedEntity);
        }

        private void Window_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key.ToString().Equals("Return"))
            {
                PinPanel.ProcessPassword(MenuUsers.SelectedEntity);
            }
        }

        private void ButtonKeyQuit_Clicked(object sender, EventArgs e)
        {
            LogicPOSAppUtils.Quit(this);
        }

        private void ButtonKeyFrontOffice_Clicked(object sender, EventArgs e)
        {
            Hide();
            POSWindow.ShowPOS();
        }

        private void OnUserSelected(User user)
        {
            PinPanel.Mode = (user.PasswordReset) ? NumberPadPinMode.PasswordOld : NumberPadPinMode.Password;

            if (user.PasswordReset)
            {
                CustomAlerts.Information(this)
                            .WithSize(new Size(500, 340))
                            .WithTitleResource("global_information")
                            .WithMessage(string.Format(GeneralUtils.GetResourceByName("dialog_message_user_request_change_password"), user.Name, XPOSettings.DefaultValueUserDetailAccessPin))
                            .ShowAlert();
            }

            PinPanel.TxtPin.GrabFocus();
        }

        private void LoginWindow_Shown(object sender, EventArgs e)
        {
            MenuUsers.Refresh();
        }
    }
}
