using DevExpress.Xpo;
using logicpos.datalayer.App;
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
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(fin_articletype), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(fin_articletype), "Code");
            HavePrice = true;
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

        Boolean fHavePrice;
        public Boolean HavePrice
        {
            get { return fHavePrice; }
            set { SetPropertyValue<Boolean>("HavePrice", ref fHavePrice, value); }
        }

        //ArticleType One <> Many Article
        [Association(@"ArticleTypeReferencesArticle", typeof(fin_article))]
        public XPCollection<fin_article> Article
        {
            get { return GetCollection<fin_article>("Article"); }
        }
    }
}
