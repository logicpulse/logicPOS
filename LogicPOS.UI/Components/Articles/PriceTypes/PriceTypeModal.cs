using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.PriceTypes.AddPriceType;
using LogicPOS.Api.Features.Articles.PriceTypes.UpdatePriceType;
using LogicPOS.UI.Components.Modals;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PriceTypeModal : EntityModal<PriceType>
    {
        public PriceTypeModal(EntityModalMode modalMode, PriceType entity = null) : base(modalMode, entity)
        {
        }

        private AddPriceTypeCommand CreateAddCommand()
        {
            return new AddPriceTypeCommand
            {
                Designation = _txtDesignation.Text,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdatePriceTypeCommand CreateUpdateCommand()
        {
            return new UpdatePriceTypeCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void AddEntity()
        {
            var command = CreateAddCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
                return;
            }
        }

        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }

        protected override void UpdateEntity()
        {
            var command = CreateUpdateCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
                return;
            }
        }
    }
}
