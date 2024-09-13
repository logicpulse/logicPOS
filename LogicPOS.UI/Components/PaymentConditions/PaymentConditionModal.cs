using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PaymentConditions.AddPaymentCondition;
using LogicPOS.Api.Features.PaymentConditions.UpdatePaymentCondition;



namespace LogicPOS.UI.Components.Modals
{
    public partial class PaymentConditionModal : EntityEditionModal<PaymentCondition>
    {
        public PaymentConditionModal(EntityEditionModalMode modalMode, PaymentCondition entity = null) : base(modalMode, entity)
        {
        }

        private AddPaymentConditionCommand CreateAddCommand()
        {
            return new AddPaymentConditionCommand
            {
                Designation = _txtDesignation.Text,
                Acronym = _txtAcronym.Text,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdatePaymentConditionCommand CreateUpdateCommand()
        {
            return new UpdatePaymentConditionCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewAcronym = _txtAcronym.Text,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void AddEntity() => ExecuteAddCommand(CreateAddCommand());
        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtAcronym.Text = _entity.Acronym;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }
    }
}
