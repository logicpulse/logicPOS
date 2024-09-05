using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.MeasurementUnits.AddMeasurementUnit;
using LogicPOS.Api.Features.MeasurementUnits.UpdateMeasurementUnit;



namespace LogicPOS.UI.Components.Modals
{
    public partial class MeasurementUnitModal : EntityModal<MeasurementUnit>
    {
        public MeasurementUnitModal(EntityModalMode modalMode, MeasurementUnit entity = null) : base(modalMode, entity)
        {
        }

        private AddMeasurementUnitCommand CreateAddCommand()
        {
            return new AddMeasurementUnitCommand
            {
                Designation = _txtDesignation.Text,
                Acronym = _txtAcronym.Text,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateMeasurementUnitCommand CreateUpdateCommand()
        {
            return new UpdateMeasurementUnitCommand
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
