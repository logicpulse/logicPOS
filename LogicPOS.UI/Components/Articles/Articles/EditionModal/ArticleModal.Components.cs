using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System.Collections.Generic;
using Printer = LogicPOS.Api.Entities.Printer;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleModal
    {
        #region Components
        private readonly TextBox _txtOrder = TextBox.CreateOrderField();
        private readonly TextBox _txtCode = TextBox.CreateCodeField();
        private readonly TextBox _txtCodeDealer = TextBox.Simple("global_record_code_dealer");
        private readonly TextBox _txtDesignation = TextBox.CreateDesignationField();
        private readonly TextBox _txtButtonName = TextBox.Simple("global_button_name");
        private readonly ImagePicker _imagePicker = new ImagePicker(GeneralUtils.GetResourceByName("global_button_image"));
        private readonly CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        private EntityComboBox<CommissionGroup> _comboCommissionGroups;
        private EntityComboBox<ArticleFamily> _comboFamilies;
        private EntityComboBox<ArticleSubfamily> _comboSubfamilies;
        private EntityComboBox<ArticleType> _comboTypes;
        private EntityComboBox<ArticleClass> _comboClasses;
        private EntityComboBox<DiscountGroup> _comboDiscountGroups;
        private EntityComboBox<VatRate> _comboVatDirectSelling;
        private EntityComboBox<MeasurementUnit> _comboMeasurementUnits;

        private EntityComboBox<Printer> _comboPrinters;
        private EntityComboBox<SizeUnit> _comboSizeUnits;
        private EntityComboBox<VatExemptionReason> _comboVatExemptionReasons;
        private readonly CheckButton _checkIsComposed = new CheckButton(GeneralUtils.GetResourceByName("global_composite_article"));
        private readonly CheckButton _checkUniqueArticles = new CheckButton(GeneralUtils.GetResourceByName("global_unique_articles"));
        private readonly CheckButton _checkFavorite = new CheckButton(GeneralUtils.GetResourceByName("global_favorite"));
        private readonly CheckButton _checkUseWeighingBalance = new CheckButton(GeneralUtils.GetResourceByName("global_use_weighing_balance"));
        private readonly CheckButton _checkPriceWithVat = new CheckButton(GeneralUtils.GetResourceByName("global_price_with_vat"));
        private readonly CheckButton _checkPVPVariable = new CheckButton(GeneralUtils.GetResourceByName("global_variable_price"));
        private readonly TextBox _txtDiscount = TextBox.Simple("global_discount", true, true, RegularExpressions.Money);
        private readonly TextBox _txtDefaultQuantity = TextBox.Simple("global_article_default_quantity", true, true, RegularExpressions.DecimalNumber);
        private readonly TextBox _txtMinimumStock = TextBox.Simple("global_minimum_stock", true, true, RegularExpressions.DecimalNumber);
        private readonly TextBox _txtTare = TextBox.Simple("global_tare", true, true, RegularExpressions.DecimalNumber).WithText("0");
        private readonly TextBox _txtWeight = TextBox.Simple("global_weight", true, true, RegularExpressions.DecimalNumber).WithText("0");
        private readonly TextBox _txtBarcode = TextBox.Simple("global_barcode", false, true, RegularExpressions.IntegerNumber);
        private List<ArticlePriceField> _prices;
        private readonly ArticleFieldsContainer _addArticlesBox = new ArticleFieldsContainer();
        private VBox _compositionTab;
        #endregion

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateDetailsTab(), GeneralUtils.GetResourceByName("global_record_main_detail"));
            yield return (CreateFinanceDetailsTab(), GeneralUtils.GetResourceByName("dialog_edit_article_tab2_label"));
            yield return (CreateOtherDetailsTab(), GeneralUtils.GetResourceByName("dialog_edit_article_tab3_label"));
            yield return (CreateCompositionTab(), GeneralUtils.GetResourceByName("dialog_edit_article_tab4_label1"));
            yield return (CreateNotesTab(), GeneralUtils.GetResourceByName("global_notes"));

            if (_entity != null)
            {
                yield return (CreateUniqueArticlesTab(), GeneralUtils.GetResourceByName("global_serial_number"));
            }

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

        private VBox CreateCompositionTab()
        {
            _compositionTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
            _compositionTab.PackStart(_addArticlesBox.Component, true, true, 0);
            return _compositionTab;
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
            otherDetailsTab.PackStart(_txtMinimumStock.Component, false, false, 0);
            otherDetailsTab.PackStart(_txtTare.Component, false, false, 0);
            otherDetailsTab.PackStart(_txtWeight.Component, false, false, 0);
            otherDetailsTab.PackStart(_txtDefaultQuantity.Component, false, false, 0);
            otherDetailsTab.PackStart(_comboMeasurementUnits.Component, false, false, 0);
            otherDetailsTab.PackStart(_comboSizeUnits.Component, false, false, 0);
            otherDetailsTab.PackStart(_comboCommissionGroups.Component, false, false, 0);
            otherDetailsTab.PackStart(_comboDiscountGroups.Component, false, false, 0);
            otherDetailsTab.PackStart(_comboPrinters.Component, false, false, 0);

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
            foreach(var price in _prices)
            {
                vbox.PackStart(price.Component, false, false, 0);
            }

            return vbox;
        }

        private VBox CreateUniqueArticlesTab()
        {
            var vbox = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
            vbox.PackStart(new UniqueArticleFieldsContainer(_entity.Id).Component, true, true, 0);
            return vbox;
        }
    }
}
