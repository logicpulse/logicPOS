using Gtk;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    public abstract class ValidatableTextBoxBase : Entry
    {
        protected Window _sourceWindow;
        
        public KeyboardMode KeyboardMode { get; set; }
       
        public string Rule { get; set; }
  
        public bool Required { get; set; }
      
        public ValidatableTextBoxBase() { }
        public ValidatableTextBoxBase(Window window) : this(window, KeyboardMode.None, "", false) { }

        public ValidatableTextBoxBase(Window window, KeyboardMode keyboardMode, string rule, bool required)
        {
            _sourceWindow = window;
            KeyboardMode = keyboardMode;
            Rule = rule;
            Required = required;
        }
    }
}
