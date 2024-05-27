using Gtk;
using System;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonIcon : TouchButtonBase
    {
        public Widget widget;

        public TouchButtonIcon(string name, System.Drawing.Color color, string icon, System.Drawing.Size sizeIcon, int width, int height)
            : base(name)
        {
            InitObject(name, color, icon, sizeIcon, width, height);
            InitObject(name, color, widget, width, height);
        }

        public void InitObject(string name, System.Drawing.Color color, string image, System.Drawing.Size sizeIcon, int width, int height)
        {
            System.Drawing.Image imageIcon;
            Image gtkimageButton = null;

            if (image != string.Empty && File.Exists(image))
            {
                try
                {
                    imageIcon = System.Drawing.Image.FromFile(image);
                    imageIcon = logicpos.Utils.ResizeAndCrop(imageIcon, sizeIcon);
                    Gdk.Pixbuf pixBuf = logicpos.Utils.ImageToPixbuf(imageIcon);
                    gtkimageButton = new Image(pixBuf);
                    imageIcon.Dispose();
                    pixBuf.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("InitObject() : Error load image [{0}]: {1}", image, ex.Message), ex);
                }
            }
            widget = gtkimageButton;
        }
    }
}
