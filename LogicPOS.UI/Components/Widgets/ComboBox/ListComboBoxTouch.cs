using Gtk;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class ListComboBoxTouch : EntryBoxBase
    {
        public ListComboBox ListComboBox { get; set; }

        public ListComboBoxTouch(Window parentWindow, string pLabelText, List<string> pItemList)
            : this(parentWindow, pLabelText, pItemList, string.Empty) { }

        public ListComboBoxTouch(Window parentWindow, string pLabelText, List<string> pItemList, string pInitialValue)
            : this(parentWindow, pLabelText, pItemList, pInitialValue, true) { }

        public ListComboBoxTouch(Window parentWindow, string pLabelText, List<string> pItemList, string pInitialValue, bool pAddUndefinedValue)
            : this(parentWindow, pLabelText, pItemList, pInitialValue, pAddUndefinedValue, false) { }

        public ListComboBoxTouch(Window parentWindow, string pLabelText, List<string> pItemList, string pInitialValue, bool pAddUndefinedValue, bool pRequired)
            : base(parentWindow, pLabelText)
        {
            //Entry
            ListComboBox = new ListComboBox(pItemList, pInitialValue, pAddUndefinedValue, pRequired);
            ListComboBox.ComboBoxCell.FontDesc = _fontDescription;

            //Pack
            Vbox.PackStart(ListComboBox);
        }
    }
}
