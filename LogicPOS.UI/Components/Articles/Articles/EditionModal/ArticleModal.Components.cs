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


        private Api.ValueObjects.Button GetButton()
        {
            return new Api.ValueObjects.Button
            {
                Label = _txtButtonName.Text,
                Image = _imagePicker.GetBase64Image(),
                ImageExtension = _imagePicker.GetImageExtension()
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
            SensitiveFields.Add(_txtMinimumStock.Entry);
            SensitiveFields.Add(_txtTare.Entry);
            SensitiveFields.Add(_txtWeight.Entry);
            SensitiveFields.Add(_txtBarcode.Entry);
            foreach (var priceField in _prices)
            {
                SensitiveFields.Add(priceField.Component);
            }

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
            foreach (var priceField in _prices)
            {
                ValidatableFields.Add(priceField);
            }
        }

        private void UpdateCompositionTabVisibility()
        {
            _compositionTab.Visible = _checkIsComposed.Active;

            if (_checkIsComposed.Active == false)
            {
                ValidatableFields.Remove(_addArticlesBox);
            }
            else
            {
                ValidatableFields.Add(_addArticlesBox);
            }
        }

    }
}
