using DevExpress.Schedule;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.MovementTypes.AddMovementType;
using LogicPOS.Api.Features.MovementTypes.UpdateMovementType;

namespace LogicPOS.UI.Components.Modals
{
    public partial class MovementTypeModal: EntityEditionModal<MovementType>
    {
        public MovementTypeModal(EntityEditionModalMode modalMode, MovementType entity = null) : base(modalMode, entity)
        {
        }

        private AddMovementTypeCommand CreateAddCommand()
        {
            return new AddMovementTypeCommand
            {
                Designation = _txtDesignation.Text,
                VatDirectSelling = _checkVatDirectSelling.Active,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateMovementTypeCommand CreateUpdateCommand()
        {
            return new UpdateMovementTypeCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewNotes = _txtNotes.Value.Text,
                VatDirectSelling= _checkVatDirectSelling.Active,
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
            _checkVatDirectSelling.Active = _entity.VatDirectSelling;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }
    }
}
