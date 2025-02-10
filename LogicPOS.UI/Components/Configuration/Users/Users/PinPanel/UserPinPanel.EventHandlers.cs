using Gtk;
using logicpos;
using logicpos.Classes.Enums.Widgets;
using LogicPOS.UI.Buttons;
using System;

namespace LogicPOS.UI.Components
{
    public partial class UserPinPanel
    {
        private void AddEventHandlers()
        {
            TxtPin.Changed += TxtPin_Changed;
            TxtPin.KeyReleaseEvent += TxtPin_KeyReleaseEvent;
        }

        private void TxtPin_Changed(object sender, EventArgs e)
        {
            ValidatableTextBox entry = (ValidatableTextBox)sender;
            ClearEntryPinStatusMessage();
            BtnOk.Sensitive = entry.Validated;
            BtnResetPassword.Sensitive = entry.Validated && _mode == NumberPadPinMode.Password;
        }

        private void BtnKey_Clicked(object sender, EventArgs e)
        {
            TextButton button = (TextButton)sender;
            ClearEntryPinStatusMessage();
            TxtPin.InsertText(button.ButtonLabel.Text, ref _tempCursorPosition);
            TxtPin.GrabFocus();
            TxtPin.Position = TxtPin.Text.Length;
        }

        private void BtnCE_Clicked(object sender, EventArgs e)
        {
            ClearEntryPinStatusMessage();
            TxtPin.DeleteText(TxtPin.Text.Length - 1, TxtPin.Text.Length);
            TxtPin.GrabFocus();
            TxtPin.Position = TxtPin.Text.Length;
        }

        private void TxtPin_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key.ToString().Equals("Return"))
            {
                if (TxtPin.Validated)
                {
                    BtnOk.Click();
                }
            }
        }
    }
}
