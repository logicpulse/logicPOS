using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using logicpos.shared.Enums;
using LogicPOS.Shared.Article;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LogicPOS.Shared.Orders
{
    public class OrderDetail
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<OrderDetailLine> Lines { get; set; }

        public decimal TotalItems { get; set; } = 0;

        public decimal TotalFinal { get; set; } = 0;

        [JsonIgnore]
        public OrderTicket OrderTicket { get; set; }

        //Required Parameterless Constructor for Json.NET (Load)
        public OrderDetail()
        {
            //Init TicketLines
            Lines = new List<OrderDetailLine>();
        }

        public OrderDetail(OrderTicket pOrderTicket)
        {
            //Reference to Parent OrderTicket
            OrderTicket = pOrderTicket;
            //Init TicketLines
            Lines = new List<OrderDetailLine>();
        }

        public void Insert(Guid pArticleId, decimal pQuantity, TaxSellType pTaxSellType)
        {
            fin_article article = (fin_article)XPOHelper.GetXPGuidObject(typeof(fin_article), pArticleId);
            PriceProperties priceProperties = ArticleUtils.GetArticlePrice(article, pTaxSellType);
            priceProperties.Quantity = pQuantity;
            Insert(article.Oid, article.Designation, priceProperties);
        }

        public void Insert(Guid pArticleId, string pDesignation, PriceProperties pPriceProperties)
        {
            OrderDetailLine line = new OrderDetailLine(pArticleId, pDesignation, pPriceProperties);
            Lines.Add(line);
            CalculateTotals();
        }

        public void Update(int pLineIndex, decimal pQuantity)
        {
            //Change Properties
            Lines[pLineIndex].Properties.Quantity = pQuantity;
            Lines[pLineIndex].Properties.Update();
            CalculateTotals();
        }

        public void Update(int pLineIndex, decimal pQuantity, decimal pPrice)
        {
            //Change Properties
            Lines[pLineIndex].Properties.Quantity = pQuantity;
            Lines[pLineIndex].Properties.PriceUser = pPrice;
            Lines[pLineIndex].Properties.Update();
            CalculateTotals();
        }

        public void Delete(int pLineIndex)
        {
            try
            {
                Lines.RemoveAt(pLineIndex);
                CalculateTotals();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Calculate OrderDetail Totals 
        /// </summary>
        private void CalculateTotals()
        {
            //Reset Totals
            TotalFinal = 0;
            TotalItems = 0;

            foreach (OrderDetailLine line in Lines)
            {
                TotalFinal += line.Properties.TotalFinal;
                TotalItems += line.Properties.Quantity;
            }
            POSSession.CurrentSession.Save();
        }
    }
}
