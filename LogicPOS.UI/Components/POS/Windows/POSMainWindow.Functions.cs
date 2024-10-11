using DevExpress.Xpo.DB;
using Gtk;
using logicpos;
using logicpos.App;
using logicpos.Classes.Enums.Hardware;
using logicpos.Classes.Enums.TicketList;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Settings;
using LogicPOS.Shared;
using LogicPOS.Shared.Orders;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.Modals;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Windows
{
    public partial class POSMainWindow
    {
        private Guid TableId = Guid.Empty;

        private void PosMainWindow_WindowStateEvent(object o, WindowStateEventArgs args)
        {
            if (args.Event.NewWindowState == Gdk.WindowState.Fullscreen)
            {
                MenuFamilies.SelectedFamily = null;
                MenuFamilies.Refresh();
                MenuSubfamilies.Refresh();
                MenuArticles.Refresh();
            }
        }

        private void PosMainWindow_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (GlobalApp.BarCodeReader != null)
            {
                GlobalApp.BarCodeReader.KeyReleaseEvent(this, o, args);
            }
        }

        private void BtnQuit_Clicked(object sender, EventArgs e)
        {
            LogicPOSApp.Quit(this);
        }

        private void BtnBackOffice_Clicked(object sender, EventArgs e)
        {
            Utils.ShowBackOffice(this);
        }

        private void BtnReports_Clicked(object sender, EventArgs e)
        {
            PosReportsDialog dialog = new PosReportsDialog(this, DialogFlags.DestroyWithParent);
            int response = dialog.Run();
            dialog.Destroy();
        }

        private void BtnLogOut_Clicked(object sender, EventArgs e)
        {
            Hide();
            GlobalApp.StartupWindow.LogOutUser(true);
        }

        private void BtnCashDrawer_Clicked(object sender, EventArgs e)
        {
            ShowCashDialog();
        }

        private void ShowCashDialog()
        {
            PosCashDialog dialog = new PosCashDialog(this, DialogFlags.DestroyWithParent);
            int response = dialog.Run();
            dialog.Destroy();
        }

        private void BtnNewDocument_Clicked(object sender, EventArgs e)
        {
            var modal = new CreateDocumentModal(this);
            modal.Run();
            modal.Destroy();

            PosDocumentFinanceDialog createDocumentModal = new PosDocumentFinanceDialog(this, DialogFlags.DestroyWithParent);
            ResponseType response = (ResponseType)createDocumentModal.Run();
            createDocumentModal.Destroy();
        }

        private void BtnDocuments_Clicked(object sender, EventArgs e)
        {
            var documentsMenu = new DocumentsMenuModal(this);
            documentsMenu.Run();
            documentsMenu.Destroy();
        }

        private void BtnChangeUser_Clicked(object sender, EventArgs e)
        {
            PosChangeUserDialog dialogChangeUser = new PosChangeUserDialog(this, DialogFlags.DestroyWithParent);


            string terminalInfo = string.Empty;

            int responseChangeUser = dialogChangeUser.Run();
            if (responseChangeUser == (int)ResponseType.Ok)
            {
                if (POSSession.CurrentSession.LoggedUsers.ContainsKey(dialogChangeUser.UserDetail.Oid))
                {
                    XPOSettings.LoggedUser = XPOUtility.GetEntityById<sys_userdetail>(dialogChangeUser.UserDetail.Oid);
                    GeneralSettings.LoggedUserPermissions = XPOUtility.GetUserPermissions();
                    TicketList.UpdateSaleOptionsPanelButtons();
                    XPOUtility.Audit("USER_CHANGE", string.Format(GeneralUtils.GetResourceByName("audit_message_user_change"), XPOSettings.LoggedUser.Name));
                    terminalInfo = string.Format("{0} : {1}", TerminalSettings.LoggedTerminal.Designation, XPOSettings.LoggedUser.Name);
                    if (LabelTerminalInfo.Text != terminalInfo) LabelTerminalInfo.Text = terminalInfo;
                }
                else
                {
                    PosPinPadDialog dialogPinPad = new PosPinPadDialog(dialogChangeUser, DialogFlags.DestroyWithParent, dialogChangeUser.UserDetail);
                    int responsePinPad = dialogPinPad.Run();
                    if (responsePinPad == (int)ResponseType.Ok)
                    {
                        if (!POSSession.CurrentSession.LoggedUsers.ContainsKey(dialogChangeUser.UserDetail.Oid))
                        {
                            POSSession.CurrentSession.LoggedUsers.Add(dialogChangeUser.UserDetail.Oid, XPOUtility.CurrentDateTimeAtomic());
                            POSSession.CurrentSession.Save();
                            XPOSettings.LoggedUser = XPOUtility.GetEntityById<sys_userdetail>(dialogChangeUser.UserDetail.Oid);
                            GeneralSettings.LoggedUserPermissions = XPOUtility.GetUserPermissions();
                            TicketList.UpdateSaleOptionsPanelButtons();
                            XPOUtility.Audit("USER_LOGIN", string.Format(GeneralUtils.GetResourceByName("audit_message_user_login"), XPOSettings.LoggedUser.Name));
                            terminalInfo = string.Format("{0} : {1}", TerminalSettings.LoggedTerminal.Designation, XPOSettings.LoggedUser.Name);
                            if (LabelTerminalInfo.Text != terminalInfo) LabelTerminalInfo.Text = terminalInfo;
                            Utils.ShowNotifications(dialogPinPad);
                        }
                    };

                    dialogPinPad.Destroy();
                }
            };


            dialogChangeUser.Destroy();

        }

        private void ButtonFavorites_Clicked(object sender, EventArgs e)
        {
            if (MenuFamilies.SelectedButton != null && !MenuFamilies.SelectedButton.Sensitive) MenuFamilies.SelectedButton.Sensitive = true;
            if (MenuSubfamilies.SelectedButton != null && !MenuSubfamilies.SelectedButton.Sensitive) MenuSubfamilies.SelectedButton.Sensitive = true;
        }

        private void ImageLogo_Clicked(object o, ButtonPressEventArgs args)
        {
            if (TerminalSettings.HasLoggedTerminal)
            {
                PosPinPadDialog dialogPinPad = new PosPinPadDialog(this, DialogFlags.Modal, XPOSettings.LoggedUser, true);
                int responsePinPad = dialogPinPad.Run();
                if (responsePinPad == (int)ResponseType.Ok)
                {
                    var resultOpenDoor = LogicPOS.Printing.Utility.PrintingUtils.OpenDoor();
                    if (!resultOpenDoor)
                    {
                        Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, GeneralUtils.GetResourceByName("global_information"), string.Format(GeneralUtils.GetResourceByName("open_cash_draw_permissions")));
                    }
                    else
                    {
                        XPOUtility.Audit("CASHDRAWER_OUT", string.Format(
                             GeneralUtils.GetResourceByName("audit_message_cashdrawer_out"),
                             TerminalSettings.LoggedTerminal.Designation,
                             "Button Open Door"));
                    }

                };

                dialogPinPad.Destroy();


            }
        }

        private void HWBarCodeReader_Captured(object sender, EventArgs e)
        {
            if (GlobalApp.PosMainWindow.TicketList.CurrentOrderDetail != null)
            {
                switch (GlobalApp.BarCodeReader.Device)
                {
                    case InputReaderDevice.None:
                        break;
                    case InputReaderDevice.BarCodeReader:
                    case InputReaderDevice.CardReader:
                        if (GeneralSettings.AppUseParkingTicketModule)
                        {
                            GlobalApp.ParkingTicket.GetTicketDetailFromWS(GlobalApp.BarCodeReader.Buffer);
                        }
                        else
                        {
                            TicketList.InsertOrUpdate(GlobalApp.BarCodeReader.Buffer);
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        public void UpdateUI()
        {
            LabelTerminalInfo.Text = string.Format("{0} : {1}", TerminalSettings.LoggedTerminal.Designation, XPOSettings.LoggedUser.Name);
            MenuFamilies.LoadEntities();
            MenuSubfamilies.LoadEntities();
            MenuArticles.LoadEntities();

            TicketList.UpdateSaleOptionsPanelButtons();
        }

        public void UpdateWorkSessionUI()
        {
            if (XPOSettings.WorkSessionPeriodDay != null)
            {
                if (XPOSettings.WorkSessionPeriodDay.SessionStatus == WorkSessionPeriodStatus.Open)
                {
                    if (XPOSettings.WorkSessionPeriodTerminal != null && XPOSettings.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open)
                    {
                        bool isTableOpened = POSSession.CurrentSession.OrderMains.ContainsKey(POSSession.CurrentSession.CurrentOrderMainId)
                          && POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId].Table != null;
                        SelectedData xpoSelectedData = null;

                        if (DatabaseSettings.DatabaseType.ToString() == "MySql" || DatabaseSettings.DatabaseType.ToString() == "SQLite")
                        {
                            string sqlQuery = @"SELECT Oid FROM pos_configurationplacetable WHERE (Disabled IS NULL or Disabled  <> 1) ORDER BY Code asc LIMIT 1";

                            xpoSelectedData = XPOSettings.Session.ExecuteQueryWithMetadata(sqlQuery);
                        }
                        else if (DatabaseSettings.DatabaseType.ToString() == "MSSqlServer")
                        {
                            string sqlQuery = @"SELECT TOP 1 Oid FROM pos_configurationplacetable WHERE (Disabled IS NULL or Disabled  <> 1) ORDER BY Code asc";
                            xpoSelectedData = XPOSettings.Session.ExecuteQueryWithMetadata(sqlQuery);
                        }

                        SelectStatementResultRow[] selectStatementResultMeta = xpoSelectedData.ResultSet[0].Rows;
                        SelectStatementResultRow[] selectStatementResultData = xpoSelectedData.ResultSet[1].Rows;
                        if (!isTableOpened && !GeneralSettings.AppUseBackOfficeMode)
                        {

                            Guid currentTableOid = Guid.Parse(selectStatementResultData[0].Values[0].ToString());

                            Guid newOrderMainOid = Guid.NewGuid();
                            POSSession.CurrentSession.OrderMains.Add(newOrderMainOid, new OrderMain(newOrderMainOid, currentTableOid));
                            OrderMain newOrderMain = POSSession.CurrentSession.OrderMains[newOrderMainOid];
                            OrderTicket orderTicket = new OrderTicket(newOrderMain, (PriceType)newOrderMain.Table.PriceType);

                            newOrderMain.OrderTickets.Add(1, orderTicket);

                            OrderMain currentOrderMain = newOrderMain;

                            TicketList.UpdateArticleBag();
                            TicketList.UpdateSaleOptionsPanelOrderButtons();
                            TicketList.UpdateOrderStatusBar();

                            currentOrderMain.PersistentOid = currentOrderMain.GetOpenTableFieldValueGuid(TableId, "Oid");
                            currentOrderMain.OrderStatus = (OrderStatus)currentOrderMain.GetOpenTableFieldValue(TableId, "OrderStatus");

                            POSSession.CurrentSession.CurrentOrderMainId = currentOrderMain.Table.OrderMainOid;
                            POSSession.CurrentSession.Save();
                            TicketList.UpdateModel();

                            SaleOptionsPanel.Sensitive = true;
                        }
                        if (!GeneralSettings.AppUseBackOfficeMode)
                            MenuArticles.Sensitive = true;

                        if (!SaleOptionsPanel.Sensitive == true && !GeneralSettings.AppUseBackOfficeMode)
                            SaleOptionsPanel.Sensitive = true;
                    }

                    else if (!GeneralSettings.AppUseBackOfficeMode)
                    {
                        if (!SaleOptionsPanel.Sensitive == false)
                            SaleOptionsPanel.Sensitive = false;
                        if (!MenuArticles.Sensitive == false)
                            MenuArticles.Sensitive = false;
                    }
                }
            }
            else if (!GeneralSettings.AppUseBackOfficeMode)
            {
                if (!SaleOptionsPanel.Sensitive == false)
                    SaleOptionsPanel.Sensitive = false;
                if (!MenuArticles.Sensitive == false)
                    MenuArticles.Sensitive = false;
            }
        }

        private void StartClock()
        {
            GLib.Timeout.Add(1000, new GLib.TimeoutHandler(UpdateClock));
        }

        private bool UpdateClock()
        {
            if (GlobalApp.PosMainWindow.Visible)
            {
                LabelClock.Text = XPOUtility.CurrentDateTime(ClockTimeFormat);

                if (POSSession.CurrentSession.CurrentOrderMainId != Guid.Empty && POSSession.CurrentSession.OrderMains.ContainsKey(POSSession.CurrentSession.CurrentOrderMainId))
                {
                    UpdateGUITimer(POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId], TicketList);
                }

                if (XPOSettings.WorkSessionPeriodTerminal == null
                  || (XPOSettings.WorkSessionPeriodTerminal != null &&
                  XPOSettings.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Close))
                {
                    pos_worksessionperiod workSessionPeriodDay = WorkSessionProcessor.GetSessionPeriod(WorkSessionPeriodType.Day);

                    if (workSessionPeriodDay == null)
                    {
                        if (XPOSettings.WorkSessionPeriodDay != null)
                        {
                            XPOUtility.WorkSession.SaveCurrentWorkSessionPeriodDayDto();
                        }

                        XPOSettings.WorkSessionPeriodDay = null;
                        UpdateWorkSessionUI();
                    }
                    else
                    {
                        if (workSessionPeriodDay.SessionStatus == WorkSessionPeriodStatus.Open)
                        {
                            XPOSettings.WorkSessionPeriodDay = workSessionPeriodDay;
                            UpdateWorkSessionUI();
                        }
                    }
                }
            }

            return true;
        }

        private void UpdateGUITimer(OrderMain orderMain, TicketList pTicketList)
        {
            string sqlOrderMainUpdatedAt = string.Format("SELECT UpdatedAt FROM fin_documentordermain WHERE (PlaceTable = '{0}' AND OrderStatus = {1}) ORDER BY UpdatedAt DESC", orderMain.Table.Oid, Convert.ToInt16(OrderStatus.Open));
            var oResultUpdatedAt = XPOSettings.Session.ExecuteScalar(sqlOrderMainUpdatedAt);

            if (oResultUpdatedAt != null)
            {
                DateTime dateLastDBUpdate = Convert.ToDateTime(oResultUpdatedAt);

                if (orderMain.UpdatedAt < dateLastDBUpdate)
                {
                    orderMain.PersistentOid = orderMain.GetOpenTableFieldValueGuid(orderMain.Table.Oid, "Oid");
                    orderMain.OrderStatus = (OrderStatus)orderMain.GetOpenTableFieldValue(orderMain.Table.Oid, "OrderStatus");
                    orderMain.UpdatedAt = dateLastDBUpdate;


                    if (pTicketList.ListMode == TicketListMode.OrderMain) pTicketList.UpdateModel();

                    pTicketList.UpdateOrderStatusBar();
                    pTicketList.UpdateSaleOptionsPanelOrderButtons();


                    POSSession.CurrentSession.Save();
                }
            }
            else
            {
                orderMain.PersistentOid = orderMain.GetOpenTableFieldValueGuid(orderMain.Table.Oid, "Oid");
                orderMain.OrderStatus = (OrderStatus)orderMain.GetOpenTableFieldValue(orderMain.Table.Oid, "OrderStatus");

                if (pTicketList.ListMode == TicketListMode.OrderMain) pTicketList.UpdateModel();

                pTicketList.UpdateOrderStatusBar();
                pTicketList.UpdateSaleOptionsPanelOrderButtons();

                POSSession.CurrentSession.Save();
            }
        }

        private void UpdateUIIfHasWorkingOrder()
        {
            if (POSSession.CurrentSession.OrderMains.ContainsKey(POSSession.CurrentSession.CurrentOrderMainId)
              && POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId].Table != null)
            {
                TicketList.UpdateOrderStatusBar();
            }
            else
            {
                TicketList.SelectTableOrder(POSSettings.XpoOidConfigurationPlaceTableDefaultOpenTable);
                TicketList.UpdateArticleBag();
                TicketList.UpdateSaleOptionsPanelOrderButtons();
                TicketList.UpdateOrderStatusBar();
            }
        }

        private void ScrollTextViewLog(object o, SizeAllocatedArgs args)
        {
            TextViewLog.ScrollToIter(TextViewLog.Buffer.EndIter, 0, false, 0, 0);
        }
    }
}
