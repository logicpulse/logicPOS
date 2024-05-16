using Gtk;
using LogicPOS.Settings;

namespace LogicPOS.UI
{
    public static class GtkUtils
    {
        public static EventBox CreateMinimizeButton()
        {
            string minimizeWindowIconLocation = GtkSettings.DefaultMinimizeWindowIconLocation;

            Gdk.Pixbuf pixBuffer = new Gdk.Pixbuf(minimizeWindowIconLocation);
            Gtk.Image buttonImage = new Gtk.Image(pixBuffer);
            EventBox button = new EventBox();
            button.WidthRequest = pixBuffer.Width;
            button.HeightRequest = pixBuffer.Height;
            button.Add(buttonImage);

            return button;
        }
    }
}
