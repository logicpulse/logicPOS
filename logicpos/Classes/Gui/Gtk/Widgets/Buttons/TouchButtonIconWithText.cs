using Gtk;
using logicpos.App;
using logicpos.datalayer.App;
using logicpos.Extensions;
using System;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonIconWithText : TouchButtonText
    {
        public TouchButtonIconWithText(string name, System.Drawing.Color color, string labelText, string font, System.Drawing.Color colorFont, string icon, System.Drawing.Size sizeIcon, int width, int height)
            : base(name)
        {
            InitObject(name, color, labelText, font, colorFont, icon, sizeIcon, width, height, false);
            InitObject(name, color, _widget, width, height);
        }

        public TouchButtonIconWithText(string name, System.Drawing.Color color, string labelText, string font, System.Drawing.Color colorFont, string icon, System.Drawing.Size sizeIcon, int width, int height, bool leftImg)
            : base(name)
        {
            InitObject(name, color, labelText, font, colorFont, icon, sizeIcon, width, height, true);
            InitObject(name, color, _widget, width, height);
        }

        public Widget Content;

        public void InitObject(string name, System.Drawing.Color color, string labelText, string font, System.Drawing.Color colorFont, string icon, System.Drawing.Size sizeIcon, int width, int height, bool leftImg)
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
                        imageIcon = logicpos.Utils.ResizeAndCrop(imageIcon, sizeIcon);
                        Gdk.Pixbuf pixBuf = logicpos.Utils.ImageToPixbuf(imageIcon);
                        Image gtkimageButton = new Image(pixBuf);
                        vbox.PackStart(gtkimageButton);
                        imageIcon.Dispose();
                        pixBuf.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(string.Format("InitObject(): Error load icon from file [{0}]: {1}", icon, ex.Message), ex);
                    }
                }
                vbox.PackStart(_label);
                _widget = vbox;
            }
            else
            {
                string fontPosBackOfficeParent = LogicPOS.Settings.GeneralSettings.Settings["fontPosBackOfficeParent"];
                string fontPosBackOfficeParentLowRes = LogicPOS.Settings.GeneralSettings.Settings["fontPosBackOfficeParentLowRes"];
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
                        imageIcon = logicpos.Utils.ResizeAndCrop(imageIcon, sizeIcon);
                        Gdk.Pixbuf pixBuf = logicpos.Utils.ImageToPixbuf(imageIcon);
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
                        _logger.Error(string.Format("InitObject(): Error load icon from file [{0}]: {1}", icon, ex.Message), ex);
                    }
                }
                _label.ModifyFont(fontDescription);
                _label.ModifyFg(StateType.Active, "0, 0, 0".StringToColor().ToGdkColor());
                _label.SetAlignment(0.0f, 0.5f);
                hbox.PackStart(_label, true, true, 0);
                _widget = hbox;


            }
        }
    }
}
