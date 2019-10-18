using Gtk;
using logicpos.App;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using System.IO;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    class PosMessageDialog : PosBaseDialog
    {
        TextView _textviewLog;

        public PosMessageDialog(Window pSourceWindow, DialogFlags pDialogFlags, String pMessage, String pImageDialog = "")
            : this(pSourceWindow, pDialogFlags, new System.Drawing.Size(700, 500), pMessage, pImageDialog)
        {
        }

        public PosMessageDialog(Window pSourceWindow, DialogFlags pDialogFlags, System.Drawing.Size pSize, String pMessage, String pImageDialog = "")
            : this(pSourceWindow, pDialogFlags, pSize, pMessage, null, pImageDialog)
        {
        }

        public PosMessageDialog(Window pSourceWindow, DialogFlags pDialogFlags, System.Drawing.Size pSize, String pTitle, String pMessage, String pImageWindowIcon, String pImageDialog = "")
            : this(pSourceWindow, pDialogFlags, pSize, pTitle, pMessage, null, pImageWindowIcon, pImageDialog)
        {
        }

        //Shortcut to Dialog with Button with ResponseType and ButtonLabel
        public PosMessageDialog(Window pSourceWindow, DialogFlags pDialogFlags, System.Drawing.Size pSize, String pMessage, MessageType pMessageType, ResponseType pResponseType, String pButtonLabel, String pImageDialog = "", bool pCconfirmDialogOnEnter = true, bool pWindowTitleCloseButton = true)
            : base(pSourceWindow, pDialogFlags)
        {
            String fileImageDialogBaseMessageTypeIcon = FrameworkUtils.OSSlash(GlobalFramework.Settings["fileImageDialogBaseMessageTypeIcon"]);
            String fileImagePath = string.Format(fileImageDialogBaseMessageTypeIcon, Enum.GetName(typeof(MessageType), pMessageType).ToLower());

            TouchButtonIconWithText button = new TouchButtonIconWithText("touchButton_DialogActionArea", _colorBaseDialogActionAreaButtonBackground, pButtonLabel, _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, _fileActionOK, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height);

            //Add to ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(button, pResponseType));

            InitObject(pSourceWindow, pDialogFlags, pSize, string.Empty, pMessage, actionAreaButtons, fileImagePath, pImageDialog);
        }

        public PosMessageDialog(Window pSourceWindow, DialogFlags pDialogFlags, System.Drawing.Size pSize, String pMessage, ActionAreaButtons pActionAreaButtons, String pImageDialog = "")
            : base(pSourceWindow, pDialogFlags)
        {
            InitObject(pSourceWindow, pDialogFlags, pSize, string.Empty, pMessage, pActionAreaButtons, string.Empty, pImageDialog);
        }

        public PosMessageDialog(Window pSourceWindow, DialogFlags pDialogFlags, System.Drawing.Size pSize, String pTitle, String pMessage, ActionAreaButtons pActionAreaButtons, String pImageWindowIcon, String pImageDialog = "")
            : base(pSourceWindow, pDialogFlags)
        {
            InitObject(pSourceWindow, pDialogFlags, pSize, pTitle, pMessage, pActionAreaButtons, pImageWindowIcon, pImageDialog);
        }

        private void InitObject(Window pSourceWindow, DialogFlags pDialogFlags, System.Drawing.Size pSize, String pTitle, String pMessage, ActionAreaButtons pActionAreaButtons, String pImageWindowIcon, String pImageDialog = "")
        {
            //Init Local Vars
            String windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_message_dialog");
            System.Drawing.Size windowSize = pSize;
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_default.png");

            if (!pTitle.Equals(string.Empty))
            {
                windowTitle = pTitle;
            }

            if (!pImageWindowIcon.Equals(string.Empty))
            {
                fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + pImageWindowIcon);
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
            _textviewLog.ModifyBase(StateType.Insensitive, Utils.ColorToGdkColor(System.Drawing.Color.Transparent));

            TextBuffer _textBuffer = _textviewLog.Buffer;
            _textBuffer.InsertAtCursor(pMessage);

            ScrolledWindow scrolledWindowTextviewLog = new ScrolledWindow();
            scrolledWindowTextviewLog.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrolledWindowTextviewLog.SetSizeRequest(sizeTextView.Width, sizeTextView.Height);
            scrolledWindowTextviewLog.Add(_textviewLog);

            //Init Content
            Fixed fixedContent = new Fixed();

            //Add content, with or without ImageDialog
            String fileImageDialog = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + pImageDialog);
            if (pImageDialog != string.Empty && File.Exists(fileImageDialog))
            {
                Gdk.Pixbuf pixBuf = Utils.FileToPixBuf(fileImageDialog);
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
                actionAreaButtons = new ActionAreaButtons();
                actionAreaButtons.Add(new ActionAreaButton(buttonOk, ResponseType.Ok));
                actionAreaButtons.Add(new ActionAreaButton(buttonCancel, ResponseType.Cancel));
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
            //  _log.Debug(string.Format("Message: [{0}] [{1}] [{2}] [{3}]", _textviewLog.HeightRequest, targetSize, pSize.Height, windowSize.Height));
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
