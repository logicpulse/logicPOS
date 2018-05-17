using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class CFG_ConfigurationUnitMeasure : XPGuidObject
    {
        public CFG_ConfigurationUnitMeasure() : base() { }
        public CFG_ConfigurationUnitMeasure(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(CFG_ConfigurationUnitMeasure), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(CFG_ConfigurationUnitMeasure), "Code");
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
        [Indexed(Unique = true)]
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        //configurationUnitMeasure One <> Many Article
        [Association(@"ConfigurationUnitMeasureReferencesArticle", typeof(FIN_Article))]
        public XPCollection<FIN_Article> Article
        {
            get { return GetCollection<FIN_Article>("Article"); }
        }
    }
}