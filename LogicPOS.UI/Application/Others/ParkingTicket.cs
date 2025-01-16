using System;
using System.Net;

namespace logicpos.Classes.Logic.Others
{
    // TK013134
    public class ParkingTicket
    {
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Constructor
        public ParkingTicket()
        {
        }

        /// <summary>
        /// Checks for existent tickets in current order and call Access.Track web services for parking ticket details.
        /// </summary>
        /// <param name="ean"></param>
        public void GetTicketDetailFromWS(string ean)
        {
            //_logger.Debug("void GetTicketDetailFromWS([ " + ean + " ])");

            //bool hasOrder = null != POSWindow.Instance.TicketList.CurrentOrderDetail.Lines;
            //bool ticketExists = false;

            //if (hasOrder) /* Checks for duplicates in current order */
            //{
            //    foreach (var line in POSWindow.Instance.TicketList.CurrentOrderDetail.Lines)
            //    {
            //        if (line.Designation.Contains(ean))
            //        {
            //            ticketExists = true;
            //            break;
            //        }
            //    }
            //}

            //if (!ticketExists)
            //{
            //    ParkingTicketResult parkingTicketResult = GetTicketInformation(ean);

            //    // http://ws.test.cloud.time.track.pt/service.asmx?op=payTicket

            //    //Always Change Mode to Ticket
            //    if (POSWindow.Instance.TicketList.ListMode != TicketListMode.Ticket)
            //    {
            //        POSWindow.Instance.TicketList.ListMode = TicketListMode.Ticket;
            //        POSWindow.Instance.TicketList.UpdateModel();
            //    }

            //    if (GeneralSettings.AppUseParkingTicketModule && parkingTicketResult == null)
            //    {

            //    }
            //    else if (GeneralSettings.AppUseParkingTicketModule && parkingTicketResult.Date == null && ean.Length == 13)
            //    {
            //        POSWindow.Instance.TicketList.ArticleNotFound();
            //    }
            //    //IN009279 Parking ticket Service - implementar Cartão cliente
            //    else if (parkingTicketResult.Ean.Length == 13)
            //    {
            //        POSWindow.Instance.UpdateWorkSessionUI();
            //        POSWindow.Instance.TicketList.UpdateOrderStatusBar();
            //        POSWindow.Instance.TicketList.InsertOrUpdate(XPOSettings.XpoOidArticleParkingTicket, parkingTicketResult);
            //    }
            //    else
            //    {
            //        POSWindow.Instance.TicketList.InsertOrUpdate(XPOSettings.XpoOidArticleParkingCard, parkingTicketResult);
            //    }
            //}
        }

    
    }
}