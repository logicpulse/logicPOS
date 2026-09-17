using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Reports.SalesByDate.GetMonthlySalesReportData
{
    public class MonthlySalesReportData
    {
        public int Year { get; set; }
        public List<int> Years { get; set; }
        public List<MonthlySale> Sales { get; set; } 
    }

    public class MonthlySale
    {
        public int Month { get; set; }
        public decimal NetTotal { get; set; }
        public decimal FinalTotal { get; set; }
    }
}
