using System;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    //Non-UI - VirtualKey Position and Levels

    public class VirtualKey
    {
        public String Type { get; set; }
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public VirtualKeyProperties L1 { get; set; }
        public VirtualKeyProperties L2 { get; set; }
        public VirtualKeyProperties L3 { get; set; }
        public KeyboardPadKey UIKey { get; set; }

        public VirtualKey()
        {
            RowIndex = -1;
            ColIndex = -1;
            VirtualKeyProperties L1 = new VirtualKeyProperties();
            VirtualKeyProperties L2 = new VirtualKeyProperties();
            VirtualKeyProperties L3 = new VirtualKeyProperties();
        }
    }
}
