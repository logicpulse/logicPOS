using Gtk;
using System;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonIconWithText : TouchButtonText
    {
        public TouchButtonIconWithText(String name, System.Drawing.Color color, String labelText, String font, System.Drawing.Color colorFont, String icon, System.Drawing.Size sizeIcon, int width, int height)
            : base(name)
        {
            InitObject(name, color, labelText, font, colorFont, icon, sizeIcon, width, height);
            base.InitObject(name, color, _widget, width, height);
        }

        public void InitObject(String name, System.Drawing.Color color, String labelText, String font, System.Drawing.Color colorFont, String icon, System.Drawing.Size sizeIcon, int width, int height)
        {
            VBox vbox = new VBox(false, 0);
            vbox.BorderWidth = 2;
            System.Drawing.Image imageIcon;
            _label = new Label(labelText);
            ChangeFont(font, colorFont);

            if (icon != string.Empty && File.Exists(icon))
            {
                try
                {
                    imageIcon = System.Drawing.Image.FromFile(icon);
                    imageIcon = Utils.ResizeAndCrop(imageIcon, sizeIcon);
                    Gdk.Pixbuf pixBuf = Utils.ImageToPixbuf(imageIcon);
                    Image gtkimageButton = new Image(pixBuf);
                    vbox.PackStart(gtkimageButton);
                    imageIcon.Dispose();
                    pixBuf.Dispose();
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("InitObject(): Error load icon from file [{0}]: {1}", icon, ex.Message), ex);
                }
            }
            vbox.PackStart(_label);
            _widget = vbox;
        }
    }
}
