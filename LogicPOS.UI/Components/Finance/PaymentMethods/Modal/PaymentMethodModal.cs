using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PaymentMethods.AddPaymentMethod;
using LogicPOS.Api.Features.PaymentMethods.UpdatePaymentMethod;


namespace LogicPOS.UI.Components.Modals
{
    public partial class PaymentMethodModal : EntityEditionModal<PaymentMethod>
    {
        public PaymentMethodModal(EntityEditionModalMode modalMode, PaymentMethod entity = null) : base(modalMode, entity)
        {
        }

        private AddPaymentMethodCommand CreateAddCommand()
        {
            return new AddPaymentMethodCommand
            {
                Designation = _txtDesignation.Text,
                Token = _txtToken.Text,
                Acronym = _txtAcronym.Text,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdatePaymentMethodCommand CreateUpdateCommand()
        {
            return new UpdatePaymentMethodCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewToken = _txtToken.Text,
                NewAcronym = _txtAcronym.Text,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override bool AddEntity() => ExecuteAddCommand(CreateAddCommand()).IsError == false;
        protected override bool UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand()).IsError == false;

        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtToken.Text = _entity.Token;
            _txtAcronym.Text = _entity.Acronym;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }
    }
}
