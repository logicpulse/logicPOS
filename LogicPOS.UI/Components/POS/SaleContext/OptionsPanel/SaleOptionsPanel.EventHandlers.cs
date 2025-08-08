using Gtk;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Printing;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.POS
{
    public partial class SaleOptionsPanel
    {
        public PosTicket LastTicket { get; private set; }
        private void BtnPrevious_Clicked(object sender, EventArgs e)
        {
            SaleContext.ItemsPage.Previous();
        }

        private void BtnNext_Clicked(object sender, EventArgs e)
        {
            SaleContext.ItemsPage.Next();
        }

        private void BtnDelete_Clicked(object sender, EventArgs e)
        {
            ResponseType responseType = CustomAlerts.Question(POSWindow.Instance)
                                                    .WithSize(new Size(400, 280))
                                                    .WithTitleResource("global_warning")
                                                    .WithMessage(GeneralUtils.GetResourceByName("dialog_message__pos_order_cancel"))
                                                    .ShowAlert();

            if (responseType != ResponseType.Yes && responseType != ResponseType.Ok)
            {
                return;
            }

            if (SaleContext.CurrentOrder.Id.HasValue)
            {
                var deleteOrderResult = OrdersService.CloseOrder(SaleContext.CurrentOrder.Id.Value);

                if (deleteOrderResult == false)
                {
                    CustomAlerts.Error(POSWindow.Instance)
                                .WithMessage("Não foi possível cancelar o pedido. Tente novamente.")
                                .ShowAlert();
                    return;
                }
            }

            SaleContext.ItemsPage.Clear(true);
            SaleContext.CurrentOrder.Close();
            SaleContext.ItemsPage.SetTicketModeBackGround();
            POSWindow.Instance.UpdateUI();
            UpdateButtonsSensitivity();
        }

        private void BtnDecrease_Clicked(object sender, EventArgs e)
        {
            if (SaleContext.ItemsPage.SelectedItem == null)
            {
                return;
            }

            SaleContext.ItemsPage.DecreaseQuantity(SaleContext.ItemsPage.SelectedItem.Article.Id);
            UpdateButtonsSensitivity();
        }

        private void BtnIncrease_Clicked(object sender, EventArgs e)
        {
            if (SaleContext.ItemsPage.SelectedItem == null)
            {
                return;
            }

            SaleContext.ItemsPage.IncreaseQuantity(SaleContext.ItemsPage.SelectedItem.Article.Id);
        }

        private void BtnQuantity_Clicked(object sender, EventArgs e)
        {
            if (SaleContext.ItemsPage.SelectedItem == null)
            {
                return;
            }

            decimal newQuantity = PosKeyboardDialog.RequestDecimalValue(POSWindow.Instance, SaleContext.ItemsPage.SelectedItem.Quantity);

            if (newQuantity == 0)
            {
                return;
            }

            SaleContext.ItemsPage.ChangeItemQuantity(SaleContext.ItemsPage.SelectedItem, newQuantity);
        }

        private void BtnPrice_Clicked(object sender, EventArgs e)
        {
            if (SaleContext.ItemsPage.SelectedItem == null)
            {
                return;
            }

            InsertMoneyModalResponse result = InsertMoneyModal.RequestDecimalValue(POSWindow.Instance,
                                                                                   GeneralUtils.GetResourceByName("window_title_dialog_moneypad_product_price"),
                                                                                   SaleContext.ItemsPage.SelectedItem.UnitPrice);

            if (result.Response != ResponseType.Ok)
            {
                return;
            }

            SaleContext.ItemsPage.ChangeItemPrice(SaleContext.ItemsPage.SelectedItem, result.Value);
        }

        private void BtnFinishOrder_Clicked(object sender, EventArgs e)
        {
            if (SaleContext.ItemsPage.Ticket == null || SaleContext.ItemsPage.Ticket.Items.Any() == false)
            {
                return;
            }

            if (SaleContext.CurrentOrder.Id.HasValue == false)
            {
                var saveOrderResult = OrdersService.SavePosOrder(SaleContext.CurrentOrder);

                if (saveOrderResult == false)
                {
                    CustomAlerts.Error(POSWindow.Instance)
                                .WithMessage("Não foi possível finalizar o pedido. Tente novamente.")
                                .ShowAlert();
                    return;
                }
            }
            else
            {
                var saveTicketResult = OrdersService.SavePosTicket(SaleContext.CurrentOrder, SaleContext.ItemsPage.Ticket);

                if (saveTicketResult == false)
                {
                    CustomAlerts.Error(POSWindow.Instance)
                                .WithMessage("Não foi possível finalizar o pedido. Tente novamente.")
                                .ShowAlert();
                    return;
                }
            }

            ThermalPrintingService.PrintTicket(SaleContext.ItemsPage.Ticket, SaleContext.CurrentTable);
            LastTicket= SaleContext.ItemsPage.Ticket;
            SaleContext.ItemsPage.FinishTicket();

            UpdateButtonsSensitivity();
        }

        private void BtnPayments_Clicked(object sender, EventArgs e)
        {
            if (SaleContext.CurrentOrder == null || SaleContext.CurrentOrder.Tickets.Any() == false)
            {
                return;
            }

            if (SaleContext.ItemsPage.Ticket != null)
            {
                ResponseType dialogResponse = CustomAlerts.Question(POSWindow.Instance)
                                                          .WithSize(new Size(400, 280))
                                                          .WithTitleResource("global_warning")
                                                          .WithMessage(GeneralUtils.GetResourceByName("dialog_message_request_close_open_ticket"))
                                                          .ShowAlert();

                if (dialogResponse != ResponseType.Yes)
                {
                    return;
                }

                SaleContext.ItemsPage.FinishTicket();
            }

            var modal = new PaymentsModal(SourceWindow);
            ResponseType response = (ResponseType)modal.Run();
            modal.Destroy();
        }

        private void BtnBarcode_Clicked(object sender, EventArgs e)
        {
            string fileWindowIcon = AppSettings.Paths.Images + @"Icons\Windows\icon_window_input_text_barcode.png";
            logicpos.Utils.ResponseText response = logicpos.Utils.GetInputText(POSWindow.Instance, DialogFlags.Modal, fileWindowIcon, GeneralUtils.GetResourceByName("global_barcode_articlecode"), string.Empty, RegularExpressions.AlfaNumericExtended, true);
            
            if (response.ResponseType != ResponseType.Ok)
            {
                return;
            }

            var code = response.Text;

            var article = ArticlesService.GetArticleByCode(code);

            if (article == null)
            {
                CustomAlerts.Warning(POSWindow.Instance)
                            .WithMessage("Artigo não encontrado")
                            .ShowAlert();
                return;
            }

            SaleContext.ItemsPage.AddItem(new SaleItem(article));
        }

        private void BtnCardCode_Clicked(object sender, EventArgs e)
        {
            string fileWindowIcon = AppSettings.Paths.Images + @"Icons\Windows\icon_pos_ticketpad_card_entry.png";
            logicpos.Utils.ResponseText dialogResponse = logicpos.Utils.GetInputText(POSWindow.Instance,
                                                                                     DialogFlags.Modal,
                                                                                     fileWindowIcon,
                                                                                     GeneralUtils.GetResourceByName("global_cardcode_small"),
                                                                                     string.Empty,
                                                                                     RegularExpressions.AlfaNumericExtended,
                                                                                     true);
        }

        private void BtnListOrder_Clicked(object sender, EventArgs e)
        {
            var modal=new SaleOrderModal(POSWindow.Instance, LastTicket);
            modal.Run();
            modal.Destroy();
            UpdateButtonsSensitivity();

        }

        private void BtnChangeTable_Clicked(object sender, EventArgs e)
        {
            GeneralUtils.ShowNotImplementedMessage();
        }

        private void BtnListMode_Clicked(object sender, EventArgs e)
        {
            GeneralUtils.ShowNotImplementedMessage();
        }

        private void BtnGifts_Clicked(object sender, EventArgs e)
        {
            GeneralUtils.ShowNotImplementedMessage();
        }

        private void BtnWeight_Clicked(object sender, EventArgs e)
        {
            GeneralUtils.ShowNotImplementedMessage();
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

        private void BtnSplitAccount_Clicked(object sender, System.EventArgs e)
        {
            var modal = new SplitAccountModal(SourceWindow, SaleContext.CurrentOrder);
            ResponseType response = (ResponseType)modal.Run();
            modal.Destroy();
            
                
        }
    }
}
