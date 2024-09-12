using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Families.GetAllArticleFamilies;
using LogicPOS.Api.Features.Articles.Subfamilies.AddArticleSubfamily;
using LogicPOS.Api.Features.Articles.Subfamilies.UpdateArticleSubfamily;
using LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups;
using LogicPOS.Api.Features.Customers.DiscountGroups.GetAllDiscountGroups;
using LogicPOS.Api.Features.Printers.GetAllPrinters;
using LogicPOS.Api.Features.VatRates.GetAllVatRate;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleSubfamilyModal : EntityModal<ArticleSubfamily>
    {
        public ArticleSubfamilyModal(EntityModalMode modalMode, ArticleSubfamily entity = null) : base(modalMode, entity)
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
                PrinterId = _comboPrinters.SelectedEntity?.Id,
                DiscountGroupId = _comboDiscountGroups.SelectedEntity?.Id,
                VatOnTableId = _comboVatOnTable.SelectedEntity?.Id,
                VatDirectSellingId = _comboVatDirectSelling.SelectedEntity?.Id,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateArticleSubfamilyCommand CreateUpdateCommand()
        {
            return new UpdateArticleSubfamilyCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewFamilyId = _comboFamilies.SelectedEntity.Id,
                NewCommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id,
                NewPrinterId = _comboPrinters.SelectedEntity?.Id,
                NewDiscountGroupId = _comboDiscountGroups.SelectedEntity?.Id,
                NewVatOnTableId = _comboVatOnTable.SelectedEntity?.Id,
                NewVatDirectSellingId = _comboVatDirectSelling.SelectedEntity?.Id,
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

        private IEnumerable<Printer> GetPrinters()
        {
            var printers = _mediator.Send(new GetAllPrintersQuery()).Result;

            if (printers.IsError)
            {
                return Enumerable.Empty<Printer>();
            }

            return printers.Value;
        }

        private IEnumerable<ArticleFamily> GetFamilies()
        {
            var families = _mediator.Send(new GetAllArticleFamiliesQuery()).Result;

            if (families.IsError)
            {
                return Enumerable.Empty<ArticleFamily>();
            }

            return families.Value;
        }

        private IEnumerable<DiscountGroup> GetDiscountGroups()
        {
            var groups = _mediator.Send(new GetAllDiscountGroupsQuery()).Result;

            if (groups.IsError)
            {
                return Enumerable.Empty<DiscountGroup>();
            }

            return groups.Value;
        }

        private IEnumerable<VatRate> GetVatRates()
        {
            var vatRates = _mediator.Send(new GetAllVatRatesQuery()).Result;

            if (vatRates.IsError)
            {
                return Enumerable.Empty<VatRate>();
            }

            return vatRates.Value;
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
