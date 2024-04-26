using Gtk;
using logicpos.Extensions;
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

        public TouchButtonText(string pName)
            : base(pName)
        {
        }

        public TouchButtonText(string pName, Color pColor, string pLabelText, string pFont, Color pColorFont, int pWidth, int pHeight)
            : base(pName)
        {
            InitObject(pName, pColor, pLabelText, pFont, pColorFont, pWidth, pHeight);
            InitObject(pName, pColor, _widget, pWidth, pHeight);
        }

        public void InitObject(string pName, Color pColor, string pLabelText, string pFont, Color pColorFont, int pWidth, int pHeight)
        {
            _labelText = pLabelText;
            _label = new Label(_labelText);
            ChangeFont(pFont, pColorFont);
            _widget = _label;
        }

        public void SetFont(string pFont)
        {
            //Font, Style, Size ex "Ahafoni CLM Bold 100"
            Pango.FontDescription fontDesc = Pango.FontDescription.FromString(pFont);
            _label.ModifyFont(fontDesc);
        }

        public void ChangeFont(string pFont, Color pColorFont)
        {
            //font
            SetFont(pFont);
            //color
            Color colNormal = pColorFont;
            Color colPrelight = colNormal.Lighten();
            Color colActive = colPrelight.Lighten();
            Color colInsensitive = colNormal.Lighten();
            Color colSelected = Color.FromArgb(125, 0, 0);
            _label.ModifyFg(StateType.Normal, colNormal.ToGdkColor());
            _label.ModifyFg(StateType.Prelight, colPrelight.ToGdkColor());
            _label.ModifyFg(StateType.Active, colActive.ToGdkColor());
            _label.ModifyFg(StateType.Insensitive,colInsensitive.ToGdkColor());
            _label.ModifyFg(StateType.Selected, colSelected.ToGdkColor());
        }
    }
}
