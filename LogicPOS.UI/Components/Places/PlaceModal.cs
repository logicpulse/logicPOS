using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.PriceTypes.GetAllPriceTypes;
using LogicPOS.Api.Features.MovementTypes.GetAllMovementTypes;
using LogicPOS.UI.Components.InputFields;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PlaceModal : EntityModal<Place>
    {
        public PlaceModal(EntityModalMode modalMode, Place entity = null) : base(modalMode, entity)
        {
        }

        protected override void BeforeDesign()
        {
            _comboPriceTypes = new EntityComboBox<PriceType>(GetPriceTypes());
        }

        private IEnumerable<PriceType> GetPriceTypes()
        {
            var getPriceTypesResult = _mediator.Send(new GetAllPriceTypesQuery()).Result;

            if (getPriceTypesResult.IsError)
            {
                return Enumerable.Empty<PriceType>();
            }

            return getPriceTypesResult.Value;
        }

        private IEnumerable<MovementType> GetMovementTypes()
        {

           var getMovementTypesResult = _mediator.Send(new GetAllMovementTypesQuery()).Result;

            if (getMovementTypesResult.IsError)
            {
                return Enumerable.Empty<MovementType>();
            }

            return getMovementTypesResult.Value;
        }

        protected override void ShowEntityData()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateEntity()
        {
            throw new NotImplementedException();
        }

        protected override void AddEntity()
        {
            throw new NotImplementedException();
        }
    }
}
