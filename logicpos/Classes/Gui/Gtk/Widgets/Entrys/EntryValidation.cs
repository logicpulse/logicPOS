using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.shared.App;
using LogicPOS.DTOs.Common;
using System;

namespace logicpos
{
    public class EntryValidation : EntryTouch
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Label Label { get; set; }
        public Label Label2 { get; set; }

        public Label Label3 { get; set; }
        public string LabelText { get; set; }

        public int Length { get; set; }
        public int Words { get; set; }

        public int Maxlength { get; set; } = 0;
        private int _maxWords = 0;
        public int MaxWords
        {
            get { return (_maxWords); }
            set { _maxWords = value; }
        }
        //Propertir from Entry(Gtk)
        private bool _validated;
        public bool Validated
        {
            get { return _validated; }
            set
            {
                _validated = value;
                Utils.ValidateUpdateColors(this, Label, _validated, Label2, Label3);
            }
        }

        //Constructor
        public EntryValidation()
        {
            try
            {
                Validate();
                _required = false;
                _keyboardMode = KeyboardMode.None;

                //Events
                Changed += delegate { Validate(); };

                //Get Initial Label Text
                if (Label != null) LabelText = Label.Text;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public EntryValidation(Window pSourceWindow, KeyboardMode pKeyboardMode, string pRule, bool pRequired)
            : this(pSourceWindow, pKeyboardMode, pRule, pRequired, 0, 0)
        {
        }

        public EntryValidation(Window pSourceWindow, KeyboardMode pKeyboardMode, string pRule, bool pRequired, int pMaxLength, int pMaxWords)
            : this()
        {
            _sourceWindow = pSourceWindow;
            _keyboardMode = pKeyboardMode;
            _rule = pRule;
            _required = pRequired;
            Maxlength = pMaxLength;
            _maxWords = pMaxWords;

            //Assign MaxLength to Be Entry MaxLength
            if (pMaxLength > 0) MaxLength = pMaxLength;
            //Events
            this.Changed += EntryValidation_Changed;
        }

        //Default FieldValidateValue is the Entry.Text
        public void Validate()
        {
            Validate(Text);
        }

        public void Validate(string pValue)
        {
            //Default FieldValidateValue is the Entry.Text
            _validated = SharedUtils.Validate(pValue, _rule, _required);
            Utils.ValidateUpdateColors(this, Label, _validated, Label2, Label3);
        }

        private void EntryValidation_Changed(object sender, EventArgs e)
        {
            if (Maxlength > 0 || _maxWords > 0)
            {
                ValidateMaxLenghtMaxWordsResult result = SharedUtils.ValidateMaxLenghtMaxWords(Text, LabelText, Maxlength, _maxWords);
                Text = result.Text;
                Label.Text = result.LabelText;
                Length = result.Length;
                Words = result.Words;
            }
        }
    }
}