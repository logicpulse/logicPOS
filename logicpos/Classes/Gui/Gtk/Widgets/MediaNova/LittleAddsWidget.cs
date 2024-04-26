using Gtk;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class LittleAddsWidget : VBox
    {
        public RadioButton RadioButton { get; set; }
        public Image Image { get; set; }

        public Label LabelInfo { get; set; }

        public Label LabelModules { get; set; }

        public Label LabelDimensions { get; set; }
        public string Value { get; set; }

        public LittleAddsWidget(string pTextValue, string pImageFileName, string pTextInfo, string pTextModules, string pTextDimensions)
        {
            //Parameters
            Value = pTextValue;

            if (File.Exists(pImageFileName))
            {
                Image = new Image(pImageFileName);
            }
            else
            {
                Image = new Image();
            }
            LabelInfo = new Label(pTextInfo);
            LabelModules = new Label(pTextModules);
            LabelDimensions = new Label(pTextDimensions);
        }
    }
}
