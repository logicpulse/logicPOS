using System;

namespace logicpos.financial.library.Classes.Reports.BOs.Customers
{
    [FRBO(Entity = "erp_customer")]
    public class FRBOCustomer : FRBOBaseObject
    {
        public UInt32 Ord { get; set; }
        public UInt32 Code { get; set; }
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
        public Boolean Supplier { get; set; }
        public Boolean Hidden { get; set; }
    }
}
