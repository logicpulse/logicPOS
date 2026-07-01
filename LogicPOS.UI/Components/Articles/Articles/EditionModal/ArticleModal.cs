using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.AddArticle;
using LogicPOS.Api.Features.Articles.AddArticleChildren;
using LogicPOS.Api.Features.Articles.Families.GetAllArticleFamilies;
using LogicPOS.Api.Features.Articles.GetArticleChildren;
using LogicPOS.Api.Features.Articles.PriceTypes.GetAllPriceTypes;
using LogicPOS.Api.Features.Articles.Subfamilies.GetAllArticleSubfamilies;
using LogicPOS.Api.Features.Articles.UpdateArticle;
using LogicPOS.Api.Features.Articles.UpdateArticleChildren;
using LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups;
using LogicPOS.Api.Features.Customers.DiscountGroups.GetAllDiscountGroups;
using LogicPOS.Api.Features.Printers.GetAllPrinters;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Finance.VatExemptionReasons;
using LogicPOS.UI.Components.Finance.VatRates;
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
            HandleMode(modalMode);
            ApplySdrDepositArticleFieldRestrictions();
        }

        private void HandleMode(EntityEditionModalMode modalMode)
        {
            if (_modalMode == EntityEditionModalMode.Insert)
            {
                _txtCode.Text = ArticlesService.GenerateRandomCode();
            }
        }

        private AddArticleCommand CreateAddCommand()
        {
            var addArticleCommand = new AddArticleCommand();

            addArticleCommand.Code = _txtCode.Text;
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
            addArticleCommand.BarcodeLabelPrintModel = _comboPrintModels.SelectedEntity.Model;
            addArticleCommand.UniqueArticles = _checkUniqueArticles.Active;
            addArticleCommand.IsSdrPackaging = _checkIsSdrPackaging.Active;
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
            updateCommand.Order = uint.Parse(_txtOrder.Text);
            updateCommand.Code = _txtCode.Text;
            updateCommand.CodeDealer = _txtCodeDealer.Text;
            updateCommand.Designation = _txtDesignation.Text;
            updateCommand.Button = GetButton();
            updateCommand.PriceWithVat = _checkPriceWithVat.Active;
            updateCommand.Discount = decimal.Parse(_txtDiscount.Text);
            updateCommand.DefaultQuantity = decimal.Parse(_txtDefaultQuantity.Text);
            updateCommand.MinimumStock = decimal.Parse(_txtMinimumStock.Text);
            updateCommand.Tare = decimal.Parse(_txtTare.Text);
            updateCommand.Weight = float.Parse(_txtWeight.Text);
            updateCommand.Barcode = _txtBarcode.Text;
            updateCommand.PVPVariable = _checkPVPVariable.Active;
            updateCommand.Favorite = _checkFavorite.Active;
            updateCommand.UseWeighingBalance = _checkUseWeighingBalance.Active;
            updateCommand.SubfamilyId = _comboSubfamilies.SelectedEntity.Id;
            updateCommand.TypeId = _comboTypes.SelectedEntity.Id;
            updateCommand.ClassId = _comboClasses.SelectedEntity.Id;
            updateCommand.MeasurementUnitId = _comboMeasurementUnits.SelectedEntity.Id;
            updateCommand.SizeUnitId = _comboSizeUnits.SelectedEntity.Id;
            updateCommand.CommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id ?? Guid.Empty;
            updateCommand.DiscountGroupId = _comboDiscountGroups.SelectedEntity?.Id ?? Guid.Empty;
            updateCommand.VatDirectSellingId = _comboVatDirectSelling.SelectedEntity.Id;
            updateCommand.VatExemptionReasonId = _comboVatExemptionReasons.SelectedEntity?.Id ?? Guid.Empty;
            updateCommand.IsComposed = _checkIsComposed.Active;
            updateCommand.BarcodeLabelPrintModel = _comboPrintModels.SelectedEntity?.Model;
            updateCommand.UniqueArticles = _checkUniqueArticles.Active;
            updateCommand.IsSdrPackaging = _checkIsSdrPackaging.Active;
            updateCommand.Notes = _txtNotes.Value.Text;
            updateCommand.IsDeleted = _checkDisabled.Active;


            switch (_prices.Count())
            {
                case 1:
                    updateCommand.Price1 = _prices[0].Price;
                    break;
                case 2:
                    updateCommand.Price1 = _prices[0].Price;
                    updateCommand.Price2 = _prices[1].Price;

                    break;
                case 3:
                    updateCommand.Price1 = _prices[0].Price;
                    updateCommand.Price2 = _prices[1].Price;
                    updateCommand.Price3 = _prices[2].Price;
                    break;
                case 4:
                    updateCommand.Price1 = _prices[0].Price;
                    updateCommand.Price2 = _prices[1].Price;
                    updateCommand.Price3 = _prices[2].Price;
                    updateCommand.Price4 = _prices[3].Price;

                    break;
                case 5:
                    updateCommand.Price1 = _prices[0].Price;
                    updateCommand.Price2 = _prices[1].Price;
                    updateCommand.Price3 = _prices[2].Price;
                    updateCommand.Price4 = _prices[3].Price;
                    updateCommand.Price5 = _prices[4].Price;
                    break;
            }

            return updateCommand;
        }

        protected override bool AddEntity()
        {
            var result = ExecuteAddCommand(CreateAddCommand());

            if (result.IsError)
            {
                return false;
            }

            if (_checkIsComposed.Active == false)
            {
                return true;
            }

            var addChildrenCommand = new AddArticleChildrenCommand
            {
                Id = result.Value,
                Children = _addArticlesBox.GetArticleChildren()
            };

            ExecuteAddCommand(addChildrenCommand);

            return true;
        }

        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtButtonName.Text = _entity.Button?.Label;
            if (EntityHasImage)
            {
                ShowImage();
            }
            _checkIsComposed.Active = _entity.IsComposed;
            _checkFavorite.Active = _entity.Favorite;
            _checkUseWeighingBalance.Active = _entity.UseWeighingBalance;
            _txtNotes.Value.Text = _entity.Notes;
            _checkPVPVariable.Active = _entity.PVPVariable;
            _checkPriceWithVat.Active = _entity.PriceWithVat;
            _txtDiscount.Text = _entity.Discount.ToString("F2");
            _txtBarcode.Text = _entity.Barcode;
            _txtMinimumStock.Text = _entity.MinimumStock.ToString("F2");
            _txtTare.Text = _entity.Tare.ToString("F2");
            _txtWeight.Text = _entity.Weight.ToString("F2");
            _txtDefaultQuantity.Text = _entity.DefaultQuantity.ToString("F2");
            _checkDisabled.Active = _entity.IsDeleted;
            _checkUniqueArticles.Active = _entity.UniqueArticles;
            _checkIsSdrPackaging.Active = _entity.IsSdrPackaging;
            if (_entity.IsComposed)
            {
                var children = ExecuteGetEntitiesQuery(new GetArticleChildrenQuery(_entity.Id));
                _addArticlesBox.AddArticleChildren(children);
            }
        }

        private bool EntityHasImage => string.IsNullOrWhiteSpace(_entity.Button?.Image) == false && string.IsNullOrWhiteSpace(_entity.Button?.ImageExtension) == false;

        private void ShowImage()
        {
            string imagePath = ButtonImageCache.GetImageLocation(_entity.Id, _entity.Button.ImageExtension) ?? ButtonImageCache.AddBase64Image(_entity.Id, _entity.Button.Image, _entity.Button.ImageExtension);
            _imagePicker.SetImage(imagePath);
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

        protected override bool UpdateEntity()
        {
            var result = ExecuteUpdateCommand(CreateUpdateCommand());

            if (result.IsError)
            {
                return false;
            }

            UpdateImageInCache();

            if (_checkIsComposed.Active)
            {
                var updateChildrenCommand = new UpdateArticleChildrenCommand
                {
                    Id = _entity.Id,
                    Children = _addArticlesBox.GetArticleChildren()
                };

                ExecuteUpdateCommand(updateChildrenCommand);
            }

            return true;
        }

        private void UpdateImageInCache()
        {
            if (_imagePicker.HasImage == true)
            {
                ButtonImageCache.DeleteImage(_entity.Id, _entity.Button.ImageExtension);
                return;
            }

        }

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
        private IEnumerable<VatRate> GetVatRates() => VatRatesService.VatRates;
        private IEnumerable<PriceType> GetPriceTypes() => ExecuteGetEntitiesQuery(new GetAllPriceTypesQuery());
        private IEnumerable<CommissionGroup> GetCommissionGroups() => ExecuteGetEntitiesQuery(new GetAllCommissionGroupsQuery());
        private IEnumerable<Printer> GetPrinters() => ExecuteGetEntitiesQuery(new GetAllPrintersQuery());
        private IEnumerable<VatExemptionReason> GetVatExemptionReasons() => VatExemptionReasonsService.Reasons;
    }
}
