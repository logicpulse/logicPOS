using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class CFG_ConfigurationUnitSize : XPGuidObject
    {
        public CFG_ConfigurationUnitSize() : base() { }
        public CFG_ConfigurationUnitSize(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(CFG_ConfigurationUnitSize), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(CFG_ConfigurationUnitSize), "Code");
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

        //ConfigurationUnitSize One <> Many Article
        [Association(@"ConfigurationUnitSizeReferencesArticle", typeof(FIN_Article))]
        public XPCollection<FIN_Article> Article
        {
            get { return GetCollection<FIN_Article>("Article"); }
        }
    }
}