using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.datalayer.DataLayer.Xpo.Documents;
using logicpos.datalayer.Xpo;
using logicpos.Extensions;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Image = Gtk.Image;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogArticle : BOBaseDialog
    {
        //UI
        //Artigos Compostos [IN:016522]
        private VBox _vboxTab2;

        public TouchButtonIconWithText ButtonInsert { get; set; }
        protected GenericTreeViewNavigator<fin_article, TreeViewArticle> _navigator;
        public GenericTreeViewNavigator<fin_article, TreeViewArticle> Navigator
        {
            get { return _navigator; }
            set { _navigator = value; }
        }

        //private XPOComboBox _xpoComboBoxCompositeArticle;
        private XPOComboBox _xpoComboBoxFamily;
        private XPOComboBox _xpoComboBoxSubFamily;
        private XPOComboBox _xpoComboBoxVatOnTable;
        private XPOComboBox _xpoComboBoxVatDirectSelling;
        private XPOComboBox _xpoComboBoxVatExemptionReason;
        //Non UI
        private fin_article _article = null;
        private fin_article _previousValue = null;

        //Other
        private VBox vboxTab4, vboxTab5;
        private ScrolledWindow _scrolledWindowCompositionView;
        private ScrolledWindow _scrolledWindowSerialNumbersView;
        private Viewport _CompositionView;
        private readonly Viewport _SerialNumbersView;
        private readonly TouchButtonIcon _buttonAddSerialNumber;
        private readonly TouchButtonIcon _buttonClearSerialNumber;
        private uint _totalNumberOfFinanceDocuments = 0;
        private XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle> _entryBoxSelectArticle1;
        private GenericCRUDWidgetXPO _genericCRUDWidgetXPO;
        public ICollection _dropdownTextCollection;
        private CheckButton _checkButtonComposite;
        private CheckButton _checkButtonUniqueArticles;
        private ICollection<Tuple<fin_articleserialnumber, Entry, GenericCRUDWidgetXPO, GenericCRUDWidgetXPO, GenericCRUDWidgetXPO, HBox>> _serialNumberCollection;
        private readonly string iconAddRecord = PathsUtils.GetImageLocationRelativeToImagesFolder(@"Icons/icon_pos_nav_new.png");
        private readonly string iconClearRecord = PathsUtils.GetImageLocationRelativeToImagesFolder(@"Icons/Windows/icon_window_delete_record.png");

        private int _totalCompositeEntrys = 0;

        private readonly ICollection<XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>> _entryCompositeLinesCollection;

        public DialogArticle(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pDialogFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pDialogFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_edit_article"));

            _entryCompositeLinesCollection = new List<XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>>();
            _articlecompositions = new List<fin_articlecomposition>();

            if (GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600)
            {
                SetSizeRequest(500, 700);
            }
            else
            {
                SetSizeRequest(500, 740);
            }

            InitUI();
            InitNotes();
            ShowAll();
            if (_scrolledWindowCompositionView != null) _scrolledWindowCompositionView.Visible = _checkButtonComposite.Active;
            if (_scrolledWindowSerialNumbersView != null) _scrolledWindowSerialNumbersView.Visible = _checkButtonUniqueArticles.Active;
            if (_checkButtonComposite.Active && _checkButtonUniqueArticles.Active) SetSizeRequest(550, 740); else { SetSizeRequest(500, 740); }
        }

        [Obsolete]
        private void InitUI()
        {
            string lastArticleCode = "0";

            try
            {
                try
                {
                    //IN009261 BackOffice - Inserir mais auto-completes nos forms
                    if (DatabaseSettings.DatabaseType.ToString() == "MSSqlServer")
                    {
                        string lastArticleSql = string.Format("SELECT MAX(CAST(Code AS INT))FROM fin_article");
                        lastArticleCode = XPOSettings.Session.ExecuteScalar(lastArticleSql).ToString();
                    }
                    else if (DatabaseSettings.DatabaseType.ToString() == "SQLite")
                    {
                        string lastArticleSql = string.Format("SELECT MAX(CAST(Code AS INT))FROM fin_article");
                        lastArticleCode = XPOSettings.Session.ExecuteScalar(lastArticleSql).ToString();
                    }
                    else if (DatabaseSettings.DatabaseType.ToString() == "MySql")
                    {
                        string lastArticleSql = string.Format("SELECT MAX(CAST(code AS UNSIGNED)) as Cast FROM fin_article;");
                        lastArticleCode = XPOSettings.Session.ExecuteScalar(lastArticleSql).ToString();
                    }

                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }

                //Init Local Vars
                _article = (_dataSourceRow as fin_article);
                if (_article.ArticleComposition.Count > 0)
                {
                    foreach (var line in _article.ArticleComposition)
                    {
                        _articlecompositions.Add(line);
                    }
                }

                if (_dialogMode != DialogMode.Insert)
                {
                    //Get totalNumberOfFinanceDocuments to check if article has already used in Finance Documents, to protect name changes etc
                    string sql = string.Format("SELECT COUNT(*) as Count FROM fin_documentfinancedetail WHERE Article = '{0}';", _article.Oid);
                    var sqlResult = XPOSettings.Session.ExecuteScalar(sql);
                    _totalNumberOfFinanceDocuments = Convert.ToUInt16(sqlResult);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxLabel = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", LogicPOS.Utility.RegexUtils.RegexAlfaNumericArticleCode, true));

                //CodeDealer
                Entry entryCodeDealer = new Entry();
                BOWidgetBox boxCodeDealer = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_code_dealer"), entryCodeDealer);
                vboxTab1.PackStart(boxCodeDealer, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCodeDealer, _dataSourceRow, "CodeDealer", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                // Changed from RegexAlfaNumeric to  RegexAlfaNumericExtended 2017-1011
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //ButtonLabel
                Entry entryButtonLabel = new Entry();
                BOWidgetBox boxButtonLabel = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_button_name"), entryButtonLabel);
                vboxTab1.PackStart(boxButtonLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxButtonLabel, _dataSourceRow, "ButtonLabel", LogicPOS.Utility.RegexUtils.RegexAlfaNumericArticleButtonLabel, false));

                //Composite article
                //_xpoComboBoxCompositeArticle = new XPOComboBox(DataSourceRow.Session, typeof(fin_articlefamily), (DataSourceRow as fin_article).Family, "Designation", null);
                //BOWidgetBox boxComposite = new BOWidgetBox(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_composite_article"), _xpoComboBoxCompositeArticle);
                //vboxTab1.PackStart(boxComposite, false, false, 0);
                //_crudWidgetList.Add(new GenericCRUDWidgetXPO(boxComposite, DataSourceRow, "CompositeArticle", SettingsApp.RegexGuid, true));

                //Family
                _xpoComboBoxFamily = new XPOComboBox(DataSourceRow.Session, typeof(fin_articlefamily), (DataSourceRow as fin_article).Family, "Designation", null);
                BOWidgetBox boxFamily = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_family"), _xpoComboBoxFamily);
                vboxTab1.PackStart(boxFamily, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxFamily, DataSourceRow, "Family", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //SubFamily
                _xpoComboBoxSubFamily = new XPOComboBox(DataSourceRow.Session, typeof(fin_articlesubfamily), (DataSourceRow as fin_article).SubFamily, "Designation", null);
                BOWidgetBox boxSubFamily = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_subfamily"), _xpoComboBoxSubFamily);
                vboxTab1.PackStart(boxSubFamily, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxSubFamily, DataSourceRow, "SubFamily", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //Type
                XPOComboBox xpoComboBoxType = new XPOComboBox(DataSourceRow.Session, typeof(fin_articletype), (DataSourceRow as fin_article).Type, "Designation", null);
                BOWidgetBox boxType = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_type"), xpoComboBoxType);
                vboxTab1.PackStart(boxType, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxType, DataSourceRow, "Type", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //ButtonImage
                FileChooserButton fileChooserButtonImage = new FileChooserButton(string.Empty, FileChooserAction.Open) { HeightRequest = 23 };
                Image fileChooserImagePreviewButtonImage = new Image() { WidthRequest = _sizefileChooserPreview.Width, HeightRequest = _sizefileChooserPreview.Height };
                Frame fileChooserFrameImagePreviewButtonImage = new Frame();
                fileChooserFrameImagePreviewButtonImage.ShadowType = ShadowType.None;
                fileChooserFrameImagePreviewButtonImage.Add(fileChooserImagePreviewButtonImage);
                fileChooserButtonImage.SetFilename(((fin_article)DataSourceRow).ButtonImage);
                fileChooserButtonImage.Filter = logicpos.Utils.GetFileFilterImages();
                fileChooserButtonImage.SelectionChanged += (sender, eventArgs) => fileChooserImagePreviewButtonImage.Pixbuf = logicpos.Utils.ResizeAndCropFileToPixBuf((sender as FileChooserButton).Filename, new System.Drawing.Size(fileChooserImagePreviewButtonImage.WidthRequest, fileChooserImagePreviewButtonImage.HeightRequest));
                BOWidgetBox boxfileChooserButtonImage = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_button_image"), fileChooserButtonImage);
                HBox hboxfileChooserAndimagePreviewButtonImage = new HBox(false, _boxSpacing);
                hboxfileChooserAndimagePreviewButtonImage.PackStart(boxfileChooserButtonImage, true, true, 0);
                hboxfileChooserAndimagePreviewButtonImage.PackStart(fileChooserFrameImagePreviewButtonImage, false, false, 0);
                vboxTab1.PackStart(hboxfileChooserAndimagePreviewButtonImage, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxfileChooserButtonImage, _dataSourceRow, "ButtonImage", string.Empty, false));

                //Artigos Compostos [IN:016522]
                //Composite
                _checkButtonComposite = new CheckButton(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"));
                vboxTab1.PackStart(_checkButtonComposite, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(_checkButtonComposite, _dataSourceRow, "IsComposed"));
                if (_article.ArticleComposition.Count > 0) { _checkButtonComposite.Active = true; }

                //Unique Articles (Have multi S/N)
                _checkButtonUniqueArticles = new CheckButton(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_unique_articles"));
                vboxTab1.PackStart(_checkButtonUniqueArticles, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(_checkButtonUniqueArticles, _dataSourceRow, "UniqueArticles"));
                _dataSourceRow.Reload();
                if (_article.ArticleSerialNumber.Count > 0) { _checkButtonUniqueArticles.Active = true; }
                _checkButtonUniqueArticles.Sensitive = false;

                //Favorite
                CheckButton checkButtonFavorite = new CheckButton(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_favorite"));
                vboxTab1.PackStart(checkButtonFavorite, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonFavorite, _dataSourceRow, "Favorite"));

                //UseWeighingBalance
                CheckButton checkButtonUseWeighingBalance = new CheckButton(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_use_weighing_balance"));
                vboxTab1.PackStart(checkButtonUseWeighingBalance, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonUseWeighingBalance, _dataSourceRow, "UseWeighingBalance"));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab2
                _vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                int col1width = 100, col2width = 90, col3width = col2width, col4width = 160;

                //hboxPrices
                Label labelPriceEmpty = new Label(string.Empty) { WidthRequest = col1width };
                Label labelPriceNormal = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "article_normal_price")) { WidthRequest = col2width };
                Label labelPricePromotion = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "article_promotion_price")) { WidthRequest = col3width };
                Label labelPriceUsePromotionPrice = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "article_use_promotion_price")) { WidthRequest = col4width };
                labelPriceNormal.SetAlignment(0.0F, 0.5F);
                labelPricePromotion.SetAlignment(0.0F, 0.5F);
                labelPriceUsePromotionPrice.SetAlignment(0.0F, 0.5F);

                VBox vboxPrices = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
                HBox hboxPrices = new HBox(false, _boxSpacing);
                hboxPrices.PackStart(labelPriceEmpty, true, true, 0);
                hboxPrices.PackStart(labelPriceNormal, false, false, 0);
                hboxPrices.PackStart(labelPricePromotion, false, false, 0);
                hboxPrices.PackStart(labelPriceUsePromotionPrice, false, false, 0);
                //PackIt VBox
                vboxPrices.PackStart(hboxPrices, false, false, 0);

                //Get PriceType Collection : Require Criteria to exclude SettingsApp.XpoOidUndefinedRecord, else we get a Price0 here
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) OR (Oid <> '{0}')", XPOSettings.XpoOidUndefinedRecord));
                XPCollection xpcConfigurationPriceType = new XPCollection(DataSourceRow.Session, typeof(fin_configurationpricetype), criteriaOperator);

                xpcConfigurationPriceType.Sorting = XPOHelper.GetXPCollectionDefaultSortingCollection();
                //Define Max 5 Rows : 5 Prices
                int priceTypeCount = (xpcConfigurationPriceType.Count > 5) ? 5 : xpcConfigurationPriceType.Count;

                //Loop and Render Columns
                for (int i = 0; i < priceTypeCount; i++)
                {
                    int priceTypeIndex = ((fin_configurationpricetype)xpcConfigurationPriceType[i]).EnumValue;

                    //FieldNames
                    string fieldNamePriceNormal = string.Format("Price{0}", priceTypeIndex);
                    string fieldNamePricePromotion = string.Format("Price{0}Promotion", priceTypeIndex);
                    string fieldNamePriceUsePromotionPrice = string.Format("Price{0}UsePromotionPrice", priceTypeIndex);
                    //PriceType
                    Label labelPriceType = new Label(((fin_configurationpricetype)xpcConfigurationPriceType[i]).Designation) { WidthRequest = col1width };
                    labelPriceType.SetAlignment(0.0F, 0.5F);

                    //Entrys
                    Entry entryPriceNormal = new Entry() { WidthRequest = col2width };
                    Entry entryPricePromotion = new Entry() { WidthRequest = col3width };
                    _crudWidgetList.Add(new GenericCRUDWidgetXPO(entryPriceNormal, _dataSourceRow, fieldNamePriceNormal, LogicPOS.Utility.RegexUtils.RegexDecimalGreaterEqualThanZero, true));
                    _crudWidgetList.Add(new GenericCRUDWidgetXPO(entryPricePromotion, _dataSourceRow, fieldNamePricePromotion, LogicPOS.Utility.RegexUtils.RegexDecimalGreaterEqualThanZero, true));
                    //UsePromotion
                    CheckButton checkButtonUsePromotion = new CheckButton(string.Empty) { WidthRequest = col4width };
                    _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonUsePromotion, _dataSourceRow, fieldNamePriceUsePromotionPrice));
                    //PackIt
                    hboxPrices = new HBox(false, _boxSpacing);
                    hboxPrices.PackStart(labelPriceType, true, true, 0);
                    hboxPrices.PackStart(entryPriceNormal, false, false, 0);
                    hboxPrices.PackStart(entryPricePromotion, false, false, 0);
                    hboxPrices.PackStart(checkButtonUsePromotion, false, false, 0);
                    //PackIt VBox
                    vboxPrices.PackStart(hboxPrices, false, false, 0);
                }
                _vboxTab2.PackStart(vboxPrices, false, false, 0);

                //PVPVariable
                CheckButton checkButtonPVPVariable = new CheckButton(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_variable_price"));
                _vboxTab2.PackStart(checkButtonPVPVariable, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonPVPVariable, _dataSourceRow, "PVPVariable"));

                //PriceWithVat
                CheckButton checkButtonPriceWithVat = new CheckButton(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_price_with_vat"));
                _vboxTab2.PackStart(checkButtonPriceWithVat, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonPriceWithVat, _dataSourceRow, "PriceWithVat"));

                //Discount
                Entry entryDiscount = new Entry();
                BOWidgetBox boxDiscount = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_discount"), entryDiscount);
                _vboxTab2.PackStart(boxDiscount, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDiscount, _dataSourceRow, "Discount", LogicPOS.Utility.RegexUtils.RegexPercentage, false));

                //Class
                XPOComboBox xpoComboBoxClass = new XPOComboBox(DataSourceRow.Session, typeof(fin_articleclass), (DataSourceRow as fin_article).Class, "Designation", null);
                BOWidgetBox boxClass = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_class"), xpoComboBoxClass);
                _vboxTab2.PackStart(boxClass, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxClass, DataSourceRow, "Class", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //Normal App Mode
                if (AppOperationModeSettings.IsDefaultTheme)/* IN008024 */
                {
                    //VatOnTable
                    _xpoComboBoxVatOnTable = new XPOComboBox(DataSourceRow.Session, typeof(fin_configurationvatrate), (DataSourceRow as fin_article).VatOnTable, "Designation", null);
                    BOWidgetBox boxVatOnTable = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_vat_on_table"), _xpoComboBoxVatOnTable);
                    _vboxTab2.PackStart(boxVatOnTable, false, false, 0);
                    _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxVatOnTable, DataSourceRow, "VatOnTable", LogicPOS.Utility.RegexUtils.RegexGuid, true));
                }

                //VatDirectSelling
                _xpoComboBoxVatDirectSelling = new XPOComboBox(DataSourceRow.Session, typeof(fin_configurationvatrate), (DataSourceRow as fin_article).VatDirectSelling, "Designation", null);
                BOWidgetBox boxVatDirectSelling = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_vat_direct_selling"), _xpoComboBoxVatDirectSelling);
                _vboxTab2.PackStart(boxVatDirectSelling, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxVatDirectSelling, DataSourceRow, "VatDirectSelling", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //VatExemptionReason
                _xpoComboBoxVatExemptionReason = new XPOComboBox(DataSourceRow.Session, typeof(fin_configurationvatexemptionreason), (DataSourceRow as fin_article).VatExemptionReason, "Designation", null);
                BOWidgetBox boxVatExemptionReason = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_vat_exemption_reason"), _xpoComboBoxVatExemptionReason);
                _vboxTab2.PackStart(boxVatExemptionReason, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxVatExemptionReason, DataSourceRow, "VatExemptionReason", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //Append Tab
                _notebook.AppendPage(_vboxTab2, new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_edit_article_tab2_label")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab3
                VBox vboxTab3 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //BarCode
                Entry entryBarCode = new Entry();
                BOWidgetBox boxBarCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_barcode"), entryBarCode);
                vboxTab3.PackStart(boxBarCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxBarCode, _dataSourceRow, "BarCode", LogicPOS.Utility.RegexUtils.RegexEan12andEan4, false));

                //Accounting
                Entry entryAccounting = new Entry();
                BOWidgetBox boxAccounting = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_stock"), entryAccounting);
                vboxTab3.PackStart(boxAccounting, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxAccounting, _dataSourceRow, "Accounting", LogicPOS.Utility.RegexUtils.RegexDecimal, false));
                //entryAccounting.Editable = false;
                try
                {
                    string stockQuery = string.Format("SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;", _article.Oid);
                    entryAccounting.Text = _dataSourceRow.Session.ExecuteScalar(stockQuery).ToString();
                }
                catch
                {
                    entryAccounting.Text = "0";
                }
                //MinimumStock
                Entry entryMinimumStock = new Entry();
                BOWidgetBox boxMinimumStock = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_minimum_stock"), entryMinimumStock);
                vboxTab3.PackStart(boxMinimumStock, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxMinimumStock, _dataSourceRow, "MinimumStock", LogicPOS.Utility.RegexUtils.RegexDecimal, false));

                //Tare
                Entry entryTare = new Entry();
                BOWidgetBox boxTare = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_tare"), entryTare);
                vboxTab3.PackStart(boxTare, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTare, _dataSourceRow, "Tare", LogicPOS.Utility.RegexUtils.RegexDecimal, false));

                //Weight
                Entry entryWeight = new Entry();
                BOWidgetBox boxWeight = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_weight"), entryWeight);
                vboxTab3.PackStart(boxWeight, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxWeight, _dataSourceRow, "Weight", LogicPOS.Utility.RegexUtils.RegexDecimal, false));

                //DefaultQuantity
                Entry entryDefaultQuantity = new Entry();
                BOWidgetBox boxDefaultQuantity = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_default_quantity"), entryDefaultQuantity);
                vboxTab3.PackStart(boxDefaultQuantity, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDefaultQuantity, _dataSourceRow, "DefaultQuantity", LogicPOS.Utility.RegexUtils.RegexDecimal, false));

                //UnitMeasure
                XPOComboBox xpoComboBoxUnitMeasure = new XPOComboBox(DataSourceRow.Session, typeof(cfg_configurationunitmeasure), (DataSourceRow as fin_article).UnitMeasure, "Designation", null);
                BOWidgetBox boxUnitMeasure = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_unit_measure"), xpoComboBoxUnitMeasure);
                vboxTab3.PackStart(boxUnitMeasure, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxUnitMeasure, DataSourceRow, "UnitMeasure", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //UnitSize
                XPOComboBox xpoComboBoxUnitSize = new XPOComboBox(DataSourceRow.Session, typeof(cfg_configurationunitsize), (DataSourceRow as fin_article).UnitSize, "Designation", null);
                BOWidgetBox boxUnitSize = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_unit_size"), xpoComboBoxUnitSize);
                vboxTab3.PackStart(boxUnitSize, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxUnitSize, DataSourceRow, "UnitSize", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //Printer
                XPOComboBox xpoComboBoxPrinter = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinters), (DataSourceRow as fin_article).Printer, "Designation", null);
                BOWidgetBox boxPrinter = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_device_printer"), xpoComboBoxPrinter);
                vboxTab3.PackStart(boxPrinter, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPrinter, DataSourceRow, "Printer", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //Template
                CriteriaOperator criteriaOperatorSelectTemplate = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (IsBarCode IS NULL OR IsBarCode = 0)"));
                XPOComboBox xpoComboBoxTemplate = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinterstemplates), (DataSourceRow as fin_article).Template, "Designation", criteriaOperatorSelectTemplate);
                BOWidgetBox boxTemplate = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ConfigurationPrintersTemplates"), xpoComboBoxTemplate);
                vboxTab3.PackStart(boxTemplate, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplate, DataSourceRow, "Template", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //TemplateBarCode
                CriteriaOperator criteriaOperatorSelectTemplateBarCode = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) AND IsBarCode = 1"));
                XPOComboBox xpoComboBoxTemplateBarCode = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinterstemplates), (DataSourceRow as fin_article).TemplateBarCode, "Designation", criteriaOperatorSelectTemplateBarCode);
                BOWidgetBox boxTemplateBarCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ConfigurationPrintersBarCodeTemplates"), xpoComboBoxTemplateBarCode);
                vboxTab3.PackStart(boxTemplateBarCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplateBarCode, DataSourceRow, "TemplateBarCode", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                if (GlobalApp.ScreenSize.Width > 800 && GlobalApp.ScreenSize.Height > 600)
                {
                    //CommissionGroup
                    XPOComboBox xpoComboBoxCommissionGroup = new XPOComboBox(DataSourceRow.Session, typeof(pos_usercommissiongroup), (DataSourceRow as fin_article).CommissionGroup, "Designation", null);
                    BOWidgetBox boxCommissionGroup = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_commission_group"), xpoComboBoxCommissionGroup);
                    vboxTab3.PackStart(boxCommissionGroup, false, false, 0);
                    _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCommissionGroup, DataSourceRow, "CommissionGroup", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                    //DiscountGroup
                    XPOComboBox xpoComboBoxDiscountGroup = new XPOComboBox(DataSourceRow.Session, typeof(erp_customerdiscountgroup), (DataSourceRow as fin_article).DiscountGroup, "Designation", null);
                    BOWidgetBox boxDiscountGroup = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_discount_group"), xpoComboBoxDiscountGroup);
                    vboxTab3.PackStart(boxDiscountGroup, false, false, 0);
                    _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDiscountGroup, DataSourceRow, "DiscountGroup", LogicPOS.Utility.RegexUtils.RegexGuid, false));
                }

                //Append Tab
                _notebook.AppendPage(vboxTab3, new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_edit_article_tab3_label")));

                //Artigos Compostos [IN:016522]
                //Tab4 Composite article
                vboxTab4 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                _scrolledWindowCompositionView = new ScrolledWindow();
                _scrolledWindowCompositionView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                _scrolledWindowCompositionView.ModifyBg(StateType.Normal, Color.White.ToGdkColor());
                _scrolledWindowCompositionView.ShadowType = ShadowType.None;
                _CompositionView = new Viewport();

                _notebook.AppendPage(_scrolledWindowCompositionView, new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_edit_article_tab4_label1")));

                //vboxTab4.PackStart(_buttonInsert, false, false, 0);
                //Add composite article #1
                _totalCompositeEntrys++;
                CriteriaOperator criteriaOperatorSelectArticle = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1)"));
                if (_article != null) { criteriaOperatorSelectArticle = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Oid != '{0}')", _article.Oid.ToString())); }
                _entryBoxSelectArticle1 = new XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article"), "Designation", "Oid", _article, criteriaOperatorSelectArticle, Enums.Keyboard.KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true, true, "", LogicPOS.Utility.RegexUtils.RegexDecimalGreaterThanZero, _totalCompositeEntrys);
                _entryBoxSelectArticle1.EntryValidation.IsEditable = true;
                _entryBoxSelectArticle1.EntryQtdValidation.IsEditable = true;
                _entryBoxSelectArticle1.Value = null;
                _entryBoxSelectArticle1.EntryValidation.Text = "";

                _entryBoxSelectArticle1.EntryValidation.Validate();
                _entryBoxSelectArticle1.EntryCodeValidation.Validate();
                _entryBoxSelectArticle1.EntryQtdValidation.Validate();

                _crudWidgetList.Add(new GenericCRUDWidgetXPO(_entryBoxSelectArticle1.EntryCodeValidation, _dataSourceRow, string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), _totalCompositeEntrys, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_code")), LogicPOS.Utility.RegexUtils.RegexAlfaNumericArticleCode, true));
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(_entryBoxSelectArticle1.EntryValidation, _dataSourceRow, string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), _totalCompositeEntrys, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation")), LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(_entryBoxSelectArticle1.EntryQtdValidation, _dataSourceRow, string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), _totalCompositeEntrys, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity")), LogicPOS.Utility.RegexUtils.RegexDecimalGreaterThanZero, true));

                _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), _totalCompositeEntrys, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_code"))) as GenericCRUDWidgetXPO);
                if (_genericCRUDWidgetXPO != null) _genericCRUDWidgetXPO.Validated = true;

                _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), _totalCompositeEntrys, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"))) as GenericCRUDWidgetXPO);
                if (_genericCRUDWidgetXPO != null) _genericCRUDWidgetXPO.Validated = true;

                _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), _totalCompositeEntrys, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity"))) as GenericCRUDWidgetXPO);
                if (_genericCRUDWidgetXPO != null) _genericCRUDWidgetXPO.Validated = true;

                vboxTab4.PackStart(_entryBoxSelectArticle1, false, false, 0);

                _entryCompositeLinesCollection.Add(_entryBoxSelectArticle1);

                _entryBoxSelectArticle1.BorderWidth = 0;

                _CompositionView.Add(vboxTab4);

                _scrolledWindowCompositionView.Add(_CompositionView);


                if (_article.ArticleComposition.Count > 0) PopulateCompositeArticleEntrys();

                //Tab 5
                vboxTab5 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                _scrolledWindowSerialNumbersView = new ScrolledWindow();
                _scrolledWindowSerialNumbersView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                _scrolledWindowSerialNumbersView.ModifyBg(StateType.Normal, Color.White.ToGdkColor());
                _scrolledWindowSerialNumbersView.ShadowType = ShadowType.None;

                _notebook.AppendPage(_scrolledWindowSerialNumbersView, new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_serial_number")));

                //SerialNumber
                _serialNumberCollection = new List<Tuple<fin_articleserialnumber, Entry, GenericCRUDWidgetXPO, GenericCRUDWidgetXPO, GenericCRUDWidgetXPO, HBox>>();
                int snCount = 0;
                if (_dataSourceRow != null && (_dataSourceRow as fin_article).ArticleSerialNumber.Count > 0)
                {
                    foreach (var serialNumber in (_dataSourceRow as fin_article).ArticleSerialNumber)
                    {
                        if (serialNumber.Oid != Guid.Empty && !serialNumber.Disabled)
                        {
                            XPGuidObject dataSourceRowSerialNumber = XPOHelper.GetXPGuidObject(typeof(fin_articleserialnumber), serialNumber.Oid);
                            if (dataSourceRowSerialNumber != null)
                            {
                                PopulateSerialNumberArticleEntrys(dataSourceRowSerialNumber);
                                snCount++;
                            }
                        }
                    }
                }
                //else if(_checkButtonUniqueArticles.Active == true)
                //{
                //    PopulateSerialNumberArticleEntrys(null);
                //}

                if (snCount == 0)
                {
                    _checkButtonUniqueArticles.Active = false;
                    _dataSourceRow.Session.Delete((_dataSourceRow as fin_article).ArticleSerialNumber);
                    _dataSourceRow.Session.Save((_dataSourceRow as fin_article).ArticleSerialNumber);
                }

                //Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(PosDocumentFinanceArticleDialog);

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Enable/Disable Components
                entryDesignation.Sensitive = _totalNumberOfFinanceDocuments == 0;

                //IN009261 BackOffice - Inserir mais auto-completes nos forms


                //Show or Hide vboxTab2  
                if (_article.Type != null) { _vboxTab2.Visible = _article.Type.HavePrice; }
                _scrolledWindowCompositionView.Visible = _checkButtonComposite.Active;
                //Assign Initial Value for Family
                DefineInitialValueForXpoComboBoxFamily();
                //Call UI Update for VatExemptionReason
                UpdateUIVatExemptionReason();

                //Events
                _xpoComboBoxFamily.Changed += xpoComboBoxFamily_Changed;
                xpoComboBoxType.Changed += xpoComboBoxType_Changed;
                _xpoComboBoxVatDirectSelling.Changed += xpoComboBoxVatDirectSelling_Changed;
                if (_xpoComboBoxVatOnTable != null) _xpoComboBoxVatOnTable.Changed += xpoComboBoxVatDirectSelling_Changed;
                _checkButtonComposite.Toggled += CheckButtonComposite_Toggled;
                _checkButtonUniqueArticles.Toggled += CheckButtonUniqueArticles_Toggled;

                //Events Composition
                _entryBoxSelectArticle1.ClosePopup += EntryBoxSelectArticle_ClosePopup;
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


                //string sql1 = string.Format(@"SELECT Oid FROM fin_configurationvatrate WHERE Oid == 'cee00590-7317-41b8-af46-66560401096b'");
                //Guid NormalVat = FrameworkUtils.GetGuidFromQuery(sql1);

                //if (_xpoComboBoxVatDirectSelling.Value == null)
                //{
                //    CriteriaOperator criteria = CriteriaOperator.Parse("");
                //    _xpoComboBoxVatOnTable.UpdateModel(criteria, XPOHelper.GetXPGuidObject(typeof(fin_configurationvatrate), NormalVat));
                //}
                //if (_xpoComboBoxVatOnTable.Value == null) _xpoComboBoxVatOnTable.Value = XPOHelper.GetXPGuidObject(typeof(fin_configurationvatrate), NormalVat);


                //Taxas de Iva por defeito na inserção de novos artigos
                if (_xpoComboBoxVatOnTable != null && _xpoComboBoxVatOnTable.Active == 0)
                {
                    _xpoComboBoxVatOnTable.Active = 1;
                }
                if (_xpoComboBoxVatDirectSelling.Active == 0)
                {
                    _xpoComboBoxVatDirectSelling.Active = 1;
                }

                int lcode = 0;
                lcode = Convert.ToInt32(lastArticleCode.ToString()) + 10;
                if (lcode != 10 && entryCode.Text == "") { entryCode.Text = lcode.ToString(); }
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void EntrySerialNumberDefault_TextInserted(object o, TextInsertedArgs args)
        {
            //_genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget("boxSerialNumberDefault") as GenericCRUDWidgetXPO);
            //_genericCRUDWidgetXPO.Validated = true;            
        }



        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void DefineInitialValueForXpoComboBoxFamily()
        {
            fin_articlefamily xpoArticleFamily = ((fin_articlefamily)_xpoComboBoxFamily.Value);
            fin_articlesubfamily xpoArticleSubFamily = ((fin_article)DataSourceRow).SubFamily;
            Guid currentOid = Guid.Empty;

            if (xpoArticleFamily != null)
            {
                currentOid = ((fin_articlefamily)_xpoComboBoxFamily.Value).Oid;
            }

            CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("Family = '{0}'", currentOid));
            _xpoComboBoxSubFamily.UpdateModel(criteria, xpoArticleSubFamily);
        }

        private void UpdateUIVatExemptionReason()
        {
            bool requireToChooseVatExemptionReason = true;

            try
            {
                // Override Default with Config Value
                requireToChooseVatExemptionReason = Convert.ToBoolean(GeneralSettings.Settings["requireToChooseVatExemptionReason"]);
            }
            catch (Exception)
            {
                _logger.Error("Error Missing Config Parameter Key: [requireToChooseVatExemptionReason]");
            }

            try
            {
                //Get VatExemptionReason
                GenericCRUDWidgetXPO genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget("VatExemptionReason") as GenericCRUDWidgetXPO);

                if (
                        (
                            (_xpoComboBoxVatOnTable != null && _xpoComboBoxVatOnTable.Value != null && _xpoComboBoxVatOnTable.Value.Oid == InvoiceSettings.XpoOidConfigurationVatRateDutyFree)
                            || (_xpoComboBoxVatDirectSelling != null && _xpoComboBoxVatDirectSelling.Value != null && _xpoComboBoxVatDirectSelling.Value.Oid == InvoiceSettings.XpoOidConfigurationVatRateDutyFree)
                        ) && requireToChooseVatExemptionReason
                    )
                {
                    _xpoComboBoxVatExemptionReason.Sensitive = true;
                    genericCRUDWidgetXPO.Required = true;
                }
                else
                {
                    //Assign Default Value to Undefined
                    _xpoComboBoxVatExemptionReason.Active = 0;
                    _xpoComboBoxVatExemptionReason.Sensitive = false;
                    genericCRUDWidgetXPO.Required = false;
                }

                //Call Validate to update UI
                genericCRUDWidgetXPO.ValidateField();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        //When Change Family, always Reset SubFamily to prevent wrong SubFamilys
        private void xpoComboBoxFamily_Changed(object sender, EventArgs e)
        {
            XPOComboBox comboBox = (XPOComboBox)sender;
            Guid currentOid = Guid.Empty;

            if (comboBox.Value != null)
            {
                currentOid = comboBox.Value.Oid;
            }

            CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("Family = '{0}'", currentOid));
            _xpoComboBoxSubFamily.UpdateModel(criteria);
        }

        private void xpoComboBoxType_Changed(object sender, EventArgs e)
        {
            XPOComboBox comboBox = (XPOComboBox)sender;
            fin_articletype xpoArticleType = (fin_articletype)comboBox.Value;

            //Fixed - Else Crash when cant get valid Object, like when choose --Indef--
            if (xpoArticleType != null)
            {
                _vboxTab2.Visible = xpoArticleType.HavePrice;
            }
            else
            {
                _vboxTab2.Visible = false;
            }
        }

        private void xpoComboBoxVatDirectSelling_Changed(object sender, EventArgs e)
        {
            UpdateUIVatExemptionReason();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Composite articles
        //Artigos Compostos [IN:016522]
        //Populate entrys on page load
        private void PopulateCompositeArticleEntrys()
        {
            try
            {
                foreach (fin_articlecomposition articleLine in _article.ArticleComposition)
                {
                    fin_article articleChild = articleLine.ArticleChild;
                    if (_entryBoxSelectArticle1.Value == null)
                    {
                        _entryBoxSelectArticle1.Value = articleChild;
                        _entryBoxSelectArticle1.EntryValidation.Text = articleChild.Designation;
                        _entryBoxSelectArticle1.EntryQtdValidation.Text = string.Format("{0:0.##}", articleLine.Quantity);
                        _entryBoxSelectArticle1.CodeEntry.Text = articleChild.Code;

                        _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), 1, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_code"))) as GenericCRUDWidgetXPO);
                        _genericCRUDWidgetXPO.Validated = true;

                        _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), 1, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"))) as GenericCRUDWidgetXPO);
                        _genericCRUDWidgetXPO.Validated = true;

                        _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), 1, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity"))) as GenericCRUDWidgetXPO);
                        if (_entryBoxSelectArticle1.EntryQtdValidation.Validated) _genericCRUDWidgetXPO.Validated = true;
                    }
                    else
                    {
                        _totalCompositeEntrys++;
                        NewBox_AddNewEntryAndPopulate(articleLine);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error populating Composite article Entry : " + ex.Message);
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
                    fin_article newArticle = (fin_article)XPOHelper.GetXPGuidObject(typeof(fin_article), articleOid);

                    if (isArticleCode)
                    {
                        pXPOEntry.EntryValidation.Text = (newArticle != null) ? newArticle.Designation.ToString() : CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_error");

                        var childQuantity = (pXPOEntry.Value.DefaultQuantity > 0) ? pXPOEntry.Value.DefaultQuantity : 1;

                        pXPOEntry.EntryQtdValidation.Text = (newArticle != null) ? string.Format("{0:0.##}", childQuantity) : CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_error");

                        pXPOEntry.EntryCodeValidation.Validate();

                        pXPOEntry.EntryValidation.Validate();

                        pXPOEntry.EntryQtdValidation.Validate();

                        _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), pXPOEntry.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_code"))) as GenericCRUDWidgetXPO);
                        _genericCRUDWidgetXPO.Validated = true;

                        _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), pXPOEntry.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"))) as GenericCRUDWidgetXPO);
                        _genericCRUDWidgetXPO.Validated = true;

                        _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), pXPOEntry.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity"))) as GenericCRUDWidgetXPO);
                        if (pXPOEntry.EntryQtdValidation.Validated) _genericCRUDWidgetXPO.Validated = true;


                        return;
                    }

                    pXPOEntry.EntryValidation.Text = (newArticle != null) ? newArticle.Designation.ToString() : CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_error");

                    pXPOEntry.EntryCodeValidation.Text = (newArticle != null) ? newArticle.Code.ToString() : CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_error");

                    decimal quantity = (pXPOEntry.Value.DefaultQuantity > 0) ? pXPOEntry.Value.DefaultQuantity : 1;

                    pXPOEntry.EntryQtdValidation.Text = (newArticle != null) ? string.Format("{0:0.##}", quantity) : CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_error");

                    pXPOEntry.Value = newArticle;

                    pXPOEntry.EntryCodeValidation.Validate();

                    pXPOEntry.EntryValidation.Validate();

                    pXPOEntry.EntryQtdValidation.Validate();

                    _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), pXPOEntry.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_code"))) as GenericCRUDWidgetXPO);
                    _genericCRUDWidgetXPO.Validated = true;

                    _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), pXPOEntry.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"))) as GenericCRUDWidgetXPO);
                    _genericCRUDWidgetXPO.Validated = true;

                    _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), pXPOEntry.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity"))) as GenericCRUDWidgetXPO);
                    if (pXPOEntry.EntryQtdValidation.Validated) _genericCRUDWidgetXPO.Validated = true;


                    //Clean previous value from colection
                    if (_previousValue != null)
                    {
                        foreach (fin_articlecomposition articleLine in _article.ArticleComposition)
                        {
                            if (articleLine.ArticleChild == _previousValue)
                            {
                                _article.ArticleComposition.Remove(articleLine);
                                break;
                            }
                        }
                    }

                    //Insert associated articles to collection
                    if (pXPOEntry.Value == _article)
                    {
                        pXPOEntry.Value = null;
                        logicpos.Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Ok, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_composite_article_same"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"));
                        pXPOEntry.EntryValidation.Text = "";
                        return;
                    }

                    bool insertNewArticleChild = false;
                    if (_article.ArticleComposition.Count > 0)
                    {
                        foreach (var line in _article.ArticleComposition)
                        {
                            if (line.ArticleChild == pXPOEntry.Value)
                            {
                                insertNewArticleChild = true;
                                pXPOEntry.EntryValidation.Text = "";
                                pXPOEntry.CodeEntry.Text = "";
                                pXPOEntry.EntryQtdValidation.Text = "";
                                pXPOEntry.Value = null;
                                pXPOEntry.EntryCodeValidation.Validate();
                                pXPOEntry.EntryValidation.Validate();
                                pXPOEntry.EntryQtdValidation.Validate();
                                _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), pXPOEntry.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_code"))) as GenericCRUDWidgetXPO);
                                _genericCRUDWidgetXPO.Validated = false;

                                _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), pXPOEntry.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"))) as GenericCRUDWidgetXPO);
                                _genericCRUDWidgetXPO.Validated = false;

                                _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), pXPOEntry.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity"))) as GenericCRUDWidgetXPO);
                                if (pXPOEntry.EntryQtdValidation.Validated) _genericCRUDWidgetXPO.Validated = false;

                                logicpos.Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Ok, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_composite_article_already"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"));
                                return;
                            }
                        }
                    }

                    if (!insertNewArticleChild)
                    {
                        var newCompostion = new fin_articlecomposition(_dataSourceRow.Session);
                        var articleChild = (fin_article)newCompostion.Session.GetObjectByKey(typeof(fin_article), pXPOEntry.Value.Oid);
                        newCompostion.ArticleChild = articleChild;
                        newCompostion.Quantity = (pXPOEntry.Value.DefaultQuantity > 0) ? pXPOEntry.Value.DefaultQuantity : 1;
                        newCompostion.Article = _article;
                        //var newComposition = new fin_articlecomposition(_dataSourceRow.Session) { ArticleChild = pXPOEntry.Value, Quantity = (pXPOEntry.Value.DefaultQuantity > 0) ? pXPOEntry.Value.DefaultQuantity : 1 };
                        _article.ArticleComposition.Add(newCompostion);
                        return;
                    }
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

        //Dynamic entrys
        private void NewBox_AddNewEntryAndPopulate(fin_articlecomposition pArticleComposition)
        {
            try
            {
                CriteriaOperator criteriaOperatorSelectArticle = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1)"));
                if (_article != null) { criteriaOperatorSelectArticle = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1 AND Oid != '{0}')", _article.Oid)); }
                XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle> NewEntryBoxSelectArticle = new XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article"), "Designation", "Oid", _article, criteriaOperatorSelectArticle, Enums.Keyboard.KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true, true, "", LogicPOS.Utility.RegexUtils.RegexDecimalGreaterThanZero, _totalCompositeEntrys);

                _crudWidgetList.Add(new GenericCRUDWidgetXPO(_entryBoxSelectArticle1.EntryCodeValidation, _dataSourceRow, string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), NewEntryBoxSelectArticle.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_code")), "", true));
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(_entryBoxSelectArticle1.EntryValidation, _dataSourceRow, string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), NewEntryBoxSelectArticle.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation")), LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(_entryBoxSelectArticle1.EntryQtdValidation, _dataSourceRow, string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), NewEntryBoxSelectArticle.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity")), LogicPOS.Utility.RegexUtils.RegexDecimalGreaterThanZero, true));

                NewEntryBoxSelectArticle.EntryValidation.IsEditable = true;
                fin_article articleChild = pArticleComposition.ArticleChild;

                NewEntryBoxSelectArticle.Value = articleChild;
                NewEntryBoxSelectArticle.EntryValidation.Text = articleChild.Designation;
                NewEntryBoxSelectArticle.EntryQtdValidation.Text = string.Format("{0:0.##}", pArticleComposition.Quantity);
                NewEntryBoxSelectArticle.CodeEntry.Text = articleChild.Code;

                //Events
                NewEntryBoxSelectArticle.ClosePopup += EntryBoxSelectArticle_ClosePopup;
                NewEntryBoxSelectArticle.CleanArticleEvent += EntryBoxSelectArticle_CleanArticleEvent;
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
                NewEntryBoxSelectArticle.EntryCodeValidation.Completion.Model = FillDropDownListStore(true, criteriaOperatorSelectArticle);
                NewEntryBoxSelectArticle.EntryCodeValidation.Completion.TextColumn = 0;
                NewEntryBoxSelectArticle.EntryCodeValidation.Completion.PopupCompletion = true;
                NewEntryBoxSelectArticle.EntryCodeValidation.Completion.InlineCompletion = false;
                NewEntryBoxSelectArticle.EntryCodeValidation.Completion.PopupSingleMatch = true;
                NewEntryBoxSelectArticle.EntryCodeValidation.Completion.InlineSelection = true;

                NewEntryBoxSelectArticle.EntryCodeValidation.Changed += delegate
                {
                    SelectRecordDropDownArticle(true, NewEntryBoxSelectArticle);
                };

                _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), _totalCompositeEntrys, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_code"))) as GenericCRUDWidgetXPO);
                _genericCRUDWidgetXPO.Validated = true;

                _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), _totalCompositeEntrys, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"))) as GenericCRUDWidgetXPO);
                _genericCRUDWidgetXPO.Validated = true;

                _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), _totalCompositeEntrys, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity"))) as GenericCRUDWidgetXPO);
                if (_entryBoxSelectArticle1.EntryQtdValidation.Validated) _genericCRUDWidgetXPO.Validated = true;

                _entryCompositeLinesCollection.Add(NewEntryBoxSelectArticle);


                vboxTab4.PackStart(NewEntryBoxSelectArticle, false, false, 0);
                _CompositionView.Add(vboxTab4);
                //scrolledWindowCompositionView.ShowAll();

            }
            catch (Exception ex)
            {
                _logger.Error("Error Adding new Composite article Entry : " + ex.Message);
            }

        }

        //Add new entry's event
        private void NewBox_AddNewEntryEvent(object sender, EventArgs e)
        {
            try
            {
                _totalCompositeEntrys++;
                //var entrySelected = (XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>)sender;
                CriteriaOperator criteriaOperatorSelectArticle = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1)"));
                if (_article != null) { criteriaOperatorSelectArticle = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) AND Oid != '{0}'", _article.Oid.ToString())); }
                XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle> NewEntryBoxSelectArticle = new XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article"), "Designation", "Oid", _article, criteriaOperatorSelectArticle, Enums.Keyboard.KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true, true, LogicPOS.Utility.RegexUtils.RegexAlfaNumericArticleCode, LogicPOS.Utility.RegexUtils.RegexDecimalGreaterThanZero, _totalCompositeEntrys);

                _crudWidgetList.Add(new GenericCRUDWidgetXPO(_entryBoxSelectArticle1.EntryCodeValidation, _dataSourceRow, string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), _totalCompositeEntrys, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_code")), LogicPOS.Utility.RegexUtils.RegexAlfaNumericArticleCode, true));

                _crudWidgetList.Add(new GenericCRUDWidgetXPO(_entryBoxSelectArticle1.EntryValidation, _dataSourceRow, string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), _totalCompositeEntrys, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation")), LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                _crudWidgetList.Add(new GenericCRUDWidgetXPO(_entryBoxSelectArticle1.EntryQtdValidation, _dataSourceRow, string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), _totalCompositeEntrys, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity")), LogicPOS.Utility.RegexUtils.RegexDecimalGreaterThanZero, true));

                NewEntryBoxSelectArticle.EntryValidation.IsEditable = true;
                NewEntryBoxSelectArticle.Value = null;
                NewEntryBoxSelectArticle.EntryValidation.Text = "";
                NewEntryBoxSelectArticle.EntryCodeValidation.Text = "";
                NewEntryBoxSelectArticle.EntryQtdValidation.Text = "";
                NewEntryBoxSelectArticle.EntryCodeValidation.Validate();
                NewEntryBoxSelectArticle.EntryQtdValidation.Validate();
                vboxTab4.PackStart(NewEntryBoxSelectArticle, false, false, 0);

                //Events
                NewEntryBoxSelectArticle.ClosePopup += EntryBoxSelectArticle_ClosePopup;
                NewEntryBoxSelectArticle.CleanArticleEvent += EntryBoxSelectArticle_CleanArticleEvent;
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
                _CompositionView.Add(vboxTab4);
                //eventBoxPosCompositionView.Add(scrolledWindowCompositionView);

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

                                    _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), 1, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_code"))) as GenericCRUDWidgetXPO);
                                    if (_genericCRUDWidgetXPO != null && _entryBoxSelectArticle1.EntryCodeValidation.Validated) _genericCRUDWidgetXPO.Validated = true; else { _genericCRUDWidgetXPO.Validated = false; }

                                    _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), 1, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"))) as GenericCRUDWidgetXPO);
                                    if (_genericCRUDWidgetXPO != null && _entryBoxSelectArticle1.EntryValidation.Validated) _genericCRUDWidgetXPO.Validated = true; else { _genericCRUDWidgetXPO.Validated = false; }

                                    _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), 1, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity"))) as GenericCRUDWidgetXPO);
                                    if (_genericCRUDWidgetXPO != null && _entryBoxSelectArticle1.EntryQtdValidation.Validated) _genericCRUDWidgetXPO.Validated = true; else { _genericCRUDWidgetXPO.Validated = false; }

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

                            _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), 1, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_code"))) as GenericCRUDWidgetXPO);
                            if (_genericCRUDWidgetXPO != null && _entryBoxSelectArticle1.EntryCodeValidation.Validated) _genericCRUDWidgetXPO.Validated = true; else { _genericCRUDWidgetXPO.Validated = false; }

                            _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), 1, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"))) as GenericCRUDWidgetXPO);
                            if (_genericCRUDWidgetXPO != null && _entryBoxSelectArticle1.EntryValidation.Validated) _genericCRUDWidgetXPO.Validated = true; else { _genericCRUDWidgetXPO.Validated = false; }

                            _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), 1, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity"))) as GenericCRUDWidgetXPO);
                            if (_genericCRUDWidgetXPO != null && _entryBoxSelectArticle1.EntryQtdValidation.Validated) _genericCRUDWidgetXPO.Validated = true; else { _genericCRUDWidgetXPO.Validated = false; }
                        }
                    }
                    else
                    {
                        entrySelected.Hide();

                        _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), entrySelected.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_code"))) as GenericCRUDWidgetXPO);
                        _crudWidgetList.Remove(_genericCRUDWidgetXPO);

                        _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), entrySelected.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"))) as GenericCRUDWidgetXPO);
                        _crudWidgetList.Remove(_genericCRUDWidgetXPO);

                        _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), entrySelected.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity"))) as GenericCRUDWidgetXPO);
                        _crudWidgetList.Remove(_genericCRUDWidgetXPO);

                        _totalCompositeEntrys--;

                        _entryCompositeLinesCollection.Remove(entrySelected);

                    }
                    if (entrySelected.Value != null)
                    {
                        fin_article auxArticle = new fin_article();

                        if (cleanFirstEntry)
                        {
                            auxArticle = (fin_article)_dataSourceRow.Session.GetObjectByKey(typeof(fin_article), articleToDeleteAux);
                        }
                        else
                        {
                            auxArticle = entrySelected.Value;
                        }
                        foreach (fin_articlecomposition articleLine in _article.ArticleComposition)
                        {
                            if (articleLine.ArticleChild == auxArticle)
                            {
                                _article.ArticleComposition.Remove(articleLine);
                                if (entrySelected != _entryBoxSelectArticle1) entrySelected = null;
                                break;
                            }
                        }
                        entrySelected = null;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        //Close popup articles event
        private void EntryBoxSelectArticle_ClosePopup(object sender, EventArgs e)
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
                    logicpos.Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Ok, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_composite_article_same"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"));
                    entrySelected.EntryValidation.Text = "";
                    return;
                }
                entrySelected.CodeEntry.Text = entrySelected.Value.Code;
                entrySelected.EntryQtdValidation.Text = string.Format("{0:0.##}", entrySelected.Value.DefaultQuantity);
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
                _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), entrySelected.EntryNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity"))) as GenericCRUDWidgetXPO);
                entrySelected.EntryQtdValidation.Validate();
                if (entrySelected.EntryQtdValidation.Validated) _genericCRUDWidgetXPO.Validated = true;

                if (entrySelected.Value != null)
                {
                    if (_article.ArticleComposition.Count > 0 && Convert.ToDecimal(entrySelected.EntryQtdValidation.Text) > 0)
                    {
                        foreach (fin_articlecomposition articleLine in _article.ArticleComposition)
                        {
                            if (articleLine.Article != null && articleLine.Article == _article && articleLine.ArticleChild == entrySelected.Value)
                            {
                                articleLine.Quantity = LogicPOS.Utility.DataConversionUtils.StringToDecimal(entrySelected.EntryQtdValidation.Text);
                                return;
                            }
                        }
                    }
                    else
                    {
                        _genericCRUDWidgetXPO.Validated = false;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error updating quantity from article child : " + ex.Message);
            }
        }


        //Toggle composite articles
        private void CheckButtonComposite_Toggled(object sender, EventArgs e)
        {
            var checkButtonToggled = (CheckButton)sender;
            if (checkButtonToggled.Active)
            {
                if (_checkButtonComposite.Active && _checkButtonUniqueArticles.Active) SetSizeRequest(550, 690); else { SetSizeRequest(500, 690); }
                _scrolledWindowCompositionView.Show();

                _entryBoxSelectArticle1.EntryCodeValidation.Validate();
                _entryBoxSelectArticle1.EntryValidation.Validate();
                _entryBoxSelectArticle1.EntryQtdValidation.Validate();

                _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), 1, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_code"))) as GenericCRUDWidgetXPO);
                if (_genericCRUDWidgetXPO != null && _entryBoxSelectArticle1.EntryCodeValidation.Validated) _genericCRUDWidgetXPO.Validated = true; else { _genericCRUDWidgetXPO.Validated = false; }

                _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), 1, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"))) as GenericCRUDWidgetXPO);
                if (_genericCRUDWidgetXPO != null && _entryBoxSelectArticle1.EntryValidation.Validated) _genericCRUDWidgetXPO.Validated = true; else { _genericCRUDWidgetXPO.Validated = false; }

                _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), 1, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity"))) as GenericCRUDWidgetXPO);
                if (_genericCRUDWidgetXPO != null && _entryBoxSelectArticle1.EntryQtdValidation.Validated) _genericCRUDWidgetXPO.Validated = true; else { _genericCRUDWidgetXPO.Validated = false; }

            }
            else
            {
                if (_checkButtonComposite.Active && _checkButtonUniqueArticles.Active) SetSizeRequest(550, 690); else { SetSizeRequest(500, 690); }
                _scrolledWindowCompositionView.Hide();

                for (int i = 1; i <= _totalCompositeEntrys; i++)
                {
                    _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), i, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_code"))) as GenericCRUDWidgetXPO);
                    if (_genericCRUDWidgetXPO != null) _genericCRUDWidgetXPO.Validated = true;

                    _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), i, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"))) as GenericCRUDWidgetXPO);
                    if (_genericCRUDWidgetXPO != null) _genericCRUDWidgetXPO.Validated = true;

                    _genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget(string.Format("{0} linha {1}: {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article"), i, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity"))) as GenericCRUDWidgetXPO);
                    if (_genericCRUDWidgetXPO != null) _genericCRUDWidgetXPO.Validated = true;
                }
            }
        }

        private void CheckButtonUniqueArticles_Toggled(object sender, EventArgs e)
        {
            try
            {
                var checkButtonToggled = (CheckButton)sender;
                if (checkButtonToggled.Active)
                {
                    if (_checkButtonComposite.Active && _checkButtonUniqueArticles.Active) SetSizeRequest(550, 690); else { SetSizeRequest(500, 690); }
                    foreach (var serialNumber in _serialNumberCollection)
                    {
                        var xpObject = serialNumber.Item1;
                        if (xpObject == null)
                        {
                            xpObject = new fin_articleserialnumber(_dataSourceRow.Session);
                        }
                        vboxTab5.PackStart(serialNumber.Item6, false, false, 0);
                        _crudWidgetList.Add(serialNumber.Item3);
                        _crudWidgetList.Add(serialNumber.Item4);
                        _crudWidgetList.Add(serialNumber.Item5);

                    }
                    _scrolledWindowSerialNumbersView.Show();
                    if (_serialNumberCollection.Count == 0)
                    {
                        PopulateSerialNumberArticleEntrys(null);
                    }
                }
                else
                {
                    if (_checkButtonComposite.Active && _checkButtonUniqueArticles.Active) SetSizeRequest(550, 690); else { SetSizeRequest(500, 690); }
                    _scrolledWindowSerialNumbersView.Hide();
                    foreach (var serialNumber in _serialNumberCollection)
                    {
                        var xpObject = serialNumber.Item1;
                        if (xpObject != null && xpObject.Oid == Guid.Empty)
                        {
                            xpObject.Delete();
                        }
                        vboxTab5.Remove(serialNumber.Item6);
                        _crudWidgetList.Remove(serialNumber.Item3);
                        _crudWidgetList.Remove(serialNumber.Item4);
                        _crudWidgetList.Remove(serialNumber.Item5);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error hiding serial numbers article Entry : " + ex.Message);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Unique articles
        //Populate entrys on page load
        private void PopulateSerialNumberArticleEntrys(XPGuidObject pDataSourceRow)
        {
            try
            {
                //Dynamic SerialNumber
                if (pDataSourceRow == null)
                {
                    pDataSourceRow = new fin_articleserialnumber(_dataSourceRow.Session);
                }
                XPGuidObject pWareHouseDataSourceRow = new fin_warehouse(_dataSourceRow.Session);
                if (pDataSourceRow != null && (pDataSourceRow as fin_articleserialnumber).ArticleWarehouse.Warehouse != null)
                {
                    pWareHouseDataSourceRow = (pDataSourceRow as fin_articleserialnumber).ArticleWarehouse.Warehouse;
                }


                //Stock Moviment In for serialNumber
                if (pDataSourceRow != null && ((pDataSourceRow as fin_articleserialnumber).StockMovimentIn == null || (pDataSourceRow as fin_articleserialnumber).StockMovimentIn.Oid == Guid.Empty))
                {
                    (pDataSourceRow as fin_articleserialnumber).StockMovimentIn = new fin_articlestock(_dataSourceRow.Session);
                    (pDataSourceRow as fin_articleserialnumber).StockMovimentIn.Article = _article;
                    (pDataSourceRow as fin_articleserialnumber).StockMovimentIn.ArticleSerialNumber = (pDataSourceRow as fin_articleserialnumber);
                    (pDataSourceRow as fin_articleserialnumber).StockMovimentIn.Customer = (erp_customer)_dataSourceRow.Session.GetObjectByKey(typeof(erp_customer), XPOSettings.XpoOidUserRecord);
                    (pDataSourceRow as fin_articleserialnumber).StockMovimentIn.DocumentNumber = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_internal_moviment");
                    (pDataSourceRow as fin_articleserialnumber).StockMovimentIn.Quantity = 1;
                    (pDataSourceRow as fin_articleserialnumber).StockMovimentIn.Date = DateTime.Now;
                }

                //SerialNumber
                (pDataSourceRow as fin_articleserialnumber).Article = _article;
                HBox hboxSerialNumber = new HBox(false, _boxSpacing);
                Entry entrySerialNumber = new Entry();
                BOWidgetBox boxSerialNumber = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_serial_number"), entrySerialNumber);
                GenericCRUDWidgetXPO serialnumberCRUDWidgetXPO = new GenericCRUDWidgetXPO(boxSerialNumber, pDataSourceRow, "SerialNumber", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true);
                _crudWidgetList.Add(serialnumberCRUDWidgetXPO);
                hboxSerialNumber.PackStart(boxSerialNumber);

                //Warehouse
                CriteriaOperator defaultWarehouseCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND IsDefault == '1'"));
                fin_warehouse defaultWareHouse = ((pDataSourceRow as fin_articleserialnumber).ArticleWarehouse.Warehouse != null && (pDataSourceRow as fin_articleserialnumber).ArticleWarehouse.Warehouse.Oid != Guid.Empty) ? (pDataSourceRow as fin_articleserialnumber).ArticleWarehouse.Warehouse : (fin_warehouse)pDataSourceRow.Session.FindObject(typeof(fin_warehouse), defaultWarehouseCriteria);
                XPOComboBox xpoComboBoxWarehouse = new XPOComboBox(DataSourceRow.Session, typeof(fin_warehouse), defaultWareHouse, "Designation", null);
                BOWidgetBox boxWareHouse = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_warehouse"), xpoComboBoxWarehouse);
                GenericCRUDWidgetXPO warehouseCRUDWidgetXPO = new GenericCRUDWidgetXPO(boxWareHouse, (pDataSourceRow as fin_articleserialnumber).ArticleWarehouse, "Warehouse", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false);
                _crudWidgetList.Add(warehouseCRUDWidgetXPO);
                hboxSerialNumber.PackStart(boxWareHouse);

                //Location
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) "));
                if (defaultWareHouse != null) criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND Warehouse == '{0}'", defaultWareHouse.Oid.ToString()));
                fin_warehouselocation defaultLocation = ((pDataSourceRow as fin_articleserialnumber).ArticleWarehouse.Location != null) ? (pDataSourceRow as fin_articleserialnumber).ArticleWarehouse.Location : (fin_warehouselocation)pDataSourceRow.Session.FindObject(typeof(fin_warehouselocation), criteria);
                XPOComboBox xpoComboBoxWarehouseLocation = new XPOComboBox(DataSourceRow.Session, typeof(fin_warehouselocation), defaultLocation, "Designation", criteria);
                BOWidgetBox boxWareHouseLocation = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ConfigurationDevice_PlaceTerminal"), xpoComboBoxWarehouseLocation);
                GenericCRUDWidgetXPO warehouseLocationCRUDWidgetXPO = new GenericCRUDWidgetXPO(boxWareHouseLocation, (pDataSourceRow as fin_articleserialnumber).ArticleWarehouse, "Location", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false);
                _crudWidgetList.Add(warehouseLocationCRUDWidgetXPO);
                hboxSerialNumber.PackStart(boxWareHouseLocation);
                if (defaultWareHouse == null)
                {
                    xpoComboBoxWarehouseLocation.Active = 0;
                    xpoComboBoxWarehouseLocation.Sensitive = false;
                }

                //If serial number already sold cannot be edited
                if ((pDataSourceRow as fin_articleserialnumber).StockMovimentOut != null)
                {
                    entrySerialNumber.Sensitive = false;
                    xpoComboBoxWarehouseLocation.Sensitive = false;
                    xpoComboBoxWarehouse.Sensitive = false;
                }

                //Clear
                TouchButtonIcon buttonClearSerialNumber = new TouchButtonIcon("touchButtonIcon", Color.Transparent, iconClearRecord, new Size(20, 20), 30, 30);
                hboxSerialNumber.PackEnd(buttonClearSerialNumber, false, false, 1);

                //Add
                TouchButtonIcon buttonAddSerialNumber = new TouchButtonIcon("touchButtonIcon", Color.Transparent, iconAddRecord, new Size(20, 20), 30, 30);
                hboxSerialNumber.PackEnd(buttonAddSerialNumber, false, false, 1);

                vboxTab5.PackStart(hboxSerialNumber, false, false, 0);
                _scrolledWindowSerialNumbersView.AddWithViewport(vboxTab5);

                //Events
                buttonAddSerialNumber.Clicked += delegate
                {
                    PopulateSerialNumberArticleEntrys(null);
                };
                buttonClearSerialNumber.Sensitive = false;
                buttonAddSerialNumber.Sensitive = false;
                buttonClearSerialNumber.Clicked += ButtonClearSerialNumber_Clicked;
                xpoComboBoxWarehouse.Changed += XpoComboBoxWarehouse_Changed;
                vboxTab5.ShowAll();
                //Add to collection
                _serialNumberCollection.Add(new Tuple<fin_articleserialnumber, Entry, GenericCRUDWidgetXPO, GenericCRUDWidgetXPO, GenericCRUDWidgetXPO, HBox>(
                    pDataSourceRow as fin_articleserialnumber,  //T1
                    entrySerialNumber,                          //T2
                    serialnumberCRUDWidgetXPO,                  //T3
                    warehouseCRUDWidgetXPO,                     //T4
                    warehouseLocationCRUDWidgetXPO,             //T5
                    hboxSerialNumber));                         //T6
            }
            catch (Exception ex)
            {
                _logger.Error("Error populating SerialNumber Entrys : " + ex.Message);
            }
        }

        private void XpoComboBoxWarehouse_Changed(object sender, EventArgs e)
        {
            try
            {
                bool found = false;
                fin_warehouse parent = new fin_warehouse();
                foreach (var serialNumber in _serialNumberCollection)
                {
                    foreach (var child in serialNumber.Item6.Children)
                    {

                        if (!found && child.GetType() == typeof(BOWidgetBox) && (child as BOWidgetBox).Children[1] as XPOComboBox == sender as XPOComboBox)
                        {
                            found = true;
                            parent = (((child as BOWidgetBox).Children[1] as XPOComboBox).Value as fin_warehouse);
                            continue;
                        }
                        if (found && child.GetType() == typeof(BOWidgetBox) && parent != null)
                        {
                            CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND Warehouse == '{0}'", parent.Oid.ToString()));
                            //var xpCollection = new XPCollection(_dataSourceRow.Session, typeof(fin_warehouselocation));
                            ((child as BOWidgetBox).Children[1] as XPOComboBox).UpdateModel(criteria, null);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error clear SerialNumber Entry : " + ex.Message);
            }
        }

        private void ButtonClearSerialNumber_Clicked(object sender, EventArgs e)
        {
            try
            {
                ResponseType responseType = logicpos.Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Question, ButtonsType.YesNo, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_delete_record"), string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_warning"), GeneralSettings.ServerVersion));

                if (responseType == ResponseType.Yes)
                {
                    foreach (var serialNumber in _serialNumberCollection)
                    {
                        if (_serialNumberCollection.Count == 1)
                        {
                            serialNumber.Item2.Text = "";
                            return;
                        }
                        foreach (var child in serialNumber.Item6.Children)
                        {
                            if (child.Equals(sender as TouchButtonIcon))
                            {
                                var xpObject = serialNumber.Item1;
                                _article.ArticleSerialNumber.Remove(xpObject);
                                vboxTab5.Remove(serialNumber.Item6);
                                _crudWidgetList.Remove(serialNumber.Item3);
                                _crudWidgetList.Remove(serialNumber.Item4);
                                _crudWidgetList.Remove(serialNumber.Item5);
                                _serialNumberCollection.Remove(serialNumber);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error clear SerialNumber Entry : " + ex.Message);
            }

        }
    }

}

