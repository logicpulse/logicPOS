using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Keyboard;
using logicpos.shared.Classes.Others;
using System;

namespace logicpos.Classes.Gui.Gtk.Widgets.Entrys
{
    class EntryBoxValidationMultiLine : EntryBoxBase
    {
        //Private Properties
        private string _initialLabelText;
        //Public
        protected string _rule;
        public string Rule
        {
            get { return _rule; }
            set { _rule = value; }
        }
        //Public Properties
        protected bool _required;
        public bool Required
        {
            get { return _required; }
            set { _required = value; }
        }
        protected int _length;
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }
        protected int _words;
        public int Words
        {
            get { return _words; }
            set { _words = value; }
        } 
        protected int _maxWords = 0;
        public int MaxWords
        {
            get { return (_maxWords); }
            set { _maxWords = value; }
        }
        protected int _maxLength = 0;
        public int MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; }
        }
        protected bool _validated;
        public bool Validated
        {
            get { return _validated; }
            set { 
                _validated = value; 
                Utils.ValidateUpdateColors(this, _label, _validated);
            }
        }
        //Public Properties
        private EntryMultiline _entryMultiline;
        public EntryMultiline EntryMultiline
        {
            get { return _entryMultiline; }
            set { _entryMultiline = value; }
        }

        public EntryBoxValidationMultiLine(Window pSourceWindow, string pLabelText)
            : this(pSourceWindow, pLabelText, KeyboardMode.None, string.Empty, false)
        {
        }

        public EntryBoxValidationMultiLine(Window pSourceWindow, string pLabelText, KeyboardMode pKeyboardMode, string pRule, bool pRequired)
            : this(pSourceWindow, pLabelText, pKeyboardMode, pRule, pRequired, 0, 0)
        {
        }

        public EntryBoxValidationMultiLine(Window pSourceWindow, string pLabelText, KeyboardMode pKeyboardMode, string pRule, bool pRequired, int pMaxLength, int pMaxWords)
            : base(pSourceWindow, pLabelText)
        { 
            //Parameters
            _initialLabelText = pLabelText;

            _entryMultiline = new EntryMultiline(pSourceWindow, pKeyboardMode) { BorderWidth = 2 };
            _entryMultiline.TextView.WrapMode = WrapMode.WordChar;
            _entryMultiline.TextView.ModifyFont(_fontDescription);

            _rule = pRule;
            _required = pRequired;
            if (pMaxLength > 0) _maxLength = pMaxLength;
            if (pMaxWords > 0) _maxWords = pMaxWords;

            //Pack It
            _hbox.PackStart(_entryMultiline, true, true, 0);
            //Init Keyboard
            InitKeyboard(_entryMultiline);
            //Started Validate
            Validate();

            //Event here to have access to Label
            _entryMultiline.Value.Changed += Value_Changed;
        }

        void Value_Changed(object sender, EventArgs e)
        {
            if (_maxLength > 0 || _maxWords > 0)
            {
                ValidateMaxLenghtMaxWordsResult result = FrameworkUtils.ValidateMaxLenghtMaxWords(_entryMultiline.Value.Text, _initialLabelText, _maxLength, _maxWords);
                //Only update if changes else infinite loop
                if (_entryMultiline.Value.Text != result.Text) _entryMultiline.Value.Text = result.Text;
                Label.Text = result.LabelText;
                _length = result.Length;
                _words = result.Words;
            }

            //Now we can validate after ValidateMaxLenghtMaxWords Work
            Validate();
        }

        //Default FieldValidateValue is the Entry.Text
        public void Validate()
        {
            Validate(_entryMultiline.Value.Text);
        }

        public void Validate(string pValue)
        {
            //Default FieldValidateValue is the Entry.Text
            Validated = FrameworkUtils.Validate(pValue, _rule, _required);
            Utils.ValidateUpdateColors(this, _label, _validated);
        }
    }
}
