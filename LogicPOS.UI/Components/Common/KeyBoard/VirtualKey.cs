namespace logicpos.Classes.Gui.Gtk.Widgets
{
    public class VirtualKey
    {
        public string Type { get; set; }
        public int Row { get; set; } = -1;
        public int Column { get; set; } = -1;
        public VirtualKeyProperties L1 { get; set; } = new VirtualKeyProperties();
        public VirtualKeyProperties L2 { get; set; } = new VirtualKeyProperties();
        public VirtualKeyProperties L3 { get; set; } = new VirtualKeyProperties();
        public KeyboardPadKey UIKey { get; set; }
    }
}
