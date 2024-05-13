using DevExpress.Data.Filtering;
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
using logicpos.datalayer.Xpo;
using logicpos.Extensions;
using logicpos.shared.App;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using LogicPOS.Settings;

namespace logicpos.Classes.Gui.Gtk.BackOffice.Dialogs.Articles
{
    internal class DialogArticleCompositionSerialNumber : BOBaseDialog
    {
        //UI
        private VBox vboxTab1;
        private VBox vboxTab2;
        private VBox vboxTab3;
        private VBox vboxTab4;
        private fin_article _selectedArticle;
        private XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber> _entryBoxArticleSerialNumber;
        private XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber> _entryBoxArticleSerialNumberToChange;
        private XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber> _entryBoxArticleSerialNumberCompositionArticles;
        private XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>  _entryBoxSelectSupplier;
        private XPOEntryBoxSelectRecordValidation<fin_documentfinancemaster, TreeViewDocumentFinanceMaster> _entryBoxSelectDocumentOut;
        private EntryBoxValidation _entryBoxDocumentNumber;
        private EntryBoxValidation _entryBoxPrice1;
        private EntryBoxValidationDatePickerDialog _entryBoxDocumentDateIn;
        private EntryBoxValidationDatePickerDialog _entryBoxDocumentDateOut;
        private readonly XPGuidObject _xPGuidObject;
        private readonly Window _sourceWindow;
        private Viewport _viewport;
        private TouchButtonIconWithText _buttonChange;
        private TouchButtonIconWithText _buttonArticleOut;
        private byte[] AttachedFile;

        public TouchButtonIconWithText ButtonInsert { get; set; }
        protected GenericTreeViewNavigator<fin_article, TreeViewArticle> _navigator;
        public GenericTreeViewNavigator<fin_article, TreeViewArticle> Navigator
        {
            get { return _navigator; }
            set { _navigator = value; }
        }

        public EntryBoxValidation EntryBoxSerialNumber1 { get; set; }

        public List<fin_articleserialnumber> SelectedAssocietedArticles { get; set; }

        private readonly List<fin_articleserialnumber> _backupAssocietedArticles;

        private readonly string _serialNumber;
        private ScrolledWindow _scrolledWindowView;

        public DialogArticleCompositionSerialNumber(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pDialogFlags, XPGuidObject pXPGuidObject, List<fin_articleserialnumber> pSelectedAssocietedArticles, string pSerialNumber = "")
            : base(pSourceWindow, pTreeView, pDialogFlags, DialogMode.Update, pXPGuidObject)
        {
            _sourceWindow = pSourceWindow;
            _xPGuidObject = pXPGuidObject;
            this.Title = "Editar Artigo Único";
            _serialNumber = pSerialNumber;
            SelectedAssocietedArticles = new List<fin_articleserialnumber> ();
            _backupAssocietedArticles = new List<fin_articleserialnumber>();
            if (pSelectedAssocietedArticles != null)
            {
                SelectedAssocietedArticles = pSelectedAssocietedArticles;
       
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
                _viewport.ModifyBg(StateType.Normal, Color.White.ToGdkColor());

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
                EntryBoxSerialNumber1 = new EntryBoxValidation(this, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_serial_number"), KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true, true);

                if(_xPGuidObject.GetType() == typeof(fin_articleserialnumber) && (_xPGuidObject as fin_articleserialnumber).IsSold)
                {
                    EntryBoxSerialNumber1.Sensitive = false;
                }

                vboxTab1.PackStart(EntryBoxSerialNumber1, false, false, 0);
                EntryBoxSerialNumber1.EntryValidation.Text = _serialNumber;

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
                    _entryBoxArticleSerialNumber = new XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>(this, "Artigo Original", "SerialNumber", "Oid", (_dataSourceRow as fin_articleserialnumber), null, LogicPOS.Utility.RegexUtils.RegexGuid, true, true);
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

                    _entryBoxArticleSerialNumberToChange = new XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>(this, "Artigo para troca", "SerialNumber", "Oid", null, serialNumberCriteria, LogicPOS.Utility.RegexUtils.RegexGuid, true, true);
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
                    _entryBoxSelectSupplier = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(this, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_supplier"), "Name", "Oid", (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.Customer, criteriaOperatorSupplier, LogicPOS.Utility.RegexUtils.RegexGuid, true, true);
                    _entryBoxSelectSupplier.EntryValidation.IsEditable = true;
                    _entryBoxSelectSupplier.EntryValidation.Completion.PopupCompletion = true;
                    _entryBoxSelectSupplier.EntryValidation.Completion.InlineCompletion = false;
                    _entryBoxSelectSupplier.EntryValidation.Completion.PopupSingleMatch = true;
                    _entryBoxSelectSupplier.EntryValidation.Completion.InlineSelection = true;

                    vboxTab3.PackStart(_entryBoxSelectSupplier, false, false, 0);

                    //_entryBoxSelectSupplier.EntryValidation.Changed += delegate { ValidateDialog(); };

                    //DocumentDate
                    _entryBoxDocumentDateIn = new EntryBoxValidationDatePickerDialog(this, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_date"), CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_date"), (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.Date, LogicPOS.Utility.RegexUtils.RegexDate, true, CultureSettings.DateFormat, true);
                    //_entryBoxDocumentDate.EntryValidation.Sensitive = true;
                    _entryBoxDocumentDateIn.EntryValidation.Text = (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.Date.ToString(CultureSettings.DateFormat);
                    _entryBoxDocumentDateIn.EntryValidation.Validate();
                    _entryBoxDocumentDateIn.EntryValidation.Sensitive = true;
                    //_entryBoxDocumentDate.ClosePopup += delegate { ValidateDialog(); };

                    vboxTab3.PackStart(_entryBoxDocumentDateIn, false, false, 0);


                    //DocumentNumber
                    Color colorBaseDialogEntryBoxBackground = GeneralSettings.Settings["colorBaseDialogEntryBoxBackground"].StringToColor();
                    string _fileIconListFinanceDocuments = GeneralSettings.Paths["images"] + @"Icons\icon_pos_toolbar_finance_document.png";
                    HBox hBoxDocument = new HBox(false, 0);
                    _entryBoxDocumentNumber = new EntryBoxValidation(this, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_document_number"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false, true);
                    if ((_dataSourceRow as fin_articleserialnumber).StockMovimentIn.DocumentNumber != string.Empty) _entryBoxDocumentNumber.EntryValidation.Text = (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.DocumentNumber;
                    //_entryBoxDocumentNumber.EntryValidation.Changed += delegate { ValidateDialog(); };
                    TouchButtonIcon attachPDFButton = new TouchButtonIcon("attachPDFButton", colorBaseDialogEntryBoxBackground, _fileIconListFinanceDocuments, new Size(20, 20), 30, 30);
                    attachPDFButton.Clicked += AttachPDFButton_Clicked;
                    ((_entryBoxDocumentNumber.Children[0] as VBox).Children[1] as HBox).PackEnd(attachPDFButton, false, false, 0);

                    vboxTab3.PackStart(_entryBoxDocumentNumber, false, false, 0);

                    //Price
                    _entryBoxPrice1 = new EntryBoxValidation(this, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_price"), KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexDecimalGreaterEqualThanZeroFinancial, false, true);
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
                    _entryBoxSelectDocumentOut = new XPOEntryBoxSelectRecordValidation<fin_documentfinancemaster, TreeViewDocumentFinanceMaster>(_sourceWindow, "Documento Venda", "DocumentNumber", "Oid", documentfinancemaster, criteriaOperatorSourceDocumentFinance, LogicPOS.Utility.RegexUtils.RegexGuid, false, true);
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
                    _entryBoxDocumentDateOut = new EntryBoxValidationDatePickerDialog(this, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_date"), CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_date"), (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.Date, LogicPOS.Utility.RegexUtils.RegexDate, true, CultureSettings.DateFormat, true);
                    //_entryBoxDocumentDate.EntryValidation.Sensitive = true;
                    _entryBoxDocumentDateOut.EntryValidation.Text = dateTime.ToString(CultureSettings.DateFormat);
                    _entryBoxDocumentDateOut.EntryValidation.Validate();
                    _entryBoxDocumentDateOut.EntryValidation.Sensitive = !((_dataSourceRow as fin_articleserialnumber).IsSold);
                    //_entryBoxDocumentDate.ClosePopup += delegate { ValidateDialog(); };
                 
                    vboxTab4.PackStart(_entryBoxDocumentDateOut, false, false, 0);

                    //Button Out
                    _buttonArticleOut = (_sourceWindow as DialogArticleStock).GetNewButton("touchButtonPrev_DialogActionArea", "Saída do artigo", @"Icons/icon_pos_toolbar_loggerout_user.png");
                    _buttonArticleOut.Sensitive = !((_dataSourceRow as fin_articleserialnumber).IsSold);
                    _buttonArticleOut.Clicked += ArticleOutButton_Clicked;

                    vboxTab4.PackStart(_buttonArticleOut, false, false, 0);

                    _notebook.AppendPage(vboxTab4, new Label("Movimento de Saída"));

                    buttonOk.Clicked += ButtonOk_Clicked;
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                LoadSavedValues();

                EntryBoxSerialNumber1.EntryValidation.Changed += _entryBoxArticleSerialNumberCompositionArticles_Changed;
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
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
                    (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.PurchasePrice = LogicPOS.Utility.DataConversionUtils.StringToDecimal(_entryBoxPrice1.EntryValidation.Text);
                    (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.Date = _entryBoxDocumentDateIn.Value;
                    if (AttachedFile != null) (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.AttachedFile = AttachedFile;
                    (_dataSourceRow as fin_articleserialnumber).StockMovimentIn.Save();
                    _logger.Debug("Sock Moviment In Changed with sucess");

                    ResponseType responseType = logicpos.Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "dialog_message_operation_successfully"), string.Format(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_documentticket_type_title_cs_short"), GeneralSettings.ServerVersion));
                }

            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
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
                var own_customer = (erp_customer)XPOSettings.Session.GetObjectByKey(typeof(erp_customer), XPOSettings.XpoOidUserRecord);
                var stockMovimentOut = (_dataSourceRow as fin_articleserialnumber).StockMovimentOut;
                bool sucess = false;;

                //Devolver artigo original
                sucess = POSFramework.StockManagementModule.Add(_dataSourceRow.Session, 
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
                    null, SelectedAssocietedArticles, true, true);

                //Criar movimento de saida do artigo novo
                sucess = POSFramework.StockManagementModule.Add(_dataSourceRow.Session, 
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
                    null, SelectedAssocietedArticles, false, true);

                if (sucess)
                {
                    ResponseType responseType = logicpos.Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "dialog_message_operation_successfully"), string.Format(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_documentticket_type_title_cs_short"), GeneralSettings.ServerVersion));
                    _entryBoxArticleSerialNumberToChange.Sensitive = false;
                    _buttonChange.Sensitive = false;
                }

            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void ArticleOutButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var own_customer = (erp_customer)XPOSettings.Session.GetObjectByKey(typeof(erp_customer), XPOSettings.XpoOidUserRecord);
                var stockMovimentOut = (_dataSourceRow as fin_articleserialnumber).StockMovimentOut;
                bool sucess = false;

                fin_documentfinancedetail documentDetail = new fin_documentfinancedetail(_dataSourceRow.Session);
                documentDetail.DocumentMaster = _entryBoxSelectDocumentOut.Value;

                //Criar movimento de saida do artigo 
                sucess = POSFramework.StockManagementModule.Add(_dataSourceRow.Session,
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
                    null, SelectedAssocietedArticles, false, false);

                if (sucess)
                {
                    ResponseType responseType = logicpos.Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "dialog_message_operation_successfully"), string.Format(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_documentticket_type_title_cs_short"), GeneralSettings.ServerVersion));
                    _entryBoxArticleSerialNumberToChange.Sensitive = false;
                    _entryBoxSelectDocumentOut.EntryValidation.Sensitive = false;
                    _entryBoxDocumentDateOut.EntryValidation.Sensitive = false;
                    _buttonArticleOut.Sensitive = false;
                }

            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void AddSerialNumber(fin_articlecomposition pArticlecomposition)
        {
            //Select SerialNumber
            CriteriaOperator serialNumberCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND IsSold == False AND Article == '{0}'", pArticlecomposition.ArticleChild.Oid));

            _entryBoxArticleSerialNumberCompositionArticles = new XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>(this, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_serialnumber") + " :: " + pArticlecomposition.ArticleChild.Designation, "SerialNumber", "Oid", null, serialNumberCriteria, LogicPOS.Utility.RegexUtils.RegexGuid, true, true);
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

        private void _entryBoxArticleSerialNumberCompositionArticles_Changed(object sender, EventArgs e)
        {
            var selectedArticle = sender as EntryValidation;
            
            SelectedAssocietedArticles.Clear();
            foreach (var entrySerialNumber in (selectedArticle.Parent.Parent.Parent.Parent as VBox).Children)
            {
                if (entrySerialNumber.GetType() == typeof(XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>))
                {
                    var selectedSerialNumber = (((entrySerialNumber as XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>).Value as fin_articleserialnumber));
                    if (selectedSerialNumber != null)
                    {
                        SelectedAssocietedArticles.Add(selectedSerialNumber);
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
            SelectedAssocietedArticles.Clear();
            foreach (var comboBox in (selectedArticle.Parent.Parent as VBox).Children)
            {
                if(comboBox.GetType() == typeof(BOWidgetBox))
                {
                    var selectedSerialNumber = (((comboBox as BOWidgetBox).Children[1] as XPOComboBox).Value as fin_articleserialnumber);
                    if (selectedSerialNumber != null)
                    {
                        SelectedAssocietedArticles.Add(selectedSerialNumber);
                    }
                }
            }
        }

        private void AttachPDFButton_Clicked(object sender, EventArgs e)
        {
            FileFilter fileFilterPDF = logicpos.Utils.GetFileFilterPDF();
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
    }

}
