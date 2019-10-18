using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using logicpos.datalayer.Enums;
using logicpos.shared.Enums;
using System;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosTablesDialog
    {
        //Override Responses
        protected override void OnResponse(ResponseType pResponse)
        {
            //No Modal Response Buttons Range
            if (pResponse >= (ResponseType)0 && pResponse <= (ResponseType)_tablesStatusShowAllIndex)
            {
                ToggleTableStatusButtons(pResponse);
                //Assign Response to _currentTableStatusId
                _currentTableStatusId = (int)pResponse;
                //Filter TablePad
                _tablePadTable.Filter = GetTablePadTableFilter(pResponse);
                //Keep Running
                this.Run();
            }
            else if (pResponse == _responseTypeTableReservation)
            {
                if (_currentButton.TableStatus == TableStatus.Free)
                {
                    _currentButton.ChangeTableStatus(_currentTableButtonOid, TableStatus.Reserved);
                }
                else
                {
                    _currentButton.ChangeTableStatus(_currentTableButtonOid, TableStatus.Free);
                }
                //if in Filter mode Update TablePad Sql
                if (!_buttonTableFilterFree.Sensitive || !_buttonTableFilterReserved.Sensitive) _tablePadTable.UpdateSql();
                //Better always disable buttons, to force user to select again
                _buttonOk.Sensitive = false;
                _buttonTableReservation.Sensitive = false;

                //Keep Running
                this.Run();
            }
            else if (pResponse == _responseTypeViewOrders || pResponse == _responseTypeViewTables)
            {
                ToggleViewMode();
                //Keep Running
                this.Run();
            }
        }

        void tablePadPlace_Clicked(object sender, EventArgs e)
        {
            TouchButtonBase button = (TouchButtonBase)sender;
            //Assign CurrentId to TablePad.CurrentId, to Know last Clicked Button Id
            _tablePadPlace.SelectedButtonOid = button.CurrentButtonOid;

            switch (_currentViewMode)
            {
                case TableViewMode.Tables:
                    //TablePadTable Filter
                    _tablePadTable.Filter = GetTablePadTableFilter((ResponseType)_currentTableStatusId);
                    //if in FilterMode OnlyFreeTables add TableStatus Filter
                    if (_FilterMode == TableFilterMode.OnlyFreeTables) _tablePadTable.Filter = string.Format("{0} AND (TableStatus = {1} OR TableStatus IS NULL)", _tablePadTable.Filter, (int)TableStatus.Free);
                    break;
                case TableViewMode.Orders:
                    //TablePadOrder Filter
                    _tablePadOrder.Filter = GetTablePadPlaceFilter();
                    break;
                default:
                    break;
            }
        }

        void tablePadTable_Clicked(object sender, EventArgs e)
        {
            TouchButtonTable button = (TouchButtonTable)sender;

            //Assign CurrentId to TablePad.CurrentId, to Know last Clicked Button Id
            _tablePadTable.SelectedButtonOid = button.CurrentButtonOid;
            //To be Used in Dialog Result
            _currentTableButtonOid = button.CurrentButtonOid;
            //Assign current clicked Reference button to CurrentButton
            _currentButton = button;

            //Update Reservation Button
            if (button.TableStatus != TableStatus.Open)
            {
                _buttonTableReservation.Sensitive = true;
            }
            else
            {
                _buttonTableReservation.Sensitive = false;
            }

            //Update buttonOk
            if (button.TableStatus == TableStatus.Reserved)
            {
                _buttonOk.Sensitive = false;
            }
            else if (_tablePadTable.SelectedButtonOid != null)
            {
                _buttonOk.Sensitive = true;
            }
        }

        void _tablePadOrder_Clicked(object sender, EventArgs e)
        {
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

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
                    /* IN008024 */
                    _labelWindowTitle.Text = resources.CustomResources.GetCustomResources("", string.Format("window_title_dialog_tables_appmode_{0}", logicpos.App.SettingsApp.CustomAppOperationMode.AppOperationTheme).ToLower());
                    //Tables
                    _currentViewMode = TableViewMode.Tables;
                    _buttonTableViewOrders.Visible = true;
                    _buttonTableViewTables.Visible = false;
                    _tablePadOrder.Visible = false;
                    _tablePadTable.Visible = true;
                    _hboxTableScrollers.Visible = true;
                    _hboxOrderScrollers.Visible = false;
                    //Orders ActionArea
                    //_buttonOrderChangeTable.Visible = false;
                    //Tables ActionArea
                    _buttonTableFilterAll.Visible = true;
                    _buttonTableFilterFree.Visible = true;
                    _buttonTableFilterOpen.Visible = true;
                    _buttonTableFilterReserved.Visible = true;
                    //Change Place Sql, to Reflect Childs Button State
                    _tablePadPlace.Sql = _sqlPlaceBaseTable;
                    break;
                case TableViewMode.Tables:
                    _labelWindowTitle.Text = resources.CustomResources.GetCustomResources("", "window_title_dialog_orders");
                    //Orders
                    _currentViewMode = TableViewMode.Orders;
                    _buttonTableViewOrders.Visible = false;
                    _buttonTableViewTables.Visible = true;
                    _tablePadOrder.Visible = true;
                    _tablePadTable.Visible = false;
                    _hboxTableScrollers.Visible = false;
                    _hboxOrderScrollers.Visible = true;
                    //Orders ActionArea
                    //_buttonOrderChangeTable.Visible = true;
                    //Tables ActionArea
                    _buttonTableFilterAll.Visible = false;
                    _buttonTableFilterFree.Visible = false;
                    _buttonTableFilterOpen.Visible = false;
                    _buttonTableFilterReserved.Visible = false;
                    //Change Place Sql, to Reflect Childs Button State
                    _tablePadPlace.Sql = _sqlPlaceBaseOrder;
                    break;
                default:
                    break;
            }
        }
    }
}
