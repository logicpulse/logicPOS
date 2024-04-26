using DevExpress.Data.Filtering;
using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using System;
using System.ComponentModel;

namespace logicpos.Classes.Gui.Gtk.WidgetsXPO
{
    internal class XPOEntryBoxSelectRecordValidation<T1, T2> : XPOEntryBoxSelectRecord<T1, T2>
        //Generic Type T1 Constrained to XPGuidObject BaseClass or XPGuidObject SubClass Objects (New)
        where T1 : XPGuidObject, new()
        //Generic Type T2 Constrained to GenericTreeViewXPO BaseClass or GenericTreeViewXPO SubClass Objects (New)
        where T2 : GenericTreeViewXPO, new()
    {
        //Hiding inherited member Entry in Favor of EntryValidation, to be unifore with EntryBoxValiadtion :)
        [Obsolete("This property is not supported in this class", true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Entry Entry { get; set; }
        public EntryValidation EntryValidation { get; set; }
        public EntryValidation EntryCodeValidation { get; set; }

        public EntryValidation EntryQtdValidation { get; set; }

        public int EntryNumber { get; set; }

        public fin_article Article { get; set; }

        //Constructor/OverLoads
        public XPOEntryBoxSelectRecordValidation(Window pSourceWindow, string pLabelText, string pFieldDisplayValue, string pRule)
            : this(pSourceWindow, pLabelText, pFieldDisplayValue, string.Empty, pRule, false) { }

        public XPOEntryBoxSelectRecordValidation(Window pSourceWindow, string pLabelText, string pFieldDisplayValue, string pFieldValidateValue, string pRule, bool pRequired)
            : this(pSourceWindow, pLabelText, pFieldDisplayValue, pFieldValidateValue, null, pRule, pRequired) { }

        public XPOEntryBoxSelectRecordValidation(Window pSourceWindow, string pLabelText, string pFieldDisplayValue, string pFieldValidateValue, T1 pCurrentValue, string pRule, bool pRequired)
            : this(pSourceWindow, pLabelText, pFieldDisplayValue, pFieldValidateValue, pCurrentValue, null, pRule, pRequired) { }

        public XPOEntryBoxSelectRecordValidation(Window pSourceWindow, string pLabelText, string pFieldDisplayValue, string pFieldValidateValue, T1 pCurrentValue, CriteriaOperator pCriteriaOperator, string pRule, bool pRequired)
            : this(pSourceWindow, pLabelText, pFieldDisplayValue, pFieldValidateValue, pCurrentValue, pCriteriaOperator, KeyboardMode.None, pRule, pRequired) { }

        public XPOEntryBoxSelectRecordValidation(Window pSourceWindow, string pLabelText, string pFieldDisplayValue, string pFieldValidateValue, T1 pCurrentValue, CriteriaOperator pCriteriaOperator, string pRule, bool pRequired, bool pBOSource)
           : this(pSourceWindow, pLabelText, pFieldDisplayValue, pFieldValidateValue, pCurrentValue, pCriteriaOperator, KeyboardMode.None, pRule, pRequired, pBOSource) { }

        public XPOEntryBoxSelectRecordValidation(Window pSourceWindow, string pLabelText, string pFieldDisplayValue, string pFieldValidateValue, T1 pCurrentValue, CriteriaOperator pCriteriaOperator, KeyboardMode pKeyboardMode, string pRule, bool pRequired, bool pBOSource = false, string pFieldValidateValueCode = "", string pFieldValidateValueQtd = "", int pEntryNumber = 0)
            : base(pSourceWindow, pLabelText, pFieldDisplayValue, pFieldValidateValue, pCurrentValue, pCriteriaOperator, pBOSource)
        {
            EntryNumber = pEntryNumber;

            //Entry: Required to Assign BaseClass _label Reference to EntryValidation.Label :)
            EntryValidation = new EntryValidation(pSourceWindow, pKeyboardMode, pRule, pRequired) { Label = _label, Label2 = _label2, Label3 = _label3 };

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
                EntryCodeValidation = new EntryValidation(pSourceWindow, pKeyboardMode, pFieldValidateValueCode, pRequired) { Label = _label, Label2 = _label2, Label3 = _label3 };

                EntryQtdValidation = new EntryValidation(pSourceWindow, KeyboardMode.None, pFieldValidateValueQtd, pRequired) { Label = _label, Label2 = _label2, Label3 = _label3 };

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
