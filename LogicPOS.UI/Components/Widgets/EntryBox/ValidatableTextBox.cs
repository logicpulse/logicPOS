using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.DTOs.Common;
using LogicPOS.UI;
using LogicPOS.Utility;
using System;

namespace logicpos
{
    public class ValidatableTextBox : ValidatableTextBoxBase
    {
        public Label Label { get; set; }
        public Label Label2 { get; set; }
        public Label Label3 { get; set; }
        public string LabelText { get; set; }

        public int Length { get; set; }
        public int Words { get; set; }

        public int Maxlength { get; set; } = 0;
        private int _maxWords = 0;

        private bool _validated;
        public bool Validated
        {
            get { return _validated; }
            set
            {
                _validated = value;
                GtkUtils.UpdateWidgetColorsAfterValidation(this, _validated, Label, Label2, Label3);
            }
        }

        //Constructor
        public ValidatableTextBox()
        {
            Validate();
            Required = false;
            KeyboardMode = KeyboardMode.None;

            //Events
            Changed += delegate { Validate(); };

            //Get Initial Label Text
            if (Label != null) LabelText = Label.Text;


        }

        public ValidatableTextBox(Window parentWindow,
                                  KeyboardMode pKeyboardMode,
                                  string pRule,
                                  bool pRequired)
            : this(parentWindow,
                   pKeyboardMode,
                   pRule,
                   pRequired,
                   0,
                   0)
        {
        }

        public ValidatableTextBox(Window parentWindow, KeyboardMode pKeyboardMode, string pRule, bool pRequired, int pMaxLength, int pMaxWords)
            : this()
        {
            _sourceWindow = parentWindow;
            KeyboardMode = pKeyboardMode;
            Rule = pRule;
            Required = pRequired;
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
            _validated = GeneralUtils.Validate(pValue, Rule, Required);
            GtkUtils.UpdateWidgetColorsAfterValidation(this, _validated, Label, Label2, Label3);
        }

        private void EntryValidation_Changed(object sender, EventArgs e)
        {
            if (Maxlength > 0 || _maxWords > 0)
            {
                ValidateMaxLenghtMaxWordsResult result = GeneralUtils.ValidateMaxLenghtMaxWords(Text, LabelText, Maxlength, _maxWords);
                Text = result.Text;
                Label.Text = result.LabelText;
                Length = result.Length;
                Words = result.Words;
            }
        }
    }
}