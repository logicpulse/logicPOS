using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logicpos.Classes.Gui.Gtk.Widgets.BackOffice
{
    public class BOWidgetBox : VBox
    {
        private Label _label;
        public Label LabelComponent
        {
            get { return _label; }
            set { _label = value; }
        }
        private Widget _widget;
        public Widget WidgetComponent
        {
            get { return _widget; }
            set { _widget = value; }
        }

        public BOWidgetBox(string pLabel, Widget pWidget)
            : this(pLabel, pWidget, false, false)
        { 
        }

        public BOWidgetBox(string pLabel, Widget pWidget, bool pExpand, bool pFill)
            : base(false, 2)//Homogeneous False
        {
            _label = new Label(pLabel);
            _label.SetAlignment(0.0F, 0.0F);
            _widget = pWidget;

            PackStart(_label, false, false, 0);
            PackStart(pWidget, pExpand, pFill, 0);
        }
    }
}
