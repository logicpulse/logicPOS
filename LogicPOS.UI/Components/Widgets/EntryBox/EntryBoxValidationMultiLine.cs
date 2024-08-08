using Gtk;
using logicpos.Classes.Enums.Keyboard;
using LogicPOS.DTOs.Common;
using LogicPOS.UI;
using LogicPOS.Utility;
using System;

namespace logicpos.Classes.Gui.Gtk.Widgets.Entrys
{
    internal class EntryBoxValidationMultiLine : EntryBoxBase
    {

        private readonly string _initialLabelText;

        public string Rule { get; set; }
        public bool Required { get; set; }
        public int Length { get; set; }
        public int Words { get; set; }
        public int MaxWords { get; set; }
        public int MaxLength { get; set; }

        protected bool _validated;
        public bool Validated
        {
            get { return _validated; }
            set
            {
                _validated = value;
                GtkUtils.UpdateWidgetColorsAfterValidation(this,  _validated, _label, _label2, _label3);
            }
        }

        public MultilineTextBox EntryMultiline { get; set; }

        public EntryBoxValidationMultiLine(Window parentWindow, string pLabelText)
            : this(parentWindow, pLabelText, KeyboardMode.None, string.Empty, false)
        {
        }

        public EntryBoxValidationMultiLine(Window parentWindow, string pLabelText, KeyboardMode pKeyboardMode, string pRule, bool pRequired)
            : this(parentWindow, pLabelText, pKeyboardMode, pRule, pRequired, 0, 0)
        {
        }

        public EntryBoxValidationMultiLine(Window parentWindow, string pLabelText, KeyboardMode pKeyboardMode, string pRule, bool pRequired, int pMaxLength, int pMaxWords)
            : base(parentWindow, pLabelText)
        {
            //Parameters
            _initialLabelText = pLabelText;

            EntryMultiline = new MultilineTextBox(parentWindow, pKeyboardMode) { BorderWidth = 2 };
            EntryMultiline.TextView.WrapMode = WrapMode.WordChar;
            EntryMultiline.TextView.ModifyFont(_fontDescription);

            Rule = pRule;
            Required = pRequired;
            if (pMaxLength > 0) MaxLength = pMaxLength;
            if (pMaxWords > 0) MaxWords = pMaxWords;

            //Pack It
            _hbox.PackStart(EntryMultiline, true, true, 0);
            //Init Keyboard
            InitKeyboard(EntryMultiline);
            //Started Validate
            Validate();

            //Event here to have access to Label
            EntryMultiline.Value.Changed += Value_Changed;
        }

        private void Value_Changed(object sender, EventArgs e)
        {
            if (MaxLength > 0 || MaxWords > 0)
            {
                ValidateMaxLenghtMaxWordsResult result = GeneralUtils.ValidateMaxLenghtMaxWords(EntryMultiline.Value.Text, _initialLabelText, MaxLength, MaxWords);
                //Only update if changes else infinite loop
                if (EntryMultiline.Value.Text != result.Text) EntryMultiline.Value.Text = result.Text;
                Label.Text = result.LabelText;
                Length = result.Length;
                Words = result.Words;
            }

            //Now we can validate after ValidateMaxLenghtMaxWords Work
            Validate();
        }

        //Default FieldValidateValue is the Entry.Text
        public void Validate()
        {
            Validate(EntryMultiline.Value.Text);
        }

        public void Validate(string pValue)
        {
            //Default FieldValidateValue is the Entry.Text
            Validated = GeneralUtils.Validate(pValue, Rule, Required);
            GtkUtils.UpdateWidgetColorsAfterValidation(this,  _validated, _label, _label2, _label3);
        }
    }
}
