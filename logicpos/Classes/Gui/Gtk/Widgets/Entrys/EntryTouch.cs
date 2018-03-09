using Gtk;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    /// <summary>
    /// Just a simple entry that Call Keyboard
    /// EntryBox and EntryBoxValidation
    /// </summary>
    public abstract class EntryTouch : Entry
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Parameters
        protected Window _sourceWindow;
        //Public
        protected KeyboardMode _keyboardMode;
        public KeyboardMode KeyboardMode
        {
            get { return _keyboardMode; }
            set { _keyboardMode = value; }
        }
        protected string _rule;
        public string Rule
        {
            get { return _rule; }
            set { _rule = value; }
        }
        protected bool _required;
        public bool Required
        {
            get { return _required; }
            set { _required = value; }
        }

        //Constructor
        public EntryTouch() { }
        public EntryTouch(Window pSourceWindow) : this(pSourceWindow, KeyboardMode.None, "", false) { }
        public EntryTouch(Window pSourceWindow, KeyboardMode pKeyboardMode, string pRule, bool pRequired)
        {
            _sourceWindow = pSourceWindow;
            _keyboardMode = pKeyboardMode;
            _rule = pRule;
            _required = pRequired;
        }
    }
}
