using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class SYS_ConfigurationWeighingMachine : XPGuidObject
    {
        public SYS_ConfigurationWeighingMachine() : base() { }
        public SYS_ConfigurationWeighingMachine(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(SYS_ConfigurationWeighingMachine), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(SYS_ConfigurationWeighingMachine), "Code");
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
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        //ConfigurationHardwareWeighingMachine One <> Many CConfigurationPlaceTerminal
        [Association(@"ConfigurationHardwareWeighingMachineReferencesConfigurationPlaceTerminal", typeof(POS_ConfigurationPlaceTerminal))]
        public XPCollection<POS_ConfigurationPlaceTerminal> Terminals
        {
            get { return GetCollection<POS_ConfigurationPlaceTerminal>("Terminals"); }
        }
    }
}
