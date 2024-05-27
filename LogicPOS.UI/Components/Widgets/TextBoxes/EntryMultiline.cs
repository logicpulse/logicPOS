using Gtk;
using logicpos.Classes.Enums.Keyboard;
using System;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    public class EntryMultiline : EventBox
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Parameters
        protected Window _sourceWindow;
        //Public Properties
        protected KeyboardMode _keyboardMode;
        public KeyboardMode KeyboardMode
        {
            get { return _keyboardMode; }
            set { _keyboardMode = value; }
        }

        public TextView TextView { get; set; }
        public TextBuffer Value { get; set; }

        public ScrolledWindow ScrolledWindow { get; set; }

        //Constructor
        public EntryMultiline()
            : this(null) { }
        public EntryMultiline(Window pSourceWindow) 
            : this(pSourceWindow, KeyboardMode.None) { }
        public EntryMultiline(Window pSourceWindow, KeyboardMode pKeyboardMode)
        {
            _sourceWindow = pSourceWindow;
            _keyboardMode = pKeyboardMode;

            //Init
            TextView = new TextView();
            TextView.WrapMode = WrapMode.WordChar;
            //ScrolledWindow
            ScrolledWindow = new ScrolledWindow();
            ScrolledWindow.BorderWidth = 2;
            ScrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Automatic);
            ScrolledWindow.Add(TextView);

            //Init Value Reference
            Value = TextView.Buffer;

            //Pack
            Add(ScrolledWindow);

            //Events
            TextView.Buffer.Changed += Buffer_Changed;
            TextView.KeyReleaseEvent += _textView_KeyReleaseEvent;
        }

        private void Buffer_Changed(object sender, EventArgs e)
        {
            //Update value Reference
            Value = TextView.Buffer;
        }

        private void _textView_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            //Prevent Enter Key to proceed to Dialog KeyReleaseEvent
            if (args.Event.Key.ToString().Equals("Return"))
            {
                args.RetVal = true;
            }
        }
    }
}
