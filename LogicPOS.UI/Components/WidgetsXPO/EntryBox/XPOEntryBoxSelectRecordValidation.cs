using DevExpress.Data.Filtering;
using Gtk;
using logicpos.Classes.Enums.Keyboard;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Components;
using System;
using System.ComponentModel;

namespace logicpos.Classes.Gui.Gtk.WidgetsXPO
{
    internal class XPOEntryBoxSelectRecordValidation<T1, T2> : XPOEntryBoxSelectRecord<T1, T2>
        //Generic Type T1 Constrained to XPGuidObject BaseClass or XPGuidObject SubClass Objects (New)
        where T1 : Entity, new()
        //Generic Type T2 Constrained to GenericTreeViewXPO BaseClass or GenericTreeViewXPO SubClass Objects (New)
        where T2 : XpoGridView, new()
    {
        //Hiding inherited member Entry in Favor of EntryValidation, to be unifore with EntryBoxValiadtion :)
        [Obsolete("This property is not supported in this class", true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Entry Entry { get; set; }
        public ValidatableTextBox EntryValidation { get; set; }
        public ValidatableTextBox EntryCodeValidation { get; set; }

        public ValidatableTextBox EntryQtdValidation { get; set; }

        public int EntryNumber { get; set; }

        public fin_article Article { get; set; }

        //Constructor/OverLoads
        public XPOEntryBoxSelectRecordValidation(Window parentWindow, string pLabelText, string pFieldDisplayValue, string pRule)
            : this(parentWindow, pLabelText, pFieldDisplayValue, string.Empty, pRule, false) { }

        public XPOEntryBoxSelectRecordValidation(Window parentWindow, string pLabelText, string pFieldDisplayValue, string pFieldValidateValue, string pRule, bool pRequired)
            : this(parentWindow, pLabelText, pFieldDisplayValue, pFieldValidateValue, null, pRule, pRequired) { }

        public XPOEntryBoxSelectRecordValidation(Window parentWindow, string pLabelText, string pFieldDisplayValue, string pFieldValidateValue, T1 pCurrentValue, string pRule, bool pRequired)
            : this(parentWindow, pLabelText, pFieldDisplayValue, pFieldValidateValue, pCurrentValue, null, pRule, pRequired) { }

        public XPOEntryBoxSelectRecordValidation(Window parentWindow, string pLabelText, string pFieldDisplayValue, string pFieldValidateValue, T1 pCurrentValue, CriteriaOperator pCriteriaOperator, string pRule, bool pRequired)
            : this(parentWindow, pLabelText, pFieldDisplayValue, pFieldValidateValue, pCurrentValue, pCriteriaOperator, KeyboardMode.None, pRule, pRequired) { }

        public XPOEntryBoxSelectRecordValidation(Window parentWindow, string pLabelText, string pFieldDisplayValue, string pFieldValidateValue, T1 pCurrentValue, CriteriaOperator pCriteriaOperator, string pRule, bool pRequired, bool pBOSource)
           : this(parentWindow, pLabelText, pFieldDisplayValue, pFieldValidateValue, pCurrentValue, pCriteriaOperator, KeyboardMode.None, pRule, pRequired, pBOSource) { }

        public XPOEntryBoxSelectRecordValidation(Window parentWindow, string pLabelText, string pFieldDisplayValue, string pFieldValidateValue, T1 pCurrentValue, CriteriaOperator pCriteriaOperator, KeyboardMode pKeyboardMode, string pRule, bool pRequired, bool pBOSource = false, string pFieldValidateValueCode = "", string pFieldValidateValueQtd = "", int pEntryNumber = 0)
            : base(parentWindow, pLabelText, pFieldDisplayValue, pFieldValidateValue, pCurrentValue, pCriteriaOperator, pBOSource)
        {
            EntryNumber = pEntryNumber;

            //Entry: Required to Assign BaseClass _label Reference to EntryValidation.Label :)
            EntryValidation = new ValidatableTextBox(parentWindow, pKeyboardMode, pRule, pRequired) { Label = _label, Label2 = _label2, Label3 = _label3 };

            //Start Validated
            EntryValidation.Validate(GetValue(pFieldValidateValue));

            //Always validate when we Change Values
            EntryValidation.Changed += delegate
            {
                EntryValidation.Validate(GetValue(pFieldValidateValue));
            };
			//Artigos Compostos [IN:016522]
            if (pBOSource && pEntryNumber > 0)
            {
                EntryCodeValidation = new ValidatableTextBox(parentWindow, pKeyboardMode, pFieldValidateValueCode, pRequired) { Label = _label, Label2 = _label2, Label3 = _label3 };

                EntryQtdValidation = new ValidatableTextBox(parentWindow, KeyboardMode.None, pFieldValidateValueQtd, pRequired) { Label = _label, Label2 = _label2, Label3 = _label3 };

                InitEntryBOSource(EntryCodeValidation,EntryValidation, EntryQtdValidation);                
            }
            else
            {
                InitEntry(EntryValidation);
            }
            
            //Init Keyboard
            InitKeyboard(EntryValidation);
        }

        //Work in Keyboard Mode or XPOObject Mode
        private string GetValue(string pFieldValidateValue)
        {
            //Work in Keyboard Mode or XPOObject Mode
            if (EntryValidation.KeyboardMode != KeyboardMode.None)
            {
                return EntryValidation.Text;
            }
            else
            {
                return _fieldValidateValue = GetFieldValidateValue(pFieldValidateValue);
            }
        }
    }
}
