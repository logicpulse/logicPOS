using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.TicketList;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.datalayer.Enums;
using logicpos.shared.Classes.Orders;
using System;
using System.Linq;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    partial class TicketPad
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Tables Button
        void buttonKeySelectTable_Clicked(object sender, EventArgs e)
        {
            PosTablesDialog dialog = new PosTablesDialog(this.SourceWindow, DialogFlags.DestroyWithParent);
            ResponseType response = (ResponseType)dialog.Run();

            if (response == ResponseType.Ok || response == ResponseType.Cancel || response == ResponseType.DeleteEvent)
            {
                if (response == ResponseType.Ok)
                {
                    SelectTableOrder(dialog.CurrentTableOid);
                    _ticketList.UpdateArticleBag();
                    _ticketList.UpdateTicketListOrderButtons();
                    _ticketList.UpdateOrderStatusBar();
                }
                dialog.Destroy();
            };
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods
        public void SelectTableOrder(Guid pTableOid)
        {
            OrderMain currentOrderMain = null;

            //Try to Get OrderMain Object From TableId Parameter
            if (GlobalFramework.SessionApp.OrdersMain.Count > 0)
            {
                //Get OrderMain Object from CurrentTableId with LINQ
                currentOrderMain = GlobalFramework.SessionApp.OrdersMain.Values.Where<OrderMain>(key => key.Table.Oid == pTableOid).FirstOrDefault<OrderMain>();
            }

            //Always start in OrderMain Mode, when we change Table, IF WE HAVE Tickets, else leave it in TicketMode, Else Alt.List button may be OFF, and we cant Toggle Mode, Only occur if we dont have Tickets or Items in OrderMain
            if (currentOrderMain != null && Convert.ToInt16(currentOrderMain.OrderStatus) != -1)
            {
                _ticketList.ListMode = TicketListMode.OrderMain;
            }
            else
            {
                _ticketList.ListMode = TicketListMode.Ticket;
            }

            //Create a Fresh Order for pTableId
            if (currentOrderMain == null)
            {
                Guid newOrderMainOid = Guid.NewGuid();
                GlobalFramework.SessionApp.OrdersMain.Add(newOrderMainOid, new OrderMain(newOrderMainOid, pTableOid));
                OrderMain newOrderMain = GlobalFramework.SessionApp.OrdersMain[newOrderMainOid];
                OrderTicket orderTicket = new OrderTicket(newOrderMain, (PriceType)newOrderMain.Table.PriceType);
                //Create Reference to SessionApp OrderMain with Open Ticket, Ready to Add Details
                newOrderMain.OrderTickets.Add(1, orderTicket);
                //Create Reference to be used in Shared Code
                currentOrderMain = newOrderMain;
            }

            //ALWAYS Update current PersistentOid and Status from database
            currentOrderMain.PersistentOid = currentOrderMain.GetOpenTableFieldValueGuid(pTableOid, "Oid");
            currentOrderMain.OrderStatus = (OrderStatus)currentOrderMain.GetOpenTableFieldValue(pTableOid, "OrderStatus");

            //Shared Code
            GlobalFramework.SessionApp.CurrentOrderMainOid = currentOrderMain.Table.OrderMainOid;
            GlobalFramework.SessionApp.Write();
            _ticketList.UpdateModel();

            //Update PosMainWindow Components
            GlobalApp.WindowPos.TablePadArticle.Sensitive = true;
        }
    }
}