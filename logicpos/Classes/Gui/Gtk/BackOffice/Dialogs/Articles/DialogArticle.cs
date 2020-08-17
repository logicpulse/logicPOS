using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;
using System;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogArticle : BOBaseDialog
    {
        //UI
        VBox _vboxTab2;
        //private XPOComboBox _xpoComboBoxCompositeArticle;
        private XPOComboBox _xpoComboBoxFamily;
        private XPOComboBox _xpoComboBoxSubFamily;
        private XPOComboBox _xpoComboBoxVatOnTable;
        private XPOComboBox _xpoComboBoxVatDirectSelling;
        private XPOComboBox _xpoComboBoxVatExemptionReason;
        //Non UI
        private fin_article _article = null;
        //Other
        private uint _totalNumberOfFinanceDocuments = 0;

        public DialogArticle(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pDialogFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pDialogFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_edit_article"));

            if (GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600)
            {
                SetSizeRequest(500, 640);
            }
            else
            {
                SetSizeRequest(500, 670);
            }

            InitUI();
            InitNotes();
            ShowAll();
        }

        private void InitUI()
        {
            string lastArticleCode = "0";

            try
            {
                try
                {
                    //IN009261 BackOffice - Inserir mais auto-completes nos forms
                    if(GlobalFramework.DatabaseType.ToString() == "MSSqlServer"){
                        string lastArticleSql = string.Format("SELECT MAX(CAST(Code AS INT))FROM fin_article");
                        lastArticleCode = GlobalFramework.SessionXpo.ExecuteScalar(lastArticleSql).ToString();
                    }
                    else if (GlobalFramework.DatabaseType.ToString() == "SQLite")
                    {
                        string lastArticleSql = string.Format("SELECT MAX(CAST(Code AS INT))FROM fin_article");
                        lastArticleCode = GlobalFramework.SessionXpo.ExecuteScalar(lastArticleSql).ToString();
                    }
                    else if (GlobalFramework.DatabaseType.ToString() == "MySql")
                    {
                        string lastArticleSql = string.Format("SELECT MAX(CAST(code AS UNSIGNED)) as Cast FROM fin_article;");
                        lastArticleCode = GlobalFramework.SessionXpo.ExecuteScalar(lastArticleSql).ToString();
                    }
                        
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                }

                //Init Local Vars
                _article = (_dataSourceRow as fin_article);

                if (_dialogMode != DialogMode.Insert)
                {
                    //Get totalNumberOfFinanceDocuments to check if article has already used in Finance Documents, to protect name changes etc
                    string sql = string.Format("SELECT COUNT(*) as Count FROM fin_documentfinancedetail WHERE Article = '{0}';", _article.Oid);
                    var sqlResult = GlobalFramework.SessionXpo.ExecuteScalar(sql);
                    _totalNumberOfFinanceDocuments = Convert.ToUInt16(sqlResult);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxLabel = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", SettingsApp.RegexAlfaNumericArticleCode, true));

                //CodeDealer
                Entry entryCodeDealer = new Entry();
                BOWidgetBox boxCodeDealer = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_code_dealer"), entryCodeDealer);
                vboxTab1.PackStart(boxCodeDealer, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCodeDealer, _dataSourceRow, "CodeDealer", SettingsApp.RegexAlfaNumeric, false));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                // Changed from RegexAlfaNumeric to  RegexAlfaNumericExtended 2017-1011
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", SettingsApp.RegexAlfaNumericExtended, true));

                //ButtonLabel
                Entry entryButtonLabel = new Entry();
                BOWidgetBox boxButtonLabel = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_name"), entryButtonLabel);
                vboxTab1.PackStart(boxButtonLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxButtonLabel, _dataSourceRow, "ButtonLabel", SettingsApp.RegexAlfaNumericArticleButtonLabel, false));

                //Composite article
                //_xpoComboBoxCompositeArticle = new XPOComboBox(DataSourceRow.Session, typeof(fin_articlefamily), (DataSourceRow as fin_article).Family, "Designation", null);
                //BOWidgetBox boxComposite = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_composite_article"), _xpoComboBoxCompositeArticle);
                //vboxTab1.PackStart(boxComposite, false, false, 0);
                //_crudWidgetList.Add(new GenericCRUDWidgetXPO(boxComposite, DataSourceRow, "CompositeArticle", SettingsApp.RegexGuid, true));

                //Family
                _xpoComboBoxFamily = new XPOComboBox(DataSourceRow.Session, typeof(fin_articlefamily), (DataSourceRow as fin_article).Family, "Designation", null);
                BOWidgetBox boxFamily = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_article_family"), _xpoComboBoxFamily);
                vboxTab1.PackStart(boxFamily, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxFamily, DataSourceRow, "Family", SettingsApp.RegexGuid, true));

                //SubFamily
                _xpoComboBoxSubFamily = new XPOComboBox(DataSourceRow.Session, typeof(fin_articlesubfamily), (DataSourceRow as fin_article).SubFamily, "Designation", null);
                BOWidgetBox boxSubFamily = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_article_subfamily"), _xpoComboBoxSubFamily);
                vboxTab1.PackStart(boxSubFamily, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxSubFamily, DataSourceRow, "SubFamily", SettingsApp.RegexGuid, true));

                //Type
                XPOComboBox xpoComboBoxType = new XPOComboBox(DataSourceRow.Session, typeof(fin_articletype), (DataSourceRow as fin_article).Type, "Designation", null);
                BOWidgetBox boxType = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_article_type"), xpoComboBoxType);
                vboxTab1.PackStart(boxType, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxType, DataSourceRow, "Type", SettingsApp.RegexGuid, true));

                //ButtonImage
                FileChooserButton fileChooserButtonImage = new FileChooserButton(string.Empty, FileChooserAction.Open) { HeightRequest = 23 };
                Image fileChooserImagePreviewButtonImage = new Image() { WidthRequest = _sizefileChooserPreview.Width, HeightRequest = _sizefileChooserPreview.Height };
                Frame fileChooserFrameImagePreviewButtonImage = new Frame();
                fileChooserFrameImagePreviewButtonImage.ShadowType = ShadowType.None;
                fileChooserFrameImagePreviewButtonImage.Add(fileChooserImagePreviewButtonImage);
                fileChooserButtonImage.SetFilename(((fin_article)DataSourceRow).ButtonImage);
                fileChooserButtonImage.Filter = Utils.GetFileFilterImages();
                fileChooserButtonImage.SelectionChanged += (sender, eventArgs) => fileChooserImagePreviewButtonImage.Pixbuf = Utils.ResizeAndCropFileToPixBuf((sender as FileChooserButton).Filename, new System.Drawing.Size(fileChooserImagePreviewButtonImage.WidthRequest, fileChooserImagePreviewButtonImage.HeightRequest));
                BOWidgetBox boxfileChooserButtonImage = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_image"), fileChooserButtonImage);
                HBox hboxfileChooserAndimagePreviewButtonImage = new HBox(false, _boxSpacing);
                hboxfileChooserAndimagePreviewButtonImage.PackStart(boxfileChooserButtonImage, true, true, 0);
                hboxfileChooserAndimagePreviewButtonImage.PackStart(fileChooserFrameImagePreviewButtonImage, false, false, 0);
                vboxTab1.PackStart(hboxfileChooserAndimagePreviewButtonImage, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxfileChooserButtonImage, _dataSourceRow, "ButtonImage", string.Empty, false));

                //Favorite
                CheckButton checkButtonFavorite = new CheckButton(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_favorite"));
                vboxTab1.PackStart(checkButtonFavorite, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonFavorite, _dataSourceRow, "Favorite"));

                //UseWeighingBalance
                CheckButton checkButtonUseWeighingBalance = new CheckButton(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_use_weighing_balance"));
                vboxTab1.PackStart(checkButtonUseWeighingBalance, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonUseWeighingBalance, _dataSourceRow, "UseWeighingBalance"));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = SettingsApp.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab2
                _vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                int col1width = 100, col2width = 90, col3width = col2width, col4width = 160;

                //hboxPrices
                Label labelPriceEmpty = new Label(string.Empty) { WidthRequest = col1width };
                Label labelPriceNormal = new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "article_normal_price")) { WidthRequest = col2width };
                Label labelPricePromotion = new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "article_promotion_price")) { WidthRequest = col3width };
                Label labelPriceUsePromotionPrice = new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "article_use_promotion_price")) { WidthRequest = col4width };
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
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) OR (Oid <> '{0}')", SettingsApp.XpoOidUndefinedRecord));
                XPCollection xpcConfigurationPriceType = new XPCollection(DataSourceRow.Session, typeof(fin_configurationpricetype), criteriaOperator);

                xpcConfigurationPriceType.Sorting = FrameworkUtils.GetXPCollectionDefaultSortingCollection();
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
                    _crudWidgetList.Add(new GenericCRUDWidgetXPO(entryPriceNormal, _dataSourceRow, fieldNamePriceNormal, SettingsApp.RegexDecimalGreaterEqualThanZero, true));
                    _crudWidgetList.Add(new GenericCRUDWidgetXPO(entryPricePromotion, _dataSourceRow, fieldNamePricePromotion, SettingsApp.RegexDecimalGreaterEqualThanZero, true));
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
                CheckButton checkButtonPVPVariable = new CheckButton(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_variable_price"));
                _vboxTab2.PackStart(checkButtonPVPVariable, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonPVPVariable, _dataSourceRow, "PVPVariable"));

                //PriceWithVat
                CheckButton checkButtonPriceWithVat = new CheckButton(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_price_with_vat"));
                _vboxTab2.PackStart(checkButtonPriceWithVat, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonPriceWithVat, _dataSourceRow, "PriceWithVat"));

                //Discount
                Entry entryDiscount = new Entry();
                BOWidgetBox boxDiscount = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_discount"), entryDiscount);
                _vboxTab2.PackStart(boxDiscount, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDiscount, _dataSourceRow, "Discount", SettingsApp.RegexPercentage, false));

                //Class
                XPOComboBox xpoComboBoxClass = new XPOComboBox(DataSourceRow.Session, typeof(fin_articleclass), (DataSourceRow as fin_article).Class, "Designation", null);
                BOWidgetBox boxClass = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_article_class"), xpoComboBoxClass);
                _vboxTab2.PackStart(boxClass, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxClass, DataSourceRow, "Class", SettingsApp.RegexGuid, true));

                //Normal App Mode
                if (SettingsApp.IsDefaultTheme)/* IN008024 */
                {
                    //VatOnTable
                    _xpoComboBoxVatOnTable = new XPOComboBox(DataSourceRow.Session, typeof(fin_configurationvatrate), (DataSourceRow as fin_article).VatOnTable, "Designation", null);
                    BOWidgetBox boxVatOnTable = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_vat_on_table"), _xpoComboBoxVatOnTable);
                    _vboxTab2.PackStart(boxVatOnTable, false, false, 0);
                    _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxVatOnTable, DataSourceRow, "VatOnTable", SettingsApp.RegexGuid, true));
                }

                //VatDirectSelling
                _xpoComboBoxVatDirectSelling = new XPOComboBox(DataSourceRow.Session, typeof(fin_configurationvatrate), (DataSourceRow as fin_article).VatDirectSelling, "Designation", null);
                BOWidgetBox boxVatDirectSelling = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_vat_direct_selling"), _xpoComboBoxVatDirectSelling);
                _vboxTab2.PackStart(boxVatDirectSelling, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxVatDirectSelling, DataSourceRow, "VatDirectSelling", SettingsApp.RegexGuid, true));

                //VatExemptionReason
                _xpoComboBoxVatExemptionReason = new XPOComboBox(DataSourceRow.Session, typeof(fin_configurationvatexemptionreason), (DataSourceRow as fin_article).VatExemptionReason, "Designation", null);
                BOWidgetBox boxVatExemptionReason = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_vat_exemption_reason"), _xpoComboBoxVatExemptionReason);
                _vboxTab2.PackStart(boxVatExemptionReason, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxVatExemptionReason, DataSourceRow, "VatExemptionReason", SettingsApp.RegexGuid, true));

                //Append Tab
                _notebook.AppendPage(_vboxTab2, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_edit_article_tab2_label")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab3
                VBox vboxTab3 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //BarCode
                Entry entryBarCode = new Entry();
                BOWidgetBox boxBarCode = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_barcode"), entryBarCode);
                vboxTab3.PackStart(boxBarCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxBarCode, _dataSourceRow, "BarCode", SettingsApp.RegexEan12andEan4, false));

                //Accounting
                Entry entryAccounting = new Entry();
                BOWidgetBox boxAccounting = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_accounting"), entryAccounting);
                vboxTab3.PackStart(boxAccounting, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxAccounting, _dataSourceRow, "Accounting", SettingsApp.RegexDecimal, false));

                //Tare
                Entry entryTare = new Entry();
                BOWidgetBox boxTare = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_tare"), entryTare);
                vboxTab3.PackStart(boxTare, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTare, _dataSourceRow, "Tare", SettingsApp.RegexDecimal, false));

                //Weight
                Entry entryWeight = new Entry();
                BOWidgetBox boxWeight = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_weight"), entryWeight);
                vboxTab3.PackStart(boxWeight, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxWeight, _dataSourceRow, "Weight", SettingsApp.RegexDecimal, false));

                //DefaultQuantity
                Entry entryDefaultQuantity = new Entry();
                BOWidgetBox boxDefaultQuantity = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_article_default_quantity"), entryDefaultQuantity);
                vboxTab3.PackStart(boxDefaultQuantity, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDefaultQuantity, _dataSourceRow, "DefaultQuantity", SettingsApp.RegexDecimal, false));

                //UnitMeasure
                XPOComboBox xpoComboBoxUnitMeasure = new XPOComboBox(DataSourceRow.Session, typeof(cfg_configurationunitmeasure), (DataSourceRow as fin_article).UnitMeasure, "Designation", null);
                BOWidgetBox boxUnitMeasure = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_unit_measure"), xpoComboBoxUnitMeasure);
                vboxTab3.PackStart(boxUnitMeasure, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxUnitMeasure, DataSourceRow, "UnitMeasure", SettingsApp.RegexGuid, true));

                //UnitSize
                XPOComboBox xpoComboBoxUnitSize = new XPOComboBox(DataSourceRow.Session, typeof(cfg_configurationunitsize), (DataSourceRow as fin_article).UnitSize, "Designation", null);
                BOWidgetBox boxUnitSize = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_unit_size"), xpoComboBoxUnitSize);
                vboxTab3.PackStart(boxUnitSize, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxUnitSize, DataSourceRow, "UnitSize", SettingsApp.RegexGuid, true));

                //Printer
                XPOComboBox xpoComboBoxPrinter = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinters), (DataSourceRow as fin_article).Printer, "Designation", null);
                BOWidgetBox boxPrinter = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_device_printer"), xpoComboBoxPrinter);
                vboxTab3.PackStart(boxPrinter, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPrinter, DataSourceRow, "Printer", SettingsApp.RegexGuid, false));

                //Template
                XPOComboBox xpoComboBoxTemplate = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinterstemplates), (DataSourceRow as fin_article).Template, "Designation", null);
                BOWidgetBox boxTemplate = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_ConfigurationPrintersTemplates"), xpoComboBoxTemplate);
                vboxTab3.PackStart(boxTemplate, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplate, DataSourceRow, "Template", SettingsApp.RegexGuid, false));

                if (GlobalApp.ScreenSize.Width > 800 && GlobalApp.ScreenSize.Height > 600)
                {
                    //CommissionGroup
                    XPOComboBox xpoComboBoxCommissionGroup = new XPOComboBox(DataSourceRow.Session, typeof(pos_usercommissiongroup), (DataSourceRow as fin_article).CommissionGroup, "Designation", null);
                    BOWidgetBox boxCommissionGroup = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_commission_group"), xpoComboBoxCommissionGroup);
                    vboxTab3.PackStart(boxCommissionGroup, false, false, 0);
                    _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCommissionGroup, DataSourceRow, "CommissionGroup", SettingsApp.RegexGuid, false));

                    //DiscountGroup
                    XPOComboBox xpoComboBoxDiscountGroup = new XPOComboBox(DataSourceRow.Session, typeof(erp_customerdiscountgroup), (DataSourceRow as fin_article).DiscountGroup, "Designation", null);
                    BOWidgetBox boxDiscountGroup = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_discount_group"), xpoComboBoxDiscountGroup);
                    vboxTab3.PackStart(boxDiscountGroup, false, false, 0);
                    _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDiscountGroup, DataSourceRow, "DiscountGroup", SettingsApp.RegexGuid, false));
                }

                //Append Tab
                _notebook.AppendPage(vboxTab3, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_edit_article_tab3_label")));


                //Tab4
                //VBox vboxTab4 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
                //_notebook.AppendPage(vboxTab4, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_edit_article_tab4_label1")));
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Enable/Disable Components
                entryDesignation.Sensitive = (_totalNumberOfFinanceDocuments > 0) ? false : true;

                //IN009261 BackOffice - Inserir mais auto-completes nos forms

                int lcode = 0;
                lcode = Convert.ToInt32(lastArticleCode.ToString()) + 10;
                if (lcode != 10 && entryCode.Text == "") { entryCode.Text = lcode.ToString(); }
                
                //Show or Hide vboxTab2  
                if (_article.Type != null) { _vboxTab2.Visible = _article.Type.HavePrice; }

                //Assign Initial Value for Family
                DefineInitialValueForXpoComboBoxFamily();
                //Call UI Update for VatExemptionReason
                UpdateUIVatExemptionReason();

                //Events
                _xpoComboBoxFamily.Changed += xpoComboBoxFamily_Changed;
                xpoComboBoxType.Changed += xpoComboBoxType_Changed;
                _xpoComboBoxVatDirectSelling.Changed += xpoComboBoxVatDirectSelling_Changed;
                if (_xpoComboBoxVatOnTable != null) _xpoComboBoxVatOnTable.Changed += xpoComboBoxVatDirectSelling_Changed;
            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
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
                requireToChooseVatExemptionReason = Convert.ToBoolean(GlobalFramework.Settings["requireToChooseVatExemptionReason"]);
            }
            catch (Exception)
            {
                _log.Error("Error Missing Config Parameter Key: [requireToChooseVatExemptionReason]");
            }

            try
            {
                //Get VatExemptionReason
                GenericCRUDWidgetXPO genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget("VatExemptionReason") as GenericCRUDWidgetXPO);

                if (
                        (
                            (_xpoComboBoxVatOnTable != null && _xpoComboBoxVatOnTable.Value != null && _xpoComboBoxVatOnTable.Value.Oid == SettingsApp.XpoOidConfigurationVatRateDutyFree)
                            || (_xpoComboBoxVatDirectSelling != null && _xpoComboBoxVatDirectSelling.Value != null && _xpoComboBoxVatDirectSelling.Value.Oid == SettingsApp.XpoOidConfigurationVatRateDutyFree)
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
                _log.Error(ex.Message, ex);
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
    }
}

