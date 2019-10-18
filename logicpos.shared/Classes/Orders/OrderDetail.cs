using logicpos.datalayer.DataLayer.Xpo;
using logicpos.shared.App;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace logicpos.shared.Classes.Orders
{
    public class OrderDetail
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Public Properties
        private List<OrderDetailLine> _lines;
        public List<OrderDetailLine> Lines
        {
            get { return _lines; }
            set { _lines = value; }
        }

        //Total Items (Articles) in OrderDetail
        private decimal _totalItems = 0;
        public decimal TotalItems
        {
            get { return _totalItems; }
            set { _totalItems = value; }
        }

        //TotalFinal
        private decimal _totalFinal = 0;
        public decimal TotalFinal
        {
            get { return _totalFinal; }
            set { _totalFinal = value; }
        }

        //Store Reference to Parent OrderTicket
        private OrderTicket _orderTicket;
        [JsonIgnore]
        public OrderTicket OrderTicket
        {
            get { return _orderTicket; }
            set { _orderTicket = value; }
        }

        //Required Parameterless Constructor for Json.NET (Load)
        public OrderDetail()
        {
            //Init TicketLines
            _lines = new List<OrderDetailLine>();
        }

        public OrderDetail(OrderTicket pOrderTicket)
        {
            //Reference to Parent OrderTicket
            _orderTicket = pOrderTicket;
            //Init TicketLines
            _lines = new List<OrderDetailLine>();
        }

        public void Insert(Guid pArticleId, decimal pQuantity, TaxSellType pTaxSellType)
        {
            fin_article article = (fin_article)FrameworkUtils.GetXPGuidObject(typeof(fin_article), pArticleId);
            PriceProperties priceProperties = FrameworkUtils.GetArticlePrice(article, pTaxSellType);
            priceProperties.Quantity = pQuantity;
            Insert(article.Oid, article.Designation, priceProperties);
        }

        public void Insert(Guid pArticleId, String pDesignation, PriceProperties pPriceProperties)
        {
            OrderDetailLine line = new OrderDetailLine(pArticleId, pDesignation, pPriceProperties);
            _lines.Add(line);
            CalculateTotals();
        }

        public void Update(int pLineIndex, decimal pQuantity)
        {
            //Change Properties
            _lines[pLineIndex].Properties.Quantity = pQuantity;
            _lines[pLineIndex].Properties.Update();
            CalculateTotals();
        }

        public void Update(int pLineIndex, decimal pQuantity, decimal pPrice)
        {
            //Change Properties
            _lines[pLineIndex].Properties.Quantity = pQuantity;
            _lines[pLineIndex].Properties.PriceUser = pPrice;
            _lines[pLineIndex].Properties.Update();
            CalculateTotals();
        }

        public void Delete(int pLineIndex)
        {
            try
            {
                _lines.RemoveAt(pLineIndex);
                CalculateTotals();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Calculate OrderDetail Totals 
        /// </summary>
        private void CalculateTotals()
        {
            //Reset Totals
            _totalFinal = 0;
            _totalItems = 0;

            foreach (OrderDetailLine line in _lines)
            {
                _totalFinal += line.Properties.TotalFinal;
                _totalItems += line.Properties.Quantity;
            }
            GlobalFramework.SessionApp.Write();
        }
    }
}
