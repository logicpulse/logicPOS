using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Enums.TicketList;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.shared.Enums;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Settings.Terminal;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Settings;
using LogicPOS.Shared;
using LogicPOS.Shared.Article;
using LogicPOS.Shared.Orders;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace LogicPOS.UI.Components
{
    public partial class TicketList
    {
        private void BtnPrevious_Clicked(object sender, EventArgs e)
        {
            Previous();
        }

        private void BtnNext_Clicked(object sender, EventArgs e)
        {
            Next();
        }

        private void BtnDelete_Clicked(object sender, EventArgs e)
        {
            if (ListMode == TicketListMode.OrderMain)
            {

                ResponseType responseType = CustomAlerts.Question(POSWindow.Instance)
                                                        .WithSize(new System.Drawing.Size(400, 280))
                                                        .WithTitleResource("global_warning")
                                                        .WithMessage(GeneralUtils.GetResourceByName("dialog_message__pos_order_cancel"))
                                                        .ShowAlert();

                if (responseType == ResponseType.Yes)
                {

                    ListStore.Clear();
                    SelectedIndex = -1;
                    TotalItems = 0;
                    //Get Reference to current OrderMain
                    OrderMain orderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
                    fin_documentordermain documentOrderMain = null;
                    //Get current OrderMain Article Bag, After Process Payment/PartialPayment to check if current OrderMain has Items, or is Empty
                    ArticleBag pParameters = ArticleBag.TicketOrderToArticleBag(orderMain);
                    //Start UnitOfWork
                    using (UnitOfWork uowSession = new UnitOfWork())
                    {
                        if (pParameters.Count > 0)
                        {
                            pos_configurationplacetable placeTable;
                            placeTable = XPOUtility.GetEntityById<pos_configurationplacetable>(orderMain.Table.Oid);
                            documentOrderMain = (fin_documentordermain)uowSession.GetObjectByKey(typeof(fin_documentordermain), orderMain.PersistentOid);

                            placeTable.TableStatus = TableStatus.Free;
                            XPOUtility.Audit("TABLE_OPEN", string.Format(GeneralUtils.GetResourceByName("audit_message_table_open"), placeTable.Designation));
                            placeTable.DateTableClosed = DateTime.Now;
                            placeTable.TotalOpen = 0;
                            placeTable.Save();
                            //Required to Reload Objects after has been changed in Another Session(uowSession)
                            if (documentOrderMain != null)
                            {
                                documentOrderMain = XPOUtility.GetEntityById<fin_documentordermain>(orderMain.PersistentOid);
                                documentOrderMain.OrderStatus = OrderStatus.Close;
                                documentOrderMain.Save();
                            }

                            if (documentOrderMain != null) documentOrderMain.Reload();
                            //aceTable = (pos_configurationplacetable)XPOUtility.GetXPGuidObject(XPOSettings.Session, typeof(pos_configurationplacetable), orderMain.Table.Oid);
                            //placeTable.Reload();
                            ArticleBag.TicketOrderToArticleBag(orderMain).Clear();
                            //Clean Session if Commited without problems
                            orderMain.OrderStatus = OrderStatus.Close;
                            orderMain.CleanSessionOrder();
                            POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId] = orderMain;
                            POSSession.CurrentSession.DeleteEmptyTickets();
                            //obalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid].CleanSessionOrder();
                            uowSession.CommitChanges();
                        }
                    }

                    CurrentOrderDetail = orderMain.OrderTickets[orderMain.CurrentTicketId].OrderDetails;

                    //Always Change to OrderMain ListMode before Update Model
                    ListMode = TicketListMode.Ticket;
                    orderMain.CleanSessionOrder();
                    Gdk.Color colorListMode = (ListMode == TicketListMode.Ticket) ? colorListMode = TicketModeBackgroundColor.ToGdkColor() : colorListMode = DocumentModeBackgroundColor.ToGdkColor();
                    TreeView.ModifyBase(StateType.Normal, colorListMode);
                    //UpdateModel();
                    UpdateOrderStatusBar();
                    UpdateSaleOptionsPanelOrderButtons();
                    //IMPORTANT & REQUIRED: Assign Current Order Details from New CurrentTicketId, ELSE we cant add items to OrderMain
                    CurrentOrderDetail = orderMain.OrderTickets[orderMain.CurrentTicketId].OrderDetails;


                }
            }
            else
            {
                DeleteItem_Event(TicketListDeleteMode.Delete);
            }

        }

        private void BtnDecrease_Clicked(object sender, EventArgs e)
        {
            int i = 0;
            var price = Convert.ToDecimal(((string)ListStore.GetValue(_treeIter, (int)TicketListColumns.Price)).Replace('.', ','));

            foreach (var item in CurrentOrderDetail.Lines)
            {
                if (item.ArticleOid == CurrentDetailArticleId && item.Properties.PriceFinal == price)
                {
                    SelectedIndex = i;
                }
                i++;
            }

            DeleteItem_Event(TicketListDeleteMode.Decrease);
            UpdateSaleOptionsPanelButtons();


        }

        private void BtnIncrease_Clicked(object sender, EventArgs e)
        {
            decimal defaultQuantity = GetArticleDefaultQuantity(CurrentDetailArticleId);
            var price = Convert.ToDecimal(((string)ListStore.GetValue(_treeIter, (int)TicketListColumns.Price)).Replace('.', ','));

            decimal oldValueQnt = Convert.ToDecimal(((string)ListStore.GetValue(_treeIter, (int)TicketListColumns.Quantity)).Replace('.', ','));
            if (!logicpos.Utils.CheckStocks())
            {
                if (CustomAlerts.ShowMinimumStockAlert(POSWindow.Instance, CurrentOrderDetail.Lines[SelectedIndex].ArticleOid, (oldValueQnt + defaultQuantity), out bool showMessage))
                {
                    ChangeQuantity(oldValueQnt + defaultQuantity);
                    UpdateSaleOptionsPanelButtons();
                }
                else
                {
                    UpdateSaleOptionsPanelButtons();
                }
            }
            else
            {
                ChangeQuantity(oldValueQnt + defaultQuantity);
                UpdateSaleOptionsPanelButtons();
            }


        }

        private void BtnQuantity_Clicked(object sender, EventArgs e)
        {
            decimal oldValueQnt = CurrentOrderDetail.Lines[SelectedIndex].Properties.Quantity;
            decimal newValueQnt = PosKeyboardDialog.RequestDecimalValue(POSWindow.Instance, oldValueQnt);
            bool showMessage;
            if (newValueQnt > 0)
            {
                if (logicpos.Utils.CheckStocks())
                {
                    if (!CustomAlerts.ShowMinimumStockAlert(POSWindow.Instance, CurrentOrderDetail.Lines[SelectedIndex].ArticleOid, newValueQnt, out showMessage))
                    {
                        if (showMessage)
                        {
                            newValueQnt = oldValueQnt;
                            return;
                        }
                    }
                }
                ChangeQuantity(newValueQnt);
            }
            UpdateSaleOptionsPanelButtons();

        }

        private void BtnPrice_Clicked(object sender, EventArgs e)
        {
            InsertMoneyModalResponse result = InsertMoneyModal.RequestDecimalValue(POSWindow.Instance, GeneralUtils.GetResourceByName("window_title_dialog_moneypad_product_price"), 0);
            decimal newValuePrice = result.Value;
        }

        private void BtnFinishOrder_Clicked(object sender, EventArgs e)
        {

            //Call Framework FinishOrder
            OrderMain orderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];

            if (GeneralSettings.AppUseParkingTicketModule)
            {
                orderMain.CheckForDuplicatedArticleInArticleBag(XPOSettings.Session);
            }

            fin_documentorderticket orderTicket = orderMain.FinishOrder(XPOSettings.Session);

            PrintingSettings.ThermalPrinter.UsingThermalPrinter = true;
            if (TerminalSettings.HasLoggedTerminal &&
                LoggedTerminalSettings.HasThermalPrinter &&
                TerminalService.Terminal.Printer.Type.ThermalPrinter &&
                orderTicket.OrderDetail.Count != 0)
            {

                if (!GeneralSettings.AppUseParkingTicketModule && logicpos.Utils.PrintTicket())
                {
                    // TK016249 Impressoras - Diferenciação entre Tipos 
                    //FrameworkCalls.PrintOrderRequest(POSWindow, TerminalService.Terminal.ThermalPrinter, orderMain, orderTicket);
                }
                // FrameworkCalls.PrintArticleRequest(POSWindow, orderTicket);
            }

            //Change Mode
            ListMode = TicketListMode.OrderMain;
            //Reset TicketList TotalItems Counter
            TotalItemsTicketListMode = 0;
            UpdateOrderStatusBar();
            UpdateModel();

        }

        private void BtnPayments_Clicked(object sender, EventArgs e)
        {
            IconButtonWithText button = (sender as IconButtonWithText);

            bool printTicket = false;

            //Request Finish Open Ticket
            if (TotalItemsTicketListMode > 0)
            {
                ResponseType dialogResponse = CustomAlerts.Question(POSWindow.Instance)
                                                          .WithSize(new System.Drawing.Size(400, 280))
                                                          .WithTitleResource("global_warning")
                                                          .WithMessage(GeneralUtils.GetResourceByName("dialog_message_request_close_open_ticket"))
                                                          .ShowAlert();

                if (dialogResponse != ResponseType.Ok)
                {
                    return;
                };
            };

            //Get Reference to current OrderMain
            OrderMain orderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];

            //Finish Order, if Has Ticket Details
            if (orderMain.OrderTickets[orderMain.CurrentTicketId].OrderDetails.Lines.Count > 0)
            {
                //Before Use FrameworkCall
                orderMain.FinishOrder(XPOSettings.Session, printTicket);

                //Reset TicketList TotalItems Counter
                TotalItemsTicketListMode = 0;
            }

            //Always Change to OrderMain ListMode before Update Model
            ListMode = TicketListMode.OrderMain;

            //Update Model and Gui
            UpdateModel();
            UpdateOrderStatusBar();
            UpdateSaleOptionsPanelOrderButtons();

            //Initialize ArticleBag to Send to Payment Dialog
            ArticleBag articleBag = ArticleBag.TicketOrderToArticleBag(orderMain);

            // Shared Referencesfor Dialog
            BaseDialog dialog = null;

            // Get Dialog Reference
            if (button.Name.Equals("touchButtonPosTicketPadPayments_Green"))
            {
                //dialog = new PaymentDialog(POSWindow, DialogFlags.DestroyWithParent, articleBag);
            }
            else
            if (button.Name.Equals("touchButtonPosTicketPadSplitAccount_Green"))
            {
                //dialog = new PosSplitPaymentsDialog(POSWindow, DialogFlags.DestroyWithParent, ArticleBag, this);
            }

            // Shared code to call Both Dialogs
            ResponseType response = (ResponseType)dialog.Run();

            if (response == ResponseType.Ok)
            {
                //Update Cleaned TreeView Model
                UpdateModel();
                UpdateOrderStatusBar();
                UpdateSaleOptionsPanelOrderButtons();
                //IMPORTANT & REQUIRED: Assign Current Order Details from New CurrentTicketId, ELSE we cant add items to OrderMain
                CurrentOrderDetail = orderMain.OrderTickets[orderMain.CurrentTicketId].OrderDetails;
                //Valid Result Destroy Dialog
                dialog.Destroy();
            };

        }

        private void BtnBarcode_Clicked(object sender, EventArgs e)
        {
            string fileWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_input_text_barcode.png";
            logicpos.Utils.ResponseText dialogResponse = logicpos.Utils.GetInputText(POSWindow.Instance, DialogFlags.Modal, fileWindowIcon, GeneralUtils.GetResourceByName("global_barcode_articlecode"), string.Empty, RegexUtils.RegexAlfaNumericExtended, true);

            if (dialogResponse.ResponseType == ResponseType.Ok)
            {
                if (GeneralSettings.AppUseParkingTicketModule) /* IN009239 */
                {
                    LogicPOSAppContext.ParkingTicket.GetTicketDetailFromWS(dialogResponse.Text);
                }
                else
                {
                    InsertOrUpdate(dialogResponse.Text);
                }
            }
        }

        private void BtnCardCode_Clicked(object sender, EventArgs e)
        {
            string fileWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_pos_ticketpad_card_entry.png";
            logicpos.Utils.ResponseText dialogResponse = logicpos.Utils.GetInputText(POSWindow.Instance, DialogFlags.Modal, fileWindowIcon, GeneralUtils.GetResourceByName("global_cardcode_small"), string.Empty, RegexUtils.RegexAlfaNumericExtended, true);

            if (dialogResponse.ResponseType == ResponseType.Ok)
            {
                if (GeneralSettings.AppUseParkingTicketModule) /* IN009239 */
                {
                    LogicPOSAppContext.ParkingTicket.GetTicketDetailFromWS(dialogResponse.Text);
                }
                else
                {
                    InsertOrUpdate(dialogResponse.Text);
                }
            }
        }

        private void BtnListOrder_Clicked(object sender, EventArgs e)
        {
            OrderMain currentOrderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
            PosOrdersDialog dialog = new PosOrdersDialog(POSWindow.Instance, DialogFlags.DestroyWithParent, currentOrderMain.Table.Name);
            ResponseType response = (ResponseType)dialog.Run();
            dialog.Destroy();

        }

        private void BtnChangeTable_Clicked(object sender, EventArgs e)
        {
            PosTablesDialog dialog = new PosTablesDialog(POSWindow.Instance, DialogFlags.DestroyWithParent, TableFilterMode.OnlyFreeTables);
            ResponseType response = (ResponseType)dialog.Run();

            if (response == ResponseType.Ok || response == ResponseType.Cancel || response == ResponseType.DeleteEvent)
            {
                if (response == ResponseType.Ok)
                {
                    OrderMain currentOrderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
                    pos_configurationplacetable xOldTable = XPOUtility.GetEntityById<pos_configurationplacetable>(currentOrderMain.Table.Oid);
                    pos_configurationplacetable xNewTable = XPOUtility.GetEntityById<pos_configurationplacetable>(dialog.CurrentTableOid);

                    if (xNewTable.TableStatus != TableStatus.Free)
                    {
                        CustomAlerts.Error(POSWindow.Instance)
                                    .WithMessage(GeneralUtils.GetResourceByName("dialog_message_table_is_not_free"))
                                    .ShowAlert();
                    }
                    else
                    {
                        //Put Old table Status to Free
                        xOldTable.TableStatus = TableStatus.Free;
                        xOldTable.Save();
                        XPOUtility.Audit("TABLE_OPEN", string.Format(GeneralUtils.GetResourceByName("audit_message_table_open"), xOldTable.Designation));

                        //Put New table Status to Open
                        xNewTable.TableStatus = TableStatus.Open;
                        xNewTable.Save();
                        XPOUtility.Audit("TABLE_CLOSE", string.Format(GeneralUtils.GetResourceByName("audit_message_table_close"), xNewTable.Designation));

                        //Change DocumentOrderMain table, If OpenOrder Exists in That table
                        Guid documentOrderMainOid = currentOrderMain.GetOpenTableFieldValueGuid(xOldTable.Oid, "Oid");
                        fin_documentordermain xDocumentOrderMain = XPOUtility.GetEntityById<fin_documentordermain>(documentOrderMainOid);
                        if (xDocumentOrderMain != null)
                        {
                            xDocumentOrderMain.PlaceTable = xNewTable;
                            xDocumentOrderMain.UpdatedAt = XPOUtility.CurrentDateTimeAtomic();
                            xDocumentOrderMain.Save();
                        }
                        //Assign Session Data
                        currentOrderMain.Table.Oid = xNewTable.Oid;
                        currentOrderMain.Table.Name = xNewTable.Designation;
                        currentOrderMain.Table.PriceType = (PriceType)xNewTable.Place.PriceType.EnumValue;
                        currentOrderMain.OrderTickets[currentOrderMain.CurrentTicketId].PriceType = (PriceType)xNewTable.Place.PriceType.EnumValue;
                        currentOrderMain.Table.PlaceId = xNewTable.Place.Oid;
                        POSSession.CurrentSession.Save();
                        //Finally Update Status Bar, Table Name, Totals Etc
                        UpdateOrderStatusBar();
                    }
                }
                dialog.Destroy();
            };
        }

        private void BtnListMode_Clicked(object sender, EventArgs e)
        {
            ListMode = (ListMode == TicketListMode.Ticket) ? TicketListMode.OrderMain : TicketListMode.Ticket;
            UpdateModel();
        }

        private void BtnGifts_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnWeight_Clicked(object sender, EventArgs e)
        {

            SelectedIndex = CurrentOrderDetail.Lines.FindIndex(item => item.ArticleOid == (Guid)ListStore.GetValue(_treeIter,
   (int)TicketListColumns.ArticleId) && item.Properties.PriceNet == Convert.ToDecimal(ListStore.GetValue(_treeIter, (int)TicketListColumns.Price)));

            decimal articlePricePerKg = decimal.Round(CurrentOrderDetail.Lines[SelectedIndex].Properties.PriceFinal, 2, MidpointRounding.AwayFromZero);
            LogicPOSAppContext.WeighingBalance.WeighArticle(articlePricePerKg);

        }

        public void WeighingBalance_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            string inData = LogicPOSAppContext.WeighingBalance.ComPort().ReadLine() + "\n";

            if (inData.Substring(0, 2) == "99")
            {
                List<int> result = LogicPOSAppContext.WeighingBalance.CalculateFromHex(inData);

                decimal quantity = Convert.ToDecimal(result[0]) / 1000;

                if (logicpos.Utils.CheckStocks())
                {
                    if (CustomAlerts.ShowMinimumStockAlert(POSWindow.Instance, CurrentOrderDetail.Lines[SelectedIndex].ArticleOid, quantity, out bool showMessage))
                    {
                        ChangeQuantity(quantity);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ChangeQuantity(quantity);
                }

            }
        }

        private Guid GetVatExemptionReason()
        {
            Guid result = new Guid();
            return result;
        }

        private void BtnSelectTable_Clicked(object sender, EventArgs e)
        {
            PosTablesDialog dialog = new PosTablesDialog(POSWindow.Instance, DialogFlags.DestroyWithParent);
            ResponseType response = (ResponseType)dialog.Run();

            if (response == ResponseType.Ok || response == ResponseType.Cancel || response == ResponseType.DeleteEvent)
            {
                if (response == ResponseType.Ok)
                {
                    SelectTableOrder(dialog.CurrentTableOid);
                    UpdateArticleBag();
                    UpdateSaleOptionsPanelOrderButtons();
                    UpdateOrderStatusBar();
                }
                dialog.Destroy();
            };
        }

        public void SelectTableOrder(Guid tableId)
        {
            OrderMain currentOrderMain = null;
            if (tableId == LogicPOSSettings.XpoOidConfigurationPlaceTableDefaultOpenTable)
            {
                var configurationPlace = (pos_configurationplace)XPOSettings.Session.GetObjectByKey(typeof(pos_configurationplace), LogicPOSSettings.XpoOidConfigurationPlaceTableDefaultOpenTable);
                if (configurationPlace == null)
                {
                    tableId = ((pos_configurationplacetable)XPOUtility.GetXPGuidObjectFromCriteria(typeof(pos_configurationplacetable), string.Format("(Code = '{0}')", "10")) as pos_configurationplacetable).Oid;
                }
            }

            if (POSSession.CurrentSession.OrderMains.Count > 0)
            {
                currentOrderMain = POSSession.CurrentSession.OrderMains.Values.Where<OrderMain>(key => key.Table.Oid == tableId).FirstOrDefault<OrderMain>();
            }

            if (currentOrderMain != null && Convert.ToInt16(currentOrderMain.OrderStatus) != -1)
            {
                ListMode = TicketListMode.OrderMain;
            }
            else
            {
                ListMode = TicketListMode.Ticket;
            }

            if (currentOrderMain == null)
            {
                Guid newOrderMainOid = Guid.NewGuid();
                POSSession.CurrentSession.OrderMains.Add(newOrderMainOid, new OrderMain(newOrderMainOid, tableId));
                OrderMain newOrderMain = POSSession.CurrentSession.OrderMains[newOrderMainOid];
                OrderTicket orderTicket = new OrderTicket(newOrderMain, (PriceType)newOrderMain.Table.PriceType);
                newOrderMain.OrderTickets.Add(1, orderTicket);
                currentOrderMain = newOrderMain;
            }

            currentOrderMain.PersistentOid = currentOrderMain.GetOpenTableFieldValueGuid(tableId, "Oid");
            currentOrderMain.OrderStatus = (OrderStatus)currentOrderMain.GetOpenTableFieldValue(tableId, "OrderStatus");

            POSSession.CurrentSession.CurrentOrderMainId = currentOrderMain.Table.OrderMainOid;
            POSSession.CurrentSession.Save();
            UpdateModel();

            POSWindow.Instance.MenuArticles.Sensitive = true;
            UpdateArticleBag();
            UpdateSaleOptionsPanelOrderButtons();
            UpdateOrderStatusBar();
        }
    }
}
