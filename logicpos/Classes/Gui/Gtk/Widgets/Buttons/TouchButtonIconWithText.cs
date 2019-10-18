using Gtk;
using logicpos.App;
using System;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonIconWithText : TouchButtonText
    {
        public TouchButtonIconWithText(String name, System.Drawing.Color color, String labelText, String font, System.Drawing.Color colorFont, String icon, System.Drawing.Size sizeIcon, int width, int height)
            : base(name)
        {
            InitObject(name, color, labelText, font, colorFont, icon, sizeIcon, width, height, false);
            base.InitObject(name, color, _widget, width, height);
        }

        public TouchButtonIconWithText(String name, System.Drawing.Color color, String labelText, String font, System.Drawing.Color colorFont, String icon, System.Drawing.Size sizeIcon, int width, int height, bool leftImg)
            : base(name)
        {
            InitObject(name, color, labelText, font, colorFont, icon, sizeIcon, width, height, true);
            base.InitObject(name, color, _widget, width, height);
        }

        public Widget Content;

        public void InitObject(String name, System.Drawing.Color color, String labelText, String font, System.Drawing.Color colorFont, String icon, System.Drawing.Size sizeIcon, int width, int height, bool leftImg)
        {
            if (!leftImg)
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
            else
            {
                String fontPosBackOfficeParent = GlobalFramework.Settings["fontPosBackOfficeParent"];
                String fontPosBackOfficeParentLowRes = GlobalFramework.Settings["fontPosBackOfficeParentLowRes"];
                Pango.FontDescription fontDescription = Pango.FontDescription.FromString(fontPosBackOfficeParent);

                if (GlobalApp.ScreenSize.Height == 800)
                {
                    fontDescription = Pango.FontDescription.FromString(fontPosBackOfficeParentLowRes);
                }
                //tmpFont.Weight = Pango.Weight.Bold;
                //tmpFont.Size = 2;

                HBox hbox = new HBox(false, 0);
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
                        if (GlobalApp.ScreenSize.Height == 800)
                        {
                            hbox.PackStart(gtkimageButton, false, false, 4);
                        }
                        else
                        {
                            hbox.PackStart(gtkimageButton, false, false, 5);
                        }
                        
                        imageIcon.Dispose();
                        pixBuf.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _log.Error(string.Format("InitObject(): Error load icon from file [{0}]: {1}", icon, ex.Message), ex);
                    }
                }
                _label.ModifyFont(fontDescription);
                _label.ModifyFg(StateType.Active, Utils.ColorToGdkColor(FrameworkUtils.StringToColor("0, 0, 0")));
                _label.SetAlignment(0.0f, 0.5f);
                hbox.PackStart(_label, true, true, 0);
                _widget = hbox;


            }
        }
    }
}
