using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Places.GetAllPlaces;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Settings;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Menus
{
    public class PlacesMenu : Menu<Place>
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();

        public PlacesMenu(CustomButton btnPrevious,
                          CustomButton btnNext,
                          Window sourceWindow) : base(5,
                                                      1,
                                                      AppSettings.Instance.SizePosTableButton,
                                                      "buttonFamilyId",
                                                      btnPrevious,
                                                      btnNext,
                                                      sourceWindow)
        {
            LoadEntities();
            ListEntities(Entities);
        }

        protected override IEnumerable<Place> FilterEntities(IEnumerable<Place> entities)
        {
            return entities.Where(t => t.Id == TerminalService.Terminal.PlaceId);
        }

        protected override string GetButtonImage(Place entity)
        {
            return null;
        }

        protected override string GetButtonLabel(Place entity)
        {
            return entity.Designation;
        }

        protected override void LoadEntities()
        {
            Entities.Clear();

            var places = _mediator.Send(new GetAllPlacesQuery()).Result;

            if (places.IsError != false)
            {
                return;
            }

            Entities.AddRange(places.Value);

        }
    }
}
