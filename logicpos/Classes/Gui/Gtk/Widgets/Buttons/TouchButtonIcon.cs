using Gtk;
using System;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonIcon : TouchButtonBase
    {
        public Widget widget;

        public TouchButtonIcon(String name, System.Drawing.Color color, String icon, System.Drawing.Size sizeIcon, int width, int height)
            : base(name)
        {
            InitObject(name, color, icon, sizeIcon, width, height);
            base.InitObject(name, color, widget, width, height);
        }

        public void InitObject(String name, System.Drawing.Color color, String image, System.Drawing.Size sizeIcon, int width, int height)
        {
            System.Drawing.Image imageIcon;
            Image gtkimageButton = null;

            if (image != string.Empty && File.Exists(image))
            {
                try
                {
                    imageIcon = System.Drawing.Image.FromFile(image);
                    imageIcon = Utils.ResizeAndCrop(imageIcon, sizeIcon);
                    Gdk.Pixbuf pixBuf = Utils.ImageToPixbuf(imageIcon);
                    gtkimageButton = new Image(pixBuf);
                    imageIcon.Dispose();
                    pixBuf.Dispose();
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("InitObject() : Error load image [{0}]: {1}", image, ex.Message), ex);
                }
            }
            widget = gtkimageButton;
        }
    }
}
