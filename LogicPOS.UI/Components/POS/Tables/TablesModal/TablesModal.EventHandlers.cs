using LogicPOS.Api.Features.POS.Orders.Orders.Common;
using LogicPOS.Api.Features.POS.Tables.Common;
using LogicPOS.UI.Components.Common.Menus;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Services;
using System;

namespace LogicPOS.UI.Components.POS
{
    public partial class TablesModal
    {
        private void BtnFilterAll_Clicked(object sender, EventArgs e)
        {
            MenuPlaces.SelectedEntity = null;
            MenuTables.Refresh();
        }

        private void BtnFilterReserved_Clicked(object sender, EventArgs e)
        {
            MenuTables.ApplyFilter(TableStatus.Reserved);
        }

        private void BtnFilterOpen_Clicked(object sender, EventArgs e)
        {
            MenuTables.ApplyFilter(TableStatus.Open);
        }

        private void BtnFilterFree_Clicked(object sender, EventArgs e)
        {
            MenuPlaces.SelectedEntity = null;
            MenuTables.ApplyFilter(TableStatus.Free);
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (MenuTables.SelectedEntity != null && _mode==MenuMode.Standard)
            {
                SaleContext.SetCurrentTable(MenuTables.SelectedEntity);
            }
        }

        private void MenuTables_TableSelected(TableViewModel table)
        {
            BtnReservation.Sensitive = (table.Status == TableStatus.Free || table.Status == TableStatus.Reserved) && table.Id != SaleContext.CurrentTable.Id;
            BtnOk.Sensitive = table.Status != TableStatus.Reserved;
        }

        private void BtnReservation_Clicked(object sender, EventArgs e)
        {
            if (MenuTables.SelectedEntity == null)
            {
                return;
            }

            if (MenuTables.SelectedEntity.Status == TableStatus.Free)
            {
                TablesService.ReserveTable(MenuTables.SelectedEntity);
            }
            else if (MenuTables.SelectedEntity.Status == TableStatus.Reserved)
            {
                TablesService.FreeTable(MenuTables.SelectedEntity);
            }

            RemoveReservedTableButtonFromCache(MenuTables.SelectedEntity);

            if (MenuTables.LastFilter.HasValue)
            {
                MenuTables.ApplyFilter(MenuTables.LastFilter.Value);
            }
            else
            {
                MenuTables.Refresh();
            }

        }

        private void RemoveReservedTableButtonFromCache(TableViewModel selectedEntity)
        {
            MenuTables.ButtonsCache.RemoveAll(mb => mb.Entity.Id == selectedEntity.Id);
        }
    }
}
