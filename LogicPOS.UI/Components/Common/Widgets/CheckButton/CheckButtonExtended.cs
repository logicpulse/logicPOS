using Gtk;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class CheckButtonExtended : CheckButton
    {
        public int Index { get; set; }

        public string Value { get; set; }

        public CheckButtonExtended(string pLabel) 
            : this (pLabel, pLabel) { }

        public CheckButtonExtended(string pLabel, string pValue) 
            : base (pLabel) { }
    }
}
