using System;
using Gtk;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class EntryBoxValidation : EntryBoxBase
    {
        //Public Properties
        private EntryValidation _entryValidation;
        public EntryValidation EntryValidation
        {
            get { return _entryValidation; }
            set { _entryValidation = value; }
        }

        public EntryBoxValidation(Window pSourceWindow, String pLabelText, KeyboardMode pKeyboardMode) 
            : this(pSourceWindow, pLabelText, pKeyboardMode, string.Empty, false)
        {
        }

        public EntryBoxValidation(Window pSourceWindow, String pLabelText, KeyboardMode pKeyboardMode, string pRule, bool pRequired)
            : base(pSourceWindow, pLabelText)
        {
            //EntryValidation
            _entryValidation = new EntryValidation(pSourceWindow, pKeyboardMode, pRule, pRequired) { Label = _label, LabelText = _label.Text };
            _entryValidation.ModifyFont(_fontDescription);
            //Started Validate
            _entryValidation.Validate();
            
            //Pack
            _hbox.PackStart(_entryValidation, true, true, 0);
            //Init Keyboard
            InitKeyboard(_entryValidation);
        }
    }
}
