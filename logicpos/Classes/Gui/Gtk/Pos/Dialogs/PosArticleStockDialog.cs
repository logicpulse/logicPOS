using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Extensions;
using logicpos.financial.library.Classes.Stocks;
using logicpos.shared.App;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    /// <summary>
    /// PosArticleStock Dialog
    /// </summary>
    public class PosArticleStockDialog : PosBaseDialog
    {
        //UI Components Dialog
        private VBox _vbox;
        private readonly TouchButtonIconWithText _buttonOk;
        private readonly TouchButtonIconWithText _buttonCancel;
        //UI Components Form
        private XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _entryBoxSelectSupplier;
        private EntryBoxValidationDatePickerDialog _entryBoxDocumentDate;
        private EntryBoxValidation _entryBoxDocumentNumber;
        private readonly XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle> _entryBoxSelectArticle;
        private readonly EntryBoxValidation _entryBoxQuantity;
        private EntryBoxValidation _entryBoxNotes;
        //InitialValues
        private erp_customer _initialSupplier = null;
        private DateTime _initialDocumentDate;
        private string _initialDocumentNumber;
        //MultiArticles
        private ICollection<XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>> _entryCompositeLinesCollection;
        //private ICollection<fin_article> _articleCollection;
        private Dictionary<fin_article, decimal> _articleCollection;
        public ICollection _dropdownTextCollection;
        private XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle> _entryBoxSelectArticle1;
        private int _totalCompositeEntrys = 0;
        private readonly fin_article _article = null;
        private fin_article _previousValue = null;
        private VBox _vboxArticles;
        private ScrolledWindow _scrolledWindowView;

        //Public Methods
        public erp_customer Customer
        {
            get { return _entryBoxSelectSupplier.Value; }
        }
        public DateTime DocumentDate
        {
            get { return _entryBoxDocumentDate.Value; }
        }
        public string DocumentNumber
        {
            get { return _entryBoxDocumentNumber.EntryValidation.Text; }
        }
        public fin_article Article;

        public Dictionary<fin_article, decimal> ArticleCollection
        {
            get { return _articleCollection; }
        }
        public decimal Quantity;

        public string Notes
        {
            get { return _entryBoxNotes.EntryValidation.Text; }
        }

        public PosArticleStockDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_article_stock");
            Size windowSize = new Size(500, 580);
            string fileDefaultWindowIcon = SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\Windows\icon_window_stocks.png");

            InitUI();

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
            _buttonOk.Sensitive = false;

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(_buttonOk, ResponseType.Ok),
                new ActionAreaButton(_buttonCancel, ResponseType.Cancel)
            };

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _scrolledWindowView, actionAreaButtons);
        }

        private void InitUI()
        {
            try
            {
                //Load Initial Values
                Load();

                //Init VBOX
                _vbox = new VBox(false, 0);
                _entryCompositeLinesCollection = new List<XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>>();
                //_articleCollection = new List<fin_article>();
                _articleCollection = new Dictionary<fin_article, decimal>();
                //Supplier
                CriteriaOperator criteriaOperatorSupplier = CriteriaOperator.Parse("(Supplier = 1)");
                _entryBoxSelectSupplier = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(this, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_supplier"), "Name", "Oid", _initialSupplier, criteriaOperatorSupplier, SharedSettings.RegexGuid, true);
                _entryBoxSelectSupplier.EntryValidation.IsEditable = true;
                _entryBoxSelectSupplier.EntryValidation.Completion.PopupCompletion = true;
                _entryBoxSelectSupplier.EntryValidation.Completion.InlineCompletion = false;
                _entryBoxSelectSupplier.EntryValidation.Completion.PopupSingleMatch = true;
                _entryBoxSelectSupplier.EntryValidation.Completion.InlineSelection = true;
                _entryBoxSelectSupplier.EntryValidation.Changed += delegate { ValidateDialog(); };

                //DocumentDate
                _entryBoxDocumentDate = new EntryBoxValidationDatePickerDialog(this, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_date"), resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_date"), _initialDocumentDate, SharedSettings.RegexDate, true, SharedSettings.DateFormat);
                //_entryBoxDocumentDate.EntryValidation.Sensitive = true;
                _entryBoxDocumentDate.EntryValidation.Text = _initialDocumentDate.ToString(SharedSettings.DateFormat);
                _entryBoxDocumentDate.EntryValidation.Validate();
                _entryBoxDocumentDate.EntryValidation.Sensitive = true;
                _entryBoxDocumentDate.ClosePopup += delegate { ValidateDialog(); };

                //DocumentNumber
                _entryBoxDocumentNumber = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_document_number"), KeyboardMode.Alfa, SharedSettings.RegexAlfaNumericExtended, false);
                if (_initialDocumentNumber != string.Empty) _entryBoxDocumentNumber.EntryValidation.Text = _initialDocumentNumber;
                _entryBoxDocumentNumber.EntryValidation.Changed += delegate { ValidateDialog(); };


                //MultiArticles
                _vboxArticles = new VBox(true, 0);
                _scrolledWindowView = new ScrolledWindow();
                _scrolledWindowView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                _scrolledWindowView.ModifyBg(StateType.Normal, Color.White.ToGdkColor());
                _scrolledWindowView.ShadowType = ShadowType.Out;
                _totalCompositeEntrys++;
                CriteriaOperator criteriaOperatorSelectArticle = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Class = '{0}')", DataLayerSettings.XpoOidArticleDefaultClass));
                _entryBoxSelectArticle1 = new XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>(this, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_article"), "Designation", "Oid", null, criteriaOperatorSelectArticle, KeyboardMode.None, SharedSettings.RegexAlfaNumericExtended, true, true, SharedSettings.RegexAlfaNumericArticleCode, SharedSettings.RegexDecimalPositiveAndNegative, _totalCompositeEntrys);
                _entryBoxSelectArticle1.EntryValidation.IsEditable = true;
                _entryBoxSelectArticle1.EntryQtdValidation.IsEditable = true;
                _entryBoxSelectArticle1.Value = null;
                _entryBoxSelectArticle1.EntryValidation.Text = "";

                _entryBoxSelectArticle1.EntryValidation.Validate();
                _entryBoxSelectArticle1.EntryCodeValidation.Validate();
                _entryBoxSelectArticle1.EntryQtdValidation.Validate();

                //Events Composition
                _entryBoxSelectArticle1.ClosePopup += _entryBoxSelectArticle_ClosePopup;
                _entryBoxSelectArticle1.CleanArticleEvent += _entryBoxSelectArticle_CleanArticleEvent;
                _entryBoxSelectArticle1.AddNewEntryEvent += NewBox_AddNewEntryEvent;
                _entryBoxSelectArticle1.EntryQtdValidation.TextInserted += QtdEntryValidation_TextInserted;

                //Auto Complete
                _entryBoxSelectArticle1.EntryValidation.Completion = new EntryCompletion();
                _entryBoxSelectArticle1.EntryValidation.Completion.Model = FillDropDownListStore(false, criteriaOperatorSelectArticle);
                _entryBoxSelectArticle1.EntryValidation.Completion.TextColumn = 0;
                _entryBoxSelectArticle1.EntryValidation.Completion.PopupCompletion = true;
                _entryBoxSelectArticle1.EntryValidation.Completion.InlineCompletion = false;
                _entryBoxSelectArticle1.EntryValidation.Completion.PopupSingleMatch = true;
                _entryBoxSelectArticle1.EntryValidation.Completion.InlineSelection = true;

                _entryBoxSelectArticle1.EntryValidation.Changed += delegate
                {
                    SelectRecordDropDownArticle(false, _entryBoxSelectArticle1);
                };

                _entryBoxSelectArticle1.EntryCodeValidation.Completion = new EntryCompletion();
                _entryBoxSelectArticle1.EntryCodeValidation.Completion.Model = FillDropDownListStore(true);
                _entryBoxSelectArticle1.EntryCodeValidation.Completion.TextColumn = 0;
                _entryBoxSelectArticle1.EntryCodeValidation.Completion.PopupCompletion = true;
                _entryBoxSelectArticle1.EntryCodeValidation.Completion.InlineCompletion = false;
                _entryBoxSelectArticle1.EntryCodeValidation.Completion.PopupSingleMatch = true;
                _entryBoxSelectArticle1.EntryCodeValidation.Completion.InlineSelection = true;

                _entryBoxSelectArticle1.EntryCodeValidation.Changed += delegate
                {
                    SelectRecordDropDownArticle(true, _entryBoxSelectArticle1);
                };

                _entryCompositeLinesCollection.Add(_entryBoxSelectArticle1);


                //SelectArticle
                //CriteriaOperator criteriaOperatorSelectArticle = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Class = '{0}')", SettingsApp.XpoOidArticleDefaultClass));
                //_entryBoxSelectArticle = new XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>(this, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_article"), "Designation", "Oid", null, criteriaOperatorSelectArticle, SettingsApp.RegexGuid, true);
                //_entryBoxSelectArticle.EntryValidation.IsEditable = false;
                //_entryBoxSelectArticle.EntryValidation.Changed += delegate { ValidateDialog(); };

                ////Quantity
                //_entryBoxQuantity = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_quantity"), KeyboardMode.Numeric, SettingsApp.RegexDecimalPositiveAndNegative, true);
                //_entryBoxQuantity.EntryValidation.Changed += delegate { ValidateDialog(); };

                //Notes
                _entryBoxNotes = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_notes"), KeyboardMode.Alfa, SharedSettings.RegexAlfaNumericExtended, false);
                _entryBoxNotes.EntryValidation.Changed += delegate { ValidateDialog(); };

                //Final Pack
                _vbox.PackStart(_entryBoxSelectSupplier, false, false, 0);
                _vbox.PackStart(_entryBoxDocumentDate, false, false, 0);
                _vbox.PackStart(_entryBoxDocumentNumber, false, false, 0);
                _vbox.PackStart(_entryBoxNotes, false, false, 0);
                _vbox.PackStart(_entryBoxSelectArticle1, false, false, 0);
                //_vbox.PackStart(_entryBoxQuantity, false, false, 0);

                _scrolledWindowView.AddWithViewport(_vbox);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void ValidateDialog()
        {
            int validateEntrys = 0;
            foreach (var item in _entryCompositeLinesCollection)
            {
                item.EntryCodeValidation.Validate();
                item.EntryQtdValidation.Validate();
                item.EntryValidation.Validate();
                if (!item.EntryCodeValidation.Validated || !item.EntryQtdValidation.Validated || !item.EntryValidation.Validated)
                {
                    validateEntrys++;
                }
            }
            bool multiEntrysValidated = true;
            if (validateEntrys > 0) multiEntrysValidated = false;
            _buttonOk.Sensitive = (
                _entryBoxSelectSupplier.EntryValidation.Validated &&
                _entryBoxDocumentDate.EntryValidation.Validated &&
                _entryBoxDocumentNumber.EntryValidation.Validated &&
                _entryBoxSelectArticle1.EntryValidation.Validated &&
                _entryBoxNotes.EntryValidation.Validated && multiEntrysValidated
            );
        }

        public void Load()
        {
            //Get From Session if Exists
            object supplier = SharedFramework.SessionApp.GetToken(string.Format("{0}_{1}", this.GetType().Name, "supplier").ToUpper());
            object documentDate = SharedFramework.SessionApp.GetToken(string.Format("{0}_{1}", this.GetType().Name, "documentDate").ToUpper());
            object documentNumber = SharedFramework.SessionApp.GetToken(string.Format("{0}_{1}", this.GetType().Name, "documentNumber").ToUpper());
            //Assign if Valid
            try
            {
                if (supplier != null) _initialSupplier = (erp_customer)DataLayerFramework.SessionXpo.GetObjectByKey(typeof(erp_customer), new Guid(supplier.ToString()));
                var own_customer = (erp_customer)DataLayerFramework.SessionXpo.GetObjectByKey(typeof(erp_customer), SharedSettings.XpoOidUserRecord);
                if (own_customer != null)
                {
                    //update owner customer for internal stock moviments                        
                    own_customer.FiscalNumber = SharedFramework.PreferenceParameters["COMPANY_FISCALNUMBER"];
                    own_customer.Name = SharedFramework.PreferenceParameters["COMPANY_NAME"];
                    own_customer.Save();

                    if (supplier == null) { supplier = own_customer; }
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            try
            {
                _initialDocumentDate = (documentDate != null) ? Convert.ToDateTime(documentDate) : DataLayerUtils.CurrentDateTimeAtomic();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            try
            {
                _initialDocumentNumber = (documentNumber != null) ? Convert.ToString(documentNumber) : string.Empty;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public void Save()
        {
            try
            {
                SharedFramework.SessionApp.SetToken(string.Format("{0}_{1}", this.GetType().Name, "supplier").ToUpper(), _entryBoxSelectSupplier.Value.Oid);
                SharedFramework.SessionApp.SetToken(string.Format("{0}_{1}", this.GetType().Name, "documentDate").ToUpper(), _entryBoxDocumentDate.Value);
                SharedFramework.SessionApp.SetToken(string.Format("{0}_{1}", this.GetType().Name, "documentNumber").ToUpper(), _entryBoxDocumentNumber.EntryValidation.Text);
                SharedFramework.SessionApp.Write();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //Select record from dropdown menu
        private void SelectRecordDropDownArticle(bool isArticleCode, XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle> pXPOEntry)
        {
            try
            {
                Guid articleOid = Guid.Empty;
                _previousValue = (fin_article)pXPOEntry.Value;
                if (_dropdownTextCollection != null)
                {
                    foreach (dynamic item in _dropdownTextCollection)
                    {
                        if (isArticleCode)
                        {
                            if (item.Code == pXPOEntry.CodeEntry.Text)
                            {
                                articleOid = item.Oid;
                                break;
                            }
                        }
                        else if (item.Designation == pXPOEntry.EntryValidation.Text)
                        {
                            articleOid = item.Oid;
                            break;
                        }
                    }
                }
                if (!articleOid.Equals(Guid.Empty))
                {
                    //Get Object from dialog else Mixing Sessions, Both belong to diferente Sessions
                    fin_article newArticle = (fin_article)DataLayerUtils.GetXPGuidObject(typeof(fin_article), articleOid);

                    if (isArticleCode)
                    {
                        pXPOEntry.EntryValidation.Changed -= delegate { pXPOEntry.EntryValidation.Validate(); };

                        pXPOEntry.EntryValidation.Text = (newArticle != null) ? newArticle.Designation.ToString() : resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_error");

                        pXPOEntry.EntryValidation.Changed += delegate { pXPOEntry.EntryValidation.Validate(); };

                        pXPOEntry.EntryQtdValidation.Text = (newArticle != null) ? string.Format("{0:0.##}", newArticle.DefaultQuantity) : resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_error");

                        pXPOEntry.EntryCodeValidation.Validate();

                        pXPOEntry.EntryValidation.Validate();

                        pXPOEntry.EntryQtdValidation.Validate();

                        ValidateDialog();

                        return;
                    }
                    pXPOEntry.EntryValidation.Changed -= delegate { pXPOEntry.EntryValidation.Validate(); };

                    pXPOEntry.EntryValidation.Text = (newArticle != null) ? newArticle.Designation.ToString() : resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_error");

                    pXPOEntry.EntryValidation.Changed += delegate { pXPOEntry.EntryValidation.Validate(); };

                    pXPOEntry.EntryCodeValidation.Text = (newArticle != null) ? newArticle.Code.ToString() : resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_error");

                    pXPOEntry.EntryQtdValidation.Text = (newArticle != null) ? string.Format("{0:0.##}", newArticle.DefaultQuantity) : resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_error");

                    pXPOEntry.Value = newArticle;

                    pXPOEntry.EntryCodeValidation.Validate();

                    pXPOEntry.EntryValidation.Validate();

                    pXPOEntry.EntryQtdValidation.Validate();


                    //Clean previous value from colection
                    if (_previousValue != null)
                    {
                        _articleCollection.Remove(_previousValue);
                        //foreach (var articleLine in _articleCollection)
                        //{
                        //    if (articleLine.Key == _previousValue)
                        //    {
                        //        _articleCollection.Remove(articleLine.Key);
                        //        break;
                        //    }
                        //}
                    }

                    //Insert associated articles to collection
                    if (pXPOEntry.Value == _article)
                    {
                        pXPOEntry.Value = null;
                        logicpos.Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Ok, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "dialog_message_composite_article_same"), resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_composite_article"));
                        pXPOEntry.EntryValidation.Text = "";
                        ValidateDialog();
                        return;
                    }
                    _articleCollection.Add(pXPOEntry.Value, Convert.ToDecimal(pXPOEntry.QtdEntry.Text));
                    ValidateDialog();
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error selecting new Composite article Entry : " + ex.Message);
            }
        }

        //Populate dropdown list
        private ListStore FillDropDownListStore(bool isArticleCode, CriteriaOperator pCriteria = null)
        {
            try
            {
                ListStore store = new ListStore(typeof(string));
                string sortProp = "Designation";
                SortingCollection sortCollection = new SortingCollection
                {
                    new SortProperty(sortProp, DevExpress.Xpo.DB.SortingDirection.Ascending)
                };
                if (ReferenceEquals(pCriteria, null)) pCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));

                _dropdownTextCollection = DataLayerFramework.SessionXpo.GetObjects(DataLayerFramework.SessionXpo.GetClassInfo(typeof(fin_article)), pCriteria, sortCollection, int.MaxValue, false, true);

                if (_dropdownTextCollection != null)
                {
                    foreach (dynamic item in _dropdownTextCollection)
                    {
                        if (isArticleCode)
                        {
                            store.AppendValues(item.Code);
                        }
                        else
                        {
                            store.AppendValues(item.Designation);
                        }
                    }
                }
                return store;
            }
            catch (Exception ex)
            {
                _logger.Error("Error populating dropdown list : " + ex.Message);
                return null;
            }
        }

        //Events
        //Add new entry's event
        private void NewBox_AddNewEntryEvent(object sender, EventArgs e)
        {
            try
            {
                _totalCompositeEntrys++;
                //var entrySelected = (XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>)sender;
                CriteriaOperator criteriaOperatorSelectArticle = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1)"));
                XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle> NewEntryBoxSelectArticle = new XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>(this, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_article"), "Designation", "Oid", null, criteriaOperatorSelectArticle, KeyboardMode.None, SharedSettings.RegexAlfaNumericExtended, true, true, SharedSettings.RegexAlfaNumericArticleCode, SharedSettings.RegexDecimalPositiveAndNegative, _totalCompositeEntrys);

                NewEntryBoxSelectArticle.EntryValidation.IsEditable = true;
                NewEntryBoxSelectArticle.Value = null;
                NewEntryBoxSelectArticle.EntryValidation.Text = "";
                NewEntryBoxSelectArticle.EntryCodeValidation.Text = "";
                NewEntryBoxSelectArticle.EntryQtdValidation.Text = "";
                NewEntryBoxSelectArticle.EntryCodeValidation.Validate();
                NewEntryBoxSelectArticle.EntryQtdValidation.Validate();
                _vbox.PackStart(NewEntryBoxSelectArticle, false, false, 0);

                //Events
                NewEntryBoxSelectArticle.ClosePopup += _entryBoxSelectArticle_ClosePopup;
                NewEntryBoxSelectArticle.CleanArticleEvent += _entryBoxSelectArticle_CleanArticleEvent;
                NewEntryBoxSelectArticle.AddNewEntryEvent += NewBox_AddNewEntryEvent;
                NewEntryBoxSelectArticle.EntryQtdValidation.TextInserted += QtdEntryValidation_TextInserted;
                NewEntryBoxSelectArticle.ShowAll();

                //Auto Complete
                NewEntryBoxSelectArticle.EntryValidation.Completion = new EntryCompletion();
                NewEntryBoxSelectArticle.EntryValidation.Completion.Model = FillDropDownListStore(false, criteriaOperatorSelectArticle);
                NewEntryBoxSelectArticle.EntryValidation.Completion.TextColumn = 0;
                NewEntryBoxSelectArticle.EntryValidation.Completion.PopupCompletion = true;
                NewEntryBoxSelectArticle.EntryValidation.Completion.InlineCompletion = false;
                NewEntryBoxSelectArticle.EntryValidation.Completion.PopupSingleMatch = true;
                NewEntryBoxSelectArticle.EntryValidation.Completion.InlineSelection = true;

                NewEntryBoxSelectArticle.EntryValidation.Changed += delegate
                {
                    SelectRecordDropDownArticle(false, NewEntryBoxSelectArticle);
                };

                NewEntryBoxSelectArticle.EntryCodeValidation.Completion = new EntryCompletion();
                NewEntryBoxSelectArticle.EntryCodeValidation.Completion.Model = FillDropDownListStore(true);
                NewEntryBoxSelectArticle.EntryCodeValidation.Completion.TextColumn = 0;
                NewEntryBoxSelectArticle.EntryCodeValidation.Completion.PopupCompletion = true;
                NewEntryBoxSelectArticle.EntryCodeValidation.Completion.InlineCompletion = false;
                NewEntryBoxSelectArticle.EntryCodeValidation.Completion.PopupSingleMatch = true;
                NewEntryBoxSelectArticle.EntryCodeValidation.Completion.InlineSelection = true;

                NewEntryBoxSelectArticle.EntryCodeValidation.Changed += delegate
                {
                    SelectRecordDropDownArticle(true, NewEntryBoxSelectArticle);
                };

                NewEntryBoxSelectArticle.EntryValidation.Validate();
                NewEntryBoxSelectArticle.EntryCodeValidation.Validate();
                NewEntryBoxSelectArticle.EntryQtdValidation.Validate();

                _entryCompositeLinesCollection.Add(NewEntryBoxSelectArticle);
                _scrolledWindowView.AddWithViewport(_vbox);
                //eventBoxPosCompositionView.Add(scrolledWindowCompositionView);
                ValidateDialog();
            }
            catch (Exception ex)
            {
                _logger.Error("Error Adding new Composite article Entry : " + ex.Message);
            }
        }

        //Clean article event
        private void _entryBoxSelectArticle_CleanArticleEvent(object sender, EventArgs e)
        {
            try
            {
                bool cleanFirstEntry = false;
                var entrySelected = (XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>)sender;
                Guid articleToDeleteAux = Guid.Empty;
                if (entrySelected != null)
                {
                    if (entrySelected.Value != null) articleToDeleteAux = entrySelected.Value.Oid;

                    if (entrySelected == _entryBoxSelectArticle1)
                    {
                        if (_totalCompositeEntrys > 1 && _entryCompositeLinesCollection.Count > 1)
                        {
                            foreach (var line in _entryCompositeLinesCollection)
                            {
                                if (line.EntryNumber == _totalCompositeEntrys)
                                {
                                    _entryBoxSelectArticle1.Value = line.Value;
                                    _entryBoxSelectArticle1.EntryValidation.Text = line.EntryValidation.Text;
                                    _entryBoxSelectArticle1.EntryQtdValidation.Text = line.EntryQtdValidation.Text;
                                    _entryBoxSelectArticle1.CodeEntry.Text = line.CodeEntry.Text;
                                    _entryBoxSelectArticle1.EntryValidation.Validate();
                                    _entryBoxSelectArticle1.EntryCodeValidation.Validate();
                                    _entryBoxSelectArticle1.EntryQtdValidation.Validate();


                                    line.Hide();
                                    _entryCompositeLinesCollection.Remove(line);
                                    line.Value = null;
                                    _totalCompositeEntrys--;
                                    cleanFirstEntry = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            _entryBoxSelectArticle1.EntryValidation.Text = "";
                            _entryBoxSelectArticle1.EntryQtdValidation.Text = "";
                            _entryBoxSelectArticle1.CodeEntry.Text = "";
                            _entryBoxSelectArticle1.EntryValidation.Validate();
                            _entryBoxSelectArticle1.EntryCodeValidation.Validate();
                            _entryBoxSelectArticle1.EntryQtdValidation.Validate();
                        }
                    }
                    else
                    {
                        entrySelected.Hide();


                        _totalCompositeEntrys--;

                        _entryCompositeLinesCollection.Remove(entrySelected);

                    }
                    if (entrySelected.Value != null)
                    {
                        fin_article auxArticle = new fin_article();

                        if (cleanFirstEntry)
                        {
                            auxArticle = (fin_article)DataLayerFramework.SessionXpo.GetObjectByKey(typeof(fin_article), articleToDeleteAux);
                        }
                        else
                        {
                            auxArticle = entrySelected.Value;
                        }
                        _articleCollection.Remove(auxArticle);
                        //foreach (var articleLine in _articleCollection)
                        //{
                        //    if (articleLine.Key == auxArticle)
                        //    {
                        //        _articleCollection.Remove(articleLine.Key);
                        //        if (entrySelected != _entryBoxSelectArticle1) entrySelected = null;
                        //        break;
                        //    }
                        //}
                        entrySelected = null;
                    }
                }
                ValidateDialog();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        //Close popup articles event
        private void _entryBoxSelectArticle_ClosePopup(object sender, EventArgs e)
        {
            try
            {
                var entrySelected = (XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>)sender;

                if (string.IsNullOrEmpty(entrySelected.EntryValidation.Text))
                {
                    return;
                }

                if (entrySelected.Value == _article)
                {
                    entrySelected.Value = null;
                    logicpos.Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Ok, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "dialog_message_composite_article_same"), resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_composite_article"));
                    entrySelected.EntryValidation.Text = "";
                    return;
                }
                entrySelected.CodeEntry.Text = entrySelected.Value.Code;
                entrySelected.EntryQtdValidation.Text = string.Format("{0:0.##}", entrySelected.Value.DefaultQuantity);
                ValidateDialog();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        //Change quantity
        private void QtdEntryValidation_TextInserted(object o, TextInsertedArgs args)
        {
            try
            {
                var entryQtdSelect = (Entry)o;
                var entrySelected = (XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>)entryQtdSelect.Parent.Parent.Parent;
                entrySelected.EntryQtdValidation.Validate();

                if (entrySelected.Value != null && _articleCollection.Count > 0)
                {
                    _articleCollection[entrySelected.Value] = Convert.ToDecimal(entrySelected.EntryQtdValidation.Text);
                }
                ValidateDialog();

            }
            catch (Exception ex)
            {
                _logger.Error("Error updating quantity from article : " + ex.Message);
                ValidateDialog();
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Helper Methods

        /// <summary>
        /// Get ArticleStock Response Object
        /// </summary>
        /// <param name="pSourceWindow"></param>
        /// <returns>PosArticleStockResponse</returns>
        public static ProcessArticleStockParameter GetProcessArticleStockParameter(Window pSourceWindow)
        {
            ProcessArticleStockParameter result = null;

            PosArticleStockDialog dialog = new PosArticleStockDialog(pSourceWindow, DialogFlags.DestroyWithParent | DialogFlags.Modal);
            ResponseType response = (ResponseType)dialog.Run();
            if (response == ResponseType.Ok)
            {
                result = new ProcessArticleStockParameter(
                  dialog.Customer,
                  dialog.DocumentDate,
                  dialog.DocumentNumber,
                  dialog.ArticleCollection,
                  dialog.Quantity,
                  dialog.Notes,
                  null
              );
                //Save to Session
                dialog.Save();
            }
            dialog.Destroy();

            return result;
        }
    }
}
