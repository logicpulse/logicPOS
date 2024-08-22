using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Customers.Types.AddCustomerType;
using LogicPOS.Api.Features.Customers.Types.UpdateCustomerType;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CustomerTypeModal : EntityModal<CustomerType>
    {

        public CustomerTypeModal(EntityModalMode modalMode,
                                 CustomerType customerType = null) : base(modalMode, customerType)
        {
        }

        private AddCustomerTypeCommand CreateAddCommand()
        {
            return new AddCustomerTypeCommand
            {
                Designation = _txtDesignation.Text,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateCustomerTypeCommand CreateUpdateCommand()
        {
            return new UpdateCustomerTypeCommand
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
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
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
