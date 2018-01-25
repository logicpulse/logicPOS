using Gtk;
using System;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonTemplate : TouchButtonBase
    {
        public Widget widget;

        public TouchButtonTemplate(String name) : base(name)
        {
        }

        public TouchButtonTemplate(String name, System.Drawing.Color color, int width, int height) : base(name)
        {
            InitObject(name, color, width, height);
            base.InitObject(name, color, widget, width, height);
        }

        public void InitObject(String name, System.Drawing.Color color, int width, int height)
        {
        }
    }
}
