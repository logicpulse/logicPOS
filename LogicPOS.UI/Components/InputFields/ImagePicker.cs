using Gtk;
using System.Drawing;

namespace LogicPOS.UI.Components.InputFields
{
    public class ImagePicker
    {
        public Label Label { get; private set; }
        public FileChooserButton FileChooserButton { get; private set; } = new FileChooserButton(string.Empty, FileChooserAction.Open) { HeightRequest = 23 };
        public Gtk.Image Preview = new Gtk.Image() { WidthRequest = 37, HeightRequest = 37 };
        public VBox Component { get; private set; }

        public ImagePicker(string labelText)
        {
            Label = new Label(labelText);
            Label.SetAlignment(0, 0);
            FileChooserButton.Filter = GetFilter();
            AddEventHandlers();
            Component = CreateComponent();
        }

        private void AddEventHandlers()
        {
            FileChooserButton.SelectionChanged += (sender, eventArgs) =>
            {
                Preview.Pixbuf = logicpos.Utils.ResizeAndCropFileToPixBuf(FileChooserButton.Filename, new Size(Preview.WidthRequest, Preview.HeightRequest));
            };
        }

        private VBox CreateComponent()
        {
            VBox vbox = new VBox(false, 0);
            vbox.PackStart(Label, false, false, 0);

            HBox hbox = new HBox(false, 0);
            hbox.PackStart(FileChooserButton, true, true, 0);
            Frame frame = new Frame();
            frame.ShadowType = ShadowType.None;
            frame.Add(Preview);
            hbox.PackStart(frame, false, false, 0);

            vbox.PackStart(hbox, false, false, 0);

            return vbox;
        }

        private FileFilter GetFilter()
        {
            FileFilter filter = new FileFilter();
            filter.Name = "BMP, PNG and JPEG images";
            filter.AddMimeType("image/png");
            filter.AddPattern("*.png");
            filter.AddMimeType("image/jpeg");
            filter.AddPattern("*.jpg");
            filter.AddMimeType("image/bmp");
            filter.AddPattern("*.bmp");
            return filter;
        }
    }
}
