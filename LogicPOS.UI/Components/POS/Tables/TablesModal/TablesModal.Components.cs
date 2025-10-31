using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Common.Menus;
using LogicPOS.UI.Components.Menus;

namespace LogicPOS.UI.Components.POS
{
    public partial class TablesModal
    {
        private MenuMode _mode;
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnFilterAll { get; set; }
        private IconButtonWithText BtnFilterFree { get; set; }
        private IconButtonWithText BtnFilterOpen { get; set; }
        private IconButtonWithText BtnFilterReserved { get; set; }
        private IconButtonWithText BtnReservation { get; set; }
        private IconButtonWithText BtnViewOrders { get; set; }
        private IconButtonWithText BtnViewTables { get; set; }
        private IconButton BtnScrollPlacesPrevious { get; set; }
        private IconButton BtnScrollPlacesNext { get; set; }
        private IconButton BtnScrollTablesPrevious { get; set; }
        private IconButton BtnScrollTablesNext { get; set; }
        private PlacesMenu MenuPlaces { get; set; }
        private TablesMenu MenuTables { get; set; }
    }
}
