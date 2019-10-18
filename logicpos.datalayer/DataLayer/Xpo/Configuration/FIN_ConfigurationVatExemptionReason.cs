using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_configurationvatexemptionreason : XPGuidObject
    {
        public fin_configurationvatexemptionreason() : base() { }
        public fin_configurationvatexemptionreason(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(fin_configurationvatexemptionreason), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(fin_configurationvatexemptionreason), "Code");
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
        [Size(60)]
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
        [Association(@"ConfigurationVatExemptionReasonReferencesDocumentDocumentFinanceDetail", typeof(fin_documentfinancedetail))]
        public XPCollection<fin_documentfinancedetail> DocumentFinanceDetail
        {
            get { return GetCollection<fin_documentfinancedetail>("DocumentFinanceDetail"); }
        }

        //ConfigurationVatExemptionReason One <> Many Article
        [Association(@"ConfigurationVatExemptionReasonReferencesArticle", typeof(fin_article))]
        public XPCollection<fin_article> Article
        {
            get { return GetCollection<fin_article>("Article"); }
        }
    }
}