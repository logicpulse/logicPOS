using System;
using Gtk;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class EntryBoxValidation : EntryBoxBase
    {
        //Public Properties
        private EntryValidation _entryValidation;
        public EntryValidation EntryValidation
        {
            get { return _entryValidation; }
            set { _entryValidation = value; }
        }

        public EntryBoxValidation(Window pSourceWindow, string pLabelText, KeyboardMode pKeyboardMode) 
            : this(pSourceWindow, pLabelText, pKeyboardMode, string.Empty, false)
        {
        }

        public EntryBoxValidation(Window pSourceWindow, string pLabelText, KeyboardMode pKeyboardMode, string pRule, bool pRequired, bool BOSource = false)
            : base(pSourceWindow, pLabelText, BOSource)
        {
            //EntryValidation
            _entryValidation = new EntryValidation(pSourceWindow, pKeyboardMode, pRule, pRequired) { Label = _label, LabelText = _label.Text, Label2 = _label2, Label3 = _label3 };
            _entryValidation.ModifyFont(_fontDescription);
            //Started Validate
            _entryValidation.Validate();
            
            //Pack
            _hbox.PackStart(_entryValidation, true, true, 0);
            //Init Keyboard
            if(!BOSource) InitKeyboard(_entryValidation);
        }
    }
}
