using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_ArticleType : XPGuidObject
    {
        public FIN_ArticleType() : base() { }
        public FIN_ArticleType(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("FIN_ArticleType", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("FIN_ArticleType", "Code");
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
        [Association(@"ArticleTypeReferencesArticle", typeof(FIN_Article))]
        public XPCollection<FIN_Article> Article
        {
            get { return GetCollection<FIN_Article>("Article"); }
        }
    }
}
