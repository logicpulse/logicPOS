using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Gui.Gtk.BackOffice;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Settings.Terminal;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Finance.DocumentProcessing;
using LogicPOS.Globalization;
using LogicPOS.Printing.Documents;
using LogicPOS.Shared;
using LogicPOS.Shared.Article;
using LogicPOS.Shared.Orders;
using LogicPOS.UI;
using LogicPOS.UI.Components;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosOrdersDialog
    {
        private void buttonTableConsult_Clicked(object sender, EventArgs e)
        {

            //Get Current OrderMain
            OrderMain currentOrderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
            //Initialize ArticleBag to Send to ProcessFinanceDocuments or Compare
            ArticleBag articleBag = ArticleBag.TicketOrderToArticleBag(currentOrderMain);

            //Get Latest DocumentConference Document without Recreate it if Diference, compare it in Above Line
            fin_documentfinancemaster lastDocument = DocumentProcessingUtils.GetOrderMainLastDocumentConference(false);

            //Reprint Existing Document After compare with current ArticleBag
            if (
                lastDocument != null && articleBag != null &&
                lastDocument.TotalFinal.Equals(articleBag.TotalFinal) && lastDocument.DocumentDetail.Count.Equals(articleBag.Count)
            )
            {
                FrameworkCalls.PrintFinanceDocument(this, lastDocument);
            }
            //Else Create new DocumentConference recalling FrameworkUtils.GetOrderMainLastDocumentConference with true to Create New One
            else
            {
                try
                {
                    //Call Recreate New Document
                    fin_documentfinancemaster newDocument = DocumentProcessingUtils.GetOrderMainLastDocumentConference(true);

                    //Call Print New Document
                    FrameworkCalls.PrintFinanceDocument(this, newDocument);
                }
                catch (Exception ex)
                {
                    string errorMessage = string.Empty;

                    switch (ex.Message)
                    {
                        case "ERROR_MISSING_SERIE":
                            errorMessage = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_error_creating_financial_document"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_error_creating_financial_document_missing_series"));
                            break;
                        case "ERROR_COMMIT_FINANCE_DOCUMENT_PAYMENT":
                        default:
                            errorMessage = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_error_creating_financial_document"), ex.Message);
                            break;
                    }
                    logicpos.Utils.ShowMessageBox(
                      WindowSettings.Source,
                      DialogFlags.Modal,
                      new Size(600, 400),
                      MessageType.Error,
                      ButtonsType.Close,
                      CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_error"),
                      errorMessage
                    );

                    this.Run();
                }
            }

            this.Destroy();
        }

        private void buttonPrintOrder_Clicked(object sender, EventArgs e)
        {
            if (logicpos.Utils.ShowMessageTouchRequiredValidPrinter(this, TerminalSettings.LoggedTerminal.ThermalPrinter)) return;
            OrderMain currentOrderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
            Guid orderTicketOid = new Guid();

            string sql = string.Format(@"SELECT COUNT(*) AS Count FROM fin_documentorderticket WHERE OrderMain = '{0}';", currentOrderMain.PersistentOid);
            var countTickets = XPOSettings.Session.ExecuteScalar(sql);

            //If has more than one ticket show requestTicket dialog
            if (countTickets != null && Convert.ToInt16(countTickets) > 1)
            {
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("OrderMain = '{0}'", currentOrderMain.PersistentOid));
                PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentOrderTicket>
                  dialog = new PosSelectRecordDialog<XPCollection, Entity, TreeViewDocumentOrderTicket>(
                    this.Source,
                    DialogFlags.DestroyWithParent,
                    CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_select_ticket"),
                    //TODO:THEME
                    GlobalApp.MaxWindowSize,
                    null, //XpoDefaultValue
                    criteria,
                    GridViewMode.Default,
                    null  //pActionAreaButtons
                  );

                int response = dialog.Run();
                if (response == (int)ResponseType.Ok)
                {
                    orderTicketOid = dialog.GenericTreeView.Entity.Oid;
                }
                dialog.Destroy();
            }
            //Else Print Unique Ticket
            else
            {
                sql = string.Format(@"SELECT Oid FROM fin_documentorderticket WHERE OrderMain = '{0}';", currentOrderMain.PersistentOid);
                //_logger.Debug(string.Format("sql: [{0}]", sql));
                orderTicketOid = XPOUtility.GetGuidFromQuery(sql);
            }

            if (orderTicketOid != new Guid())
            {
                fin_documentorderticket orderTicket = (fin_documentorderticket)XPOSettings.Session.GetObjectByKey(typeof(fin_documentorderticket), orderTicketOid);
                //POS front-end - Consulta Mesa + Impressão Ticket's + Gerar PDF em modo Thermal Printer [IN009344]
                var ThermalPrinter = LoggedTerminalSettings.GetPrinterDto();
                var orderTicketDto = MappingUtils.GetPrintOrderTicketDto(orderTicket);
                OrderRequest thermalPrinterInternalDocumentOrderRequest = new OrderRequest(ThermalPrinter, orderTicketDto);
                thermalPrinterInternalDocumentOrderRequest.Print();
            }
        }
    }
}
