using Gtk;
using logicpos.Classes.Enums.Keyboard;
using System;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    public class EntryMultiline : EventBox
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Parameters
        protected Window _sourceWindow;
        //Public Properties
        protected KeyboardMode _keyboardMode;
        public KeyboardMode KeyboardMode
        {
            get { return _keyboardMode; }
            set { _keyboardMode = value; }
        }
        private TextView _textView;
        public TextView TextView
        {
            get { return _textView; }
            set { _textView = value; }
        }
        private TextBuffer _value;
        public TextBuffer Value
        {
            get { return _value; }
            set { _value = value; }
        }
        private ScrolledWindow _scrolledWindow;
        public ScrolledWindow ScrolledWindow
        {
            get { return _scrolledWindow; }
            set { _scrolledWindow = value; }
        }

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
            _textView = new TextView();
            _textView.WrapMode = WrapMode.WordChar;
            //ScrolledWindow
            _scrolledWindow = new ScrolledWindow();
            _scrolledWindow.BorderWidth = 2;
            _scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Automatic);
            _scrolledWindow.Add(_textView);

            //Init Value Reference
            _value = _textView.Buffer;

            //Pack
            Add(_scrolledWindow);

            //Events
            _textView.Buffer.Changed += Buffer_Changed;
            _textView.KeyReleaseEvent += _textView_KeyReleaseEvent;
        }

        void Buffer_Changed(object sender, EventArgs e)
        {
            //Update value Reference
            _value = _textView.Buffer;
        }

        void _textView_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            //Prevent Enter Key to proceed to Dialog KeyReleaseEvent
            if (args.Event.Key.ToString().Equals("Return"))
            {
                args.RetVal = true;
            }
        }
    }
}
