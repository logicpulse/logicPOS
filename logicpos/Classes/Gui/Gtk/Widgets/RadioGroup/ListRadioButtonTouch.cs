using Gtk;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class ListRadioButtonTouch : EntryBoxBase
    {
        //Public Properties
        private ListRadioButton _listRadioButton;
        public ListRadioButton ListRadioButton
        {
            get { return _listRadioButton; }
            set { _listRadioButton = value; }
        }

        public ListRadioButtonTouch(Window pSourceWindow, string pLabelText, List<string> pItemList)
            : this(pSourceWindow, pLabelText, pItemList, string.Empty) { }

        public ListRadioButtonTouch(Window pSourceWindow, string pLabelText, List<string> pItemList, string pInitialValue)
            : this(pSourceWindow, pLabelText, pItemList, string.Empty, false) { }

        public ListRadioButtonTouch(Window pSourceWindow, string pLabelText, List<string> pItemList, string pInitialValue, bool pRequired)
            : base(pSourceWindow, pLabelText)
        {
            _listRadioButton = new ListRadioButton(pItemList, pInitialValue);
            for (int i = 0; i < _listRadioButton.RadioButtonList.Count; i++)
            {
                _listRadioButton.RadioButtonList[i].Child.ModifyFont(_fontDescription);
            }

            //Pack
            EventBox evbox = new EventBox();
            evbox.Add(_listRadioButton);
            evbox.Add(Vbox);
            Vbox.BorderWidth = 5;
            Vbox.PackStart(evbox);
        }
    }
}
