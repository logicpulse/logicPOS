using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.POS.Tables.Common;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Services;
using System.Collections.Generic;
using System.Linq;
using Table = LogicPOS.Api.Features.POS.Tables.Common.Table;

namespace LogicPOS.UI.Components.Menus
{
    public class TablesMenu : Menu<TableViewModel>
    {
        public PlacesMenu MenuPlaces { get; }
        public TableStatus? LastFilter { get; set; }
        public TableStatus? Filter { get; private set; } = null;

        public TablesMenu(CustomButton btnPrevious,
                          CustomButton btnNext,
                          PlacesMenu palcesMenu,
                          Window sourceWindow) : base(5,
                                                      4,
                                                      btnPrevious,
                                                      btnNext,
                                                      sourceWindow)
        {

            MenuPlaces = palcesMenu;
            AddEventHandlers();
            Refresh();
        }

        private void AddEventHandlers()
        {
            MenuPlaces.OnEntitySelected += PlacesMenu_PlaceSelected;
        }

        private void PlacesMenu_PlaceSelected(Place place)
        {
            Refresh();
        }

        protected override CustomButton CreateButtonForEntity(TableViewModel entity)
        {
            return new TableButton(entity);
        }

        protected override void LoadEntities()
        {
            Entities.Clear();
            var filteredEntities = FilterEntities(TablesService.GetAllTables());
            Entities.AddRange(filteredEntities);
        }

        public override void Refresh()
        {
            base.Refresh();
            SelectCurrentTable();
        }

        public void ApplyFilter(TableStatus filter)
        {
            Filter = filter;
            var selectedPlace = MenuPlaces.SelectedEntity;
            MenuPlaces.SelectedEntity = null;

            Refresh();

            LastFilter = filter;
            Filter = null;
            MenuPlaces.SelectedEntity = selectedPlace;
        }

        private void SelectCurrentTable()
        {
            if (SelectedEntity == null || SaleContext.CurrentTable == null || SelectedEntity.Id == SaleContext.CurrentTable.Id)
            {
                return;
            }

            SelectedEntity = SaleContext.CurrentTable;
            SelectedButton.Sensitive = true;
            SelectedButton = ButtonsCache.Where(mb => mb.Entity.Id == SaleContext.CurrentTable.Id).Select(mb => mb.Button).FirstOrDefault();

            if (SelectedButton != null)
            {
                SelectedButton.Sensitive = false;
            }
        }

        private IEnumerable<TableViewModel> FilterEntities(IEnumerable<TableViewModel> entities)
        {
            if (MenuPlaces.SelectedEntity != null)
            {
                entities = entities.Where(x => x.PlaceId == MenuPlaces.SelectedEntity.Id);
            }

            if (Filter != null)
            {
                entities = entities.Where(x => x.Status == Filter);
            }

            return entities;
        }
    }
}
