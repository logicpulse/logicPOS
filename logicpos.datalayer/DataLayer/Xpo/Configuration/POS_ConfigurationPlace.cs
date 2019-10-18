using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    public enum OrderPrintMode
    {
        ByArticle, Grouped
    }

    [DeferredDeletion(false)]
    public class pos_configurationplace : XPGuidObject
    {
        public pos_configurationplace() : base() { }
        public pos_configurationplace(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(pos_configurationplace), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(pos_configurationplace), "Code");
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

        string fButtonImage;
        [Size(255)]
        public string ButtonImage
        {
            get { return fButtonImage; }
            set { SetPropertyValue<string>("ButtonImage", ref fButtonImage, value); }
        }

        string fTypeSubtotal;
        public string TypeSubtotal
        {
            get { return fTypeSubtotal; }
            set { SetPropertyValue<string>("TypeSubtotal", ref fTypeSubtotal, value); }
        }

        string fAccountType;
        public string AccountType
        {
            get { return fAccountType; }
            set { SetPropertyValue<string>("AccountType", ref fAccountType, value); }
        }

        OrderPrintMode fOrderPrintMode;
        public OrderPrintMode OrderPrintMode
        {
            get { return fOrderPrintMode; }
            set { SetPropertyValue<OrderPrintMode>("OrderPrintMode", ref fOrderPrintMode, value); }
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
        fin_configurationpricetype fPriceType;
        [Association(@"ConfigurationPriceTypeReferencesConfigurationPlace")]
        public fin_configurationpricetype PriceType
        {
            get { return fPriceType; }
            set { SetPropertyValue<fin_configurationpricetype>("PriceType", ref fPriceType, value); }
        }

        //ConfigurationPlaceMovementType One <> Many ConfigurationPlace
        pos_configurationplacemovementtype fMovementType;
        [Association(@"ConfigurationPlaceMovementTypeReferencesConfigurationPlace")]
        public pos_configurationplacemovementtype MovementType
        {
            get { return fMovementType; }
            set { SetPropertyValue<pos_configurationplacemovementtype>("MovementType", ref fMovementType, value); }
        }
    }
}