using System;

namespace LogicPOS.Api.Features.Reports.SalesByDate.GetSalesTotalForDay
{
    public class TotalSalesForDay
    {
        public DateTime Day { get; set; }
        public decimal DayTotal { get; set; }
        public decimal MonthTotal { get; set; }
        public decimal YearTotal { get; set; }
    }
}

