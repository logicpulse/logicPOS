using Gtk;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class EntryBoxValidation : EntryBoxBase
    {
        public ValidatableTextBox EntryValidation { get; set; }

        public EntryBoxValidation(Window parentWindow, string pLabelText, KeyboardMode pKeyboardMode) 
            : this(parentWindow, pLabelText, pKeyboardMode, string.Empty, false)
        {
        }

        public EntryBoxValidation(Window parentWindow, string pLabelText, KeyboardMode pKeyboardMode, string pRule, bool pRequired, bool BOSource = false)
            : base(parentWindow, pLabelText, BOSource)
        {
            //EntryValidation
            EntryValidation = new ValidatableTextBox(parentWindow, pKeyboardMode, pRule, pRequired) { Label = _label, LabelText = _label.Text, Label2 = _label2, Label3 = _label3 };
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
