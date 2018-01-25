using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_ConfigurationPaymentCondition : XPGuidObject
    {
        public FIN_ConfigurationPaymentCondition() : base() { }
        public FIN_ConfigurationPaymentCondition(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("FIN_ConfigurationPaymentCondition", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("FIN_ConfigurationPaymentCondition", "Code");
        }
        
        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        UInt32 fCode;
        [Indexed(Unique = true)]
        public UInt32 Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        string fAcronym;
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        //ConfigurationPaymentCondition One <> Many DocumentFinanceMaster
        [Association(@"ConfigurationPaymentConditionReferencesDocumentFinanceMaster", typeof(FIN_DocumentFinanceMaster))]
        public XPCollection<FIN_DocumentFinanceMaster> DocumentMaster
        {
            get { return GetCollection<FIN_DocumentFinanceMaster>("DocumentMaster"); }
        }
    }
}