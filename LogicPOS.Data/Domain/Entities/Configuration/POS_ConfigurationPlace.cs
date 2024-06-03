using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;
using System;

namespace LogicPOS.Domain.Entities
{
    public enum OrderPrintMode
    {
        ByArticle, Grouped
    }

    [DeferredDeletion(false)]
    public class pos_configurationplace : Entity
    {
        public pos_configurationplace() : base() { }
        public pos_configurationplace(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(pos_configurationplace), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(pos_configurationplace), "Code");
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

        private string fButtonImage;
        [Size(255)]
        public string ButtonImage
        {
            get { return fButtonImage; }
            set { SetPropertyValue<string>("ButtonImage", ref fButtonImage, value); }
        }

        private string fTypeSubtotal;
        public string TypeSubtotal
        {
            get { return fTypeSubtotal; }
            set { SetPropertyValue<string>("TypeSubtotal", ref fTypeSubtotal, value); }
        }

        private string fAccountType;
        public string AccountType
        {
            get { return fAccountType; }
            set { SetPropertyValue<string>("AccountType", ref fAccountType, value); }
        }

        private OrderPrintMode fOrderPrintMode;
        public OrderPrintMode OrderPrintMode
        {
            get { return fOrderPrintMode; }
            set { SetPropertyValue("OrderPrintMode", ref fOrderPrintMode, value); }
        }

        //ConfigurationPlace One <> Many ConfigurationPlaceTerminal
        [Association(@"ConfigurationPlaceReferencesConfigurationPlaceTerminal", typeof(pos_configurationplaceterminal))]
        public XPCollection<pos_configurationplaceterminal> Terminal
        {
            get { return GetCollection<pos_configurationplaceterminal>("Terminal"); }
        }

        //ConfigurationPlace One <> Many ConfigurationPlaceTable
        [Association(@"ConfigurationPlaceReferencesConfigurationPlaceTable", typeof(pos_configurationplacetable))]
        public XPCollection<pos_configurationplacetable> PlaceTable
        {
            get { return GetCollection<pos_configurationplacetable>("PlaceTable"); }
        }

        //ConfigurationPriceType One <> Many ConfigurationPlace
        private fin_configurationpricetype fPriceType;
        [Association(@"ConfigurationPriceTypeReferencesConfigurationPlace")]
        public fin_configurationpricetype PriceType
        {
            get { return fPriceType; }
            set { SetPropertyValue("PriceType", ref fPriceType, value); }
        }

        //ConfigurationPlaceMovementType One <> Many ConfigurationPlace
        private pos_configurationplacemovementtype fMovementType;
        [Association(@"ConfigurationPlaceMovementTypeReferencesConfigurationPlace")]
        public pos_configurationplacemovementtype MovementType
        {
            get { return fMovementType; }
            set { SetPropertyValue("MovementType", ref fMovementType, value); }
        }
    }
}