using Gtk;
using System.IO;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Extensions;
using logicpos;
using LogicPOS.UI.Buttons;

namespace LogicPOS.UI.Dialogs
{
    internal class CustomAlert : BaseDialog
    {
        private TextView _txtLog;

        public CustomAlert(Window parentWindow,
                           DialogFlags pDialogFlags,
                           System.Drawing.Size pSize,
                           string pTitle,
                           string pMessage,
                           ActionAreaButtons pActionAreaButtons,
                           string pImageWindowIcon,
                           string pImageDialog = "")
            : base(parentWindow, pDialogFlags)
        {
            InitObject(parentWindow,
                       pDialogFlags,
                       pSize,
                       pTitle,
                       pMessage,
                       pActionAreaButtons,
                       pImageWindowIcon,
                       pImageDialog);
        }

        private void InitObject(Window parentWindow,
                                DialogFlags pDialogFlags,
                                System.Drawing.Size pSize,
                                string pTitle,
                                string pMessage,
                                ActionAreaButtons pActionAreaButtons,
                                string pImageWindowIcon,
                                string pImageDialog = "")
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
            //Removed to be Transparent - CHANGE COLOR ex to System.Drawing.Color.Aqua to View TextView to Position
            _txtLog.ModifyBase(StateType.Insensitive, System.Drawing.Color.Transparent.ToGdkColor());


            TextBuffer _textBuffer = _txtLog.Buffer;
            _textBuffer.InsertAtCursor(pMessage);

            ScrolledWindow scrolledWindowTextviewLog = new ScrolledWindow();
            //scrolledWindowTextviewLog.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrolledWindowTextviewLog.SetSizeRequest(sizeTextView.Width, sizeTextView.Height);
            scrolledWindowTextviewLog.Add(_txtLog);
            scrolledWindowTextviewLog.Vadjustment.Value = scrolledWindowTextviewLog.Vadjustment.Upper;


            //Init Content
            Fixed fixedContent = new Fixed();

            //Add content, with or without ImageDialog
            string fileImageDialog = PathsSettings.ImagesFolderLocation + pImageDialog;
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
                IconButtonWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
                IconButtonWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
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
            Initialize(parentWindow/*this*/, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, fixedContent, actionAreaButtons);
        }

 
        private void ScrollTextViewLog(object o,
                                       SizeAllocatedArgs args)
        {
            _txtLog.ScrollToIter(_txtLog.Buffer.EndIter, 0,false, 0,0);
        }
    }
}
