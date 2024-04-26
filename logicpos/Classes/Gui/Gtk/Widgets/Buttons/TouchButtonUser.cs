using Gtk;
using logicpos.Extensions;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonUser : TouchButtonText
    {
        private Color _buttonColor;

        public TouchButtonUser(string pName)
            : base(pName)
        {
        }

        public TouchButtonUser(string pName, Color pColor, string pLabelText, string pFont, int pWidth, int pHeight, bool pLogged)
            : base(pName)
        {
            _buttonColor = pColor;
            InitObject(pName, pLabelText, pFont, pLogged);
            InitObject(pName, _buttonColor, _widget, pWidth, pHeight);
        }

        public void InitObject(string pName, string pLabelText, string pFont, bool pLogged)
        {
            _label = new Label(pLabelText);
            _widget = _label;

            if (pLogged)
            {
                ChangeFont(pFont, Color.Green);
                //Override default button color
                if (_buttonColor != Color.Transparent) _buttonColor = _buttonColor.Lighten(0.50F);
            }
            else
            {
                ChangeFont(pFont, Color.Black);
                //Override default button color
                if (_buttonColor != Color.Transparent) _buttonColor = _buttonColor.Darken(0.50F);
            }
        }
    }
}
