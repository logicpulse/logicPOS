using System;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    //Non-UI - VirtualKey Properties

    public class VirtualKeyProperties
    {
        public String Glyph { get; set; }
        public String IbmId { get; set; }
        public bool IsDeadKey { get; set; }
        public String Diacritical { get; set; }
        public bool IsNotEngraved { get; set; }
        public String CharacterName { get; set; }
        public String UnicodeId { get; set; }
        public int KeyWidth { get; set; }
        public bool IsNumPad { get; set; }
        public bool HideL2 { get; set; }
        public bool IsBold { get; set; }
        public String HAlign { get; set; }
    }
}
