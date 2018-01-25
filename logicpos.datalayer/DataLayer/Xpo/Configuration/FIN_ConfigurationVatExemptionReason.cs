using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_ConfigurationVatExemptionReason : XPGuidObject
    {
        public FIN_ConfigurationVatExemptionReason() : base() { }
        public FIN_ConfigurationVatExemptionReason(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("FIN_ConfigurationVatExemptionReason", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("FIN_ConfigurationVatExemptionReason", "Code");
        }

        //This Can be Optional
        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        //This Can be Optional
        UInt32 fCode;
        [Indexed(Unique = true)]
        public UInt32 Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        string fDesignation;
        [Size(255)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        string fAcronym;
        [Size(3)]
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        string fStandardApplicable;
        [Size(512)]
        public string StandardApplicable
        {
            get { return fStandardApplicable; }
            set { SetPropertyValue<string>("StandardApplicable", ref fStandardApplicable, value); }
        }

        //ConfigurationVatExemptionReason One <> Many DocumentOrderDetail
        [Association(@"ConfigurationVatExemptionReasonReferencesDocumentDocumentFinanceDetail", typeof(FIN_DocumentFinanceDetail))]
        public XPCollection<FIN_DocumentFinanceDetail> DocumentFinanceDetail
        {
            get { return GetCollection<FIN_DocumentFinanceDetail>("DocumentFinanceDetail"); }
        }

        //ConfigurationVatExemptionReason One <> Many Article
        [Association(@"ConfigurationVatExemptionReasonReferencesArticle", typeof(FIN_Article))]
        public XPCollection<FIN_Article> Article
        {
            get { return GetCollection<FIN_Article>("Article"); }
        }
    }
}