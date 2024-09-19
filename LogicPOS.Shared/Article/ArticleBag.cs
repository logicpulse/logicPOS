using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.shared.Enums;
using LogicPOS.Data.XPO;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Shared.Orders;
using System;
using System.Collections.Generic;

namespace LogicPOS.Shared.Article
{
    public class ArticleBag : Dictionary<ArticleBagKey, ArticleBagProperties>
    {
        public decimal DiscountGlobal { get; set; } = 0.0m;
        public decimal TotalQuantity { get; set; } = 0.0m;
        public decimal TotalNet { get; set; } = 0;
        public decimal TotalGross { get; set; } = 0;
        public decimal TotalDiscount { get; set; } = 0;
        public decimal TotalTax { get; set; } = 0;
        public decimal TotalFinal { get; set; } = 0;
        public Dictionary<decimal, TaxBagProperties> TaxBag { get; set; } = new Dictionary<decimal, TaxBagProperties>();
        public ArticleBag()
            : base(new ArticleBagKeyEqualityComparer())
        {
            DiscountGlobal = POSSession.GetGlobalDiscount();
        }

        public ArticleBag(decimal globalDiscount)
            : base(new ArticleBagKeyEqualityComparer())
        {
            DiscountGlobal = globalDiscount;
        }

        public void UpdateTotals()
        {
            TotalNet = 0.0m;
            TotalGross = 0.0m;
            TotalTax = 0.0m;
            TotalDiscount = 0.0m;
            TotalFinal = 0.0m;
            TotalQuantity = 0.0m;
            TaxBag = new Dictionary<decimal, TaxBagProperties>();

            foreach (var item in this)
            {
                UpdateKeyProperties(item.Key);
                TotalNet += item.Value.TotalNet;
                TotalGross += item.Value.TotalGross;
                TotalTax += item.Value.TotalTax;
                TotalDiscount += item.Value.TotalDiscount;
                TotalFinal += item.Value.TotalFinal;
                TotalQuantity += item.Value.Quantity;

                if (!TaxBag.ContainsKey(item.Key.Vat))
                {
                    TaxBag.Add(item.Key.Vat, new TaxBagProperties(item.Key.Designation, item.Value.TotalTax, item.Value.TotalNet));
                }
                else
                {
                    TaxBag[item.Key.Vat].Total += item.Value.TotalTax;
                    TaxBag[item.Key.Vat].TotalBase += item.Value.TotalNet;
                }
            }
        }

        public void UpdateKeyProperties(ArticleBagKey pKey)
        {

            PriceProperties priceProperties = PriceProperties.GetPriceProperties(
              PricePropertiesSourceMode.FromPriceNet,
              false,
              pKey.Price,
              this[pKey].Quantity,
              pKey.Discount,
              this.DiscountGlobal,
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
        }

        new public void Add(ArticleBagKey key, ArticleBagProperties properties)
        {
            PriceProperties addPriceProperties = PriceProperties.GetPriceProperties(
              PricePropertiesSourceMode.FromPriceNet,
              false,
              key.Price,
              properties.Quantity,
              key.Discount,
              this.DiscountGlobal,
              key.Vat
            );

            if (ContainsKey(key) == false)
            {
                base.Add(key, properties);
            }
            else
            {
                this[key].Quantity += properties.Quantity;
                if (!string.IsNullOrEmpty(this[key].SerialNumber) && !string.IsNullOrEmpty(properties.SerialNumber))
                {
                    this[key].SerialNumber += ";" + properties.SerialNumber;
                    this[key].Notes += "; " + properties.SerialNumber;
                    properties.SerialNumber += ";" + this[key].SerialNumber;
                }
                if (!string.IsNullOrEmpty(this[key].Warehouse) && !string.IsNullOrEmpty(properties.Warehouse))
                {
                    this[key].Warehouse += ";" + properties.Warehouse;
                    properties.Warehouse = this[key].Warehouse;
                }

            }

            //Refresh Current Key Price Properties after Add Quantity)
            UpdateKeyProperties(key);

            //TaxBag Add Key
            if (!TaxBag.ContainsKey(key.Vat))
            {
                //Get Designation from Key
                //Get VatRate formated for filter, in sql server gives error without this it filters 23,0000 and not 23.0000 resulting in null vatRate
                string sql = $"SELECT Designation FROM fin_configurationvatrate WHERE VALUE = '{Utility.DataConversionUtils.DecimalToString(key.Vat)}'";
                string designation = XPOSettings.Session.ExecuteScalar(sql).ToString();
                //Now Add New Key with Designation
                TaxBag.Add(key.Vat, new TaxBagProperties(designation, addPriceProperties.TotalTax, addPriceProperties.TotalNet));
            }
            //Update Key, Add Vat
            else
            {
                TaxBag[key.Vat].Total += addPriceProperties.TotalTax;
                TaxBag[key.Vat].TotalBase += addPriceProperties.TotalNet;
            }

            TotalQuantity += addPriceProperties.Quantity;
            TotalNet += addPriceProperties.TotalNet;
            TotalGross += addPriceProperties.TotalGross;
            TotalTax += addPriceProperties.TotalTax;
            TotalDiscount += addPriceProperties.TotalDiscount;
            TotalFinal += addPriceProperties.TotalFinal;
        }

        public void Remove(ArticleBagKey pKey, decimal pRemoveQuantity)
        {
            //Get PriceProperties Helper Object to Remove from current Key
            PriceProperties removePriceProperties = PriceProperties.GetPriceProperties(
              PricePropertiesSourceMode.FromPriceNet,
              false,
              pKey.Price,
              pRemoveQuantity,
              pKey.Discount,
              this.DiscountGlobal,
              pKey.Vat
            );

            //Decrease Quantity
            this[pKey].Quantity -= pRemoveQuantity;

            // SplitPayment : Sometimes we get 0.000000000000001, that makes key dont be removed because its not < 0
            // To prevent this we must round value before compare using DecimalFormatStockQuantity
            string roundedFormat = $"{{0:{CultureSettings.DecimalFormatStockQuantity}}}";//{0:0.00000000}
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
            TotalQuantity -= removePriceProperties.Quantity;
            TotalNet -= removePriceProperties.TotalNet;
            TotalGross -= removePriceProperties.TotalGross;
            TotalTax -= removePriceProperties.TotalTax;
            TotalDiscount -= removePriceProperties.TotalDiscount;
            TotalFinal -= removePriceProperties.TotalFinal;

            //TaxBag Update 
            TaxBag[pKey.Vat].Total -= removePriceProperties.TotalTax;
            TaxBag[pKey.Vat].TotalBase -= removePriceProperties.TotalNet;
        }

        public void Add(fin_article pArticle, Guid pPlaceOid, Guid pTableOid, PriceType pPriceType, decimal pQuantity)
        {
            Add(pArticle, pPlaceOid, pTableOid, pPriceType, pQuantity, null);
        }

        public void Add(fin_article pArticle, Guid pPlaceOid, Guid pTableOid, PriceType pPriceType, decimal pQuantity, fin_configurationvatexemptionreason pVatExemptionReason)
        {
            ArticleBagKey articleBagKey;
            ArticleBagProperties articleBagProps;

            //Get Place Object to extract TaxSellType Normal|TakeWay
            pos_configurationplace configurationPlace = (pos_configurationplace)XPOSettings.Session.GetObjectByKey(typeof(pos_configurationplace), pPlaceOid);
            TaxSellType taxSellType = (configurationPlace.MovementType.VatDirectSelling) ? TaxSellType.TakeAway : TaxSellType.Normal;

            PriceProperties priceProperties = ArticleUtils.GetArticlePrice(pArticle, taxSellType);

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

        public Dictionary<string, decimal> GetClassTotals()
        {
            bool debug = false;
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();

            fin_article article;

            foreach (var item in this)
            {
                article = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), item.Key.ArticleId);
                if (!result.ContainsKey(article.Class.Acronym))
                {
                    result.Add(article.Class.Acronym, item.Value.TotalFinal);
                }
                else
                {
                    result[article.Class.Acronym] += item.Value.TotalFinal;
                }
            }


            return result;
        }

        public decimal GetClassTotals(string pClassAcronym)
        {
            decimal result = 0.0m;

            Dictionary<string, decimal> articleBagClassTotals = GetClassTotals();
            if (articleBagClassTotals.ContainsKey(pClassAcronym))
            {
                result = articleBagClassTotals[pClassAcronym];
            }

            return result;
        }

        public decimal DeleteFromDocumentOrder(ArticleBagKey pKey, decimal pRemoveQuantity)
        {
            bool isDone = false;
            decimal resultRemainQuantity = 0;
            string where = string.Empty;
            fin_documentorderdetail deleteOrderDetail = null;
            string articleDesignation = string.Empty;

            //Start UnitOfWork
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                OrderMain orderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
                fin_documentordermain xDocumentOrderMain = XPOUtility.GetEntityById<fin_documentordermain>(orderMain.PersistentOid, unitOfWork);

                if (xDocumentOrderMain != null && xDocumentOrderMain.OrderTicket != null)
                {
                    foreach (fin_documentorderticket ticket in xDocumentOrderMain.OrderTicket)
                    {
                        foreach (fin_documentorderdetail detail in ticket.OrderDetail)
                        {

                            if (pKey.ArticleId == detail.Article.Oid && pKey.Price == detail.Price && pKey.Discount == detail.Discount && pKey.Vat == detail.Vat)
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
                    }
                }

                XPOUtility.Audit("ORDER_ARTICLE_REMOVED", string.Format(
                        CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "audit_message_order_article_removed"),
                        articleDesignation,
                        1,
                        resultRemainQuantity - 1,
                        XPOSettings.LoggedUser.Name
                    )
                );

                if (isDone)
                {
                    xDocumentOrderMain.UpdatedAt = XPOUtility.CurrentDateTimeAtomic();

                    resultRemainQuantity -= pRemoveQuantity;

                    if (deleteOrderDetail != null)
                    {
                        fin_documentorderticket deleteOrderTicket = deleteOrderDetail.OrderTicket;
                        fin_documentordermain deleteOrderMain = deleteOrderTicket.OrderMain;

                        deleteOrderDetail.Delete();

                        if (deleteOrderTicket.OrderDetail.Count <= 0)
                        {
                            deleteOrderTicket.Delete();
                        };

                        if (deleteOrderMain.OrderTicket.Count <= 0)
                        {
                            string sql = string.Format(@"UPDATE fin_documentfinancemaster SET SourceOrderMain = NULL WHERE SourceOrderMain = '{0}';", deleteOrderMain.Oid);
                            unitOfWork.ExecuteScalar(sql);
                            deleteOrderMain.PlaceTable.TableStatus = TableStatus.Free;
                            XPOUtility.Audit("TABLE_OPEN", string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "audit_message_table_open"), deleteOrderMain.PlaceTable.Designation));
                            deleteOrderMain.Delete();
                        };
                    };
                };

                try
                {
                    unitOfWork.CommitChanges();
                    orderMain.UpdatedAt = XPOUtility.CurrentDateTimeAtomic();

                    this[pKey].Quantity = resultRemainQuantity;
                    UpdateKeyProperties(pKey);

                    return resultRemainQuantity - XPOUtility.GetPartialPaymentPayedItems(unitOfWork, xDocumentOrderMain.Oid, pKey.ArticleId);
                }
                catch (Exception)
                {
                    unitOfWork.RollbackTransaction();
                    return -1;
                }
            }
        }

        public static ArticleBag TicketOrderToArticleBag(OrderMain pOrderMain)
        {
            OrderMain orderMain = pOrderMain;
            ArticleBag articleBag = new ArticleBag();
            ArticleBagKey articleBagKey;
            ArticleBagProperties articleBagProps;

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
                SQLSelectResultData selectedDataOrders = XPOUtility.GetSelectedDataFromQuery(sqlOrders);

                if (selectedDataOrders.DataRows.Length > 0)
                {
                    foreach (SelectStatementResultRow row in selectedDataOrders.DataRows)
                    {
                        articleBagKey = new ArticleBagKey(
                            new Guid(row.Values[selectedDataOrders.GetFieldIndexFromName("Article")].ToString()),
                            Convert.ToString(row.Values[selectedDataOrders.GetFieldIndexFromName("Designation")]),
                            Convert.ToDecimal(row.Values[selectedDataOrders.GetFieldIndexFromName("Price")]),
                            Convert.ToDecimal(row.Values[selectedDataOrders.GetFieldIndexFromName("Discount")]),
                            Convert.ToDecimal(row.Values[selectedDataOrders.GetFieldIndexFromName("Vat")])
                        );

                        articleBagProps = new ArticleBagProperties(
                            new Guid(row.Values[selectedDataOrders.GetFieldIndexFromName("ConfigurationPlace")].ToString()),
                            new Guid(row.Values[selectedDataOrders.GetFieldIndexFromName("ConfigurationPlaceTable")].ToString()),
                            (PriceType)Enum.Parse(typeof(PriceType), row.Values[selectedDataOrders.GetFieldIndexFromName("PriceType")].ToString()),
                            Convert.ToString(row.Values[selectedDataOrders.GetFieldIndexFromName("Code")]),
                            Convert.ToDecimal(row.Values[selectedDataOrders.GetFieldIndexFromName("Quantity")]),
                            Convert.ToString(row.Values[selectedDataOrders.GetFieldIndexFromName("UnitMeasure")])
                        );

                        //Detect and Assign VatExemptionReason
                        if (row.Values[selectedDataOrders.GetFieldIndexFromName("VatExemptionReason")] != null
                            && Convert.ToString(row.Values[selectedDataOrders.GetFieldIndexFromName("VatExemptionReason")]) != Guid.Empty.ToString()
                        )
                        {
                            articleBagKey.VatExemptionReasonId = new Guid(Convert.ToString(row.Values[selectedDataOrders.GetFieldIndexFromName("VatExemptionReason")]));
                        }

                        //Tokens
                        articleBagProps.Token1 = Convert.ToString(row.Values[selectedDataOrders.GetFieldIndexFromName("Token1")]);
                        articleBagProps.Token2 = Convert.ToString(row.Values[selectedDataOrders.GetFieldIndexFromName("Token2")]);


                        if (articleBagProps.Quantity > 0)
                        {
                            articleBag.Add(articleBagKey, articleBagProps);
                        }
                    }
                }

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

                SQLSelectResultData selectedDataDocuments = XPOUtility.GetSelectedDataFromQuery(sqlDocuments);
                if (selectedDataDocuments.DataRows.Length > 0)
                {
                    foreach (SelectStatementResultRow row in selectedDataDocuments.DataRows)
                    {
                        if (row.Values[selectedDataDocuments.GetFieldIndexFromName("DocumentType")].ToString() != DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument.ToString()
                            && row.Values[selectedDataDocuments.GetFieldIndexFromName("Price")] != null)
                        {

                            //Generate Key/Props
                            articleBagKey = new ArticleBagKey(
                                new Guid(row.Values[selectedDataDocuments.GetFieldIndexFromName("Article")].ToString()),
                                Convert.ToString(row.Values[selectedDataDocuments.GetFieldIndexFromName("Designation")]),
                                Convert.ToDecimal(row.Values[selectedDataDocuments.GetFieldIndexFromName("Price")]),
                                Convert.ToDecimal(row.Values[selectedDataDocuments.GetFieldIndexFromName("Discount")]),
                                Convert.ToDecimal(row.Values[selectedDataDocuments.GetFieldIndexFromName("Vat")])
                            );
                            //Detect and Assign VatExemptionReason
                            if (row.Values[selectedDataDocuments.GetFieldIndexFromName("VatExemptionReason")] != null
                                && Convert.ToString(row.Values[selectedDataDocuments.GetFieldIndexFromName("VatExemptionReason")]) != Guid.Empty.ToString()
                            )
                            {
                                //Add VatException Reason to Key
                                articleBagKey.VatExemptionReasonId = new Guid(Convert.ToString(row.Values[selectedDataDocuments.GetFieldIndexFromName("VatExemptionReason")]));
                            }
                            if (articleBag.ContainsKey(articleBagKey))
                            {
                                //Remove PartialPayed Item Quantity from ArticleBag
                                articleBag.Remove(articleBagKey, Convert.ToDecimal(row.Values[selectedDataDocuments.GetFieldIndexFromName("Quantity")]));
                            }
                            else
                            {
                                foreach (var article in articleBag)
                                {
                                    if (article.Key.ArticleId == articleBagKey.ArticleId)
                                    {
                                        articleBag.Remove(article.Key, Convert.ToDecimal(row.Values[selectedDataDocuments.GetFieldIndexFromName("Quantity")]));
                                    }
                                }

                            }
                        }
                    }
                }
            }

            return articleBag;
        }

        public static ArticleBag ConvertDocumentToArticleBag(fin_documentfinancemaster pDocumentFinanceMaster)
        {
            ArticleBag articleBag = new ArticleBag();
            ArticleBagKey articleBagKey;
            ArticleBagProperties articleBagProps;

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

            return articleBag;
        }
    }
}
