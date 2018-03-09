using System;
using System.Collections.Generic;

namespace logicpos.financial.library.Classes.Reports.BOs.Customers
{
    [FRBO(Entity = "erp_customertype")]
    public class FRBOCustomerType : FRBOBaseObject
    {
        public UInt32 Ord { get; set; }
        public UInt32 Code { get; set; }
        public string Designation { get; set; }
        // Related Objects
        public List<FRBOCustomer> Customer { get; set; }
    }
}
