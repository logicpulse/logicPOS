using DevExpress.Xpo.DB;
using Gtk;
using logicpos;
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
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Windows
{
    public partial class POSWindow
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
            if (LogicPOSAppContext.BarCodeReader != null)
            {
                LogicPOSAppContext.BarCodeReader.KeyReleaseEvent(this, o, args);
            }
        }

        private void BtnQuit_Clicked(object sender, EventArgs e)
        {
            LogicPOSAppUtils.Quit(this);
        }

        private void BtnBackOffice_Clicked(object sender, EventArgs e)
        {
            Utils.ShowBackOffice(this);
        }

        private void BtnReports_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnLogOut_Clicked(object sender, EventArgs e)
        {
            Hide();
            LoginWindow.Instance.LogOutUser(true);
        }

        private void BtnCashDrawer_Clicked(object sender, EventArgs e)
        {
            SessionOpeningModal modal = new SessionOpeningModal(this);
            modal.Run();
            modal.Destroy();
        }

        private void BtnNewDocument_Clicked(object sender, EventArgs e)
        {
            if (FiscalYearService.HasFiscalYear == false)
            {
                FiscalYearService.ShowOpenFiscalYearAlert();
                return;
            }

            var modal = new CreateDocumentModal(this);
            modal.Run();
            modal.Destroy();
        }

        private void BtnDocuments_Clicked(object sender, EventArgs e)
        {
            var documentsMenu = new DocumentsMenuModal(this);
            documentsMenu.Run();
            documentsMenu.Destroy();
        }

        private void BtnChangeUser_Clicked(object sender, EventArgs e)
        {
            ChangeUserModal dialogChangeUser = new ChangeUserModal(this, DialogFlags.DestroyWithParent);


            int responseChangeUser = dialogChangeUser.Run();
            if (responseChangeUser != (int)ResponseType.Ok)
            {
                dialogChangeUser.Destroy();
                return;
            }

            UserPinModal pinModal = new UserPinModal(dialogChangeUser, DialogFlags.DestroyWithParent, dialogChangeUser.User);
            int responsePinPad = pinModal.Run();
            if (responsePinPad != (int)ResponseType.Ok)
            {
                pinModal.Destroy();
                dialogChangeUser.Destroy();
                return;
            }

            LabelTerminalInfo.Text = $"{TerminalService.Terminal.Designation} : {AuthenticationService.User.Name}";

            //Utils.ShowNotifications(pinModal); -> tchial0

            pinModal.Destroy();
            dialogChangeUser.Destroy();
        }

        private void ButtonFavorites_Clicked(object sender, EventArgs e)
        {
            if (MenuFamilies.SelectedButton != null && !MenuFamilies.SelectedButton.Sensitive) MenuFamilies.SelectedButton.Sensitive = true;
            if (MenuSubfamilies.SelectedButton != null && !MenuSubfamilies.SelectedButton.Sensitive) MenuSubfamilies.SelectedButton.Sensitive = true;
        }

        private void ImageLogo_Clicked(object o, ButtonPressEventArgs args)
        {
            UserPinModal dialogPinPad = new UserPinModal(this,
                                                               DialogFlags.Modal,
                                                               null, //tchial0
                                                               true);
            int responsePinPad = dialogPinPad.Run();
            if (responsePinPad == (int)ResponseType.Ok)
            {
                var resultOpenDoor = LogicPOS.Printing.Utility.PrintingUtils.OpenDoor();
                if (!resultOpenDoor)
                {
                    CustomAlerts.Information(this)
                                .WithSize(new Size(500, 340))
                                .WithTitleResource("global_information")
                                .WithMessage(string.Format(GeneralUtils.GetResourceByName("open_cash_draw_permissions")))
                                .ShowAlert();
                }
                else
                {
                    XPOUtility.Audit("CASHDRAWER_OUT", string.Format(
                         GeneralUtils.GetResourceByName("audit_message_cashdrawer_out"),
                         TerminalService.Terminal.Designation,
                         "Button Open Door"));
                }

            };

            dialogPinPad.Destroy();

        }

        private void HWBarCodeReader_Captured(object sender, EventArgs e)
        {
            if (POSWindow.Instance.TicketList.CurrentOrderDetail != null)
            {
                switch (LogicPOSAppContext.BarCodeReader.Device)
                {
                    case InputReaderDevice.None:
                        break;
                    case InputReaderDevice.BarCodeReader:
                    case InputReaderDevice.CardReader:
                        if (GeneralSettings.AppUseParkingTicketModule)
                        {
                            LogicPOSAppContext.ParkingTicket.GetTicketDetailFromWS(LogicPOSAppContext.BarCodeReader.Buffer);
                        }
                        else
                        {
                            TicketList.InsertOrUpdate(LogicPOSAppContext.BarCodeReader.Buffer);
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        public void UpdateUI()
        {
            LabelTerminalInfo.Text = $"{TerminalService.Terminal.Designation} : {AuthenticationService.User.Name}";
            MenuFamilies.LoadEntities();
            MenuSubfamilies.LoadEntities();
            MenuArticles.PresentArticles();
        }

        public void UpdateWorkSessionUI()
        {
            if (WorkSessionService.DayIsOpen() == false)
            {
                if (GeneralSettings.AppUseBackOfficeMode == false)
                {
                    SaleOptionsPanel.Sensitive = false;
                    MenuArticles.Sensitive = false;
                }

                return;
            }

            if (!WorkSessionService.TerminalIsOpen())
            {
                return;
            }
        }

        private void StartClock()
        {
            return; //tchial0
            GLib.Timeout.Add(1000, new GLib.TimeoutHandler(UpdateClock));
        }

        private bool UpdateClock()
        {
            if (POSWindow.Instance.Visible == false)
            {
                return true;
            }

            LabelClock.Text = DateTime.Now.ToString(ClockTimeFormat);

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
                TicketList.SelectTableOrder(LogicPOSSettings.XpoOidConfigurationPlaceTableDefaultOpenTable);
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
