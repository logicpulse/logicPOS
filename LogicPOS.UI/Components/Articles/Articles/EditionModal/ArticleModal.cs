using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.AddArticle;
using LogicPOS.Api.Features.Articles.AddArticleChildren;
using LogicPOS.Api.Features.Articles.Classes.GetAllArticleClasses;
using LogicPOS.Api.Features.Articles.Families.GetAllArticleFamilies;
using LogicPOS.Api.Features.Articles.GetArticleChildren;
using LogicPOS.Api.Features.Articles.PriceTypes.GetAllPriceTypes;
using LogicPOS.Api.Features.Articles.Subfamilies.GetAllArticleSubfamilies;
using LogicPOS.Api.Features.Articles.Types.GetAllArticleTypes;
using LogicPOS.Api.Features.Articles.UpdateArticle;
using LogicPOS.Api.Features.Articles.UpdateArticleChildren;
using LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups;
using LogicPOS.Api.Features.Customers.DiscountGroups.GetAllDiscountGroups;
using LogicPOS.Api.Features.MeasurementUnits.GetAllMeasurementUnits;
using LogicPOS.Api.Features.Printers.GetAllPrinters;
using LogicPOS.Api.Features.SizeUnits.GetAllSizeUnits;
using LogicPOS.Api.Features.VatExemptionReasons.GetAllVatExemptionReasons;
using LogicPOS.Api.Features.VatRates.GetAllVatRate;
using LogicPOS.UI.Components.POS.Devices.Printers.PrinterAssociation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleModal : EntityEditionModal<Article>
    {

        private IEnumerable<ArticleSubfamily> _subfamilies;

        public ArticleModal(EntityEditionModalMode modalMode, Article entity = null) : base(modalMode, entity)
        {
            UpdateCompositionTabVisibility();
            UpdateValidatableFields();
        }

        private AddArticleCommand CreateAddCommand()
        {
            var addArticleCommand = new AddArticleCommand();
            
                addArticleCommand.CodeDealer = _txtCodeDealer.Text;
                addArticleCommand.Designation = _txtDesignation.Text;
                addArticleCommand.Button = GetButton();
                addArticleCommand.PriceWithVat = _checkPriceWithVat.Active;
                addArticleCommand.Discount = decimal.Parse(_txtDiscount.Text);
                addArticleCommand.DefaultQuantity = decimal.Parse(_txtDefaultQuantity.Text);
                addArticleCommand.MinimumStock = decimal.Parse(_txtMinimumStock.Text);
                addArticleCommand.Tare = decimal.Parse(_txtTare.Text);
                addArticleCommand.Weight = float.Parse(_txtWeight.Text);
                addArticleCommand.Barcode = _txtBarcode.Text;
                addArticleCommand.PVPVariable = _checkPVPVariable.Active;
                addArticleCommand.Favorite = _checkFavorite.Active;
                addArticleCommand.UseWeighingBalance = _checkUseWeighingBalance.Active;
                addArticleCommand.SubfamilyId = _comboSubfamilies.SelectedEntity.Id;
                addArticleCommand.TypeId = _comboTypes.SelectedEntity.Id;
                addArticleCommand.ClassId = _comboClasses.SelectedEntity.Id;
                addArticleCommand.MeasurementUnitId = _comboMeasurementUnits.SelectedEntity.Id;
                addArticleCommand.SizeUnitId = _comboSizeUnits.SelectedEntity.Id;
                addArticleCommand.CommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id;
                addArticleCommand.DiscountGroupId = _comboDiscountGroups.SelectedEntity?.Id;
                addArticleCommand.VatDirectSellingId = _comboVatDirectSelling.SelectedEntity.Id;
                addArticleCommand.VatExemptionReasonId = _comboVatExemptionReasons.SelectedEntity?.Id;
                addArticleCommand.IsComposed = _checkIsComposed.Active;
                addArticleCommand.UniqueArticles = _checkUniqueArticles.Active;
                addArticleCommand.Notes = _txtNotes.Value.Text;


            switch (_prices.Count())
            {
                case 1:
                    addArticleCommand.Price1 = _prices[0].Price;

                    break;
                case 2:
                    addArticleCommand.Price1 = _prices[0].Price;
                    addArticleCommand.Price2 = _prices[1].Price;

                    break;
                case 3:
                    addArticleCommand.Price1 = _prices[0].Price;
                    addArticleCommand.Price2 = _prices[1].Price;
                    addArticleCommand.Price3 = _prices[2].Price;
                    break;
                case 4:
                    addArticleCommand.Price1 = _prices[0].Price;
                    addArticleCommand.Price2 = _prices[1].Price;
                    addArticleCommand.Price3 = _prices[2].Price;
                    addArticleCommand.Price4 = _prices[3].Price;

                    break;
                case 5:
                    addArticleCommand.Price1 = _prices[0].Price;
                    addArticleCommand.Price2 = _prices[1].Price;
                    addArticleCommand.Price3 = _prices[2].Price;
                    addArticleCommand.Price4 = _prices[3].Price;
                    addArticleCommand.Price5 = _prices[4].Price;
                    break;
            }

            return addArticleCommand;
        }

        private UpdateArticleCommand CreateUpdateCommand()
        {
            if (_comboPrinters.SelectedEntity != null)
            {
                PrinterAssociationService.Set(_entity.Id, _comboPrinters.SelectedEntity.Id);
            }

            var updateCommand = new UpdateArticleCommand();

            updateCommand.Id = _entity.Id;
            updateCommand.NewOrder = uint.Parse(_txtOrder.Text);
            updateCommand.NewCode = _txtCode.Text;
            updateCommand.NewCodeDealer = _txtCodeDealer.Text;
            updateCommand.NewDesignation = _txtDesignation.Text;
            updateCommand.NewButton = GetButton();
            updateCommand.NewPriceWithVat = _checkPriceWithVat.Active;
            updateCommand.NewDiscount = decimal.Parse(_txtDiscount.Text);
            updateCommand.NewDefaultQuantity = decimal.Parse(_txtDefaultQuantity.Text);
            updateCommand.NewMinimumStock = decimal.Parse(_txtMinimumStock.Text);
            updateCommand.NewTare = decimal.Parse(_txtTare.Text);
            updateCommand.NewWeight = float.Parse(_txtWeight.Text);
            updateCommand.NewBarcode = _txtBarcode.Text;
            updateCommand.NewPVPVariable = _checkPVPVariable.Active;
            updateCommand.Favorite = _checkFavorite.Active;
            updateCommand.UseWeighingBalance = _checkUseWeighingBalance.Active;
            updateCommand.NewSubfamilyId = _comboSubfamilies.SelectedEntity.Id;
            updateCommand.NewTypeId = _comboTypes.SelectedEntity.Id;
            updateCommand.NewClassId = _comboClasses.SelectedEntity.Id;
            updateCommand.NewMeasurementUnitId = _comboMeasurementUnits.SelectedEntity.Id;
            updateCommand.NewSizeUnitId = _comboSizeUnits.SelectedEntity.Id;
            updateCommand.NewCommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id ?? Guid.Empty;
            updateCommand.NewDiscountGroupId = _comboDiscountGroups.SelectedEntity?.Id ?? Guid.Empty;
            updateCommand.NewVatDirectSellingId = _comboVatDirectSelling.SelectedEntity.Id;
            updateCommand.NewVatExemptionReasonId = _comboVatExemptionReasons.SelectedEntity?.Id ?? Guid.Empty;
            updateCommand.IsComposed = _checkIsComposed.Active;
            updateCommand.UniqueArticles = _checkUniqueArticles.Active;
            updateCommand.NewNotes = _txtNotes.Value.Text;
            updateCommand.IsDeleted = _checkDisabled.Active;


            switch (_prices.Count())
            {
                case 1:
                    updateCommand.NewPrice1 = _prices[0].Price;

                    break;
                case 2:
                    updateCommand.NewPrice1 = _prices[0].Price;
                    updateCommand.NewPrice2 = _prices[1].Price;

                    break;
                case 3:
                    updateCommand.NewPrice1 = _prices[0].Price;
                    updateCommand.NewPrice2 = _prices[1].Price;
                    updateCommand.NewPrice3 = _prices[2].Price;
                    break;
                case 4:
                    updateCommand.NewPrice1 = _prices[0].Price;
                    updateCommand.NewPrice2 = _prices[1].Price;
                    updateCommand.NewPrice3 = _prices[2].Price;
                    updateCommand.NewPrice4 = _prices[3].Price;

                    break;
                case 5:
                    updateCommand.NewPrice1 = _prices[0].Price;
                    updateCommand.NewPrice2 = _prices[1].Price;
                    updateCommand.NewPrice3 = _prices[2].Price;
                    updateCommand.NewPrice4 = _prices[3].Price;
                    updateCommand.NewPrice5 = _prices[4].Price;
                    break;
            }

            return updateCommand;
        }

        protected override void AddEntity()
        {
            var result = ExecuteAddCommand(CreateAddCommand());

            if (result.IsError || _checkIsComposed.Active == false)
            {
                return;
            }

            var addChildrenCommand = new AddArticleChildrenCommand
            {
                Id = result.Value,
                Children = _addArticlesBox.GetArticleChildren()
            };

            ExecuteAddCommand(addChildrenCommand);
        }

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

            if (_entity.IsComposed)
            {
                var children = ExecuteGetEntitiesQuery(new GetArticleChildrenQuery(_entity.Id));
                _addArticlesBox.AddArticleChildren(children);
            }
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

        protected override void UpdateEntity()
        {
            var result = ExecuteUpdateCommand(CreateUpdateCommand());

            if (result.IsError || _checkIsComposed.Active == false)
            {
                return;
            }

            var updateChildrenCommand = new UpdateArticleChildrenCommand
            {
                Id = _entity.Id,
                Children = _addArticlesBox.GetArticleChildren()
            };

            ExecuteUpdateCommand(updateChildrenCommand);
        }

        private IEnumerable<ArticleType> GetTypes() => ExecuteGetEntitiesQuery(new GetAllArticleTypesQuery());
        private IEnumerable<ArticleFamily> GetFamilies() => ExecuteGetEntitiesQuery(new GetAllArticleFamiliesQuery());
        private IEnumerable<ArticleSubfamily> GetSubfamilies(Guid? familyId = null)
        {
            if (familyId == null)
            {
                return Enumerable.Empty<ArticleSubfamily>();
            }

            if (_subfamilies == null)
            {
                _subfamilies = ExecuteGetEntitiesQuery(new GetAllArticleSubfamiliesQuery());
            }

            return _subfamilies.Where(s => s.FamilyId == familyId);
        }
        private IEnumerable<DiscountGroup> GetDiscountGroups() => ExecuteGetEntitiesQuery(new GetAllDiscountGroupsQuery());
        private IEnumerable<VatRate> GetVatRates() => ExecuteGetEntitiesQuery(new GetAllVatRatesQuery());
        private IEnumerable<PriceType> GetPriceTypes() => ExecuteGetEntitiesQuery(new GetAllPriceTypesQuery());
        private IEnumerable<CommissionGroup> GetCommissionGroups() => ExecuteGetEntitiesQuery(new GetAllCommissionGroupsQuery());
        private IEnumerable<ArticleClass> GetClasses() => ExecuteGetEntitiesQuery(new GetAllArticleClassesQuery());
        private IEnumerable<MeasurementUnit> GetMeasurementUnits() => ExecuteGetEntitiesQuery(new GetAllMeasurementUnitsQuery());
        private IEnumerable<SizeUnit> GetSizeUnits() => ExecuteGetEntitiesQuery(new GetAllSizeUnitsQuery());
        private IEnumerable<Printer> GetPrinters() => ExecuteGetEntitiesQuery(new GetAllPrintersQuery());
        private IEnumerable<VatExemptionReason> GetVatExemptionReasons() => ExecuteGetEntitiesQuery(new GetAllVatExemptionReasonsQuery());
    }
}
