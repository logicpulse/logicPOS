using DevExpress.Data.Browsing.Design;
using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Extensions;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals.Common
{
    public abstract class Modal : Dialog
    {
        public ModalWindowSettings WindowSettings { get; set; } = new ModalWindowSettings();

        public Modal(Window parent,
                     string title,
                     Size size,
                     string icon = null,
                     DialogFlags flags = DialogFlags.DestroyWithParent,
                     bool render = true) : base(title,parent, flags)
        {
            if (string.IsNullOrEmpty(icon))
            {
                icon = ModalIconsSettings.Default.WindowIcon;
            }
         
            InitializeWindow(parent, title, size, icon);

            if (render)
            {
                Render();
            }
        }

        protected void Render()
        {
            Design();
            ShowAll();
            ActionArea.Visible = false;
        }

        private void InitializeWindow(Window parent,
                                      string title,
                                      Size size,
                                      string icon)
        {
            WindowSettings.Icon = icon;
            WindowSettings.Source = parent;
            WindowSettings.DesignTitleLabel(title);
            WindowSettings.DesignMinimizeButton(this);
            WindowSettings.DesignCloseButton(this);
            WindowSettings.Size = size;
            Modal = false;
            Decorated = false;
            Resizable = false;
            WindowPosition = WindowPosition.Center;
            SetSizeRequest(size.Width, size.Height);
            DefaultResponse = ResponseType.Cancel;
            ModifyBg(StateType.Normal, ModalColorSettings.Default.WindowBackgroundBorder.ToGdkColor());
            TransientFor = parent;
        }

        protected abstract ActionAreaButtons CreateActionAreaButtons();
        protected abstract Widget CreateBody();
        protected virtual Widget CreateLeftContent() => null;

        private void Design()
        {
            WindowSettings.RightButtons = CreateActionAreaButtons();
            WindowSettings.Body = CreateBody();
            WindowSettings.LeftContent = CreateLeftContent();

            VBox layout = new VBox(false, 0) { BorderWidth = WindowSettings.BorderWidth };
            layout.PackStart(CreateTitleBarEventBox(), false, false, 0);
            layout.PackStart(WindowSettings.CreateBodyEventBox(), true, true, 0);

            if(WindowSettings.RightButtons != null)
            {
                layout.PackStart(CreateActionAreaBox(), false, false, 0);
            }
          
            EventBox eventboxWindowBorderInner = new EventBox() { BorderWidth = 0 };
            eventboxWindowBorderInner.ModifyBg(StateType.Normal, EntitySelectionModalColors.WindowBackground.ToGdkColor());
            eventboxWindowBorderInner.Add(layout);

            EventBox eventboxWindowBorderOuter = new EventBox();
            eventboxWindowBorderOuter.ModifyBg(StateType.Normal, EntitySelectionModalColors.WindowBackgroundBorder.ToGdkColor());
            eventboxWindowBorderOuter.Add(eventboxWindowBorderInner);

            VBox.PackStart(eventboxWindowBorderOuter, true, true, 0);
        }

        protected EventBox CreateTitleBarEventBox()
        {
            EventBox title = new EventBox();
            title.ModifyBg(StateType.Normal, EntitySelectionModalColors.TitleBackground.ToGdkColor());
            title.HeightRequest = 40;
            title.Add(WindowSettings.CreateTitleBar());

            title.ButtonPressEvent += WindowStartDrag;
            title.ButtonReleaseEvent += WindowEndDrag;
            title.MotionNotifyEvent += WindowMotionDrag;

            return title;
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

        protected HBox CreateActionAreaBox()
        {
            HBox box = new HBox(false, 0);

            if (WindowSettings.LeftContent != null)
            {
                box.PackStart(WindowSettings.LeftContent, false, false, 0);
            }

            box.PackStart(new HBox(), true, true, 0);
            box.PackStart(CreateRightButtonsBox(), false, false, 0);

            return box;
        }

        protected HBox CreateRightButtonsBox()
        {
            HBox box = new HBox() { HeightRequest = 60 };

            if (WindowSettings.RightButtons != null && WindowSettings.RightButtons.Count > 0)
            {
                box = DesignRightButtonsBox(WindowSettings.RightButtons);
            };

            return box;
        }

        protected void Button_Clicked(object sender, EventArgs e)
        {
            ActionAreaButton button = (ActionAreaButton)sender;
            Respond(button.Response);
        }

        protected HBox DesignRightButtonsBox(ActionAreaButtons buttons)
        {
            int position;
            HBox box = new HBox(false, 5) { Direction = TextDirection.Rtl };
            for (int i = 0; i < buttons.Count; i++)
            {
                position = buttons.Count - 1 - i;
                buttons[position].Clicked += Button_Clicked;
                box.PackStart(buttons[position].Button, false, false, 0);

                if (WindowSettings.BtnConfirm == null)
                {
                    switch (buttons[position].Response)
                    {
                        case ResponseType.Accept:
                        case ResponseType.Apply:
                        case ResponseType.Ok:
                        case ResponseType.Yes:
                        case ResponseType.Close:

                            WindowSettings.BtnConfirm = buttons[position].Button;
                            break;
                    }
                }
            }
            return box;
        }

        protected void HideCloseButton() => WindowSettings.ShowCloseButton = false;

        protected void ApplyMask(Window parent)
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
