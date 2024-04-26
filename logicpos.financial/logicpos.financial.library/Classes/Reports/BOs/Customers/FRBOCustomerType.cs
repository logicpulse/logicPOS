using System.Collections.Generic;

namespace logicpos.financial.library.Classes.Reports.BOs.Customers
{
    [FRBO(Entity = "erp_customertype")]
    public class FRBOCustomerType : FRBOBaseObject
    {
        public uint Ord { get; set; }
        public uint Code { get; set; }
        public string Designation { get; set; }
        // Related Objects
        public List<FRBOCustomer> Customer { get; set; }
    }
}
