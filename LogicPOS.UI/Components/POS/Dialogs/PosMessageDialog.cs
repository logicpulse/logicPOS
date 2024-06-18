using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Extensions;
using System;
using System.IO;
using LogicPOS.Globalization;
using LogicPOS.Settings;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosMessageDialog : PosBaseDialog
    {
        private TextView _textviewLog;

        public PosMessageDialog(Window pSourceWindow, DialogFlags pDialogFlags, string pMessage, string pImageDialog = "")
            : this(pSourceWindow, pDialogFlags, new System.Drawing.Size(700, 500), pMessage, pImageDialog)
        {
        }

        public PosMessageDialog(Window pSourceWindow, DialogFlags pDialogFlags, System.Drawing.Size pSize, string pMessage, string pImageDialog = "")
            : this(pSourceWindow, pDialogFlags, pSize, pMessage, null, pImageDialog)
        {
        }

        public PosMessageDialog(Window pSourceWindow, DialogFlags pDialogFlags, System.Drawing.Size pSize, string pTitle, string pMessage, string pImageWindowIcon, string pImageDialog = "")
            : this(pSourceWindow, pDialogFlags, pSize, pTitle, pMessage, null, pImageWindowIcon, pImageDialog)
        {
        }

        //Shortcut to Dialog with Button with ResponseType and ButtonLabel
        public PosMessageDialog(Window pSourceWindow, DialogFlags pDialogFlags, System.Drawing.Size pSize, string pMessage, MessageType pMessageType, ResponseType pResponseType, string pButtonLabel, string pImageDialog = "", bool pCconfirmDialogOnEnter = true, bool pWindowTitleCloseButton = true)
            : base(pSourceWindow, pDialogFlags)
        {
            string fileImageDialogBaseMessageTypeIcon = GeneralSettings.Settings["fileImageDialogBaseMessageTypeIcon"];
            string fileImagePath = string.Format(fileImageDialogBaseMessageTypeIcon, Enum.GetName(typeof(MessageType), pMessageType).ToLower());

            TouchButtonIconWithText button = new TouchButtonIconWithText("touchButton_DialogActionArea", _colorBaseDialogActionAreaButtonBackground, pButtonLabel, _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, _fileActionOK, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height);

            //Add to ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(button, pResponseType)
            };

            InitObject(pSourceWindow, pDialogFlags, pSize, string.Empty, pMessage, actionAreaButtons, fileImagePath, pImageDialog);
        }

        public PosMessageDialog(Window pSourceWindow, DialogFlags pDialogFlags, System.Drawing.Size pSize, string pMessage, ActionAreaButtons pActionAreaButtons, string pImageDialog = "")
            : base(pSourceWindow, pDialogFlags)
        {
            InitObject(pSourceWindow, pDialogFlags, pSize, string.Empty, pMessage, pActionAreaButtons, string.Empty, pImageDialog);
        }

        public PosMessageDialog(Window pSourceWindow, DialogFlags pDialogFlags, System.Drawing.Size pSize, string pTitle, string pMessage, ActionAreaButtons pActionAreaButtons, string pImageWindowIcon, string pImageDialog = "")
            : base(pSourceWindow, pDialogFlags)
        {
            InitObject(pSourceWindow, pDialogFlags, pSize, pTitle, pMessage, pActionAreaButtons, pImageWindowIcon, pImageDialog);
        }

        private void InitObject(Window pSourceWindow, DialogFlags pDialogFlags, System.Drawing.Size pSize, string pTitle, string pMessage, ActionAreaButtons pActionAreaButtons, string pImageWindowIcon, string pImageDialog = "")
        {
            //Init Local Vars
            string windowTitle = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_message_dialog");
            System.Drawing.Size windowSize = pSize;
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_default.png";

            if (!pTitle.Equals(string.Empty))
            {
                windowTitle = pTitle;
            }

            if (!pImageWindowIcon.Equals(string.Empty))
            {
                fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + pImageWindowIcon;
            }

            //Text View
            System.Drawing.Size sizeTextView = new System.Drawing.Size(pSize.Width - 40, pSize.Height - 130);
            _textviewLog = new TextView()
            {
                BorderWidth = 0,
                CursorVisible = false,
                Editable = false
            };
            _textviewLog.SetSizeRequest(sizeTextView.Width, sizeTextView.Height);
            _textviewLog.ModifyFont(Pango.FontDescription.FromString("14"));
            _textviewLog.SizeAllocated += new SizeAllocatedHandler(ScrollTextViewLog);
            _textviewLog.WrapMode = WrapMode.Word;
            _textviewLog.Sensitive = false;
            //Removed to be Transparent - CHANGE COLOR ex to System.Drawing.Color.Aqua to View TextView to Position
            _textviewLog.ModifyBase(StateType.Insensitive, System.Drawing.Color.Transparent.ToGdkColor());


            TextBuffer _textBuffer = _textviewLog.Buffer;
            _textBuffer.InsertAtCursor(pMessage);

            ScrolledWindow scrolledWindowTextviewLog = new ScrolledWindow();
            //scrolledWindowTextviewLog.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrolledWindowTextviewLog.SetSizeRequest(sizeTextView.Width, sizeTextView.Height);
            scrolledWindowTextviewLog.Add(_textviewLog);
            scrolledWindowTextviewLog.Vadjustment.Value = scrolledWindowTextviewLog.Vadjustment.Upper;


            //Init Content
            Fixed fixedContent = new Fixed();

            //Add content, with or without ImageDialog
            string fileImageDialog = PathsSettings.ImagesFolderLocation + pImageDialog;
            if (pImageDialog != string.Empty && File.Exists(fileImageDialog))
            {
                Gdk.Pixbuf pixBuf = logicpos.Utils.FileToPixBuf(fileImageDialog);
                Image imageDialog = new Image(pixBuf);


                scrolledWindowTextviewLog.WidthRequest -= pixBuf.Width;
                fixedContent.Put(imageDialog, 10, 10);
                fixedContent.Put(scrolledWindowTextviewLog, pixBuf.Width + 25, 10);
            }
            else
            {
                fixedContent.Put(scrolledWindowTextviewLog, 0, 0);
            }

            ActionAreaButtons actionAreaButtons;

            //Default ActionArea Buttons if Not Defined
            if (pActionAreaButtons == null)
            {
                //ActionArea Buttons
                TouchButtonIconWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
                TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
                //Add to ActionArea
                actionAreaButtons = new ActionAreaButtons
                {
                    new ActionAreaButton(buttonOk, ResponseType.Ok),
                    new ActionAreaButton(buttonCancel, ResponseType.Cancel)
                };
            }
            else
            {
                //Use ActionAreaButtons from Parameters
                actionAreaButtons = pActionAreaButtons;
            }

            //Detect Autosize
            //if (windowSize.Height == 0 )
            //{
            //  int maxSize = 700;
            //  int targetSize = _textviewLog.HeightRequest + 125;
            //  windowSize.Height = (targetSize < maxSize) ? targetSize : maxSize;
            //  _logger.Debug(string.Format("Message: [{0}] [{1}] [{2}] [{3}]", _textviewLog.HeightRequest, targetSize, pSize.Height, windowSize.Height));
            //}

            //Init Object
            this.InitObject(pSourceWindow/*this*/, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, fixedContent, actionAreaButtons);
        }

        // Automatically scroll to bottom of _textviewLog
        private void ScrollTextViewLog(object o, SizeAllocatedArgs args)
        {
            _textviewLog.ScrollToIter(_textviewLog.Buffer.EndIter, 0, false, 0, 0);
        }
    }
}
