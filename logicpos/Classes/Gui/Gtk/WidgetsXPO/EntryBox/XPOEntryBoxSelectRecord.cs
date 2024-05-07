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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using logicpos.Classes.Enums.Dialogs;
using System.Collections;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.financial.library.App;
using logicpos.shared.App;
using logicpos.datalayer.App;
using logicpos.datalayer.Xpo;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;

//Note: This component dont have Validation, is used to be the Base XPOEntryBoxSelectRecordValidation 
//and to be used with CrudWidgetList Validation

namespace logicpos.Classes.Gui.Gtk.WidgetsXPO
{
    //Genertic Types T1:XPGuidObject Childs (Ex Customer), T2:GenericTreeView Childs (ex TreeViewConfigurationCountry)
    internal class XPOEntryBoxSelectRecord<T1, T2> : EntryBoxBase
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

        public TouchButtonIcon ButtonSelectValue { get; set; }

        public TouchButtonIcon ButtonClearValue { get; set; }

        public TouchButtonIcon ButtonAddValue { get; set; }


        //Custom Events
        public event EventHandler OpenPopup;
        public event EventHandler ClosePopup;
        public event EventHandler CleanArticleEvent;
        public event EventHandler AddNewEntryEvent;

        public CriteriaOperator CriteriaOperator { get; set; }
        //Defaults
        private Size _dialogSize;
        private bool _articleCode;

        public Entry Entry { get; set; }
        public Entry QtdEntry { get; set; }

        public Entry CodeEntry { get; set; }

        public ICollection dropdownTextCollection;

        public XPCollection MoreResultsTree { get; private set; }
        public CriteriaOperator CriteriaOperatorLastFilter { get; private set; }

        //Constructor/OverLoads
        public XPOEntryBoxSelectRecord(Window pSourceWindow, string pLabelText)
            : this(pSourceWindow, pLabelText, string.Empty, string.Empty) { }

        public XPOEntryBoxSelectRecord(Window pSourceWindow, string pLabelText, string pFieldDisplayValue, string pFieldValidateValue)
            : this(pSourceWindow, pLabelText, pFieldDisplayValue, pFieldDisplayValue, null) { }

        public XPOEntryBoxSelectRecord(Window pSourceWindow, string pLabelText, string pFieldDisplayValue, string pFieldValidateValue, T1 pValue)
            : this(pSourceWindow, pLabelText, pFieldDisplayValue, pFieldDisplayValue, pValue, null) { }

        public XPOEntryBoxSelectRecord(Window pSourceWindow, string pLabelText, string pFieldDisplayValue, string pFieldValidateValue, T1 pValue, CriteriaOperator pCriteriaOperator, bool BOSource = false)
            : base(pSourceWindow, pLabelText, BOSource)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
            _fieldDisplayValue = (pFieldDisplayValue != string.Empty) ? pFieldDisplayValue : "Designation";
            _value = pValue;
            CriteriaOperator = pCriteriaOperator;
            //Init Private
            _dialogSize = GlobalApp.MaxWindowSize;

            //Add Entry if is BaseClass XPOEntryBoxSelectRecord, Else Leave it for SubClassed Classes (Create Diferente Entry Types:)
            if (this.GetType() == typeof(XPOEntryBoxSelectRecord<T1, T2>))
            {
                Entry = new Entry();
                InitEntry(Entry);
            }
        }
		//Artigos Compostos [IN:016522]
        protected void InitEntryBOSource(Entry pCodeEntry, Entry pEntry, Entry pQtdentry)
        {
            //params
            Entry = pEntry;
            QtdEntry = pQtdentry;
            CodeEntry = pCodeEntry;
            //Settings
            string iconSelectRecord = string.Format("{0}{1}", DataLayerFramework.Path["images"], @"Icons/Windows/icon_window_select_record.png");
            string iconClearRecord = string.Format("{0}{1}", DataLayerFramework.Path["images"], @"Icons/Windows/icon_window_delete_record.png");
            string iconAddRecord = string.Format("{0}{1}", DataLayerFramework.Path["images"], @"Icons/icon_pos_nav_new.png");
            //Init Button
            ButtonSelectValue = new TouchButtonIcon("touchButtonIcon", Color.Transparent, iconSelectRecord, new Size(20, 20), 30, 30);
            ButtonClearValue = new TouchButtonIcon("touchButtonIcon", Color.Transparent, iconClearRecord, new Size(20, 20), 30, 30);
            ButtonAddValue = new TouchButtonIcon("touchButtonIcon", Color.Transparent, iconAddRecord, new Size(20, 20), 30, 30);

            //UI/Pack
            //Assign Initial Value
            if (_value != null && _value.GetMemberValue(_fieldDisplayValue) != null)
            {
                try
                {
                    if(_value.GetType() == typeof(fin_articlewarehouse))
                    {
                        if((_value as fin_articlewarehouse).ArticleSerialNumber != null)
                        {
                            Entry.Text = (_value as fin_articlewarehouse).ArticleSerialNumber.SerialNumber;
                        }
                        else if((_value as fin_articlewarehouse).Article != null)
                        {
                            Entry.Text = (_value as fin_articlewarehouse).Article.Designation;
                        }
                        else Entry.Text = (string)Convert.ChangeType(_value.GetMemberValue(_fieldDisplayValue), typeof(string));
                    }
                    else Entry.Text = (string)Convert.ChangeType(_value.GetMemberValue(_fieldDisplayValue), typeof(string));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    Entry.Text = string.Empty;
                }
            }
            //TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 
            //Se for código de artigo, lista os códigos, por defeito é a designação
            if (_fieldDisplayValue == "Code") _articleCode = true;
            else _articleCode = false;

            //Preenche o dropdown de auto-complete
            //store = fillDropDowntext(_value, _criteriaOperator);

            //_entry.Completion = new EntryCompletion();
            //_entry.Completion.Model = store;
            //_entry.Completion.TextColumn = 0;
            //_entry.Completion.PopupCompletion = true;
            //_entry.Completion.InlineCompletion = true;
            //_entry.Completion.PopupSingleMatch = true;
            //_entry.Completion.InlineSelection = true;
            Entry.ModifyFont(_fontDescription);
            CodeEntry.ModifyFont(_fontDescription);
            QtdEntry.ModifyFont(_fontDescription);
            //_articleCode = true;
            //_codeEntry.Completion = new EntryCompletion();
            //_codeEntry.Completion.Model = fillDropDowntext(_value, _criteriaOperator);
            //_codeEntry.Completion.TextColumn = 0;
            //_codeEntry.Completion.PopupCompletion = true;
            //_codeEntry.Completion.InlineCompletion = true;
            //_codeEntry.Completion.PopupSingleMatch = true;
            //_codeEntry.Completion.InlineSelection = true;
            //_codeEntry.ModifyFont(_fontDescription);
            CodeEntry.WidthRequest = 50;
            QtdEntry.WidthRequest = 50;

            _hbox.PackStart(CodeEntry, false, false, 0);
            _hbox.PackStart(Entry, true, true, 0);
            _hbox.PackStart(QtdEntry, false, false, 0);
            _hbox.PackStart(ButtonSelectValue, false, false, 0);
            _hbox.PackStart(ButtonClearValue, false, false, 0);
            _hbox.PackStart(ButtonAddValue, false, false, 0);
            //Events
            ButtonSelectValue.Clicked += delegate { PopupDialog(Entry); };
            ButtonClearValue.Clicked += delegate { OnCleanArticleEvent();  };
            ButtonAddValue.Clicked += delegate { OnAddNewEntryEvent(); };

            //_entry.Changed += delegate
            //{
            //    SelectRecordDropDownBOSourceCode(_entry, _codeEntry, _qtdentry);
            //};

            //_entry.FocusGrabbed += delegate { PopupDialog(_entry); };
        }

        [Obsolete]
        protected void InitEntry(Entry pEntry)
        {
            //params
            Entry = pEntry;
            ListStore store = null;
            //Settings
            string iconSelectRecord = string.Format("{0}{1}", DataLayerFramework.Path["images"], @"Icons/Windows/icon_window_select_record.png");
            //Init Button
            ButtonSelectValue = new TouchButtonIcon("touchButtonIcon", Color.Transparent, iconSelectRecord, new Size(20, 20), 30, 30);
            //UI/Pack
            //Assign Initial Value
            if (_value != null && _value.GetMemberValue(_fieldDisplayValue) != null)
            {
                try
                {
                    if (_value.GetType() == typeof(fin_articlewarehouse))
                    {
                        if ((_value as fin_articlewarehouse).ArticleSerialNumber != null)
                        {
                            Entry.Text = (_value as fin_articlewarehouse).ArticleSerialNumber.SerialNumber;
                        }
                        else if ((_value as fin_articlewarehouse).Article != null)
                        {
                            Entry.Text = (_value as fin_articlewarehouse).Article.Designation;
                        }
                        else Entry.Text = (string)Convert.ChangeType(_value.GetMemberValue(_fieldDisplayValue), typeof(string));
                    }
                    else Entry.Text = (string)Convert.ChangeType(_value.GetMemberValue(_fieldDisplayValue), typeof(string));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    Entry.Text = string.Empty;
                }
            }
			//TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 
            //Se for código de artigo, lista os códigos, por defeito é a designação
            if (_fieldDisplayValue == "Code") _articleCode = true;
            else _articleCode = false;

            //Preenche o dropdown de auto-complete
            store = fillDropDowntext(_value, CriteriaOperator);

            Entry.Completion = new EntryCompletion();
            Entry.Completion.Model = store;
            Entry.Completion.TextColumn = 0;
            Entry.Completion.PopupCompletion = true;
            Entry.Completion.InlineCompletion = false;
            Entry.Completion.PopupSingleMatch = true;
            Entry.Completion.InlineSelection = false;


            Entry.ModifyFont(_fontDescription);
            _hbox.PackStart(Entry, true, true, 0);
            _hbox.PackStart(ButtonSelectValue, false, false, 0);
            //Events
            ButtonSelectValue.Clicked += delegate { PopupDialog(Entry); };

            Entry.Changed += delegate
            {
                SelectRecordDropDown(Entry);
            };

            //_entry.FocusGrabbed += delegate { PopupDialog(_entry); };
        }

        //TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 	
        [Obsolete]
        private ListStore fillDropDowntext(T1 value, CriteriaOperator pCriteria = null)
        {
            ListStore store = new ListStore(typeof(string));
            if (value != null)
            {
                string sortProp = "Designation";
                if (value.ClassInfo.ToString() == "logicpos.datalayer.DataLayer.Xpo.erp_customer")
                {
                    sortProp = "Name";
                }
                else if (value.GetType() == typeof(fin_articleserialnumber))
                {
                    sortProp = "SerialNumber";
                }
                else if (value.GetType() == typeof(fin_articlewarehouse))
                {
                    sortProp = "CreatedAt";
                }
                else if (value.GetType() == typeof(fin_articlestock))
                {
                    sortProp = "DocumentNumber";
                }
                else if (value.GetType() == typeof(fin_documentfinancemaster))
                {
                    sortProp = "DocumentNumber";
                }
                SortingCollection sortCollection = new SortingCollection
                {
                    new SortProperty(sortProp, DevExpress.Xpo.DB.SortingDirection.Ascending)
                };
                if(pCriteria == null) pCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));

                dropdownTextCollection = XPOSettings.Session.GetObjects(XPOSettings.Session.GetClassInfo(value), pCriteria, sortCollection, int.MaxValue, false, true);

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
                        else if (value.GetType() == typeof(fin_articleserialnumber))
                        {
                            store.AppendValues(item.SerialNumber);
                        }
                        else if (value.GetType() == typeof(fin_articlestock) || value.GetType() == typeof(fin_documentfinancemaster))
                        {
                            if(item.DocumentNumber != null)
                            {
                                store.AppendValues(item.DocumentNumber);
                            }
                        }
                        else if (value.GetType() == typeof(fin_articlewarehouse))
                        {
                            if(item.ArticleSerialNumber != null)
                            {
                                store.AppendValues(item.ArticleSerialNumber.SerialNumber);
                            }
                            else
                            {
                                store.AppendValues(item.Article.Designation);
                            }                            
                        }
                        else { store.AppendValues(item.Designation); }
                    }
                }

            }
            return store;
        }
		//Artigos Compostos [IN:016522]
        private void SelectRecordDropDownBOSourceCode(Entry pEntry, Entry pEntryCode, Entry pEntryQtd)
        {
            PropertyInfo propertyInfo;
            //Store previousValue before update _value, to keep it
            _previousValue = _value;

            Guid articleOid = Guid.Empty;
            if (dropdownTextCollection != null && _value != null)
            {
                foreach (fin_article item in dropdownTextCollection)
                {
                    if (item.Code == pEntryCode.Text)
                    {
                        articleOid = item.Oid;
                        break;
                    }
                    if (item.Designation == pEntry.Text)
                    {
                        articleOid = item.Oid;
                        break;
                    }
                }
            }
            if (!articleOid.Equals(Guid.Empty))
            {
                //Get Object from dialog else Mixing Sessions, Both belong to diferente Sessions
                _value = (T1)DataLayerUtils.GetXPGuidObject(typeof(T1), articleOid);
                propertyInfo = typeof(T1).GetProperty(_fieldDisplayValue);

                fin_article value = null;
                if (propertyInfo != null)
                {
                    // Get value from XPGuidObject Instance
                    value = (fin_article)propertyInfo.GetValue(_value, null);
                }
                else
                {
                    string invalidFieldMessage = string.Format("Invalid Field DisplayValue:[{0}] on XPGuidObject:[{1}]", _fieldDisplayValue, _value.GetType().Name);
                    _logger.Error(invalidFieldMessage);
                    //value = invalidFieldMessage;
                }
                

                pEntry.Text = (value != null) ? value.ToString() : CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error");

                pEntryCode.Text = (value != null) ? value.Code.ToString() : CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error");
                pEntryQtd.Text = (value != null) ? value.DefaultQuantity.ToString() : CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error");
                
                OnClosePopup();
            }
            else
            {
                //Front-End - Adicionar artigos na criação de Documentos [IN:010335]
                propertyInfo = typeof(T1).GetProperty(_fieldDisplayValue);
            }
        }

        //TK016251 - FrontOffice - Criar novo documento com auto-complete para artigos e clientes 
        private void SelectRecordDropDown(Entry pEntry, bool pBOSource = false)
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
                    else if (_value.GetType() == typeof(fin_articleserialnumber))
                    {
                        if (item.SerialNumber == pEntry.Text)
                        {
                            articleOid = item.Oid;
                        }
                    }
                    else if (_value.GetType() == typeof(fin_articlestock))
                    {
                        if (item.DocumentNumber == pEntry.Text)
                        {
                            articleOid = item.Oid;
                        }
                    }
                    else if (_value.GetType() == typeof(fin_articlewarehouse))
                    {
                        if (item.ArticleSerialNumber != null)
                        {
                            if (item.ArticleSerialNumber.SerialNumber == pEntry.Text)
                            {
                                articleOid = item.Oid;
                            }
                        }
                        else
                        {
                            if (item.Article.Designation == pEntry.Text)
                            {
                                articleOid = item.Oid;
                            }
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
                _value = (T1)DataLayerUtils.GetXPGuidObject(typeof(T1), articleOid);
                propertyInfo = typeof(T1).GetProperty(_fieldDisplayValue);

                object value;
                if (propertyInfo != null)
                {
                    // Get value from XPGuidObject Instance
                    value = propertyInfo.GetValue(_value, null);
                }
                else
                {
                    string invalidFieldMessage = string.Format("Invalid Field DisplayValue:[{0}] on XPGuidObject:[{1}]", _fieldDisplayValue, _value.GetType().Name);
                    _logger.Error(invalidFieldMessage);
                    value = invalidFieldMessage;
                }
                if (value != null && value.GetType() == typeof(fin_articleserialnumber))
                {
                    Entry.Text = (value as fin_articleserialnumber).SerialNumber;
                }
                else if(propertyInfo.Name == "ArticleSerialNumber") 
                {
                    pEntry.Text = (value != null) ? value.ToString() : "";
                }
                else
                {
                    pEntry.Text = (value != null) ? value.ToString() : CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error");
                }           
                OnClosePopup();
            }
            //else if (typeof(T2) == typeof(TreeViewArticleSerialNumber))
            //{
            //    var articleSerialNumber = (fin_articleserialnumber)DataLayerUtils.GetXPGuidObject(typeof(fin_article), (_value as fin_articleserialnumber).Article.Oid);
            //    pEntry.Text = articleSerialNumber.SerialNumber;
            //}
            else {
                //Front-End - Adicionar artigos na criação de Documentos [IN:010335]
                propertyInfo = typeof(T1).GetProperty(_fieldDisplayValue);
                if (!pBOSource && _value != null && _value.ClassInfo.ToString() == "logicpos.datalayer.DataLayer.Xpo.fin_article")
                {
                    OnClosePopup();
                }

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
                    CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_dialog_select_record"),
                    _dialogSize,
                    _value,
                    CriteriaOperator,
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
                _logger.Error(ex.Message, ex);
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
            object value = null;
            //Get Object from dialog else Mixing Sessions, Both belong to diferente Sessions
            _value = (T1)DataLayerUtils.GetXPGuidObject(typeof(T1), dialog.GenericTreeView.DataSourceRow.Oid);
            propertyInfo = typeof(T1).GetProperty(_fieldDisplayValue);

            if (typeof(T2) == typeof(TreeViewArticleSerialNumber))
            {
                propertyInfo = typeof(T1).GetProperty("SerialNumber");
            }

            if (typeof(T2) == typeof(TreeViewArticleWarehouse))
            {
                _value = (T1)DataLayerUtils.GetXPGuidObject(typeof(T1), (dialog.GenericTreeView.DataSourceRow as fin_articlewarehouse).Oid);
                propertyInfo = typeof(T1).GetProperty(_fieldDisplayValue);
                if(propertyInfo == null) { propertyInfo = typeof(T1).GetProperty("Warehouse"); }
            }

            
            else if (propertyInfo != null)
            {
                // Get value from XPGuidObject Instance
                value = propertyInfo.GetValue(_value, null);
            }
            else
            {
                string invalidFieldMessage = string.Format("Invalid Field DisplayValue:[{0}] on XPGuidObject:[{1}]", _fieldDisplayValue, _value.GetType().Name);
                _logger.Error(invalidFieldMessage);
                value = invalidFieldMessage;
            }
            if (value != null && value.GetType() == typeof(fin_articleserialnumber))
            {
                Entry.Text = (value as fin_articleserialnumber).SerialNumber;
            }
            else
            {
                if(_value != null && _value.Oid == SharedSettings.XpoOidUserRecord)
                {
                    pEntry.Text = (value != null) ? CryptorEngine.Decrypt(value.ToString(), true, FinancialLibrarySettings.SecretKey) : "";
                }
                else
                {
                    pEntry.Text = (value != null) ? value.ToString() : "";
                }
                 //CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error");
            }
            if (typeof(T2) == typeof(TreeViewArticleWarehouse))
            {
                var articleWarehouse = (fin_articlewarehouse)DataLayerUtils.GetXPGuidObject(typeof(fin_articlewarehouse), (dialog.GenericTreeView.DataSourceRow as fin_articlewarehouse).Oid);
                if((articleWarehouse as fin_articlewarehouse).ArticleSerialNumber != null)
                {
                    pEntry.Text = (articleWarehouse as fin_articlewarehouse).ArticleSerialNumber.SerialNumber;
                }                
                if(string.IsNullOrEmpty(pEntry.Text)) pEntry.Text = "";
            }
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
                dialog.GenericTreeView.DataSource.TopReturnedObjects = (POSSettings.PaginationRowsPerPage * dialog.GenericTreeView.CurrentPageNumber);
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
				//Titulo nas janelas de filtro de relatório [IN:014328]
                PosReportsQueryDialog dialogFilter = new PosReportsQueryDialog(dialog, DialogFlags.DestroyWithParent, ReportsQueryDialogMode.FILTER_DOCUMENTS_PAGINATION, "fin_documentfinancemaster", CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_dialog_report_filter"));
                DialogResponseType responseFilter = (DialogResponseType)dialogFilter.Run();

                //If button Clean Filter Clicked
                if (DialogResponseType.CleanFilter.Equals(responseFilter))
                {
                    dialog.GenericTreeView.CurrentPageNumber = 1;
                    dialog.GenericTreeView.DataSource.Criteria = CriteriaOperatorLastFilter;
                    dialog.GenericTreeView.DataSource.TopReturnedObjects = POSSettings.PaginationRowsPerPage * dialog.GenericTreeView.CurrentPageNumber;
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
                    CriteriaOperator criteriaOperator = CriteriaOperator.And(CriteriaOperatorLastFilter, CriteriaOperator.Parse(result[0]));

                    //lastData = dialog.GenericTreeView.DataSource;

                    dialog.GenericTreeView.DataSource.Criteria = criteriaOperator;
                    dialog.GenericTreeView.DataSource.TopReturnedObjects = POSSettings.PaginationRowsPerPage * dialog.GenericTreeView.CurrentPageNumber;
                    dialog.GenericTreeView.Refresh();

                    //se retornar zero resultados apresenta dados anteriores ao filtro
                    if (dialog.GenericTreeView.DataSource.Count == 0)
                    {
                        dialog.GenericTreeView.DataSource.Criteria = criteriaOperatorLast;
                        dialog.GenericTreeView.DataSource.TopReturnedObjects = POSSettings.PaginationRowsPerPage * dialog.GenericTreeView.CurrentPageNumber;
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
            OpenPopup?.Invoke(this, EventArgs.Empty);
        }

        private void OnClosePopup()
        {
            ClosePopup?.Invoke(this, EventArgs.Empty);
        }

        private void OnCleanArticleEvent()
        {
            CleanArticleEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnAddNewEntryEvent()
        {
            AddNewEntryEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
