using logicpos.datalayer.Enums;
using logicpos.shared.App;
using Newtonsoft.Json;
using System;

namespace logicpos.shared.Classes.Orders
{
    public class OrderTicket
    {
        //Public Properties
        private PriceType _priceType;
        public PriceType PriceType
        {
            get { return _priceType; }
            set { _priceType = value; }
        }

        private DateTime _dateStart;
        public DateTime DateStart
        {
            get { return _dateStart; }
            set { _dateStart = value; }
        }

        private OrderDetail _orderDetails;
        public OrderDetail OrderDetails
        {
            get { return _orderDetails; }
            set { _orderDetails = value; }
        }

        //Store Reference to Parent OrderMain
        private OrderMain _orderMain;
        [JsonIgnore]
        public OrderMain OrderMain
        {
            get { return _orderMain; }
            set { _orderMain = value; }
        }

        //Required Parameterless Constructor for Json.NET (Load)
        public OrderTicket() { }
        public OrderTicket(OrderMain pOrderMain, PriceType pPriceType)
        {
            //Reference to Parent OrderMain
            _orderMain = pOrderMain;
            _priceType = pPriceType;
            _dateStart = FrameworkUtils.CurrentDateTimeAtomic();
            _orderDetails = new OrderDetail(this);
        }
    }
}
