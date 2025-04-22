using LogicPOS.Api.Enums;
using LogicPOS.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            MenuPlaces.SelectedEntity = null;
            MenuTables.Refresh(TableStatus.Reserved);
        }

        private void BtnFilterOpen_Clicked(object sender, EventArgs e)
        {
            MenuPlaces.SelectedEntity = null;
            MenuTables.Refresh(TableStatus.Open);
        }

        private void BtnFilterFree_Clicked(object sender, EventArgs e)
        {
            MenuPlaces.SelectedEntity = null;
            MenuTables.Refresh(TableStatus.Free);
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (MenuTables.SelectedEntity != null)
            {
                SaleContext.SetCurrentTable(MenuTables.SelectedEntity);
            }
        }

        private void MenuTables_TableSelected(Api.Entities.Table table)
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

            MenuTables.Refresh(MenuTables.Filter);
        }
    }
}
