using Gtk;
using logicpos;

namespace LogicPOS.UI.Buttons
{
    public class IconButton : CustomButton
    {
       
        public IconButton(ButtonSettings settings)
            : base(settings)
        {
            ButtonSettings.Widget = CreateWidget();
            Initialize();
        }

        private Widget CreateWidget()
        {
            var icon = System.Drawing.Image.FromFile(ButtonSettings.Icon);
            icon = Utils.ResizeAndCrop(icon, ButtonSettings.IconSize);
            Gdk.Pixbuf pixBuf = Utils.ImageToPixbuf(icon);
            Image button = new Image(pixBuf);

            icon.Dispose();
            pixBuf.Dispose();

            return button;
        }

    }
}
