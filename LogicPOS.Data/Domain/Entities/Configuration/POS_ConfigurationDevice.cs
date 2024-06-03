using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;
using System;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class pos_configurationdevice : Entity
    {
        public pos_configurationdevice() : base() { }
        public pos_configurationdevice(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(pos_configurationdevice), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(pos_configurationdevice), "Code");
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

        private string fType;
        public string Type
        {
            get { return fType; }
            set { SetPropertyValue<string>("Type", ref fType, value); }
        }

        private string fProperties;
        [Size(SizeAttribute.Unlimited)]
        public string Properties
        {
            get { return fProperties; }
            set { SetPropertyValue<string>("Properties", ref fProperties, value); }
        }

        //ConfigurationPlaceTerminal <> Many ConfigurationDevice
        private pos_configurationplaceterminal fPlaceTerminal;
        [Association(@"ConfigurationPlaceTerminalReferencesConfigurationDevice")]
        public pos_configurationplaceterminal PlaceTerminal
        {
            get { return fPlaceTerminal; }
            set { SetPropertyValue("Terminal", ref fPlaceTerminal, value); }
        }
    }
}