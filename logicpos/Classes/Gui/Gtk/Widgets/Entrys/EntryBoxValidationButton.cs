using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.App;
using logicpos.shared.App;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class EntryBoxValidationButton : EntryBoxBase
    {
        protected EntryValidation _entryValidation;
        public EntryValidation EntryValidation
        {
            get { return _entryValidation; }
            set { _entryValidation = value; }
        }
        protected TouchButtonIcon _button;
        public TouchButtonIcon Button
        {
            get { return _button; }
            set { _button = value; }
        }

        public EntryBoxValidationButton(Window pSourceWindow, string pLabelText, KeyboardMode pKeyboardMode, string pRule, bool pRequired, string pIconFile = "", bool pBOSource = false)
            : base(pSourceWindow, pLabelText, pBOSource)
        {
            //Settings
            string iconSelectRecord = string.Format("{0}{1}", DataLayerFramework.Path["images"], @"Icons/Windows/icon_window_select_record.png");
            string iconFile = (pIconFile != string.Empty) ? pIconFile : iconSelectRecord;

            //Init Button
            _button = GetButton(iconFile);

            //EntryValidation
            _entryValidation = new EntryValidation(pSourceWindow, pKeyboardMode, pRule, pRequired) { Label = _label, LabelText = _label.Text };
            _entryValidation.ModifyFont(_fontDescription);
            //Started Validate
            _entryValidation.Validate();

            //Pack Hbox
            _hbox.PackStart(_entryValidation, true, true, 0);
            _hbox.PackStart(_button, false, false, 0);
            //Init Keyboard
            if (!pBOSource) InitKeyboard(_entryValidation);
        }
    }
}
