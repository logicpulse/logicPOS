﻿using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Enums;
using Newtonsoft.Json;
using System;

namespace LogicPOS.Shared.Orders
{
    public class OrderTicket
    {
        public PriceType PriceType { get; set; }

        public DateTime DateStart { get; set; }

        public OrderDetail OrderDetails { get; set; }

        [JsonIgnore]
        public OrderMain OrderMain { get; set; }

        //Required Parameterless Constructor for Json.NET (Load)
        public OrderTicket() { }
        public OrderTicket(OrderMain pOrderMain, PriceType pPriceType)
        {
            //Reference to Parent OrderMain
            OrderMain = pOrderMain;
            PriceType = pPriceType;
            DateStart = XPOUtility.CurrentDateTimeAtomic();
            OrderDetails = new OrderDetail(this);
        }
    }
}
