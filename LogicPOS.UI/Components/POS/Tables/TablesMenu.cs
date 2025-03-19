using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Enums;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Table = LogicPOS.Api.Entities.Table;

namespace LogicPOS.UI.Components.Menus
{
    public class TablesMenu : Menu<Table>
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public PlacesMenu MenuPlaces { get; }
        public TableStatus? Filter { get; private set; } = null;

        public TablesMenu(CustomButton btnPrevious,
                          CustomButton btnNext,
                          PlacesMenu palcesMenu,
                          Window sourceWindow) : base(5,
                                                      4,
                                                      AppSettings.Instance.sizePosTableButton,
                                                      "",
                                                      btnPrevious,
                                                      btnNext,
                                                      sourceWindow)
        {

            MenuPlaces = palcesMenu;
            AddEventHandlers();
            PresentEntities();
        }

        private void AddEventHandlers()
        {
            MenuPlaces.OnEntitySelected += PlacesMenu_PlaceSelected;
        }

        private void PlacesMenu_PlaceSelected(Place place)
        {
            Refresh();
        }

        protected override CustomButton CreateMenuButton(Table entity)
        {
            return new TableButton(entity);
        }

        protected override IEnumerable<Table> GetFilteredEntities()
        {
            IEnumerable<Table> tables = Entities;

            if (MenuPlaces.SelectedEntity != null)
            {
                tables = tables.Where(x => x.PlaceId == MenuPlaces.SelectedEntity.Id);
            }

            if (Filter != null)
            {
                tables = tables.Where(x => x.Status == Filter);
            }

            return tables;
        }


        protected override string GetButtonLabel(Table entity)
        {
            return entity.Designation;
        }

        protected override string GetButtonImage(Table entity)
        {
            return null;
        }

        protected override void LoadEntities()
        {
            Entities.Clear();
            Entities.AddRange(TablesService.GetAllTables());
        }

        public override void Refresh()
        {
            Refresh(null);
        }

        public void Refresh(TableStatus? tableStatus = null)
        {
            Buttons.Clear();
            Filter = tableStatus;
            LoadEntities();
            PresentEntities();
            SelectCurrentTable();
        }

        private void SelectCurrentTable()
        {
            if (SelectedEntity == null || SaleContext.CurrentTable == null || SelectedEntity.Id == SaleContext.CurrentTable.Id)
            {
                return;
            }

            SelectedEntity = SaleContext.CurrentTable;
            SelectedButton.Sensitive = true;
            SelectedButton = Buttons.FirstOrDefault(x => x.Item1.Id == SaleContext.CurrentTable.Id).Button;
            
            if(SelectedButton != null)
            {
                SelectedButton.Sensitive = false;
            }
        }
    }
}
