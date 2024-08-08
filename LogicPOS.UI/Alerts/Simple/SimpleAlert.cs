using Gtk;
using LogicPOS.Utility;

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

        public ResponseType Show()
        {
            MessageDialog dialog = new MessageDialog(_parentWindow,
                                                     _flags,
                                                     _messageType,
                                                     _buttonsType,
                                                     _message);
            dialog.Title = _title;
            ResponseType responseType = (ResponseType)dialog.Run();
            dialog.Destroy();

            return responseType;
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
            _title = GeneralUtils.GetResourceByName(titleResource);
            return this;
        }

        public SimpleAlert WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public SimpleAlert WithMessageResource(string messageResource)
        {
            _message = GeneralUtils.GetResourceByName(messageResource);
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
