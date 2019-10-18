using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Enums.Reports;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using logicpos.Classes.Enums.Dialogs;
using System.Collections;

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
        private bool _articleCode;
        //Public Properties
        private Entry _entry;
        public Entry Entry
        {
            get { return _entry; }
            set { _entry = value; }
        }

        public ICollection dropdownTextCollection;

        public XPCollection MoreResultsTree { get; private set; }
        public CriteriaOperator CriteriaOperatorLastFilter { get; private set; }

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
            ListStore store = null;
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
			//TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 
            //Se for código de artigo, lista os códigos, por defeito é a designação
            if (_fieldDisplayValue == "Code") _articleCode = true;
            else _articleCode = false;

            //Preenche o dropdown de auto-complete
            store = fillDropDowntext(_value);

            _entry.Completion = new EntryCompletion();
            _entry.Completion.Model = store;
            _entry.Completion.TextColumn = 0;
            _entry.Completion.PopupCompletion = true;
            _entry.Completion.InlineCompletion = false;
            _entry.Completion.PopupSingleMatch = true;
            _entry.Completion.InlineSelection = false;


            _entry.ModifyFont(_fontDescription);
            _hbox.PackStart(_entry, true, true, 0);
            _hbox.PackStart(_buttonSelectValue, false, false, 0);
            //Events
            _buttonSelectValue.Clicked += delegate { PopupDialog(_entry); };

            _entry.Changed += delegate
            {
                SelectRecordDropDown(_entry);
            };

            //_entry.FocusGrabbed += delegate { PopupDialog(_entry); };
        }
		//TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 	
        private ListStore fillDropDowntext(T1 value)
        {
            ListStore store = new ListStore(typeof(string));
            if (value != null)
            {
                string sortProp = "Designation";
                if (value.ClassInfo.ToString() == "logicpos.datalayer.DataLayer.Xpo.erp_customer")
                {
                    sortProp = "Name";
                }
                SortingCollection sortCollection = new SortingCollection();
                sortCollection.Add(new SortProperty(sortProp, DevExpress.Xpo.DB.SortingDirection.Ascending));
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));

                dropdownTextCollection = GlobalFramework.SessionXpo.GetObjects(GlobalFramework.SessionXpo.GetClassInfo(value), criteria, sortCollection, int.MaxValue, false, true);

                if (dropdownTextCollection != null)
                {
                    foreach (dynamic item in dropdownTextCollection)
                    {
                        if (value.ClassInfo.ToString() == "logicpos.datalayer.DataLayer.Xpo.erp_customer")
                        {
                            store.AppendValues(item.Name);
                        }
                        else if (_articleCode)
                        {
                            store.AppendValues(item.Code);
                        }
                        else { store.AppendValues(item.Designation); }
                    }
                }

            }
            return store;
        }
		//TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 
        private void SelectRecordDropDown(Entry pEntry)
        {
            PropertyInfo propertyInfo;
            //Store previousValue before update _value, to keep it
            _previousValue = _value;

            Guid articleOid = Guid.Empty;
            if (dropdownTextCollection != null && _value != null)
            {
                foreach (dynamic item in dropdownTextCollection)
                {
                    if (_value.ClassInfo.ToString() == "logicpos.datalayer.DataLayer.Xpo.erp_customer")
                    {
                        if (item.Name == pEntry.Text)
                        {
                            articleOid = item.Oid;
                        }
                    }
                    else if (_articleCode)
                    {
                        if (item.Code == pEntry.Text)
                        {
                            articleOid = item.Oid;
                        }
                    }
                    else if (item.Designation == pEntry.Text)
                    {
                        articleOid = item.Oid;
                    }
                }
            }
            if (!articleOid.Equals(Guid.Empty))
            {
                //Get Object from dialog else Mixing Sessions, Both belong to diferente Sessions
                _value = (T1)FrameworkUtils.GetXPGuidObject(typeof(T1), articleOid);
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

                pEntry.Text = (value != null) ? value.ToString() : resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error");
                OnClosePopup();
            }

        }

        //Events
        protected void PopupDialog(Entry pEntry)
        {
            try
            {
                //Call Custom Event
                OnOpenPopup();

                PosSelectRecordDialog<XPCollection, XPGuidObject, T2>
                  dialog = new PosSelectRecordDialog<XPCollection, XPGuidObject, T2>(
                    _sourceWindow,
                    DialogFlags.DestroyWithParent,
                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_select_record"),
                    _dialogSize,
                    _value,
                    _criteriaOperator,
                    GenericTreeViewMode.Default,
                    null //ActionAreaButtons
                  );

                CriteriaOperatorLastFilter = dialog.GenericTreeView.DataSource.Criteria;
                int response = 0;
                // Recapture RowActivated : DoubleClick and trigger dialog.Respond
                dialog.GenericTreeView.TreeView.RowActivated += delegate
                {
                    dialog.Respond(ResponseType.Ok);
                };

                //_buttonSelectValue.Clicked += delegate { SelectRecord(pEntry, dialog); };
                response = PopuDialogMore(pEntry, dialog);

                /* IN009223 - Call to:
                 * - SelectRecord(pEntry, dialog); 
                 * - dialog.Destroy();
                 * Were causing issues to dialog box messages, therefore call to them removed.
                 */
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
        /// <summary>
        /// If Record is Selected and Ok clicked
        /// </summary>
        /// <param name="pEntry"></param>
        /// <param name="dialog"></param>
        private void SelectRecord(Entry pEntry, PosSelectRecordDialog<XPCollection, XPGuidObject, T2> dialog)
        {
            PropertyInfo propertyInfo;
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

            pEntry.Text = (value != null) ? value.ToString() : resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error");

            //Call Custom Event, Only if OK, if Cancel Dont Trigger Event   
            OnClosePopup();
        }


        /// <summary>
        /// Recursive function for pagination
        /// </summary>
        /// <param name="pEntry"></param>
        /// <param name="dialog"></param>
        /// <returns></returns>
        public int PopuDialogMore(Entry pEntry, PosSelectRecordDialog<XPCollection, XPGuidObject, T2> dialog)
        {
            DialogResponseType response = (DialogResponseType)dialog.Run();

            // Recapture RowActivated : DoubleClick and trigger dialog.Respond
            dialog.GenericTreeView.TreeView.RowActivated += delegate
            {
                SelectRecord(pEntry, dialog);
            };
            if (DialogResponseType.Ok.Equals(response))
            {
                SelectRecord(pEntry, dialog);
            }

            //Pagination response 
            if (DialogResponseType.LoadMore.Equals(response))
            {
                dialog.GenericTreeView.DataSource.TopReturnedObjects = (SettingsApp.PaginationRowsPerPage * dialog.GenericTreeView.CurrentPageNumber);
                dialog.GenericTreeView.Refresh();
                PopuDialogMore(pEntry, dialog);
            }

            //Filter  response
            else if (DialogResponseType.Filter.Equals(response))
            {
                //Reset current page to 1 ( Pagination go to defined initialy )


                // Filter SellDocuments
                string filterField = string.Empty;
                string statusField = string.Empty;
                string extraFilter = string.Empty;

                List<string> result = new List<string>();

                PosReportsQueryDialog dialogFilter = new PosReportsQueryDialog(dialog, DialogFlags.DestroyWithParent, ReportsQueryDialogMode.FILTER_DOCUMENTS_PAGINATION, "fin_documentfinancemaster");
                DialogResponseType responseFilter = (DialogResponseType)dialogFilter.Run();

                //If button Clean Filter Clicked
                if (DialogResponseType.CleanFilter.Equals(responseFilter))
                {
                    dialog.GenericTreeView.CurrentPageNumber = 1;
                    dialog.GenericTreeView.DataSource.Criteria = CriteriaOperatorLastFilter;
                    dialog.GenericTreeView.DataSource.TopReturnedObjects = SettingsApp.PaginationRowsPerPage * dialog.GenericTreeView.CurrentPageNumber;
                    dialog.GenericTreeView.Refresh();
                    dialogFilter.Destroy();
                    PopuDialogMore(pEntry, dialog);
                }
                //If OK filter clicked
                else if (DialogResponseType.Ok.Equals(responseFilter))
                {
                    dialog.GenericTreeView.CurrentPageNumber = 1;
                    filterField = "DocumentType";
                    statusField = "DocumentStatusStatus";

                    /* IN009066 - FS and NC added to reports */
                    //extraFilter = $@" AND ({statusField} <> 'A') AND (
                    //   {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeInvoice}' OR 
                    //   {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice}' OR 
                    //   {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment}' OR 
                    //   {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice}' OR 
                    //   {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeDebitNote}' OR 
                    //   {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeCreditNote}' OR 
                    //   {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypePayment}' 
                    //   OR 
                    //   {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput}'
                    //   )".Replace(Environment.NewLine, string.Empty);
                    /* IN009089 - # TO DO: above, we need to check with business this condition:  {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput}' */

                    //Assign Dialog FilterValue to Method Result Value
                    result.Add($"{dialogFilter.FilterValue}");
                    result.Add(dialogFilter.FilterValueHumanReadble);
                    //string addFilter = FilterValue;

                    CriteriaOperator criteriaOperatorLast = dialog.GenericTreeView.DataSource.Criteria;
                    CriteriaOperator criteriaOperator = GroupOperator.And(CriteriaOperatorLastFilter, CriteriaOperator.Parse(result[0]));

                    //lastData = dialog.GenericTreeView.DataSource;

                    dialog.GenericTreeView.DataSource.Criteria = criteriaOperator;
                    dialog.GenericTreeView.DataSource.TopReturnedObjects = SettingsApp.PaginationRowsPerPage * dialog.GenericTreeView.CurrentPageNumber;
                    dialog.GenericTreeView.Refresh();

                    //se retornar zero resultados apresenta dados anteriores ao filtro
                    if (dialog.GenericTreeView.DataSource.Count == 0)
                    {
                        dialog.GenericTreeView.DataSource.Criteria = criteriaOperatorLast;
                        dialog.GenericTreeView.DataSource.TopReturnedObjects = SettingsApp.PaginationRowsPerPage * dialog.GenericTreeView.CurrentPageNumber;
                        dialog.GenericTreeView.Refresh();
                    }
                    dialogFilter.Destroy();
                    PopuDialogMore(pEntry, dialog);
                }
                //If Cancel Filter Clicked
                else
                {
                    dialogFilter.Destroy();
                    PopuDialogMore(pEntry, dialog);
                }
            }
            //Button Close clicked
            else
            {
                dialog.Destroy();
            }

            return (int)response;
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
