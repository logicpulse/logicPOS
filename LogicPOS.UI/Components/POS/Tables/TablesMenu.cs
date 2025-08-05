using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Enums;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using System.Collections.Generic;
using System.Linq;
using Table = LogicPOS.Api.Entities.Table;

namespace LogicPOS.UI.Components.Menus
{
    public class TablesMenu : Menu<Table>
    {
        public PlacesMenu MenuPlaces { get; }
        public TableStatus? Filter { get; private set; } = null;

        public TablesMenu(CustomButton btnPrevious,
                          CustomButton btnNext,
                          PlacesMenu palcesMenu,
                          Window sourceWindow) : base(5,
                                                      4,
                                                      AppSettings.Instance.SizePosTableButton,
                                                      "",
                                                      btnPrevious,
                                                      btnNext,
                                                      sourceWindow)
        {

            MenuPlaces = palcesMenu;
            AddEventHandlers();
            LoadEntities();
            ListEntities(Entities);
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
            var filteredTables = FilterEntities(Entities);
            ListEntities(filteredTables);
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

            if (SelectedButton != null)
            {
                SelectedButton.Sensitive = false;
            }
        }

        protected override IEnumerable<Table> FilterEntities(IEnumerable<Table> entities)
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
