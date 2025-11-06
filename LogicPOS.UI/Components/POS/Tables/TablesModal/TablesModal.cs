using Gtk;
using LogicPOS.Api.Features.POS.Tables.Common;
using LogicPOS.Globalization;
using LogicPOS.UI.Application.Enums;
using LogicPOS.UI.Components.Common.Menus;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System.Drawing;

namespace LogicPOS.UI.Components.POS
{
    public partial class TablesModal : Modal
    {
        private static string MotalTitle => LocalizedString.Instance[$"window_title_dialog_tables_appmode_{AppSettings.Instance.AppOperationModeTheme}"];
        private static string ModalIcon => AppSettings.Instance.OperationMode.IsRetailMode() ? AppSettings.Paths.Images + @"Icons\Windows\icon_window_tables_retail.png" : AppSettings.Paths.Images + @"Icons\Windows\icon_window_tables.png";

        public TablesModal(MenuMode menuMode, Window parent) : base(parent,
                                                 MotalTitle,
                                                 new Size(720, 580),
                                                 ModalIcon)
        {
            BtnViewTables.Visible = false;
            _mode = menuMode;

            if (_mode == MenuMode.SelectFree)
            {
                InitializeSelectionMode();
                MenuTables.SetSelectFreeMode();
            }
            if (_mode == MenuMode.SelectOther)
            {
                InitializeSelectionMode();
                MenuTables.SetSelectOtherMode();
            }
        }

        public TableViewModel GetSelectedTable()
        {
            return MenuTables.SelectedEntity;
        }

        private void InitializeSelectionMode()
        {
            BtnFilterAll.Visible = false;
            BtnFilterFree.Visible = false;
            BtnFilterOpen.Visible = false;
            BtnFilterReserved.Visible = false;
            BtnReservation.Visible = false;
            BtnViewOrders.Visible = false;
            BtnViewTables.Visible = false;
            MenuTables.ApplyFilter(TableStatus.Free);
        }
        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
            BtnFilterFree.Clicked += BtnFilterFree_Clicked;
            BtnFilterOpen.Clicked += BtnFilterOpen_Clicked;
            BtnFilterReserved.Clicked += BtnFilterReserved_Clicked;
            BtnFilterAll.Clicked += BtnFilterAll_Clicked;
            BtnReservation.Clicked += BtnReservation_Clicked;
        }

        private void InitializeScrollerButtons()
        {
            InitializePlacesScrollersButtons();
            InitializeTablesScrollersButtons();
        }

        protected override void OnResponse(ResponseType response)
        {
            if (response != ResponseType.Ok && response != ResponseType.Cancel && response != ResponseType.Close)
            {
                Run();
                return;
            }

            base.OnResponse(response);
        }
    }
}
;