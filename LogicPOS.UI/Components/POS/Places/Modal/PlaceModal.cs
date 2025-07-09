using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.PriceTypes.GetAllPriceTypes;
using LogicPOS.Api.Features.MovementTypes.GetAllMovementTypes;
using LogicPOS.Api.Features.Places.AddPlace;
using LogicPOS.Api.Features.Places.UpdatePlace;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PlaceModal : EntityEditionModal<Place>
    {
        public PlaceModal(EntityEditionModalMode modalMode, Place entity = null) : base(modalMode, entity)
        {
        }
        private IEnumerable<PriceType> GetPriceTypes() => ExecuteGetEntitiesQuery(new GetAllPriceTypesQuery());

        private IEnumerable<MovementType> GetMovementTypes()=>ExecuteGetEntitiesQuery(new GetAllMovementTypesQuery());

        protected override void BeforeDesign()
        {
            InitializePriceTypesComboBox();
            InitializeMovementTypesComboBox();
        }
        protected override void ShowEntityData()
        {
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtOrder.Text = _entity.Order.ToString();
            _txtNotes.Value.Text = _entity.Notes;
            _checkDisabled.Active = _entity.IsDeleted;
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

        private UpdatePlaceCommand CreateUpdateCommand()
        {
            return new UpdatePlaceCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active,
                NewPriceTypeId = _comboPriceTypes.SelectedEntity.Id,
                NewMovementTypeId = _comboMovementTypes.SelectedEntity.Id
            };
        }

        private AddPlaceCommand CreateAddCommand()
        {
            return new AddPlaceCommand
            {
                Designation = _txtDesignation.Text,
                Notes = _txtNotes.Value.Text,
                PriceTypeId = _comboPriceTypes.SelectedEntity.Id,
                MovementTypeId = _comboMovementTypes.SelectedEntity.Id
            };
        }
        protected override void AddEntity() => ExecuteAddCommand(CreateAddCommand());

    }
}
