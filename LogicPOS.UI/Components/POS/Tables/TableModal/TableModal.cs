using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Places.GetAllPlaces;
using LogicPOS.Api.Features.Tables.AddTable;
using LogicPOS.Api.Features.Tables.UpdateTable;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class TableModal : EntityEditionModal<Table>
    {
        public TableModal(EntityEditionModalMode modalMode, Table entity = null) : base(modalMode, entity)
        {
        }

        private IEnumerable<Place> GetPlaces() => ExecuteGetEntitiesQuery(new GetAllPlacesQuery());

        protected override void ShowEntityData()
        {
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtOrder.Text = _entity.Order.ToString();
            _txtNotes.Value.Text = _entity.Notes;
            _checkDisabled.Active = _entity.IsDeleted;
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

        private UpdateTableCommand CreateUpdateCommand()
        {
            return new UpdateTableCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active,
                NewPlaceId = _comboPlaces.SelectedEntity.Id
            };
        }

        private AddTableCommand CreateAddCommand()
        {
            return new AddTableCommand
            {
                Designation = _txtDesignation.Text,
                Notes = _txtNotes.Value.Text,
                PlaceId = _comboPlaces.SelectedEntity.Id
            };
        }

        protected override void AddEntity() => ExecuteAddCommand(CreateAddCommand());

    }
}
