using Gtk;
using logicpos.shared.Enums;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using System;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    public partial class PosTablesDialog
    {
        protected override void OnResponse(ResponseType pResponse)
        {
            if (pResponse >= (ResponseType)0 && pResponse <= (ResponseType)_tablesStatusShowAllIndex)
            {
                ToggleTableStatusButtons(pResponse);

                _currentTableStatusId = (int)pResponse;
   
                _tablePadTable.Filter = GetTablePadTableFilter(pResponse);

                this.Run();
            }
            else if (pResponse == _responseTypeTableReservation)
            {
                if (_currentButton.TableSettings.TableStatus == TableStatus.Free)
                {
                    _currentButton.ChangeTableStatus(_currentTableButtonOid, TableStatus.Reserved);
                }
                else
                {
                    _currentButton.ChangeTableStatus(_currentTableButtonOid, TableStatus.Free);
                }

                if (!_buttonTableFilterFree.Sensitive || !_buttonTableFilterReserved.Sensitive) _tablePadTable.UpdateSql();

                _buttonOk.Sensitive = false;
                _buttonTableReservation.Sensitive = false;


                this.Run();
            }
            else if (pResponse == _responseTypeViewOrders || pResponse == _responseTypeViewTables)
            {
                ToggleViewMode();

                this.Run();
            }
        }

        private void tablePadPlace_Clicked(object sender, EventArgs e)
        {
            CustomButton button = (CustomButton)sender;

            _tablePadPlace.SelectedButtonOid = button.CurrentButtonId;

            switch (_currentViewMode)
            {
                case TableViewMode.Tables:

                    _tablePadTable.Filter = GetTablePadTableFilter((ResponseType)_currentTableStatusId);

                    if (_FilterMode == TableFilterMode.OnlyFreeTables) _tablePadTable.Filter = string.Format("{0} AND (TableStatus = {1} OR TableStatus IS NULL)", _tablePadTable.Filter, (int)TableStatus.Free);
                    break;
                case TableViewMode.Orders:
                    _tablePadOrder.Filter = GetTablePadPlaceFilter();
                    break;
                default:
                    break;
            }
        }

        private void tablePadTable_Clicked(object sender, EventArgs e)
        {
            TableButton button = (TableButton)sender;

            _tablePadTable.SelectedButtonOid = button.CurrentButtonId;
           
            _currentTableButtonOid = button.CurrentButtonId;
            
            _currentButton = button;

            if (button.TableSettings.TableStatus != TableStatus.Open)
            {
                _buttonTableReservation.Sensitive = true;
            }
            else
            {
                _buttonTableReservation.Sensitive = false;
            }

            if (button.TableSettings.TableStatus == TableStatus.Reserved)
            {
                _buttonOk.Sensitive = false;
            }
            else if (_tablePadTable.SelectedButtonOid != null)
            {
                _buttonOk.Sensitive = true;
            }
        }

        private void _tablePadOrder_Clicked(object sender, EventArgs e)
        {
        }

        private string GetTablePadPlaceFilter()
        {
            return string.Format("AND (Place = '{0}')", _tablePadPlace.SelectedButtonOid);
        }

        private string GetTablePadTableFilter(ResponseType pResponse)
        {
            string filter = GetTablePadPlaceFilter();
            if (pResponse < (ResponseType)_tablesStatusShowAllIndex)
            {
                if (pResponse == (ResponseType)TableStatus.Free)
                {
                    filter += string.Format(" AND (TableStatus = {0} OR TableStatus IS NULL)", pResponse);
                }
                else
                {
                    filter += string.Format(" AND (TableStatus = {0})", pResponse);
                }
            };
            return filter;
        }

        private void ToggleTableStatusButtons(ResponseType pResponse)
        {
            if (!_buttonTableFilterFree.Sensitive) _buttonTableFilterFree.Sensitive = true;
            if (!_buttonTableFilterOpen.Sensitive) _buttonTableFilterOpen.Sensitive = true;
            if (!_buttonTableFilterReserved.Sensitive) _buttonTableFilterReserved.Sensitive = true;
            if (!_buttonTableFilterAll.Sensitive) _buttonTableFilterAll.Sensitive = true;

            switch ((TableStatus)pResponse)
            {
                case TableStatus.Free:
                    _buttonTableFilterFree.Sensitive = false;
                    break;
                case TableStatus.Open:
                    _buttonTableFilterOpen.Sensitive = false;
                    break;
                case TableStatus.Reserved:
                    _buttonTableFilterReserved.Sensitive = false;
                    break;
                default:
                    _buttonTableFilterAll.Sensitive = false;
                    break;
            }
        }

        private void ToggleViewMode()
        {
            switch (_currentViewMode)
            {
                case TableViewMode.Orders:
                    WindowSettings.WindowTitle.Text = LocalizedString.Instance[string.Format("window_title_dialog_tables_appmode_{0}", AppOperationModeSettings.CustomAppOperationMode.AppOperationTheme).ToLower()];
                    _currentViewMode = TableViewMode.Tables;
                    _buttonTableViewOrders.Visible = true;
                    _buttonTableViewTables.Visible = false;
                    _tablePadOrder.Visible = false;
                    _tablePadTable.Visible = true;
                    _hboxTableScrollers.Visible = true;
                    _hboxOrderScrollers.Visible = false;
        
                    _buttonTableFilterAll.Visible = true;
                    _buttonTableFilterFree.Visible = true;
                    _buttonTableFilterOpen.Visible = true;
                    _buttonTableFilterReserved.Visible = true;
                    
                    _tablePadPlace.Sql = _sqlPlaceBaseTable;
                    break;
                case TableViewMode.Tables:
                    WindowSettings.WindowTitle.Text = LocalizedString.Instance["window_title_dialog_orders"];
                    _currentViewMode = TableViewMode.Orders;
                    _buttonTableViewOrders.Visible = false;
                    _buttonTableViewTables.Visible = true;
                    _tablePadOrder.Visible = true;
                    _tablePadTable.Visible = false;
                    _hboxTableScrollers.Visible = false;
                    _hboxOrderScrollers.Visible = true;
                    _buttonTableFilterAll.Visible = false;
                    _buttonTableFilterFree.Visible = false;
                    _buttonTableFilterOpen.Visible = false;
                    _buttonTableFilterReserved.Visible = false;
                    _tablePadPlace.Sql = _sqlPlaceBaseOrder;
                    break;
                default:
                    break;
            }
        }
    }
}
