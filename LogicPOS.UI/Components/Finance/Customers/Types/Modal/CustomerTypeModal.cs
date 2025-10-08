using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Customers.Types.AddCustomerType;
using LogicPOS.Api.Features.Customers.Types.UpdateCustomerType;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CustomerTypeModal : EntityEditionModal<CustomerType>
    {

        public CustomerTypeModal(EntityEditionModalMode modalMode,
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

        protected override bool AddEntity() => ExecuteAddCommand(CreateAddCommand()).IsError == false;
        protected override bool UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand()).IsError == false;


    }
}
