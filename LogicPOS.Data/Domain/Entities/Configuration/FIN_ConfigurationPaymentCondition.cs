using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_configurationpaymentcondition : Entity
    {
        public fin_configurationpaymentcondition() : base() { }
        public fin_configurationpaymentcondition(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOUtility.GetNextTableFieldID(nameof(fin_configurationpaymentcondition), "Ord");
            Code = XPOUtility.GetNextTableFieldID(nameof(fin_configurationpaymentcondition), "Code");
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
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        //ConfigurationPaymentCondition One <> Many DocumentFinanceMaster
        [Association(@"ConfigurationPaymentConditionReferencesDocumentFinanceMaster", typeof(fin_documentfinancemaster))]
        public XPCollection<fin_documentfinancemaster> DocumentMaster
        {
            get { return GetCollection<fin_documentfinancemaster>("DocumentMaster"); }
        }
    }
}