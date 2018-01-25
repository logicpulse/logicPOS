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
    public class POS_ConfigurationPlace : XPGuidObject
    {
        public POS_ConfigurationPlace() : base() { }
        public POS_ConfigurationPlace(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("POS_ConfigurationPlace", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("POS_ConfigurationPlace", "Code");
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
        [Association(@"ConfigurationPlaceReferencesConfigurationPlaceTerminal", typeof(POS_ConfigurationPlaceTerminal))]
        public XPCollection<POS_ConfigurationPlaceTerminal> Terminal
        {
            get { return GetCollection<POS_ConfigurationPlaceTerminal>("Terminal"); }
        }

        //ConfigurationPlace One <> Many ConfigurationPlaceTable
        [Association(@"ConfigurationPlaceReferencesConfigurationPlaceTable", typeof(POS_ConfigurationPlaceTable))]
        public XPCollection<POS_ConfigurationPlaceTable> PlaceTable
        {
            get { return GetCollection<POS_ConfigurationPlaceTable>("PlaceTable"); }
        }

        //ConfigurationPriceType One <> Many ConfigurationPlace
        FIN_ConfigurationPriceType fPriceType;
        [Association(@"ConfigurationPriceTypeReferencesConfigurationPlace")]
        public FIN_ConfigurationPriceType PriceType
        {
            get { return fPriceType; }
            set { SetPropertyValue<FIN_ConfigurationPriceType>("PriceType", ref fPriceType, value); }
        }

        //ConfigurationPlaceMovementType One <> Many ConfigurationPlace
        POS_ConfigurationPlaceMovementType fMovementType;
        [Association(@"ConfigurationPlaceMovementTypeReferencesConfigurationPlace")]
        public POS_ConfigurationPlaceMovementType MovementType
        {
            get { return fMovementType; }
            set { SetPropertyValue<POS_ConfigurationPlaceMovementType>("MovementType", ref fMovementType, value); }
        }
    }
}