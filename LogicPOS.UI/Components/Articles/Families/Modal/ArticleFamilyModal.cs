using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Families.AddArticleFamily;
using LogicPOS.Api.Features.Articles.Families.UpdateArticleFamily;
using LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups;
using LogicPOS.Api.Features.Printers.GetAllPrinters;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.POS.Devices.Printers.PrinterAssociation;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleFamilyModal : EntityEditionModal<ArticleFamily>
    {
        public ArticleFamilyModal(EntityEditionModalMode modalMode, ArticleFamily entity = null) : base(modalMode, entity)
        {
        }

        private AddArticleFamilyCommand CreateAddCommand()
        {
            return new AddArticleFamilyCommand
            {
                Designation = _txtDesignation.Text,
                Button = GetButton(),
                CommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id,
                Notes = _txtNotes.Value.Text

            };
        }

        private UpdateArticleFamilyCommand CreateUpdateCommand()
        {
            if (_comboPrinters.SelectedEntity != null)
            {
                PrinterAssociationService.Set(_entity.Id, _comboPrinters.SelectedEntity.Id);
            }
            else
            {
                PrinterAssociationService.Set(_entity.Id);
            }
            return new UpdateArticleFamilyCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewCommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id ?? Guid.Empty,
                NewDesignation = _txtDesignation.Text,
                NewButton = GetButton(),
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

        private void UpdateImageInCache()
        {
            if (_imagePicker.HasImage == true)
            {
                ButtonImageCache.DeleteImage(_entity.Id, _entity.Button.ImageExtension);
                return;
            }

        }

        protected override void UpdateEntity()
        {
            var result = ExecuteUpdateCommand(CreateUpdateCommand());
            if (result.IsError)
            {
                return;
            }
            UpdateImageInCache();
        }

        private IEnumerable<Printer> GetPrinters() => ExecuteGetEntitiesQuery(new GetAllPrintersQuery());
        private IEnumerable<CommissionGroup> GetCommissionGroups() => ExecuteGetEntitiesQuery(new GetAllCommissionGroupsQuery());

    }
}
