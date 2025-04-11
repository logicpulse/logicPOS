using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Families.AddArticleFamily;
using LogicPOS.Api.Features.Articles.Families.UpdateArticleFamily;
using LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups;
using LogicPOS.Api.Features.Printers.GetAllPrinters;
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
                PrinterAssociationService.CreateOrRemoveAssociation(_entity.Id, _comboPrinters.SelectedEntity.Id);
            }
            else
            {
                PrinterAssociationService.CreateOrRemoveAssociation(_entity.Id);
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
            _imagePicker.SetBase64Image(_entity.Button?.Image, _entity.Button?.ImageExtension);
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

        private IEnumerable<Printer> GetPrinters() => ExecuteGetEntitiesQuery(new GetAllPrintersQuery());
        private IEnumerable<CommissionGroup> GetCommissionGroups() => ExecuteGetEntitiesQuery(new GetAllCommissionGroupsQuery());

    }
}
