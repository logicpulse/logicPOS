using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_articleclass : Entity
    {
        public fin_articleclass() : base() { }
        public fin_articleclass(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOUtility.GetNextTableFieldID(nameof(fin_articleclass), "Ord");
            Code = XPOUtility.GetNextTableFieldID(nameof(fin_articleclass), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue("Code", ref fCode, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fAcronym;
        [Indexed(Unique = true)]
        [Size(1)]
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        private bool fWorkInStock;
        public bool WorkInStock
        {
            get { return fWorkInStock; }
            set { SetPropertyValue<bool>("WorkInStock", ref fWorkInStock, value); }
        }

        //ArticleType One <> Many Article
        [Association(@"ArticleClassReferencesArticle", typeof(fin_article))]
        public XPCollection<fin_article> Article
        {
            get { return GetCollection<fin_article>("Article"); }
        }
    }
}
