using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.datalayer.DataLayer.Xpo.Documents;
using logicpos.datalayer.Xpo;
using logicpos.Extensions;
using logicpos.financial.library.Classes.Stocks;
using logicpos.shared.App;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using LogicPOS.Settings.Extensions;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class DialogAddArticleStock : BOBaseDialog
    {
        //UI Components Dialog
        private VBox _vbox;
        //UI Components Form
        private XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _entryBoxSelectSupplier;
        private EntryBoxValidationDatePickerDialog _entryBoxDocumentDate;
        private EntryBoxValidation _entryBoxDocumentNumber;
        private readonly XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle> _entryBoxSelectArticle;
        private readonly EntryBoxValidation _entryBoxQuantity;
        private EntryBoxValidation _entryBoxNotes;
        private HSeparator _separator;
        //InitialValues
        private readonly erp_customer _initialSupplier = null;
        private DateTime _initialDocumentDate;
        private readonly string _initialDocumentNumber;
        //MultiArticles
        private ICollection<VBox> _articleEntryWidgetCollection;
        public ICollection _dropdownTextCollection;
        private XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle> _entryBoxSelectArticle1;
        private EntryBoxValidation _entryBoxSerialNumber1;
        private EntryBoxValidation _entryBoxPrice1;
        private int _totalCompositeEntrys = 0;
        private readonly fin_article _article = null;
        private fin_article _previousValue = null;
        private VBox _vboxArticles;
        private ScrolledWindow _scrolledWindowView;
        private Viewport _viewport;
        private ICollection _collectionSavedArticleSerialNumber;
        private Dictionary<EntryValidation, List<fin_articleserialnumber>> _entrySerialNumberCacheList;
        private Dictionary<EntryValidation, string> _serialNumbersInCache;

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

        public Dictionary<fin_article, Tuple<decimal, Dictionary<EntryValidation, List<fin_articleserialnumber>>, decimal, fin_warehouselocation>> ArticleCollection { get; private set; }
        public decimal Quantity;

        public byte[] AttachedFile;

        public string Notes
        {
            get { return _entryBoxNotes.EntryValidation.Text; }
        }

        public DialogAddArticleStock(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pDialogFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, DialogFlags.Modal, pDialogMode, pXPGuidObject)
        {
            //Init Local Vars
            string windowTitle = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_dialog_article_stock");

            this.Title = windowTitle;

            if (GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600)
            {
                SetSizeRequest(500, 650);
            }
            else
            {
                SetSizeRequest(500, 660);
            }
            string fileDefaultWindowIcon = DataLayerFramework.Path["images"] + @"Icons\Windows\icon_window_stocks.png";
            InitUI();
            //InitNotes();
            ValidateDialog();
            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                //Load Initial Values
                Load();
                //Init VBOX
                _vbox = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
                _vbox.ModifyBase(StateType.Normal, Color.White.ToGdkColor());
                _articleEntryWidgetCollection = new List<VBox>();
                //_articleCollection = new List<fin_article>();
                ArticleCollection = new Dictionary<fin_article, Tuple<decimal, Dictionary<EntryValidation, List<fin_articleserialnumber>>, decimal, fin_warehouselocation>>();
                //Supplier
                CriteriaOperator criteriaOperatorSupplier = CriteriaOperator.Parse("(Supplier = 1)");
                _entryBoxSelectSupplier = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(this, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_supplier"), "Name", "Oid", _initialSupplier, criteriaOperatorSupplier, LogicPOS.Utility.RegexUtils.RegexGuid, true, true);
                _entryBoxSelectSupplier.EntryValidation.IsEditable = true;
                _entryBoxSelectSupplier.EntryValidation.Completion.PopupCompletion = true;
                _entryBoxSelectSupplier.EntryValidation.Completion.InlineCompletion = false;
                _entryBoxSelectSupplier.EntryValidation.Completion.PopupSingleMatch = true;
                _entryBoxSelectSupplier.EntryValidation.Completion.InlineSelection = true;
                _entryBoxSelectSupplier.EntryValidation.Changed += delegate { ValidateDialog(); };

                //DocumentDate
                _entryBoxDocumentDate = new EntryBoxValidationDatePickerDialog(this, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_date"), resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_date"), _initialDocumentDate, LogicPOS.Utility.RegexUtils.RegexDate, true, LogicPOS.Settings.CultureSettings.DateFormat, true);
                //_entryBoxDocumentDate.EntryValidation.Sensitive = true;
                _entryBoxDocumentDate.EntryValidation.Text = _initialDocumentDate.ToString(LogicPOS.Settings.CultureSettings.DateFormat);
                _entryBoxDocumentDate.EntryValidation.Validate();
                _entryBoxDocumentDate.EntryValidation.Sensitive = true;
                _entryBoxDocumentDate.ClosePopup += delegate { ValidateDialog(); };

                //DocumentNumber
                Color colorBaseDialogEntryBoxBackground = LogicPOS.Settings.GeneralSettings.Settings["colorBaseDialogEntryBoxBackground"].StringToColor();
                string _fileIconListFinanceDocuments = DataLayerFramework.Path["images"] + @"Icons\icon_pos_toolbar_finance_document.png";
                HBox hBoxDocument = new HBox(false, 0);
                _entryBoxDocumentNumber = new EntryBoxValidation(this, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_document_number"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false, true);
                if (_initialDocumentNumber != string.Empty) _entryBoxDocumentNumber.EntryValidation.Text = _initialDocumentNumber;
                _entryBoxDocumentNumber.EntryValidation.Changed += delegate { ValidateDialog(); };
                TouchButtonIcon attachPDFButton = new TouchButtonIcon("attachPDFButton", colorBaseDialogEntryBoxBackground, _fileIconListFinanceDocuments, new Size(20, 20), 30, 30);
                attachPDFButton.Clicked += AttachPDFButton_Clicked;
                ((_entryBoxDocumentNumber.Children[0] as VBox).Children[1] as HBox).PackEnd(attachPDFButton, false, false, 0);
                //hBoxDocument.PackStart(_entryBoxDocumentNumber, true, true, 1);
                //hBoxDocument.PackEnd(attachPDFButton, false, false, 1);


                //MultiArticles
                _vboxArticles = new VBox(true, 0);
                HBox hBoxArticles = new HBox(false, 0);
                _separator = new HSeparator();
                //_vboxArticles.PackStart(separator, false, true, 5);
                _scrolledWindowView = new ScrolledWindow();
                _scrolledWindowView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                _scrolledWindowView.ShadowType = ShadowType.EtchedIn;
                _viewport = new Viewport() { ShadowType = ShadowType.None };


                _viewport.ModifyBg(StateType.Normal, Color.White.ToGdkColor());
                _totalCompositeEntrys++;
                CriteriaOperator criteriaOperatorSelectArticle = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Class = '{0}')", DataLayerSettings.XpoOidArticleDefaultClass));
                _entryBoxSelectArticle1 = new XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>(this, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_article"), "Designation", "Oid", null, criteriaOperatorSelectArticle, KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true, true, LogicPOS.Utility.RegexUtils.RegexAlfaNumericArticleCode, LogicPOS.Utility.RegexUtils.RegexDecimalPositiveAndNegative, _totalCompositeEntrys);

                //SerialNumber
                HBox hBoxSerialNumber = new HBox(false, 0);
                _entryBoxSerialNumber1 = new EntryBoxValidation(this, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_serial_number"), KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false, true);
                _entryBoxSerialNumber1.EntryValidation.Changed += EntrySerialNumberValidation_Changed;
                _entryBoxSerialNumber1.EntryValidation.FocusGrabbed += EntryValidation_FocusGrabbed;
                _entryBoxSerialNumber1.Sensitive = true;
                var compositeArticleBtn = new TouchButtonIcon("compositeArticleBtn", Color.FromArgb(255, 0, 0), "", new Size(15, 15), 20, 20);


                //Price
                _entryBoxPrice1 = new EntryBoxValidation(this, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_price"), KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexDecimalGreaterEqualThanZeroFinancial, false, true);
                _entryBoxPrice1.EntryValidation.TooltipText = "Ultimo preço inserido";
                _entryBoxPrice1.WidthRequest = 40;
                _entryBoxPrice1.Sensitive = true;
                _entryBoxPrice1.EntryValidation.Changed += EntryPurchasedPriceValidation_Changed;

                //Warehouse
                CriteriaOperator defaultWarehouseCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND IsDefault == '1'"));
                fin_warehouse defaultWareHouse = (fin_warehouse)XPOSettings.Session.FindObject(typeof(fin_warehouse), defaultWarehouseCriteria);
                XPOComboBox xpoComboBoxWarehouse = new XPOComboBox(XPOSettings.Session, typeof(fin_warehouse), defaultWareHouse, "Designation", null);
                BOWidgetBox boxWareHouse = new BOWidgetBox(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_warehouse"), xpoComboBoxWarehouse);
                xpoComboBoxWarehouse.Changed += XpoComboBoxWarehouse_Changed;

                //Location
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) "));
                if (defaultWareHouse != null) criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND Warehouse == '{0}'", defaultWareHouse.Oid.ToString()));
                fin_warehouselocation defaultLocation = (fin_warehouselocation)XPOSettings.Session.FindObject(typeof(fin_warehouselocation), criteria);
                XPOComboBox xpoComboBoxWarehouseLocation = new XPOComboBox(XPOSettings.Session, typeof(fin_warehouselocation), defaultLocation, "Designation", criteria);
                BOWidgetBox boxWareHouseLocation = new BOWidgetBox(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_ConfigurationDevice_PlaceTerminal"), xpoComboBoxWarehouseLocation);
                xpoComboBoxWarehouseLocation.Changed += XpoComboBoxWarehouselocation_Changed;

                //Unique Articles (Have multi S/N)
                CheckButton _checkButtonUniqueArticles = new CheckButton(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_unique_articles"));
                _checkButtonUniqueArticles.Sensitive = false;
                _checkButtonUniqueArticles.Toggled += CheckButtonUniqueArticles_Toggled;

                if (defaultWareHouse == null)
                {
                    xpoComboBoxWarehouseLocation.Active = 0;
                    xpoComboBoxWarehouseLocation.Sensitive = false;
                }

                hBoxArticles.PackStart(boxWareHouse, true, true, 2);
                hBoxArticles.PackStart(boxWareHouseLocation, true, true, 2);
                hBoxArticles.PackStart(_entryBoxPrice1, false, false, 2);
                //hBoxArticles.PackStart(_checkButtonUniqueArticles, false, false, 0);

                _entryBoxSelectArticle1.EntryValidation.IsEditable = true;
                _entryBoxSelectArticle1.EntryQtdValidation.IsEditable = true;
                _entryBoxSelectArticle1.Value = null;
                _entryBoxSelectArticle1.EntryValidation.Text = "";

                _entryBoxSelectArticle1.EntryValidation.Validate();
                _entryBoxSelectArticle1.EntryCodeValidation.Validate();
                _entryBoxSelectArticle1.EntryQtdValidation.Validate();

                //Events Composition
                _entryBoxSelectArticle1.ClosePopup += _entryBoxSelectArticle_ClosePopup;
                _entryBoxSelectArticle1.CleanArticleEvent += EntryBoxSelectArticle_CleanArticleEvent;
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
                    SelectRecordDropDownArticle(false, _entryBoxSelectArticle1, _entryBoxSerialNumber1, _entryBoxPrice1, xpoComboBoxWarehouseLocation.Value as fin_warehouselocation);
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
                    SelectRecordDropDownArticle(true, _entryBoxSelectArticle1, _entryBoxSerialNumber1, _entryBoxPrice1, xpoComboBoxWarehouseLocation.Value as fin_warehouselocation);
                };

                _vboxArticles.PackStart(_entryBoxSelectArticle1, false, false, 0);
                _vboxArticles.PackStart(hBoxArticles, false, false, 0);
                _vboxArticles.PackStart(_entryBoxSerialNumber1);
                _separator = new HSeparator();
                _vboxArticles.PackStart(_separator, false, true, 5);

                _articleEntryWidgetCollection.Add(_vboxArticles);

                //Notes
                _entryBoxNotes = new EntryBoxValidation(this, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_notes"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false, true);
                _entryBoxNotes.EntryValidation.Changed += delegate { ValidateDialog(); };

                //Final Pack
                _vbox.PackStart(_entryBoxSelectSupplier, false, false, 0);
                _vbox.PackStart(_entryBoxDocumentDate, false, false, 0);
                _vbox.PackStart(_entryBoxDocumentNumber, false, false, 0);
                _vbox.PackStart(_entryBoxNotes, false, false, 0);
                _vbox.PackStart(_vboxArticles, false, false, 0);

                _vbox.ModifyFg(StateType.Normal, Color.White.ToGdkColor());
                _vbox.ModifyBg(StateType.Normal, Color.White.ToGdkColor());
                //_vbox.PackStart(_entryBoxQuantity, false, false, 0);

                _viewport.Add(_vbox);
                _scrolledWindowView.Add(_viewport);

                this.ModifyFg(StateType.Normal, Color.White.ToGdkColor());
                this.ModifyBg(StateType.Normal, Color.White.ToGdkColor());

                //Append Tab
                _notebook.AppendPage(_scrolledWindowView, new Label("Inserir Artigos"));
                _notebook.ModifyFg(StateType.Normal, Color.White.ToGdkColor());
                _notebook.ModifyBg(StateType.Normal, Color.White.ToGdkColor());
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void CheckButtonUniqueArticles_Toggled(object sender, EventArgs e)
        {
            try
            {
                var toggle = sender as CheckButton;
                var entryArticleSelected = (XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>)(toggle.Parent.Parent as VBox).Children[0];
                if (entryArticleSelected.Value != null)
                {
                    (entryArticleSelected.Value as fin_article).UniqueArticles = toggle.Active;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private void EntryValidation_FocusGrabbed(object sender, EventArgs e)
        {
            try
            {
                var SelectedAssocietedArticles = new List<fin_articleserialnumber>();
                var entrySerialNumberSelected = (EntryValidation)sender;
                var entryArticleSelected = (XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>)(entrySerialNumberSelected.Parent.Parent.Parent.Parent as VBox).Children[0];

                if (!(entryArticleSelected.Value as fin_article).IsComposed) return;

                if (!string.IsNullOrEmpty(entrySerialNumberSelected.Text))
                {
                    if (ArticleCollection.ContainsKey(entryArticleSelected.Value as fin_article))
                    {
                        SelectedAssocietedArticles = ArticleCollection[entryArticleSelected.Value as fin_article].Item2[entrySerialNumberSelected];

                        entrySerialNumberSelected.Text = logicpos.Utils.OpenNewSerialNumberCompositePopUpWindow(this, entryArticleSelected.Value, out SelectedAssocietedArticles, entrySerialNumberSelected.Text, SelectedAssocietedArticles);
                    }
                }

                else if (entryArticleSelected.Value != null && (entryArticleSelected.Value as fin_article).IsComposed)
                {
                    entrySerialNumberSelected.Text = logicpos.Utils.OpenNewSerialNumberCompositePopUpWindow(this, entryArticleSelected.Value, out SelectedAssocietedArticles, "");
                }

                fin_article selectedArticle = new fin_article();
                selectedArticle = ((entrySerialNumberSelected.Parent.Parent.Parent.Parent as VBox).Children[0] as XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>).Value;
                if (ArticleCollection.Count > 0)
                {
                    Dictionary<EntryValidation, List<fin_articleserialnumber>> serialNumberList;
                    if (ArticleCollection[selectedArticle].Item2 == null)
                    {
                        serialNumberList = new Dictionary<EntryValidation, List<fin_articleserialnumber>>
                        {
                            { entrySerialNumberSelected, SelectedAssocietedArticles }
                        };
                    }
                    else
                    {
                        serialNumberList = ArticleCollection[selectedArticle].Item2;
                        serialNumberList[entrySerialNumberSelected] = SelectedAssocietedArticles;
                        if (!serialNumberList.ContainsKey(entrySerialNumberSelected))
                        {
                            serialNumberList.Add(entrySerialNumberSelected, SelectedAssocietedArticles);
                        }
                    }
                    var newTuple = new Tuple<decimal, Dictionary<EntryValidation, List<fin_articleserialnumber>>, decimal, fin_warehouselocation>(ArticleCollection[selectedArticle].Item1, serialNumberList, ArticleCollection[selectedArticle].Item3, ArticleCollection[selectedArticle].Item4);
                    ArticleCollection[selectedArticle] = newTuple;
                    ValidateDialog();
                    return;
                }
                ValidateDialog();

            }
            catch (Exception ex)
            {
                _logger.Error("Error opening Composite article window " + ex.Message);
            }
        }

        private void AttachPDFButton_Clicked(object sender, EventArgs e)
        {
            FileFilter fileFilterPDF = logicpos.Utils.GetFileFilterPDF();
            PosFilePickerDialog dialog = new PosFilePickerDialog(this, DialogFlags.DestroyWithParent, fileFilterPDF, FileChooserAction.Open);
            ResponseType response = (ResponseType)dialog.Run();
            if (response == ResponseType.Ok)
            {
                string fileNamePacked = dialog.FilePicker.Filename;
                string fileName = string.Format("{0}/", System.IO.Path.GetFileNameWithoutExtension(fileNamePacked));
                AttachedFile = File.ReadAllBytes(fileNamePacked);
                _entryBoxDocumentNumber.EntryValidation.Text = fileName.Replace("/", "");
                dialog.Destroy();
            }
            else { dialog.Destroy(); }
        }

        private void ValidateDialog()
        {
            int validateEntrys = 0;
            foreach (var item in _articleEntryWidgetCollection)
            {
                foreach (var widgt in item)
                {
                    //if(widgt.GetType() == typeOf())
                }
                //item.EntryCodeValidation.Validate();
                //item.EntryQtdValidation.Validate();
                //item.EntryValidation.Validate();
                //if (!item.EntryCodeValidation.Validated || !item.EntryQtdValidation.Validated || !item.EntryValidation.Validated)
                //{
                //    validateEntrys++;
                //}
            }
            bool multiEntrysValidated = true;
            //_entryBoxPrice1.Sensitive = _articleCollection.Count > 0;
            //_entryBoxSerialNumber1.Sensitive = _articleCollection.Count > 0;
            if (validateEntrys > 0) multiEntrysValidated = false;
            buttonOk.Sensitive = (
                _entryBoxSelectSupplier.EntryValidation.Validated &&
                _entryBoxDocumentDate.EntryValidation.Validated &&
                _entryBoxDocumentNumber.EntryValidation.Validated &&
                _entryBoxSelectArticle1.EntryValidation.Validated &&
                _entryBoxNotes.EntryValidation.Validated && multiEntrysValidated && ArticleCollection.Count > 0
            );
        }

        public void Load()
        {
            _serialNumbersInCache = new Dictionary<EntryValidation, string>();

            //Get From Session if Exists
            object supplier = SharedFramework.SessionApp.GetToken(string.Format("{0}_{1}", this.GetType().Name, "supplier").ToUpper());
            object documentDate = SharedFramework.SessionApp.GetToken(string.Format("{0}_{1}", this.GetType().Name, "documentDate").ToUpper());
            //object documentNumber = GlobalFramework.SessionApp.GetToken(string.Format("{0}_{1}", this.GetType().Name, "documentNumber").ToUpper());
            //Assign if Valid
            try
            {
               //if (supplier != null) //_initialSupplier = (erp_customer)XPOSettings.Session.GetObjectByKey(typeof(erp_customer), new Guid(supplier.ToString()));
                var own_customer = (erp_customer)XPOSettings.Session.GetObjectByKey(typeof(erp_customer), SharedSettings.XpoOidUserRecord);
                if (own_customer != null && string.IsNullOrEmpty(own_customer.Name))
                {
                    //update owner customer for internal stock moviments                        
                    //own_customer.FiscalNumber = CryptorEngine.Encrypt(GlobalFramework.PreferenceParameters["COMPANY_FISCALNUMBER"], true, SettingsApp.SecretKey);
                    own_customer.FiscalNumber = LogicPOS.Settings.GeneralSettings.PreferenceParameters["COMPANY_FISCALNUMBER"];
                    own_customer.Name = LogicPOS.Settings.GeneralSettings.PreferenceParameters["COMPANY_NAME"];
                    own_customer.Save();
                    _logger.Debug("Updating own supplier name and fiscal number");
                    //if (supplier == null) { supplier = own_customer; }
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
                //_initialDocumentNumber = (documentNumber != null) ? Convert.ToString(documentNumber) : string.Empty;
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
        private void SelectRecordDropDownArticle(bool isArticleCode, XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle> pXPOEntry, EntryBoxValidation pSerialNumber, EntryBoxValidation pPurchasePrice, fin_warehouselocation pWarehouselocation)
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

                        pXPOEntry.EntryValidation.Text = (newArticle != null) ? newArticle.Designation.ToString() : resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error");

                        pXPOEntry.EntryValidation.Changed += delegate { pXPOEntry.EntryValidation.Validate(); };

                        pXPOEntry.EntryQtdValidation.Text = (newArticle != null) ? string.Format("{0:0.##}", newArticle.DefaultQuantity) : resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error");

                        pXPOEntry.EntryCodeValidation.Validate();

                        pXPOEntry.EntryValidation.Validate();

                        pXPOEntry.EntryQtdValidation.Validate();

                        object getLastPrice;

                        if (Customer != null)
                        {
                            getLastPrice = XPOSettings.Session.ExecuteScalar(string.Format("SELECT TOP(1) PurchasePrice FROM fin_articlestock WHERE Article = '{0}' AND Customer = '{1}'", articleOid, Customer.Oid));
                            if (getLastPrice == null)
                            {
                                getLastPrice = XPOSettings.Session.ExecuteScalar(string.Format("SELECT TOP(1) PurchasePrice FROM fin_articlestock WHERE Article = '{0}'", articleOid));
                            }
                            pPurchasePrice.EntryValidation.TooltipText = "Ultimo preço inserido do fornecedor";
                        }
                        else
                        {
                            getLastPrice = XPOSettings.Session.ExecuteScalar(string.Format("SELECT TOP(1) PurchasePrice FROM fin_articlestock WHERE Article = '{0}'", articleOid));
                            pPurchasePrice.EntryValidation.TooltipText = "Ultimo preço inserido";
                        }

                        pPurchasePrice.EntryValidation.Text = (getLastPrice != null) ? LogicPOS.Utility.DataConversionUtils.DecimalToString(Convert.ToDecimal(getLastPrice), "0.00").Replace(".", ",") : LogicPOS.Utility.DataConversionUtils.StringToDecimal("0,00").ToString();

                    
                        return;
                    }
                    pXPOEntry.EntryValidation.Changed -= delegate { pXPOEntry.EntryValidation.Validate(); };

                    pXPOEntry.EntryValidation.Text = (newArticle != null) ? newArticle.Designation.ToString() : resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error");

                    pXPOEntry.EntryValidation.Changed += delegate { pXPOEntry.EntryValidation.Validate(); };

                    pXPOEntry.EntryCodeValidation.Text = (newArticle != null) ? newArticle.Code.ToString() : resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error");

                    pXPOEntry.EntryQtdValidation.Text = (newArticle != null) ? string.Format("{0:0.##}", newArticle.DefaultQuantity) : resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error");

                    pXPOEntry.Value = newArticle;

                    pXPOEntry.EntryCodeValidation.Validate();

                    pXPOEntry.EntryValidation.Validate();

                    pXPOEntry.EntryQtdValidation.Validate();

                    //if ((pXPOEntry.Value as fin_article).IsComposed)
                    //{
                    //    Utils.OpenNewPopUpWindow(this, pXPOEntry.Value);
                    //}

                    //Clean previous value from colection
                    if (_previousValue != null)
                    {
                        ArticleCollection.Remove(_previousValue);
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
                        logicpos.Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Ok, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_composite_article_same"), resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_composite_article"));
                        pXPOEntry.EntryValidation.Text = "";
                        ValidateDialog();
                        return;
                    }
                    var newSerialNumberList = new Dictionary<EntryValidation, List<fin_articleserialnumber>>();
                    decimal price = Convert.ToDecimal(pPurchasePrice.EntryValidation.Text);
                    ArticleCollection.Add(pXPOEntry.Value, new Tuple<decimal, Dictionary<EntryValidation, List<fin_articleserialnumber>>, decimal, fin_warehouselocation>(0, newSerialNumberList, price, pWarehouselocation));

                    //if (_article.IsComposed)
                    //{
                    //    foreach (var entrySerialNumber in _articleCollection[_article].Item2)
                    //    {
                    //        entrySerialNumber.Key.Sensitive = true;
                    //    }
                    //}
                    //else
                    //{
                    //    foreach (var entrySerialNumber in _articleCollection[_article].Item2)
                    //    {
                    //        entrySerialNumber.Key.Sensitive = false;
                    //    }
                    //}

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

                _dropdownTextCollection = XPOSettings.Session.GetObjects(XPOSettings.Session.GetClassInfo(typeof(fin_article)), pCriteria, sortCollection, int.MaxValue, false, true);

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
                VBox box1 = new VBox(true, 0);

                _totalCompositeEntrys++;
                //var entrySelected = (XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>)sender;
                CriteriaOperator criteriaOperatorSelectArticle = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1)"));
                XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle> NewEntryBoxSelectArticle = new XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>(this, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_article"), "Designation", "Oid", null, criteriaOperatorSelectArticle, KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true, true, LogicPOS.Utility.RegexUtils.RegexAlfaNumericArticleCode, LogicPOS.Utility.RegexUtils.RegexDecimalPositiveAndNegative, _totalCompositeEntrys);

                HBox hBoxArticles = new HBox(false, 0);

                //SerialNumber
                EntryBoxValidation NewEntryBoxSerialNumber = new EntryBoxValidation(this, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_serial_number"), KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false, true);
                NewEntryBoxSerialNumber.EntryValidation.Changed += delegate { ValidateDialog(); };
                NewEntryBoxSerialNumber.EntryValidation.Changed += EntrySerialNumberValidation_Changed;
                NewEntryBoxSerialNumber.EntryValidation.FocusGrabbed += EntryValidation_FocusGrabbed;

                //Price
                EntryBoxValidation NewEntryBoxPrice = new EntryBoxValidation(this, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_price"), KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexDecimalGreaterEqualThanZeroFinancial, false, true);
                NewEntryBoxPrice.EntryValidation.Changed += EntryPurchasedPriceValidation_Changed;
                NewEntryBoxPrice.WidthRequest = 40;

                //Warehouse
                CriteriaOperator defaultWarehouseCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND IsDefault == '1'"));
                fin_warehouse defaultWareHouse = (fin_warehouse)XPOSettings.Session.FindObject(typeof(fin_warehouse), defaultWarehouseCriteria);
                XPOComboBox xpoComboBoxWarehouse = new XPOComboBox(XPOSettings.Session, typeof(fin_warehouse), defaultWareHouse, "Designation", null);
                BOWidgetBox boxWareHouse = new BOWidgetBox(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_warehouse"), xpoComboBoxWarehouse);
                xpoComboBoxWarehouse.Changed += XpoComboBoxWarehouse_Changed;

                //Location
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) "));
                if (defaultWareHouse != null) criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND Warehouse == '{0}'", defaultWareHouse.Oid.ToString()));
                fin_warehouselocation defaultLocation = (fin_warehouselocation)XPOSettings.Session.FindObject(typeof(fin_warehouselocation), criteria);
                XPOComboBox xpoComboBoxWarehouseLocation = new XPOComboBox(XPOSettings.Session, typeof(fin_warehouselocation), defaultLocation, "Designation", criteria);
                BOWidgetBox boxWareHouseLocation = new BOWidgetBox(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_ConfigurationDevice_PlaceTerminal"), xpoComboBoxWarehouseLocation);
                xpoComboBoxWarehouseLocation.Changed += XpoComboBoxWarehouselocation_Changed;

                NewEntryBoxSelectArticle.EntryValidation.IsEditable = true;
                NewEntryBoxSelectArticle.Value = null;
                NewEntryBoxSelectArticle.EntryValidation.Text = "";
                NewEntryBoxSelectArticle.EntryCodeValidation.Text = "";
                NewEntryBoxSelectArticle.EntryQtdValidation.Text = "";
                NewEntryBoxSelectArticle.EntryCodeValidation.Validate();
                NewEntryBoxSelectArticle.EntryQtdValidation.Validate();


                hBoxArticles.PackStart(boxWareHouse, true, true, 2);
                hBoxArticles.PackStart(boxWareHouseLocation, true, true, 2);
                hBoxArticles.PackStart(NewEntryBoxPrice, false, false, 2);

                box1.PackStart(NewEntryBoxSelectArticle, false, false, 0);
                box1.PackStart(hBoxArticles, false, false, 0);
                box1.PackStart(NewEntryBoxSerialNumber);

                box1.PackStart(_separator, false, true, 5);

                _articleEntryWidgetCollection.Add(box1);
                _vbox.Add(box1);

                //Events
                NewEntryBoxSelectArticle.ClosePopup += _entryBoxSelectArticle_ClosePopup;
                NewEntryBoxSelectArticle.CleanArticleEvent += EntryBoxSelectArticle_CleanArticleEvent;
                NewEntryBoxSelectArticle.AddNewEntryEvent += NewBox_AddNewEntryEvent;
                NewEntryBoxSelectArticle.EntryQtdValidation.TextInserted += QtdEntryValidation_TextInserted;
                NewEntryBoxSelectArticle.ShowAll();
                _vbox.ShowAll();

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
                    SelectRecordDropDownArticle(false, NewEntryBoxSelectArticle, NewEntryBoxSerialNumber, NewEntryBoxPrice, xpoComboBoxWarehouseLocation.Value as fin_warehouselocation);
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
                    SelectRecordDropDownArticle(true, NewEntryBoxSelectArticle, NewEntryBoxSerialNumber, NewEntryBoxPrice, xpoComboBoxWarehouseLocation.Value as fin_warehouselocation);
                };

                NewEntryBoxSelectArticle.EntryValidation.Validate();
                NewEntryBoxSelectArticle.EntryCodeValidation.Validate();
                NewEntryBoxSelectArticle.EntryQtdValidation.Validate();

                _viewport.Add(_vbox);
                //_scrolledWindowView.Add(_viewport);


                //_scrolledWindowView.Add(_vbox);
                //eventBoxPosCompositionView.Add(scrolledWindowCompositionView);
                ValidateDialog();
            }
            catch (Exception ex)
            {
                _logger.Error("Error Adding new Composite article Entry : " + ex.Message);
            }
        }

        //Clean article event
        private void EntryBoxSelectArticle_CleanArticleEvent(object sender, EventArgs e)
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
                        if (_totalCompositeEntrys > 1 && _articleEntryWidgetCollection.Count > 1)
                        {
                            //foreach (var line in _articleEntryWidgetCollection)
                            //{
                            //    if (line.EntryNumber == _totalCompositeEntrys)
                            //    {
                            //        _entryBoxSelectArticle1.Value = line.Value;
                            //        _entryBoxSelectArticle1.EntryValidation.Text = line.EntryValidation.Text;
                            //        _entryBoxSelectArticle1.EntryQtdValidation.Text = line.EntryQtdValidation.Text;
                            //        _entryBoxSelectArticle1.CodeEntry.Text = line.CodeEntry.Text;
                            //        _entryBoxSelectArticle1.EntryValidation.Validate();
                            //        _entryBoxSelectArticle1.EntryCodeValidation.Validate();
                            //        _entryBoxSelectArticle1.EntryQtdValidation.Validate();


                            //        line.Hide();
                            //        _articleEntryWidgetCollection.Remove(line);
                            //        line.Value = null;
                            //        _totalCompositeEntrys--;
                            //        cleanFirstEntry = true;
                            //        break;
                            //    }
                            //}
                        }
                        else
                        {
                            _entryBoxSelectArticle1.EntryValidation.Text = "";
                            _entryBoxSelectArticle1.EntryQtdValidation.Text = "0";
                            _entryBoxSelectArticle1.CodeEntry.Text = "";
                            _entryBoxSelectArticle1.EntryValidation.Validate();
                            _entryBoxSelectArticle1.EntryCodeValidation.Validate();
                            _entryBoxSelectArticle1.EntryQtdValidation.Validate();
                            _entryBoxSerialNumber1.EntryValidation.Text = "";
                            _entryBoxPrice1.EntryValidation.Text = "";
                        }
                    }
                    else
                    {
                        entrySelected.Hide();

                        entrySelected.Parent.Hide();
                        _totalCompositeEntrys--;

                        //_articleEntryWidgetCollection.Remove(entrySelected);

                    }
                    if (entrySelected.Value != null)
                    {
                        fin_article auxArticle = new fin_article();

                        if (cleanFirstEntry)
                        {
                            auxArticle = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), articleToDeleteAux);
                        }
                        else
                        {
                            auxArticle = entrySelected.Value;
                        }
                        ArticleCollection.Remove(auxArticle);
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
                    logicpos.Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Ok, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_composite_article_same"), resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_composite_article"));
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
                if (entryQtdSelect.Text == "0") entryQtdSelect.Text = "1";
                var entrySelected = (XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>)entryQtdSelect.Parent.Parent.Parent;
                entrySelected.EntryQtdValidation.Validate();

                if (entrySelected.Value != null && ArticleCollection.Count > 0)
                {
                    var iTuple = new Tuple<decimal, Dictionary<EntryValidation, List<fin_articleserialnumber>>, decimal, fin_warehouselocation>(Convert.ToDecimal(entrySelected.EntryQtdValidation.Text), ArticleCollection[entrySelected.Value].Item2, ArticleCollection[entrySelected.Value].Item3, ArticleCollection[entrySelected.Value].Item4);
                    ArticleCollection[entrySelected.Value] = iTuple;
                }
                ValidateDialog();
                int countSerialNumberEntrys = 0;
                ArrayList serialNumberEntrys = new ArrayList();
                foreach (var item in (entryQtdSelect.Parent.Parent.Parent.Parent as VBox).Children)
                {
                    if (item.GetType() == typeof(EntryBoxValidation))
                    {
                        serialNumberEntrys.Add(item as EntryBoxValidation);
                        countSerialNumberEntrys++;
                    }
                }
                if (entrySelected.Value != null && (entrySelected.Value as fin_article).UnitMeasure.Code == 10)
                {
                    if (Convert.ToInt32(entrySelected.EntryQtdValidation.Text) > 50){
                        entrySelected.EntryQtdValidation.TooltipText = "Numero demasiado grande para adicionar SN!";
                        return;
                    }
                    else if (countSerialNumberEntrys < Convert.ToInt32(entrySelected.EntryQtdValidation.Text))
                    {
                        (entryQtdSelect.Parent.Parent.Parent.Parent as VBox).Remove(_separator);
                        for (int i = countSerialNumberEntrys; i < Convert.ToInt32(entrySelected.EntryQtdValidation.Text); i++)
                        {
                            (entryQtdSelect.Parent.Parent.Parent.Parent as VBox).Add(AddNewSerialNumber());
                        }
                        (entryQtdSelect.Parent.Parent.Parent.Parent as VBox).Add(_separator);
                    }
                    else if (countSerialNumberEntrys > Convert.ToInt32(entrySelected.EntryQtdValidation.Text))
                    {

                        for (int i = countSerialNumberEntrys - 1; i >= Convert.ToInt32(entrySelected.EntryQtdValidation.Text); i--)
                        {
                            (entryQtdSelect.Parent.Parent.Parent.Parent as VBox).Remove(serialNumberEntrys[i] as EntryBoxValidation);
                            serialNumberEntrys.RemoveAt(i);
                        }
                    }
                    entrySelected.EntryQtdValidation.TooltipText = "";
                }
                //(((entryQtdSelect.Parent.Parent.Parent.Parent as VBox).Children[1] as HBox).Children[3] as CheckButton).Sensitive = true;
                (entryQtdSelect.Parent.Parent.Parent.Parent as VBox).ShowAll();
            }
            catch (Exception ex)
            {
                _logger.Error("Error updating quantity from article : " + ex.Message);
                ValidateDialog();
            }
        }

        //Add new SerialNumber Entry
        private EntryBoxValidation AddNewSerialNumber()
        {
            //SerialNumber
            EntryBoxValidation NewEntryBoxSerialNumber = new EntryBoxValidation(this, "", KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false, true);
            NewEntryBoxSerialNumber.EntryValidation.Changed += EntrySerialNumberValidation_Changed;
            NewEntryBoxSerialNumber.EntryValidation.FocusGrabbed += EntryValidation_FocusGrabbed;
            return NewEntryBoxSerialNumber;
        }

        //Change SerialNumber
        private void EntrySerialNumberValidation_Changed(object sender, EventArgs e)
        {
            try
            {
                var entrySerialNumber = (EntryValidation)sender;
                fin_article selectedArticle = new fin_article();
                selectedArticle = ((entrySerialNumber.Parent.Parent.Parent.Parent as VBox).Children[0] as XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>).Value;

                //Check if exists
                //Get all article serial Number to check if already exists 
                SortingCollection sortCollection = new SortingCollection
                {
                    new SortProperty("SerialNumber", DevExpress.Xpo.DB.SortingDirection.Ascending)
                };
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND SerialNumber == '{0}'", entrySerialNumber.Text));
                _collectionSavedArticleSerialNumber = XPOSettings.Session.GetObjects(XPOSettings.Session.GetClassInfo(typeof(fin_articleserialnumber)), criteria, sortCollection, int.MaxValue, false, true);

                if((_collectionSavedArticleSerialNumber != null && _collectionSavedArticleSerialNumber.Count > 0) || (_serialNumbersInCache.ContainsValue(entrySerialNumber.Text)))
                {
                    entrySerialNumber.Validated = false;
                    entrySerialNumber.TooltipText = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_serial_number") + " já existe!";
                    buttonOk.Sensitive = false;
                    return;
                }
                else
                {
                    entrySerialNumber.Validated = true;
                    entrySerialNumber.TooltipText = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_serial_number");
                }

                if (ArticleCollection.Count > 0 && entrySerialNumber.Validated)
                {
                    
                    if (ArticleCollection[selectedArticle].Item2 == null)
                    {
                        _entrySerialNumberCacheList = new Dictionary<EntryValidation, List<fin_articleserialnumber>>
                        {
                            { entrySerialNumber, null }
                        };
                    }
                    else
                    {
                        _entrySerialNumberCacheList = ArticleCollection[selectedArticle].Item2;
                        if (!_entrySerialNumberCacheList.ContainsKey(entrySerialNumber))
                        {
                            _entrySerialNumberCacheList.Add(entrySerialNumber, null);
                        }
                    }
                    var newTuple = new Tuple<decimal, Dictionary<EntryValidation, List<fin_articleserialnumber>>, decimal, fin_warehouselocation>(ArticleCollection[selectedArticle].Item1, _entrySerialNumberCacheList, ArticleCollection[selectedArticle].Item3, ArticleCollection[selectedArticle].Item4);
                    ArticleCollection[selectedArticle] = newTuple;

                    if (!_serialNumbersInCache.ContainsKey(entrySerialNumber))
                    {
                        _serialNumbersInCache.Add(entrySerialNumber, entrySerialNumber.Text);
                    }
                    else
                    {
                        _serialNumbersInCache[entrySerialNumber] = entrySerialNumber.Text;
                    }
                    if (((((((entrySerialNumber.Parent.Parent.Parent.Parent as VBox).Children[1] as HBox).Children[1] as BOWidgetBox).Children[1] as XPOComboBox).Value == null) || (((((entrySerialNumber.Parent.Parent.Parent.Parent as VBox).Children[1] as HBox).Children[0] as BOWidgetBox).Children[1] as XPOComboBox).Value == null)) && !string.IsNullOrEmpty(entrySerialNumber.Text))
                    {
                        buttonOk.Sensitive = false;
                        entrySerialNumber.TooltipText = "Por favor insira um armazém primeiro";
                        return;
                    }
                    else
                    {
                        buttonOk.Sensitive = true;
                    }
                    if (!entrySerialNumber.Validated)
                    {
                        buttonOk.Sensitive = false;
                        return;
                    }

                    ValidateDialog();
                    return;
                }
                ValidateDialog();

            }
            catch (Exception ex)
            {
                _logger.Error("Error updating serialNumber from article : " + ex.Message);
                ValidateDialog();
            }
        }

        //Change Purchased Price
        private void EntryPurchasedPriceValidation_Changed(object sender, EventArgs e)
        {
            try
            {
                var entryPrice = (EntryValidation)sender;
                fin_article selectedArticle = new fin_article();
                if (ArticleCollection.Count > 0)
                {
                    selectedArticle = ((entryPrice.Parent.Parent.Parent.Parent.Parent as VBox).Children[0] as XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>).Value;
                    var newTuple = new Tuple<decimal, Dictionary<EntryValidation, List<fin_articleserialnumber>>, decimal, fin_warehouselocation>(ArticleCollection[selectedArticle].Item1, ArticleCollection[selectedArticle].Item2, Convert.ToDecimal(entryPrice.Text), ArticleCollection[selectedArticle].Item4);
                    ArticleCollection[selectedArticle] = newTuple;
                    ValidateDialog();
                    return;
                }
                ValidateDialog();

            }
            catch (Exception ex)
            {
                _logger.Error("Error updating Purchased Price from article : " + ex.Message);
                ValidateDialog();
            }
        }

        //Change WareHouse
        private void XpoComboBoxWarehouse_Changed(object sender, EventArgs e)
        {
            try
            {
                var selectedWareHouseCB = sender as XPOComboBox;
                fin_article selectedArticle = new fin_article();
                var wareHouseLocationCB = ((selectedWareHouseCB.Parent.Parent as HBox).Children[1] as BOWidgetBox).Children[1] as XPOComboBox;
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
                if (wareHouseLocationCB.Value != null) criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND Warehouse == '{0}'", selectedWareHouseCB.Value.Oid.ToString()));
                wareHouseLocationCB.UpdateModel(criteria, null);

                if (ArticleCollection.Count > 0)
                {
                    selectedArticle = ((selectedWareHouseCB.Parent.Parent.Parent as VBox).Children[0] as XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>).Value;
                    var newTuple = new Tuple<decimal, Dictionary<EntryValidation, List<fin_articleserialnumber>>, decimal, fin_warehouselocation>(ArticleCollection[selectedArticle].Item1, ArticleCollection[selectedArticle].Item2, ArticleCollection[selectedArticle].Item3, wareHouseLocationCB.Value as fin_warehouselocation);
                    ArticleCollection[selectedArticle] = newTuple;                
                }

                if (selectedWareHouseCB.Value != null)
                {
                    ((selectedWareHouseCB.Parent.Parent as HBox).Children[1] as BOWidgetBox).Children[1].Sensitive = true;
                }
                ValidateDialog();

            }
            catch (Exception ex)
            {
                _logger.Error("Error change warehouse Location model : " + ex.Message);
            }
        }

        //Change WareHouse Location
        private void XpoComboBoxWarehouselocation_Changed(object sender, EventArgs e)
        {
            try
            {
                var selectedWareHouseLocationCB = sender as XPOComboBox;
                if(selectedWareHouseLocationCB.Value == null)
                {
                    buttonOk.Sensitive = false;
                    return;
                }

                fin_article selectedArticle = new fin_article();

                if (ArticleCollection.Count > 0)
                {
                    selectedArticle = ((selectedWareHouseLocationCB.Parent.Parent.Parent as VBox).Children[0] as XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>).Value;
                    var newTuple = new Tuple<decimal, Dictionary<EntryValidation, List<fin_articleserialnumber>>, decimal, fin_warehouselocation>(ArticleCollection[selectedArticle].Item1, ArticleCollection[selectedArticle].Item2, ArticleCollection[selectedArticle].Item3, selectedWareHouseLocationCB.Value as fin_warehouselocation);
                    ArticleCollection[selectedArticle] = newTuple;
                    ValidateDialog();
                    return;
                }

                ValidateDialog();

            }
            catch (Exception ex)
            {
                _logger.Error("Error Updating WarehouseLocation Entry : " + ex.Message);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Helper Methods

        /// <summary>
        /// Get ArticleStock Response Object
        /// </summary>
        /// <param name="pSourceWindow"></param>
        /// <returns>PosArticleStockResponse</returns>
        public static ProcessArticleStockParameter GetProcessArticleStockParameter(DialogAddArticleStock pDialog)
        {

            //Convert Entry Serial Numbers to string
            Dictionary<string, List<fin_articleserialnumber>> serialNumbers = new Dictionary<string, List<fin_articleserialnumber>>();
            var ProcessArticleCollection = new Dictionary<fin_article, Tuple<decimal, Dictionary<string, List<fin_articleserialnumber>>, decimal, fin_warehouselocation>>();
            foreach (var articles in pDialog.ArticleCollection)
            {
                serialNumbers = new Dictionary<string, List<fin_articleserialnumber>>();
                if (articles.Value.Item2.Count > 0)
                {
                    foreach (var entry in articles.Value.Item2)
                    {
                        serialNumbers.Add(entry.Key.Text, entry.Value);
                    }
                }
                var tuple = new Tuple<decimal, Dictionary<string, List<fin_articleserialnumber>>, decimal, fin_warehouselocation>(articles.Value.Item1, serialNumbers, articles.Value.Item3, articles.Value.Item4);
                ProcessArticleCollection.Add(articles.Key, tuple);
            }
            ProcessArticleStockParameter result = new ProcessArticleStockParameter(
      pDialog.Customer,
      pDialog.DocumentDate,
      pDialog.DocumentNumber,
      ProcessArticleCollection,
      pDialog.Quantity,
      pDialog.Notes,
      pDialog.AttachedFile
  );
            //Save to Session
            pDialog.Save();
            pDialog.Destroy();
            return result;
        }

        //attach PDF to DataBase
        public static void AttachPDFtoStockMoviment(Window pSourceWindow)
        {

            FileFilter fileFilterBackups = logicpos.Utils.GetFileFilterBackups();
            PosFilePickerDialog dialog = new PosFilePickerDialog(pSourceWindow, DialogFlags.DestroyWithParent, fileFilterBackups, FileChooserAction.Open);
            ResponseType response = (ResponseType)dialog.Run();
            if (response == ResponseType.Ok)
            {
                string fileNamePacked = dialog.FilePicker.Filename;
                //string pathFile = string.Format("{0}/", Path.GetDirectoryName(fileNamePacked));

                dialog.Destroy();
            }
        }

    }
}
