using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Places.GetAllPlaces;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Common.Menus;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Settings;
using MediatR;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Menus
{
    public class PlacesMenu : Menu<Place>
    {
        private readonly ISender _mediator = DependencyInjection.Mediator;
        private string ButtonName => "touchButton_Green.buttonPlaceId";
        private Size ButtonSize => AppSettings.Instance.SizePosTableButton;

        public PlacesMenu(CustomButton btnPrevious,
                          CustomButton btnNext,
                          Window sourceWindow) : base(5,
                                                      1,
                                                      btnPrevious,
                                                      btnNext,
                                                      sourceWindow)
        {
            Refresh();
        }

        protected override CustomButton CreateButtonForEntity(Place entity)
        {
            string label = entity.Designation;
            return MenuButton<ArticleViewModel>.CreateButton(ButtonName, label, null, ButtonSize);
        }

        protected override void LoadEntities()
        {
            Entities.Clear();
            Guid? terminalPlaceId = TerminalService.Terminal.PlaceId;
            var places = _mediator.Send(new GetAllPlacesQuery(terminalPlaceId)).Result;

            if (places.IsError != false)
            {
                return;
            }

            Entities.AddRange(places.Value);

        }
    }
}
