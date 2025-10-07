using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.POS.Devices.Printers.PrinterAssociation;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleModal
    {
        private void InitializeFields()
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
        }


        private void InitializeArticlePriceFields()
        {
            _prices = new List<ArticlePriceField>();
            var pricetypes = GetPriceTypes().OrderByDescending(price => price.EnumValue).Take(5).OrderBy(x => x.Code).ToArray();



            switch (pricetypes.Length)
            {
                case 1:
                    _prices.Add(new ArticlePriceField(pricetypes[0], _entity?.Price1));

                    break;
                case 2:
                    _prices.Add(new ArticlePriceField(pricetypes[0], _entity?.Price1));
                    _prices.Add(new ArticlePriceField(pricetypes[1], _entity?.Price2));

                    break;
                case 3:
                    _prices.Add(new ArticlePriceField(pricetypes[0], _entity?.Price1));
                    _prices.Add(new ArticlePriceField(pricetypes[1], _entity?.Price2));
                    _prices.Add(new ArticlePriceField(pricetypes[2], _entity?.Price3));

                    break;
                case 4:
                    _prices.Add(new ArticlePriceField(pricetypes[0], _entity?.Price1));
                    _prices.Add(new ArticlePriceField(pricetypes[1], _entity?.Price2));
                    _prices.Add(new ArticlePriceField(pricetypes[2], _entity?.Price3));
                    _prices.Add(new ArticlePriceField(pricetypes[3], _entity?.Price4));

                    break;
                case 5:
                    _prices.Add(new ArticlePriceField(pricetypes[0], _entity?.Price1));
                    _prices.Add(new ArticlePriceField(pricetypes[1], _entity?.Price2));
                    _prices.Add(new ArticlePriceField(pricetypes[2], _entity?.Price3));
                    _prices.Add(new ArticlePriceField(pricetypes[3], _entity?.Price4));
                    _prices.Add(new ArticlePriceField(pricetypes[4], _entity?.Price5));
                    break;
            }
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

            _comboVatDirectSelling.ComboBox.Changed += ComboBox_VatDirectSelling_Changed;
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
            var currentClass = _entity != null ? classes.FirstOrDefault(c => c.Id == _entity.ClassId) : null;
           
            if (_modalMode == EntityEditionModalMode.Insert && classes.Any())
            {
                currentClass = classes.FirstOrDefault(c => c.Acronym == "P");
            }

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
            if (_modalMode == EntityEditionModalMode.Insert && units.Any())
            {
                currentUnit = units.First();
            }

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
            var currentPrinter = _entity != null ? PrinterAssociationService.GetPrinter(_entity.Id) : null;

            _comboPrinters = new EntityComboBox<Printer>(labelText,
                                                         printers,
                                                         currentPrinter,
                                                         false);
        }

    }
}
