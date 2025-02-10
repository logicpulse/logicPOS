using Gtk;
using logicpos;
using LogicPOS.Settings;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Extensions;
using System.Drawing;

namespace LogicPOS.UI.Buttons
{
    public class XAccordionParentButton
    {
        public Button Button { get; private set; }

        public XAccordionParentButton(string text, string icon)
        {
            Button = CreateComponent(icon, text);
        }

        private Button CreateComponent(string icon, string text)
        {
            HBox component = new HBox(false, 0);
            var image = new Gtk.Image(icon);

            if (BackOfficeWindow.ScreenSize.Height <= 800)
            {
                Size sizeIcon = new Size(20, 20);
                System.Drawing.Image imageIcon;
                imageIcon = System.Drawing.Image.FromFile(icon);
                imageIcon = Utils.ResizeAndCrop(imageIcon, sizeIcon);
                Gdk.Pixbuf pixBuf = Utils.ImageToPixbuf(imageIcon);
                Gtk.Image gtkimageButton = new Gtk.Image(pixBuf);
                component.PackStart(gtkimageButton, false, false, 3);
                imageIcon.Dispose();
                pixBuf.Dispose();
            }
            else
            {
                component.PackStart(image, false, false, 3);
            }

            component.PackStart(CreateLabel(text), true, true, 0);
            var button = new Button(component);
            button.Name = "AccordionParentButton";

            if (BackOfficeWindow.ScreenSize.Height <= 800)
            {
                button.HeightRequest = 25;
            }
            else
            {
                button.HeightRequest = 35;
            }
           
            return button;
        }

        public static Label CreateLabel(string text)
        {
            var label = new Label(text);
            Color colNormal = "61, 61, 61".StringToColor();
            Color colPrelight = colNormal.Lighten();
            Color colActive = colPrelight.Lighten();
            Color colInsensitive = colNormal.Darken();
            Color colSelected = Color.FromArgb(125, 0, 0);

            string _fontPosBackOfficeParent = AppSettings.Instance.fontPosBackOfficeParent;
            string _fontPosBackOfficeChild = AppSettings.Instance.fontPosBackOfficeChild;
            string _fontPosBackOfficeParentLowRes = AppSettings.Instance.fontPosBackOfficeParentLowRes;
            string _fontPosBackOfficeChildLowRes = AppSettings.Instance.fontPosBackOfficeChildLowRes;

            Pango.FontDescription fontPosBackOfficeparentLowRes = Pango.FontDescription.FromString(_fontPosBackOfficeParentLowRes);
            Pango.FontDescription fontPosBackOfficeParent = Pango.FontDescription.FromString(_fontPosBackOfficeParent);
            Pango.FontDescription fontPosBackOfficeChildLowRes = Pango.FontDescription.FromString(_fontPosBackOfficeChildLowRes);
            Pango.FontDescription fontPosBackOfficeChild = Pango.FontDescription.FromString(_fontPosBackOfficeChild);


            if (BackOfficeWindow.ScreenSize.Height <= 800)
            {
                label.ModifyFont(fontPosBackOfficeparentLowRes);
            }
            else
            {
                label.ModifyFont(fontPosBackOfficeParent);
            }

            label.ModifyFg(StateType.Normal, colNormal.ToGdkColor());
            label.ModifyFg(StateType.Prelight, colPrelight.ToGdkColor());
            label.ModifyFg(StateType.Active, colActive.ToGdkColor());
            label.ModifyFg(StateType.Insensitive, colInsensitive.ToGdkColor());
            label.ModifyFg(StateType.Selected, colSelected.ToGdkColor());

            label.SetAlignment(0.0f, 0.5f);

            return label;
        }
    }
}
