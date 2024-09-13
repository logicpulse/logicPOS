using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleModal
    {
        public override Size ModalSize => new Size(500, 740);
        public override string ModalTitleResourceName => "window_title_edit_article";

        #region Components
        private TextBox _txtOrder = TextBoxes.CreateOrderField();
        private TextBox _txtCode = TextBoxes.CreateCodeField();
        private TextBox _txtCodeDealer = new TextBox("global_record_code_dealer");
        private TextBox _txtDesignation = TextBoxes.CreateDesignationField();
        private TextBox _txtButtonName = new TextBox("global_button_name");
        private ImagePicker _imagePicker = new ImagePicker(GeneralUtils.GetResourceByName("global_button_image"));
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        private EntityComboBox<CommissionGroup> _comboCommissionGroups;
        private EntityComboBox<ArticleFamily> _comboFamilies;
        private EntityComboBox<ArticleSubfamily> _comboSubfamilies;
        private EntityComboBox<ArticleType> _comboTypes;
        private EntityComboBox<ArticleClass> _comboClasses;
        private EntityComboBox<DiscountGroup> _comboDiscountGroups;
        private EntityComboBox<VatRate> _comboVatDirectSelling;
        private EntityComboBox<MeasurementUnit> _comboMeasurementUnits;
        private EntityComboBox<SizeUnit> _comboSizeUnits;
        private EntityComboBox<VatExemptionReason> _comboVatExemptionReasons;
        private CheckButton _checkIsComposed = new CheckButton(GeneralUtils.GetResourceByName("global_composite_article"));
        private CheckButton _checkUniqueArticles = new CheckButton(GeneralUtils.GetResourceByName("global_unique_articles"));
        private CheckButton _checkFavorite = new CheckButton(GeneralUtils.GetResourceByName("global_favorite"));
        private CheckButton _checkUseWeighingBalance = new CheckButton(GeneralUtils.GetResourceByName("global_use_weighing_balance"));
        private CheckButton _checkPriceWithVat = new CheckButton(GeneralUtils.GetResourceByName("global_price_with_vat"));
        private CheckButton _checkPVPVariable = new CheckButton(GeneralUtils.GetResourceByName("global_variable_price"));
        private TextBox _txtDiscount = new TextBox("global_discount",true,true,RegularExpressions.Money);
        private TextBox _txtDefaultQuantity = new TextBox("global_article_default_quantity",true,true,RegularExpressions.IntegerNumber);
        private TextBox _txtTotalStock = new TextBox("global_total_stock") { Text = "0" };
        private TextBox _txtMinimumStock = new TextBox("global_minimum_stock",true,true,RegularExpressions.IntegerNumber);
        private TextBox _txtTare = new TextBox("global_tare", true, true, RegularExpressions.DecimalNumber) { Text = "0" };
        private TextBox _txtWeight = new TextBox("global_weight", true, true, RegularExpressions.DecimalNumber) { Text= "0"};
        private TextBox _txtBarcode = new TextBox("global_barcode", false, true, RegularExpressions.IntegerNumber);
        private ArticlePriceField _price1;
        private ArticlePriceField _price2;
        private ArticlePriceField _price3;
        private ArticlePriceField _price4;
        private ArticlePriceField _price5;
        #endregion

        protected override void BeforeDesign()
        {
            InitializeCommissionGroupsComboBox();
            InitializeFamiliesComboBox();
            InitializeSubfamiliesComboBox();
            InitializeDiscountGroupsComboBox();
            InitializeVatDirectSellingComboBox();
            InitializeVatExemptionReasonsComboBox();
            InitializeArticlePriceFields();
            InitializeMeasurementUnitsComboBox();
            InitializeSizeUnitsComboBox();
            InitializeArticleClassesComboBox();
            InitializeArticleTypesComboBox();

            _txtTotalStock.Entry.IsEditable = false;
            _checkUniqueArticles.Sensitive = false;
        }

        private void InitializeArticlePriceFields()
        {
            var pricetypes = GetPriceTypes().OrderBy(price => price.EnumValue).Take(5).ToArray();

            _price1 = new ArticlePriceField(pricetypes[0],_entity?.Price1);
            _price2 = new ArticlePriceField(pricetypes[1],_entity?.Price2);
            _price3 = new ArticlePriceField(pricetypes[2],_entity?.Price3);
            _price4 = new ArticlePriceField(pricetypes[3], _entity?.Price4);
            _price5 = new ArticlePriceField(pricetypes[4], _entity?.Price5);
        }

        private void InitializeCommissionGroupsComboBox()
        {
            var groups = GetCommissionGroups();
            var labelText = GeneralUtils.GetResourceByName("global_commission_group");
            var currentCommissionGroup = _entity != null ? _entity.CommissionGroup : null;

            _comboCommissionGroups = new EntityComboBox<CommissionGroup>(labelText,
                                                             groups,
                                                             currentCommissionGroup);
        }

        private void InitializeFamiliesComboBox()
        {
            var families = GetFamilies();
            var labelText = GeneralUtils.GetResourceByName("global_families");
            var currentFamily = _entity != null ? _entity.Subfamily.Family : null;

            _comboFamilies = new EntityComboBox<ArticleFamily>(labelText,
                                                             families,
                                                             currentFamily,
                                                             true);

            _comboFamilies.ComboBox.Changed += (sender, e) =>
            {
               _comboSubfamilies.Entities = GetSubfamilies(_comboFamilies.SelectedEntity?.Id);
               _comboSubfamilies.ReLoad();
            };
        }

        private void InitializeSubfamiliesComboBox()
        {
            var labelText = GeneralUtils.GetResourceByName("global_article_subfamily");
            var currentSubfamily = _entity != null ? _entity.Subfamily : null;
            var subfamilies = Enumerable.Empty<ArticleSubfamily>();

            if (_entity != null)
            {
                subfamilies = GetSubfamilies(currentSubfamily.FamilyId);
            }

            _comboSubfamilies = new EntityComboBox<ArticleSubfamily>(labelText,
                                                             subfamilies,
                                                             currentSubfamily,
                                                             true);
        }

        private void InitializeDiscountGroupsComboBox()
        {
            var groups = GetDiscountGroups();
            var labelText = GeneralUtils.GetResourceByName("global_discount_group");
            var currentDiscountGroup = _entity != null ? _entity.DiscountGroup : null;

            _comboDiscountGroups = new EntityComboBox<DiscountGroup>(labelText,
                                                             groups,
                                                             currentDiscountGroup);
        }

        private void InitializeVatDirectSellingComboBox()
        {
            var vatRates = GetVatRates();
            var labelText = GeneralUtils.GetResourceByName("global_vat_direct_selling");
            var currentVatRate = _entity != null ? _entity.VatDirectSelling : null;

            _comboVatDirectSelling = new EntityComboBox<VatRate>(labelText,
                                                             vatRates,
                                                             currentVatRate,
                                                             true);
        }

        private void InitializeVatExemptionReasonsComboBox()
        {
            var reasons = GetVatExemptionReasons();
            var labelText = GeneralUtils.GetResourceByName("global_vat_exemption_reason");
            var currentReason = _entity != null ? _entity.VatExemptionReason : null;

            _comboVatExemptionReasons = new EntityComboBox<VatExemptionReason>(labelText,
                                                             reasons,
                                                             currentReason,
                                                             true);
        }

        private void InitializeArticleTypesComboBox()
        {
            var types = GetTypes();
            var labelText = GeneralUtils.GetResourceByName("global_article_type");
            var currentType = _entity != null ? _entity.Type : null;

            _comboTypes = new EntityComboBox<ArticleType>(labelText,
                                                             types,
                                                             currentType,
                                                             true);
        }

        private void InitializeArticleClassesComboBox()
        {
            var classes = GetClasses();
            var labelText = GeneralUtils.GetResourceByName("global_article_class");
            var currentClass = _entity != null ? _entity.Class : null;

            _comboClasses = new EntityComboBox<ArticleClass>(labelText,
                                                             classes,
                                                             currentClass,
                                                             true);
        }

        private void InitializeMeasurementUnitsComboBox()
        {
            var units = GetMeasurementUnits();
            var labelText = GeneralUtils.GetResourceByName("global_unit_measure");
            var currentUnit = _entity != null ? _entity.MeasurementUnit : null;

            _comboMeasurementUnits = new EntityComboBox<MeasurementUnit>(labelText,
                                                             units,
                                                             currentUnit,
                                                             true);
        }

        private void InitializeSizeUnitsComboBox()
        {
            var units = GetSizeUnits();
            var labelText = GeneralUtils.GetResourceByName("global_unit_size");
            var currentUnit = _entity != null ? _entity.SizeUnit : null;

            _comboSizeUnits = new EntityComboBox<SizeUnit>(labelText,
                                                             units,
                                                             currentUnit,
                                                             true);
        }

        private Api.ValueObjects.Button GetButton()
        {
            return new Api.ValueObjects.Button
            {
                ButtonLabel = _txtButtonName.Text,
                ButtonImage = _imagePicker.FileChooserButton.Filename
            };
        }

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtButtonName.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
            SensitiveFields.Add(_imagePicker.FileChooserButton);
            SensitiveFields.Add(_comboCommissionGroups.ComboBox);
            SensitiveFields.Add(_comboFamilies.ComboBox);
            SensitiveFields.Add(_comboDiscountGroups.ComboBox);
            SensitiveFields.Add(_comboVatDirectSelling.ComboBox);
            SensitiveFields.Add(_comboVatExemptionReasons.ComboBox);
            SensitiveFields.Add(_comboMeasurementUnits.ComboBox);
            SensitiveFields.Add(_comboSizeUnits.ComboBox);
            SensitiveFields.Add(_comboTypes.ComboBox);
            SensitiveFields.Add(_comboSubfamilies.ComboBox);
            SensitiveFields.Add(_comboClasses.ComboBox);
            SensitiveFields.Add(_checkIsComposed);
            SensitiveFields.Add(_checkUniqueArticles);
            SensitiveFields.Add(_checkFavorite);
            SensitiveFields.Add(_checkUseWeighingBalance);
            SensitiveFields.Add(_checkPriceWithVat);
            SensitiveFields.Add(_checkPVPVariable);
            SensitiveFields.Add(_txtDiscount.Entry);
            SensitiveFields.Add(_txtDefaultQuantity.Entry);
            SensitiveFields.Add(_txtTotalStock.Entry);
            SensitiveFields.Add(_txtMinimumStock.Entry);
            SensitiveFields.Add(_txtTare.Entry);
            SensitiveFields.Add(_txtWeight.Entry);
            SensitiveFields.Add(_txtBarcode.Entry);
            SensitiveFields.Add(_price1.Component);
            SensitiveFields.Add(_price2.Component);
            SensitiveFields.Add(_price3.Component);
            SensitiveFields.Add(_price4.Component);
            SensitiveFields.Add(_price5.Component);
            SensitiveFields.Add(_txtCodeDealer.Entry);
        }

        protected override void AddValidatableFields()
        {
            if (_modalMode == EntityEditionModalMode.Update)
            {
                ValidatableFields.Add(_txtOrder);
                ValidatableFields.Add(_txtCode);
            }

            ValidatableFields.Add(_txtDesignation);
            ValidatableFields.Add(_txtDiscount);
            ValidatableFields.Add(_txtDefaultQuantity);
            ValidatableFields.Add(_txtMinimumStock);
            ValidatableFields.Add(_txtTare);
            ValidatableFields.Add(_txtWeight);
            ValidatableFields.Add(_txtBarcode);
            ValidatableFields.Add(_price1);
            ValidatableFields.Add(_price2);
            ValidatableFields.Add(_price3);
            ValidatableFields.Add(_price4);
            ValidatableFields.Add(_price5);        
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateDetailsTab(), GeneralUtils.GetResourceByName("global_record_main_detail"));
            yield return (CreateFinanceDetailsTab(), GeneralUtils.GetResourceByName("dialog_edit_article_tab2_label"));
            yield return (CreateOtherDetailsTab(), GeneralUtils.GetResourceByName("dialog_edit_article_tab3_label"));
            yield return (CreateArticleCompositionTab(), GeneralUtils.GetResourceByName("dialog_edit_article_tab4_label1"));
            yield return (CreateNotesTab(), GeneralUtils.GetResourceByName("global_notes"));
        }

        private VBox CreateDetailsTab()
        {
            var detailsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_txtOrder.Component, false, false, 0);
                detailsTab.PackStart(_txtCode.Component, false, false, 0);
            }

            detailsTab.PackStart(_txtCodeDealer.Component, false, false, 0);
            detailsTab.PackStart(_txtDesignation.Component, false, false, 0);
            detailsTab.PackStart(_txtButtonName.Component, false, false, 0);
            detailsTab.PackStart(_comboFamilies.Component, false, false, 0);
            detailsTab.PackStart(_comboSubfamilies.Component, false, false, 0);
            detailsTab.PackStart(_comboTypes.Component, false, false, 0);
            detailsTab.PackStart(_imagePicker.Component, false, false, 0);
            detailsTab.PackStart(_checkIsComposed, false, false, 0);
            detailsTab.PackStart(_checkUniqueArticles, false, false, 0);
            detailsTab.PackStart(_checkFavorite, false, false, 0);
            detailsTab.PackStart(_checkUseWeighingBalance, false, false, 0);

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_checkDisabled, false, false, 0);
            }

            return detailsTab;
        }

        private VBox CreateArticleCompositionTab()
        {
            var articlesTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
            articlesTab.PackStart( new ArticleField().Component, false, false, 0);
            articlesTab.PackStart(new ArticleField().Component, false, false, 0);
            articlesTab.PackStart(new ArticleField().Component, false, false, 0);
            articlesTab.PackStart(new ArticleField().Component, false, false, 0);
            return articlesTab;
        }

        private VBox CreateFinanceDetailsTab()
        {
            var financeDetailsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            financeDetailsTab.PackStart(CreatePricesArea(), false, false, 0);
            financeDetailsTab.PackStart(_checkPVPVariable, false, false, 0);
            financeDetailsTab.PackStart(_checkPriceWithVat, false, false, 0);
            financeDetailsTab.PackStart(_txtDiscount.Component, false, false, 0);
            financeDetailsTab.PackStart(_comboClasses.Component, false, false, 0);
            financeDetailsTab.PackStart(_comboVatDirectSelling.Component, false, false, 0);
            financeDetailsTab.PackStart(_comboVatExemptionReasons.Component, false, false, 0);

            return financeDetailsTab;
        }

        private VBox CreateOtherDetailsTab()
        {
            var otherDetailsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
            
            otherDetailsTab.PackStart(_txtBarcode.Component, false, false, 0);
            otherDetailsTab.PackStart(_txtTotalStock.Component, false, false, 0);
            otherDetailsTab.PackStart(_txtMinimumStock.Component, false, false, 0);
            otherDetailsTab.PackStart(_txtTare.Component, false, false, 0);
            otherDetailsTab.PackStart(_txtWeight.Component, false, false, 0);
            otherDetailsTab.PackStart(_txtDefaultQuantity.Component, false, false, 0);
            otherDetailsTab.PackStart(_comboMeasurementUnits.Component, false, false, 0);
            otherDetailsTab.PackStart(_comboSizeUnits.Component, false, false, 0);
            otherDetailsTab.PackStart(_comboCommissionGroups.Component, false, false, 0);
            otherDetailsTab.PackStart(_comboDiscountGroups.Component, false, false, 0);

            return otherDetailsTab;
        }

        private VBox CreatePricesArea()
        {
            int[] columnsWidths = new int[] { 100, 90, 90, 160 };
            var vbox = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            Label labelEmpty = new Label(string.Empty) { WidthRequest = columnsWidths[0] };
            Label labelNormal = new Label(GeneralUtils.GetResourceByName("article_normal_price")) { WidthRequest = columnsWidths[1] };
            Label labelPromotion = new Label(GeneralUtils.GetResourceByName("article_promotion_price")) { WidthRequest = columnsWidths[2] };
            Label labelUsePromotion = new Label(GeneralUtils.GetResourceByName("article_use_promotion_price")) { WidthRequest = columnsWidths[3] };
            labelNormal.SetAlignment(0.0F, 0.5F);
            labelPromotion.SetAlignment(0.0F, 0.5F);
            labelUsePromotion.SetAlignment(0.0F, 0.5F);

            //Header
            HBox header = new HBox(false, _boxSpacing);
            header.PackStart(labelEmpty, true, true, 0);
            header.PackStart(labelNormal, false, false, 0);
            header.PackStart(labelPromotion, false, false, 0);
            header.PackStart(labelUsePromotion, false, false, 0);
            vbox.PackStart(header, false, false, 0);

            //Prices
            vbox.PackStart(_price1.Component, false, false, 0);
            vbox.PackStart(_price2.Component, false, false, 0);
            vbox.PackStart(_price3.Component, false, false, 0);
            vbox.PackStart(_price4.Component, false, false, 0);
            vbox.PackStart(_price5.Component, false, false, 0);
              
            return vbox;
        }
    }
}
