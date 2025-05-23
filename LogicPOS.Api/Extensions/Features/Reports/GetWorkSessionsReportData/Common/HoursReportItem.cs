using System;

namespace LogicPOS.Api.Features.Reports.WorkSession.Common
{
    public class HoursReportItem
    {
        public DateTime Date{ get; set; }
        public decimal Quantity { get; set; } 
        public decimal Total { get; set; }
    }
}
