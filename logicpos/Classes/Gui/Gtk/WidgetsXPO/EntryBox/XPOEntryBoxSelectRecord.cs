using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;
using System;
using System.Drawing;
using System.Reflection;

//Note: This component dont have Validation, is used to be the Base XPOEntryBoxSelectRecordValidation 
//and to be used with CrudWidgetList Validation

namespace logicpos.Classes.Gui.Gtk.WidgetsXPO
{
    //Genertic Types T1:XPGuidObject Childs (Ex Customer), T2:GenericTreeView Childs (ex TreeViewConfigurationCountry)
    class XPOEntryBoxSelectRecord<T1, T2> : EntryBoxBase
        //Generic Type T1 Constrained to XPGuidObject BaseClass or XPGuidObject SubClass Objects (New)
        where T1 : XPGuidObject, new()
        //Generic Type T2 Constrained to GenericTreeView BaseClass or GenericTreeView SubClass Objects (New)
        where T2 : GenericTreeViewXPO, new()
    {
        //Param: Optional: Used to get Display value to Show to user, ex Default is "Designation"
        protected string _fieldDisplayValue;
        //Param: Used to get the Validated Value, this way we can Validate Oids and Other Non Text Entry Values
        protected string _fieldValidateValue;
        //Param: XPGuidObject Value
        protected T1 _value;
        public T1 Value
        {
            get { return _value; }
            set { _value = value; }
        }
        //Used to store Previous value, before change
        protected T1 _previousValue;
        public T1 PreviousValue
        {
            get { return _previousValue; }
            set { _previousValue = value; }
        }
        //UI
        private TouchButtonIcon _buttonSelectValue;
        public TouchButtonIcon ButtonSelectValue
        {
            get { return _buttonSelectValue; }
            set { _buttonSelectValue = value; }
        }
        //Custom Events
        public event EventHandler OpenPopup;
        public event EventHandler ClosePopup;
        //Param: Optional 
        private CriteriaOperator _criteriaOperator;
        public CriteriaOperator CriteriaOperator
        {
            get { return _criteriaOperator; }
            set { _criteriaOperator = value; }
        }
        //Defaults
        private Size _dialogSize;

        //Public Properties
        private Entry _entry;
        public Entry Entry
        {
            get { return _entry; }
            set { _entry = value; }
        }

        //Constructor/OverLoads
        public XPOEntryBoxSelectRecord(Window pSourceWindow, String pLabelText)
            : this(pSourceWindow, pLabelText, string.Empty, string.Empty) { }

        public XPOEntryBoxSelectRecord(Window pSourceWindow, String pLabelText, String pFieldDisplayValue, String pFieldValidateValue)
            : this(pSourceWindow, pLabelText, pFieldDisplayValue, pFieldDisplayValue, null) { }

        public XPOEntryBoxSelectRecord(Window pSourceWindow, String pLabelText, String pFieldDisplayValue, String pFieldValidateValue, T1 pValue)
            : this(pSourceWindow, pLabelText, pFieldDisplayValue, pFieldDisplayValue, pValue, null) { }

        public XPOEntryBoxSelectRecord(Window pSourceWindow, String pLabelText, String pFieldDisplayValue, String pFieldValidateValue, T1 pValue, CriteriaOperator pCriteriaOperator)
            : base(pSourceWindow, pLabelText)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
            _fieldDisplayValue = (pFieldDisplayValue != string.Empty) ? pFieldDisplayValue : "Designation";
            _value = pValue;
            _criteriaOperator = pCriteriaOperator;
            //Init Private
            _dialogSize = GlobalApp.MaxWindowSize;

            //Add Entry if is BaseClass XPOEntryBoxSelectRecord, Else Leave it for SubClassed Classes (Create Diferente Entry Types:)
            if (this.GetType() == typeof(XPOEntryBoxSelectRecord<T1, T2>))
            {
                _entry = new Entry();
                InitEntry(_entry);
            }
        }

        protected void InitEntry(Entry pEntry)
        {
            //params
            _entry = pEntry;
            //Settings
            String iconSelectRecord = FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], @"Icons/Windows/icon_window_select_record.png"));
            //Init Button
            _buttonSelectValue = new TouchButtonIcon("touchButtonIcon", Color.Transparent, iconSelectRecord, new Size(20, 20), 30, 30);
            //UI/Pack
            //Assign Initial Value
            if (_value != null && _value.GetMemberValue(_fieldDisplayValue) != null)
            {
                try
                {
                    _entry.Text = (String)Convert.ChangeType(_value.GetMemberValue(_fieldDisplayValue), typeof(String));
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    _entry.Text = string.Empty;
                }
            }
            _entry.ModifyFont(_fontDescription);
            _hbox.PackStart(_entry, true, true, 0);
            _hbox.PackStart(_buttonSelectValue, false, false, 0);
            //Events
            _buttonSelectValue.Clicked += delegate { PopupDialog(_entry); };
            //_entry.FocusGrabbed += delegate { PopupDialog(_entry); };
        }

        //Events
        protected void PopupDialog(Entry pEntry)
        {
            try
            {
                //Call Custom Event
                OnOpenPopup();

                //Local Vars
                PropertyInfo propertyInfo;

                PosSelectRecordDialog<XPCollection, XPGuidObject, T2>
                  dialog = new PosSelectRecordDialog<XPCollection, XPGuidObject, T2>(
                    _sourceWindow,
                    DialogFlags.DestroyWithParent,
                    Resx.window_title_dialog_select_record,
                    _dialogSize,
                    _value,
                    _criteriaOperator,
                    GenericTreeViewMode.Default,
                    null //ActionAreaButtons
                  );

                // Recapture RowActivated : DoubleClick and trigger dialog.Respond
                dialog.GenericTreeView.TreeView.RowActivated += delegate
                {
                    dialog.Respond(ResponseType.Ok);
                };

                int response = dialog.Run();
                if (response == (int)ResponseType.Ok)
                {
                    //Store previousValue before update _value, to keep it
                    _previousValue = _value;

                    //Get Object from dialog else Mixing Sessions, Both belong to diferente Sessions
                    _value = (T1)FrameworkUtils.GetXPGuidObject(typeof(T1), dialog.GenericTreeView.DataSourceRow.Oid);
                    propertyInfo = typeof(T1).GetProperty(_fieldDisplayValue);

                    object value = null;
                    if (propertyInfo != null)
                    {
                        // Get value from XPGuidObject Instance
                        value = propertyInfo.GetValue(_value, null);
                    }
                    else
                    {
                        string invalidFieldMessage = string.Format("Invalid Field DisplayValue:[{0}] on XPGuidObject:[{1}]", _fieldDisplayValue, _value.GetType().Name);
                        _log.Error(invalidFieldMessage);
                        value = invalidFieldMessage;
                    }

                    pEntry.Text = (value != null) ? value.ToString() : Resx.global_error;

                    //Call Custom Event, Only if OK, if Cancel Dont Trigger Event   
                    OnClosePopup();
                }
                dialog.Destroy();

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //Get Field Validate Value
        protected string GetFieldValidateValue(string pFieldValidateValue)
        {
            string result = string.Empty;
            
            //If use a FieldDisplayValue use it to get Reflection Value from XPGuidObject, else use Default Text Value from Entry, this Way we can Validate any Value in XPGuidObject not only the FieldDisplayValue, example text Values
            if (pFieldValidateValue != string.Empty) 
            {
                //If Value is Null, Use string.Empty until we have a valid Value
                if (this.Value != null && this.Value.GetMemberValue(pFieldValidateValue) != null)
                {
                    result = this.Value.GetMemberValue(pFieldValidateValue).ToString();
                }
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Custom Events

        private void OnOpenPopup()
        {
            if (OpenPopup != null)
            {
                OpenPopup(this, EventArgs.Empty);
            }
        }

        private void OnClosePopup()
        {
            if (ClosePopup != null)
            {
                ClosePopup(this, EventArgs.Empty);
            }
        }
    }
}
