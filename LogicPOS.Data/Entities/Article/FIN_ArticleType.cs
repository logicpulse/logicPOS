using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_articletype : XPGuidObject
    {
        public fin_articletype() : base() { }
        public fin_articletype(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(fin_articletype), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(fin_articletype), "Code");
            HavePrice = true;
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private bool fHavePrice;
        public bool HavePrice
        {
            get { return fHavePrice; }
            set { SetPropertyValue<bool>("HavePrice", ref fHavePrice, value); }
        }

        //ArticleType One <> Many Article
        [Association(@"ArticleTypeReferencesArticle", typeof(fin_article))]
        public XPCollection<fin_article> Article
        {
            get { return GetCollection<fin_article>("Article"); }
        }
    }
}
