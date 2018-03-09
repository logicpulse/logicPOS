using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using System;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class EntryBoxValidationButton : EntryBoxBase
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

        public EntryBoxValidationButton(Window pSourceWindow, String pLabelText, KeyboardMode pKeyboardMode, string pRule, bool pRequired, string pIconFile = "")
            : base(pSourceWindow, pLabelText)
        {
            //Settings
            string iconSelectRecord = FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], @"Icons/Windows/icon_window_select_record.png"));
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
            InitKeyboard(_entryValidation);
        }
    }
}
