using DevExpress.Xpo;
using logicpos.datalayer.Xpo;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class cfg_configurationunitsize : XPGuidObject
    {
        public cfg_configurationunitsize() : base() { }
        public cfg_configurationunitsize(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(cfg_configurationunitsize), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(cfg_configurationunitsize), "Code");
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

        //ConfigurationUnitSize One <> Many Article
        [Association(@"ConfigurationUnitSizeReferencesArticle", typeof(fin_article))]
        public XPCollection<fin_article> Article
        {
            get { return GetCollection<fin_article>("Article"); }
        }
    }
}