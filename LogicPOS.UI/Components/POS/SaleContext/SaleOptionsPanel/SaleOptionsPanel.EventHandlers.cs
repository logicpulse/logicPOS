using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Enums.TicketList;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Data.XPO.Settings.Terminal;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Settings;
using LogicPOS.Shared.Article;
using logicpos.shared.Enums;
using LogicPOS.Shared.Orders;
using LogicPOS.Shared;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Dialogs;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using LogicPOS.Api.Features.Articles.GetArticleByCode;
using LogicPOS.UI.Alerts;

namespace LogicPOS.UI.Components.POS
{
    public partial class SaleOptionsPanel
    {
        private void BtnPrevious_Clicked(object sender, EventArgs e)
        {
            ItemsPage.Previous();
        }

        private void BtnNext_Clicked(object sender, EventArgs e)
        {
            ItemsPage.Next();
        }

        private void BtnDelete_Clicked(object sender, EventArgs e)
        {
            ResponseType responseType = logicpos.Utils.ShowMessageBox(GlobalApp.PosMainWindow,
                                                                      DialogFlags.Modal,
                                                                      new System.Drawing.Size(400, 280),
                                                                      MessageType.Question,
                                                                      ButtonsType.YesNo,
                                                                      string.Format(GeneralUtils.GetResourceByName("global_warning"), GeneralSettings.ServerVersion),
                                                                      GeneralUtils.GetResourceByName("dialog_message__pos_order_cancel"));
        }

        private void BtnDecrease_Clicked(object sender, EventArgs e)
        {
            if (ItemsPage.SelectedItem == null)
            {
                return;
            }

            ItemsPage.DecreaseQuantity(ItemsPage.SelectedItem.Article.Id);
        }

        private void BtnIncrease_Clicked(object sender, EventArgs e)
        {
            if(ItemsPage.SelectedItem == null)
            {
                return;
            }

            ItemsPage.IncreaseQuantity(ItemsPage.SelectedItem.Article.Id);
        }

        private void BtnQuantity_Clicked(object sender, EventArgs e)
        {
            if (ItemsPage.SelectedItem == null)
            {
                return;
            }

            decimal newQuantity = PosKeyboardDialog.RequestDecimalValue(GlobalApp.PosMainWindow, ItemsPage.SelectedItem.Quantity);

            if (newQuantity == 0)
            {
                return;
            }

            ItemsPage.ChangeItemQuantity(ItemsPage.SelectedItem, newQuantity);
        }

        private void BtnPrice_Clicked(object sender, EventArgs e)
        {
            if (ItemsPage.SelectedItem == null)
            {
                return;
            }

            InsertMoneyModalResponse result = InsertMoneyModal.RequestDecimalValue(GlobalApp.PosMainWindow,
                                                                                   GeneralUtils.GetResourceByName("window_title_dialog_moneypad_product_price"),
                                                                                   ItemsPage.SelectedItem.UnitPrice);

            if (result.Response != ResponseType.Ok)
            {
                return;
            }

            ItemsPage.ChangeItemPrice(ItemsPage.SelectedItem, result.Value);
        }

        private void BtnFinishOrder_Clicked(object sender, EventArgs e)
        {
            ItemsPage.FinishTicket();
            UpdateButtonsSensitivity();
        }

        private void BtnPayments_Clicked(object sender, EventArgs e)
        {
            IconButtonWithText button = (sender as IconButtonWithText);

            bool printTicket = false;


            ResponseType dialogResponse = logicpos.Utils.ShowMessageTouch(GlobalApp.PosMainWindow, DialogFlags.DestroyWithParent, MessageType.Question, ButtonsType.OkCancel, GeneralUtils.GetResourceByName("window_title_dialog_message_dialog"), GeneralUtils.GetResourceByName("dialog_message_request_close_open_ticket"));
            if (dialogResponse != ResponseType.Ok)
            {
                return;
            }
        }

        private void BtnBarcode_Clicked(object sender, EventArgs e)
        {
            string fileWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_input_text_barcode.png";
            logicpos.Utils.ResponseText response = logicpos.Utils.GetInputText(GlobalApp.PosMainWindow, DialogFlags.Modal, fileWindowIcon, GeneralUtils.GetResourceByName("global_barcode_articlecode"), string.Empty, RegexUtils.RegexAlfaNumericExtended, true);

            if (response.ResponseType != ResponseType.Ok)
            {
                return;
            }

            var code = response.Text;

            var getArticle = DependencyInjection.Services.GetRequiredService<ISender>().Send(new GetArticleByCodeQuery(code)).Result;

            if (getArticle.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(SourceWindow, getArticle.FirstError);
                return;
            }

            if (getArticle.Value == null)
            {
                return;
            }

            ItemsPage.AddItem(new SaleItem(getArticle.Value));
        }

        private void BtnCardCode_Clicked(object sender, EventArgs e)
        {
            string fileWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_pos_ticketpad_card_entry.png";
            logicpos.Utils.ResponseText dialogResponse = logicpos.Utils.GetInputText(GlobalApp.PosMainWindow, DialogFlags.Modal, fileWindowIcon, GeneralUtils.GetResourceByName("global_cardcode_small"), string.Empty, RegexUtils.RegexAlfaNumericExtended, true);

            if (dialogResponse.ResponseType == ResponseType.Ok)
            {
                if (GeneralSettings.AppUseParkingTicketModule) /* IN009239 */
                {
                    GlobalApp.ParkingTicket.GetTicketDetailFromWS(dialogResponse.Text);
                }
                else
                {
                   
                }
            }
        }

        private void BtnListOrder_Clicked(object sender, EventArgs e)
        {
            OrderMain currentOrderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
            PosOrdersDialog dialog = new PosOrdersDialog(GlobalApp.PosMainWindow, DialogFlags.DestroyWithParent, currentOrderMain.Table.Name);
            ResponseType response = (ResponseType)dialog.Run();
            dialog.Destroy();

        }

        private void BtnChangeTable_Clicked(object sender, EventArgs e)
        {
            PosTablesDialog dialog = new PosTablesDialog(GlobalApp.PosMainWindow, DialogFlags.DestroyWithParent, TableFilterMode.OnlyFreeTables);
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
                        logicpos.Utils.ShowMessageTouch(
                            GlobalApp.PosMainWindow, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_error"),
                            GeneralUtils.GetResourceByName("dialog_message_table_is_not_free")
                        );
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

                    }
                }
                dialog.Destroy();
            };
        }

        private void BtnListMode_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnGifts_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnWeight_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnSelectTable_Clicked(object sender, EventArgs e)
        {
            var modal = new TablesModal(SourceWindow);
            modal.Run();
            modal.Destroy();
        }

        private void ItemsPage_TicketOpened(object sender, EventArgs e)
        {
            UpdateButtonsSensitivity();
        }
    }
}
