using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class POS_ConfigurationDevice : XPGuidObject
    {
        public POS_ConfigurationDevice() : base() { }
        public POS_ConfigurationDevice(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(POS_ConfigurationDevice), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(POS_ConfigurationDevice), "Code");
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

        string fType;
        public string Type
        {
            get { return fType; }
            set { SetPropertyValue<string>("Type", ref fType, value); }
        }

        string fProperties;
        [Size(SizeAttribute.Unlimited)]
        public string Properties
        {
            get { return fProperties; }
            set { SetPropertyValue<string>("Properties", ref fProperties, value); }
        }

        //ConfigurationPlaceTerminal <> Many ConfigurationDevice
        POS_ConfigurationPlaceTerminal fPlaceTerminal;
        [Association(@"ConfigurationPlaceTerminalReferencesConfigurationDevice")]
        public POS_ConfigurationPlaceTerminal PlaceTerminal
        {
            get { return fPlaceTerminal; }
            set { SetPropertyValue<POS_ConfigurationPlaceTerminal>("Terminal", ref fPlaceTerminal, value); }
        }
    }
}