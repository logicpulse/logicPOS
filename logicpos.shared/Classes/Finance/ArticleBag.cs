using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.resources.Resources.Localization;
using logicpos.shared.App;
using logicpos.shared.Classes.Orders;
using logicpos.shared.Enums;
using System;
using System.Collections.Generic;

namespace logicpos.shared.Classes.Finance
{
    public class ArticleBag : Dictionary<ArticleBagKey, ArticleBagProperties>
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        decimal _discountGlobal = 0.0m;
        public decimal DiscountGlobal
        {
            get { return _discountGlobal; }
            set { _discountGlobal = value; }
        }

        decimal _totalQuantity = 0.0m;
        public decimal TotalQuantity
        {
            get { return _totalQuantity; }
            set { _totalQuantity = value; }
        }

        decimal _totalNet = 0;
        public decimal TotalNet
        {
            get { return _totalNet; }
            set { _totalNet = value; }
        }

        decimal _totalGross = 0;
        public decimal TotalGross
        {
            get { return _totalGross; }
            set { _totalGross = value; }
        }

        decimal _totalDiscount = 0;
        public decimal TotalDiscount
        {
            get { return _totalDiscount; }
            set { _totalDiscount = value; }
        }

        decimal _totalTax = 0;
        public decimal TotalTax
        {
            get { return _totalTax; }
            set { _totalTax = value; }
        }

        //Total Document with Taxs (TotalFinal)
        decimal _totalFinal = 0;
        public decimal TotalFinal
        {
            get { return _totalFinal; }
            set { _totalFinal = value; }
        }

        //TaxBag
        Dictionary<decimal, TaxBagProperties> _taxBag = new Dictionary<decimal, TaxBagProperties>();
        public Dictionary<decimal, TaxBagProperties> TaxBag
        {
            get { return _taxBag; }
            set { _taxBag = value; }
        }

        //New Override Dictionary EqualityComparer
        public ArticleBag()
            : base(new ArticleBagKey.EqualityComparer())
        {
            //Get Default Global Discount
            _discountGlobal = FrameworkUtils.GetDiscountGlobal();
        }

        public ArticleBag(decimal pDiscountGlobal)
            : base(new ArticleBagKey.EqualityComparer())
        {
            //Get Discount from Parameter
            _discountGlobal = pDiscountGlobal;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        /// <summary>
        /// Used to Update Global ArticleBag Totals and Properties
        /// </summary>
        public void UpdateTotals()
        {
            _totalNet = 0.0m;
            _totalGross = 0.0m;
            _totalTax = 0.0m;
            _totalDiscount = 0.0m;
            _totalFinal = 0.0m;
            _totalQuantity = 0.0m;

            //Require to ReCreate TaxBag else in Payment window when we change Discount, 
            //TaxBag Reflect totals without discount appyed, ex after ArticleBag costructed with one Discount and change it after construct
            _taxBag = new Dictionary<decimal, TaxBagProperties>();

            foreach (var item in this)
            {
                UpdateKeyProperties(item.Key);
                _totalNet += item.Value.TotalNet;
                _totalGross += item.Value.TotalGross;
                _totalTax += item.Value.TotalTax;
                _totalDiscount += item.Value.TotalDiscount;
                _totalFinal += item.Value.TotalFinal;
                _totalQuantity += item.Value.Quantity;

                //Required to Update TaxBag Totals
                //TaxBag Add Key
                if (!_taxBag.ContainsKey(item.Key.Vat))
                {
                    _taxBag.Add(item.Key.Vat, new TaxBagProperties(item.Key.Designation, item.Value.TotalTax, item.Value.TotalNet));
                }
                //Update Key, Add Vat
                else
                {
                    _taxBag[item.Key.Vat].Total += item.Value.TotalTax;
                    _taxBag[item.Key.Vat].TotalBase += item.Value.TotalNet;
                }
            }
        }

        /// <summary>
        /// Used to Update Article Bag Price Properties, Prices, Totals etc, All Other Fields that not are Sent via Key and Props
        /// </summary>
        /// <param name="pKey"></param>
        /// <param name="pProps"></param>
        /// <returns></returns>
        public void UpdateKeyProperties(ArticleBagKey pKey)
        {
            bool debug = false;

            //Get Fresh PriceProperties Helper Object to Calc
            PriceProperties priceProperties = PriceProperties.GetPriceProperties(
              PricePropertiesSourceMode.FromPriceNet,
              false,
              pKey.Price,
              this[pKey].Quantity,
              pKey.Discount,
              this._discountGlobal,
              pKey.Vat
            );

            this[pKey].PriceWithDiscount = priceProperties.PriceWithDiscount;
            this[pKey].PriceWithDiscountGlobal = priceProperties.PriceWithDiscountGlobal;
            this[pKey].TotalGross = priceProperties.TotalGross;
            this[pKey].TotalNet = priceProperties.TotalNet;
            this[pKey].TotalDiscount = priceProperties.TotalDiscount;
            this[pKey].TotalTax = priceProperties.TotalTax;
            this[pKey].TotalFinal = priceProperties.TotalFinal;
            this[pKey].PriceFinal = priceProperties.PriceFinal;

            if (debug)
            {
                priceProperties.SendToLog("");
                _log.Debug(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}",
                  this[pKey].Code, pKey.Designation, this[pKey].Quantity, this[pKey].TotalGross, this[pKey].PriceWithDiscount, this[pKey].PriceWithDiscountGlobal, this[pKey].TotalGross, this[pKey].TotalNet, this[pKey].TotalDiscount, this[pKey].TotalTax, this[pKey].TotalFinal));
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Add From ArticleBag Key/Props

        //NEW: Override/Replace Dictionary Add with() Custom Add()
        new public void Add(ArticleBagKey pKey, ArticleBagProperties pProps)
        {
            //Init local vars
            ArticleBagKey key = pKey;
            ArticleBagProperties props = pProps;

            //Get Fresh PriceProperties Helper Object to used for Addition (Vat and Totals)
            PriceProperties addPriceProperties = PriceProperties.GetPriceProperties(
              PricePropertiesSourceMode.FromPriceNet,
              false,
              pKey.Price,
              pProps.Quantity,
              pKey.Discount,
              this._discountGlobal,
              pKey.Vat
            );

            //If Key doesnt exists Add it
            if (!this.ContainsKey(key))
            {
                base.Add(key, props);
            }
            //Else Update Key, Increase Quantity
            else
            {
                this[key].Quantity += props.Quantity;
            }

            //Refresh Current Key Price Properties after Add Quantity)
            UpdateKeyProperties(key);

            //TaxBag Add Key
            if (!_taxBag.ContainsKey(key.Vat))
            {
                //Get Designation from Key
                //Get VatRate formated for filter, in sql server gives error without this it filters 23,0000 and not 23.0000 resulting in null vatRate
                string sql = string.Format("SELECT Designation FROM fin_configurationvatrate WHERE VALUE = '{0}'", FrameworkUtils.DecimalToString(key.Vat, GlobalFramework.CurrentCultureNumberFormat));
                string designation = GlobalFramework.SessionXpo.ExecuteScalar(sql).ToString();
                //Now Add New Key with Designation
                _taxBag.Add(key.Vat, new TaxBagProperties(designation, addPriceProperties.TotalTax, addPriceProperties.TotalNet));
            }
            //Update Key, Add Vat
            else
            {
                _taxBag[key.Vat].Total += addPriceProperties.TotalTax;
                _taxBag[key.Vat].TotalBase += addPriceProperties.TotalNet;
            }

            _totalQuantity += addPriceProperties.Quantity;
            _totalNet += addPriceProperties.TotalNet;
            _totalGross += addPriceProperties.TotalGross;
            _totalTax += addPriceProperties.TotalTax;
            _totalDiscount += addPriceProperties.TotalDiscount;
            _totalFinal += addPriceProperties.TotalFinal;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //Used to Remove PartialPayments from ArticleBag
        public void Remove(ArticleBagKey pKey, decimal pRemoveQuantity)
        {
            //Get PriceProperties Helper Object to Remove from current Key
            PriceProperties removePriceProperties = PriceProperties.GetPriceProperties(
              PricePropertiesSourceMode.FromPriceNet,
              false,
              pKey.Price,
              pRemoveQuantity,
              pKey.Discount,
              this._discountGlobal,
              pKey.Vat
            );

            //Decrease Quantity
            this[pKey].Quantity -= pRemoveQuantity;

            // SplitPayment : Sometimes we get 0.000000000000001, that makes key dont be removed because its not < 0
            // To prevent this we must round value before compare using DecimalFormatStockQuantity
            string roundedFormat = $"{{0:{SettingsApp.DecimalFormatStockQuantity}}}";//{0:0.00000000}
            decimal roundedQuantity = Convert.ToDecimal(string.Format(roundedFormat, this[pKey].Quantity));

            //if (this[pKey].Quantity <= 0)
            if (roundedQuantity <= 0)
            {
                this.Remove(pKey);
            }
            else
            {
                //Refresh Current Key Price Properties after Add Quantity)
                UpdateKeyProperties(pKey);
            }

            //Calc Article Grand Totals
            _totalQuantity -= removePriceProperties.Quantity;
            _totalNet -= removePriceProperties.TotalNet;
            _totalGross -= removePriceProperties.TotalGross;
            _totalTax -= removePriceProperties.TotalTax;
            _totalDiscount -= removePriceProperties.TotalDiscount;
            _totalFinal -= removePriceProperties.TotalFinal;

            //TaxBag Update 
            _taxBag[pKey.Vat].Total -= removePriceProperties.TotalTax;
            _taxBag[pKey.Vat].TotalBase -= removePriceProperties.TotalNet;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //Add From Article
        public void Add(fin_article pArticle, Guid pPlaceOid, Guid pTableOid, PriceType pPriceType, decimal pQuantity)
        {
            Add(pArticle, pPlaceOid, pTableOid, pPriceType, pQuantity, null);
        }

        public void Add(fin_article pArticle, Guid pPlaceOid, Guid pTableOid, PriceType pPriceType, decimal pQuantity, fin_configurationvatexemptionreason pVatExemptionReason)
        {
            ArticleBagKey articleBagKey;
            ArticleBagProperties articleBagProps;
            
            //Get Place Object to extract TaxSellType Normal|TakeWay
            pos_configurationplace configurationPlace = (pos_configurationplace)GlobalFramework.SessionXpo.GetObjectByKey(typeof(pos_configurationplace), pPlaceOid);
            TaxSellType taxSellType = (configurationPlace.MovementType.VatDirectSelling) ? TaxSellType.TakeAway : TaxSellType.Normal;

            PriceProperties priceProperties = FrameworkUtils.GetArticlePrice(pArticle, taxSellType);

            //Prepare articleBag Key and Props
            articleBagKey = new ArticleBagKey(
              pArticle.Oid,
              pArticle.Designation,
              priceProperties.PriceNet,
              priceProperties.DiscountArticle,
              priceProperties.Vat,
              pVatExemptionReason.Oid
            );
            articleBagProps = new ArticleBagProperties(
              pPlaceOid,
              pTableOid,
              pPriceType,
              pArticle.Code,
              pQuantity,
              pArticle.UnitMeasure.Acronym
            );

            //Send to Bag
            Add(articleBagKey, articleBagProps);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Get ArticleBag Articles Splitted by ArticleClass ex Totals Class P,S,O,I

        public Dictionary<string, decimal> GetClassTotals()
        {
            bool debug = false;
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();

            fin_article article;

            try
            {
                foreach (var item in this)
                {
                    article = (fin_article)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_article), item.Key.ArticleOid);
                    if (!result.ContainsKey(article.Class.Acronym))
                    {
                        result.Add(article.Class.Acronym, item.Value.TotalFinal);
                    }
                    else
                    {
                        result[article.Class.Acronym] += item.Value.TotalFinal;
                    }
                    if (debug) _log.Debug(String.Format("Acronym: [{0}], TotalFinal : [{1}], ClassTotalFinal: [{2}]", article.Class.Acronym, item.Value.TotalFinal, result[article.Class.Acronym]));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        public decimal GetClassTotals(string pClassAcronym)
        {
            decimal result = 0.0m;

            try
            {
                Dictionary<string, decimal> articleBagClassTotals = GetClassTotals();
                if (articleBagClassTotals.ContainsKey(pClassAcronym))
                {
                    result = articleBagClassTotals[pClassAcronym];
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Used to Remove Articles from DocumentOrder ex when we Delete Article From TicketList.OrderMain Details

        public decimal DeleteFromDocumentOrder(ArticleBagKey pKey, decimal pRemoveQuantity)
        {
            bool isDone = false;
            decimal resultRemainQuantity = 0;
            string where = string.Empty;
            //Store Reference to Future delete Object (After foreach Loop)
            fin_documentordermain deleteOrderMain = null;
            fin_documentorderticket deleteOrderTicket = null;
            fin_documentorderdetail deleteOrderDetail = null;
            string articleDesignation = string.Empty;

            //Start UnitOfWork
            using (UnitOfWork uowSession = new UnitOfWork())
            {
                OrderMain orderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];
                fin_documentordermain xDocumentOrderMain = (fin_documentordermain)FrameworkUtils.GetXPGuidObject(uowSession, typeof(fin_documentordermain), orderMain.PersistentOid);

                if (xDocumentOrderMain != null && xDocumentOrderMain.OrderTicket != null)
                {
                    foreach (fin_documentorderticket ticket in xDocumentOrderMain.OrderTicket)
                    {
                        foreach (fin_documentorderdetail detail in ticket.OrderDetail)
                        {
                            try
                            {
                                //Check Equal Key
                                if (pKey.ArticleOid == detail.Article.Oid && pKey.Price == detail.Price && pKey.Discount == detail.Discount && pKey.Vat == detail.Vat)
                                {
                                    articleDesignation = pKey.Designation;
                                    resultRemainQuantity += detail.Quantity;
                                    if (!isDone)
                                    {
                                        detail.Quantity -= pRemoveQuantity;
                                        //Assign references to Future Deletes
                                        if (detail.Quantity <= 0) { deleteOrderDetail = detail; }
                                        isDone = true;
                                    }
                                    else
                                    {
                                        where += string.Format(" OR Oid = '{0}'", detail.Oid);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _log.Error(ex.Message, ex);
                            }
                        }
                    }
                }

                //Debug
                //string sql = @"SELECT * FROM fin_documentorderdetail WHERE 1=0{0};";
                //_log.Debug(string.Format("Delete(): sql [{0}]", string.Format(sql, where)));

                //Audit
                FrameworkUtils.Audit("ORDER_ARTICLE_REMOVED", string.Format(
                        resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "audit_message_order_article_removed"),
                        articleDesignation,
                        1,
                        resultRemainQuantity - 1,
                        GlobalFramework.LoggedUser.Name
                    )
                );

                if (isDone)
                {
                    //Update xDocumentOrderMain UpdatedAt, Required for RealTime Update
                    xDocumentOrderMain.UpdatedAt = FrameworkUtils.CurrentDateTimeAtomic();

                    //Remove Quantity
                    resultRemainQuantity -= pRemoveQuantity;

                    //Delete Records, OrderMain, OrderTicket and OrderDetails
                    if (deleteOrderDetail != null)
                    {
                        deleteOrderTicket = deleteOrderDetail.OrderTicket;
                        deleteOrderMain = deleteOrderTicket.OrderMain;

                        //Delete Details
                        deleteOrderDetail.Delete();

                        //Check if OrderTicket in Empty, If so Delete it, its not required anymore
                        if (deleteOrderTicket.OrderDetail.Count <= 0)
                        {
                            deleteOrderTicket.Delete();
                        };

                        //Check if OrderMain in Empty, If so Delete it, its not required anymore
                        if (deleteOrderMain.OrderTicket.Count <= 0)
                        {
                            //Before Delete OrderMain, we must UnAssign DocumentMaster SourceOrderMain else we have a CONSTRAINT ERROR on FK_DocumentFinanceMaster_SourceOrderMain trying to delete used OrderMain
                            string sql = string.Format(@"UPDATE fin_documentfinancemaster SET SourceOrderMain = NULL WHERE SourceOrderMain = '{0}';", deleteOrderMain.Oid);
                            uowSession.ExecuteScalar(sql);
                            //Open Table
                            deleteOrderMain.PlaceTable.TableStatus = TableStatus.Free;
                            //Audit
                            FrameworkUtils.Audit("TABLE_OPEN", string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "audit_message_table_open"), deleteOrderMain.PlaceTable.Designation));
                            //Delete OrderMain
                            deleteOrderMain.Delete();
                        };
                    };
                };

                try
                {
                    //Commit UOW Changes
                    uowSession.CommitChanges();
                    //Update OrderMain UpdatedAt, Required to Sync Terminals
                    orderMain.UpdatedAt = FrameworkUtils.CurrentDateTimeAtomic();

                    //Update ArticleBag Price Properties
                    this[pKey].Quantity = resultRemainQuantity;
                    UpdateKeyProperties(pKey);

                    //SEARCH#001
                    //Require to Remove PartialPayed Items Quantity
                    return resultRemainQuantity - FrameworkUtils.GetPartialPaymentPayedItems(uowSession, xDocumentOrderMain.Oid, pKey.ArticleOid);
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    uowSession.RollbackTransaction();
                    return -1;
                }
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public void ShowInLog()
        {
            _log.Debug("\tCode\tDesignation\tQuantity\tPriceUser\tDiscount\tVat\tPriceNet\tPriceWithDiscount\tPriceWithDiscountGlobal\tTotalNet\tTotalGross\tTotalDiscount\tTotalTax\tTotalFinal\tPriceFinal");
            foreach (var item in this)
            {
                _log.Debug(string.Format("\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}",
                    item.Value.Code,
                    item.Key.Designation,
                    item.Value.Quantity,
                    String.Empty,//PriceUser no Used
                    item.Key.Discount,
                    item.Key.Vat,
                    item.Key.Price,
                    item.Value.PriceWithDiscount,
                    item.Value.PriceWithDiscountGlobal,
                    item.Value.TotalNet,
                    item.Value.TotalGross,
                    item.Value.TotalDiscount,
                    item.Value.TotalTax,
                    item.Value.TotalFinal,
                    item.Value.PriceFinal
                  ));
            }
            //TaxBag
            _log.Debug("\tVat\tTotal");
            foreach (var item in this._taxBag)
            {
                _log.Debug(string.Format("\t{0}\t{1}", item.Key, item.Value));
            }
            //Totals
            _log.Debug("\tTotalItems\tTotalNet\tTotalGross\tTotalDiscount\tTotalTax\tTotalFinal");
            _log.Debug(string.Format("\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}", _totalQuantity, _totalNet, _totalGross, _totalDiscount, _totalTax, _totalFinal));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Methods

        //Create ArticleBag From OrderMain.OrderTicket, and Discount PartialPayments for Working OrderMain
        public static ArticleBag TicketOrderToArticleBag(OrderMain pOrderMain)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            bool debug = false;

            //OrderMain
            OrderMain orderMain = pOrderMain;
            //ArticleBag
            ArticleBag articleBag = new ArticleBag();
            ArticleBagKey articleBagKey;
            ArticleBagProperties articleBagProps;

            //Removed, gives problems, Avoid used DropIdentityMap
            //GlobalFramework.SessionXpo.DropIdentityMap();

            try
            {
                if (orderMain.PersistentOid != Guid.Empty)
                {
                    string sqlOrders = string.Format(@"
                        SELECT 
                            dmOid AS DocumentOrderMain, ddArticle AS Article, ddDesignation AS Designation,ddPrice AS Price,ddDiscount AS Discount,ddVat AS Vat,ddVatExemptionReason AS VatExemptionReason,
                            cpOid AS ConfigurationPlace, ctOid AS ConfigurationPlaceTable, dtPriceType AS PriceType, ddCode AS Code, ddQuantity AS Quantity, ddUnitMeasure AS UnitMeasure,
                            ddToken1 as Token1, ddToken2 as Token2
                        FROM 
                            view_orders 
                        WHERE 
                            dmOid = '{0}'
                        ORDER BY 
                            dtTicketId
                        ;"
                        , orderMain.PersistentOid
                    );
                    XPSelectData selectedDataOrders = FrameworkUtils.GetSelectedDataFromQuery(sqlOrders);

                    //Process Tickets and Add to ArticleBag
                    if (selectedDataOrders.Data.Length > 0)
                    {
                        foreach (SelectStatementResultRow row in selectedDataOrders.Data)
                        {
                            //Generate Key/Props
                            articleBagKey = new ArticleBagKey(
                                new Guid(row.Values[selectedDataOrders.GetFieldIndex("Article")].ToString()),                                   //ticketLine.Article.Oid
                                Convert.ToString(row.Values[selectedDataOrders.GetFieldIndex("Designation")]),                                  //ticketLine.Designation
                                Convert.ToDecimal(row.Values[selectedDataOrders.GetFieldIndex("Price")]),                                       //ticketLine.Price
                                Convert.ToDecimal(row.Values[selectedDataOrders.GetFieldIndex("Discount")]),                                    //ticketLine.Discount
                                Convert.ToDecimal(row.Values[selectedDataOrders.GetFieldIndex("Vat")])                                          //ticketLine.Vat
                            );

                            articleBagProps = new ArticleBagProperties(
                                new Guid(row.Values[selectedDataOrders.GetFieldIndex("ConfigurationPlace")].ToString()),                        //ticket.PlaceTable.Place.Oid
                                new Guid(row.Values[selectedDataOrders.GetFieldIndex("ConfigurationPlaceTable")].ToString()),                   //ticket.PlaceTable.Oid
                                (PriceType)Enum.Parse(typeof(PriceType), row.Values[selectedDataOrders.GetFieldIndex("PriceType")].ToString()), //ticket.PriceType
                                Convert.ToString(row.Values[selectedDataOrders.GetFieldIndex("Code")]),                                         //ticketLine.Code
                                Convert.ToDecimal(row.Values[selectedDataOrders.GetFieldIndex("Quantity")]),                                    //ticketLine.Quantity
                                Convert.ToString(row.Values[selectedDataOrders.GetFieldIndex("UnitMeasure")])                                   //ticketLine.UnitMeasure
                            );

                            //Detect and Assign VatExemptionReason
                            if (row.Values[selectedDataOrders.GetFieldIndex("VatExemptionReason")] != null 
                                && Convert.ToString(row.Values[selectedDataOrders.GetFieldIndex("VatExemptionReason")]) != Guid.Empty.ToString()
                            )
                            {
                                //Add VatException Reason to Key
                                articleBagKey.VatExemptionReasonOid = new Guid(Convert.ToString(row.Values[selectedDataOrders.GetFieldIndex("VatExemptionReason")]));
                            }

                            //Tokens
                            articleBagProps.Token1 = Convert.ToString(row.Values[selectedDataOrders.GetFieldIndex("Token1")]); //ticketLine.Token1
                            articleBagProps.Token2 = Convert.ToString(row.Values[selectedDataOrders.GetFieldIndex("Token2")]); //ticketLine.Token2

                            //Send to Bag
                            if(articleBagProps.Quantity > 0)
                            {
                                articleBag.Add(articleBagKey, articleBagProps);
                            }
                            
                            //if (debug) log.Debug(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}", ticket.PlaceTable.Place.Oid, ticket.PlaceTable.Designation, ticket.PriceType, ticketLine.Article.Oid, ticketLine.Code, ticketLine.Designation, ticketLine.Price, ticketLine.Quantity, ticketLine.UnitMeasure, ticketLine.Discount, ticketLine.Vat));
                        }
                    }

                    //Process PartialPayed Items and Remove From ArticleBag
                    string sqlDocuments = string.Format(@"
                        SELECT 
                            ftOid AS DocumentType,fdArticle AS Article,fdDesignation AS Designation,fdPrice AS Price,fdQuantity AS Quantity, fdDiscount AS Discount, fdVat AS Vat, fdVatExemptionReason AS VatExemptionReason
                        FROM 
                            view_documentfinance 
                        WHERE 
                            fmSourceOrderMain = '{0}'
                        ORDER BY 
                            ftOid,fmOid
                        ;"
                        , orderMain.PersistentOid
                    );

                    XPSelectData selectedDataDocuments = FrameworkUtils.GetSelectedDataFromQuery(sqlDocuments);
                    if (selectedDataDocuments.Data.Length > 0)
                    {
                        foreach (SelectStatementResultRow row in selectedDataDocuments.Data)
                        {
                            // If Not ConferenceDocument or TableConsult
                            if (row.Values[selectedDataDocuments.GetFieldIndex("DocumentType")].ToString() != SettingsApp.XpoOidDocumentFinanceTypeConferenceDocument.ToString())
                            {
                                //Generate Key/Props
                                articleBagKey = new ArticleBagKey(
                                    new Guid(row.Values[selectedDataDocuments.GetFieldIndex("Article")].ToString()),
                                    Convert.ToString(row.Values[selectedDataDocuments.GetFieldIndex("Designation")]),
                                    Convert.ToDecimal(row.Values[selectedDataDocuments.GetFieldIndex("Price")]),
                                    Convert.ToDecimal(row.Values[selectedDataDocuments.GetFieldIndex("Discount")]),
                                    Convert.ToDecimal(row.Values[selectedDataDocuments.GetFieldIndex("Vat")])
                                );
                                //Detect and Assign VatExemptionReason
                                if (row.Values[selectedDataDocuments.GetFieldIndex("VatExemptionReason")] != null 
                                    && Convert.ToString(row.Values[selectedDataDocuments.GetFieldIndex("VatExemptionReason")]) != Guid.Empty.ToString()
                                )
                                {
                                    //Add VatException Reason to Key
                                    articleBagKey.VatExemptionReasonOid = new Guid(Convert.ToString(row.Values[selectedDataDocuments.GetFieldIndex("VatExemptionReason")]));
                                }
                                if (articleBag.ContainsKey(articleBagKey))
                                {
                                    //Remove PartialPayed Item Quantity from ArticleBag
                                    articleBag.Remove(articleBagKey, Convert.ToDecimal(row.Values[selectedDataDocuments.GetFieldIndex("Quantity")]));
                                }
                                else
                                {
                                    if (debug) log.Debug(string.Format("articleBagKey: [{0}]", articleBagKey));
                                }
                            }
                        }
                    }
                    if (debug) articleBag.ShowInLog();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return articleBag;
        }

        //Create ArticleBag From DocumentFinanceMaster
        public static ArticleBag DocumentFinanceMasterToArticleBag(fin_documentfinancemaster pDocumentFinanceMaster)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //Init Global ArticleBag
            ArticleBag articleBag = new ArticleBag();
            ArticleBagKey articleBagKey;
            ArticleBagProperties articleBagProps;

            try
            {
                if (pDocumentFinanceMaster != null
                    && pDocumentFinanceMaster.DocumentDetail != null
                    && pDocumentFinanceMaster.DocumentDetail.Count > 0
                )
                {
                    foreach (fin_documentfinancedetail detail in pDocumentFinanceMaster.DocumentDetail)
                    {
                        //Prepare articleBag Key and Props
                        articleBagKey = new ArticleBagKey(
                          detail.Article.Oid,
                          detail.Designation,
                          detail.Price,
                          detail.Discount,
                          detail.Vat
                        );
                        articleBagProps = new ArticleBagProperties(
                          detail.DocumentMaster.SourceOrderMain.PlaceTable.Place.Oid,
                          detail.DocumentMaster.SourceOrderMain.PlaceTable.Oid,
                          (PriceType)detail.DocumentMaster.SourceOrderMain.PlaceTable.Place.PriceType.EnumValue,
                          detail.Code,
                          detail.Quantity,
                          detail.UnitMeasure
                        );
                        //Send to Bag
                        articleBag.Add(articleBagKey, articleBagProps);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return articleBag;
        }
    }
}
