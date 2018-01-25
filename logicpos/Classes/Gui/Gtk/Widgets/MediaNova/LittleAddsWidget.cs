using Gtk;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class LittleAddsWidget : VBox
    {
        //Public
        private RadioButton _radioButton;
        public RadioButton RadioButton
        {
            get { return _radioButton; }
            set { _radioButton = value; }
        }
        private Image _image;
        public Image Image
        {
            get { return _image; }
            set { _image = value; }
        }
        private Label _labelInfo;

        public Label LabelInfo
        {
            get { return _labelInfo; }
            set { _labelInfo = value; }
        }
        private Label _labelModules;

        public Label LabelModules
        {
            get { return _labelModules; }
            set { _labelModules = value; }
        }
        private Label _labelDimensions;

        public Label LabelDimensions
        {
            get { return _labelDimensions; }
            set { _labelDimensions = value; }
        }
        private string _value;
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public LittleAddsWidget(string pTextValue, string pImageFileName, string pTextInfo, string pTextModules, string pTextDimensions)
        {
            //Parameters
            _value = pTextValue;

            if (File.Exists(pImageFileName))
            {
                _image = new Image(pImageFileName);
            }
            else
            {
                _image = new Image();
            }
            _labelInfo = new Label(pTextInfo);
            _labelModules = new Label(pTextModules);
            _labelDimensions = new Label(pTextDimensions);
        }
    }
}
