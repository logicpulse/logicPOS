using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.SizeUnits.AddSizeUnit;
using LogicPOS.Api.Features.SizeUnits.UpdateSizeUnit;

namespace LogicPOS.UI.Components.Modals
{
    public partial class SizeUnitModal : EntityEditionModal<SizeUnit>
    {
        public SizeUnitModal(EntityEditionModalMode modalMode, SizeUnit entity = null) : base(modalMode, entity)
        {
        }

        private AddSizeUnitCommand CreateAddCommand()
        {
            return new AddSizeUnitCommand
            {
                Designation = _txtDesignation.Text,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateSizeUnitCommand CreateUpdateCommand()
        {
            return new UpdateSizeUnitCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
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
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }

    }
}
