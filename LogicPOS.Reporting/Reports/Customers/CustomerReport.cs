using LogicPOS.Reporting.Common;

namespace LogicPOS.Reporting.Reports.Customers
{
    [Report(Entity = "erp_customer")]
    public class CustomerReport : ReportData
    {
        public uint Ord { get; set; }
        public uint Code { get; set; }
        public string CodeInternal { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Locality { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string FiscalNumber { get; set; }
        public string CardNumber { get; set; }
        public string DiscountType { get; set; }
        public decimal Discount { get; set; }
        public decimal CardCredit { get; set; }
        public decimal TotalDebt { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal CurrentBalance { get; set; }
        public string CreditLine { get; set; }
        public string Remarks { get; set; }
        public bool Supplier { get; set; }
        public bool Hidden { get; set; }
    }
}
