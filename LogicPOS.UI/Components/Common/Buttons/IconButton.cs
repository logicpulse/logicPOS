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
            var image = System.Drawing.Image.FromFile(ButtonSettings.Icon);
            image = Utils.ResizeAndCrop(image, ButtonSettings.IconSize);
            Gdk.Pixbuf pixBuf = Utils.ImageToPixbuf(image);
            Image button = new Image(pixBuf);

            image.Dispose();
            pixBuf.Dispose();

            return button;
        }

    }
}
