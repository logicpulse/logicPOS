using Gtk;
using LogicPOS.Utility;
using LogicPOS.Globalization;
using LogicPOS.UI.Extensions;using WinFormsMessageBox = System.Windows.Forms.MessageBox;
using WinFormsMessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
using WinFormsMessageBoxIcon = System.Windows.Forms.MessageBoxIcon;

namespace LogicPOS.UI.Alerts
{
    public class SimpleAlert
    {
        private string _title = "LogicPOS";
        private string _message;
        private DialogFlags _flags = DialogFlags.Modal;
        private MessageType _messageType = MessageType.Info;
        private ButtonsType _buttonsType = ButtonsType.Ok;
        private Window _parentWindow = null;

        public ResponseType ShowAlert()
        {
            if (!CanUseGtkDialog())
            {
                ShowWinFormsFallback();
                return ResponseType.Ok;
            }

            var parentWindow = CustomAlerts.ResolveParentWindow(_parentWindow);

            using (var dialog = new MessageDialog(
                parentWindow,
                _flags,
                _messageType,
                _buttonsType,
                _message))
            {
                dialog.Title = _title;
                dialog.Modal = true;
                if (parentWindow != null)
                {
                    dialog.TransientFor = parentWindow;
                }

                dialog.Present();
                return (ResponseType)dialog.RunWithDisabledParent(parentWindow);
            }
        }

        private static bool CanUseGtkDialog()
        {
            try
            {
                return Gdk.Screen.Default != null;
            }
            catch
            {
                return false;
            }
        }

        private void ShowWinFormsFallback()
        {
            WinFormsMessageBox.Show(
                _message ?? string.Empty,
                _title ?? "LogicPOS",
                WinFormsMessageBoxButtons.OK,
                MapMessageType(_messageType));
        }

        private static WinFormsMessageBoxIcon MapMessageType(MessageType type)
        {
            switch (type)
            {
                case MessageType.Error:
                    return WinFormsMessageBoxIcon.Error;
                case MessageType.Warning:
                    return WinFormsMessageBoxIcon.Warning;
                case MessageType.Question:
                    return WinFormsMessageBoxIcon.Question;
                default:
                    return WinFormsMessageBoxIcon.Information;
            }
        }

        public SimpleAlert WithParent(Window parentWindow)
        {
            _parentWindow = parentWindow;
            return this;
        }

        public SimpleAlert WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public SimpleAlert WithTitleResource(string titleResource)
        {
            _title = LocalizedString.Instance[titleResource];
            return this;
        }

        public SimpleAlert WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public SimpleAlert WithMessageResource(string messageResource)
        {
            _message = LocalizedString.Instance[messageResource];
            return this;
        }

        public SimpleAlert WithFlag(DialogFlags flag)
        {
            _flags = flag;
            return this;
        }

        public SimpleAlert WithMessageType(MessageType type)
        {
            _messageType = type;
            return this;
        }

        public SimpleAlert WithButton(ButtonsType type)
        {
            _buttonsType = type;
            return this;
        }
    }
}
