using System;
using Gtk;
using System.Drawing;
using logicpos.financial;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class ListComboBoxTouch : EntryBoxBase
    {
        //Public Properties
        private ListComboBox _listComboBox;
        public ListComboBox ListComboBox
        {
            get { return _listComboBox; }
            set { _listComboBox = value; }
        }

        public ListComboBoxTouch(Window pSourceWindow, string pLabelText, List<string> pItemList)
            : this(pSourceWindow, pLabelText, pItemList, string.Empty) { }

        public ListComboBoxTouch(Window pSourceWindow, string pLabelText, List<string> pItemList, string pInitialValue)
            : this(pSourceWindow, pLabelText, pItemList, pInitialValue, true) { }

        public ListComboBoxTouch(Window pSourceWindow, string pLabelText, List<string> pItemList, string pInitialValue, bool pAddUndefinedValue)
            : this(pSourceWindow, pLabelText, pItemList, pInitialValue, pAddUndefinedValue, false) { }

        public ListComboBoxTouch(Window pSourceWindow, string pLabelText, List<string> pItemList, string pInitialValue, bool pAddUndefinedValue, bool pRequired)
            : base(pSourceWindow, pLabelText)
        {
            //Entry
            _listComboBox = new ListComboBox(pItemList, pInitialValue, pAddUndefinedValue, pRequired);
            _listComboBox.ComboBoxCell.FontDesc = _fontDescription;

            //Pack
            Vbox.PackStart(_listComboBox);
        }
    }
}
