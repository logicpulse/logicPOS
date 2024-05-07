using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets;
using logicpos.shared.App;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Classes.Orders;
using System;
using System.Drawing;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosOrdersDialog
    {
        private void buttonTableConsult_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Get Current OrderMain
                OrderMain currentOrderMain = SharedFramework.SessionApp.OrdersMain[SharedFramework.SessionApp.CurrentOrderMainOid];
                //Initialize ArticleBag to Send to ProcessFinanceDocuments or Compare
                ArticleBag articleBag = ArticleBag.TicketOrderToArticleBag(currentOrderMain);

                //Get Latest DocumentConference Document without Recreate it if Diference, compare it in Above Line
                fin_documentfinancemaster lastDocument = FinancialLibraryUtils.GetOrderMainLastDocumentConference(false);

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
                        fin_documentfinancemaster newDocument = FinancialLibraryUtils.GetOrderMainLastDocumentConference(true);

                        //Call Print New Document
                        FrameworkCalls.PrintFinanceDocument(this, newDocument);
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = string.Empty;

                        switch (ex.Message)
                        {
                            case "ERROR_MISSING_SERIE":
                                errorMessage = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_creating_financial_document"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_creating_financial_document_missing_series"));
                                break;
                            case "ERROR_COMMIT_FINANCE_DOCUMENT_PAYMENT":
                            default:
                                errorMessage = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_error_creating_financial_document"), ex.Message);
                                break;
                        }
                        logicpos.Utils.ShowMessageTouch(
                          _sourceWindow,
                          DialogFlags.Modal,
                          new Size(600, 400),
                          MessageType.Error,
                          ButtonsType.Close,
                          CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error"),
                          errorMessage
                        );

                        this.Run();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            finally
            {
                this.Destroy();
            }
        }

        private void buttonPrintOrder_Clicked(object sender, EventArgs e)
        {
            if (logicpos.Utils.ShowMessageTouchRequiredValidPrinter(this, DataLayerFramework.LoggedTerminal.ThermalPrinter)) return;
            OrderMain currentOrderMain = SharedFramework.SessionApp.OrdersMain[SharedFramework.SessionApp.CurrentOrderMainOid];
            Guid orderTicketOid = new Guid();

            string sql = string.Format(@"SELECT COUNT(*) AS Count FROM fin_documentorderticket WHERE OrderMain = '{0}';", currentOrderMain.PersistentOid);
            var countTickets = XPOSettings.Session.ExecuteScalar(sql);

            //If has more than one ticket show requestTicket dialog
            if (countTickets != null && Convert.ToInt16(countTickets) > 1)
            {
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("OrderMain = '{0}'", currentOrderMain.PersistentOid));
                PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentOrderTicket>
                  dialog = new PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewDocumentOrderTicket>(
                    this.SourceWindow,
                    DialogFlags.DestroyWithParent,
                    CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_select_ticket"),
                    //TODO:THEME
                    GlobalApp.MaxWindowSize,
                    null, //XpoDefaultValue
                    criteria,
                    GenericTreeViewMode.Default,
                    null  //pActionAreaButtons
                  );

                int response = dialog.Run();
                if (response == (int)ResponseType.Ok)
                {
                    orderTicketOid = dialog.GenericTreeView.DataSourceRow.Oid;
                }
                dialog.Destroy();
            }
            //Else Print Unique Ticket
            else
            {
                sql = string.Format(@"SELECT Oid FROM fin_documentorderticket WHERE OrderMain = '{0}';", currentOrderMain.PersistentOid);
                //_logger.Debug(string.Format("sql: [{0}]", sql));
                orderTicketOid = XPOHelper.GetGuidFromQuery(sql);
            }

            if (orderTicketOid != new Guid())
            {
                fin_documentorderticket orderTicket = (fin_documentorderticket)XPOSettings.Session.GetObjectByKey(typeof(fin_documentorderticket), orderTicketOid);
                //POS front-end - Consulta Mesa + Impressão Ticket's + Gerar PDF em modo Thermal Printer [IN009344]
                ThermalPrinterInternalDocumentOrderRequest thermalPrinterInternalDocumentOrderRequest = new ThermalPrinterInternalDocumentOrderRequest(DataLayerFramework.LoggedTerminal.ThermalPrinter, orderTicket);
                thermalPrinterInternalDocumentOrderRequest.Print();
            }
        }
    }
}
