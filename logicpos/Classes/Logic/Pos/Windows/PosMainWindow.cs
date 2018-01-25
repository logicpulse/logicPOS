using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.Classes.Hardware.Printers;
using logicpos.financial.library.Classes.WorkSession;
using logicpos.Classes.Enums.TicketList;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using logicpos.shared.Classes.Orders;
using System;

namespace logicpos
{
    public partial class PosMainWindow
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: Window

        private void PosMainWindow_WindowStateEvent(object o, WindowStateEventArgs args)
        {
            // Activate Pos Window, ex Activated From BackOffice
            if (args.Event.NewWindowState == Gdk.WindowState.Fullscreen)
            {
                //Always Reset tablePadFamily before Refresh, to Prevent Keep old selected when we delete articles and create a new Familiy 
                //This way we always have the first family when we come from backoffice
                _tablePadFamily.SelectedButtonOid = Guid.Empty;
                _tablePadFamily.Refresh();
                //Always Apply Filter to prevent error when work in a clean database, and we create first Records
                //Filter is "  AND (Family = '00000000-0000-0000-0000-000000000000')"
                _tablePadSubFamily.Filter = String.Format("  AND (Family = '{0}')", _tablePadFamily.SelectedButtonOid);
                _tablePadSubFamily.Refresh();
                _tablePadArticle.Refresh();
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: KeyRelease

        void PosMainWindow_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            //Redirect Event to BarCodeReader.KeyReleaseEvent
            GlobalApp.HWBarCodeReader.KeyReleaseEvent(this, o, args);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarApplicationClose

        void touchButtonPosToolbarApplicationClose_Clicked(object sender, EventArgs e)
        {
            LogicPos.Quit(this);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarBackOffice

        void touchButtonPosToolbarBackOffice_Clicked(object sender, EventArgs e)
        {
            Utils.ShowBackOffice(this);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarReports

        // Deprecated
        void touchButtonPosToolbarReports_Clicked(object sender, EventArgs e)
        {
            //Temporary Disable
            //Utils.ShowReportsWinForms(this);

            //Old GTK Report Window, Not Used Anymore
            //Utils.ShowReports(this);

            //Test Report With DocumentFinanceMaster Documents
            //Guid documentFinanceMasterOid = new Guid("1d062c73-ca1d-434d-a95c-60d09bece5cd");
            //CustomReport.ProcessReportFinanceDocument(CustomReportDisplayMode.Design, 4, documentFinanceMasterOid, "aQpO");

            //Test Report With DocumentFinancePayment Documents
            //Guid documentFinancePaymentOid = new Guid("88f082ce-da52-48b5-a31d-32bfa87f119d");
            //CustomReport.ProcessReportFinanceDocumentPayment(CustomReportDisplayMode.Design, 4, documentFinancePaymentOid);

            //Test Printer
            //ConfigurationPrintersTemplates configurationPrintersTemplates = (ConfigurationPrintersTemplates)FrameworkUtils.GetXPGuidObjectFromSession(typeof(ConfigurationPrintersTemplates), new Guid("5409255A-3741-411C-B05B-056CBD470226"));
            //DocumentFinancePayment documentFinancePayment = (DocumentFinancePayment)FrameworkUtils.GetXPGuidObjectFromSession(typeof(DocumentFinancePayment), new Guid("88F082CE-DA52-48B5-A31D-32BFA87F119D"));
            //FrameworkCalls.PrintFinanceDocumentPayment(this, GlobalFramework.LoggedTerminal.Printer, configurationPrintersTemplates, documentFinancePayment);

            //Test WorkSession
            //PrintTicket.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, GlobalFramework.WorkSessionPeriodTerminal);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarLogoutUser

        void touchButtonPosToolbarLogoutUser_Clicked(object sender, EventArgs e)
        {
            Hide();
            //Call Shared WindowStartup LogOutUser, and Show WindowStartup
            GlobalApp.WindowStartup.LogOutUser(true);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarShowSystemDialog

        // System Dialog
        void touchButtonPosToolbarShowSystemDialog_Clicked(object sender, EventArgs e)
        {
            PosSystemDialog dialog = new PosSystemDialog(this, Gtk.DialogFlags.DestroyWithParent);
            int response = dialog.Run();
            dialog.Destroy();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarCashDrawer

        void touchButtonPosToolbarCashDrawer_Clicked(object sender, EventArgs e)
        {
            ShowCashDialog();
        }

        void ShowCashDialog()
        {
            PosCashDialog dialog = new PosCashDialog(this, Gtk.DialogFlags.DestroyWithParent);
            int response = dialog.Run();
            dialog.Destroy();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarNewFinanceDocument

        void touchButtonPosToolbarNewFinanceDocument_Clicked(object sender, EventArgs e)
        {
            //Call New DocumentFinance Dialog
            PosDocumentFinanceDialog dialogNewDocument = new PosDocumentFinanceDialog(this, DialogFlags.DestroyWithParent);
            ResponseType responseNewDocument = (ResponseType)dialogNewDocument.Run();
            if (responseNewDocument == ResponseType.Ok)
            {
            }
            dialogNewDocument.Destroy();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarFinanceDocuments

        void touchButtonPosToolbarFinanceDocuments_Clicked(object sender, EventArgs e)
        {
            PosDocumentFinanceSelectRecordDialog dialog = new PosDocumentFinanceSelectRecordDialog(this, Gtk.DialogFlags.DestroyWithParent);
            ResponseType response = (ResponseType)dialog.Run();
            dialog.Destroy();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarShowChangeUser

        void touchButtonPosToolbarShowChangeUserDialog_Clicked(object sender, EventArgs e)
        {
            PosChangeUserDialog dialogChangeUser = new PosChangeUserDialog(this, Gtk.DialogFlags.DestroyWithParent);

            try
            {
                string terminalInfo = string.Empty;

                int responseChangeUser = dialogChangeUser.Run();
                if (responseChangeUser == (int)ResponseType.Ok)
                {
                    //Already logged
                    if (GlobalFramework.SessionApp.LoggedUsers.ContainsKey(dialogChangeUser.UserDetail.Oid))
                    {
                        GlobalFramework.LoggedUser = (SYS_UserDetail)FrameworkUtils.GetXPGuidObject(typeof(SYS_UserDetail), dialogChangeUser.UserDetail.Oid);
                        GlobalFramework.LoggedUserPermissions = FrameworkUtils.GetUserPermissions();
                        _ticketList.UpdateTicketListButtons();
                        FrameworkUtils.Audit("USER_CHANGE", string.Format(Resx.audit_message_user_change, GlobalFramework.LoggedUser.Name));
                        terminalInfo = string.Format("{0} : {1}", GlobalFramework.LoggedTerminal.Designation, GlobalFramework.LoggedUser.Name);
                        if (_labelTerminalInfo.Text != terminalInfo) _labelTerminalInfo.Text = terminalInfo;
                    }
                    //Not Logged, Request Pin Login
                    else
                    {
                        PosPinPadDialog dialogPinPad = new PosPinPadDialog(dialogChangeUser, Gtk.DialogFlags.DestroyWithParent, dialogChangeUser.UserDetail);
                        int responsePinPad = dialogPinPad.Run();
                        if (responsePinPad == (int)ResponseType.Ok)
                        {
                            if (!GlobalFramework.SessionApp.LoggedUsers.ContainsKey(dialogChangeUser.UserDetail.Oid))
                            {
                                GlobalFramework.SessionApp.LoggedUsers.Add(dialogChangeUser.UserDetail.Oid, FrameworkUtils.CurrentDateTimeAtomic());
                                GlobalFramework.SessionApp.Write();
                                GlobalFramework.LoggedUser = (SYS_UserDetail)FrameworkUtils.GetXPGuidObject(typeof(SYS_UserDetail), dialogChangeUser.UserDetail.Oid);
                                GlobalFramework.LoggedUserPermissions = FrameworkUtils.GetUserPermissions();
                                _ticketList.UpdateTicketListButtons();
                                FrameworkUtils.Audit("USER_LOGIN", string.Format(Resx.audit_message_user_login, GlobalFramework.LoggedUser.Name));
                                terminalInfo = string.Format("{0} : {1}", GlobalFramework.LoggedTerminal.Designation, GlobalFramework.LoggedUser.Name);
                                if (_labelTerminalInfo.Text != terminalInfo) _labelTerminalInfo.Text = terminalInfo;
                                //After First time Login ShowNotifications
                                Utils.ShowNotifications(dialogPinPad);
                            }
                        };

                        dialogPinPad.Destroy();
                    }
                };
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            finally
            {
                dialogChangeUser.Destroy();
            }

        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: Favorites Button

        void buttonFavorites_Clicked(object sender, EventArgs e)
        {
            _tablePadArticle.Filter = " AND (Favorite = 1)";
            //Enable Buttons, else when we have only a Family or Subfamily, the buttons will never be enabled
            if (_tablePadFamily.SelectedButton != null && !_tablePadFamily.SelectedButton.Sensitive) _tablePadFamily.SelectedButton.Sensitive = true;
            if (_tablePadSubFamily.SelectedButton != null && !_tablePadSubFamily.SelectedButton.Sensitive) _tablePadSubFamily.SelectedButton.Sensitive = true;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: Families Button Clicks

        void _tablePadFamily_Clicked(object sender, EventArgs e)
        {
            TouchButtonBase button = (TouchButtonBase)sender;

            //Assign CurrentId to TablePad.CurrentId, to Know last Clicked Button Id
            _tablePadFamily.SelectedButtonOid = button.CurrentButtonOid;
            //SubFamily Filter
            _tablePadSubFamily.Filter = string.Format(" AND (Family = '{0}')", button.CurrentButtonOid);
            //Article Filter : When Change Family always change Article too
            TablePadArticle.Filter = " AND (SubFamily = '" + _tablePadSubFamily.SelectedButtonOid + "')";

            //Debug
            //_log.Debug(string.Format("_tablePadFamily_Clicked(): F:CurrentId: [{0}], Name: [{1}]", button.CurrentId, button.Name));
            //_log.Debug(string.Format("_tablePadFamily_Clicked(): SubFamily.Sql:[{0}{1}{2}]", _tablePadSubFamily.Sql, _tablePadSubFamily.Filter, _tablePadSubFamily.Order));
            //_log.Debug(string.Format("_tablePadFamily_Clicked(): Article.Sql:[{0}{1}{2}]", _tablePadArticle.Sql, _tablePadArticle.Filter, _tablePadArticle.Order));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: SubFamilies Button Clicks

        void _tablePadSubFamily_Clicked(object sender, EventArgs e)
        {
            TouchButtonBase button = (TouchButtonBase)sender;

            //Assign CurrentId to TablePad.CurrentId, to Know last Clicked Button Id
            _tablePadSubFamily.SelectedButtonOid = button.CurrentButtonOid;

            //Article Filter
            _tablePadArticle.Filter = string.Format(" AND (SubFamily = '{0}')", button.CurrentButtonOid);

            //Debug
            //_log.Debug(string.Format("_tablePadSubFamily_Clicked(): S:CurrentId:[{0}], Name:[{1}]", button.CurrentId, button.Name));
            //_log.Debug(string.Format("_tablePadSubFamily_Clicked(): Article.Sql:[{0}{1}{2}]", _tablePadArticle.Sql, _tablePadArticle.Filter, _tablePadArticle.Order));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: Articles Button Clicks

        void _tablePadArticle_Clicked(object sender, EventArgs e)
        {
            TouchButtonBase button = (TouchButtonBase)sender;
            //_log.Debug(string.Format("_tablePadArticle_Clicked(): A:CurrentId:[{0}], Name:[{1}]", button.CurrentButtonOid, button.Name));

            //Change Mode
            if (_ticketList.ListMode != TicketListMode.Ticket)
            {
                _ticketList.ListMode = TicketListMode.Ticket;
                _ticketList.UpdateModel();
            }

            //Assign CurrentId to TablePad.CurrentId, to Know last Clicked Button Id
            _tablePadArticle.SelectedButtonOid = button.CurrentButtonOid;

            //Send to TicketList
            _ticketList.InsertOrUpdate(button.CurrentButtonOid);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: Open Cash Drawer

        private void eventBoxImageLogo_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            if (GlobalFramework.LoggedTerminal.Printer != null && GlobalFramework.LoggedTerminal.Printer.PrinterType.ThermalPrinter)
            {
                PrintRouter.OpenDoor(GlobalFramework.LoggedTerminal.Printer);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: BarCodeReader

        private void HWBarCodeReader_Captured(object sender, EventArgs e)
        {
            //_log.Debug(String.Format("Window: [{0}] Device: [{1}] Captured: [{2}] Length: [{3}]", GlobalApp.HWBarCodeReader.Window, GlobalApp.HWBarCodeReader.Device, GlobalApp.HWBarCodeReader.Buffer, GlobalApp.HWBarCodeReader.Buffer.Length));

            switch (GlobalApp.HWBarCodeReader.Device)
            {
                case logicpos.Classes.Logic.Hardware.InputReaderDevice.None:
                    break;
                case logicpos.Classes.Logic.Hardware.InputReaderDevice.BarCodeReader:
                    TicketList.InsertOrUpdate(GlobalApp.HWBarCodeReader.Buffer);
                    break;
                case logicpos.Classes.Logic.Hardware.InputReaderDevice.CardReader:
                    break;
                default:
                    break;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Public Methods

        //Update UI, Required to Reflect BO or Outside Changes, Like Database Changes
        public void UpdateUI()
        {
            LabelTerminalInfo.Text = string.Format("{0} : {1}", GlobalFramework.LoggedTerminal.Designation, GlobalFramework.LoggedUser.Name);
            TablePadFamily.UpdateSql();
            TablePadSubFamily.UpdateSql();
            TablePadArticle.UpdateSql();

            _ticketList.UpdateTicketListButtons();
        }

        public void UpdateWorkSessionUI()
        {
            //Update Toolbar UI Buttons After ToolBox and ToolBar
            if (GlobalFramework.WorkSessionPeriodDay != null)
            {
                //With Valid WorkSessionPeriodDay
                if (GlobalFramework.WorkSessionPeriodDay.SessionStatus == WorkSessionPeriodStatus.Open)
                {
                    //if (_touchButtonPosToolbarCashDrawer.LabelText != Resx.global_worksession_close_day)
                    //  _touchButtonPosToolbarCashDrawer.LabelText = Resx.global_worksession_close_day;
                    //if (!_touchButtonPosToolbarCashDrawer.Sensitive == true)
                    //  _touchButtonPosToolbarCashDrawer.Sensitive = true;

                    //With Valid WorkSessionPeriodTerminal
                    if (GlobalFramework.WorkSessionPeriodTerminal != null && GlobalFramework.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open)
                    {
                        if (GlobalFramework.SessionApp.OrdersMain.ContainsKey(GlobalFramework.SessionApp.CurrentOrderMainOid)
                          && GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid].Table != null)
                            _tablePadArticle.Sensitive = true;
                        if (!_ticketPad.Sensitive == true)
                            _ticketPad.Sensitive = true;
                    }
                    //With No WorkSessionPeriodTerminal
                    else
                    {
                        if (!_ticketPad.Sensitive == false)
                            _ticketPad.Sensitive = false;
                        if (!_tablePadArticle.Sensitive == false)
                            _tablePadArticle.Sensitive = false;
                    }
                }
            }
            //No WorkSessionPeriodDay
            else
            {
                //if (_touchButtonPosToolbarCashDrawer.LabelText != Resx.global_worksession_open_day)
                //  _touchButtonPosToolbarCashDrawer.LabelText = Resx.global_worksession_open_day;
                //if (!_touchButtonPosToolbarCashDrawer.Sensitive == false)
                //  _touchButtonPosToolbarCashDrawer.Sensitive = false;
                if (!_ticketPad.Sensitive == false)
                    _ticketPad.Sensitive = false;
                if (!_tablePadArticle.Sensitive == false)
                    _tablePadArticle.Sensitive = false;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Timer/Clock/UpdateUI

        private void StartClock()
        {
            // Every second call `update_status' (1000 milliseconds)
            GLib.Timeout.Add(1000, new GLib.TimeoutHandler(UpdateClock));
        }

        private bool UpdateClock()
        {
            if (GlobalApp.WindowPos.Visible)
            {
                _labelClock.Text = FrameworkUtils.CurrentDateTime(_clockFormat);

                //Call Current OrderMain Update Status
                if (GlobalFramework.SessionApp.CurrentOrderMainOid != Guid.Empty && GlobalFramework.SessionApp.OrdersMain.ContainsKey(GlobalFramework.SessionApp.CurrentOrderMainOid))
                {
                    UpdateGUITimer(GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid], _ticketList);
                }

                //Update UI Button and Get WorkSessionPeriodDay if is Opened by Other Terminal
                if (GlobalFramework.WorkSessionPeriodTerminal == null
                  || (GlobalFramework.WorkSessionPeriodTerminal != null && GlobalFramework.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Close))
                {
                    POS_WorkSessionPeriod workSessionPeriodDay = ProcessWorkSessionPeriod.GetSessionPeriod(WorkSessionPeriodType.Day);

                    if (workSessionPeriodDay == null)
                    {
                        GlobalFramework.WorkSessionPeriodDay = null;
                        UpdateWorkSessionUI();
                    }
                    else
                    {
                        if (workSessionPeriodDay.SessionStatus == WorkSessionPeriodStatus.Open)
                        {
                            GlobalFramework.WorkSessionPeriodDay = workSessionPeriodDay;
                            UpdateWorkSessionUI();
                        }
                    }
                }
            }
            // returning true means that the timeout routine should be invoked
            // again after the timeout period expires. Returning false would
            // terminate the timeout.
            return true;
        }

        private void UpdateGUITimer(OrderMain orderMain, TicketList pTicketList)
        {
            bool debug = false;

            string sqlOrderMainUpdatedAt = string.Format("SELECT UpdatedAt FROM fin_documentordermain WHERE (PlaceTable = '{0}' AND OrderStatus = {1}) ORDER BY UpdatedAt DESC", orderMain.Table.Oid, Convert.ToInt16(OrderStatus.Open));
            var oResultUpdatedAt = GlobalFramework.SessionXpo.ExecuteScalar(sqlOrderMainUpdatedAt);

            if (oResultUpdatedAt != null)
            {
                DateTime dateLastDBUpdate = Convert.ToDateTime(oResultUpdatedAt);

                if (orderMain.UpdatedAt < dateLastDBUpdate)
                {
                    orderMain.PersistentOid = orderMain.GetOpenTableFieldValueGuid(orderMain.Table.Oid, "Oid");
                    orderMain.OrderStatus = (OrderStatus)orderMain.GetOpenTableFieldValue(orderMain.Table.Oid, "OrderStatus");
                    orderMain.UpdatedAt = dateLastDBUpdate;

                    //If is in OrderMain List Mode Update Model
                    if (pTicketList.ListMode == TicketListMode.OrderMain) pTicketList.UpdateModel();

                    pTicketList.UpdateOrderStatusBar();
                    pTicketList.UpdateTicketListOrderButtons();

                    //Debug
                    if (debug) _log.Debug(string.Format("UpdateGUITimer(): Table Status Updated [{0}], _persistentOid [{1}], _orderStatus [{2}], _UpdatedAt [{3}], dateLastDBUpdate [{4}]", orderMain.Table.Name, orderMain.PersistentOid, Convert.ToInt16(OrderStatus.Open), orderMain.UpdatedAt, dateLastDBUpdate));

                    GlobalFramework.SessionApp.Write();
                }
            }
            //Cant Get Table Open Status
            else
            {
                orderMain.PersistentOid = orderMain.GetOpenTableFieldValueGuid(orderMain.Table.Oid, "Oid");
                orderMain.OrderStatus = (OrderStatus)orderMain.GetOpenTableFieldValue(orderMain.Table.Oid, "OrderStatus");

                //If is in OrderMain List Mode Update Model
                if (pTicketList.ListMode == TicketListMode.OrderMain) pTicketList.UpdateModel();

                pTicketList.UpdateOrderStatusBar();
                pTicketList.UpdateTicketListOrderButtons();

                GlobalFramework.SessionApp.Write();

                //Debug
                //if (debug) _log.Debug(string.Format("UpdateGUITimer(): Cant Get Table Status [{0}], sql:[{1}]", orderMain.Table.Name, sqlOrderMainUpdatedAt));
            }
        }

        //UpdateUI if detect open Orders
        private void UpdateUIIfHasWorkingOrder()
        {
            if (GlobalFramework.SessionApp.OrdersMain.ContainsKey(GlobalFramework.SessionApp.CurrentOrderMainOid)
              && GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid].Table != null)
            {
                //Update Order Summary Status Bar
                _ticketList.UpdateOrderStatusBar();
            }
            else
            {
                //Force Start With Default Table
                _ticketPad.SelectTableOrder(SettingsApp.XpoOidConfigurationPlaceTableDefaultOpenTable);
                _ticketList.UpdateArticleBag();
                _ticketList.UpdateTicketListOrderButtons();
                _ticketList.UpdateOrderStatusBar();
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        // Automatically scroll to bottom of _textviewLog
        private void ScrollTextViewLog(object o, SizeAllocatedArgs args)
        {
            _textviewLog.ScrollToIter(_textviewLog.Buffer.EndIter, 0, false, 0, 0);
        }
    }
}
