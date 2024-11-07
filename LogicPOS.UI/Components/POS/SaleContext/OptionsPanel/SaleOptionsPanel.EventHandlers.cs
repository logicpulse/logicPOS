using Gtk;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.shared.Enums;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.GetArticleByCode;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.DTOs.Printing;
using LogicPOS.Printing.Documents;
using LogicPOS.Printing.Utility;
using LogicPOS.Settings;
using LogicPOS.Shared;
using LogicPOS.Shared.Orders;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using PriceType = LogicPOS.Domain.Enums.PriceType;

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
            ResponseType responseType = logicpos.Utils.ShowMessageBox(LogicPOSAppContext.PosMainWindow,
                                                                      DialogFlags.Modal,
                                                                      new System.Drawing.Size(400, 280),
                                                                      MessageType.Question,
                                                                      ButtonsType.YesNo,
                                                                      string.Format(GeneralUtils.GetResourceByName("global_warning"), GeneralSettings.ServerVersion),
                                                                      GeneralUtils.GetResourceByName("dialog_message__pos_order_cancel"));
            if (responseType != ResponseType.Yes && responseType != ResponseType.Ok)
            {
                return;
            }

            SaleContext.ItemsPage.Clear(true);
            SaleContext.CurrentOrder.Clear();
            SaleContext.ItemsPage.SetTicketModeBackGround();
            SaleContext.UpdatePOSLabels();
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
            if (ItemsPage.SelectedItem == null)
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

            decimal newQuantity = PosKeyboardDialog.RequestDecimalValue(LogicPOSAppContext.PosMainWindow, ItemsPage.SelectedItem.Quantity);

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

            InsertMoneyModalResponse result = InsertMoneyModal.RequestDecimalValue(LogicPOSAppContext.PosMainWindow,
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
            if (ItemsPage.Ticket == null || ItemsPage.Ticket.Items.Any() == false)
            {
                return;
            }
            PrintOrder();
            ItemsPage.FinishTicket();
            UpdateButtonsSensitivity();

        }

        private void PrintOrder()
        {
            var orderTicket = new PrintOrderTicketDto();
            orderTicket.OrderDetails = new List<PrintOrderDetailDto>();

            orderTicket.TicketId = (int)ItemsPage.Ticket.Number;
            orderTicket.TableDesignation = ItemsPage.Order.Table.Designation;
            orderTicket.PlaceDesignation = ItemsPage.Order.Table.Place.Designation;
            foreach (var item in ItemsPage.Ticket.Items)
            {
                orderTicket.OrderDetails.Add(new PrintOrderDetailDto() { Designation = item.Article.Designation, Quantity = item.Quantity, UnitMeasure = item.Article.MeasurementUnit.Acronym });
            }
            PrinterDto printer = GetTerminalThermalPrinter(TerminalService.Terminal);
            if (printer == null)
            {
                return;
            }
            CompanyInformationsDto companyInformationsDto = GetCompanyInformation();
            new ThermalPrinting(printer, companyInformationsDto, orderTicket, TerminalService.Terminal.Designation, AuthenticationService.User.Name);

        }


        protected PrinterDto GetTerminalThermalPrinter(Terminal terminal)
        {

            if (terminal.ThermalPrinter != null)
            {
                return new PrinterDto()
                {
                    Designation = terminal.ThermalPrinter.Designation,
                    Token = terminal.ThermalPrinter.Type.Token,
                    IsThermal = terminal.ThermalPrinter.Type.ThermalPrinter,
                    CutCommand = "0x42,0x00"
                };
            }
            else
            {
                return null;
            }
        }

        private CompanyInformationsDto GetCompanyInformation()
        {
            var companyInformations = new CompanyInformations();
            return new CompanyInformationsDto()
            {
                Address = companyInformations.Address,
                Name = companyInformations.Name,
                BusinessName = companyInformations.BusinessName,
                ComercialName = companyInformations.ComercialName,
                City = companyInformations.City,
                Logo = companyInformations.Logo,

                Email = companyInformations.Email,
                MobilePhone = companyInformations.MobilePhone,
                FiscalNumber = companyInformations.FiscalNumber,
                Phone = companyInformations.Phone,
                StockCapital = companyInformations.StockCapital,
                Website = companyInformations.Website,
                DocumentFinalLine1 = companyInformations.DocumentFinalLine1,
                DocumentFinalLine2 = companyInformations.DocumentFinalLine2,
                TicketFinalLine1 = companyInformations.TicketFinalLine1,
                TicketFinalLine2 = companyInformations.TicketFinalLine2,
            };
        }



        private void BtnPayments_Clicked(object sender, EventArgs e)
        {
            if (ItemsPage.Order == null || ItemsPage.Order.Tickets.Any() == false)
            {
                return;
            }

            if (ItemsPage.Ticket != null)
            {
                ResponseType dialogResponse = logicpos.Utils.ShowMessageTouch(LogicPOSAppContext.PosMainWindow,
                                                                              DialogFlags.DestroyWithParent,
                                                                              MessageType.Question,
                                                                              ButtonsType.OkCancel,
                                                                              GeneralUtils.GetResourceByName("window_title_dialog_message_dialog"),
                                                                              GeneralUtils.GetResourceByName("dialog_message_request_close_open_ticket"));
                if (dialogResponse != ResponseType.Ok)
                {
                    return;
                }

                ItemsPage.FinishTicket();
            }

            var modal = new PaymentsModal(SourceWindow);
            modal.Run();
            modal.Destroy();
        }

        private void BtnBarcode_Clicked(object sender, EventArgs e)
        {
            string fileWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_input_text_barcode.png";
            logicpos.Utils.ResponseText response = logicpos.Utils.GetInputText(LogicPOSAppContext.PosMainWindow, DialogFlags.Modal, fileWindowIcon, GeneralUtils.GetResourceByName("global_barcode_articlecode"), string.Empty, RegexUtils.RegexAlfaNumericExtended, true);

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
            logicpos.Utils.ResponseText dialogResponse = logicpos.Utils.GetInputText(LogicPOSAppContext.PosMainWindow, DialogFlags.Modal, fileWindowIcon, GeneralUtils.GetResourceByName("global_cardcode_small"), string.Empty, RegexUtils.RegexAlfaNumericExtended, true);

            if (dialogResponse.ResponseType == ResponseType.Ok)
            {
                if (GeneralSettings.AppUseParkingTicketModule) /* IN009239 */
                {
                    LogicPOSAppContext.ParkingTicket.GetTicketDetailFromWS(dialogResponse.Text);
                }
                else
                {

                }
            }
        }

        private void BtnListOrder_Clicked(object sender, EventArgs e)
        {
            OrderMain currentOrderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
            PosOrdersDialog dialog = new PosOrdersDialog(LogicPOSAppContext.PosMainWindow, DialogFlags.DestroyWithParent, currentOrderMain.Table.Name);
            ResponseType response = (ResponseType)dialog.Run();
            dialog.Destroy();

        }

        private void BtnChangeTable_Clicked(object sender, EventArgs e)
        {
            PosTablesDialog dialog = new PosTablesDialog(LogicPOSAppContext.PosMainWindow, DialogFlags.DestroyWithParent, TableFilterMode.OnlyFreeTables);
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
                            LogicPOSAppContext.PosMainWindow, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_error"),
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
