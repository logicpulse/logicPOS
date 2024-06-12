using LogicPOS.Reporting.Data.Common;
using System.Collections.Generic;

namespace LogicPOS.Reporting.Reports.Customers
{
    [ReportData(Entity = "erp_customertype")]
    public class CustomerTypeReportData : ReportData
    {
        public uint Ord { get; set; }
        public uint Code { get; set; }
        public string Designation { get; set; }
        // Related Objects
        public List<CustomerReportData> Customer { get; set; }
    }
}
