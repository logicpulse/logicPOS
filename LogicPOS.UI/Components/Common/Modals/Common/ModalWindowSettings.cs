using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using System.IO;

namespace LogicPOS.UI.Components.Modals.Common
{
    public class ModalWindowSettings
    {
        public string Icon { get; set; }
        public System.Drawing.Size Size { get; set; }
        public EventBox Close { get; set; }
        public EventBox Minimize { get; set; }
        public Widget Body { get; set; } = new Fixed();
        public Widget LeftContent { get; set; }
        public CustomButton BtnConfirm { get; set; }
        public ActionAreaButtons RightButtons { get; set; }
        public Window Source { get; set; }
        public int DragOffsetX { get; set; }
        public int DragOffsetY { get; set; }
        public uint BorderWidth => 5;

        private bool _showCloseButton = true;

        public bool ShowCloseButton

        {
            get { return _showCloseButton; }
            set
            {
                _showCloseButton = value;

                if (value)
                {
                    Close?.ShowAll();
                }
                else
                {
                    Close?.HideAll();
                }
            }
        }

        public bool UseMask = AppSettings.Instance.UseBaseDialogWindowMask;
        public Window Mask { get; set; }

        public Label Title { get; set; } = new Label();
        public bool ConfirmOnEnter { get; set; }

        public void DesignTitleLabel(string title)
        {
            Title = new Label(title);
            Pango.FontDescription fontDescription = new Pango.FontDescription();
            fontDescription.Weight = Pango.Weight.Bold;
            fontDescription.Size = 18;

            Title.SetAlignment(0, 0.5F);
            Title.ModifyFg(StateType.Normal, System.Drawing.Color.White.ToGdkColor());
            Title.ModifyFont(fontDescription);
        }

        private Gtk.Image CreateMinimizeIcon()
        {
            Gdk.Pixbuf icon = new Gdk.Pixbuf(ModalIconsSettings.Default.MinimizeWindowIcon);
            Gtk.Image image = new Gtk.Image(icon);
            return image;
        }

        public void DesignMinimizeButton(Modal modal)
        {
            var icon = CreateMinimizeIcon();
            Minimize = new EventBox();
            Minimize.WidthRequest = icon.Pixbuf.Width;
            Minimize.Add(icon);
            Minimize.VisibleWindow = false;

            Minimize.ButtonReleaseEvent += delegate
            {
                modal.Respond(ResponseType.None);
            };
        }

        private Gtk.Image CreateCloseIcon()
        {
            Gdk.Pixbuf icon = new Gdk.Pixbuf(EntitySelectionModalIcons.CloseWindowIcon);
            Gtk.Image image = new Gtk.Image(icon);
            return image;
        }

        public void DesignCloseButton(Modal modal)
        {
            var icon = CreateCloseIcon();
            Close = new EventBox();
            Close.WidthRequest = icon.Pixbuf.Width;
            Close.Add(icon);
            Close.VisibleWindow = false;

            Close.ButtonPressEvent += (o, args) =>
            {
                modal.Destroy();
            };
        }

        private Gtk.Image CreateIcon()
        {
            Gdk.Pixbuf pixbufWindowIcon = new Gdk.Pixbuf(Icon);

            if (Icon != string.Empty && File.Exists(Icon))
            {
                pixbufWindowIcon = new Gdk.Pixbuf(Icon);
            }

            Gtk.Image imageWindowsIcon = new Gtk.Image(pixbufWindowIcon);

            return imageWindowsIcon;
        }

        public HBox CreateTitleBar()
        {
            HBox box = new HBox(false, 0);
            box.PackStart(CreateIcon(), false, false, 2);
            box.PackStart(Title, true, true, 2);

            if (ShowCloseButton)
            {
                box.PackStart(Close, false, false, 2);
            }
            return box;
        }

        public EventBox CreateBodyEventBox()
        {
            EventBox box = new EventBox();
            box.ModifyBg(StateType.Normal, EntitySelectionModalColors.WindowBackground.ToGdkColor());
            box.Add(Body);
            return box;
        }
    }
}
