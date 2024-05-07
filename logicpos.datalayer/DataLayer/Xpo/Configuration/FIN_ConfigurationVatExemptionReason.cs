using DevExpress.Xpo;
using logicpos.datalayer.App;
using logicpos.datalayer.Xpo;
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
            Ord = XPOHelper.GetNextTableFieldID(nameof(fin_configurationvatexemptionreason), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(fin_configurationvatexemptionreason), "Code");
        }

        //This Can be Optional
        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        //This Can be Optional
        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        private string fDesignation;
        [Size(60)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fAcronym;
        [Size(3)]
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        private string fStandardApplicable;
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