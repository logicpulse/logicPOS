using System;

namespace logicpos.financial.library.Classes.Reports.BOs
{
    //Notes
    //Try to use same Types has XpoObjects
    //Tip1: Class Name : [FRBO][DatabaseEntityName] ex.: [FRBO][Article] (if is a XPGuidObject not a VIEW)
    //Tip2: Only Public Properties are Included in DataSources
    //Tip3: Can Override Oid;

    //Now Entity is Required to be defined, since implementation of Table Prefix, else entity name is getted from object name without FRBO and returns invalid DB Object ex (FRBO)DocumentFinanceDetail
    [FRBO(Entity = "pfx_template")]
    class FRBOTemplate : FRBOBaseObject
    {
        public int Code { get; set; }
        public string Designation { get; set; }
    }
}
