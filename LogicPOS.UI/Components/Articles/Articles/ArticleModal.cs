using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.AddArticle;
using LogicPOS.Api.Features.Articles.Classes.GetAllArticleClasses;
using LogicPOS.Api.Features.Articles.Families.GetAllArticleFamilies;
using LogicPOS.Api.Features.Articles.PriceTypes.GetAllPriceTypes;
using LogicPOS.Api.Features.Articles.Subfamilies.GetAllArticleSubfamilies;
using LogicPOS.Api.Features.Articles.Types.GetAllArticleTypes;
using LogicPOS.Api.Features.Articles.UpdateArticle;
using LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups;
using LogicPOS.Api.Features.Customers.DiscountGroups.GetAllDiscountGroups;
using LogicPOS.Api.Features.MeasurementUnits.GetAllMeasurementUnits;
using LogicPOS.Api.Features.SizeUnits.GetAllSizeUnits;
using LogicPOS.Api.Features.VatExemptionReasons.GetAllVatExemptionReasons;
using LogicPOS.Api.Features.VatRates.GetAllVatRate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleModal : EntityEditionModal<Article>
    {
        private string _temporaryButtonImageLocation;

        private IEnumerable<ArticleSubfamily> _subfamilies;
        public ArticleModal(EntityEditionModalMode modalMode, Article entity = null) : base(modalMode, entity)
        {
            UpdateCompositionTabVisibility();
            UpdateValidatableFields();
        }

        private AddArticleCommand CreateAddCommand()
        {
            return new AddArticleCommand
            {
                CodeDealer = _txtCodeDealer.Text,
                Designation = _txtDesignation.Text,
                Button = GetButton(),
                Price1 = _price1.Price,
                Price2 = _price2.Price,
                Price3 = _price3.Price,
                Price4 = _price4.Price,
                Price5 = _price5.Price,
                PriceWithVat = _checkPriceWithVat.Active,
                Discount = decimal.Parse(_txtDiscount.Text),
                DefaultQuantity = uint.Parse(_txtDefaultQuantity.Text),
                MinimumStock = uint.Parse(_txtMinimumStock.Text),
                Tare = decimal.Parse(_txtTare.Text),
                Weight = float.Parse(_txtWeight.Text),
                Barcode = _txtBarcode.Text,
                PVPVariable = _checkPVPVariable.Active,
                Favorite = _checkFavorite.Active,
                UseWeighingBalance = _checkUseWeighingBalance.Active,
                SubfamilyId = _comboSubfamilies.SelectedEntity.Id,
                TypeId = _comboTypes.SelectedEntity.Id,
                ClassId = _comboClasses.SelectedEntity.Id,
                MeasurementUnitId = _comboMeasurementUnits.SelectedEntity.Id,
                SizeUnitId = _comboSizeUnits.SelectedEntity.Id,
                CommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id,
                DiscountGroupId = _comboDiscountGroups.SelectedEntity?.Id,
                VatDirectSellingId = _comboVatDirectSelling.SelectedEntity.Id,
                VatExemptionReasonId = _comboVatExemptionReasons.SelectedEntity?.Id,
                IsComposed = _checkIsComposed.Active,
                UniqueArticles = _checkUniqueArticles.Active,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateArticleCommand CreateUpdateCommand()
        {
            return new UpdateArticleCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewCodeDealer = _txtCodeDealer.Text,
                NewDesignation = _txtDesignation.Text,
                NewButton = GetButton(),
                NewPrice1 = _price1.Price,
                NewPrice2 = _price2.Price,
                NewPrice3 = _price3.Price,
                NewPrice4 = _price4.Price,
                NewPrice5 = _price5.Price,
                NewPriceWithVat = _checkPriceWithVat.Active,
                NewDiscount = decimal.Parse(_txtDiscount.Text),
                NewDefaultQuantity = uint.Parse(_txtDefaultQuantity.Text),
                NewMinimumStock = uint.Parse(_txtMinimumStock.Text),
                NewTare = decimal.Parse(_txtTare.Text),
                NewWeight = float.Parse(_txtWeight.Text),
                NewBarcode = _txtBarcode.Text,
                NewPVPVariable = _checkPVPVariable.Active,
                Favorite = _checkFavorite.Active,
                UseWeighingBalance = _checkUseWeighingBalance.Active,
                NewSubfamilyId = _comboSubfamilies.SelectedEntity.Id,
                NewTypeId = _comboTypes.SelectedEntity.Id,
                NewClassId = _comboClasses.SelectedEntity.Id,
                NewMeasurementUnitId = _comboMeasurementUnits.SelectedEntity.Id,
                NewSizeUnitId = _comboSizeUnits.SelectedEntity.Id,
                NewCommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id ?? Guid.Empty,
                NewDiscountGroupId = _comboDiscountGroups.SelectedEntity?.Id ?? Guid.Empty,
                NewVatDirectSellingId = _comboVatDirectSelling.SelectedEntity.Id,
                NewVatExemptionReasonId = _comboVatExemptionReasons.SelectedEntity?.Id ?? Guid.Empty,
                IsComposed = _checkIsComposed.Active,
                UniqueArticles = _checkUniqueArticles.Active,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void AddEntity() => ExecuteAddCommand(CreateAddCommand());

        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtButtonName.Text = _entity.Button?.Label;
            _imagePicker.SetBase64Image(_entity.Button?.Image, _entity.Button?.ImageExtension);
            _checkIsComposed.Active = _entity.IsComposed;
            _checkFavorite.Active = _entity.Favorite;
            _checkUseWeighingBalance.Active = _entity.UseWeighingBalance;
            _txtNotes.Value.Text = _entity.Notes;
            _checkPVPVariable.Active = _entity.PVPVariable;
            _checkPriceWithVat.Active = _entity.PriceWithVat;
            _txtDiscount.Text = _entity.Discount.ToString();
            _txtBarcode.Text = _entity.Barcode;
            _txtMinimumStock.Text = _entity.MinimumStock.ToString();
            _txtTare.Text = _entity.Tare.ToString();
            _txtWeight.Text = _entity.Weight.ToString();
            _txtDefaultQuantity.Text = _entity.DefaultQuantity.ToString();
            _checkDisabled.Active = _entity.IsDeleted;
            _checkUniqueArticles.Active = _entity.UniqueArticles;
        }

        private void ComboBox_Changed(object sender, EventArgs e)
        {
            UpdateValidatableFields();
        }

        private void UpdateValidatableFields()
        {
            if (_comboVatDirectSelling.SelectedEntity == null)
            {
                return;
            }

            if (_comboVatDirectSelling.SelectedEntity.Value != 0)
            {
                _comboVatExemptionReasons.ComboBox.Sensitive = false;
                ValidatableFields.Remove(_comboVatExemptionReasons);
                _comboVatExemptionReasons.IsRequired = false;
                _comboVatExemptionReasons.UpdateValidationColors();
                return;
            }

            _comboVatExemptionReasons.ComboBox.Sensitive = true;
            ValidatableFields.Add(_comboVatExemptionReasons);
            _comboVatExemptionReasons.IsRequired = true;
            _comboVatExemptionReasons.UpdateValidationColors();
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

        private IEnumerable<ArticleType> GetTypes() => ExecuteGetAllQuery(new GetAllArticleTypesQuery());
        private IEnumerable<ArticleFamily> GetFamilies() => ExecuteGetAllQuery(new GetAllArticleFamiliesQuery());
        private IEnumerable<ArticleSubfamily> GetSubfamilies(Guid? familyId = null)
        {
            if (familyId == null)
            {
                return Enumerable.Empty<ArticleSubfamily>();
            }

            if (_subfamilies == null)
            {
                _subfamilies = ExecuteGetAllQuery(new GetAllArticleSubfamiliesQuery());
            }

            return _subfamilies.Where(s => s.FamilyId == familyId);
        }
        private IEnumerable<DiscountGroup> GetDiscountGroups() => ExecuteGetAllQuery(new GetAllDiscountGroupsQuery());
        private IEnumerable<VatRate> GetVatRates() => ExecuteGetAllQuery(new GetAllVatRatesQuery());
        private IEnumerable<PriceType> GetPriceTypes() => ExecuteGetAllQuery(new GetAllPriceTypesQuery());
        private IEnumerable<CommissionGroup> GetCommissionGroups() => ExecuteGetAllQuery(new GetAllCommissionGroupsQuery());
        private IEnumerable<ArticleClass> GetClasses() => ExecuteGetAllQuery(new GetAllArticleClassesQuery());
        private IEnumerable<MeasurementUnit> GetMeasurementUnits() => ExecuteGetAllQuery(new GetAllMeasurementUnitsQuery());
        private IEnumerable<SizeUnit> GetSizeUnits() => ExecuteGetAllQuery(new GetAllSizeUnitsQuery());
        private IEnumerable<VatExemptionReason> GetVatExemptionReasons() => ExecuteGetAllQuery(new GetAllVatExemptionReasonsQuery());
    }
}
