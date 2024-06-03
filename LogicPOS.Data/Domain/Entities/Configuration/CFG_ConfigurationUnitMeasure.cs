using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;
using System;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class cfg_configurationunitmeasure : Entity
    {
        public cfg_configurationunitmeasure() : base() { }
        public cfg_configurationunitmeasure(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(cfg_configurationunitmeasure), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(cfg_configurationunitmeasure), "Code");
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
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        //configurationUnitMeasure One <> Many Article
        [Association(@"ConfigurationUnitMeasureReferencesArticle", typeof(fin_article))]
        public XPCollection<fin_article> Article
        {
            get { return GetCollection<fin_article>("Article"); }
        }
    }
}