using Gtk;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonText : TouchButtonBase
    {
        protected Widget _widget;
        protected Label _label;
        protected string _labelText;
        public string LabelText
        {
            get { return _labelText; }
            set { _labelText = value; _label.Text = value; }
        }

        public TouchButtonText(String pName)
            : base(pName)
        {
        }

        public TouchButtonText(String pName, Color pColor, String pLabelText, String pFont, Color pColorFont, int pWidth, int pHeight)
            : base(pName)
        {
            InitObject(pName, pColor, pLabelText, pFont, pColorFont, pWidth, pHeight);
            base.InitObject(pName, pColor, _widget, pWidth, pHeight);
        }

        public void InitObject(String pName, Color pColor, String pLabelText, String pFont, Color pColorFont, int pWidth, int pHeight)
        {
            _labelText = pLabelText;
            _label = new Label(_labelText);
            ChangeFont(pFont, pColorFont);
            _widget = _label;
        }

        public void SetFont(String pFont)
        {
            //Font, Style, Size ex "Ahafoni CLM Bold 100"
            Pango.FontDescription fontDesc = Pango.FontDescription.FromString(pFont);
            _label.ModifyFont(fontDesc);
        }

        public void ChangeFont(String pFont, Color pColorFont)
        {
            //font
            SetFont(pFont);
            //color
            Color colNormal = pColorFont;
            Color colPrelight = Utils.Lighten(colNormal);
            Color colActive = Utils.Lighten(colPrelight);
            Color colInsensitive = Utils.Darken(colNormal);
            Color colSelected = Color.FromArgb(125, 0, 0);
            _label.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(colNormal));
            _label.ModifyFg(StateType.Prelight, Utils.ColorToGdkColor(colPrelight));
            _label.ModifyFg(StateType.Active, Utils.ColorToGdkColor(colActive));
            _label.ModifyFg(StateType.Insensitive, Utils.ColorToGdkColor(colInsensitive));
            _label.ModifyFg(StateType.Selected, Utils.ColorToGdkColor(colSelected));
        }
    }
}
