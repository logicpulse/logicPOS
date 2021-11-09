using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.BackOffice.Dialogs.Articles
{
    class DialogArticleCompositionSerialNumber : BOBaseDialog
    {
        //UI
        VBox vboxTab1;
        VBox vboxTab2;
        VBox vboxTab3;
        VBox vboxTab4;
        fin_article _selectedArticle;
        XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber> _entryBoxArticleSerialNumber;
        XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber> _entryBoxArticleSerialNumberToChange;
        XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber> _entryBoxArticleSerialNumberCompositionArticles;
        XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>  _entryBoxSelectSupplier;
        XPOEntryBoxSelectRecordValidation<fin_documentfinancemaster, TreeViewDocumentFinanceMaster> _entryBoxSelectDocumentOut;
        EntryBoxValidation _entryBoxDocumentNumber;
        EntryBoxValidation _entryBoxPrice1;
        EntryBoxValidationDatePickerDialog _entryBoxDocumentDateIn;
        EntryBoxValidationDatePickerDialog _entryBoxDocumentDateOut;
        XPGuidObject _xPGuidObject;
        Window _sourceWindow;
        Viewport _viewport;
        TouchButtonIconWithText _buttonChange;
        TouchButtonIconWithText _buttonArticleOut;
        byte[] AttachedFile;

        private TouchButtonIconWithText _buttonInsert;
        public TouchButtonIconWithText ButtonInsert
        {
            get { return _buttonInsert; }
            set { _buttonInsert = value; }
        }
        protected GenericTreeViewNavigator<fin_article, TreeViewArticle> _navigator;
        public GenericTreeViewNavigator<fin_article, TreeViewArticle> Navigator
        {
            get { return _navigator; }
            set { _navigator = value; }
        }

        private EntryBoxValidation _entryBoxSerialNumber1;
        public EntryBoxValidation EntryBoxSerialNumber1
        {
            get { return _entryBoxSerialNumber1; }
            set { _entryBoxSerialNumber1 = value; }
        }

        private List<fin_articleserialnumber> _selectedAssocietedArticles;
        public List<fin_articleserialnumber> SelectedAssocietedArticles
        {
            get { return _selectedAssocietedArticles; }
            set { _selectedAssocietedArticles = value; }
        }

        private List<fin_articleserialnumber> _backupAssocietedArticles;

        private string _serialNumber;

        ScrolledWindow _scrolledWindowView;


        private ICollection<XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>> _entryCompositeLinesCollection;

        public DialogArticleCompositionSerialNumber(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pDialogFlags, XPGuidObject pXPGuidObject, List<fin_articleserialnumber> pSelectedAssocietedArticles, string pSerialNumber = "")
            : base(pSourceWindow, pTreeView, pDialogFlags, DialogMode.Update, pXPGuidObject)
        {
            _sourceWindow = pSourceWindow;
            _xPGuidObject = pXPGuidObject;
            this.Title = "Editar Artigo Único";
            _serialNumber = pSerialNumber;
            _selectedAssocietedArticles = new List<fin_articleserialnumber> ();
            _backupAssocietedArticles = new List<fin_articleserialnumber>();
            if (pSelectedAssocietedArticles != null)
            {
                _selectedAssocietedArticles = pSelectedAssocietedArticles;
       
                foreach (var line in pSelectedAssocietedArticles)
                {
                    _backupAssocietedArticles.Add(line);
                }
            }

            if (GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600)
            {
                SetSizeRequest(320, 250);
            }
            else
            {
                SetSizeRequest(450, 520);
            }
            InitUI();
            ShowAll();
        }


        private void InitUI()
        {
            try
            {
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab1
                _viewport = new Viewport() { ShadowType = ShadowType.None };
                _viewport.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));

                vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                _scrolledWindowView = new ScrolledWindow();
                _scrolledWindowView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                _scrolledWindowView.ShadowType = ShadowType.Out;

                _selectedArticle = _xPGuidObject as fin_article;

                if(_selectedArticle == null) _selectedArticle = (_xPGuidObject as fin_articleserialnumber).Article;

                //Selected Article
                HBox hBoxTitle = new HBox();
                Label selectedArticleTitleLabel = new Label("Artigo : " + _selectedArticle.Designation);

                vboxTab1.PackStart(selectedArticleTitleLabel, false, false, 5);

                //SerialNumber
                _entryBoxSerialNumber1 = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_serial_number"), KeyboardMode.None, SettingsApp.RegexAlfaNumericExtended, true, true);

                if(_xPGuidObject.GetType() == typeof(fin_articleserialnumber) && (_xPGuidObject as fin_articleserialnumber).IsSold)
                {
                    _entryBoxSerialNumber1.Sensitive = false;
                }

                vboxTab1.PackStart(_entryBoxSerialNumber1, false, false, 0);
                _entryBoxSerialNumber1.EntryValidation.Text = _serialNumber;

                //Associated articles                
                Label associatedArticlesTitleLabel = new Label("Artigos Associados");
                
                if(_selectedArticle.ArticleComposition.Count > 0 && _selectedArticle.IsComposed)
                {
                    vboxTab1.PackStart(associatedArticlesTitleLabel, false, false, 5);
                }

                //if( SelectedAssocietedArticles.Count > 0)
                //{
                //    foreach (var associatedArticles in SelectedAssocietedArticles)
                //    {
                //        ModifySerialNumber(associatedArticles);                        
                //    }
                //}
                //else
                
                foreach (var associatedArticles in _selectedArticle.ArticleComposition)
                {
                    for(int i=0; i < associatedArticles.Quantity; i++)
                    {
                        AddSerialNumber(associatedArticles);
                    }                  
                }
                _viewport.Add(vboxTab1);

                _scrolledWindowView.Add(_viewport);

                //Append Tab
                _notebook.AppendPage(_scrolledWindowView, new Label("Editar"));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Selected Article
                if (_xPGuidObject.GetType() == typeof(fin_articleserialnumber))
                {

                    //SerialNumber Changed
                    _entryBoxArticleSerialNumber = new XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>(this, "Artigo Original", "SerialNumber", "Oid", (_dataSourceRow as fin_articleserialnumber), null, SettingsApp.RegexGuid, true, true);
                    _entryBoxArticleSerialNumber.EntryValidation.IsEditable = true;
                    _entryBoxArticleSerialNumber.EntryValidation.Completion.PopupCompletion = true;
                    _entryBoxArticleSerialNumber.EntryValidation.Completion.InlineCompletion = false;
                    _entryBoxArticleSerialNumber.EntryValidation.Completion.PopupSingleMatch = true;
                    _entryBoxArticleSerialNumber.EntryValidation.Completion.InlineSelection = true;
                    _entryBoxArticleSerialNumber.Sensitive = false;
                    vboxTab2.PackStart(_entryBoxArticleSerialNumber, false, false, 0);

                    //SerialNumber to Change
                    CriteriaOperator serialNumberCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
                    if ((_dataSourceRow as fin_articleserialnumber).Article != null)
                    {
                        serialNumberCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND SerialNumber != '{0}' AND IsSold == False", (_dataSourceRow as fin_articleserialnumber).SerialNumber));
                    }

                    _entryBoxArticleSerialNumberToChange = new XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>(this, "Artigo para troca", "SerialNumber", "Oid", null, serialNumberCriteria, SettingsApp.RegexGuid, true, true);
                    _entryBoxArticleSerialNumberToChange.EntryValidation.IsEditable = true;
                    _entryBoxArticleSerialNumberToChange.EntryValidation.Completion.PopupCompletion = true;
                    _entryBoxArticleSerialNumberToChange.EntryValidation.Completion.InlineCompletion = false;
                    _entryBoxArticleSerialNumberToChange.EntryValidation.Completion.PopupSingleMatch = true;
                    _entryBoxArticleSerialNumberToChange.EntryValidation.Completion.InlineSelection = true;
                    _entryBoxArticleSerialNumberToChange.EntryValidation.Changed += _entryBoxArticleSerialNumberToChange_Changed;

                    if (!(_dataSourceRow as fin_articleserialnumber).IsSold) _entryBoxArticleSerialNumberToChange.Sensitive = false;

                    vboxTab2.PackStart(_entryBoxArticleSerialNumberToChange, false, false, 0);


                    _buttonChange = (_sourceWindow as DialogArticleStock).GetNewButton("touchButtonPrev_DialogActionArea", "Trocar", @"Icons/icon_pos_nav_refresh.png");
                    _buttonChange.Sensitive = false;

                    _buttonChange.Clicked += ChangeButton_Clicked;

                    vboxTab2.PackStart(_buttonChange, false, false, 0);

                    //Append Tab
                    _notebook.AppendPage(vboxTab2, new Label("Trocar"));

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                    //Moviment Out Edit details

                    vboxTab3 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                    //Supplier
                    CriteriaOperator criteriaOperatorSupplier = CriteriaOperator.Parse("(Supplier = 1)");
                    _entryBoxSelectSupplier = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_supplier"), "Name", "Oid", (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.Customer, criteriaOperatorSupplier, SettingsApp.RegexGuid, true, true);
                    _entryBoxSelectSupplier.EntryValidation.IsEditable = true;
                    _entryBoxSelectSupplier.EntryValidation.Completion.PopupCompletion = true;
                    _entryBoxSelectSupplier.EntryValidation.Completion.InlineCompletion = false;
                    _entryBoxSelectSupplier.EntryValidation.Completion.PopupSingleMatch = true;
                    _entryBoxSelectSupplier.EntryValidation.Completion.InlineSelection = true;

                    vboxTab3.PackStart(_entryBoxSelectSupplier, false, false, 0);

                    //_entryBoxSelectSupplier.EntryValidation.Changed += delegate { ValidateDialog(); };

                    //DocumentDate
                    _entryBoxDocumentDateIn = new EntryBoxValidationDatePickerDialog(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date"), (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.Date, SettingsApp.RegexDate, true, SettingsApp.DateFormat, true);
                    //_entryBoxDocumentDate.EntryValidation.Sensitive = true;
                    _entryBoxDocumentDateIn.EntryValidation.Text = (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.Date.ToString(SettingsApp.DateFormat);
                    _entryBoxDocumentDateIn.EntryValidation.Validate();
                    _entryBoxDocumentDateIn.EntryValidation.Sensitive = true;
                    //_entryBoxDocumentDate.ClosePopup += delegate { ValidateDialog(); };

                    vboxTab3.PackStart(_entryBoxDocumentDateIn, false, false, 0);


                    //DocumentNumber
                    Color colorBaseDialogEntryBoxBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogEntryBoxBackground"]);
                    string _fileIconListFinanceDocuments = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_toolbar_finance_document.png");
                    HBox hBoxDocument = new HBox(false, 0);
                    _entryBoxDocumentNumber = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_document_number"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false, true);
                    if ((_dataSourceRow as fin_articleserialnumber).StockMovimentIn.DocumentNumber != string.Empty) _entryBoxDocumentNumber.EntryValidation.Text = (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.DocumentNumber;
                    //_entryBoxDocumentNumber.EntryValidation.Changed += delegate { ValidateDialog(); };
                    TouchButtonIcon attachPDFButton = new TouchButtonIcon("attachPDFButton", colorBaseDialogEntryBoxBackground, _fileIconListFinanceDocuments, new Size(20, 20), 30, 30);
                    attachPDFButton.Clicked += AttachPDFButton_Clicked;
                    ((_entryBoxDocumentNumber.Children[0] as VBox).Children[1] as HBox).PackEnd(attachPDFButton, false, false, 0);

                    vboxTab3.PackStart(_entryBoxDocumentNumber, false, false, 0);

                    //Price
                    _entryBoxPrice1 = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_price"), KeyboardMode.None, SettingsApp.RegexDecimalGreaterEqualThanZeroFinancial, false, true);
                    _entryBoxPrice1.WidthRequest = 40;
                    _entryBoxPrice1.EntryValidation.Text = (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.PurchasePrice.ToString();
                    //_entryBoxPrice1.EntryValidation.Changed += EntryPurchasedPriceValidation_Changed;

                    vboxTab3.PackStart(_entryBoxPrice1, false, false, 0);

                    _notebook.AppendPage(vboxTab3, new Label("Movimento de Entrada"));

                    //___________________________________________________________________________
                    //Moviment In Edit details

                    vboxTab4 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
                    fin_documentfinancemaster documentfinancemaster = ((_dataSourceRow as fin_articleserialnumber).StockMovimentOut != null) ? (_dataSourceRow as fin_articleserialnumber).StockMovimentOut.DocumentMaster : null;

                    //Document Number
                    CriteriaOperator criteriaOperatorSourceDocumentFinance = CriteriaOperator.Parse("([Disabled] Is Null Or [Disabled] <> 1) And [DocumentStatusStatus] <> 'A' And [DocumentStatusStatus] <> 'F' ");
                    _entryBoxSelectDocumentOut = new XPOEntryBoxSelectRecordValidation<fin_documentfinancemaster, TreeViewDocumentFinanceMaster>(_sourceWindow, "Documento Venda", "DocumentNumber", "Oid", documentfinancemaster, criteriaOperatorSourceDocumentFinance, SettingsApp.RegexGuid, false, true);
                    _entryBoxSelectDocumentOut.EntryValidation.IsEditable = true;
                    _entryBoxSelectDocumentOut.EntryValidation.Completion.PopupCompletion = true;
                    _entryBoxSelectDocumentOut.EntryValidation.Completion.InlineCompletion = false;
                    _entryBoxSelectDocumentOut.EntryValidation.Completion.PopupSingleMatch = true;
                    _entryBoxSelectDocumentOut.EntryValidation.Completion.InlineSelection = true;

                    _entryBoxSelectDocumentOut.EntryValidation.Sensitive = !((_dataSourceRow as fin_articleserialnumber).IsSold);


                    vboxTab4.PackStart(_entryBoxSelectDocumentOut, false, false, 0);

                    //_entryBoxSelectSupplier.EntryValidation.Changed += delegate { ValidateDialog(); };

                    //DocumentDate
                    DateTime dateTime = ((_dataSourceRow as fin_articleserialnumber).StockMovimentOut != null) ? (_dataSourceRow as fin_articleserialnumber).StockMovimentOut.Date : DateTime.Now;
                    _entryBoxDocumentDateOut = new EntryBoxValidationDatePickerDialog(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date"), (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.Date, SettingsApp.RegexDate, true, SettingsApp.DateFormat, true);
                    //_entryBoxDocumentDate.EntryValidation.Sensitive = true;
                    _entryBoxDocumentDateOut.EntryValidation.Text = dateTime.ToString(SettingsApp.DateFormat);
                    _entryBoxDocumentDateOut.EntryValidation.Validate();
                    _entryBoxDocumentDateOut.EntryValidation.Sensitive = !((_dataSourceRow as fin_articleserialnumber).IsSold);
                    //_entryBoxDocumentDate.ClosePopup += delegate { ValidateDialog(); };
                 
                    vboxTab4.PackStart(_entryBoxDocumentDateOut, false, false, 0);

                    //Button Out
                    _buttonArticleOut = (_sourceWindow as DialogArticleStock).GetNewButton("touchButtonPrev_DialogActionArea", "Saída do artigo", @"Icons/icon_pos_toolbar_logout_user.png");
                    _buttonArticleOut.Sensitive = !((_dataSourceRow as fin_articleserialnumber).IsSold);
                    _buttonArticleOut.Clicked += ArticleOutButton_Clicked;

                    vboxTab4.PackStart(_buttonArticleOut, false, false, 0);

                    _notebook.AppendPage(vboxTab4, new Label("Movimento de Saída"));

                    base.buttonOk.Clicked += ButtonOk_Clicked;
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                LoadSavedValues();

                _entryBoxSerialNumber1.EntryValidation.Changed += _entryBoxArticleSerialNumberCompositionArticles_Changed;
            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private void ButtonOk_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Edit stock moviment IN
                if(_entryBoxSelectSupplier.EntryValidation.Validated && _entryBoxDocumentNumber.EntryValidation.Validated && _entryBoxPrice1.EntryValidation.Validated && _entryBoxDocumentDateIn.EntryValidation.Validated)
                {
                    (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.Customer = _entryBoxSelectSupplier.Value;
                    (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.DocumentNumber = _entryBoxDocumentNumber.EntryValidation.Text;
                    (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.PurchasePrice = FrameworkUtils.StringToDecimal(_entryBoxPrice1.EntryValidation.Text);
                    (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.Date = _entryBoxDocumentDateIn.Value;
                    if (AttachedFile != null) (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.AttachedFile = AttachedFile;
                    (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.Save();
                    _log.Debug("Sock Moviment In Changed with sucess");

                    ResponseType responseType = Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_operation_successfully"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentticket_type_title_cs_short"), GlobalFramework.ServerVersion));
                }

            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private void _entryBoxArticleSerialNumberToChange_Changed(object sender, EventArgs e)
        {
            if ((sender as EntryValidation).Validated) _buttonChange.Sensitive = true;
            else _buttonChange.Sensitive = false;
        }

        private void ChangeButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var own_customer = (erp_customer)GlobalFramework.SessionXpo.GetObjectByKey(typeof(erp_customer), SettingsApp.XpoOidUserRecord);
                var stockMovimentOut = (_dataSourceRow as fin_articleserialnumber).StockMovimentOut;
                bool sucess = false;;

                //Devolver artigo original
                sucess = GlobalFramework.StockManagementModule.Add(_dataSourceRow.Session, 
                    datalayer.Enums.ProcessArticleStockMode.In, 
                    own_customer, 
                    0, 
                    DateTime.Now, 
                    "", 
                    (_dataSourceRow as fin_articleserialnumber).Article, 
                    1, 
                    string.Format("Troca de artigos: " + (_dataSourceRow as fin_articleserialnumber).SerialNumber + " / " + (_entryBoxArticleSerialNumberToChange.Value as fin_articleserialnumber).SerialNumber), 
                    (_dataSourceRow as fin_articleserialnumber).SerialNumber, 
                    (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.PurchasePrice, 
                    (_dataSourceRow as fin_articleserialnumber).ArticleWarehouse.Location, 
                    null, _selectedAssocietedArticles, true, true);

                //Criar movimento de saida do artigo novo
                sucess = GlobalFramework.StockManagementModule.Add(_dataSourceRow.Session, 
                    datalayer.Enums.ProcessArticleStockMode.Out,
                    stockMovimentOut.DocumentDetail,
                    own_customer, 
                    0, 
                    DateTime.Now,
                    "", 
                    (_dataSourceRow as fin_articleserialnumber).Article, 
                    1,
                     string.Format("Troca de artigos: " + (_dataSourceRow as fin_articleserialnumber).SerialNumber + " / " + (_entryBoxArticleSerialNumberToChange.Value as fin_articleserialnumber).SerialNumber),
                    (_entryBoxArticleSerialNumberToChange.Value as fin_articleserialnumber).SerialNumber, 
                    (_entryBoxArticleSerialNumberToChange.Value as fin_articleserialnumber).StockMovimentIn.PurchasePrice,
                    (_entryBoxArticleSerialNumberToChange.Value as fin_articleserialnumber).ArticleWarehouse.Location, 
                    null, _selectedAssocietedArticles, false, true);

                if (sucess)
                {
                    ResponseType responseType = Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_operation_successfully"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentticket_type_title_cs_short"), GlobalFramework.ServerVersion));
                    _entryBoxArticleSerialNumberToChange.Sensitive = false;
                    _buttonChange.Sensitive = false;
                }

            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private void ArticleOutButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var own_customer = (erp_customer)GlobalFramework.SessionXpo.GetObjectByKey(typeof(erp_customer), SettingsApp.XpoOidUserRecord);
                var stockMovimentOut = (_dataSourceRow as fin_articleserialnumber).StockMovimentOut;
                bool sucess = false;

                fin_documentfinancedetail documentDetail = new fin_documentfinancedetail(_dataSourceRow.Session);
                documentDetail.DocumentMaster = _entryBoxSelectDocumentOut.Value;

                //Criar movimento de saida do artigo 
                sucess = GlobalFramework.StockManagementModule.Add(_dataSourceRow.Session,
                    datalayer.Enums.ProcessArticleStockMode.Out,
                    documentDetail,
                    own_customer,
                    0,
                    _entryBoxDocumentDateOut.Value,
                    "",
                    (_dataSourceRow as fin_articleserialnumber).Article,
                    1,
                     string.Format("Saída de artigo unico: " + (_dataSourceRow as fin_articleserialnumber).SerialNumber),
                    (_dataSourceRow as fin_articleserialnumber).SerialNumber,
                    (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.PurchasePrice,
                    (_dataSourceRow as fin_articleserialnumber).ArticleWarehouse.Location,
                    null, _selectedAssocietedArticles, false, false);

                if (sucess)
                {
                    ResponseType responseType = Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_operation_successfully"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentticket_type_title_cs_short"), GlobalFramework.ServerVersion));
                    _entryBoxArticleSerialNumberToChange.Sensitive = false;
                    _entryBoxSelectDocumentOut.EntryValidation.Sensitive = false;
                    _entryBoxDocumentDateOut.EntryValidation.Sensitive = false;
                    _buttonArticleOut.Sensitive = false;
                }

            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private void AddSerialNumber(fin_articlecomposition pArticlecomposition)
        {
            //Select SerialNumber
            CriteriaOperator serialNumberCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND IsSold == False AND Article == '{0}'", pArticlecomposition.ArticleChild.Oid));

            _entryBoxArticleSerialNumberCompositionArticles = new XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_serialnumber") + " :: " + pArticlecomposition.ArticleChild.Designation, "SerialNumber", "Oid", null, serialNumberCriteria, SettingsApp.RegexGuid, true, true);
            _entryBoxArticleSerialNumberCompositionArticles.EntryValidation.IsEditable = true;
            _entryBoxArticleSerialNumberCompositionArticles.EntryValidation.Completion.PopupCompletion = true;
            _entryBoxArticleSerialNumberCompositionArticles.EntryValidation.Completion.InlineCompletion = false;
            _entryBoxArticleSerialNumberCompositionArticles.EntryValidation.Completion.PopupSingleMatch = true;
            _entryBoxArticleSerialNumberCompositionArticles.EntryValidation.Completion.InlineSelection = true;
            _entryBoxArticleSerialNumberCompositionArticles.Article = pArticlecomposition.ArticleChild;
            if (_entryBoxArticleSerialNumberCompositionArticles.Value != null) _entryBoxArticleSerialNumberCompositionArticles.EntryValidation.Changed += _entryBoxArticleSerialNumberCompositionArticles_Changed;

            if (_xPGuidObject.GetType() == typeof(fin_articleserialnumber) && (_xPGuidObject as fin_articleserialnumber).IsSold)
            {
                _entryBoxArticleSerialNumberCompositionArticles.Sensitive = false;
            }
            vboxTab1.PackStart(_entryBoxArticleSerialNumberCompositionArticles, false, false, 0);

        }

        private void ModifySerialNumber(fin_articleserialnumber pArticleserialnumber)
        {

            //Select SerialNumber
            CriteriaOperator serialNumberCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND IsSold == False AND Article == '{0}'", pArticleserialnumber.Article.Oid));

            _entryBoxArticleSerialNumberToChange = new XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_serialnumber") + " :: " + pArticleserialnumber.Article.Designation, "SerialNumber", "Oid", pArticleserialnumber, serialNumberCriteria, SettingsApp.RegexGuid, true, true);
            _entryBoxArticleSerialNumberToChange.EntryValidation.IsEditable = true;
            _entryBoxArticleSerialNumberToChange.EntryValidation.Completion.PopupCompletion = true;
            _entryBoxArticleSerialNumberToChange.EntryValidation.Completion.InlineCompletion = false;
            _entryBoxArticleSerialNumberToChange.EntryValidation.Completion.PopupSingleMatch = true;
            _entryBoxArticleSerialNumberToChange.EntryValidation.Completion.InlineSelection = true;
            _entryBoxArticleSerialNumberToChange.EntryValidation.Changed += _entryBoxArticleSerialNumberCompositionArticles_Changed;


            //entrySerialNumber = new Entry();
            ////entrySerialNumber.Text = (_dataSourceRow as fin_articlewarehouse).SerialNumber;
            //SortProperty[] sortProperty = new SortProperty[1];
            //sortProperty[0] = new SortProperty("SerialNumber", DevExpress.Xpo.DB.SortingDirection.Ascending);
            //xpoComboBoxArticleSerialNumber = new XPOComboBox(DataSourceRow.Session, typeof(fin_articleserialnumber), null, "SerialNumber", serialNumberCriteria, sortProperty);
            //BOWidgetBox boxArticleSerialNumber = new BOWidgetBox(string.Format(pArticleserialnumber.Article.Designation + " : " + resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_serial_number")), xpoComboBoxArticleSerialNumber);
            //xpoComboBoxArticleSerialNumber.Sensitive = true;
            //xpoComboBoxArticleSerialNumber.Value = pArticleserialnumber;
            //TreeIter iter;
            //xpoComboBoxArticleSerialNumber.Model.GetIterFirst(out iter);
            //do
            //{
            //    GLib.Value thisRow = new GLib.Value();
            //    xpoComboBoxArticleSerialNumber.Model.GetValue(iter, 0, ref thisRow);
            //    if ((thisRow.Val as string).Equals(pArticleserialnumber.SerialNumber))
            //    {
            //        xpoComboBoxArticleSerialNumber.SetActiveIter(iter);
            //        break;
            //    }
            //} while (xpoComboBoxArticleSerialNumber.Model.IterNext(ref iter));

            //xpoComboBoxArticleSerialNumber.Changed += XpoComboBoxArticleSerialNumber_Changed;

            vboxTab1.PackStart(_entryBoxArticleSerialNumberToChange, false, false, 0);
        }

        private void _entryBoxArticleSerialNumberCompositionArticles_Changed(object sender, EventArgs e)
        {
            var selectedArticle = sender as EntryValidation;
            
            _selectedAssocietedArticles.Clear();
            foreach (var entrySerialNumber in (selectedArticle.Parent.Parent.Parent.Parent as VBox).Children)
            {
                if (entrySerialNumber.GetType() == typeof(XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>))
                {
                    var selectedSerialNumber = (((entrySerialNumber as XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>).Value as fin_articleserialnumber));
                    if (selectedSerialNumber != null)
                    {
                        _selectedAssocietedArticles.Add(selectedSerialNumber);
                    }
                }
            }
        }

        private void LoadSavedValues()
        {
            foreach (var entrySerialNumber in vboxTab1.Children)
            {
                if (entrySerialNumber.GetType() == typeof(XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>))
                {
                    var selectedSerialNumberEntry = entrySerialNumber as XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>;
                    for(int i= 0; i < _backupAssocietedArticles.Count; i++)
                    {
                        if(selectedSerialNumberEntry.Article == _backupAssocietedArticles[i].Article)
                        {
                            selectedSerialNumberEntry.Value = _backupAssocietedArticles[i];
                            selectedSerialNumberEntry.EntryValidation.Text = _backupAssocietedArticles[i].SerialNumber;
                            _backupAssocietedArticles.RemoveAt(i);
                        }
                    }
                    selectedSerialNumberEntry.EntryValidation.Changed += _entryBoxArticleSerialNumberCompositionArticles_Changed;
                }
            }
        }

        private void XpoComboBoxArticleSerialNumber_Changed(object sender, EventArgs e)
        {
            var selectedArticle = sender as XPOComboBox;
            _selectedAssocietedArticles.Clear();
            foreach (var comboBox in (selectedArticle.Parent.Parent as VBox).Children)
            {
                if(comboBox.GetType() == typeof(BOWidgetBox))
                {
                    var selectedSerialNumber = (((comboBox as BOWidgetBox).Children[1] as XPOComboBox).Value as fin_articleserialnumber);
                    if (selectedSerialNumber != null)
                    {
                        _selectedAssocietedArticles.Add(selectedSerialNumber);
                    }
                }
            }
        }

        private void AttachPDFButton_Clicked(object sender, EventArgs e)
        {
            FileFilter fileFilterPDF = Utils.GetFileFilterPDF();
            Pos.Dialogs.PosFilePickerDialog dialog = new Pos.Dialogs.PosFilePickerDialog(this, DialogFlags.DestroyWithParent, fileFilterPDF, FileChooserAction.Open);
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

        //Get Criteria of included DocumentTypes for EntryBoxSelectSourceDocumentFinance related to EntryBoxSelectDocumentFinanceType
        private CriteriaOperator GetDocumentFinanceTypeSourceDocumentCriteria()
        {
            bool debug = false;

            //Hide Cancelled and Invoiced Documents from Source
            string filterBase = "(Disabled IS NULL OR Disabled  <> 1) AND (DocumentStatusStatus <> 'A' AND DocumentStatusStatus <> 'F') {0}";
            string filterDocs = string.Empty;
            string filter = string.Empty;
            Guid[] listDocumentTypes = FrameworkUtils.GetDocumentTypeValidSourceDocuments(SettingsApp.XpoOidDocumentFinanceTypeInvoice);

            //Generate Filter Docs from listDocumentTypes Array
            for (int i = 0; i < listDocumentTypes.Length; i++)
            {
                string filterDocumentType = string.Empty;

                //COMMENTED, now all documents use "AND DocumentChild IS NULL" - Leave this Block Here, May be usefull to Use in Future
                //
                //              //Specific Extra Filter for ConsignationInvoice, When Document Type is Invoice, Must add DocumentChild IS NULL to DocumentType filter
                //              if (listDocumentTypes[i] == _xpoOidDocumentFinanceTypeConsignationInvoice)
                //              {
                //                  //If DocumentFinanceTypeInvoice or WayBill, Show ConsignationInvoices, if not Invoiced Yet
                //                  if (
                //                      _entryBoxSelectDocumentFinanceType.Value.Oid.ToString() == _xpoOidDocumentFinanceTypeInvoice
                //                      //|| (int) _entryBoxSelectDocumentFinanceType.Value.SaftDocumentType == 2
                //                  )
                //                  {
                //                      filterDocumentType = string.Format("(DocumentType = '{0}' AND DocumentChild IS NULL)", listDocumentTypes[i]);
                //                      filterDocs += filterDocumentType;
                //                  }
                //              }
                //              //Default for all listDocumentTypes
                //              else
                //              {
                filterDocumentType += string.Format("DocumentType = '{0}'", listDocumentTypes[i]);
                filterDocs += filterDocumentType;
                //              }

                if (filterDocumentType != string.Empty && i < listDocumentTypes.Length - 1) filterDocs += " OR ";
            }

            //Add filterDocs if filterDocs is not Empty
            if (filterDocs != string.Empty) filterDocs = string.Format("AND ({0})", filterDocs);

            filter = string.Format(filterBase, filterDocs);
            if (debug) _log.Debug(string.Format("GetDocumentFinanceTypeSourceDocumentCriteria.Filter: [{0}]", filter));

            //Generate Final Result Criteria
            CriteriaOperator result = CriteriaOperator.Parse(filter);

            return result;
        }
    }

}
