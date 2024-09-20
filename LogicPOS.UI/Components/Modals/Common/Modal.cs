using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Extensions;
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
                     DialogFlags flags = DialogFlags.DestroyWithParent) : base(title,parent, flags)
        {
            if (!string.IsNullOrEmpty(icon))
            {
                icon = ModalIconsSettings.Default.WindowIcon;
            }

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

        public Modal WithBody(Widget body)
        {
            WindowSettings.Body = body;
            return this;
        }

        public Modal WithRightButtons(ActionAreaButtons rightButtons)
        {
            WindowSettings.RightButtons = rightButtons;
            return this;
        }

        private EventBox CreateTitleBarEventBox()
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



    }
}
