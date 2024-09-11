using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Families.AddArticleFamily;
using LogicPOS.Api.Features.Articles.Families.UpdateArticleFamily;
using LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups;
using LogicPOS.Api.Features.Printers.GetAllPrinters;
using LogicPOS.Api.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleFamilyModal : EntityModal<ArticleFamily>
    {
        public ArticleFamilyModal(EntityModalMode modalMode, ArticleFamily entity = null) : base(modalMode, entity)
        {
        }

        private AddArticleFamilyCommand CreateAddCommand()
        {
            var button = new Button { 
                ButtonLabel = _txtButtonName.Text,
                ButtonImage = _imagePicker.FileChooserButton.Filename
            };

            return new AddArticleFamilyCommand
            {
                Designation = _txtDesignation.Text,
                Button = button,
                CommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id,
                PrinterId = _comboPrinters.SelectedEntity?.Id,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateArticleFamilyCommand CreateUpdateCommand()
        {
            var button = new Button
            {
                ButtonLabel = _txtButtonName.Text,
                ButtonImage = _imagePicker.FileChooserButton.Filename
            };

            return new UpdateArticleFamilyCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewCommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id,
                NewPrinterId = _comboPrinters.SelectedEntity?.Id,
                NewDesignation = _txtDesignation.Text,
                NewButton = button,
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
            _txtButtonName.Text = _entity.Button?.ButtonLabel;
            _imagePicker.FileChooserButton.SetFilename(_entity.Button?.ButtonImage);
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

        private IEnumerable<Printer> GetPrinters()
        {
            var printers = _mediator.Send(new GetAllPrintersQuery()).Result;

            if (printers.IsError)
            {
                return Enumerable.Empty<Printer>();
            }

            return printers.Value;
        }

        private IEnumerable<CommissionGroup> GetCommissionGroups()
        {
            var groups = _mediator.Send(new GetAllCommissionGroupsQuery()).Result;

            if (groups.IsError)
            {
                return Enumerable.Empty<CommissionGroup>();
            }

            return groups.Value;
        }
    }
}
