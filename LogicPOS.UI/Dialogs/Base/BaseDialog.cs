using Gtk;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Extensions;
using System;
using System.IO;

namespace LogicPOS.UI.Dialogs
{
    public abstract class BaseDialog : Dialog
    {
        protected BaseDialogColorSettings ColorSettings { get; set; } = new BaseDialogColorSettings();
        protected BaseDialogIconSettings IconSettings { get; set; } = new BaseDialogIconSettings();
        protected BaseDialogSizeSettings SizeSettings { get; set; } = new BaseDialogSizeSettings();
        protected BaseDialogFontSettings FontSettings { get; set; } = new BaseDialogFontSettings();
        public BaseDialogWindowSettings WindowSettings { get; set; } = new BaseDialogWindowSettings();

        public Window Source
        {
            get { return WindowSettings.Source; }
            set
            {
                WindowSettings.Source = value;
                TransientFor = value;
            }
        }

        public BaseDialog() { }

        public BaseDialog(Window parent,
                          DialogFlags flags,
                          bool confirmDialogOnEnter = true,
                          bool showCloseButton = true)
            : base("Base Dialog Window",
                   parent,
                   flags)
        {
            WindowSettings.ConfirmDialogOnEnter = confirmDialogOnEnter;
            WindowSettings.ShowCloseButton = showCloseButton;

            if (WindowSettings.UseMask == false)
            {
                if (parent != null)
                {
                    TransientFor = parent;

                    parent.DisableClicks();

                    Destroyed += delegate
                    {
                        parent.EnableClicks();
                    };
                }

                return;
            }

            ApplyMask(parent);
        }

   
        public void Initialize(Window parentWindow,
                                     DialogFlags flags,
                                     string icon,
                                     string title,
                                     System.Drawing.Size size,
                                     Widget content,
                                     ActionAreaButtons rightButtons)
        {
            Initialize(parentWindow,
                       flags,
                       icon,
                       title,
                       size,
                       content,
                       null,
                       rightButtons);
        }

        public void Initialize(
            Window parentWindow,
            DialogFlags flags,
            string icon,
            string title,
            System.Drawing.Size size,
            Widget content,
            Widget leftContent,
            ActionAreaButtons rightButtons)
        {
 
            WindowSettings.Source = parentWindow;
            WindowSettings.Size = size;
            WindowSettings.WindowTitle = new Label(title);
            WindowSettings.Icon = icon;
            WindowSettings.Content = content;
            WindowSettings.LeftContent = leftContent;
            WindowSettings.RightButtons = rightButtons;

            Modal = false;//require else cant drag and drop on linux
            Decorated = false;
            Resizable = false;
            WindowPosition = WindowPosition.Center;
            SetSizeRequest(size.Width, size.Height);
            DefaultResponse = ResponseType.Cancel;
            ModifyBg(StateType.Normal, ColorSettings.WindowBackgroundBorder.ToGdkColor());
            TransientFor = WindowSettings.Source;
 
            KeyReleaseEvent += PosBaseDialog_KeyReleaseEvent;

            Build();
        }

        private void PosBaseDialog_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (
                WindowSettings.ConfirmDialogOnEnter
                && args.Event.Key.ToString().Equals("Return")
                && WindowSettings.ConfirmButton != null
                && WindowSettings.ConfirmButton.Sensitive
            )
                WindowSettings.ConfirmButton.Click();
        }

        protected void Build()
        {
            //Prepare default icon
            Gdk.Pixbuf pixbufWindowIcon = new Gdk.Pixbuf(IconSettings.WindowIcon);

            if (WindowSettings.Icon != string.Empty && File.Exists(WindowSettings.Icon))
            {
                pixbufWindowIcon = new Gdk.Pixbuf(WindowSettings.Icon);
            }


            //Title
            EventBox eventboxWindowTitle = new EventBox();
            eventboxWindowTitle.ModifyBg(StateType.Normal, ColorSettings.TitleBackground.ToGdkColor());
            eventboxWindowTitle.HeightRequest = 40;
            //WindowIcon
            Image imageWindowsIcon = new Image(pixbufWindowIcon);
            Pango.FontDescription fontDescription = new Pango.FontDescription();
            fontDescription.Weight = Pango.Weight.Bold;
            fontDescription.Size = 18;

            WindowSettings.WindowTitle.SetAlignment(0, 0.5F);
            WindowSettings.WindowTitle.ModifyFg(StateType.Normal, System.Drawing.Color.White.ToGdkColor());
            WindowSettings.WindowTitle.ModifyFont(fontDescription);

            //HBox TitleBar - Container for Title and Icon
            HBox hboxWindowTitleBar = new HBox(false, 0);
            hboxWindowTitleBar.PackStart(imageWindowsIcon, false, false, 2);
            hboxWindowTitleBar.PackStart(WindowSettings.WindowTitle, true, true, 2);
            eventboxWindowTitle.Add(hboxWindowTitleBar);



            //Add Minimize TitleBar Icon
            Gdk.Pixbuf pixbufIconWindowMinimize = new Gdk.Pixbuf(IconSettings.MinimizeWindowIcon);
            Image gtkimageIconWindowMinimize = new Image(pixbufIconWindowMinimize);
            WindowSettings.MinimizeWindow = new EventBox();
            WindowSettings.MinimizeWindow.WidthRequest = pixbufIconWindowMinimize.Width;
            WindowSettings.MinimizeWindow.Add(gtkimageIconWindowMinimize);
            WindowSettings.MinimizeWindow.VisibleWindow = false;
            //if (_windowTitleCloseButton) hboxWindowTitleBar.PackStart(_eventboxMinimizeWindow, false, false, 2);

            //Add Close TitleBar Icon
            Gdk.Pixbuf pixbufIconWindowClose = new Gdk.Pixbuf(IconSettings.CloseWindowIcon);
            Image gtkimageIconWindowClose = new Image(pixbufIconWindowClose);
            WindowSettings.CloseWindow = new EventBox();
            WindowSettings.CloseWindow.WidthRequest = pixbufIconWindowClose.Width;
            WindowSettings.CloseWindow.Add(gtkimageIconWindowClose);
            WindowSettings.CloseWindow.VisibleWindow = false;

            if (WindowSettings.ShowCloseButton) hboxWindowTitleBar.PackStart(WindowSettings.CloseWindow, false, false, 2);

            //ActionArea Buttons Default Buttons
            //TouchButtonIconWithText buttonClose = new TouchButtonIconWithText("touchButtonClose_DialogActionArea", Color.Transparent, CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_button_label_close, _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, _fileActionClose, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height);
            //AddActionWidget(buttonClose, ResponseType.Close);

            //Dont Destroy Keyboard - Keep it in Memory
            WindowSettings.CloseWindow.ButtonPressEvent += delegate
            {
                if (this is PosKeyboardDialog)
                {
                    if (WindowSettings.UseMask) WindowSettings.Mask.Hide();
                    Respond(ResponseType.Close);
                }
                else
                {
                    Destroy();
                }
            };

            //Minimize Window
            WindowSettings.MinimizeWindow.ButtonReleaseEvent += delegate
            {

                Respond(ResponseType.None);
            };

            //Window Content - Box for Content to Add Border Arround Content Widget
            EventBox eventboxWindowContent = new EventBox();
            eventboxWindowContent.ModifyBg(StateType.Normal, ColorSettings.WindowBackground.ToGdkColor());
            eventboxWindowContent.Add(WindowSettings.Content);

            //Prepare ActionAreaButtons
            HBox hboxActionAreaRightButtons = new HBox() { HeightRequest = 60 };
            //Right
            if (WindowSettings.RightButtons != null && WindowSettings.RightButtons.Count > 0)
            {
                hboxActionAreaRightButtons = GetActionAreaButtonsHbox(WindowSettings.RightButtons, TextDirection.Rtl);
            };

            //Pack Navigator Box with Left and Right Content
            HBox hboxActionArea = new HBox(false, 0);
            //Left
            if (WindowSettings.LeftContent != null)
            {
                hboxActionArea.PackStart(WindowSettings.LeftContent, false, false, 0);
            }
            //MiddleSpace
            hboxActionArea.PackStart(new HBox(), true, true, 0);
            //Right
            if (WindowSettings.RightButtons != null && WindowSettings.RightButtons.Count > 0)
            {
                hboxActionArea.PackStart(hboxActionAreaRightButtons, false, false, 0);
            }

            //Final Dialog Pack
            VBox vboxWindow = new VBox(false, 0) { BorderWidth = WindowSettings.BorderWidth };
            vboxWindow.PackStart(eventboxWindowTitle, false, false, 0);
            vboxWindow.PackStart(eventboxWindowContent, true, true, 0);
            vboxWindow.PackStart(hboxActionArea, false, false, 0);

            //Window Content - Inner
            EventBox eventboxWindowBorderInner = new EventBox() { BorderWidth = 0 };
            eventboxWindowBorderInner.ModifyBg(StateType.Normal, ColorSettings.WindowBackground.ToGdkColor());
            //eventboxWindowBorderInner.Style = _styleBackground;
            eventboxWindowBorderInner.Add(vboxWindow);

            //Window Border - Outer
            EventBox eventboxWindowBorderOuter = new EventBox();
            eventboxWindowBorderOuter.ModifyBg(StateType.Normal, ColorSettings.WindowBackgroundBorder.ToGdkColor());
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
                if (WindowSettings.ConfirmButton == null)
                {
                    switch (pActionAreaRightButtons[pos].Response)
                    {
                        case ResponseType.Accept:
                        case ResponseType.Apply:
                        case ResponseType.Ok:
                        case ResponseType.Yes:
                        case ResponseType.Close:

                            WindowSettings.ConfirmButton = pActionAreaRightButtons[pos].Button;
                            break;
                    }
                }
            }
            return hboxActionArea;
        }
   
        private void Button_Clicked(object sender, EventArgs e)
        {
            ActionAreaButton button = (ActionAreaButton)sender;
            Respond(button.Response);
        }

        protected void WindowStartDrag(object o, ButtonPressEventArgs args)
        {
            int windowX, windowY, mouseX, mouseY;
            Gdk.Display.Default.GetPointer(out mouseX, out mouseY);
            GetPosition(out windowX, out windowY);
            WindowSettings.DragOffsetX = mouseX - windowX;
            WindowSettings.DragOffsetY = mouseY - windowY;
            GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Cross);
        }

        protected void WindowEndDrag(object o, ButtonReleaseEventArgs args)
        {
            GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Arrow);
        }

        protected virtual void WindowMotionDrag(object o, MotionNotifyEventArgs args)
        {
            int mouseX, mouseY, windowX, windowY, moveX, moveY, currentX, currentY;
            Gdk.Display.Default.GetPointer(out mouseX, out mouseY);
            Display.GetPointer(out windowX, out windowY);
            moveX = windowX - WindowSettings.DragOffsetX;
            moveY = windowY - WindowSettings.DragOffsetY;
            Move(moveX, moveY);
            GetPosition(out currentX, out currentY);
        }

        public void HideCloseButton() => WindowSettings.ShowCloseButton = false;

        private void ApplyMask(Window parent)
        {
            //Window Mask Background Hack
            WindowSettings.Mask = new Window("");
            WindowSettings.Mask.TransientFor = parent;
            WindowSettings.Mask.SetSizeRequest(10, 10);
            WindowSettings.Mask.Move(-100, -100);
            WindowSettings.Mask.ModifyBg(StateType.Normal, System.Drawing.Color.Black.ToGdkColor());

            //Prevent click outside Dialog
            WindowSettings.Mask.Opacity = 0.35F;//0.55F | 0.75F
            WindowSettings.Mask.CanFocus = false;
            WindowSettings.Mask.AcceptFocus = false;
            WindowSettings.Mask.Sensitive = false;
            WindowSettings.Mask.Fullscreen();
            WindowSettings.Mask.Show();

            TransientFor = WindowSettings.Mask;

            Destroyed += delegate { WindowSettings.Mask.Destroy(); };
        }


    }
}