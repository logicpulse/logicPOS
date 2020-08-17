using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.resources.Resources.Localization;
using logicpos.shared.App;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace logicpos.shared.Classes.Orders
{
    public class OrderMain
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Public Properties
        private int _currentTicketId;
        public int CurrentTicketId
        {
            get { return _currentTicketId; }
            set { _currentTicketId = value; }
        }

        private Guid _persistentOid;
        public Guid PersistentOid
        {
            get { return _persistentOid; }
            set { _persistentOid = value; }
        }

        private OrderStatus _orderStatus;
        public OrderStatus OrderStatus
        {
            get { return _orderStatus; }
            set { _orderStatus = value; }
        }

        private OrderMainTable _table;
        public OrderMainTable Table
        {
            get { return _table; }
            set { _table = value; }
        }

        private DateTime _dateStart;
        public DateTime DateStart
        {
            get { return _dateStart; }
            set { _dateStart = value; }
        }

        private DateTime _updatedAt;
        public DateTime UpdatedAt
        {
            get { return _updatedAt; }
            set { _updatedAt = value; }
        }

        private Dictionary<int, OrderTicket> _orderTickets;
        public Dictionary<int, OrderTicket> OrderTickets
        {
            get { return _orderTickets; }
            set { _orderTickets = value; }
        }

        //JsonIgnored Members for DB Results
        private decimal _globalTotalGross;
        [JsonIgnore]
        public decimal GlobalTotalGross
        {
            get { return _globalTotalGross; }
            set { _globalTotalGross = value; }
        }

        private decimal _globalTotalDiscount;
        [JsonIgnore]
        public decimal GlobalTotalDiscount
        {
            get { return _globalTotalDiscount; }
            set { _globalTotalDiscount = value; }
        }

        private decimal _globalTotalTax;
        [JsonIgnore]
        public decimal GlobalTotalTax
        {
            get { return _globalTotalTax; }
            set { _globalTotalTax = value; }
        }

        private decimal _globalTotalFinal;
        [JsonIgnore]
        public decimal GlobalTotalFinal
        {
            get { return _globalTotalFinal; }
            set { _globalTotalFinal = value; }
        }

        private decimal _globalTotalQuantity;
        [JsonIgnore]
        public decimal GlobalTotalQuantity
        {
            get { return _globalTotalQuantity; }
            set { _globalTotalQuantity = value; }
        }

        private int _globalTotalTickets;
        [JsonIgnore]
        public int GlobalTotalTickets
        {
            get { return _globalTotalTickets; }
            set { _globalTotalTickets = value; }
        }

        private sys_userdetail _globalLastUser;
        [JsonIgnore]
        public sys_userdetail GlobalLastUser
        {
            get { return _globalLastUser; }
            set { _globalLastUser = value; }
        }

        private pos_configurationplaceterminal _globalLastTerminal;
        [JsonIgnore]
        public pos_configurationplaceterminal GlobalLastTerminal
        {
            get { return _globalLastTerminal; }
            set { _globalLastTerminal = value; }
        }

        //Required Parameterless Constructor for Json.NET (Load)
        public OrderMain() { }
        //Constructor without Json.NET Load, With Defaults
        public OrderMain(Guid pOrderMainOid, Guid pTableOid)
        {
            _currentTicketId = 1;
            _persistentOid = new Guid();
            _orderStatus = OrderStatus.Null;
            _table = new OrderMainTable(pOrderMainOid, pTableOid);
            _dateStart = FrameworkUtils.CurrentDateTimeAtomic();
            _orderTickets = new Dictionary<int, OrderTicket>();
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
            //Result
            fin_documentorderticket xOrderTicket = null;

            //Get current Working Order from SessionApp
            OrderMain currentOrderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];
            OrderTicket currentOrderTicket = currentOrderMain.OrderTickets[currentOrderMain.CurrentTicketId];

            //Get Place Object to extract TaxSellType Normal|TakeWay
            pos_configurationplace configurationPlace = (pos_configurationplace)GlobalFramework.SessionXpo.GetObjectByKey(typeof(pos_configurationplace), currentOrderMain.Table.PlaceId);
            //Use VatDirectSelling if in Retail or in TakeWay mode
            TaxSellType taxSellType = (configurationPlace.MovementType.VatDirectSelling || SettingsApp.AppMode == AppOperationMode.Retail) ? TaxSellType.TakeAway : TaxSellType.Normal;

            //Open Table on First Finish OrderTicket
            pos_configurationplacetable xTable = (pos_configurationplacetable)FrameworkUtils.GetXPGuidObject(_sessionXpo, typeof(pos_configurationplacetable), _table.Oid);
            xTable.Reload();
            if (xTable.TableStatus != TableStatus.Open)
            {
                xTable.TableStatus = TableStatus.Open;
                FrameworkUtils.Audit("TABLE_OPEN", string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "audit_message_table_open"), xTable.Designation));
                xTable.DateTableOpen = FrameworkUtils.CurrentDateTimeAtomic();
                if (!isInUOW) xTable.Save();
            }

            //Get Current _persistentOid and _from Database
            _persistentOid = GetOpenTableFieldValueGuid(_table.Oid, "Oid");
            _orderStatus = (OrderStatus)GetOpenTableFieldValue(_table.Oid, "OrderStatus");
            _updatedAt = FrameworkUtils.CurrentDateTimeAtomic();
            Guid orderTicketOid = Guid.Empty;
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
                    UpdatedAt = FrameworkUtils.CurrentDateTimeAtomic()
                };
                if (!isInUOW) xOrderMain.Save();
                //After Save, Get Oid
                _persistentOid = xOrderMain.Oid;
                //Change to Open Status
                _orderStatus = OrderStatus.Open;
            }
            //Update
            else
            {
                xOrderMain = (fin_documentordermain)FrameworkUtils.GetXPGuidObject(_sessionXpo, typeof(fin_documentordermain), _persistentOid);
                if (xOrderMain.PlaceTable != xTable) xOrderMain.PlaceTable = xTable;
                //Force Changes in Record, else UpdatedAt dont Update
                xOrderMain.UpdatedAt = FrameworkUtils.CurrentDateTimeAtomic();
                //TODO: Check if User was Automatically Updated
                //if (xOrderMain.UpdatedBy != GlobalFramework.LoggedUser) xOrderMain.UpdatedBy = GlobalFramework.LoggedUser;
                if (!isInUOW) xOrderMain.Save();
            }

            //Create OrderTicket
            //if (pTicketDrecrease)
            //{
            //var sql = string.Format(@"SELECT * FROM fin_documentorderticket WHERE TicketId = '{0}' AND OrderMain = '{1}';", currentOrderMain.CurrentTicketId, currentOrderMain.PersistentOid);
            //_log.Debug(string.Format("sql: [{0}]", sql));
            string sql = string.Format(@"SELECT Oid FROM fin_documentorderticket WHERE OrderMain = '{0}' AND TicketId = '{1}';", currentOrderMain.PersistentOid, currentOrderMain.CurrentTicketId);
            //_log.Debug(string.Format("sql: [{0}]", sql));
            orderTicketOid = FrameworkUtils.GetGuidFromQuery(sql);
            xOrderTicket = (fin_documentorderticket)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentorderticket), orderTicketOid);

            //xOrderTicket = (fin_documentorderticket)FrameworkUtils.GetXPGuidObject(_sessionXpo, typeof(fin_documentorderticket), currentOrderMain._persistentOid);
            if (xOrderTicket != null)
            {
                xOrderTicket.TicketId = currentOrderMain.CurrentTicketId;
                xOrderTicket.DateStart = currentOrderTicket.DateStart;
                xOrderTicket.PriceType = currentOrderTicket.PriceType;
                xOrderTicket.Discount = xTable.Discount;
                xOrderTicket.OrderMain = xOrderMain;
                xOrderTicket.PlaceTable = xTable;
                xOrderTicket.UpdatedAt = FrameworkUtils.CurrentDateTimeAtomic();
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
            decimal priceTax = 0;

            foreach (OrderDetailLine line in currentOrderTicket.OrderDetails.Lines)
            {
                //Use Order in print tickets etc
                itemOrd++;
                xArticle = (fin_article)FrameworkUtils.GetXPGuidObject(_sessionXpo, typeof(fin_article), line.ArticleOid);
                //Get PriceTax from TaxSellType
                priceTax = (taxSellType == TaxSellType.Normal) ? xArticle.VatOnTable.Value : xArticle.VatDirectSelling.Value;
				//Edit/cancel orders lindote 10/07/2020
                //Get order ticket Oid from DB
                string sql3 = string.Format(@"SELECT Oid FROM fin_documentorderticket WHERE OrderMain = '{0}' AND TicketId = '{1}';", currentOrderMain.PersistentOid, currentOrderMain.CurrentTicketId);                
                orderTicketOid = FrameworkUtils.GetGuidFromQuery(sql3);
                
                //Get order detail Oid from DB
                string sql4 = string.Format(@"SELECT Oid FROM fin_documentorderdetail WHERE OrderTicket = '{0}' AND Article = '{1}' AND Price = '{2}'  AND TotalDiscount = '{3}'  AND Vat = '{4}';", 
                             orderTicketOid, line.ArticleOid, line.Properties.PriceNet.ToString().Replace(",", "."),
                                                              line.Properties.TotalDiscount.ToString().Replace(",", "."),
                                                              line.Properties.Vat.ToString().Replace(",", "."));                
                Guid orderDetailOid = FrameworkUtils.GetGuidFromQuery(sql4);

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
                    xOrderDetailLine = (fin_documentorderdetail)FrameworkUtils.GetXPGuidObject(_sessionXpo, typeof(fin_documentorderdetail), orderDetailOid);

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
                _currentTicketId += 1;
                currentOrderMain.OrderTickets = new Dictionary<int, OrderTicket>();
                currentOrderMain.OrderTickets.Add(currentOrderMain.CurrentTicketId, new OrderTicket(this, _table.PriceType));
            }


            //Finish Writing Session
            GlobalFramework.SessionApp.Write();

            return xOrderTicket;

            //Debug
            //_log.Debug(string.Format("FinishOrder(): xOrderMain.Oid [{0}]", xOrderMain.Oid));
            //_log.Debug(string.Format("FinishOrder(): _table.OrderMainId [{0}], _currentTicketId [{1}], _table.Name [{2}]", _table.OrderMainId, _currentTicketId, _table.Name));
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
                var totalTickets = GlobalFramework.SessionXpo.ExecuteScalar(sqlTotalTickets);

                //Assign Totals
                _globalTotalTickets = (totalTickets != null) ? Convert.ToInt32(totalTickets) : 0;
                _globalTotalGross = articleBag.TotalFinal;
                _globalTotalDiscount = articleBag.TotalDiscount;
                _globalTotalTax = articleBag.TotalTax;
                _globalTotalFinal = articleBag.TotalFinal;
                _globalTotalQuantity = articleBag.TotalQuantity;
                //Persist Final TotalOpen
                pos_configurationplacetable currentTable = (pos_configurationplacetable)FrameworkUtils.GetXPGuidObject(typeof(pos_configurationplacetable), _table.Oid);

                if(currentTable != null)
                {
                    //Required Reload, after ProcessFinanceDocument uowSession, else we get cached object, and apply changes to old object, ex we get a OpenedTable vs a ClosedTable by uowSession
                    currentTable.Reload();
                    currentTable.TotalOpen = _globalTotalFinal;
                    currentTable.Save();
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            /* METHOD 3 : When we Change table it dont Update GlobalDiscounts
            //Get Total Order (Payed/Invoiced and NonPayed)
            string sqlTotalViewOrders = string.Format(@"
              SELECT 
                SUM(ddTotalGross) AS TotalGross, 
                SUM(ddTotalDiscount) AS TotalDiscount, 
                SUM(ddTotalTax) AS TotalTax,
                SUM(ddTotalFinal) AS TotalFinal, 
                SUM(ddQuantity) AS TotalQuantity
              FROM 
                view_orders 
              WHERE 
                dmOid = '{0}'
              ORDER BY 
                dtTicketId,ddOrd;
              ;"
              , this.PersistentOid
            );

            string sqlTotalViewDocumentFinance = string.Format(@"
              SELECT 
                  SUM(fdTotalGross) AS TotalGross, 
                SUM(fdTotalDiscount) AS TotalDiscount, 
                SUM(fdTotalTax) AS TotalTax,
                SUM(fdTotalFinal) AS TotalFinal, 
                SUM(fdQuantity) AS TotalQuantity
              FROM 
                  view_documentfinance 
              WHERE 
                  fmSourceOrderMain = '{0}'
              ;"
              , this.PersistentOid
            );

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

            try
            {
              XPSelectData sdTotalViewOrders = FrameworkUtils.GetSelectedDataFromQuery(sqlTotalViewOrders);
              XPSelectData sdTotalViewDocumentFinance = FrameworkUtils.GetSelectedDataFromQuery(sqlTotalViewDocumentFinance);

              if (sdTotalViewOrders.Data.Length > 0 && sdTotalViewDocumentFinance.Data.Length > 0)
              {
                //TotalGross
                _globalTotalGross =
                  Convert.ToDecimal(sdTotalViewOrders.Data[0].Values[sdTotalViewOrders.GetFieldIndex("TotalGross")]) -
                  Convert.ToDecimal(sdTotalViewDocumentFinance.Data[0].Values[sdTotalViewOrders.GetFieldIndex("TotalGross")]);
                //TotalDiscount
                _globalTotalDiscount =
                  Convert.ToDecimal(sdTotalViewOrders.Data[0].Values[sdTotalViewOrders.GetFieldIndex("TotalDiscount")]) -
                  Convert.ToDecimal(sdTotalViewDocumentFinance.Data[0].Values[sdTotalViewOrders.GetFieldIndex("TotalDiscount")]);
                //TotalTax
                _globalTotalTax =
                  Convert.ToDecimal(sdTotalViewOrders.Data[0].Values[sdTotalViewOrders.GetFieldIndex("TotalTax")]) -
                  Convert.ToDecimal(sdTotalViewDocumentFinance.Data[0].Values[sdTotalViewOrders.GetFieldIndex("TotalTax")]);
                //TotalFinal
                _globalTotalFinal =
                  Convert.ToDecimal(sdTotalViewOrders.Data[0].Values[sdTotalViewOrders.GetFieldIndex("TotalFinal")]) -
                  Convert.ToDecimal(sdTotalViewDocumentFinance.Data[0].Values[sdTotalViewOrders.GetFieldIndex("TotalFinal")]);
                //TotalQuantity
                _globalTotalQuantity =
                  Convert.ToDecimal(sdTotalViewOrders.Data[0].Values[sdTotalViewOrders.GetFieldIndex("TotalQuantity")]) -
                  Convert.ToDecimal(sdTotalViewDocumentFinance.Data[0].Values[sdTotalViewOrders.GetFieldIndex("TotalQuantity")]);
              }

              //sqlTotalTickets
              var totalTickets = GlobalFramework.SessionXpo.ExecuteScalar(sqlTotalTickets);
              _globalTotalTickets = (totalTickets != null) ? Convert.ToInt32(totalTickets) : 0;

              //Persist Final TotalOpen 
              ConfigurationPlaceTable currentTable = (ConfigurationPlaceTable)FrameworkUtils.GetXPGuidObjectFromSession(typeof(ConfigurationPlaceTable), _table.Oid);
              currentTable.TotalOpen = _globalTotalFinal;
              currentTable.Save();
            }
            catch (Exception ex)
            {
              _log.Error(ex.Message, ex);
            }
            */

            /* OLD DEPRECATED METHOD #1 : Bugged In Orders with PartialPayments
            bool debug = false;

            //Settings

            //Always Reset Totals, With Persistent and Non Persistent Orders
            _globalNumOfTickets = 0;
            _globalTotalGross = 0;
            _globalTotalDiscount = 0;
            _globalTotalTax = 0;
            _globalTotalFinal = 0;

            //Get Current _persistentOid and _orderStatus from Database
            _persistentOid = GetOpenTableFieldValueGuid(_table.Oid, "Oid");
            _orderStatus = (OrderStatus)GetOpenTableFieldValue(_table.Oid, "OrderStatus");

            if (_persistentOid != Guid.Empty)
            {
              CriteriaOperator binaryOperator = new BinaryOperator("OrderMain", _persistentOid, BinaryOperatorType.Equal);
              XPCollection _xpcDocumentOrderTicket = new XPCollection(GlobalFramework.SessionXpo, typeof(DocumentOrderTicket), binaryOperator);

              //Required to ByPass Cache
              _xpcDocumentOrderTicket.Reload();

              //Process DocumentOrderTickets Totals
              if (_xpcDocumentOrderTicket.Count > 0)
              {
                foreach (DocumentOrderTicket ticket in _xpcDocumentOrderTicket)
                {
                  //Required to ByPass Cache
                  ticket.OrderDetail.Reload();

                  //Increase Ticket
                  _globalNumOfTickets++;

                  foreach (DocumentOrderDetail line in ticket.OrderDetail)
                  {
                    _globalTotalGross += line.TotalGross;
                    _globalTotalDiscount += line.TotalDiscount;
                    _globalTotalTax += line.TotalTax;
                    _globalTotalFinal += line.TotalFinal;
                    if (debug) _log.Debug(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", line.Article.Oid, line.Designation, line.Price, line.Quantity, line.Discount, line.Vat));
                  }
                  _globalLastUser = ticket.UpdatedBy;
                  _globalLastTerminal = ticket.UpdatedWhere;
                }
              }

              //Process PartialPayed Items and Discount its Totals
              CriteriaOperator binaryOperatorDocumentFinanceMaster = new BinaryOperator("SourceOrderMain", _persistentOid, BinaryOperatorType.Equal);
              XPCollection _xpcDocumentFinanceMaster = new XPCollection(GlobalFramework.SessionXpo, typeof(DocumentFinanceMaster), binaryOperatorDocumentFinanceMaster);
              if (_xpcDocumentFinanceMaster.Count > 0)
              {
                foreach (DocumentFinanceMaster master in _xpcDocumentFinanceMaster)
                {
                  //SEARCH#001 - Change here and in Other Search, to SYNC RESULTS
                  //Only Discount items from ArticleBag if is NOT a TableConsult, in TableConsult keep Full ArticleBag From OrderMain
                  if (master.DocumentType.Oid != new Guid(xpoOidDocumentFinanceTypeConferenceDocument))
                  {
                    foreach (DocumentFinanceDetail line in master.DocumentDetail)
                    {
                      _globalTotalGross -= line.TotalGross;
                      _globalTotalDiscount -= line.TotalDiscount;
                      _globalTotalTax -= line.TotalTax;
                      _globalTotalFinal -= line.TotalFinal;
                    }
                  }
                }
              }

              //Persist Final TotalOpen 
              ConfigurationPlaceTable currentTable = (ConfigurationPlaceTable)FrameworkUtils.GetXPGuidObjectFromSession(typeof(ConfigurationPlaceTable), _table.Oid);
              currentTable.TotalOpen = _globalTotalFinal;
              currentTable.Save();

              //Debug
              //_log.Debug(string.Format("GetGlobalOrderSummary(): _table.Id:[{0}], _table.Name:[{1}]", _table.Id, _table.Name));
              //_log.Debug(string.Format("GetGlobalOrderSummary(): _globalTotalGross [{0}]", _globalTotalGross));
              //_log.Debug(string.Format("GetGlobalOrderSummary(): _globalTotalTax [{0}]", _globalTotalTax));
              //_log.Debug(string.Format("GetGlobalOrderSummary(): _globalTotalDiscount [{0}]", _globalTotalDiscount));
              //_log.Debug(string.Format("GetGlobalOrderSummary(): _globalTotalFinal [{0}]", _globalTotalFinal));
              //_log.Debug(string.Format("GetGlobalOrderSummary(): _globalNumOfTickets [{0}]", _globalNumOfTickets));
              //if (_globalLastUser != null) _log.Debug(string.Format("GetGlobalOrderSummary(): _globalLastUser.Name [{0}]", _globalLastUser.Name));
              //if (_globalLastTerminal != null) _log.Debug(string.Format("GetGlobalOrderSummary(): _globalLastTerminal.Designation [{0}]", _globalLastTerminal.Designation));
            }
            */
        }

        public int GetOpenTableFieldValue(Guid pTableOid, string pField)
        {
            int iResult;
            string sql = string.Format("SELECT {1} FROM fin_documentordermain WHERE PlaceTable = '{0}' and OrderStatus = {2};", pTableOid, pField, Convert.ToInt16(OrderStatus.Open));
            var oResult = GlobalFramework.SessionXpo.ExecuteScalar(sql);
            iResult = (oResult != null) ? Convert.ToInt16(oResult) : -1;

            //Debug
            //_log.Debug(string.Format("GetOpenTableFieldValue(): field:[{0}], result:[{1}], sql:[{2}]", pField, iResult, sql));

            return iResult;
        }

        public Guid GetOpenTableFieldValueGuid(Guid pTableOid, string pField)
        {
            Guid iResult;
            string sql = string.Format("SELECT {1} FROM fin_documentordermain WHERE PlaceTable = '{0}' and OrderStatus = {2};", pTableOid, pField, Convert.ToInt16(OrderStatus.Open));
            var oResult = GlobalFramework.SessionXpo.ExecuteScalar(sql);
            iResult = (oResult != null) ? new Guid(Convert.ToString(oResult)) : Guid.Empty;

            //Debug
            //_log.Debug(string.Format("GetOpenTableFieldValueGuid(): field:[{0}], result:[{1}], sql:[{2}]", pField, iResult, sql));

            return iResult;
        }

        /// <summary>
        /// Clean this Order, but Keep it Selected, Used in Payment CreateDocument
        /// </summary>
        public void CleanSessionOrder()
        {
            _orderStatus = OrderStatus.Close;
            _currentTicketId = 1;
            _persistentOid = Guid.Empty;
            _globalTotalGross = 0;
            _globalTotalTax = 0;
            _globalTotalFinal = 0;
            _globalTotalTickets = 0;
            _orderTickets = new Dictionary<int, OrderTicket>();
            _orderTickets.Add(_currentTicketId, new OrderTicket(this, _table.PriceType));
            GlobalFramework.SessionApp.Write();
        }

        /// <summary>
        /// Add new ticket to order
        /// </summary>
        public void AddNewTicket()
        {
            _orderStatus = OrderStatus.Open;
            _currentTicketId = _currentTicketId + 1;
            //_persistentOid = Guid.Empty;
            _globalTotalGross = 0;
            _globalTotalTax = 0;
            _globalTotalFinal = 0;
            _globalTotalTickets = 0;
            _orderTickets.Add(_currentTicketId, new OrderTicket(this, _table.PriceType));
            GlobalFramework.SessionApp.Write();
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
            _log.Debug("OrderMain.CheckForDuplicatedArticleInArticleBag(Session session)");

            Session _sessionXpo = session;
            fin_documentordermain xOrderMain = (fin_documentordermain)FrameworkUtils.GetXPGuidObject(_sessionXpo, typeof(fin_documentordermain), _persistentOid);

            //Get current Working Order from SessionApp
            OrderMain currentOrderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];
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