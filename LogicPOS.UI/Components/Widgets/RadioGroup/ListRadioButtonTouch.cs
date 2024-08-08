using Gtk;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class ListRadioButtonTouch : EntryBoxBase
    {
        public ListRadioButton ListRadioButton { get; set; }

        public ListRadioButtonTouch(Window parentWindow, string pLabelText, List<string> pItemList)
            : this(parentWindow, pLabelText, pItemList, string.Empty) { }

        public ListRadioButtonTouch(Window parentWindow, string pLabelText, List<string> pItemList, string pInitialValue)
            : this(parentWindow, pLabelText, pItemList, string.Empty, false) { }

        public ListRadioButtonTouch(Window parentWindow, string pLabelText, List<string> pItemList, string pInitialValue, bool pRequired)
            : base(parentWindow, pLabelText)
        {
            ListRadioButton = new ListRadioButton(pItemList, pInitialValue);
            for (int i = 0; i < ListRadioButton.RadioButtonList.Count; i++)
            {
                ListRadioButton.RadioButtonList[i].Child.ModifyFont(_fontDescription);
            }

            //Pack
            EventBox evbox = new EventBox
            {
                ListRadioButton,
                Vbox
            };
            Vbox.BorderWidth = 5;
            Vbox.PackStart(evbox);
        }
    }
}
