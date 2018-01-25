using Gtk;
using System;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class CheckButtonExtended : CheckButton
    {
        private int _index;
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public CheckButtonExtended(string pLabel) 
            : this (pLabel, pLabel) { }

        public CheckButtonExtended(string pLabel, string pValue) 
            : base (pLabel) { }
    }
}
