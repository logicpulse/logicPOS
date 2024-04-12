using Gtk;
using logicpos.Extensions;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonUser : TouchButtonText
    {
        private Color _buttonColor;

        public TouchButtonUser(String pName)
            : base(pName)
        {
        }

        public TouchButtonUser(String pName, Color pColor, String pLabelText, String pFont, int pWidth, int pHeight, Boolean pLogged)
            : base(pName)
        {
            _buttonColor = pColor;
            InitObject(pName, pLabelText, pFont, pLogged);
            base.InitObject(pName, _buttonColor, _widget, pWidth, pHeight);
        }

        public void InitObject(String pName, String pLabelText, String pFont, Boolean pLogged)
        {
            _label = new Label(pLabelText);
            _widget = _label;

            if (pLogged)
            {
                base.ChangeFont(pFont, Color.Green);
                //Override default button color
                if (_buttonColor != Color.Transparent) _buttonColor = _buttonColor.Lighten(0.50F);
            }
            else
            {
                base.ChangeFont(pFont, Color.Black);
                //Override default button color
                if (_buttonColor != Color.Transparent) _buttonColor = _buttonColor.Darken(0.50F);
            }
        }
    }
}
