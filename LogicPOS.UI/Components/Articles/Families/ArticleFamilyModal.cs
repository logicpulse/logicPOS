using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Families.AddArticleFamily;
using LogicPOS.Api.Features.Articles.Families.UpdateArticleFamily;
using LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups;
using LogicPOS.Api.Features.Customers.DiscountGroups.GetAllDiscountGroups;
using LogicPOS.Api.Features.Printers.GetAllPrinters;
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
            return new AddArticleFamilyCommand
            {
                Designation = _txtDesignation.Text,
                Button = GetButton(),
                CommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id,
                PrinterId = _comboPrinters.SelectedEntity?.Id,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateArticleFamilyCommand CreateUpdateCommand()
        {
            return new UpdateArticleFamilyCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewCommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id,
                NewPrinterId = _comboPrinters.SelectedEntity?.Id,
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
            _txtButtonName.Text = _entity.Button?.ButtonLabel;
            _imagePicker.SetImage(_entity.Button?.ButtonImage);
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

        private IEnumerable<Printer> GetPrinters() => ExecuteGetAllQuery(new GetAllPrintersQuery());
        private IEnumerable<CommissionGroup> GetCommissionGroups() => ExecuteGetAllQuery(new GetAllCommissionGroupsQuery());

    }
}
