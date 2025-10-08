using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Families.GetAllArticleFamilies;
using LogicPOS.Api.Features.Articles.Subfamilies.AddArticleSubfamily;
using LogicPOS.Api.Features.Articles.Subfamilies.UpdateArticleSubfamily;
using LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups;
using LogicPOS.Api.Features.Customers.DiscountGroups.GetAllDiscountGroups;
using LogicPOS.Api.Features.Printers.GetAllPrinters;
using LogicPOS.Api.Features.VatRates.GetAllVatRate;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.POS.Devices.Printers.PrinterAssociation;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleSubfamilyModal : EntityEditionModal<ArticleSubfamily>
    {
        public ArticleSubfamilyModal(EntityEditionModalMode modalMode, ArticleSubfamily entity = null) : base(modalMode, entity)
        {
        }

        private AddArticleSubfamilyCommand CreateAddCommand()
        {
            return new AddArticleSubfamilyCommand
            {
                Designation = _txtDesignation.Text,
                Button = GetButton(),
                FamilyId = _comboFamilies.SelectedEntity.Id,
                CommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id,
                DiscountGroupId = _comboDiscountGroups.SelectedEntity?.Id,
                VatOnTableId = _comboVatOnTable.SelectedEntity?.Id,
                VatDirectSellingId = _comboVatDirectSelling.SelectedEntity?.Id,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateArticleSubfamilyCommand CreateUpdateCommand()
        {
            if (_comboPrinters.SelectedEntity != null)
            {
                PrinterAssociationService.Set(_entity.Id, _comboPrinters.SelectedEntity.Id);
            }
            else
            {
                PrinterAssociationService.Set(_entity.Id);
            }
            return new UpdateArticleSubfamilyCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewFamilyId = _comboFamilies.SelectedEntity.Id,
                NewCommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id ?? Guid.Empty,
                NewDiscountGroupId = _comboDiscountGroups.SelectedEntity?.Id ?? Guid.Empty,
                NewVatOnTableId = _comboVatOnTable.SelectedEntity?.Id ?? Guid.Empty,
                NewVatDirectSellingId = _comboVatDirectSelling.SelectedEntity?.Id ?? Guid.Empty,
                NewDesignation = _txtDesignation.Text,
                NewButton = GetButton(),
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override bool AddEntity() => ExecuteAddCommand(CreateAddCommand()).IsError == false;

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
            _comboPrinters.SelectedEntity = PrinterAssociationService.GetPrinter(_entity.Id);
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;

        }

        private bool EntityHasImage => string.IsNullOrWhiteSpace(_entity.Button?.Image) == false && string.IsNullOrWhiteSpace(_entity.Button?.ImageExtension) == false;

        private void ShowImage()
        {
            string imagePath = ButtonImageCache.GetImageLocation(_entity.Id, _entity.Button.ImageExtension) ?? ButtonImageCache.AddBase64Image(_entity.Id, _entity.Button.Image, _entity.Button.ImageExtension);
            _imagePicker.SetImage(imagePath);
        }

        protected override bool UpdateEntity()
        {
            var result = ExecuteUpdateCommand(CreateUpdateCommand());
            if (result.IsError)
            {
                return false;
            }
            UpdateImageInCache();

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
        private IEnumerable<Printer> GetPrinters() => ExecuteGetEntitiesQuery(new GetAllPrintersQuery());
        private IEnumerable<ArticleFamily> GetFamilies() => ExecuteGetEntitiesQuery(new GetAllArticleFamiliesQuery());
        private IEnumerable<DiscountGroup> GetDiscountGroups() => ExecuteGetEntitiesQuery(new GetAllDiscountGroupsQuery());
        private IEnumerable<VatRate> GetVatRates() => ExecuteGetEntitiesQuery(new GetAllVatRatesQuery());

        private IEnumerable<CommissionGroup> GetCommissionGroups() => ExecuteGetEntitiesQuery(new GetAllCommissionGroupsQuery());
    }
}
