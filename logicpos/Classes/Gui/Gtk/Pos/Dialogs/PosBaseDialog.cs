using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    // Dialog
    public abstract class PosBaseDialog : Dialog
    {
        //Log4Net
        protected log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //Protected
        /* IN008024 */
        //protected string _appOperationModeToken = GlobalFramework.Settings["appOperationModeToken"];
        protected string _windowTitle;
        protected string _windowIcon;
        protected System.Drawing.Size _windowSize;
        protected EventBox _eventboxCloseWindow;
        protected EventBox _eventboxMinimizeWindow;
        private Widget _content;
        private Widget _actionAreaLeftContent;
        //Used to detect Confirmation Button for Enter Key
        private TouchButtonBase _actionAreaConfirmButton;
        private ActionAreaButtons _actionAreaRightButtons;
        private uint _borderWidth = 5;
        //private Gtk.Style _styleBackground;
        //Assets
        protected String _fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_default.png");
        private String _fileDefaultWindowIconClose = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_window_close.png");
        private String _fileDefaultWindowIconMinimize = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_window_minimize.png");
        //Colors
        private System.Drawing.Color _colorBaseDialogTitleBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogTitleBackground"]);
        private System.Drawing.Color _colorBaseDialogWindowBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogWindowBackground"]);
        private System.Drawing.Color _colorBaseDialogWindowBackgroundBorder = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogWindowBackgroundBorder"]);
        private Boolean _useBaseDialogWindowMask = Convert.ToBoolean(GlobalFramework.Settings["useBaseDialogWindowMask"]);
        //Protected Members (Shared for Child Dialogs)
        protected int _dragOffsetX, _dragOffsetY;
        protected Label _labelWindowTitle;        
        public System.Drawing.Size _sizeBasePaymentButton = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogDefaultButton"]);
        public System.Drawing.Size _sizeBasePaymentButtonIcon = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogDefaultButtonIcon"]);
        public System.Drawing.Size _sizeBaseDialogDefaultButton = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogDefaultButton"]);
        public System.Drawing.Size _sizeBaseDialogDefaultButtonIcon = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogDefaultButtonIcon"]);

        public System.Drawing.Size _sizeBaseDialogActionAreaButton = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogActionAreaButton"]);
        public System.Drawing.Size _sizeBaseDialogActionAreaButtonIcon = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogActionAreaButtonIcon"]);
        //IN009257 Ends
        protected System.Drawing.Color _colorBaseDialogDefaultButtonFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogDefaultButtonFont"]);
        protected System.Drawing.Color _colorBaseDialogDefaultButtonBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogDefaultButtonBackground"]);
        protected System.Drawing.Color _colorBaseDialogActionAreaButtonFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogActionAreaButtonFont"]);
        protected System.Drawing.Color _colorBaseDialogActionAreaButtonBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogActionAreaButtonBackground"]);
        protected String _fontBaseDialogButton = FrameworkUtils.OSSlash(GlobalFramework.Settings["fontBaseDialogButton"]);
        protected String _fontBaseDialogActionAreaButton = FrameworkUtils.OSSlash(GlobalFramework.Settings["fontBaseDialogActionAreaButton"]);
        protected String _fileActionDefault = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_default.png");
        protected String _fileActionOK = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_ok.png");
        protected String _fileActionCancel = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png");
        protected String _fileDemoData = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_demo.png");
        //IN009223 IN009227
		protected String _fileActionMore = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_nav_new.png");

        //Public Properties
        protected Window _sourceWindow;
        public Window SourceWindow
        {
            get { return (_sourceWindow); }
            set { _sourceWindow = value; TransientFor = value; }
        }
        protected Window _windowMaskBackground;
        public Window WindowMaskBackground
        {
            get { return _windowMaskBackground; }
            set { _windowMaskBackground = value; }
        }
        public String WindowTitle
        {
            get { return (_labelWindowTitle.Text); }
            set { _labelWindowTitle.Text = value; }
        }
        protected bool _confirmDialogOnEnter = true;
        public bool ConfirmDialogOnEnter
        {
            get { return _confirmDialogOnEnter; }
            set { _confirmDialogOnEnter = value; }
        }
        protected bool _windowTitleCloseButton = true;
        public bool WindowTitleCloseButton
        {
            get { return _windowTitleCloseButton; }
            set
            {
                _windowTitleCloseButton = value;
                if (value) { _eventboxCloseWindow.ShowAll(); } else { _eventboxCloseWindow.HideAll(); }
            }
        }
        public PosBaseDialog() { }
        //Constructor
        public PosBaseDialog(Window pSourceWindow, DialogFlags pFlags, bool pCconfirmDialogOnEnter = true, bool pWindowTitleCloseButton = true)
            : base("Base Dialog Window", pSourceWindow, pFlags)
        {
            //Parameters
            _confirmDialogOnEnter = pCconfirmDialogOnEnter;
            _windowTitleCloseButton = pWindowTitleCloseButton;

            //Init Window Black Mask
            if (_useBaseDialogWindowMask /*&& ! Debugger.IsAttached*/)
            {
                //Window Mask Background Hack
                _windowMaskBackground = new Window("");
                _windowMaskBackground.TransientFor = pSourceWindow;
                _windowMaskBackground.SetSizeRequest(10, 10);
                _windowMaskBackground.Move(-100, -100);
                _windowMaskBackground.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.Black));
                //Prevent click outside Dialog
                _windowMaskBackground.Opacity = 0.35F;//0.55F | 0.75F
                _windowMaskBackground.CanFocus = false;
                _windowMaskBackground.AcceptFocus = false;
                _windowMaskBackground.Sensitive = false;
                _windowMaskBackground.Fullscreen();
                _windowMaskBackground.Show();

                TransientFor = _windowMaskBackground;
                Destroyed += delegate { _windowMaskBackground.Destroy(); };
            }
            else
            {
                //Check if is not null, before create startup window, ex xpo create scheme
                if (pSourceWindow != null)
                {
                    TransientFor = pSourceWindow;

                    //Prevent click outside Dialog
                    pSourceWindow.CanFocus = false;
                    pSourceWindow.AcceptFocus = false;
                    Destroyed += delegate
                    {
                        pSourceWindow.CanFocus = true;
                        pSourceWindow.AcceptFocus = true;
                    };
                }
            }

            //Theme Background
            //if (pFileImageBackground != string.Empty)
            //{
            //  _styleBackground = Utils.GetThemeStyleBackground(pFileImageBackground);
            //}
            //else
            //{
            //  _styleBackground = Utils.GetThemeStyleBackground(FrameworkUtils.OSSlash(GlobalFramework.Settings["fileImageBackgroundDialogDefault"]));
            //}
        }

        public void InitObject(Window pSourceWindow, DialogFlags pDialogFlags, String pIcon, String pTitle, System.Drawing.Size pSize, Widget pContent, ActionAreaButtons pActionAreaRightButtons)
        {
            InitObject(pSourceWindow, pDialogFlags, pIcon, pTitle, pSize, pContent, null, pActionAreaRightButtons);
        }

        public void InitObject(Window pSourceWindow, DialogFlags pDialogFlags, String pIcon, String pTitle, System.Drawing.Size pSize, Widget pContent, Widget pActionAreaLeftContent, ActionAreaButtons pActionAreaRightButtons)
        {
            //parameters
            _sourceWindow = pSourceWindow;
            _windowSize = pSize;
            _windowTitle = pTitle;
            _windowIcon = pIcon;
            _content = pContent;
            _actionAreaLeftContent = pActionAreaLeftContent;
            _actionAreaRightButtons = pActionAreaRightButtons;
            Modal = false;//require else cant drag and drop on linux
            Decorated = false;
            Resizable = false;
            WindowPosition = WindowPosition.Center;
            SetSizeRequest(pSize.Width, pSize.Height);
            DefaultResponse = ResponseType.Cancel;
            ModifyBg(StateType.Normal, Utils.ColorToGdkColor(_colorBaseDialogWindowBackgroundBorder));
            TransientFor = _sourceWindow;
            //ActionArea
            //ActionArea.HeightRequest = _buttonSize.Height + 10 + (int) (BorderWidth * 2);

            //Events
            KeyReleaseEvent += PosBaseDialog_KeyReleaseEvent;

            Build();
        }

        void PosBaseDialog_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (
                _confirmDialogOnEnter
                && args.Event.Key.ToString().Equals("Return")
                && _actionAreaConfirmButton != null
                && _actionAreaConfirmButton.Sensitive
            )
                _actionAreaConfirmButton.Click();
        }

        protected void Build()
        {
            //Prepare default icon
            Gdk.Pixbuf pixbufWindowIcon = new Gdk.Pixbuf(_fileDefaultWindowIcon);

            if (_windowIcon != string.Empty && File.Exists(_windowIcon))
            {
                try
                {
                    pixbufWindowIcon = new Gdk.Pixbuf(_windowIcon);
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Build(): Error load WindowIcon from file [{0}] : {1}", _windowIcon, ex.Message), ex);
                }
            }
            else
            {
                _log.Debug(string.Format("Build(): File not found [{0}]", _windowIcon));
            }

            //Title
            EventBox eventboxWindowTitle = new EventBox();
            eventboxWindowTitle.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(_colorBaseDialogTitleBackground));
            eventboxWindowTitle.HeightRequest = 40;
            //WindowIcon
            Image imageWindowsIcon = new Image(pixbufWindowIcon);
            _labelWindowTitle = new Label(_windowTitle);
            Pango.FontDescription fontDescription = new Pango.FontDescription();
            fontDescription.Weight = Pango.Weight.Bold;
            if (FrameworkUtils.OSVersion() != "unix") fontDescription.Size = 18; //qnd ligado, nao aparece no ubuntu!
            _labelWindowTitle.SetAlignment(0, 0.5F);
            _labelWindowTitle.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
            _labelWindowTitle.ModifyFont(fontDescription);
            //HBox TitleBar - Container for Title and Icon
            HBox hboxWindowTitleBar = new HBox(false, 0);
            hboxWindowTitleBar.PackStart(imageWindowsIcon, false, false, 2);
            hboxWindowTitleBar.PackStart(_labelWindowTitle, true, true, 2);
            eventboxWindowTitle.Add(hboxWindowTitleBar);

           

            //Add Minimize TitleBar Icon
            Gdk.Pixbuf pixbufIconWindowMinimize = new Gdk.Pixbuf(_fileDefaultWindowIconMinimize);
            Image gtkimageIconWindowMinimize = new Image(pixbufIconWindowMinimize);
            _eventboxMinimizeWindow = new EventBox();
            _eventboxMinimizeWindow.WidthRequest = pixbufIconWindowMinimize.Width;
            _eventboxMinimizeWindow.Add(gtkimageIconWindowMinimize);
            _eventboxMinimizeWindow.VisibleWindow = false;
            //if (_windowTitleCloseButton) hboxWindowTitleBar.PackStart(_eventboxMinimizeWindow, false, false, 2);

            //Add Close TitleBar Icon
            Gdk.Pixbuf pixbufIconWindowClose = new Gdk.Pixbuf(_fileDefaultWindowIconClose);
            Image gtkimageIconWindowClose = new Image(pixbufIconWindowClose);
            _eventboxCloseWindow = new EventBox();
            _eventboxCloseWindow.WidthRequest = pixbufIconWindowClose.Width;
            _eventboxCloseWindow.Add(gtkimageIconWindowClose);
            _eventboxCloseWindow.VisibleWindow = false;
            if (_windowTitleCloseButton) hboxWindowTitleBar.PackStart(_eventboxCloseWindow, false, false, 2);

            //ActionArea Buttons Default Buttons
            //TouchButtonIconWithText buttonClose = new TouchButtonIconWithText("touchButtonClose_DialogActionArea", Color.Transparent, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_close, _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, _fileActionClose, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height);
            //AddActionWidget(buttonClose, ResponseType.Close);

            //Dont Destroy Keyboard - Keep it in Memory
            _eventboxCloseWindow.ButtonPressEvent += delegate
            {
                if (this is PosKeyboardDialog)
                {
                    if (_useBaseDialogWindowMask) _windowMaskBackground.Hide();
                    Respond(ResponseType.Close);
                }
                else
                {
                    Destroy();
                }
            };

            //Minimize Window
            _eventboxMinimizeWindow.ButtonReleaseEvent += delegate
            {
                
                Respond(ResponseType.None);
            };

            //Window Content - Box for Content to Add Border Arround Content Widget
            EventBox eventboxWindowContent = new EventBox();
            eventboxWindowContent.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(_colorBaseDialogWindowBackground));
            eventboxWindowContent.Add(_content);

            //Prepare ActionAreaButtons
            HBox hboxActionAreaRightButtons = new HBox() { HeightRequest = 60 };
            //Right
            if (_actionAreaRightButtons != null && _actionAreaRightButtons.Count > 0)
            {
                hboxActionAreaRightButtons = GetActionAreaButtonsHbox(_actionAreaRightButtons, TextDirection.Rtl);
            };

            //Pack Navigator Box with Left and Right Content
            HBox hboxActionArea = new HBox(false, 0);
            //Left
            if (_actionAreaLeftContent != null)
            {
                hboxActionArea.PackStart(_actionAreaLeftContent, false, false, 0);
            }
            //MiddleSpace
            hboxActionArea.PackStart(new HBox(), true, true, 0);
            //Right
            if (_actionAreaRightButtons != null && _actionAreaRightButtons.Count > 0)
            {
                hboxActionArea.PackStart(hboxActionAreaRightButtons, false, false, 0);
            }

            //Final Dialog Pack
            VBox vboxWindow = new VBox(false, 0) { BorderWidth = _borderWidth };
            vboxWindow.PackStart(eventboxWindowTitle, false, false, 0);
            vboxWindow.PackStart(eventboxWindowContent, true, true, 0);
            vboxWindow.PackStart(hboxActionArea, false, false, 0);

            //Window Content - Inner
            EventBox eventboxWindowBorderInner = new EventBox() { BorderWidth = 0 };
            eventboxWindowBorderInner.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(_colorBaseDialogWindowBackground));
            //eventboxWindowBorderInner.Style = _styleBackground;
            eventboxWindowBorderInner.Add(vboxWindow);

            //Window Border - Outer
            EventBox eventboxWindowBorderOuter = new EventBox();
            eventboxWindowBorderOuter.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(_colorBaseDialogWindowBackgroundBorder));
            eventboxWindowBorderOuter.Add(eventboxWindowBorderInner);

            //Finish Pack
            VBox.PackStart(eventboxWindowBorderOuter, true, true, 0);

            //Drag and Drop
            eventboxWindowTitle.ButtonPressEvent += WindowStartDrag;
            eventboxWindowTitle.ButtonReleaseEvent += WindowEndDrag;
            eventboxWindowTitle.MotionNotifyEvent += WindowMotionDrag;

            //Prevent Show All Widgets, This way we can have Hidden ActionArea Buttons
            ShowAll();

            //Hide ActionArea after ShowAll, even if is empty to remove extra pixels
            ActionArea.Visible = false;
        }

        private HBox GetActionAreaButtonsHbox(ActionAreaButtons pActionAreaRightButtons, TextDirection pTextDirection)
        {
            int pos;
            HBox hboxActionArea = new HBox(false, 5) { Direction = pTextDirection };
            for (int i = 0; i < pActionAreaRightButtons.Count; i++)
            {
                //Reverse Index
                pos = pActionAreaRightButtons.Count - 1 - i;
                //pActionAreaRightButtons[pos].Button.SetSizeRequest(_sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height);
                pActionAreaRightButtons[pos].Clicked += Button_Clicked;
                hboxActionArea.PackStart(pActionAreaRightButtons[pos].Button, false, false, 0);

                //Detect Confirmation Button and Assign it to actionAreaConfirmButton, used in KeyReleaseEvent
                if (_actionAreaConfirmButton == null)
                {
                    switch (pActionAreaRightButtons[pos].Response)
                    {
                        case ResponseType.Accept:
                        case ResponseType.Apply:
                        case ResponseType.Ok:
                        case ResponseType.Yes:
                        case ResponseType.Close:

                            _actionAreaConfirmButton = pActionAreaRightButtons[pos].Button;
                            break;
                    }
                }
            }
            return hboxActionArea;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        void Button_Clicked(object sender, EventArgs e)
        {
            ActionAreaButton button = (ActionAreaButton)sender;
            Respond(button.Response);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Drag&Drop

        protected void WindowStartDrag(object o, ButtonPressEventArgs args)
        {
            int windowX, windowY, mouseX, mouseY;
            Gdk.Display.Default.GetPointer(out mouseX, out mouseY);
            GetPosition(out windowX, out windowY);
            _dragOffsetX = mouseX - windowX;
            _dragOffsetY = mouseY - windowY;
            GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Cross);
        }

        protected void WindowEndDrag(object o, ButtonReleaseEventArgs args)
        {
            GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Arrow);
        }

        protected virtual void WindowMotionDrag(object o, MotionNotifyEventArgs args)
        {
            EventBox eb = (EventBox)o;
            int mouseX, mouseY, windowX, windowY, moveX, moveY, currentX, currentY;
            Gdk.Display.Default.GetPointer(out mouseX, out mouseY);
            Display.GetPointer(out windowX, out windowY);
            moveX = windowX - _dragOffsetX;
            moveY = windowY - _dragOffsetY;
            Move(moveX, moveY);
            GetPosition(out currentX, out currentY);
            //_log.Debug(string.Format("{0}.WindowMotionDrag(): Mouse:{1}x{2}, Window:{3}x{4}, DragOffset:{5}x{6}, Move:{7}x{8}, CurrentPosition:{9}x{10}"
            //  , GetType().Name, mouseX, mouseY, windowX, windowY, _dragOffsetX, _dragOffsetY, moveX, moveY, currentX, currentY));
        }

        //protected override void OnResponse(ResponseType responseId)
        //{
        //}
    }

    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Used to pass ActionButtons from first to Second Dialog
    public class ActionAreaButton
    {
        //Public Properties
        private TouchButtonBase _button;
        public TouchButtonBase Button
        {
            get { return _button; }
            set { _button = value; }
        }
        private ResponseType _response;
        public ResponseType Response
        {
            get { return _response; }
            set { _response = value; }
        }
        //Events
        public event EventHandler Clicked;

        public ActionAreaButton(TouchButtonBase button, ResponseType response)
        {
            _button = button;
            _response = response;

            _button.Clicked += ActionAreaButton_Clicked;
        }

        void ActionAreaButton_Clicked(object sender, EventArgs e)
        {
            //Send this and Not sender, to catch base object
            if (Clicked != null) Clicked(this, e);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Helpers to get Button References

        public static TouchButtonIconWithText FactoryGetDialogButtonType(PosBaseDialogButtonType pButtonType)
        {
            return FactoryGetDialogButtonType(pButtonType, string.Empty, string.Empty, string.Empty);
        }

        public static TouchButtonIconWithText FactoryGetDialogButtonType(PosBaseDialogButtonType pButtonType, string pButtonName)
        {
            return FactoryGetDialogButtonType(pButtonType, pButtonName, string.Empty, string.Empty);
        }

        public static TouchButtonIconWithText FactoryGetDialogButtonType(string pButtonName, string pButtonLabel, string pButtonImage)
        {
            return FactoryGetDialogButtonType(PosBaseDialogButtonType.Default, pButtonName, pButtonLabel, pButtonImage);
        }

        public static TouchButtonIconWithText FactoryGetDialogButtonType(PosBaseDialogButtonType pButtonType, string pButtonName, string pButtonLabel, string pButtonImage)
        {
			System.Drawing.Size sizeBaseDialogDefaultButton = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogDefaultButton"]); 
            System.Drawing.Size sizeBaseDialogDefaultButtonIcon = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogDefaultButtonIcon"]);
            System.Drawing.Size sizeBaseDialogActionAreaButton = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogActionAreaButton"]);
            System.Drawing.Size sizeBaseDialogActionAreaButtonIcon = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogActionAreaButtonIcon"]);
            System.Drawing.Color colorBaseDialogActionAreaButtonFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogActionAreaButtonFont"]);
            //System.Drawing.Color colorBaseDialogActionAreaButtonBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogActionAreaButtonBackground"]);
            string fontBaseDialogActionAreaButton = FrameworkUtils.OSSlash(GlobalFramework.Settings["fontBaseDialogActionAreaButton"]);
            //Icons
            string fileActionDefault = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_default.png");
            string fileActionOK = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_ok.png");
            string fileActionCancel = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png");
            string fileActionYes = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_yes.png");
            string fileActionNo = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_no.png");
            string fileActionClose = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_close.png");
            string fileActionPrint = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_print.png");
            string fileActionPrintAs = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_print_as.png");
            string fileActionHelp = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_help.png");
            string fileActionCloneDocument = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_clone_document.png ");
            string fileActionOpenDocument = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_open_document.png");
            string fileActionSendEmailDocument = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_send_email.png");

            //Assign ButtonType to Name, Override After Switch
            string buttonName = (pButtonName != string.Empty) ? pButtonName : string.Format("touchButton{0}_DialogActionArea", pButtonType.ToString());
            string buttonLabel = string.Empty;
            string buttonImage = string.Empty;

            switch (pButtonType)
            {
                case PosBaseDialogButtonType.Default:
                    //Required to be changed by Code
                    buttonLabel = "Default";
                    buttonImage = fileActionDefault;
                    break;
                case PosBaseDialogButtonType.Ok:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_ok");
                    buttonImage = fileActionOK;
                    break;
                case PosBaseDialogButtonType.Cancel:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_cancel");
                    buttonImage = fileActionCancel;
                    break;
                case PosBaseDialogButtonType.Yes:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_yes");
                    buttonImage = fileActionYes;
                    break;
                case PosBaseDialogButtonType.No:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_no");
                    buttonImage = fileActionNo;
                    break;
                case PosBaseDialogButtonType.Close:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_close");
                    buttonImage = fileActionClose;
                    break;
                case PosBaseDialogButtonType.Print:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_print");
                    buttonImage = fileActionPrint;
                    break;
                case PosBaseDialogButtonType.PrintAs:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_print_as");
                    buttonImage = fileActionPrintAs;
                    break;
                case PosBaseDialogButtonType.Help:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_help");
                    buttonImage = fileActionHelp;
                    break;
                case PosBaseDialogButtonType.CloneDocument:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_clone_document");
                    buttonImage = fileActionCloneDocument;
                    break;
                case PosBaseDialogButtonType.OpenDocument:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_open_document");
                    buttonImage = fileActionOpenDocument;
                    break;
                case PosBaseDialogButtonType.SendEmailDocument:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_send_email_document");
                    buttonImage = fileActionSendEmailDocument;
                    break;
                default:
                    break;
            }

            //Overrride buttonName, buttonLabel, buttonImage if Defined form Parameters
            if (pButtonLabel != string.Empty) buttonLabel = pButtonLabel;
            if (pButtonImage != string.Empty) buttonImage = pButtonImage;

            //Result Button
            return new TouchButtonIconWithText(buttonName, System.Drawing.Color.Transparent, buttonLabel, fontBaseDialogActionAreaButton, colorBaseDialogActionAreaButtonFont, buttonImage, sizeBaseDialogActionAreaButtonIcon, sizeBaseDialogActionAreaButton.Width, sizeBaseDialogActionAreaButton.Height);
        }


        //IN009257 Redimensionar botões para a resolução 1024 x 768.
        public static TouchButtonIconWithText FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType pButtonType)
        {
            return FactoryGetDialogButtonTypeDocuments(pButtonType, string.Empty, string.Empty, string.Empty);
        }
        public static TouchButtonIconWithText FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType pButtonType, string pButtonName)
        {
            return FactoryGetDialogButtonTypeDocuments(pButtonType, pButtonName, string.Empty, string.Empty);
        }
        public static TouchButtonIconWithText FactoryGetDialogButtonTypeDocuments(string pButtonName, string pButtonLabel, string pButtonImage)
        {
            return FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType.Default, pButtonName, pButtonLabel, pButtonImage);
        }

        public static TouchButtonIconWithText FactoryGetDialogButtonTypeDocuments(PosBaseDialogButtonType pButtonType, string pButtonName, string pButtonLabel, string pButtonImage)
        {          
            System.Drawing.Color colorBaseDialogActionAreaButtonFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogActionAreaButtonFont"]);   
            //Icons
            string fileActionDefault = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_default.png");
            string fileActionOK = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_ok.png");
            string fileActionCancel = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png");
            string fileActionYes = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_yes.png");
            string fileActionNo = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_no.png");
            string fileActionClose = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_close.png");
            string fileActionPrint = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_print.png");
            string fileActionPrintAs = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_print_as.png");
            string fileActionHelp = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_help.png");
            string fileActionCloneDocument = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_clone_document.png ");
            string fileActionOpenDocument = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_open_document.png");
            string fileActionSendEmailDocument = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_send_email.png");

            //Assign ButtonType to Name, Override After Switch
            string buttonName = (pButtonName != string.Empty) ? pButtonName : string.Format("touchButton{0}_DialogActionArea", pButtonType.ToString());
            string buttonLabel = string.Empty;
            string buttonImage = string.Empty;

            switch (pButtonType)
            {
                case PosBaseDialogButtonType.Default:
                    //Required to be changed by Code
                    buttonLabel = "Default";
                    buttonImage = fileActionDefault;
                    break;
                case PosBaseDialogButtonType.Ok:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_ok");
                    buttonImage = fileActionOK;
                    break;
                case PosBaseDialogButtonType.Cancel:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_cancel");
                    buttonImage = fileActionCancel;
                    break;
                case PosBaseDialogButtonType.Yes:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_yes");
                    buttonImage = fileActionYes;
                    break;
                case PosBaseDialogButtonType.No:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_no");
                    buttonImage = fileActionNo;
                    break;
                case PosBaseDialogButtonType.Close:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_close");
                    buttonImage = fileActionClose;
                    break;
                case PosBaseDialogButtonType.Print:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_print");
                    buttonImage = fileActionPrint;
                    break;
                case PosBaseDialogButtonType.PrintAs:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_print_as");
                    buttonImage = fileActionPrintAs;
                    break;
                case PosBaseDialogButtonType.Help:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_help");
                    buttonImage = fileActionHelp;
                    break;
                case PosBaseDialogButtonType.CloneDocument:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_clone_document");
                    buttonImage = fileActionCloneDocument;
                    break;
                case PosBaseDialogButtonType.OpenDocument:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_open_document");
                    buttonImage = fileActionOpenDocument;
                    break;
                case PosBaseDialogButtonType.SendEmailDocument:
                    buttonLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_send_email_document");
                    buttonImage = fileActionSendEmailDocument;
                    break;
                default:
                    break;
            }

            //Overrride buttonName, buttonLabel, buttonImage if Defined form Parameters
            if (pButtonLabel != string.Empty) buttonLabel = pButtonLabel;
            if (pButtonImage != string.Empty) buttonImage = pButtonImage;

            //Result Button
            return new TouchButtonIconWithText(buttonName, System.Drawing.Color.Transparent, buttonLabel, ExpressionEvaluatorExtended.fontDocumentsSizeDefault, colorBaseDialogActionAreaButtonFont, buttonImage, ExpressionEvaluatorExtended.sizePosToolbarButtonIconSizeDefault, ExpressionEvaluatorExtended.sizePosToolbarButtonSizeDefault.Width, ExpressionEvaluatorExtended.sizePosToolbarButtonSizeDefault.Height);
        }

    }

    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Apenas usada para evitar usar a System.Collections.Generic, assim e so usar new ActionAreaButtonList() nos child dialogs
    public class ActionAreaButtons : List<ActionAreaButton>
    {
        public ActionAreaButtons()
        {
        }
    }
}