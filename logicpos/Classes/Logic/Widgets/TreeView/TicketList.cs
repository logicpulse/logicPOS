using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial;
using logicpos.Classes.Enums.TicketList;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using logicpos.datalayer.Enums;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Classes.Orders;
using logicpos.shared.Enums;
using System;
using logicpos.Classes.Enums.GenericTreeView;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    partial class TicketList
    {
        //Prev
        void _buttonKeyPrev_Clicked(object sender, EventArgs e)
        {
            Prev();
        }

        //Next
        void _buttonKeyNext_Clicked(object sender, EventArgs e)
        {
            Next();
        }

        //Delete
        void _buttonKeyDelete_Clicked(object sender, EventArgs e)
        {
            DeleteItem_Event(TicketListDeleteMode.Delete);
        }

        //Decrease
        void _buttonKeyDecrease_Clicked(object sender, EventArgs e)
        {
            DeleteItem_Event(TicketListDeleteMode.Decrease);
        }

        //Increase
        void _buttonKeyIncrease_Clicked(object sender, EventArgs e)
        {
            //Get Article defaultQuantity
            decimal defaultQuantity = GetArticleDefaultQuantity(_currentDetailArticleOid);
            decimal oldValueQnt = _currentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity;
            ChangeQuantity(oldValueQnt + defaultQuantity);
        }

        //Change Quantity
        void _buttonKeyChangeQuantity_Clicked(object sender, EventArgs e)
        {
            try
            {
                decimal oldValueQnt = _currentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity;
                decimal newValueQnt = PosKeyboardDialog.RequestDecimalValue(_sourceWindow, oldValueQnt);

                if (newValueQnt > 0)
                {
                    ChangeQuantity(newValueQnt);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //Change Price
        void _buttonKeyChangePrice_Clicked(object sender, EventArgs e)
        {
            decimal oldValueQuantity = _currentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity;
            decimal oldValuePrice = _currentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.PriceFinal;

            MoneyPadResult result = PosMoneyPadDialog.RequestDecimalValue(_sourceWindow, Resx.window_title_dialog_moneypad_product_price, oldValuePrice);
            decimal newValuePrice = result.Value;

            if (result.Response == ResponseType.Ok && newValuePrice > 0)
            {
                //Create a Fresh Object to Get Input Price and Calc from TotalFinal with Quantity 1, Without Touch Quantity in current Line
                PriceProperties priceProperties = PriceProperties.GetPriceProperties(
                  PricePropertiesSourceMode.FromTotalFinal,
                  _currentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.PriceWithVat,
                  newValuePrice,
                  1.0m,
                  _currentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.DiscountArticle,
                  _currentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.DiscountGlobal,
                  _currentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Vat
                );

                //Update orderDetails 
                _currentOrderDetails.Update(_listStoreModelSelectedIndex, oldValueQuantity, priceProperties.PriceUser);
                //Update TreeView Model Price
                _listStoreModel.SetValue(_treeIter, (int)TicketListColumns.Price, FrameworkUtils.DecimalToString(newValuePrice));
                //Update Total
                decimal totalLine = _currentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.TotalFinal;
                _listStoreModel.SetValue(_treeIter, (int)TicketListColumns.Total, FrameworkUtils.DecimalToString(totalLine));
            }
            UpdateTicketListTotal();
        }

        //Finish Order
        void _buttonKeyFinishOrder_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Call Framework FinishOrder
                OrderMain orderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];
                FIN_DocumentOrderTicket orderTicket = orderMain.FinishOrder(GlobalFramework.SessionXpo);

                if (orderTicket != null)
                {
                    //public static bool PrintOrderRequest(Window pSourceWindow, SYS_ConfigurationPrinters pPrinter, OrderMain pDocumentOrderMain, FIN_DocumentOrderTicket pOrderTicket)
                    FrameworkCalls.PrintOrderRequest(_sourceWindow, GlobalFramework.LoggedTerminal.Printer, orderMain, orderTicket);
                    FrameworkCalls.PrintArticleRequest(_sourceWindow, orderTicket);
                }

                //Change Mode
                _listMode = TicketListMode.OrderMain;
                //Reset TicketList TotalItems Counter
                _listStoreModelTotalItemsTicketListMode = 0;
                UpdateOrderStatusBar();
                UpdateModel();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //Payments
        void _buttonKeyPayments_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Used when we pay without FinishOrder, to Skip print Ticket
                bool printTicket = false;

                //Request Finish Open Ticket
                if (_listStoreModelTotalItemsTicketListMode > 0)
                {
                    ResponseType dialogResponse = Utils.ShowMessageTouch(_sourceWindow, DialogFlags.DestroyWithParent, MessageType.Question, ButtonsType.OkCancel, Resx.window_title_dialog_message_dialog, Resx.dialog_message_request_close_open_ticket);
                    if (dialogResponse != ResponseType.Ok)
                    {
                        return;
                    };
                };

                //Get Reference to current OrderMain
                OrderMain orderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];

                //Finish Order, if Has Ticket Details
                if (orderMain.OrderTickets[orderMain.CurrentTicketId].OrderDetails.Lines.Count > 0)
                {
                    //Before Use FrameworkCall
                    orderMain.FinishOrder(GlobalFramework.SessionXpo, printTicket);
                    //TODO: Continue to implement FrameworkCall here
                    //DocumentOrderTicket documentOrderTicket = orderMain.FinishOrder(GlobalFramework.SessionXpo, printTicket);
                    //if (printTicket) FrameworkCalls.PrintTableTicket(_sourceWindow, GlobalFramework.LoggedTerminal.Printer, GlobalFramework.LoggedTerminal.TemplateTicket, orderMain, documentOrderTicket.Oid);

                    //Reset TicketList TotalItems Counter
                    _listStoreModelTotalItemsTicketListMode = 0;
                }

                //Always Change to OrderMain ListMode before Update Model
                _listMode = TicketListMode.OrderMain;

                //Update Model and Gui
                UpdateModel();
                UpdateOrderStatusBar();
                UpdateTicketListOrderButtons();

                //Initialize ArticleBag to Send to Payment Dialog
                ArticleBag articleBag = ArticleBag.TicketOrderToArticleBag(orderMain);

                PosPaymentsDialog dialog = new PosPaymentsDialog(_sourceWindow, DialogFlags.DestroyWithParent, articleBag);
                int response = dialog.Run();

                if (response == (int)ResponseType.Ok)
                {
                    //Update Cleaned TreeView Model
                    UpdateModel();
                    UpdateOrderStatusBar();
                    UpdateTicketListOrderButtons();
                    //IMPORTANT & REQUIRED: Assign Current Order Details from New CurrentTicketId, ELSE we cant add items to OrderMain
                    _currentOrderDetails = orderMain.OrderTickets[orderMain.CurrentTicketId].OrderDetails;
                    //Valid Result Destroy Dialog
                    dialog.Destroy();
                };
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //BarCode
        void _buttonKeyBarCode_Clicked(object sender, EventArgs e)
        {
            string fileWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_input_text_barcode.png");
            logicpos.Utils.ResponseText dialogResponse = Utils.GetInputText(_sourceWindow, DialogFlags.Modal, fileWindowIcon, Resx.global_barcode, string.Empty, SettingsApp.RegexInteger, true);
            if (dialogResponse.ResponseType == ResponseType.Ok)
            {
                InsertOrUpdate(dialogResponse.Text);
            }
        }

        //ListOrder
        void _buttonKeyListOrder_Clicked(object sender, EventArgs e)
        {
            try
            {
                OrderMain currentOrderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];
                PosOrdersDialog dialog = new PosOrdersDialog(this.SourceWindow, DialogFlags.DestroyWithParent, currentOrderMain.Table.Name);
                ResponseType response = (ResponseType)dialog.Run();
                dialog.Destroy();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //ChangeTable - Change Order From Table to Table
        void _buttonKeyChangeTable_Clicked(object sender, EventArgs e)
        {
            PosTablesDialog dialog = new PosTablesDialog(this.SourceWindow, DialogFlags.DestroyWithParent, TableFilterMode.OnlyFreeTables);
            ResponseType response = (ResponseType)dialog.Run();

            if (response == ResponseType.Ok || response == ResponseType.Cancel || response == ResponseType.DeleteEvent)
            {
                if (response == ResponseType.Ok)
                {
                    OrderMain currentOrderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];
                    POS_ConfigurationPlaceTable xOldTable = (POS_ConfigurationPlaceTable)FrameworkUtils.GetXPGuidObject(typeof(POS_ConfigurationPlaceTable), currentOrderMain.Table.Oid);
                    POS_ConfigurationPlaceTable xNewTable = (POS_ConfigurationPlaceTable)FrameworkUtils.GetXPGuidObject(typeof(POS_ConfigurationPlaceTable), dialog.CurrentTableOid);
                    //Require to Prevent A first chance exception of type 'DevExpress.Xpo.DB.Exceptions.LockingException' occurred in DevExpress.Xpo.v13.2.dll when it is Changed in Diferent Session ex UnitOfWork
                    //TODO: Confirm working with Reload Commented
                    //xOldTable.Reload();
                    //xNewTable.Reload();

                    if (xNewTable.TableStatus != TableStatus.Free)
                    {
                        Utils.ShowMessageTouch(
                            GlobalApp.WindowPos, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, Resx.global_error, 
                            Resx.dialog_message_table_is_not_free
                        );
                    }
                    else
                    {
                        //Put Old table Status to Free
                        xOldTable.TableStatus = TableStatus.Free;
                        xOldTable.Save();
                        FrameworkUtils.Audit("TABLE_OPEN", string.Format(Resx.audit_message_table_open, xOldTable.Designation));

                        //Put New table Status to Open
                        xNewTable.TableStatus = TableStatus.Open;
                        xNewTable.Save();
                        FrameworkUtils.Audit("TABLE_CLOSE", string.Format(Resx.audit_message_table_close, xNewTable.Designation));

                        //Change DocumentOrderMain table, If OpenOrder Exists in That table
                        Guid documentOrderMainOid = currentOrderMain.GetOpenTableFieldValueGuid(xOldTable.Oid, "Oid");
                        FIN_DocumentOrderMain xDocumentOrderMain = (FIN_DocumentOrderMain)FrameworkUtils.GetXPGuidObject(typeof(FIN_DocumentOrderMain), documentOrderMainOid);
                        if (xDocumentOrderMain != null)
                        {
                            xDocumentOrderMain.PlaceTable = xNewTable;
                            xDocumentOrderMain.UpdatedAt = FrameworkUtils.CurrentDateTimeAtomic();
                            xDocumentOrderMain.Save();
                        }
                        //Assign Session Data
                        currentOrderMain.Table.Oid = xNewTable.Oid;
                        currentOrderMain.Table.Name = xNewTable.Designation;
                        currentOrderMain.Table.PriceType = (PriceType)xNewTable.Place.PriceType.EnumValue;
                        currentOrderMain.OrderTickets[currentOrderMain.CurrentTicketId].PriceType = (PriceType)xNewTable.Place.PriceType.EnumValue;
                        currentOrderMain.Table.PlaceId = xNewTable.Place.Oid;
                        GlobalFramework.SessionApp.Write();
                        //Finally Update Status Bar, Table Name, Totals Etc
                        UpdateOrderStatusBar();
                    }
                }
                dialog.Destroy();
            };
        }

        //Change List Mode
        void _buttonKeyListMode_Clicked(object sender, EventArgs e)
        {
            //Toggle Mode
            _listMode = (_listMode == TicketListMode.Ticket) ? TicketListMode.OrderMain : TicketListMode.Ticket;
            //ArticleBag
            UpdateModel();
        }

        //Gifts
        void _buttonKeyGifts_Clicked(object sender, EventArgs e)
        {
            Utils.ShowMessageTouchUnderConstruction(this.SourceWindow);
        }

        //Weight
        void _buttonKeyWeight_Clicked(object sender, EventArgs e)
        {
            Utils.ShowMessageTouchUnderConstruction(this.SourceWindow);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        /// <summary>
        /// Helper method to get VatExemptionReason
        /// </summary>
        /// <returns>VatExemptionReason Guid</returns>
        private Guid GetVatExemptionReason()
        {
            Guid result = new Guid();

            CriteriaOperator criteria = CriteriaOperator.Parse("(Disabled = 0 OR Disabled IS NULL)");
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewConfigurationVatExceptionReason>
              dialog = new PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewConfigurationVatExceptionReason>(
                _sourceWindow,
                DialogFlags.DestroyWithParent,
                    Resx.global_vat_exemption_reason,
                GlobalApp.MaxWindowSize,
                null, //XpoDefaultValue
                criteria,
                GenericTreeViewMode.Default,
                null  //pActionAreaButtons
              );

            int response = dialog.Run();
            if (response == (int)ResponseType.Ok)
            {
                result = dialog.GenericTreeView.DataSourceRow.Oid;
            }
            dialog.Destroy();

            return result;
        }
    }
}
