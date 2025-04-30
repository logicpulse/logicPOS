using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.POS.Devices.Printers.PrinterAssociation;
using LogicPOS.Utility;
using System.Drawing;
using System.Linq;


namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleModal
    {
        public override Size ModalSize => new Size(500, 740);
        public override string ModalTitleResourceName => "window_title_edit_article";

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
            InitializePrinterComboBox();

            _checkUniqueArticles.Sensitive = false;

            AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            _checkIsComposed.Toggled += (sender, e) => UpdateCompositionTabVisibility();
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

            _comboVatDirectSelling.ComboBox.Changed += ComboBox_Changed;
        }

        private void InitializeVatExemptionReasonsComboBox()
        {
            var reasons = GetVatExemptionReasons();
            var labelText = GeneralUtils.GetResourceByName("global_vat_exemption_reason");
            var currentReason = _entity != null ? _entity.VatExemptionReason : null;

            _comboVatExemptionReasons = new EntityComboBox<VatExemptionReason>(labelText,
                                                             reasons,
                                                             currentReason,
                                                             false);
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

        private void InitializePrinterComboBox()
        {
            var printers = GetPrinters();
            var labelText = GeneralUtils.GetResourceByName("global_printers");
            var currentPrinter = _entity != null ? PrinterAssociationService.GetEntityAssociatedPrinterById(_entity.Id) : null;

            _comboPrinters = new EntityComboBox<Printer>(labelText,
                                                         printers,
                                                         currentPrinter,
                                                         false);
        }

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
