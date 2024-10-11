using logicpos.Classes.Enums.TicketList;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Settings;
using LogicPOS.UI;
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
            _logger.Debug("void GetTicketDetailFromWS([ " + ean + " ])");

            bool hasOrder = null != GlobalApp.PosMainWindow.TicketList.CurrentOrderDetail.Lines;
            bool ticketExists = false;

            if (hasOrder) /* Checks for duplicates in current order */
            {
                foreach (var line in GlobalApp.PosMainWindow.TicketList.CurrentOrderDetail.Lines)
                {
                    if (line.Designation.Contains(ean))
                    {
                        ticketExists = true;
                        break;
                    }
                }
            }

            if (!ticketExists)
            {
                ParkingTicketResult parkingTicketResult = GetTicketInformation(ean);

                // http://ws.test.cloud.time.track.pt/service.asmx?op=payTicket

                //Always Change Mode to Ticket
                if (GlobalApp.PosMainWindow.TicketList.ListMode != TicketListMode.Ticket)
                {
                    GlobalApp.PosMainWindow.TicketList.ListMode = TicketListMode.Ticket;
                    GlobalApp.PosMainWindow.TicketList.UpdateModel();
                }

                if (GeneralSettings.AppUseParkingTicketModule && parkingTicketResult == null)
                {

                }
                else if (GeneralSettings.AppUseParkingTicketModule && parkingTicketResult.Date == null && ean.Length == 13)
                {
                    GlobalApp.PosMainWindow.TicketList.ArticleNotFound();
                }
                //IN009279 Parking ticket Service - implementar Cartão cliente
                else if (parkingTicketResult.Ean.Length == 13)
                {
                    GlobalApp.PosMainWindow.UpdateWorkSessionUI();
                    GlobalApp.PosMainWindow.TicketList.UpdateOrderStatusBar();
                    GlobalApp.PosMainWindow.TicketList.InsertOrUpdate(XPOSettings.XpoOidArticleParkingTicket, parkingTicketResult);
                }
                else
                {
                    GlobalApp.PosMainWindow.TicketList.InsertOrUpdate(XPOSettings.XpoOidArticleParkingCard, parkingTicketResult);
                }
            }
        }

        /// <summary>
        /// Method responsible for communicates to "AccessTrackParkingTicketService.TimeService" to retrieve parking tivket details.
        /// </summary>
        /// <param name="ean"></param>
        /// <returns></returns>
        /// 
        private ParkingTicketResult GetTicketInformation(string ean)
        {
            ParkingTicketResult parkingTicketResult = new ParkingTicketResult();
            AccessTrackParkingTicketService.TimeService accessTrackParkingTicketService = new AccessTrackParkingTicketService.TimeService();
            //IN009279 Parking ticket Service - implementar Cartão cliente
            if (ean.Length == 13)
            {

                try
                {
                    //accessTrackParkingTicketService.Url = System.Configuration.ConfigurationManager.AppSettings["wsParkingURL"].ToString();
                    //accessTrackParkingTicketService.Url = "http://localhost/wstimetrack/service.asmx".ToString();
                    _logger.Debug("ParkingTicket URL([ " + accessTrackParkingTicketService.Url + " ]) :: ");
                    System.Data.DataTable accessTrackParkingTicketServiceResult = accessTrackParkingTicketService.getTicketInformation(ean);

                    /* Business rules defined that only one ticket will be created at a time */
                    foreach (System.Data.DataRow ticketInformationItem in accessTrackParkingTicketServiceResult.Rows)
                    {
                        parkingTicketResult = ExtractTicketInformation(ticketInformationItem);
                        parkingTicketResult.Ean = ean;
                    }
                }
                catch (WebException ex)
                {
                    _logger.Error("ParkingTicketResult GetTicketInformation([ " + ean + " ]) :: " + ex.Message, ex);
                    GlobalApp.PosMainWindow.TicketList.WsNotFound();
                    parkingTicketResult = null;
                    // throw ex;
                }
            }
            else
            {
                //IN009279
                if (!accessTrackParkingTicketService.getCardInformation(ean))
                {
                    DateTime localDate = DateTime.Now;
                    string dateNow = localDate.ToString();
                    _logger.Debug("ParkingTicket URL([ " + accessTrackParkingTicketService.Url + " ]) :: ");
                    accessTrackParkingTicketService.addInCard(ean, dateNow);
                }
                string sql = "SELECT Price1 FROM[logicposdb].[dbo].[fin_article] where Oid = '32829702-33fa-48d5-917c-4c1db8720777'";
                var getCardPrice = XPOSettings.Session.ExecuteScalar(sql);
                parkingTicketResult.Price = Convert.ToInt32(getCardPrice);
                parkingTicketResult.Ean = ean;
                string sql2 = "SELECT DefaultQuantity FROM[logicposdb].[dbo].[fin_article] where Oid = '32829702-33fa-48d5-917c-4c1db8720777'";
                var getDefaultQuantitysql = XPOSettings.Session.ExecuteScalar(sql2);

                int quantity = Convert.ToInt32(getDefaultQuantitysql);
                parkingTicketResult.Quantity = quantity.ToString();
                parkingTicketResult.Quantity = string.Format("{0}", quantity.ToString());
            }

            return parkingTicketResult;
        }

        private static ParkingTicketResult ExtractTicketInformation(System.Data.DataRow ticketInformationItem)
        {

            ParkingTicketResult parkingTicketResult = new ParkingTicketResult
            {
                Date = ticketInformationItem["date"].ToString(),
                Minutes = ticketInformationItem["minutes"].ToString(),
                Quantity = ticketInformationItem["quantity"].ToString(),
                Price = decimal.Parse(ticketInformationItem["price"].ToString()),
                AlreadyPaid = Convert.ToBoolean(ticketInformationItem["alreadyPaid"]),
                AlreadyExit = Convert.ToBoolean(ticketInformationItem["alreadyExit"].ToString()),
                Description = ticketInformationItem["description"].ToString(),
                DatePaid = ticketInformationItem["datePaid"].ToString(),
                DateExits = ticketInformationItem["dateExit"].ToString(),
                DateTolerance = ticketInformationItem["dateTolerance"].ToString()
            };

            return parkingTicketResult;
        }
    }
}