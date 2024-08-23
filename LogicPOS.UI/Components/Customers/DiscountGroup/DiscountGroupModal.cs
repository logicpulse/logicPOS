using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Customers.DiscountGroups.AddDiscountGroup;
using LogicPOS.Api.Features.Customers.DiscountGroups.UpdateDiscountGroup;
using LogicPOS.Api.Features.Customers.Types.AddCustomerType;
using LogicPOS.Api.Features.Customers.Types.UpdateCustomerType;
using LogicPOS.Domain.Entities;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DiscountGroupModal:EntityModal<DiscountGroup>
    {
        private EntityModalMode mode;

        public DiscountGroupModal(EntityModalMode modalMode, DiscountGroup entity = null) : base(modalMode, entity)
        {
        }

        private AddDiscountGroupCommand CreateAddCommand()
        {
            return new AddDiscountGroupCommand
            {
                Designation = _txtDesignation.Text,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateDiscountGroupCommand CreateUpdateCommand()
        {
            return new UpdateDiscountGroupCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void ShowEntityData()
        {
            _txtCode.Text = _entity.Code.ToString();
            _txtOrder.Text = _entity.Order.ToString();
            _txtDesignation.Text = _entity.Designation;
            _txtNotes.Value.Text = _entity.Notes;
            _checkDisabled.Active = _entity.IsDeleted;
        }

        protected override void UpdateEntity()
        {
            var command = CreateUpdateCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
            }
        }

        protected override void AddEntity()
        {
            var command = CreateAddCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
            }
        }
    }
}