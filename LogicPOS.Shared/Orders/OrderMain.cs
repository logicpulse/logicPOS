﻿using DevExpress.Xpo;
using logicpos.shared.Enums;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Settings.Enums;
using LogicPOS.Shared.Article;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LogicPOS.Shared.Orders
{
    public class OrderMain
    {
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int CurrentTicketId { get; set; }

        private Guid _persistentOid;
        public Guid PersistentOid
        {
            get { return _persistentOid; }
            set { _persistentOid = value; }
        }

        public OrderStatus OrderStatus { get; set; }

        public OrderMainTable Table { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Dictionary<int, OrderTicket> OrderTickets { get; set; }

        [JsonIgnore]
        public decimal GlobalTotalGross { get; set; }

        [JsonIgnore]
        public decimal GlobalTotalDiscount { get; set; }

        [JsonIgnore]
        public decimal GlobalTotalTax { get; set; }

        [JsonIgnore]
        public decimal GlobalTotalFinal { get; set; }

        [JsonIgnore]
        public decimal GlobalTotalQuantity { get; set; }

        [JsonIgnore]
        public int GlobalTotalTickets { get; set; }

        [JsonIgnore]
        public sys_userdetail GlobalLastUser { get; set; }

        [JsonIgnore]
        public pos_configurationplaceterminal GlobalLastTerminal { get; set; }

        //Required Parameterless Constructor for Json.NET (Load)
        public OrderMain() { }
        //Constructor without Json.NET Load, With Defaults
        public OrderMain(Guid pOrderMainOid, Guid pTableOid)
        {
            CurrentTicketId = 1;
            _persistentOid = new Guid();
            OrderStatus = OrderStatus.Null;
            Table = new OrderMainTable(pOrderMainOid, pTableOid);
            DateStart = XPOUtility.CurrentDateTimeAtomic();
            OrderTickets = new Dictionary<int, OrderTicket>();
        }

        /// <summary>
        /// Finish Order, Clean Session OrderTicket and OrderDetails, Persist in Database
        /// </summary>
        public fin_documentorderticket FinishOrder(Session pSession)
        {
            return FinishOrder(pSession, true);
        }

        public fin_documentorderticket FinishOrder(Session pSession, bool pPrintTicket)
        {
            return FinishOrder(pSession, pPrintTicket, false);
        }


        public fin_documentorderticket FinishOrder(Session pSession, bool pPrintTicket, bool pTicketDrecrease)
        {
            //Local Vars
            DateTime currentDateTime = DateTime.Now;
            fin_documentordermain xOrderMain;
            Session _sessionXpo = pSession;
            bool isInUOW = (_sessionXpo.GetType() == typeof(UnitOfWork));

            //Get current Working Order from SessionApp
            OrderMain currentOrderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
            OrderTicket currentOrderTicket = currentOrderMain.OrderTickets[currentOrderMain.CurrentTicketId];

            //Get Place Object to extract TaxSellType Normal|TakeWay
            pos_configurationplace configurationPlace = (pos_configurationplace)XPOSettings.Session.GetObjectByKey(typeof(pos_configurationplace), currentOrderMain.Table.PlaceId);
            //Use VatDirectSelling if in Retail or in TakeWay mode
            TaxSellType taxSellType = (AppOperationModeSettings.AppMode == AppOperationMode.Retail || configurationPlace.MovementType.VatDirectSelling) ? TaxSellType.TakeAway : TaxSellType.Normal;

            //Open Table on First Finish OrderTicket
            pos_configurationplacetable xTable = XPOUtility.GetEntityById<pos_configurationplacetable>(Table.Oid, _sessionXpo);
            //Proteção para mesas vazias, escolhe a primeira
            if (xTable == null)
            {
                //Order #1 by default
                //TicketPad - Modo Retalho - Mesa/ordem por defeito [IN:016529]
                xTable = ((pos_configurationplacetable)XPOUtility.GetXPGuidObjectFromCriteria(typeof(pos_configurationplacetable), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Code = '{0}')", "10")) as pos_configurationplacetable);
            }
            xTable.Reload();
            if (xTable.TableStatus != TableStatus.Open)
            {
                xTable.TableStatus = TableStatus.Open;
                XPOUtility.Audit("TABLE_OPEN", string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "audit_message_table_open"), xTable.Designation));
                xTable.DateTableOpen = XPOUtility.CurrentDateTimeAtomic();
                if (!isInUOW) xTable.Save();
            }

            //Get Current _persistentOid and _from Database
            _persistentOid = GetOpenTableFieldValueGuid(Table.Oid, "Oid");
            OrderStatus = (OrderStatus)GetOpenTableFieldValue(Table.Oid, "OrderStatus");
            UpdatedAt = XPOUtility.CurrentDateTimeAtomic();
            //Insert
            if (_persistentOid == Guid.Empty)
            {
                //OrderMain
                xOrderMain = new fin_documentordermain(_sessionXpo)
                {
                    //Always assign New date to Persistent Date
                    DateStart = currentDateTime,//currentOrderMain.DateStart,
                    OrderStatus = OrderStatus.Open,
                    PlaceTable = xTable,
                    UpdatedAt = XPOUtility.CurrentDateTimeAtomic()
                };
                if (!isInUOW) xOrderMain.Save();
                //After Save, Get Oid
                _persistentOid = xOrderMain.Oid;
                //Change to Open Status
                OrderStatus = OrderStatus.Open;
            }
            //Update
            else
            {
                xOrderMain = XPOUtility.GetEntityById<fin_documentordermain>(_persistentOid, _sessionXpo);
                if (xOrderMain.PlaceTable != xTable) xOrderMain.PlaceTable = xTable;
                //Force Changes in Record, else UpdatedAt dont Update
                xOrderMain.UpdatedAt = XPOUtility.CurrentDateTimeAtomic();
                //TODO: Check if User was Automatically Updated
                //if (xOrderMain.UpdatedBy != XPOSettings.LoggedUser) xOrderMain.UpdatedBy = XPOSettings.LoggedUser;
                if (!isInUOW) xOrderMain.Save();
            }

            //Create OrderTicket
            //if (pTicketDrecrease)
            //{
            //var sql = string.Format(@"SELECT * FROM fin_documentorderticket WHERE TicketId = '{0}' AND OrderMain = '{1}';", currentOrderMain.CurrentTicketId, currentOrderMain.PersistentOid);
            //_logger.Debug(string.Format("sql: [{0}]", sql));
            string sql = string.Format(@"SELECT Oid FROM fin_documentorderticket WHERE OrderMain = '{0}' AND TicketId = '{1}';", currentOrderMain.PersistentOid, currentOrderMain.CurrentTicketId);
            //_logger.Debug(string.Format("sql: [{0}]", sql));
            Guid orderTicketOid = XPOUtility.GetGuidFromQuery(sql);
            //Result
            fin_documentorderticket xOrderTicket = (fin_documentorderticket)XPOSettings.Session.GetObjectByKey(typeof(fin_documentorderticket), orderTicketOid);

            //xOrderTicket = (fin_documentorderticket)XPOUtility.GetXPGuidObject(_sessionXpo, typeof(fin_documentorderticket), currentOrderMain._persistentOid);
            if (xOrderTicket != null)
            {
                xOrderTicket.TicketId = currentOrderMain.CurrentTicketId;
                xOrderTicket.DateStart = currentOrderTicket.DateStart;
                xOrderTicket.PriceType = currentOrderTicket.PriceType;
                xOrderTicket.Discount = xTable.Discount;
                xOrderTicket.OrderMain = xOrderMain;
                xOrderTicket.PlaceTable = xTable;
                xOrderTicket.UpdatedAt = XPOUtility.CurrentDateTimeAtomic();
                if (!isInUOW) xOrderTicket.Save();
            }

            //}
            else
            {
                xOrderTicket = new fin_documentorderticket(_sessionXpo)
                {
                    TicketId = currentOrderMain.CurrentTicketId,
                    DateStart = currentOrderTicket.DateStart,
                    PriceType = currentOrderTicket.PriceType,
                    Discount = xTable.Discount,
                    OrderMain = xOrderMain,
                    PlaceTable = xTable
                };
                if (!isInUOW) xOrderTicket.Save();
            }


            //Create OrderDetail
            fin_documentorderdetail xOrderDetailLine;
            fin_article xArticle;
            uint itemOrd = 0;
            foreach (OrderDetailLine line in currentOrderTicket.OrderDetails.Lines)
            {
                //Use Order in print tickets etc
                itemOrd++;
                xArticle = XPOUtility.GetEntityById<fin_article>(line.ArticleOid, _sessionXpo);
                //Get PriceTax from TaxSellType
                decimal priceTax = (taxSellType == TaxSellType.Normal) ? xArticle.VatOnTable.Value : xArticle.VatDirectSelling.Value;
                //Edit/cancel orders lindote 10/07/2020
                //Get order ticket Oid from DB
                string sql3 = string.Format(@"SELECT Oid FROM fin_documentorderticket WHERE OrderMain = '{0}' AND TicketId = '{1}';", currentOrderMain.PersistentOid, currentOrderMain.CurrentTicketId);
                orderTicketOid = XPOUtility.GetGuidFromQuery(sql3);

                //Get order detail Oid from DB
                string sql4 = string.Format(@"SELECT Oid FROM fin_documentorderdetail WHERE OrderTicket = '{0}' AND Article = '{1}' AND Price = '{2}'  AND TotalDiscount = '{3}'  AND Vat = '{4}';",
                             orderTicketOid, line.ArticleOid, line.Properties.PriceNet.ToString().Replace(",", "."),
                                                              line.Properties.TotalDiscount.ToString().Replace(",", "."),
                                                              line.Properties.Vat.ToString().Replace(",", "."));
                Guid orderDetailOid = XPOUtility.GetGuidFromQuery(sql4);

                string pToken2 = "";
                if (pTicketDrecrease)
                {
                    pToken2 = "decreased";
                }

                if (orderDetailOid == Guid.Empty)
                {
                    xOrderDetailLine = new fin_documentorderdetail(_sessionXpo)
                    {
                        //Values                        
                        Ord = itemOrd,
                        Code = xArticle.Code,
                        Designation = line.Designation,
                        Quantity = line.Properties.Quantity,
                        UnitMeasure = xArticle.UnitMeasure.Acronym,
                        Price = line.Properties.PriceNet,
                        Discount = (xArticle.Discount > 0) ? xArticle.Discount : 0.0m,
                        TotalGross = line.Properties.TotalGross,
                        TotalDiscount = line.Properties.TotalDiscount,
                        TotalTax = line.Properties.TotalTax,
                        TotalFinal = line.Properties.TotalFinal,
                        Token2 = pToken2,
                        //Use PriceTax Normal|TakeAway
                        Vat = priceTax,
                        //XPGuidObjects
                        Article = xArticle,
                        OrderTicket = xOrderTicket
                    };

                    //Detect VatExemptionReason
                    if (line.Properties.VatExemptionReason != Guid.Empty)
                    {
                        xOrderDetailLine.VatExemptionReason = line.Properties.VatExemptionReason;
                    }
                    if (!isInUOW)
                    {
                        xOrderDetailLine.Save();
                    }
                }
                else
                {
                    xOrderDetailLine = XPOUtility.GetEntityById<fin_documentorderdetail>(orderDetailOid, _sessionXpo);

                    if (xOrderDetailLine.Token2 != "decreased" && !pTicketDrecrease)
                    {

                        xOrderDetailLine.Ord = itemOrd;
                        xOrderDetailLine.Code = xArticle.Code;
                        xOrderDetailLine.Designation = line.Designation;
                        xOrderDetailLine.Quantity += line.Properties.Quantity;
                        xOrderDetailLine.UnitMeasure = xArticle.UnitMeasure.Acronym;
                        xOrderDetailLine.Price = line.Properties.PriceNet;
                        xOrderDetailLine.Discount = (xArticle.Discount > 0) ? xArticle.Discount : 0.0m;
                        xOrderDetailLine.TotalGross = line.Properties.TotalGross;
                        xOrderDetailLine.TotalDiscount = line.Properties.TotalDiscount;
                        xOrderDetailLine.TotalTax = line.Properties.TotalTax;
                        xOrderDetailLine.TotalFinal = line.Properties.TotalFinal;
                        xOrderDetailLine.Token2 = pToken2;
                        //Use PriceTax Normal|TakeAway
                        xOrderDetailLine.Vat = priceTax;
                        //XPGuidObjects
                        xOrderDetailLine.Article = xArticle;
                        xOrderDetailLine.OrderTicket = xOrderTicket;

                        if (!isInUOW)
                        {
                            xOrderDetailLine.Save();
                        }
                    }
                    else
                    if (xOrderDetailLine.Token2 == "decreased" && pTicketDrecrease)
                    {

                        xOrderDetailLine.Ord = itemOrd;
                        xOrderDetailLine.Code = xArticle.Code;
                        xOrderDetailLine.Designation = line.Designation;
                        xOrderDetailLine.Quantity += line.Properties.Quantity;
                        xOrderDetailLine.UnitMeasure = xArticle.UnitMeasure.Acronym;
                        xOrderDetailLine.Price = line.Properties.PriceNet;
                        xOrderDetailLine.Discount = (xArticle.Discount > 0) ? xArticle.Discount : 0.0m;
                        xOrderDetailLine.TotalGross = line.Properties.TotalGross;
                        xOrderDetailLine.TotalDiscount = line.Properties.TotalDiscount;
                        xOrderDetailLine.TotalTax = line.Properties.TotalTax;
                        xOrderDetailLine.TotalFinal = line.Properties.TotalFinal;
                        xOrderDetailLine.Token2 = pToken2;
                        //Use PriceTax Normal|TakeAway
                        xOrderDetailLine.Vat = priceTax;
                        //XPGuidObjects
                        xOrderDetailLine.Article = xArticle;
                        xOrderDetailLine.OrderTicket = xOrderTicket;

                        if (!isInUOW)
                        {
                            xOrderDetailLine.Save();
                        }
                    }
                    else
                    if (xOrderDetailLine.Token2 == "decreased" && !pTicketDrecrease)
                    {

                        xOrderDetailLine.Ord = itemOrd;
                        xOrderDetailLine.Code = xArticle.Code;
                        xOrderDetailLine.Designation = line.Designation;
                        xOrderDetailLine.Quantity = xOrderDetailLine.Quantity;
                        xOrderDetailLine.UnitMeasure = xArticle.UnitMeasure.Acronym;
                        xOrderDetailLine.Price = line.Properties.PriceNet;
                        xOrderDetailLine.Discount = (xArticle.Discount > 0) ? xArticle.Discount : 0.0m;
                        xOrderDetailLine.TotalGross = line.Properties.TotalGross;
                        xOrderDetailLine.TotalDiscount = line.Properties.TotalDiscount;
                        xOrderDetailLine.TotalTax = line.Properties.TotalTax;
                        xOrderDetailLine.TotalFinal = line.Properties.TotalFinal;
                        xOrderDetailLine.Token2 = pToken2;
                        //Use PriceTax Normal|TakeAway
                        xOrderDetailLine.Vat = priceTax;
                        //XPGuidObjects
                        xOrderDetailLine.Article = xArticle;
                        xOrderDetailLine.OrderTicket = xOrderTicket;

                        if (!isInUOW)
                        {
                            xOrderDetailLine.Save();
                        }
                    }





                }


                //}

            };

            //Clean Details and Open a New Blank Ticket in Session
            //Increment Terminal SessionApp CurrentTicketId
            //Only increase in new ticket, else stays the same
            if (!pTicketDrecrease)
            {
                CurrentTicketId += 1;
                currentOrderMain.OrderTickets = new Dictionary<int, OrderTicket>
                {
                    { currentOrderMain.CurrentTicketId, new OrderTicket(this, Table.PriceType) }
                };
            }


            //Finish Writing Session
            POSSession.CurrentSession.Save();

            return xOrderTicket;

            //Debug
            //_logger.Debug(string.Format("FinishOrder(): xOrderMain.Oid [{0}]", xOrderMain.Oid));
            //_logger.Debug(string.Format("FinishOrder(): _table.OrderMainId [{0}], _currentTicketId [{1}], _table.Name [{2}]", _table.OrderMainId, _currentTicketId, _table.Name));
        }

        /// <summary>
        /// Get Total of All Persistent Tickets (Without PartialPayments), Used to Update StatusBar, And Update OrderMain main Object
        /// </summary>
        public void UpdateTotals()
        {
            try
            {
                /* METHOD #2 : From Article Bag
                //OrderMain orderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];
                */
                ArticleBag articleBag = ArticleBag.TicketOrderToArticleBag(this);
                articleBag.UpdateTotals();
                //sqlTotalTickets
                string sqlTotalTickets = string.Format(@"
                    SELECT 
                        COUNT(*) AS TotalTickets    
                    FROM 
                        fin_documentorderticket
                    WHERE 
                        OrderMain = '{0}'
                ;"
                 , this.PersistentOid
                 );
                var totalTickets = XPOSettings.Session.ExecuteScalar(sqlTotalTickets);

                //Assign Totals
                GlobalTotalTickets = (totalTickets != null) ? Convert.ToInt32(totalTickets) : 0;
                GlobalTotalGross = articleBag.TotalFinal;
                GlobalTotalDiscount = articleBag.TotalDiscount;
                GlobalTotalTax = articleBag.TotalTax;
                GlobalTotalFinal = articleBag.TotalFinal;
                GlobalTotalQuantity = articleBag.TotalQuantity;
                //Persist Final TotalOpen
                pos_configurationplacetable currentTable = XPOUtility.GetEntityById<pos_configurationplacetable>(Table.Oid);

                if (currentTable != null)
                {
                    //Required Reload, after ProcessFinanceDocument uowSession, else we get cached object, and apply changes to old object, ex we get a OpenedTable vs a ClosedTable by uowSession
                    currentTable.Reload();
                    currentTable.TotalOpen = GlobalTotalFinal;
                    currentTable.Save();
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public int GetOpenTableFieldValue(Guid pTableOid, string pField)
        {
            int iResult;
            string sql = string.Format("SELECT {1} FROM fin_documentordermain WHERE PlaceTable = '{0}' and OrderStatus = {2};", pTableOid, pField, Convert.ToInt16(OrderStatus.Open));
            var oResult = XPOSettings.Session.ExecuteScalar(sql);
            iResult = (oResult != null) ? Convert.ToInt16(oResult) : -1;

            //Debug
            //_logger.Debug(string.Format("GetOpenTableFieldValue(): field:[{0}], result:[{1}], sql:[{2}]", pField, iResult, sql));

            return iResult;
        }

        public Guid GetOpenTableFieldValueGuid(Guid pTableOid, string pField)
        {
            Guid iResult;
            string sql = string.Format("SELECT {1} FROM fin_documentordermain WHERE PlaceTable = '{0}' and OrderStatus = {2};", pTableOid, pField, Convert.ToInt16(OrderStatus.Open));
            var oResult = XPOSettings.Session.ExecuteScalar(sql);
            iResult = (oResult != null) ? new Guid(Convert.ToString(oResult)) : Guid.Empty;

            //Debug
            //_logger.Debug(string.Format("GetOpenTableFieldValueGuid(): field:[{0}], result:[{1}], sql:[{2}]", pField, iResult, sql));

            return iResult;
        }

        /// <summary>
        /// Clean this Order, but Keep it Selected, Used in Payment CreateDocument
        /// </summary>
        public void CleanSessionOrder()
        {
            OrderStatus = OrderStatus.Close;
            CurrentTicketId = 1;
            _persistentOid = Guid.Empty;
            GlobalTotalGross = 0;
            GlobalTotalTax = 0;
            GlobalTotalFinal = 0;
            GlobalTotalTickets = 0;
            OrderTickets = new Dictionary<int, OrderTicket>
            {
                { CurrentTicketId, new OrderTicket(this, Table.PriceType) }
            };
            POSSession.CurrentSession.Save();
        }

        /// <summary>
        /// Add new ticket to order
        /// </summary>
        public void AddNewTicket()
        {
            OrderStatus = OrderStatus.Open;
            CurrentTicketId = CurrentTicketId + 1;
            //_persistentOid = Guid.Empty;
            GlobalTotalGross = 0;
            GlobalTotalTax = 0;
            GlobalTotalFinal = 0;
            GlobalTotalTickets = 0;
            OrderTickets.Add(CurrentTicketId, new OrderTicket(this, Table.PriceType));
            POSSession.CurrentSession.Save();
        }

        /// <summary>
        /// <para>
        /// Method responsible for check all items from Current Order (from temp file).
        /// </para>
        /// For Parking Ticket purposes, it is not allowed to have 2 units of same article, 
        /// therefore, after adding a new article in Current Order and finishing this order, 
        /// all items from the same will be persisted in data base, merging temp order and database order.
        /// <para>
        /// It is necessary to check for duplicates, removing it from database order.
        /// </para>
        /// </summary>
        /// <remarks>Please see TK013134 for further details</remarks>
        /// <param name="session"></param>
        public void CheckForDuplicatedArticleInArticleBag(Session session)
        {
            _logger.Debug("OrderMain.CheckForDuplicatedArticleInArticleBag(Session session)");

            Session _sessionXpo = session;
            fin_documentordermain xOrderMain = XPOUtility.GetEntityById<fin_documentordermain>(_persistentOid, _sessionXpo);

            //Get current Working Order from SessionApp
            OrderMain currentOrderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
            OrderTicket currentOrderTicket = currentOrderMain.OrderTickets[currentOrderMain.CurrentTicketId];

            OrderDetailLine[] orderDetailsLines = currentOrderTicket.OrderDetails.Lines.ToArray();

            if (xOrderMain != null)
            {
                /* iterates over current Order ticket list */
                foreach (OrderDetailLine line in orderDetailsLines)
                {
                    string currentDesignation = line.Designation;

                    /* iterates over main Order list */
                    foreach (var xOrderMainTicket in xOrderMain.OrderTicket)
                    {
                        xOrderMainTicket.OrderDetail.Load();

                        /* iterates over main Order ticket list */
                        foreach (var xOrderMainTicketOrderDetail in xOrderMainTicket.OrderDetail)
                        {
                            {
                                fin_documentorderdetail aa = xOrderMainTicket.OrderDetail[0];

                                /* When order already has the parking ticket, remove and break */
                                if (xOrderMainTicketOrderDetail.Designation.Equals(currentDesignation))
                                {
                                    currentOrderTicket.OrderDetails.Lines.Remove(line);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}