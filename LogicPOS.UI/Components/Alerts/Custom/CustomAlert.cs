using Gtk;
using logicpos;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using System.IO;

namespace LogicPOS.UI.Alerts
{
    public class CustomAlert : BaseDialog
    {
        private TextView _txtLog;
        private string _title = LocalizedString.Instance["window_title_dialog_message_dialog"];
        private string _message;
        private DialogFlags _flags = DialogFlags.Modal;
        private MessageType _messageType = MessageType.Info;
        private ButtonsType _buttonsType = ButtonsType.Ok;
        private Window _parentWindow = null;
        private ActionAreaButtons _buttons;
        private System.Drawing.Size _size = new System.Drawing.Size(600, 400);
        private string _icon = AppSettings.Paths.Images + @"Icons\Windows\icon_window_default.png";
        private string _image = string.Empty;

        public CustomAlert(Window parentWindow)
            : base(parentWindow, DialogFlags.Modal)
        {

        }

        public CustomAlert WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public CustomAlert WithTitleResource(string titleResource)
        {
            _title = LocalizedString.Instance[titleResource];
            return this;
        }

        public CustomAlert WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public CustomAlert WithMessageResource(string messageResource)
        {
            _message = LocalizedString.Instance[messageResource];
            return this;
        }


        public CustomAlert WithMessageType(MessageType messageType)
        {
            _messageType = messageType;
            return this;
        }

        public CustomAlert WithButtonsType(ButtonsType buttonsType)
        {
            _buttonsType = buttonsType;
            return this;
        }

        public CustomAlert WithSize(System.Drawing.Size size)
        {
            _size = size;
            return this;
        }

        public ResponseType ShowAlert()
        {
            var alertSettings = new CustomAlertSettings();
            var alertButtons = new CustomAlertButtons(alertSettings);
            _buttons = alertButtons.GetActionAreaButtons(_buttonsType);
            _image = AppSettings.Paths.Images + alertSettings.GetDialogImage(_messageType);
            _icon = AppSettings.Paths.Images + alertSettings.GetDialogIcon(_messageType);

            InitObject();

            HideCloseButton();
            ResponseType responseType = (ResponseType)Run();

            if (responseType != ResponseType.Apply)
            {
                Destroy();
            }

            return responseType;
        }

        private void InitObject()
        {
            System.Drawing.Size sizeTextView = new System.Drawing.Size(_size.Width - 40, _size.Height - 130);
            _txtLog = new TextView()
            {
                BorderWidth = 0,
                CursorVisible = false,
                Editable = false
            };
            _txtLog.SetSizeRequest(sizeTextView.Width, sizeTextView.Height);
            _txtLog.ModifyFont(Pango.FontDescription.FromString("14"));
            _txtLog.SizeAllocated += new SizeAllocatedHandler(ScrollTextViewLog);
            _txtLog.WrapMode = WrapMode.Word;
            _txtLog.Sensitive = false;
            _txtLog.ModifyBase(StateType.Insensitive, System.Drawing.Color.Transparent.ToGdkColor());

            TextBuffer _textBuffer = _txtLog.Buffer;
            _textBuffer.InsertAtCursor(_message);

            ScrolledWindow scrolledWindowTextviewLog = new ScrolledWindow();
            scrolledWindowTextviewLog.SetSizeRequest(sizeTextView.Width, sizeTextView.Height);
            scrolledWindowTextviewLog.Add(_txtLog);
            scrolledWindowTextviewLog.Vadjustment.Value = scrolledWindowTextviewLog.Vadjustment.Upper;

            Fixed body = new Fixed();

            if (_image != string.Empty && File.Exists(_image))
            {
                Gdk.Pixbuf pixBuf = Utils.FileToPixBuf(_image);
                Image imageDialog = new Image(pixBuf);


                scrolledWindowTextviewLog.WidthRequest -= pixBuf.Width;
                body.Put(imageDialog, 10, 10);
                body.Put(scrolledWindowTextviewLog, pixBuf.Width + 25, 10);
            }
            else
            {
                body.Put(scrolledWindowTextviewLog, 0, 0);
            }

            if (_buttons == null)
            {
                IconButtonWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
                IconButtonWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);

                _buttons = new ActionAreaButtons
                {
                    new ActionAreaButton(buttonOk, ResponseType.Ok),
                    new ActionAreaButton(buttonCancel, ResponseType.Cancel)
                };
            }

            Initialize(_parentWindow,
                       _flags,
                       _icon,
                       _title,
                       _size,
                       body,
                      _buttons);
        }


        private void ScrollTextViewLog(object o,
                                       SizeAllocatedArgs args)
        {
            _txtLog.ScrollToIter(_txtLog.Buffer.EndIter, 0, false, 0, 0);
        }
    }
}
