using DevExpress.Data.Filtering;
using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using System;
using System.ComponentModel;

namespace logicpos.Classes.Gui.Gtk.WidgetsXPO
{
    class XPOEntryBoxSelectRecordValidation<T1, T2> : XPOEntryBoxSelectRecord<T1, T2>
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

        //Public Fields
        private EntryValidation _entryValidation;
        public EntryValidation EntryValidation
        {
            get { return _entryValidation; }
            set { _entryValidation = value; }
        }
		//Artigos Compostos [IN:016522]
        private EntryValidation _entryCodeValidation;
        public EntryValidation EntryCodeValidation
        {
            get { return _entryCodeValidation; }
            set { _entryCodeValidation = value; }
        }

        private EntryValidation _qtdEntryValidation;
        public EntryValidation EntryQtdValidation
        {
            get { return _qtdEntryValidation; }
            set { _qtdEntryValidation = value; }
        }

        private int _entryNumber;
        public int EntryNumber
        {
            get { return _entryNumber; }
            set { _entryNumber = value; }
        }

        private fin_article _article;
        public fin_article Article
        {
            get { return _article; }
            set { _article = value; }
        }

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
            _entryNumber = pEntryNumber;

            //Entry: Required to Assign BaseClass _label Reference to EntryValidation.Label :)
            _entryValidation = new EntryValidation(pSourceWindow, pKeyboardMode, pRule, pRequired) { Label = _label, Label2 = _label2, Label3 = _label3 };

            //Start Validated
            _entryValidation.Validate(GetValue(pFieldValidateValue));

            //Always validate when we Change Values
            _entryValidation.Changed += delegate
            {
                _entryValidation.Validate(GetValue(pFieldValidateValue));
            };
			//Artigos Compostos [IN:016522]
            if (pBOSource && pEntryNumber > 0)
            {
                _entryCodeValidation = new EntryValidation(pSourceWindow, pKeyboardMode, pFieldValidateValueCode, pRequired) { Label = _label, Label2 = _label2, Label3 = _label3 };

                _qtdEntryValidation = new EntryValidation(pSourceWindow, KeyboardMode.None, pFieldValidateValueQtd, pRequired) { Label = _label, Label2 = _label2, Label3 = _label3 };

                InitEntryBOSource(_entryCodeValidation,_entryValidation, _qtdEntryValidation);                
            }
            else
            {
                InitEntry(_entryValidation);
            }
            
            //Init Keyboard
            InitKeyboard(_entryValidation);
        }

        //Work in Keyboard Mode or XPOObject Mode
        private string GetValue(string pFieldValidateValue)
        {
            //Work in Keyboard Mode or XPOObject Mode
            if (_entryValidation.KeyboardMode != KeyboardMode.None)
            {
                return _entryValidation.Text;
            }
            else
            {
                return _fieldValidateValue = GetFieldValidateValue(pFieldValidateValue);
            }
        }
    }
}
