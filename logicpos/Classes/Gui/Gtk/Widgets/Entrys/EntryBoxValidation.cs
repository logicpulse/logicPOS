using Gtk;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class EntryBoxValidation : EntryBoxBase
    {
        public EntryValidation EntryValidation { get; set; }

        public EntryBoxValidation(Window pSourceWindow, string pLabelText, KeyboardMode pKeyboardMode) 
            : this(pSourceWindow, pLabelText, pKeyboardMode, string.Empty, false)
        {
        }

        public EntryBoxValidation(Window pSourceWindow, string pLabelText, KeyboardMode pKeyboardMode, string pRule, bool pRequired, bool BOSource = false)
            : base(pSourceWindow, pLabelText, BOSource)
        {
            //EntryValidation
            EntryValidation = new EntryValidation(pSourceWindow, pKeyboardMode, pRule, pRequired) { Label = _label, LabelText = _label.Text, Label2 = _label2, Label3 = _label3 };
            EntryValidation.ModifyFont(_fontDescription);
            //Started Validate
            EntryValidation.Validate();
            
            //Pack
            _hbox.PackStart(EntryValidation, true, true, 0);
            //Init Keyboard
            if(!BOSource) InitKeyboard(EntryValidation);
        }
    }
}
